using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading;
using SharpYaml;
using SharpYaml.Events;

namespace Microsoft.OpenApi.YamlReader;

/// <summary>
/// Iteratively materializes the first YAML document from parser events under resource limits.
/// </summary>
internal sealed class YamlJsonParser
{
    private const int LookAheadBufferCapacity = 8;

    private readonly YamlConversionBudget _budget;
    private readonly Dictionary<string, MaterializedNode> _anchors = new(StringComparer.Ordinal);
    private readonly HashSet<string> _activeAnchors = new(StringComparer.Ordinal);
    private readonly Stack<ContainerFrame> _containers = new();
    private readonly uint _maxScalarLength;
    private JsonNode? _root;

    public YamlJsonParser(OpenApiYamlReaderSettings settings)
    {
        _budget = new(settings.MaxDepth, settings.MaxNodeCount, settings.MaxAliasExpansionNodeCount);
        _maxScalarLength = settings.MaxScalarLength;
    }

    public JsonNode Parse(TextReader input, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var cancellationReader = new CancellationTokenTextReader(input, cancellationToken);
        var parser = new Parser<LookAheadBuffer>(new LookAheadBuffer(cancellationReader, LookAheadBufferCapacity));
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
                    StartContainer(new JsonObject(), mappingStart.Anchor);
                    break;
                case SequenceStart sequenceStart:
                    StartContainer(new JsonArray(), sequenceStart.Anchor);
                    break;
                case Scalar scalar:
                    AddScalar(scalar, cancellationToken);
                    break;
                case AnchorAlias alias:
                    AddAlias(alias, cancellationToken);
                    break;
                case MappingEnd:
                case SequenceEnd:
                    EndContainer();
                    break;
                case DocumentEnd:
                    return _root ?? throw new OpenApiReaderException("No content found in the YAML document.");
                case StreamEnd:
                    if (documentStarted)
                    {
                        return _root ?? throw new OpenApiReaderException("No content found in the YAML document.");
                    }

                    throw new OpenApiReaderException("No documents found in the YAML stream.");
                default:
                    throw new OpenApiReaderException(
                        $"Unsupported YAML parser event '{parser.Current?.GetType().Name ?? "<null>"}'.");
            }
        }

        throw new OpenApiReaderException("No documents found in the YAML stream.");
    }

    private void StartContainer(JsonNode container, string? anchor)
    {
        _budget.EnterNode((uint)_containers.Count);
        RegisterActiveAnchor(anchor);
        _containers.Push(new(container, anchor));
    }

    private void AddScalar(Scalar scalar, CancellationToken cancellationToken)
    {
        if (scalar.Value is { } value && value.Length > _maxScalarLength)
        {
            throw new OpenApiReaderException(
                $"The YAML scalar exceeds the maximum supported length of {_maxScalarLength} characters.");
        }

        _budget.EnterNode((uint)_containers.Count);
        cancellationToken.ThrowIfCancellationRequested();
        var materialized = new MaterializedNode(
            YamlConverter.ToJsonValue(scalar.Value, scalar.Style),
            1,
            scalar.Value);

        RegisterCompletedAnchor(scalar.Anchor, materialized);
        AddNode(materialized);
    }

    private void AddAlias(AnchorAlias alias, CancellationToken cancellationToken)
    {
        if (_activeAnchors.Contains(alias.Value))
        {
            throw new OpenApiReaderException($"The YAML alias '*{alias.Value}' forms a cycle.");
        }

        if (!_anchors.TryGetValue(alias.Value, out var anchor))
        {
            throw new OpenApiReaderException($"The YAML alias '*{alias.Value}' refers to an unknown anchor.");
        }

        _budget.EnterAlias((uint)_containers.Count, anchor.NodeCount);
        cancellationToken.ThrowIfCancellationRequested();
        AddNode(new(anchor.Node.DeepClone(), anchor.NodeCount, anchor.MappingKey));
    }

    private void EndContainer()
    {
        if (_containers.Count == 0)
        {
            throw new OpenApiReaderException("The YAML document contains an unexpected container terminator.");
        }

        var frame = _containers.Pop();
        if (frame.PendingKey is not null)
        {
            throw new OpenApiReaderException("The YAML mapping contains a key without a value.");
        }

        var materialized = new MaterializedNode(frame.Container, frame.NodeCount, null);
        if (frame.Anchor is not null)
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
            if (_root is not null)
            {
                throw new OpenApiReaderException("The YAML document contains more than one root node.");
            }

            _root = materialized.Node;
            return;
        }

        var frame = _containers.Peek();
        switch (frame.Container)
        {
            case JsonArray array:
                array.Add(materialized.Node);
                frame.NodeCount = checked(frame.NodeCount + materialized.NodeCount);
                break;
            case JsonObject map when frame.PendingKey is null:
                frame.PendingKey = materialized.MappingKey
                    ?? throw new OpenApiReaderException("YAML mapping keys must be scalar values.");
                break;
            case JsonObject map:
                if (map.ContainsKey(frame.PendingKey))
                {
                    throw new OpenApiReaderException($"The YAML mapping contains the duplicate key '{frame.PendingKey}'.");
                }

                map.Add(frame.PendingKey, materialized.Node);
                frame.PendingKey = null;
                frame.NodeCount = checked(frame.NodeCount + materialized.NodeCount);
                break;
        }
    }

    private void RegisterActiveAnchor(string? anchor)
    {
        if (anchor is null || anchor.Length == 0)
        {
            return;
        }

        if (_anchors.ContainsKey(anchor) || !_activeAnchors.Add(anchor))
        {
            throw new OpenApiReaderException($"The YAML document contains the duplicate anchor '&{anchor}'.");
        }
    }

    private void RegisterCompletedAnchor(string? anchor, MaterializedNode materialized)
    {
        if (anchor is null || anchor.Length == 0)
        {
            return;
        }

        if (_anchors.ContainsKey(anchor) || _activeAnchors.Contains(anchor))
        {
            throw new OpenApiReaderException($"The YAML document contains the duplicate anchor '&{anchor}'.");
        }

        _anchors.Add(anchor, materialized);
    }

    private sealed class ContainerFrame(JsonNode container, string? anchor)
    {
        public JsonNode Container { get; } = container;
        public string? Anchor { get; } = anchor;
        public string? PendingKey { get; set; }
        public uint NodeCount { get; set; } = 1;
    }

    private sealed class MaterializedNode
    {
        public MaterializedNode(JsonNode node, uint nodeCount, string? mappingKey)
        {
            Node = node;
            NodeCount = nodeCount;
            MappingKey = mappingKey;
        }

        public JsonNode Node { get; }
        public uint NodeCount { get; }
        public string? MappingKey { get; }
    }

    private sealed class CancellationTokenTextReader(TextReader innerReader, CancellationToken cancellationToken) : TextReader
    {
        public override int Peek()
        {
            cancellationToken.ThrowIfCancellationRequested();
            return innerReader.Peek();
        }

        public override int Read()
        {
            cancellationToken.ThrowIfCancellationRequested();
            return innerReader.Read();
        }

        public override int Read(char[] buffer, int index, int count)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return innerReader.Read(buffer, index, count);
        }

        public override string? ReadLine()
        {
            cancellationToken.ThrowIfCancellationRequested();
            return base.ReadLine();
        }

        public override string ReadToEnd()
        {
            cancellationToken.ThrowIfCancellationRequested();
            return base.ReadToEnd();
        }
    }
}
