// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.OpenApi.Commons;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Properties;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// A simple object to allow referencing other components in the specification, internally and externally.
    /// </summary>
    public class OpenApiReference : OpenApiElement
    {
        /// <summary>
        /// External resource in the reference.
        /// It maybe:
        /// 1. a absolute/relative file path, for example:  ../commons/pet.json
        /// 2. a Url, for example: http://localhost/pet.json
        /// </summary>
        public string ExternalResource { get; }

        /// <summary>
        /// The element type referenced.
        /// </summary>
        public ReferenceType ReferenceType { get; }

        /// <summary>
        /// The identifier of the reusable component of one particular ReferenceType without the starting '/'.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiReference"/> class.
        /// </summary>
        /// <param name="externalResource">The external resource.</param>
        public OpenApiReference(string externalResource)
            : this(externalResource, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiReference"/> class.
        /// </summary>
        /// <param name="externalResource">The external resource.</param>
        /// <param name="externalPointer">The external pointer, it could be null.</param>
        public OpenApiReference(string externalResource, string externalPointer)
        {
            if (String.IsNullOrWhiteSpace(externalResource))
            {
                throw Error.ArgumentNullOrWhiteSpace(nameof(externalResource));
            }

            ExternalResource = externalResource;
            Name = externalPointer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiReference"/> class.
        /// </summary>
        /// <param name="localType">The local reference type.</param>
        /// <param name="localPointer">The local pointer, it could not be null.</param>
        public OpenApiReference(ReferenceType localType, string localPointer)
        {
            if (String.IsNullOrEmpty(localPointer))
            {
                throw Error.ArgumentNullOrWhiteSpace(nameof(localPointer));
            }

            ReferenceType = localType;
            Name = localPointer;
        }

        /// <summary>
        /// Gets a flag indicating whether this reference is an external reference.
        /// </summary>
        public bool IsExternal
        {
            get
            {
                return ExternalResource != null;
            }
        }

        /// <summary>
        /// Gets a flag indicating whether this reference is a local reference.
        /// </summary>
        public bool IsLocal
        {
            get
            {
                return ExternalResource == null && Name != null;
            }
        }

        /// <summary>
        /// Gets the reference string for v3.0.
        /// </summary>
        public string ReferenceV3
        {
            get
            {
                if (IsExternal)
                {
                    return GetExternalReference();
                }

                if (ReferenceType == ReferenceType.Tag)
                {
                    return "#/tags/" + Name;
                }
                else
                {
                    return "#/components/" + ReferenceType.GetDisplayName() + "/" + Name;
                }
            }
        }

        /// <summary>
        /// Gets the reference string for V2.0
        /// </summary>
        public string ReferenceV2
        {
            get
            {
                if (IsExternal)
                {
                    return GetExternalReference();
                }

                return "#/" + GetReferenceTypeNameAsV2() + "/" + Name;
            }
        }

        /// <summary>
        /// Get a <see cref="JsonPointer"/>
        /// </summary>
        /// <returns>The Json Pointer object.</returns>
        public JsonPointer GetLocalPointer()
        {
            if (IsLocal)
            {
                return new JsonPointer(ReferenceV3);
            }

            return null;
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
            writer.WriteProperty(OpenApiConstants.DollarRef, ReferenceV3);

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
            writer.WriteProperty(OpenApiConstants.DollarRef, ReferenceV2);

            writer.WriteEndObject();
        }

        private string GetExternalReference()
        {
            if (!String.IsNullOrEmpty(ExternalResource))
            {
                if (Name != null)
                {
                    return ExternalResource + "#/" + Name;
                }
                else
                {
                    return ExternalResource;
                }
            }

            return null;
        }

        private string GetReferenceTypeNameAsV2()
        {
            switch (ReferenceType)
            {
                case ReferenceType.Schema:
                    return OpenApiConstants.Definitions;

                case ReferenceType.Parameter:
                    return OpenApiConstants.Parameters;

                case ReferenceType.Response:
                    return OpenApiConstants.Responses;

                case ReferenceType.Header:
                    return OpenApiConstants.Headers;

                case ReferenceType.Tag:
                    return OpenApiConstants.Tags;

                case ReferenceType.SecurityScheme:
                    return OpenApiConstants.SecurityDefinitions;

                default:
                    throw new OpenApiException(String.Format(SRResource.ReferenceTypeNotSupportedV2, ReferenceType));
            }
        }
    }
}
