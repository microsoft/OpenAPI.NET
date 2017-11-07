// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.OpenApi.Properties;
using Microsoft.OpenApi.Writers;
using Microsoft.OpenApi.Commons;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// A simple object to allow referencing other components in the specification, internally and externally.
    /// </summary>
    public class OpenApiReference : OpenApiElement
    {
        /// <summary>
        /// The element type referenced.
        /// </summary>
        public ReferenceType ReferenceType { get; }

        /// <summary>
        /// The given name of the object being referenced.
        /// </summary>
        public string TypeName { get; }

        /// <summary>
        /// External file path in the reference.
        /// </summary>
        public string ExternalFilePath { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiReference"/> class.
        /// </summary>
        /// <param name="reference">The reference string.</param>
        public OpenApiReference(string reference)
        {
            if (String.IsNullOrWhiteSpace(reference))
            {
                throw Error.ArgumentNullOrWhiteSpace(nameof(reference));
            }

            var segments = reference.Split('/');

            if (segments.Length == 1) //  Pet.json
            {
                if (String.IsNullOrEmpty(segments[0]) || segments[0].Contains("#"))
                {
                    throw new OpenApiException(String.Format(SRResource.ReferenceHasInvalidFormat, reference));
                }

                ExternalFilePath = segments[0];
                ReferenceType = ReferenceType.Schema; // it means only to reference schema types
            }
            else if (segments.Length == 2) // definitions.json#/Pet
            {
                if (String.IsNullOrEmpty(segments[0]) || !segments[0].EndsWith("#"))
                {
                    throw new OpenApiException(String.Format(SRResource.ReferenceHasInvalidFormat, reference));
                }

                ExternalFilePath = segments[0].Substring(0, segments[0].Length -1);
                ReferenceType = ReferenceType.Schema; // it means only to reference schema types
                TypeName = segments[1];
            }
            else if (segments.Length == 4) // #/components/schemas/Pet
            {
                ReferenceType referenceType;
                if (!segments[0].EndsWith("#") ||
                    segments[1] != "components" ||
                    (referenceType = segments[2].GetEnumFromDisplayName<ReferenceType>()) == ReferenceType.Unknown)
                {
                    throw new OpenApiException(String.Format(SRResource.ReferenceHasInvalidFormat, reference));
                }

                ReferenceType = referenceType;
                TypeName = segments[3];
                if (segments[0] != "#")
                {
                    ExternalFilePath = segments[0].Substring(0, segments[0].Length - 1);
                }
            }
            else
            {
                throw new OpenApiException(String.Format(SRResource.ReferenceHasInvalidFormat, reference));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiReference"/> class.
        /// </summary>
        /// <param name="referenceType">The Reference type.</param>
        /// <param name="typeName">The type name.</param>
        public OpenApiReference(ReferenceType referenceType, string typeName)
        {
            if (String.IsNullOrWhiteSpace(typeName))
            {
                throw Error.ArgumentNullOrWhiteSpace(nameof(typeName));
            }

            ReferenceType = referenceType;
            TypeName = typeName;
        }

        /// <summary>
        /// Get a <see cref="JsonPointer"/>
        /// </summary>
        /// <returns>The Json Pointer object.</returns>
        public JsonPointer GetLocalPointer()
        {
            return new JsonPointer(GetPointerV3());
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return GetPointerV3();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiReference"/> to Open Api v3.0.
        /// </summary>
        internal override void WriteAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // $ref
            writer.WriteStringProperty(OpenApiConstants.DollarRef, ToString());

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiReference"/> to Open Api v2.0.
        /// </summary>
        internal override void WriteAsV2(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // $ref
            writer.WriteStringProperty(OpenApiConstants.DollarRef, GetPointerV2());

            writer.WriteEndObject();
        }

        private string GetPointerV3()
        {
            var external = GetExternalReference();
            if (external != null)
            {
                return external;
            }

            return "#/components/" + ReferenceType.GetDisplayName() + "/" + TypeName;
        }

        private string GetPointerV2()
        {
            var external = GetExternalReference();
            if (external != null)
            {
                return external;
            }

            return "#/definitions/" + ReferenceType.GetDisplayName() + "/" + TypeName;
        }

        private string GetExternalReference()
        {
            if (!String.IsNullOrEmpty(ExternalFilePath))
            {
                string pointer = ExternalFilePath;
                if (TypeName != null)
                {
                    return pointer + "#/" + TypeName;
                }

                return pointer;
            }

            return null;
        }
    }
}
