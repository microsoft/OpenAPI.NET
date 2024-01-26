// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Parameter Object.
    /// </summary>
    public class OpenApiParameter : IOpenApiReferenceable, IEffective<OpenApiParameter>, IOpenApiExtensible
    {
        private bool? _explode;
        /// <summary>
        /// The style of the parameter.
        /// </summary>
        public ParameterStyle? _style;

        /// <summary>
        /// Indicates if object is populated with data or is just a reference to the data
        /// </summary>
        public bool UnresolvedReference { get; set; }

        /// <summary>
        /// Reference object.
        /// </summary>
        public OpenApiReference Reference { get; set; }

        /// <summary>
        /// REQUIRED. The name of the parameter. Parameter names are case sensitive.
        /// If in is "path", the name field MUST correspond to the associated path segment from the path field in the Paths Object.
        /// If in is "header" and the name field is "Accept", "Content-Type" or "Authorization", the parameter definition SHALL be ignored.
        /// For all other cases, the name corresponds to the parameter name used by the in property.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// REQUIRED. The location of the parameter.
        /// Possible values are "query", "header", "path" or "cookie".
        /// </summary>
        public ParameterLocation? In { get; set; }

        /// <summary>
        /// A brief description of the parameter. This could contain examples of use.
        /// CommonMark syntax MAY be used for rich text representation.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Determines whether this parameter is mandatory.
        /// If the parameter location is "path", this property is REQUIRED and its value MUST be true.
        /// Otherwise, the property MAY be included and its default value is false.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Specifies that a parameter is deprecated and SHOULD be transitioned out of usage.
        /// </summary>
        public bool Deprecated { get; set; }

        /// <summary>
        /// Sets the ability to pass empty-valued parameters.
        /// This is valid only for query parameters and allows sending a parameter with an empty value.
        /// Default value is false.
        /// If style is used, and if behavior is n/a (cannot be serialized),
        /// the value of allowEmptyValue SHALL be ignored.
        /// </summary>
        public bool AllowEmptyValue { get; set; }

        /// <summary>
        /// Describes how the parameter value will be serialized depending on the type of the parameter value.
        /// Default values (based on value of in): for query - form; for path - simple; for header - simple;
        /// for cookie - form.
        /// </summary>
        public ParameterStyle? Style
        {
            get => _style ?? SetDefaultStyleValue();
            set => _style = value;
        }

        /// <summary>
        /// When this is true, parameter values of type array or object generate separate parameters
        /// for each value of the array or key-value pair of the map.
        /// For other types of parameters this property has no effect.
        /// When style is form, the default value is true.
        /// For all other styles, the default value is false.
        /// </summary>
        public bool Explode
        {
            get => _explode ?? Style == ParameterStyle.Form;
            set => _explode = value;
        }

        /// <summary>
        /// Determines whether the parameter value SHOULD allow reserved characters,
        /// as defined by RFC3986 :/?#[]@!$&amp;'()*+,;= to be included without percent-encoding.
        /// This property only applies to parameters with an in value of query.
        /// The default value is false.
        /// </summary>
        public bool AllowReserved { get; set; }

        /// <summary>
        /// The schema defining the type used for the parameter.
        /// </summary>
        public OpenApiSchema Schema { get; set; }

        /// <summary>
        /// Examples of the media type. Each example SHOULD contain a value
        /// in the correct format as specified in the parameter encoding.
        /// The examples object is mutually exclusive of the example object.
        /// Furthermore, if referencing a schema which contains an example,
        /// the examples value SHALL override the example provided by the schema.
        /// </summary>
        public IDictionary<string, OpenApiExample> Examples { get; set; } = new Dictionary<string, OpenApiExample>();

        /// <summary>
        /// Example of the media type. The example SHOULD match the specified schema and encoding properties
        /// if present. The example object is mutually exclusive of the examples object.
        /// Furthermore, if referencing a schema which contains an example,
        /// the example value SHALL override the example provided by the schema.
        /// To represent examples of media types that cannot naturally be represented in JSON or YAML,
        /// a string value can contain the example with escaping where necessary.
        /// </summary>
        public IOpenApiAny Example { get; set; }

        /// <summary>
        /// A map containing the representations for the parameter.
        /// The key is the media type and the value describes it.
        /// The map MUST only contain one entry.
        /// For more complex scenarios, the content property can define the media type and schema of the parameter.
        /// A parameter MUST contain either a schema property, or a content property, but not both.
        /// When example or examples are provided in conjunction with the schema object,
        /// the example MUST follow the prescribed serialization strategy for the parameter.
        /// </summary>
        public IDictionary<string, OpenApiMediaType> Content { get; set; } = new Dictionary<string, OpenApiMediaType>();

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// A parameterless constructor
        /// </summary>
        public OpenApiParameter() {}

        /// <summary>
        /// Initializes a clone instance of <see cref="OpenApiParameter"/> object
        /// </summary>
        public OpenApiParameter(OpenApiParameter parameter)
        {
            UnresolvedReference = parameter?.UnresolvedReference ?? UnresolvedReference;
            Reference = parameter?.Reference != null ? new(parameter?.Reference) : null;
            Name = parameter?.Name ?? Name;
            In = parameter?.In ?? In;
            Description = parameter?.Description ?? Description;
            Required = parameter?.Required ?? Required;
            Style = parameter?.Style ?? Style;
            Explode = parameter?.Explode ?? Explode;
            AllowReserved = parameter?.AllowReserved ?? AllowReserved;
            Schema = parameter?.Schema != null ? new(parameter?.Schema) : null;
            Examples = parameter?.Examples != null ? new Dictionary<string, OpenApiExample>(parameter.Examples) : null;
            Example = OpenApiAnyCloneHelper.CloneFromCopyConstructor(parameter?.Example);
            Content = parameter?.Content != null ? new Dictionary<string, OpenApiMediaType>(parameter.Content) : null;
            Extensions = parameter?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(parameter.Extensions) : null;
            AllowEmptyValue = parameter?.AllowEmptyValue ?? AllowEmptyValue;
            Deprecated = parameter?.Deprecated ?? Deprecated;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiParameter"/> to Open Api v3.0
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            Utils.CheckArgumentNull(writer);

            var target = this;

            if (Reference != null)
            {
                if (!writer.GetSettings().ShouldInlineReference(Reference))
                {
                    Reference.SerializeAsV3(writer);
                    return;
                }
                else
                {
                    target = this.GetEffective(Reference.HostDocument);
                }
            }

            target.SerializeAsV3WithoutReference(writer);
        }

        /// <summary>
        /// Returns an effective OpenApiParameter object based on the presence of a $ref
        /// </summary>
        /// <param name="doc">The host OpenApiDocument that contains the reference.</param>
        /// <returns>OpenApiParameter</returns>
        public OpenApiParameter GetEffective(OpenApiDocument doc)
        {
            return Reference != null ? doc.ResolveReferenceTo<OpenApiParameter>(Reference) : this;
        }

        /// <summary>
        /// Serialize to OpenAPI V3 document without using reference.
        /// </summary>
        public void SerializeAsV3WithoutReference(IOpenApiWriter writer)
        {
            writer.WriteStartObject();

            // name
            writer.WriteProperty(OpenApiConstants.Name, Name);

            // in
            writer.WriteProperty(OpenApiConstants.In, In?.GetDisplayName());

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            // required
            writer.WriteProperty(OpenApiConstants.Required, Required, false);

            // deprecated
            writer.WriteProperty(OpenApiConstants.Deprecated, Deprecated, false);

            // allowEmptyValue
            writer.WriteProperty(OpenApiConstants.AllowEmptyValue, AllowEmptyValue, false);

            // style
            if (_style.HasValue)
            {
                writer.WriteProperty(OpenApiConstants.Style, _style.Value.GetDisplayName());
            }

            // explode
            writer.WriteProperty(OpenApiConstants.Explode, _explode, _style is ParameterStyle.Form);

            // allowReserved
            writer.WriteProperty(OpenApiConstants.AllowReserved, AllowReserved, false);

            // schema
            writer.WriteOptionalObject(OpenApiConstants.Schema, Schema, (w, s) => s.SerializeAsV3(w));

            // example
            writer.WriteOptionalObject(OpenApiConstants.Example, Example, (w, s) => w.WriteAny(s));

            // examples
            writer.WriteOptionalMap(OpenApiConstants.Examples, Examples, (w, e) => e.SerializeAsV3(w));

            // content
            writer.WriteOptionalMap(OpenApiConstants.Content, Content, (w, c) => c.SerializeAsV3(w));

            // extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiParameter"/> to Open Api v2.0
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            Utils.CheckArgumentNull(writer);

            var target = this;
            if (Reference != null)
            {
                if (!writer.GetSettings().ShouldInlineReference(Reference))
                {
                    Reference.SerializeAsV2(writer);
                    return;
                }
                else
                {
                    target = this.GetEffective(Reference.HostDocument);
                }
            }

            target.SerializeAsV2WithoutReference(writer);
        }

        /// <summary>
        /// Serialize to OpenAPI V2 document without using reference.
        /// </summary>
        public void SerializeAsV2WithoutReference(IOpenApiWriter writer)
        {
            writer.WriteStartObject();

            // in
            if (this is OpenApiFormDataParameter)
            {
                writer.WriteProperty(OpenApiConstants.In, "formData");
            }
            else if (this is OpenApiBodyParameter)
            {
                writer.WriteProperty(OpenApiConstants.In, "body");
            }
            else
            {
                writer.WriteProperty(OpenApiConstants.In, In?.GetDisplayName());
            }

            // name
            writer.WriteProperty(OpenApiConstants.Name, Name);

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            // required
            writer.WriteProperty(OpenApiConstants.Required, Required, false);

            // deprecated
            writer.WriteProperty(OpenApiConstants.Deprecated, Deprecated, false);

            var extensionsClone = new Dictionary<string, IOpenApiExtension>(Extensions);

            // schema
            if (this is OpenApiBodyParameter)
            {
                writer.WriteOptionalObject(OpenApiConstants.Schema, Schema, (w, s) => s.SerializeAsV2(w));
            }
            // In V2 parameter's type can't be a reference to a custom object schema or can't be of type object
            // So in that case map the type as string.
            else
            if (Schema?.UnresolvedReference == true || "object".Equals(Schema?.Type, StringComparison.OrdinalIgnoreCase))
            {
                writer.WriteProperty(OpenApiConstants.Type, "string");
            }
            else
            {
                // type
                // format
                // items
                // collectionFormat
                // default
                // maximum
                // exclusiveMaximum
                // minimum
                // exclusiveMinimum
                // maxLength
                // minLength
                // pattern
                // maxItems
                // minItems
                // uniqueItems
                // enum
                // multipleOf
                if (Schema != null)
                {
                    Schema.WriteAsItemsProperties(writer);

                    if (Schema.Extensions != null)
                    {
                        foreach (var key in Schema.Extensions.Keys)
                        {
                            // The extension will already have been serialized as part of the call to WriteAsItemsProperties above,
                            // so remove it from the cloned collection so we don't write it again.
                            extensionsClone.Remove(key);
                        }
                    }
                }

                // allowEmptyValue
                writer.WriteProperty(OpenApiConstants.AllowEmptyValue, AllowEmptyValue, false);

                if (this.In == ParameterLocation.Query && "array".Equals(Schema?.Type, StringComparison.OrdinalIgnoreCase))
                {
                    if (this.Style == ParameterStyle.Form && this.Explode == true)
                    {
                        writer.WriteProperty("collectionFormat", "multi");
                    }
                    else if (this.Style == ParameterStyle.PipeDelimited)
                    {
                        writer.WriteProperty("collectionFormat", "pipes");
                    }
                    else if (this.Style == ParameterStyle.SpaceDelimited)
                    {
                        writer.WriteProperty("collectionFormat", "ssv");
                    }
                }
            }

            //examples
            if (Examples != null && Examples.Any())
            {
                writer.WritePropertyName(OpenApiConstants.ExamplesExtension);
                writer.WriteStartObject();

                foreach (var example in Examples)
                {
                    writer.WritePropertyName(example.Key);
                    example.Value.Serialize(writer, OpenApiSpecVersion.OpenApi2_0);
                }
                writer.WriteEndObject();
            }

            // extensions
            writer.WriteExtensions(extensionsClone, OpenApiSpecVersion.OpenApi2_0);

            writer.WriteEndObject();
        }

        private ParameterStyle? SetDefaultStyleValue()
        {
            Style = In switch
            {
                ParameterLocation.Query => ParameterStyle.Form,
                ParameterLocation.Header => ParameterStyle.Simple,
                ParameterLocation.Path => ParameterStyle.Simple,
                ParameterLocation.Cookie => ParameterStyle.Form,
                _ => (ParameterStyle?)ParameterStyle.Simple,
            };

            return Style;
        }
    }

    /// <summary>
    /// Body parameter class to propagate information needed for <see cref="OpenApiParameter.SerializeAsV2"/>
    /// </summary>
    internal class OpenApiBodyParameter : OpenApiParameter
    {
    }

    /// <summary>
    /// Form parameter class to propagate information needed for <see cref="OpenApiParameter.SerializeAsV2"/>
    /// </summary>
    internal class OpenApiFormDataParameter : OpenApiParameter
    {
    }
}
