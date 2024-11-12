// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// A simple object to allow referencing other components in the specification, internally and externally.
    /// </summary>
    public class OpenApiReference : IOpenApiSerializable
    {
        /// <summary>
        /// A short summary which by default SHOULD override that of the referenced component.
        /// If the referenced object-type does not allow a summary field, then this field has no effect.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// A description which by default SHOULD override that of the referenced component.
        /// CommonMark syntax MAY be used for rich text representation.
        /// If the referenced object-type does not allow a description field, then this field has no effect.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// External resource in the reference.
        /// It maybe:
        /// 1. a absolute/relative file path, for example:  ../commons/pet.json
        /// 2. a Url, for example: http://localhost/pet.json
        /// </summary>
        public string ExternalResource { get; set; }

        /// <summary>
        /// The element type referenced.
        /// </summary>
        /// <remarks>This must be present if <see cref="ExternalResource"/> is not present.</remarks>
        public ReferenceType? Type { get; set; }

        /// <summary>
        /// The identifier of the reusable component of one particular ReferenceType.
        /// If ExternalResource is present, this is the path to the component after the '#/'.
        /// For example, if the reference is 'example.json#/path/to/component', the Id is 'path/to/component'.
        /// If ExternalResource is not present, this is the name of the component without the reference type name.
        /// For example, if the reference is '#/components/schemas/componentName', the Id is 'componentName'.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets a flag indicating whether this reference is an external reference.
        /// </summary>
        public bool IsExternal => ExternalResource != null;

        /// <summary>
        /// Gets a flag indicating whether this reference is a local reference.
        /// </summary>
        public bool IsLocal => ExternalResource == null;

        /// <summary>
        /// Gets a flag indicating whether a file is a valid OpenAPI document or a fragment
        /// </summary>
        public bool IsFragment = false;

        /// <summary>
        /// The OpenApiDocument that is hosting the OpenApiReference instance. This is used to enable dereferencing the reference.
        /// </summary>
        public OpenApiDocument HostDocument { get; set; }

        /// <summary>
        /// Gets the full reference string for v3.0.
        /// </summary>
        public string ReferenceV3
        {
            get
            {
                if (IsExternal)
                {
                    return GetExternalReferenceV3();
                }

                if (!Type.HasValue)
                {
                    throw new ArgumentNullException(nameof(Type));
                }

                if (Type == ReferenceType.Tag)
                {
                    return Id;
                }

                if (Type == ReferenceType.SecurityScheme)
                {
                    return Id;
                }
                if (Id.StartsWith("http"))
                {
                    return Id;
                }

                return "#/components/" + Type.Value.GetDisplayName() + "/" + Id;
            }
        }

        /// <summary>
        /// Gets the full reference string for V2.0
        /// </summary>
        public string ReferenceV2
        {
            get
            {
                if (IsExternal)
                {
                    return GetExternalReferenceV2();
                }

                if (!Type.HasValue)
                {
                    throw new ArgumentNullException(nameof(Type));
                }

                if (Type == ReferenceType.Tag)
                {
                    return Id;
                }

                if (Type == ReferenceType.SecurityScheme)
                {
                    return Id;
                }

                return "#/" + GetReferenceTypeNameAsV2(Type.Value) + "/" + Id;
            }
        }

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public OpenApiReference() { }

        /// <summary>
        /// Initializes a copy instance of the <see cref="OpenApiReference"/> object
        /// </summary>
        public OpenApiReference(OpenApiReference reference)
        {
            Summary = reference?.Summary;
            Description = reference?.Description;
            ExternalResource = reference?.ExternalResource;
            Type = reference?.Type;
            Id = reference?.Id;
            HostDocument = new(reference?.HostDocument);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiReference"/> to Open Api v3.1.
        /// </summary>
        public void SerializeAsV31(IOpenApiWriter writer)
        {
            // summary and description are in 3.1 but not in 3.0
            writer.WriteProperty(OpenApiConstants.Summary, Summary);
            writer.WriteProperty(OpenApiConstants.Description, Description);

            SerializeInternal(writer);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiReference"/> to Open Api v3.0.
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiReference"/>
        /// </summary>
        private void SerializeInternal(IOpenApiWriter writer)
        {
            Utils.CheckArgumentNull(writer);

            if (Type == ReferenceType.Tag)
            {
                // Write the string value only
                writer.WriteValue(ReferenceV3);
                return;
            }

            writer.WriteStartObject();

            // $ref
            writer.WriteProperty(OpenApiConstants.DollarRef, ReferenceV3);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiReference"/> to Open Api v2.0.
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            Utils.CheckArgumentNull(writer);;

            if (Type == ReferenceType.Tag)
            {
                // Write the string value only
                writer.WriteValue(ReferenceV2);
                return;
            }

            if (Type == ReferenceType.SecurityScheme)
            {
                // Write the string as property name
                writer.WritePropertyName(ReferenceV2);
                return;
            }

            writer.WriteStartObject();

            // $ref
            writer.WriteProperty(OpenApiConstants.DollarRef, ReferenceV2);

            writer.WriteEndObject();
        }

        private string GetExternalReferenceV3()
        {
            if (Id != null)
            {
                if (IsFragment)
                {
                    return ExternalResource + "#" + Id;
                }

                if (Id.StartsWith("http"))
                {
                    return Id;
                }

                if (Type.HasValue)
                {
                    return ExternalResource + "#/components/" + Type.Value.GetDisplayName() + "/"+ Id; 
                }
            }

            return ExternalResource;
        }

        private string GetExternalReferenceV2()
        {
            if (Id != null)
            {
                return ExternalResource + "#/" + GetReferenceTypeNameAsV2((ReferenceType)Type) + "/" + Id;
            }

            return ExternalResource;
        }

        private string GetReferenceTypeNameAsV2(ReferenceType type)
        {
            return type switch
            {
                ReferenceType.Schema => OpenApiConstants.Definitions,
                ReferenceType.Parameter or ReferenceType.RequestBody => OpenApiConstants.Parameters,
                ReferenceType.Response => OpenApiConstants.Responses,
                ReferenceType.Header => OpenApiConstants.Headers,
                ReferenceType.Tag => OpenApiConstants.Tags,
                ReferenceType.SecurityScheme => OpenApiConstants.SecurityDefinitions,
                _ => null,// If the reference type is not supported in V2, simply return null
                          // to indicate that the reference is not pointing to any object.
            };
        }
    }
}
