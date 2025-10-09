// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// ExternalDocs object.
    /// </summary>
    public class OpenApiEncoding : IOpenApiSerializable, IOpenApiExtensible
    {
        /// <summary>
        /// Explode backing variable
        /// </summary>
        private bool? _explode;
        /// <summary>
        /// The Content-Type for encoding a specific property.
        /// The value can be a specific media type (e.g. application/json),
        /// a wildcard media type (e.g. image/*), or a comma-separated list of the two types.
        /// </summary>
        public string? ContentType { get; set; }

        /// <summary>
        /// A map allowing additional information to be provided as headers.
        /// </summary>
        public IDictionary<string, IOpenApiHeader>? Headers { get; set; }

        /// <summary>
        /// A map of property names to their encoding information.
        /// The key is the property name and the value is the encoding object.
        /// </summary>
        public IDictionary<string, OpenApiEncoding>? Encoding { get; set; }

        /// <summary>
        /// Encoding object for array items.
        /// </summary>
        public OpenApiEncoding? ItemEncoding { get; set; }

        /// <summary>
        /// Encoding objects for tuple-style arrays.
        /// </summary>
        public IList<OpenApiEncoding>? PrefixEncoding { get; set; }

        /// <summary>
        /// Describes how a specific property value will be serialized depending on its type.
        /// </summary>
        public ParameterStyle? Style { get; set; }

        /// <summary>
        /// When this is true, property values of type array or object generate separate parameters
        /// for each value of the array, or key-value-pair of the map. For other types of properties
        /// this property has no effect. When style is form, the default value is true.
        /// For all other styles, the default value is false.
        /// This property SHALL be ignored if the request body media type is not application/x-www-form-urlencoded.
        /// </summary>
        public bool? Explode
        {
            get => _explode ?? Style == ParameterStyle.Form;
            set => _explode = value;
        }

        /// <summary>
        /// Determines whether the parameter value SHOULD allow reserved characters,
        /// as defined by RFC3986 :/?#[]@!$&amp;'()*+,;= to be included without percent-encoding.
        /// The default value is false. This property SHALL be ignored
        /// if the request body media type is not application/x-www-form-urlencoded.
        /// </summary>
        public bool? AllowReserved { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

        /// <summary>
        /// Parameter-less constructor
        /// </summary>
        public OpenApiEncoding() { }

        /// <summary>
        /// Initializes a copy of an <see cref="OpenApiEncoding"/> object
        /// </summary>
        public OpenApiEncoding(OpenApiEncoding encoding)
        {
            ContentType = encoding?.ContentType ?? ContentType;
            Headers = encoding?.Headers != null ? new Dictionary<string, IOpenApiHeader>(encoding.Headers) : null;
            Encoding = encoding?.Encoding != null ? new Dictionary<string, OpenApiEncoding>(encoding.Encoding.ToDictionary(kvp => kvp.Key, kvp => new OpenApiEncoding(kvp.Value))) : null;
            ItemEncoding = encoding?.ItemEncoding != null ? new OpenApiEncoding(encoding.ItemEncoding) : null;
            PrefixEncoding = encoding?.PrefixEncoding != null ? new List<OpenApiEncoding>(encoding.PrefixEncoding.Select(e => new OpenApiEncoding(e))) : null;
            Style = encoding?.Style ?? Style;
            Explode = encoding?._explode;
            AllowReserved = encoding?.AllowReserved ?? AllowReserved;
            Extensions = encoding?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(encoding.Extensions) : null;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiEncoding"/> to Open Api v3.2
        /// </summary>
        /// <param name="writer"></param>
        public virtual void SerializeAsV32(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_2, (writer, element) => element.SerializeAsV32(writer));
        }

        /// <summary>
        /// Serialize <see cref="OpenApiEncoding"/> to Open Api v3.1
        /// </summary>
        /// <param name="writer"></param>
        public virtual void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, (writer, element) => element.SerializeAsV31(writer));
        }

        /// <summary>
        /// Serialize <see cref="OpenApiEncoding"/> to Open Api v3.0
        /// </summary>
        /// <param name="writer"></param>
        public virtual void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0, (writer, element) => element.SerializeAsV3(writer));
        }

        /// <summary>
        /// Serialize <see cref="OpenApiExternalDocs"/> to Open Api v3.0.
        /// </summary>
        private void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version,
            Action<IOpenApiWriter, IOpenApiSerializable> callback)
        {
            Utils.CheckArgumentNull(writer);

            writer.WriteStartObject();

            // contentType
            writer.WriteProperty(OpenApiConstants.ContentType, ContentType);

            // headers
            writer.WriteOptionalMap(OpenApiConstants.Headers, Headers, callback);

            // encoding - serialize as native field in v3.2+, as extension in earlier versions
            if (Encoding != null)
            {
                if (version >= OpenApiSpecVersion.OpenApi3_2)
                {
                    writer.WriteOptionalMap(OpenApiConstants.Encoding, Encoding, callback);
                }
                else
                {
                    writer.WriteOptionalMap(OpenApiConstants.ExtensionFieldNamePrefix + "oai-" + OpenApiConstants.Encoding, Encoding, callback);
                }
            }

            // itemEncoding - serialize as native field in v3.2+, as extension in earlier versions
            if (ItemEncoding != null)
            {
                if (version >= OpenApiSpecVersion.OpenApi3_2)
                {
                    writer.WriteOptionalObject(OpenApiConstants.ItemEncoding, ItemEncoding, callback);
                }
                else
                {
                    writer.WriteOptionalObject(OpenApiConstants.ExtensionFieldNamePrefix + "oai-" + OpenApiConstants.ItemEncoding, ItemEncoding, callback);
                }
            }

            // prefixEncoding - serialize as native field in v3.2+, as extension in earlier versions
            if (PrefixEncoding != null)
            {
                if (version >= OpenApiSpecVersion.OpenApi3_2)
                {
                    writer.WriteOptionalCollection(OpenApiConstants.PrefixEncoding, PrefixEncoding, callback);
                }
                else
                {
                    writer.WriteOptionalCollection(OpenApiConstants.ExtensionFieldNamePrefix + "oai-" + OpenApiConstants.PrefixEncoding, PrefixEncoding, callback);
                }
            }

            // style
            writer.WriteProperty(OpenApiConstants.Style, Style?.GetDisplayName());

            // explode
            if (_explode.HasValue)
            {
                writer.WriteProperty(OpenApiConstants.Explode, Explode);
            }

            // allowReserved
            writer.WriteProperty(OpenApiConstants.AllowReserved, AllowReserved, false);

            // extensions
            writer.WriteExtensions(Extensions, version);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiExternalDocs"/> to Open Api v2.0.
        /// </summary>
        public virtual void SerializeAsV2(IOpenApiWriter writer)
        {
            // nothing here
        }
    }
}
