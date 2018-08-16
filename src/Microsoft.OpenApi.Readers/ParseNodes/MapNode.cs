// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Exceptions;
using SharpYaml.Schemas;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers.ParseNodes
{
    /// <summary>
    /// Abstraction of a Map to isolate semantic parsing from details of
    /// </summary>
    internal class MapNode : ParseNode, IEnumerable<PropertyNode>
    {
        private readonly YamlMappingNode _node;
        private readonly List<PropertyNode> _nodes;

        public MapNode(ParsingContext context, OpenApiDiagnostic diagnostic, string yamlString) :
            this(context, diagnostic, (YamlMappingNode)YamlHelper.ParseYamlString(yamlString))
        {
        }

        public MapNode(ParsingContext context, OpenApiDiagnostic diagnostic, YamlNode node) : base(
            context,
            diagnostic)
        {
            if (!(node is YamlMappingNode mapNode))
            {
                throw new OpenApiReaderException("Expected map.", node);
            }

            this._node = mapNode;

            _nodes = this._node.Children
                .Select(kvp => new PropertyNode(Context, Diagnostic, kvp.Key.GetScalarValue(), kvp.Value))
                .Cast<PropertyNode>()
                .ToList();
        }

        public PropertyNode this[string key]
        {
            get
            {
                YamlNode node = null;
                if (this._node.Children.TryGetValue(new YamlScalarNode(key), out node))
                {
                    return new PropertyNode(Context, Diagnostic, key, this._node.Children[new YamlScalarNode(key)]);
                }

                return null;
            }
        }

        public override Dictionary<string, T> CreateMap<T>(Func<MapNode, T> map)
        {
            var yamlMap = _node;
            if (yamlMap == null)
            {
                throw new OpenApiException($"Expected map at line {yamlMap.Start.Line} while parsing {typeof(T).Name}");
            }

            var nodes = yamlMap.Select(
                n => new
                {
                    key = n.Key.GetScalarValue(),
                    value = n.Value as YamlMappingNode == null
                        ? default(T)
                        : map(new MapNode(Context, Diagnostic, n.Value as YamlMappingNode))
                });

            return nodes.ToDictionary(k => k.key, v => v.value);
        }

         public override Dictionary<string, T> CreateMapWithReference<T>(
             ReferenceType referenceType,
             Func<MapNode, T> map)
         {
             var yamlMap = _node;
             if (yamlMap == null)
             {
                 throw new OpenApiException($"Expected map at line {yamlMap.Start.Line} while parsing {typeof(T).Name}");
             }

             var nodes = yamlMap.Select(
                 n => {
                     var entry = new
                     {
                         key = n.Key.GetScalarValue(),
                         value = map(new MapNode(Context, Diagnostic, (YamlMappingNode)n.Value))
                     };
                     if (entry.value == null)
                     {
                         return null;  // Body Parameters shouldn't be converted to Parameters
                     }
                     entry.value.Reference = new OpenApiReference()
                     {
                         Type = referenceType,
                         Id = entry.key
                     };
                     return entry;
                 }
                 );
             return nodes.Where(n => n!= null).ToDictionary(k => k.key, v => v.value);
         }

        public override Dictionary<string, T> CreateSimpleMap<T>(Func<ValueNode, T> map)
        {
            var yamlMap = _node;
            if (yamlMap == null)
            {
                throw new OpenApiException($"Expected map at line {yamlMap.Start.Line} while parsing {typeof(T).Name}");
            }

            var nodes = yamlMap.Select(
                n => new
                {
                    key = n.Key.GetScalarValue(),
                    value = map(new ValueNode(Context, Diagnostic, (YamlScalarNode)n.Value))
                });
            return nodes.ToDictionary(k => k.key, v => v.value);
        }

        public IEnumerator<PropertyNode> GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }

        public override string GetRaw()
        {
            var x = new Serializer(new SerializerSettings(new JsonSchema()) {EmitJsonComptible = true});
            return x.Serialize(_node);
        }

        public T GetReferencedObject<T>(ReferenceType referenceType, string referenceId)
            where T : IOpenApiReferenceable, new()
        {
            return new T()
                    {
                        UnresolvedReference = true,
                        Reference = Context.VersionService.ConvertToOpenApiReference(referenceId,referenceType)  
                    };
        }

        public string GetReferencePointer()
        {
            YamlNode refNode;

            if (!_node.Children.TryGetValue(new YamlScalarNode("$ref"), out refNode))
            {
                return null;
            }

            return refNode.GetScalarValue();
        }

        public string GetScalarValue(ValueNode key)
        {
            var scalarNode = _node.Children[new YamlScalarNode(key.GetScalarValue())] as YamlScalarNode;
            if (scalarNode == null)
            {
                throw new OpenApiException($"Expected scalar at line {_node.Start.Line} for key {key.GetScalarValue()}");
            }

            return scalarNode.Value;
        }

        /// <summary>
        /// Create a <see cref="OpenApiObject"/>
        /// </summary>
        /// <returns>The created Any object.</returns>
        public override IOpenApiAny CreateAny()
        {
            var apiObject = new OpenApiObject();
            foreach (var node in this)
            {
                apiObject.Add(node.Name, node.Value.CreateAny());
            }

            return apiObject;
        }
    }
}