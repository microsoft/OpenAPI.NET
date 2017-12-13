﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Exceptions;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers.ParseNodes
{
    internal class ListNode : ParseNode, IEnumerable<ParseNode>
    {
        private readonly YamlSequenceNode _nodeList;

        public ListNode(ParsingContext context, OpenApiDiagnostic diagnostic, YamlSequenceNode sequenceNode) : base(
            context,
            diagnostic)
        {
            _nodeList = sequenceNode;
        }

        public override List<T> CreateList<T>(Func<MapNode, T> map)
        {
            var yamlSequence = _nodeList;
            if (yamlSequence == null)
            {
                throw new OpenApiException(
                    $"Expected list at line {_nodeList.Start.Line} while parsing {typeof(T).Name}");
            }

            return yamlSequence.Select(n => map(new MapNode(Context, Diagnostic, n as YamlMappingNode)))
                .Where(i => i != null)
                .ToList();
        }

        public override List<T> CreateSimpleList<T>(Func<ValueNode, T> map)
        {
            var yamlSequence = _nodeList;
            if (yamlSequence == null)
            {
                throw new OpenApiException(
                    $"Expected list at line {_nodeList.Start.Line} while parsing {typeof(T).Name}");
            }

            return yamlSequence.Select(n => map(new ValueNode(Context, Diagnostic, (YamlScalarNode)n))).ToList();
        }

        public IEnumerator<ParseNode> GetEnumerator()
        {
            return _nodeList.Select(n => Create(Context, Diagnostic, n)).ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Create a <see cref="OpenApiArray"/>
        /// </summary>
        /// <returns>The created Any object.</returns>
        public override IOpenApiAny CreateAny()
        {
            var array = new OpenApiArray();
            foreach (var node in this)
            {
                array.Add(node.CreateAny());
            }

            return array;
        }
    }
}