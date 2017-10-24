using SharpYaml.Schemas;
using SharpYaml.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.OpenApi.Readers.YamlReaders.ParseNodes
{
    /// <summary>
    /// Abstraction of a Map to isolate semantic parsing from details of 
    /// </summary>
    internal class MapNode : ParseNode, IEnumerable<PropertyNode>
    {
        YamlMappingNode node;
        private List<PropertyNode> nodes;

        public MapNode(ParsingContext context, OpenApiDiagnostic log, string yamlString) :
            this(context, log, (YamlMappingNode)YamlHelper.ParseYamlString(yamlString))
        {

        }

        public MapNode(ParsingContext context, OpenApiDiagnostic log, YamlMappingNode node) : base(context, log)
        {
            if (node == null)
            {
                throw new OpenApiException($"Expected map");
            }

            this.node = node;

            nodes = this.node.Children.Select(kvp => new PropertyNode(Context, Log, kvp.Key.GetScalarValue(), kvp.Value)).Cast<PropertyNode>().ToList();
        }

        public IEnumerator<PropertyNode> GetEnumerator()
        {
            return this.nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.nodes.GetEnumerator();
        }

        public PropertyNode this[string key]
        {
            get
            {
                YamlNode node = null;
                if (this.node.Children.TryGetValue(new YamlScalarNode(key), out node))
                {
                    return new PropertyNode(Context, Log, key, this.node.Children[new YamlScalarNode(key)]);
                } else
                {
                    return null;
                }
            }
        }

        public override string GetRaw()
        {
            var x = new Serializer(new SerializerSettings(new JsonSchema()) { EmitJsonComptible = true } );
            return x.Serialize(this.node);
            
        }

        public string GetScalarValue(ValueNode key)
        {
            var scalarNode = this.node.Children[new YamlScalarNode(key.GetScalarValue())] as YamlScalarNode;
            if (scalarNode == null) throw new OpenApiException($"Expected scalar at line {this.node.Start.Line} for key {key.GetScalarValue()}");

            return scalarNode.Value;
        }

        public T GetReferencedObject<T>(string refPointer) where T : IOpenApiReference
        {
            return (T)this.Context.GetReferencedObject(Log, refPointer); 
        }
        public T CreateOrReferenceDomainObject<T>(Func<T> factory) where T: IOpenApiReference
        {
            T domainObject;
            var refPointer = GetReferencePointer(); // What should the DOM of a reference look like?
            // Duplicated object - poor perf/more memory/unsynchronized changes
            // Intermediate object - require common base class/client code has to explicitly code for it.
            // Delegating object - lot of setup work/maintenance/ require full interfaces
            // **current favourite***Shared object - marker to indicate its a reference/serialization code must serialize as reference everywhere except components.
            if (refPointer != null)
            {
                domainObject = (T)this.Context.GetReferencedObject(Log, refPointer);
                
            }
            else
            {
                domainObject = factory();
            }

            return domainObject;
        }

        public override Dictionary<string, T> CreateMap<T>(Func<MapNode, T> map)
        {
            var yamlMap = this.node;
            if (yamlMap == null) throw new OpenApiException($"Expected map at line {yamlMap.Start.Line} while parsing {typeof(T).Name}");
            var nodes = yamlMap.Select(n => new { key = n.Key.GetScalarValue(), value = map(new MapNode(this.Context, this.Log, n.Value as YamlMappingNode)) });
            return nodes.ToDictionary(k => k.key, v => v.value);
        }

        public override Dictionary<string, T> CreateMapWithReference<T>(string refpointerbase, Func<MapNode, T> map) 
        {

            var yamlMap = this.node;
            if (yamlMap == null) throw new OpenApiException($"Expected map at line {yamlMap.Start.Line} while parsing {typeof(T).Name}");
            var nodes = yamlMap.Select(n => new { key = n.Key.GetScalarValue(),
                value = this.GetReferencedObject<T>(refpointerbase + n.Key.GetScalarValue()) 
                ?? map(new MapNode(this.Context, this.Log, (YamlMappingNode)n.Value))
            });
            return nodes.ToDictionary(k => k.key, v => v.value);
        }

        public override Dictionary<string, T> CreateSimpleMap<T>(Func<ValueNode, T> map)
        {
            var yamlMap = this.node;
            if (yamlMap == null) throw new OpenApiException($"Expected map at line {yamlMap.Start.Line} while parsing {typeof(T).Name}");
            var nodes = yamlMap.Select(n => new { key = n.Key.GetScalarValue(), value = map(new ValueNode(Context, Log, (YamlScalarNode)n.Value)) });
            return nodes.ToDictionary(k => k.key, v => v.value);
        }

        public string GetReferencePointer()
        {
            YamlNode refNode;

            if (!this.node.Children.TryGetValue(new YamlScalarNode("$ref"), out refNode))
            {
                return null;
            }
            return refNode.GetScalarValue();
        }
    }


}
