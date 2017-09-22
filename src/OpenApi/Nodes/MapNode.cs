

namespace Tavis.OpenApi
{
    using SharpYaml.Serialization;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Tavis.OpenApi.Model;

    /// <summary>
    /// Abstraction of a Map to isolate semantic parsing from details of 
    /// </summary>
    public class MapNode : ParseNode, IEnumerable<PropertyNode>
    {
        YamlMappingNode node;
        private List<PropertyNode> nodes;

        public static MapNode Create(string yaml)
        {
            
            var parsingContent = new ParsingContext();
            return new MapNode(parsingContent, (YamlMappingNode)YamlHelper.ParseYaml(yaml));
        }

        public MapNode(ParsingContext ctx, YamlMappingNode node) : base(ctx)
        {
            if (node == null) throw new DomainParseException($"Expected map");
            this.node = node;
            nodes = this.node.Children.Select(kvp => new PropertyNode(Context, kvp.Key.GetScalarValue(), kvp.Value)).ToList();
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
                    return new PropertyNode(this.Context, key, this.node.Children[new YamlScalarNode(key)]);
                } else
                {
                    return null;
                }
            }
        }
        public string GetScalarValue(ValueNode key)
        {
            var scalarNode = this.node.Children[new YamlScalarNode(key.GetScalarValue())] as YamlScalarNode;
            if (scalarNode == null) throw new DomainParseException($"Expected scalar at line {this.node.Start.Line} for key {key.GetScalarValue()}");

            return scalarNode.Value;
        }

        public T GetReferencedObject<T>(string refPointer) where T : IReference
        {
            return (T)this.Context.GetReferencedObject(refPointer); 
        }
        public T CreateOrReferenceDomainObject<T>(Func<T> factory) where T: IReference
        {
            T domainObject;
            var refPointer = GetReferencePointer(); // What should the DOM of a reference look like?
            // Duplicated object - poor perf/more memory/unsynchronized changes
            // Intermediate object - require common base class/client code has to explicitly code for it.
            // Delegating object - lot of setup work/maintenance/ require full interfaces
            // **current favourite***Shared object - marker to indicate its a reference/serialization code must serialize as reference everywhere except components.
            if (refPointer != null)
            {
                domainObject = (T)this.Context.GetReferencedObject(refPointer);
                
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
            if (yamlMap == null) throw new DomainParseException($"Expected map at line {yamlMap.Start.Line} while parsing {typeof(T).Name}");
            var nodes = yamlMap.Select(n => new { key = n.Key.GetScalarValue(), value = map(new MapNode(this.Context,n.Value as YamlMappingNode)) });
            return nodes.ToDictionary(k => k.key, v => v.value);
        }

        public override Dictionary<string, T> CreateMapWithReference<T>(string refpointerbase, Func<MapNode, T> map) 
        {

            var yamlMap = this.node;
            if (yamlMap == null) throw new DomainParseException($"Expected map at line {yamlMap.Start.Line} while parsing {typeof(T).Name}");
            var nodes = yamlMap.Select(n => new { key = n.Key.GetScalarValue(),
                value = this.GetReferencedObject<T>(refpointerbase + n.Key.GetScalarValue()) 
                ?? map(new MapNode(this.Context, (YamlMappingNode)n.Value))
            });
            return nodes.ToDictionary(k => k.key, v => v.value);
        }

        public override Dictionary<string, T> CreateSimpleMap<T>(Func<ValueNode, T> map)
        {
            var yamlMap = this.node;
            if (yamlMap == null) throw new DomainParseException($"Expected map at line {yamlMap.Start.Line} while parsing {typeof(T).Name}");
            var nodes = yamlMap.Select(n => new { key = n.Key.GetScalarValue(), value = map(new ValueNode(this.Context, (YamlScalarNode)n.Value)) });
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
