

namespace Microsoft.OpenApi.Readers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.OpenApi.Writers;
    using Microsoft.OpenApi.Services;

    public class FixedFieldMap<T> : Dictionary<string, Action<T, IParseNode>>
    {
    }

    public class PatternFieldMap<T> : Dictionary<Func<string, bool>, Action<T, string, IParseNode>>
    {
    }
    public abstract class ParseNode : IParseNode
    {
        public ParseNode(ParsingContext ctx)
        {
            this.Context = ctx;
        }
        public ParsingContext Context { get; }
        public string DomainType { get; internal set; }

        public IMapNode CheckMapNode(string nodeName)
        {
            var mapNode = this as IMapNode;
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

        public virtual Dictionary<string, T> CreateMap<T>(Func<IMapNode, T> map)
        {
            throw new DomainParseException("Cannot create map");
        }
        public virtual Dictionary<string, T> CreateMapWithReference<T>(string refpointer, Func<IMapNode, T> map) where T : class, IReference
        {
            throw new DomainParseException("Cannot create map from reference");
        }
        public virtual Dictionary<string, T> CreateSimpleMap<T>(Func<IValueNode, T> map)
        {
            throw new DomainParseException("Cannot create simple map");
        }
        public virtual List<T> CreateList<T>(Func<IMapNode, T> map)
        {
            throw new DomainParseException("Cannot create list");

        }
        public virtual List<T> CreateSimpleList<T>(Func<IValueNode, T> map)
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

        public static void WriteAnyNode(IParseNodeWriter writer, IAnyNode node)
        {
            node.Write(writer);
        }

    }


    public interface IParseNode
    {
        ParsingContext Context { get; }
        IMapNode CheckMapNode(string nodeName);

        string GetScalarValue();

        Dictionary<string, T> CreateMap<T>(Func<IMapNode, T> map);
        Dictionary<string, T> CreateMapWithReference<T>(string refpointer, Func<IMapNode, T> map) where T : class, IReference;

        Dictionary<string, T> CreateSimpleMap<T>(Func<IValueNode, T> map);

        List<T> CreateList<T>(Func<IMapNode, T> map);
        List<T> CreateSimpleList<T>(Func<IValueNode, T> map);

    }


    public interface IMapNode : IParseNode,IEnumerable<IPropertyNode>
    {
        string GetReferencePointer();
    }
    public interface IPropertyNode : IParseNode {
        string Name { get; set; }
        IParseNode Value { get; set; }

    }

    public interface IValueNode : IParseNode {
    }

    public interface IListNode { }
    public interface IAnyNode {
        void Write(IParseNodeWriter writer);
    }

    public interface IRootNode {
        IParseNode Find(JsonPointer refPointer);
    }

    public static class ParseNodeExtensions {

        public static T GetReferencedObject<T>(this IMapNode node, string refPointer) where T : IReference
        {
            return (T)node.Context.GetReferencedObject(refPointer);
        }
        public static T CreateOrReferenceDomainObject<T>(this IMapNode node, Func<T> factory) where T : IReference
        {
            T domainObject;
            var refPointer = node.GetReferencePointer(); // What should the DOM of a reference look like?
            // Duplicated object - poor perf/more memory/unsynchronized changes
            // Intermediate object - require common base class/client code has to explicitly code for it.
            // Delegating object - lot of setup work/maintenance/ require full interfaces
            // **current favourite***Shared object - marker to indicate its a reference/serialization code must serialize as reference everywhere except components.
            if (refPointer != null)
            {
                domainObject = (T)node.Context.GetReferencedObject(refPointer);

            }
            else
            {
                domainObject = factory();
            }

            return domainObject;
        }


        public static void ParseField<T>(this IPropertyNode node,
                    T parentInstance,
                    IDictionary<string, Action<T, IParseNode>> fixedFields,
                    IDictionary<Func<string, bool>, Action<T, string, IParseNode>> patternFields
    )
        {

            Action<T, IParseNode> fixedFieldMap;
            var found = fixedFields.TryGetValue(node.Name, out fixedFieldMap);

            if (fixedFieldMap != null)
            {
                try
                {
                    node.Context.StartObject(node.Name);
                    fixedFieldMap(parentInstance, node.Value);
                }
                catch (DomainParseException ex)
                {
                    ex.Pointer = node.Context.GetLocation();
                    node.Context.ParseErrors.Add(new OpenApiError(ex));
                }
                finally
                {
                    node.Context.EndObject();
                }
            }
            else
            {
                var map = patternFields.Where(p => p.Key(node.Name)).Select(p => p.Value).FirstOrDefault();
                if (map != null)
                {
                    try
                    {
                        node.Context.StartObject(node.Name);
                        map(parentInstance, node.Name, node.Value);
                    }
                    catch (DomainParseException ex)
                    {
                        ex.Pointer = node.Context.GetLocation();
                        node.Context.ParseErrors.Add(new OpenApiError(ex));
                    }
                    finally
                    {
                        node.Context.EndObject();
                    }
                }
                else
                {
                    node.Context.ParseErrors.Add(new OpenApiError("", $"{node.Name} is not a valid property at {node.Context.GetLocation()}"));
                }
            }
        }

    }
}
