

namespace Microsoft.OpenApi.Readers
{
    using SharpYaml.Serialization;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PropertyNode : ParseNode
    {
        public PropertyNode(ParsingContext ctx, string name, YamlNode node) : base(ctx)
        {
            this.Name = name;
            Value = YamlHelper.Create(ctx,node);
        }


        public string Name { get;  set; }
        public ParseNode Value { get; set; }


        public void ParseField<T>(
                            T parentInstance,
                            IDictionary<string, Action<T, ParseNode>> fixedFields,
                            IDictionary<Func<string, bool>, Action<T, string, ParseNode>> patternFields
            )
        {

            Action<T, ParseNode> fixedFieldMap;
            var found = fixedFields.TryGetValue(this.Name, out fixedFieldMap);

            if (fixedFieldMap != null)
            {
                try
                {
                    this.Context.StartObject(this.Name);
                    fixedFieldMap(parentInstance, this.Value);
                } catch (DomainParseException ex)
                {
                    ex.Pointer = this.Context.GetLocation();
                    this.Context.ParseErrors.Add(new OpenApiError(ex));
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
                    catch (DomainParseException ex)
                    {
                        ex.Pointer = this.Context.GetLocation();
                        this.Context.ParseErrors.Add(new OpenApiError(ex));
                    }
                    finally
                    {
                        this.Context.EndObject();
                    }
                }
                else
                {
                    this.Context.ParseErrors.Add(new OpenApiError("", $"{this.Name} is not a valid property at {this.Context.GetLocation()}" ));
                }
            }
        }

    }


}
