﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// XML Object.
    /// </summary>
    public class OpenApiXml : OpenApiElement, IOpenApiExtension
    {
        /// <summary>
        /// Replaces the name of the element/attribute used for the described schema property.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The URI of the namespace definition.
        /// </summary>
        public Uri Namespace { get; set; }

        /// <summary>
        /// The prefix to be used for the name
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Declares whether the property definition translates to an attribute instead of an element.
        /// Default value is false.
        /// </summary>
        public bool Attribute { get; set; }

        /// <summary>
        /// Signifies whether the array is wrapped.
        /// Default value is false.
        /// </summary>
        public bool Wrapped { get; set; }

        /// <summary>
        /// Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiAny> Extensions { get; set; }

        /// <summary>
        /// Serialize <see cref="OpenApiXml"/> to Open Api v3.0
        /// </summary>
        internal override void WriteAsV3(IOpenApiWriter writer)
        {
            Write(writer);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiXml"/> to Open Api v2.0
        /// </summary>
        internal override void WriteAsV2(IOpenApiWriter writer)
        {
            Write(writer);
        }

        private void Write(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // name
            writer.WriteStringProperty(OpenApiConstants.Name, Name);

            // namespace
            writer.WriteStringProperty(OpenApiConstants.Namespace, Namespace?.AbsoluteUri);

            // prefix
            writer.WriteStringProperty(OpenApiConstants.Prefix, Prefix);

            // attribute
            writer.WriteBoolProperty(OpenApiConstants.Attribute, Attribute, false);

            // wrapped
            writer.WriteBoolProperty(OpenApiConstants.Wrapped, Wrapped, false);

            // extensions
            writer.WriteExtensions(Extensions);

            writer.WriteEndObject();
        }
    }
}
