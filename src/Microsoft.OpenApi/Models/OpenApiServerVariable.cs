// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Server Variable Object.
    /// </summary>
    public class OpenApiServerVariable : IOpenApiSerializable, IOpenApiExtensible
    {
        /// <summary>
        /// An optional description for the server variable. CommonMark syntax MAY be used for rich text representation.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// REQUIRED. The default value to use for substitution, and to send, if an alternate value is not supplied.
        /// Unlike the Schema Object's default, this value MUST be provided by the consumer.
        /// </summary>
        public string Default { get; set; }

        /// <summary>
        /// An enumeration of string values to be used if the substitution options are from a limited set.
        /// </summary>
        /// <remarks>
        /// If the server variable in the OpenAPI document has no <code>enum</code> member, this property will be null.
        /// </remarks>
        public List<string> Enum { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public OpenApiServerVariable() { }

        /// <summary>
        /// Initializes a copy of an <see cref="OpenApiServerVariable"/> object
        /// </summary>
        public OpenApiServerVariable(OpenApiServerVariable serverVariable)
        {
            Description = serverVariable?.Description;
            Default = serverVariable?.Default;
            Enum = serverVariable?.Enum != null ? new(serverVariable?.Enum) : serverVariable?.Enum;
            Extensions = serverVariable?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(serverVariable?.Extensions) : serverVariable?.Extensions;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiServerVariable"/> to Open Api v3.1
        /// </summary>
        public void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiServerVariable"/> to Open Api v3.0
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiServerVariable"/> to Open Api v3.0
        /// </summary>
        private void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version)
        {
            Utils.CheckArgumentNull(writer);;

            writer.WriteStartObject();

            // default
            writer.WriteProperty(OpenApiConstants.Default, Default);

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            // enums
            writer.WriteOptionalCollection(OpenApiConstants.Enum, Enum, (w, s) => w.WriteValue(s));

            // specification extensions
            writer.WriteExtensions(Extensions, version);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiServerVariable"/> to Open Api v2.0
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            // ServerVariable does not exist in V2.
        }
    }
}
