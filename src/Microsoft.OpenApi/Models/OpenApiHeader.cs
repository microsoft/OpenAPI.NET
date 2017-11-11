// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Commons;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Header Object.
    /// The Header Object follows the structure of the Parameter Object.
    /// </summary>
    public class OpenApiHeader : OpenApiElement, IOpenApiReference, IOpenApiExtension
    {
        /// <summary>
        /// Reference pointer.
        /// </summary>
        public OpenApiReference Pointer { get; set; }

        /// <summary>
        /// A brief description of the header.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Determines whether this header is mandatory.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Specifies that a header is deprecated and SHOULD be transitioned out of usage.
        /// </summary>
        public bool Deprecated { get; set; }

        /// <summary>
        /// Sets the ability to pass empty-valued headers.
        /// </summary>
        public bool AllowEmptyValue { get; set; }

        /// <summary>
        /// Describes how the header value will be serialized depending on the type of the header value.
        /// </summary>
        public ParameterStyle? Style { get; set; }

        /// <summary>
        /// When this is true, header values of type array or object generate separate parameters
        /// for each value of the array or key-value pair of the map.
        /// </summary>
        public bool Explode { get; set; }

        /// <summary>
        /// Determines whether the header value SHOULD allow reserved characters, as defined by RFC3986.
        /// </summary>
        public bool AllowReserved { get; set; }

        /// <summary>
        /// The schema defining the type used for the header.
        /// </summary>
        public OpenApiSchema Schema { get; set; }

        /// <summary>
        /// Example of the media type.
        /// </summary>
        public IOpenApiAny Example { get; set; }

        /// <summary>
        /// Examples of the media type.
        /// </summary>
        public IList<OpenApiExample> Examples { get; set; }

        /// <summary>
        /// A map containing the representations for the header.
        /// </summary>
        public IDictionary<string, OpenApiMediaType> Content { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiAny> Extensions { get; set; }

        /// <summary>
        /// Serialize <see cref="OpenApiHeader"/> to Open Api v3.0
        /// </summary>
        internal override void WriteAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (Pointer != null)
            {
                Pointer.WriteAsV3(writer);
            }
            else
            {
                writer.WriteStartObject();

                // description
                writer.WriteProperty(OpenApiConstants.Description, Description);

                // required
                writer.WriteProperty(OpenApiConstants.Required, Required, false);

                // deprecated
                writer.WriteProperty(OpenApiConstants.Deprecated, Deprecated, false);

                // allowEmptyValue
                writer.WriteProperty(OpenApiConstants.AllowEmptyValue, AllowEmptyValue, false);

                // style
                writer.WriteProperty(OpenApiConstants.Style, Style?.GetDisplayName());

                // explode
                writer.WriteProperty(OpenApiConstants.Explode, Explode, false);

                // allowReserved
                writer.WriteProperty(OpenApiConstants.AllowReserved, AllowReserved, false);

                // schema
                writer.WriteOptionalObject(OpenApiConstants.Schema, Schema, (w, s) => s.WriteAsV3(w));

                // example
                writer.WriteOptionalObject(OpenApiConstants.Example, Example, (w, s) => w.WriteAny(s));

                // examples
                writer.WriteOptionalCollection(OpenApiConstants.Examples, Examples, (w, e) => e.WriteAsV3(w));

                // content
                writer.WriteOptionalMap(OpenApiConstants.Content, Content, (w, c) => c.WriteAsV3(w));

                // extensions
                writer.WriteExtensions(Extensions);

                writer.WriteEndObject();
            }
        }

        /// <summary>
        /// Serialize <see cref="OpenApiHeader"/> to Open Api v2.0
        /// </summary>
        internal override void WriteAsV2(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (Pointer != null)
            {
                Pointer.WriteAsV2(writer);
            }
            else
            {
                writer.WriteStartObject();

                // description
                writer.WriteProperty(OpenApiConstants.Description, Description);

                // required
                writer.WriteProperty(OpenApiConstants.Required, Required, false);

                // deprecated
                writer.WriteProperty(OpenApiConstants.Deprecated, Deprecated, false);

                // allowEmptyValue
                writer.WriteProperty(OpenApiConstants.AllowEmptyValue, AllowEmptyValue, false);

                // style
                writer.WriteProperty(OpenApiConstants.Style, Style?.GetDisplayName());

                // explode
                writer.WriteProperty(OpenApiConstants.Explode, Explode, false);

                // allowReserved
                writer.WriteProperty(OpenApiConstants.AllowReserved, AllowReserved, false);

                // schema
                writer.WriteOptionalObject(OpenApiConstants.Schema, Schema, (w, s) => s.WriteAsV2(w));

                // example
                writer.WriteOptionalObject(OpenApiConstants.Example, Example, (w, s) => w.WriteAny(s));

                // extensions
                writer.WriteExtensions(Extensions);

                writer.WriteEndObject();
            }
        }
    }
}
