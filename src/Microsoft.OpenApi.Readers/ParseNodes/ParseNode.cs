﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Exceptions;
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
        }

        public ParsingContext Context { get; }

        public MapNode CheckMapNode(string nodeName)
        {
            if (!(this is MapNode mapNode))
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

        public virtual IOpenApiAny CreateAny()
        {
            throw new OpenApiReaderException("Cannot create an Any object this type of node.", Context);
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
