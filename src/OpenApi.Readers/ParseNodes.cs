

namespace Microsoft.OpenApi.Readers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class FixedFieldMap<T> : Dictionary<string, Action<T, ParseNode>>
    {
    }

    public class PatternFieldMap<T> : Dictionary<Func<string, bool>, Action<T, string, ParseNode>>
    {
    }
    public abstract class ParseNode 
    {
        public ParseNode(ParsingContext ctx)
        {
            this.Context = ctx;
        }
        public ParsingContext Context { get; }
        public string DomainType { get; internal set; }

        public MapNode CheckMapNode(string nodeName)
        {
            var mapNode = this as MapNode;
            if (mapNode == null)
            {
                this.Context.ParseErrors.Add(new OpenApiError("", $"{nodeName} must be a map/object at " + this.Context.GetLocation() ));
            }

            return mapNode;
        }

        public virtual string GetScalarValue()
        {
            throw new DomainParseException("Cannot get scalar value");
        }

        public virtual Dictionary<string, T> CreateMap<T>(Func<MapNode, T> map)
        {
            throw new DomainParseException("Cannot create map");
        }
        public virtual Dictionary<string, T> CreateMapWithReference<T>(string refpointer, Func<MapNode, T> map) where T : class, IReference
        {
            throw new DomainParseException("Cannot create map from reference");
        }
        public virtual Dictionary<string, T> CreateSimpleMap<T>(Func<ValueNode, T> map)
        {
            throw new DomainParseException("Cannot create simple map");
        }
        public virtual List<T> CreateList<T>(Func<MapNode, T> map)
        {
            throw new DomainParseException("Cannot create list");

        }
        public virtual List<T> CreateSimpleList<T>(Func<ValueNode, T> map)
        {
            throw new DomainParseException("Cannot create simple list");
        }

        internal string CheckRegex(string value, Regex versionRegex, string defaultValue)
        {
            if (!versionRegex.IsMatch(value))
            {
                this.Context.ParseErrors.Add(new OpenApiError("", "Value does not match regex: " + versionRegex.ToString()));
                return defaultValue;
            }
            return value;
        }
    }
    
}
