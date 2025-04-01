// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Helpers;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Parameter Object.
    /// </summary>
    public class OpenApiParameter : IOpenApiExtensible, IOpenApiParameter
    {
        private bool? _explode;
        private ParameterStyle? _style;

        /// <inheritdoc/>
        public string? Name { get; set; }

        /// <inheritdoc/>
        public ParameterLocation? In { get; set; }

        /// <inheritdoc/>
        public string? Description { get; set; }

        /// <inheritdoc/>
        public bool Required { get; set; }

        /// <inheritdoc/>
        public bool Deprecated { get; set; }

        /// <inheritdoc/>
        public bool AllowEmptyValue { get; set; }

        /// <inheritdoc/>
        public ParameterStyle? Style
        {
            get => _style ?? GetDefaultStyleValue();
            set => _style = value;
        }

        /// <inheritdoc/>
        public bool Explode
        {
            get => _explode ?? Style == ParameterStyle.Form;
            set => _explode = value;
        }

        /// <inheritdoc/>
        public bool AllowReserved { get; set; }

        /// <inheritdoc/>
        public IOpenApiSchema? Schema { get; set; }

        /// <inheritdoc/>
        public IDictionary<string, IOpenApiExample>? Examples { get; set; }

        /// <inheritdoc/>
        public JsonNode? Example { get; set; }

        /// <inheritdoc/>
        public IDictionary<string, OpenApiMediaType>? Content { get; set; }

        /// <inheritdoc/>
        public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

        /// <summary>
        /// A parameterless constructor
        /// </summary>
        public OpenApiParameter() { }

        /// <summary>
        /// Initializes a clone instance of <see cref="OpenApiParameter"/> object
        /// </summary>
        internal OpenApiParameter(IOpenApiParameter parameter)
        {
            Utils.CheckArgumentNull(parameter);
            Name = parameter.Name ?? Name;
            In = parameter.In ?? In;
            Description = parameter.Description ?? Description;
            Required = parameter.Required;
            Style = parameter.Style ?? Style;
            Explode = parameter.Explode;
            AllowReserved = parameter.AllowReserved;
            Schema = parameter.Schema?.CreateShallowCopy();
            Examples = parameter.Examples != null ? new Dictionary<string, IOpenApiExample>(parameter.Examples) : null;
            Example = parameter.Example != null ? JsonNodeCloneHelper.Clone(parameter.Example) : null;
            Content = parameter.Content != null ? new Dictionary<string, OpenApiMediaType>(parameter.Content) : null;
            Extensions = parameter.Extensions != null ? new Dictionary<string, IOpenApiExtension>(parameter.Extensions) : null;
            AllowEmptyValue = parameter.AllowEmptyValue;
            Deprecated = parameter.Deprecated;
        }

        /// <inheritdoc/>
        public void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, (writer, element) => element.SerializeAsV31(writer));
        }

        /// <inheritdoc/>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0, (writer, element) => element.SerializeAsV3(writer));
        }

        internal void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version, 
            Action<IOpenApiWriter, IOpenApiSerializable> callback)
        {
            Utils.CheckArgumentNull(writer);

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
            if (Style.HasValue && Style != GetDefaultStyleValue())
            {
                writer.WriteProperty(OpenApiConstants.Style, Style.Value.GetDisplayName());
            }

            // explode
            writer.WriteProperty(OpenApiConstants.Explode, _explode, Style is ParameterStyle.Form);

            // allowReserved
            writer.WriteProperty(OpenApiConstants.AllowReserved, AllowReserved, false);

            // schema
            writer.WriteOptionalObject(OpenApiConstants.Schema, Schema, callback);

            // example
            writer.WriteOptionalObject(OpenApiConstants.Example, Example, (w, s) => w.WriteAny(s));

            // examples
            writer.WriteOptionalMap(OpenApiConstants.Examples, Examples, callback);

            // content
            writer.WriteOptionalMap(OpenApiConstants.Content, Content, callback);

            // extensions
            writer.WriteExtensions(Extensions, version);

            writer.WriteEndObject();
        }

        /// <inheritdoc/>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            Utils.CheckArgumentNull(writer);

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

            var extensionsClone = Extensions is not null ? new Dictionary<string, IOpenApiExtension>(Extensions) : null;

            // schema
            if (this is OpenApiBodyParameter)
            {
                writer.WriteOptionalObject(OpenApiConstants.Schema, Schema, (w, s) => s.SerializeAsV2(w));
            }
            // In V2 parameter's type can't be a reference to a custom object schema or can't be of type object
            // So in that case map the type as string.
            else if (Schema is OpenApiSchemaReference { UnresolvedReference: true } || (Schema?.Type & JsonSchemaType.Object) == JsonSchemaType.Object)
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
                var targetSchema = Schema switch {
                    OpenApiSchemaReference schemaReference => schemaReference.RecursiveTarget,
                    OpenApiSchema schema => schema,
                    _ => null,
                };
                if (targetSchema is not null)
                {
                    targetSchema.WriteAsItemsProperties(writer);
                    var extensions = Schema?.Extensions;
                    if (extensions != null)
                    {
                        foreach (var key in extensions.Keys)
                        {
                            // The extension will already have been serialized as part of the call to WriteAsItemsProperties above,
                            // so remove it from the cloned collection so we don't write it again.
                            extensionsClone?.Remove(key);
                        }
                    }
                }

                // allowEmptyValue
                writer.WriteProperty(OpenApiConstants.AllowEmptyValue, AllowEmptyValue, false);

                if (this.In == ParameterLocation.Query && Schema?.Type == JsonSchemaType.Array)
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
                    example.Value.SerializeAsV2(writer);
                }
                writer.WriteEndObject();
            }

            // extensions
            writer.WriteExtensions(extensionsClone, OpenApiSpecVersion.OpenApi2_0);

            writer.WriteEndObject();
        }

        internal virtual ParameterStyle? GetDefaultStyleValue()
        {
            return In switch
            {
                ParameterLocation.Query => ParameterStyle.Form,
                ParameterLocation.Header => ParameterStyle.Simple,
                ParameterLocation.Path => ParameterStyle.Simple,
                ParameterLocation.Cookie => ParameterStyle.Form,
                _ => (ParameterStyle?)ParameterStyle.Simple,
            };
        }

        /// <inheritdoc/>
        public IOpenApiParameter CreateShallowCopy()
        {
            return new OpenApiParameter(this);
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
