// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SharpYaml.Schemas;
using SharpYaml.Serialization;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Readers.YamlReaders.ParseNodes
{
    /// <summary>
    /// Abstraction of a Map to isolate semantic parsing from details of
    /// </summary>
    internal class MapNode : ParseNode, IEnumerable<PropertyNode>
    {
        private readonly YamlMappingNode node;
        private readonly List<PropertyNode> nodes;

        public MapNode(ParsingContext context, OpenApiDiagnostic diagnostic, string yamlString) :
            this(context, diagnostic, (YamlMappingNode)YamlHelper.ParseYamlString(yamlString))
        {
        }

        public MapNode(ParsingContext context, OpenApiDiagnostic diagnostic, YamlMappingNode node) : base(context, diagnostic)
        {
            if (node == null)
            {
                throw new OpenApiException($"Expected map");
            }

            this.node = node;

            nodes = this.node.Children
                .Select(kvp => new PropertyNode(Context, Diagnostic, kvp.Key.GetScalarValue(), kvp.Value))
                .Cast<PropertyNode>()
                .ToList();
        }

        public PropertyNode this[string key]
        {
            get
            {
                YamlNode node = null;
                if (this.node.Children.TryGetValue(new YamlScalarNode(key), out node))
                {
                    return new PropertyNode(Context, Diagnostic, key, this.node.Children[new YamlScalarNode(key)]);
                }

                return null;
            }
        }

        public override Dictionary<string, T> CreateMap<T>(Func<MapNode, T> map)
        {
            var yamlMap = node;
            if (yamlMap == null)
            {
                throw new OpenApiException($"Expected map at line {yamlMap.Start.Line} while parsing {typeof(T).Name}");
            }

            var nodes = yamlMap.Select(
                n => new
                {
                    key = n.Key.GetScalarValue(),
                    value = map(new MapNode(Context, Diagnostic, n.Value as YamlMappingNode))
                });
            return nodes.ToDictionary(k => k.key, v => v.value);
        }

        public override Dictionary<string, T> CreateMapWithReference<T>(string refpointerbase, Func<MapNode, T> map)
        {
            var yamlMap = node;
            if (yamlMap == null)
            {
                throw new OpenApiException($"Expected map at line {yamlMap.Start.Line} while parsing {typeof(T).Name}");
            }

            var nodes = yamlMap.Select(
                n => new
                {
                    key = n.Key.GetScalarValue(),
                    value = GetReferencedObject<T>(refpointerbase + n.Key.GetScalarValue()) ??
                    map(new MapNode(Context, Diagnostic, (YamlMappingNode)n.Value))
                });
            return nodes.ToDictionary(k => k.key, v => v.value);
        }

        public T CreateOrReferenceDomainObject<T>(Func<T> factory) where T : IOpenApiReference
        {
            T domainObject;
            var refPointer = GetReferencePointer(); // What should the DOM of a reference look like?
            // Duplicated object - poor perf/more memory/unsynchronized changes
            // Intermediate object - require common base class/client code has to explicitly code for it.
            // Delegating object - lot of setup work/maintenance/ require full interfaces
            // **current favourite***Shared object - marker to indicate its a reference/serialization code must serialize as reference everywhere except components.
            if (refPointer != null)
            {
                domainObject = (T)Context.GetReferencedObject(Diagnostic, refPointer);
            }
            else
            {
                domainObject = factory();
            }

            return domainObject;
        }

        public override Dictionary<string, T> CreateSimpleMap<T>(Func<ValueNode, T> map)
        {
            var yamlMap = node;
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
            return nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return nodes.GetEnumerator();
        }

        public override string GetRaw()
        {
            var x = new Serializer(new SerializerSettings(new JsonSchema()) {EmitJsonComptible = true});
            return x.Serialize(node);
        }

        public T GetReferencedObject<T>(string refPointer) where T : IOpenApiReference
        {
            return (T)Context.GetReferencedObject(Diagnostic, refPointer);
        }

        public string GetReferencePointer()
        {
            YamlNode refNode;

            if (!node.Children.TryGetValue(new YamlScalarNode("$ref"), out refNode))
            {
                return null;
            }

            return refNode.GetScalarValue();
        }

        public string GetScalarValue(ValueNode key)
        {
            var scalarNode = node.Children[new YamlScalarNode(key.GetScalarValue())] as YamlScalarNode;
            if (scalarNode == null)
            {
                throw new OpenApiException($"Expected scalar at line {node.Start.Line} for key {key.GetScalarValue()}");
            }

            return scalarNode.Value;
        }
    }
}