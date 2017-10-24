using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers.YamlReaders.ParseNodes
{
    internal abstract class ParseNode 
    {
        public OpenApiDiagnostic Log { get; }

        protected ParseNode(ParsingContext parsingContext, OpenApiDiagnostic log)
        {
            this.Context = parsingContext;
            this.Log = log;
        }

        public ParsingContext Context { get; }

        public string DomainType { get; internal set; }

        public MapNode CheckMapNode(string nodeName)
        {
            var mapNode = this as MapNode;
            if (mapNode == null)
            {
                this.Log.Errors.Add(new OpenApiError("", $"{nodeName} must be a map/object at " + this.Context.GetLocation() ));
            }

            return mapNode;
        }

        public static ParseNode Create(ParsingContext context, OpenApiDiagnostic log, YamlNode node)
        {
            var listNode = node as YamlSequenceNode;

            if (listNode != null)
            {
                return new ListNode(context, log, listNode);
            }

            var mapNode = node as YamlMappingNode;
            if (mapNode != null)
            {
                return new MapNode(context, log, mapNode);
            }

            return new ValueNode(context, log, node as YamlScalarNode);
        }

        public virtual string GetRaw()
        {
            throw new OpenApiException("Cannot get raw value");
        }
        
        public virtual string GetScalarValue()
        {
            throw new OpenApiException("Cannot get scalar value");
        }

        public virtual Dictionary<string, T> CreateMap<T>(Func<MapNode, T> map)
        {
            throw new OpenApiException("Cannot create map");
        }

        public virtual Dictionary<string, T> CreateMapWithReference<T>(string refpointer, Func<MapNode, T> map) where T : class, IOpenApiReference
        {
            throw new OpenApiException("Cannot create map from reference");
        }

        public virtual Dictionary<string, T> CreateSimpleMap<T>(Func<ValueNode, T> map)
        {
            throw new OpenApiException("Cannot create simple map");
        }

        public virtual List<T> CreateList<T>(Func<MapNode, T> map)
        {
            throw new OpenApiException("Cannot create list");

        }
        public virtual List<T> CreateSimpleList<T>(Func<ValueNode, T> map)
        {
            throw new OpenApiException("Cannot create simple list");
        }

        internal string CheckRegex(string value, Regex versionRegex, string defaultValue)
        {
            if (!versionRegex.IsMatch(value))
            {
                this.Log.Errors.Add(new OpenApiError("", "Value does not match regex: " + versionRegex.ToString()));
                return defaultValue;
            }
            return value;
        }
    }
    
}
