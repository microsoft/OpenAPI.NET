// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.OpenApi.Readers.Exceptions;
using SharpYaml;
using SharpYaml.Events;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers
{
    /// <summary>
    /// Iteratively composes the first YAML document under configured resource limits.
    /// </summary>
    internal sealed class BoundedYamlDocumentParser
    {
        private const int LookAheadBufferCapacity = 8;

        private readonly YamlResourceBudget _budget;
        private readonly Dictionary<string, MaterializedNode> _anchors = new(StringComparer.Ordinal);
        private readonly HashSet<string> _activeAnchors = new(StringComparer.Ordinal);
        private readonly Stack<ContainerFrame> _containers = new();
        private readonly uint _maxScalarLength;
        private readonly uint _maxDepth;
        private YamlNode _root;

        public BoundedYamlDocumentParser(OpenApiReaderSettings settings)
            : this(
                settings.MaxDepth,
                settings.MaxNodeCount,
                settings.MaxAliasExpansionNodeCount,
                settings.MaxScalarLength)
        {
        }

        public BoundedYamlDocumentParser(ParsingContext context)
            : this(
                context.MaxDepth,
                context.MaxNodeCount,
                context.MaxAliasExpansionNodeCount,
                context.MaxScalarLength)
        {
        }

        private BoundedYamlDocumentParser(
            uint maxDepth,
            uint maxNodeCount,
            uint maxAliasExpansionNodeCount,
            uint maxScalarLength)
        {
            _budget = new(
                maxDepth,
                maxNodeCount,
                maxAliasExpansionNodeCount);
            _maxScalarLength = maxScalarLength;
            _maxDepth = maxDepth;
        }

        public YamlDocument Parse(
            TextReader input,
            uint maxInputByteCount,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var boundedReader = new InputLimitTextReader(input, maxInputByteCount);
            return Parse(boundedReader, cancellationToken);
        }

        private YamlDocument Parse(InputLimitTextReader input, CancellationToken cancellationToken)
        {
            var parser = new SharpYaml.Parser<SharpYaml.LookAheadBuffer>(
                new SharpYaml.LookAheadBuffer(new CancellationTokenTextReader(input, cancellationToken), LookAheadBufferCapacity),
                (int)_maxDepth);
            var documentStarted = false;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!parser.MoveNext())
                {
                    break;
                }

                switch (parser.Current)
                {
                    case StreamStart:
                        break;
                    case DocumentStart:
                        documentStarted = true;
                        break;
                    case MappingStart mappingStart:
                        var mapping = new YamlMappingNode
                        {
                            Anchor = mappingStart.Anchor,
                            Tag = mappingStart.Tag,
                            Style = mappingStart.Style
                        };
                        YamlNodeLocationRegistry.Register(mapping, mappingStart.Start.Line);
                        StartContainer(mapping, mappingStart.Anchor);
                        break;
                    case SequenceStart sequenceStart:
                        var sequence = new YamlSequenceNode
                        {
                            Anchor = sequenceStart.Anchor,
                            Tag = sequenceStart.Tag,
                            Style = sequenceStart.Style
                        };
                        YamlNodeLocationRegistry.Register(sequence, sequenceStart.Start.Line);
                        StartContainer(sequence, sequenceStart.Anchor);
                        break;
                    case Scalar scalar:
                        AddScalar(scalar);
                        break;
                    case AnchorAlias alias:
                        AddAlias(alias);
                        break;
                    case MappingEnd:
                    case SequenceEnd:
                        EndContainer();
                        break;
                    case DocumentEnd:
                        return CreateDocument();
                    case StreamEnd:
                        if (documentStarted)
                        {
                            return CreateDocument();
                        }

                        throw new OpenApiReaderException("No documents found in the YAML stream.");
                    default:
                        throw new OpenApiReaderException(
                            $"Unsupported YAML parser event '{parser.Current?.GetType().Name ?? "<null>"}'.");
                }
            }

            throw new OpenApiReaderException("No documents found in the YAML stream.");
        }

        private void StartContainer(YamlNode container, string anchor)
        {
            _budget.EnterNode((uint)_containers.Count);
            RegisterActiveAnchor(anchor);
            _containers.Push(new(container, anchor));
        }

        private void AddScalar(Scalar scalar)
        {
            if (scalar.Value != null && scalar.Value.Length > _maxScalarLength)
            {
                throw new OpenApiReaderException(
                    $"The YAML scalar exceeds the maximum supported length of {_maxScalarLength} characters.");
            }

            _budget.EnterNode((uint)_containers.Count);
            var node = new YamlScalarNode(scalar.Value)
            {
                Anchor = scalar.Anchor,
                Tag = scalar.Tag,
                Style = scalar.Style
            };
            YamlNodeLocationRegistry.Register(node, scalar.Start.Line);
            var materialized = new MaterializedNode(node, 1, 1, scalar.Value);
            RegisterCompletedAnchor(scalar.Anchor, materialized);
            AddNode(materialized);
        }

        private void AddAlias(AnchorAlias alias)
        {
            if (_activeAnchors.Contains(alias.Value))
            {
                throw new OpenApiReaderException($"The YAML alias '*{alias.Value}' forms a cycle.");
            }

            if (!_anchors.TryGetValue(alias.Value, out var anchor))
            {
                throw new OpenApiReaderException(
                    $"The YAML alias '*{alias.Value}' refers to an unknown anchor.");
            }

            _budget.EnterAlias((uint)_containers.Count, anchor.NodeCount, anchor.Height);
            AddNode(anchor);
        }

        private void EndContainer()
        {
            if (_containers.Count == 0)
            {
                throw new OpenApiReaderException(
                    "The YAML document contains an unexpected container terminator.");
            }

            var frame = _containers.Pop();
            if (frame.PendingKey != null)
            {
                throw new OpenApiReaderException("The YAML mapping contains a key without a value.");
            }

            var materialized = new MaterializedNode(
                frame.Container,
                frame.NodeCount,
                frame.MaxChildHeight + 1,
                null);
            if (!string.IsNullOrEmpty(frame.Anchor))
            {
                _activeAnchors.Remove(frame.Anchor);
                _anchors.Add(frame.Anchor, materialized);
            }

            AddNode(materialized);
        }

        private void AddNode(MaterializedNode materialized)
        {
            if (_containers.Count == 0)
            {
                if (_root != null)
                {
                    throw new OpenApiReaderException(
                        "The YAML document contains more than one root node.");
                }

                _root = materialized.Node;
                return;
            }

            var frame = _containers.Peek();
            if (frame.Container is YamlSequenceNode sequence)
            {
                sequence.Add(materialized.Node);
                frame.NodeCount = checked(frame.NodeCount + materialized.NodeCount);
                frame.MaxChildHeight = Math.Max(frame.MaxChildHeight, materialized.Height);
                return;
            }

            var mapping = (YamlMappingNode)frame.Container;
            if (frame.PendingKey == null)
            {
                if (materialized.MappingKey == null || materialized.Node is not YamlScalarNode scalarKey)
                {
                    throw new OpenApiReaderException("YAML mapping keys must be scalar values.");
                }

                frame.PendingKey = scalarKey;
                return;
            }

            var key = frame.PendingKey.Value;
            if (!frame.MappingKeys.Add(key))
            {
                throw new OpenApiReaderException(
                    $"The YAML mapping contains the duplicate key '{key}'.");
            }

            mapping.Add(frame.PendingKey, materialized.Node);
            frame.PendingKey = null;
            frame.NodeCount = checked(frame.NodeCount + materialized.NodeCount);
            frame.MaxChildHeight = Math.Max(frame.MaxChildHeight, materialized.Height);
        }

        private void RegisterActiveAnchor(string anchor)
        {
            if (string.IsNullOrEmpty(anchor))
            {
                return;
            }

            if (_anchors.ContainsKey(anchor) || !_activeAnchors.Add(anchor))
            {
                throw new OpenApiReaderException(
                    $"The YAML document contains the duplicate anchor '&{anchor}'.");
            }
        }

        private void RegisterCompletedAnchor(string anchor, MaterializedNode materialized)
        {
            if (string.IsNullOrEmpty(anchor))
            {
                return;
            }

            if (_anchors.ContainsKey(anchor) || _activeAnchors.Contains(anchor))
            {
                throw new OpenApiReaderException(
                    $"The YAML document contains the duplicate anchor '&{anchor}'.");
            }

            _anchors.Add(anchor, materialized);
        }

        private YamlDocument CreateDocument()
        {
            return _root != null
                ? new YamlDocument(_root)
                : throw new OpenApiReaderException("No content found in the YAML document.");
        }

        private sealed class ContainerFrame
        {
            public ContainerFrame(YamlNode container, string anchor)
            {
                Container = container;
                Anchor = anchor;
            }

            public YamlNode Container { get; }
            public string Anchor { get; }
            public YamlScalarNode PendingKey { get; set; }
            public HashSet<string> MappingKeys { get; } = new(StringComparer.Ordinal);
            public uint NodeCount { get; set; } = 1;
            public uint MaxChildHeight { get; set; }
        }

        private sealed class MaterializedNode
        {
            public MaterializedNode(
                YamlNode node,
                uint nodeCount,
                uint height,
                string mappingKey)
            {
                Node = node;
                NodeCount = nodeCount;
                Height = height;
                MappingKey = mappingKey;
            }

            public YamlNode Node { get; }
            public uint NodeCount { get; }
            public uint Height { get; }
            public string MappingKey { get; }
        }

        private sealed class CancellationTokenTextReader : TextReader
        {
            private readonly TextReader _innerReader;
            private readonly CancellationToken _cancellationToken;

            public CancellationTokenTextReader(
                TextReader innerReader,
                CancellationToken cancellationToken)
            {
                _innerReader = innerReader;
                _cancellationToken = cancellationToken;
            }

            public override int Peek()
            {
                _cancellationToken.ThrowIfCancellationRequested();
                return _innerReader.Peek();
            }

            public override int Read()
            {
                _cancellationToken.ThrowIfCancellationRequested();
                return _innerReader.Read();
            }

            public override int Read(char[] buffer, int index, int count)
            {
                _cancellationToken.ThrowIfCancellationRequested();
                return _innerReader.Read(buffer, index, count);
            }
        }
    }
}
