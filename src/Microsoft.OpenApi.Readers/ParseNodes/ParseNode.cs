// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Exceptions;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers.ParseNodes
{
    internal abstract class ParseNode
    {
        protected ParseNode(ParsingContext parsingContext)
        {
            Context = parsingContext;
            Context?.CountNode();
        }

        public ParsingContext Context { get; }

        public MapNode CheckMapNode(string nodeName)
        {
            if (this is not MapNode mapNode)
            {
                throw new OpenApiReaderException($"{nodeName} must be a map/object", Context);
            }

            return mapNode;
        }

        public static ParseNode Create(ParsingContext context, YamlNode node)
        {
            if (node is YamlSequenceNode listNode)
            {
                return new ListNode(context, listNode);
            }

            if (node is YamlMappingNode mapNode)
            {
                return new MapNode(context, mapNode);
            }

            return new ValueNode(context, node as YamlScalarNode);
        }

        public virtual List<T> CreateList<T>(Func<MapNode, T> map)
        {
            throw new OpenApiReaderException("Cannot create list from this type of node.", Context);
        }

        public virtual Dictionary<string, T> CreateMap<T>(Func<MapNode, T> map)
        {
            throw new OpenApiReaderException("Cannot create map from this type of node.", Context);
        }

        public virtual Dictionary<string, T> CreateMapWithReference<T>(
            ReferenceType referenceType,
            Func<MapNode, T> map)
            where T : class, IOpenApiReferenceable
        {
            throw new OpenApiReaderException("Cannot create map from this reference.", Context);
        }

        public virtual List<T> CreateSimpleList<T>(Func<ValueNode, T> map)
        {
            throw new OpenApiReaderException("Cannot create simple list from this type of node.", Context);
        }

        public virtual Dictionary<string, T> CreateSimpleMap<T>(Func<ValueNode, T> map)
        {
            throw new OpenApiReaderException("Cannot create simple map from this type of node.", Context);
        }

        public IOpenApiAny CreateAny()
        {
            return CreateAny(0);
        }

        /// <summary>
        /// Materializes the node, and everything below it, into an <see cref="IOpenApiAny"/>.
        /// </summary>
        /// <param name="depth">Nesting depth of the current node, bounded by <see cref="OpenApiReaderSettings.MaxDepth"/>.</param>
        internal virtual IOpenApiAny CreateAny(uint depth)
        {
            throw new OpenApiReaderException("Cannot create an Any object this type of node.", Context);
        }

        /// <summary>
        /// Fails fast when the node graph is nested more deeply than the reader supports,
        /// protecting the recursive readers from stack exhaustion.
        /// </summary>
        protected void EnsureDepthWithinLimit(uint depth)
        {
            var maxDepth = Context?.MaxDepth ?? OpenApiReaderSettings.DefaultMaxDepth;
            if (depth > maxDepth)
            {
                throw new OpenApiReaderException($"The document exceeds the maximum supported nesting depth of {maxDepth}.", Context);
            }
        }

        public virtual string GetRaw()
        {
            throw new OpenApiReaderException("Cannot get raw value from this type of node.", Context);
        }

        public virtual string GetScalarValue()
        {
            throw new OpenApiReaderException("Cannot create a scalar value from this type of node.", Context);
        }

        public virtual List<IOpenApiAny> CreateListOfAny()
        {
            throw new OpenApiReaderException("Cannot create a list from this type of node.", Context);
        }
    }
}
