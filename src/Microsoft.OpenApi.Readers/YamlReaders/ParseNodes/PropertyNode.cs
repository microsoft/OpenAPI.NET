using SharpYaml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OpenApi.Readers.YamlReaders.ParseNodes
{
    internal class PropertyNode : ParseNode
    {
        public PropertyNode(ParsingContext context, OpenApiDiagnostic log, string name, YamlNode node) : base(context, log)
        {
            Name = name;
            Value = Create(context, log, node);
        }
        
        public string Name { get;  set; }

        public ParseNode Value { get; set; }

        public void ParseField<T>(
            T parentInstance,
            IDictionary<string, Action<T, ParseNode>> fixedFields,
            IDictionary<Func<string, bool>, Action<T, string, ParseNode>> patternFields)
        {
            Action<T, ParseNode> fixedFieldMap;
            var found = fixedFields.TryGetValue(this.Name, out fixedFieldMap);

            if (fixedFieldMap != null)
            {
                try
                {
                    this.Context.StartObject(this.Name);
                    fixedFieldMap(parentInstance, this.Value);
                } catch (OpenApiException ex)
                {
                    ex.Pointer = this.Context.GetLocation();
                    this.Log.Errors.Add(new OpenApiError(ex));
                }
                finally
                {
                    this.Context.EndObject();
                }
            }
            else
            {
                var map = patternFields.Where(p => p.Key(this.Name)).Select(p => p.Value).FirstOrDefault();
                if (map != null)
                {
                    try
                    {
                        this.Context.StartObject(this.Name);
                        map(parentInstance, this.Name, this.Value);
                    }
                    catch (OpenApiException ex)
                    {
                        ex.Pointer = this.Context.GetLocation();
                        this.Log.Errors.Add(new OpenApiError(ex));
                    }
                    finally
                    {
                        this.Context.EndObject();
                    }
                }
                else
                {
                    this.Log.Errors.Add(new OpenApiError("", $"{this.Name} is not a valid property at {this.Context.GetLocation()}" ));
                }
            }
        }

    }


}
