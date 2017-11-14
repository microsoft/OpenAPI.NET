// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Interface;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.Properties;

namespace Microsoft.OpenApi.Readers.V2
{
    /// <summary>
    /// The reference service for the Open API V2.0.
    /// </summary>
    internal class OpenApiV2ReferenceService : OpenApiReferenceServiceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiV2ReferenceService"/> class.
        /// </summary>
        /// <param name="rootNode">The root node.</param>
        public OpenApiV2ReferenceService(RootNode rootNode)
            : base(rootNode)
        {
        }

        /// <inheritdoc />
        public override string ToString(OpenApiReference reference)
        {
            if (reference == null)
            {
                return null;
            }

            if (reference.IsExternal)
            {
                if (reference.Name != null)
                {
                    return reference.ExternalResource + "#/" + reference.Name;
                }

                return reference.ExternalResource;
            }

            return "#/" + GetReferenceTypeName(reference.ReferenceType) + "/" + reference.Name;
        }

        /// <inheritdoc />
        protected override IOpenApiReferenceable LoadReference(OpenApiReference reference, ParseNode node)
        {
            if (reference == null || node == null)
            {
                return null;
            }

            IOpenApiReferenceable referencedObject = null;
            switch (reference.ReferenceType)
            {
                case ReferenceType.Schema:
                    referencedObject = OpenApiV2Deserializer.LoadSchema(node);
                    break;

                case ReferenceType.Parameter:
                    referencedObject = OpenApiV2Deserializer.LoadParameter(node);
                    break;

                case ReferenceType.SecurityScheme:
                    referencedObject = OpenApiV2Deserializer.LoadSecurityScheme(node);
                    break;

                case ReferenceType.Tag:
                    var list = (ListNode)node;
                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            var tag = OpenApiV2Deserializer.LoadTag(item);
                            if (tag.Name == reference.Name)
                            {
                                referencedObject = tag;
                                break;
                            }
                        }
                    }
                    else
                    {
                        referencedObject = new OpenApiTag { Name = reference.Name };
                    }

                    break;

                default:
                    throw new OpenApiException(
                        String.Format(SRResource.ReferenceHasInvalidValue, reference.ReferenceType, GetLocalPointer(reference).ToString()));
            }

            if (referencedObject == null)
            {
                throw new OpenApiException($"Cannot locate $ref {reference.ToString()}");
            }

            return referencedObject;
        }

        /// <inheritdoc />
        protected override JsonPointer GetLocalPointer(OpenApiReference reference)
        {
            return new JsonPointer("#/" + GetReferenceTypeName(reference.ReferenceType) + "/" + reference.Name);
        }

        /// <inheritdoc />
        protected override OpenApiReference ParseLocalPointer(string localPointer)
        {
            if(String.IsNullOrWhiteSpace(localPointer))
            {
                throw new ArgumentException(String.Format(SRResource.ArgumentNullOrWhiteSpace, nameof(localPointer)));
            }

            var segments = localPointer.Split('/');
            if (segments.Length >= 3) // /definitions/Pet/...
            {
                ReferenceType referenceType = ParseReferenceType(segments[1]);
                string typeName = localPointer.Substring(segments[0].Length + segments[1].Length + 2);
                return new OpenApiReference(referenceType, typeName);
            }

            throw new OpenApiException(String.Format(SRResource.ReferenceHasInvalidFormat, localPointer));
        }

        private static ReferenceType ParseReferenceType(string referenceTypeName)
        {
            switch (referenceTypeName)
            {
                case "definitions":
                    return ReferenceType.Schema;

                case "parameters":
                    return ReferenceType.Parameter;

                case "responses":
                    return ReferenceType.Response;

                case "headers":
                    return ReferenceType.Header;

                case "tags":
                    return ReferenceType.Tag;

                case "securityDefinitions":
                    return ReferenceType.SecurityScheme;

                default:
                    throw new ArgumentException();
            }
        }

        private static string GetReferenceTypeName(ReferenceType referenceType)
        {
            switch (referenceType)
            {
                case ReferenceType.Schema:
                    return "definitions";

                case ReferenceType.Parameter:
                    return "parameters";

                case ReferenceType.Response:
                    return "responses";

                case ReferenceType.Header:
                    return "headers";

                case ReferenceType.Tag:
                    return "tags";

                case ReferenceType.SecurityScheme:
                    return "securityDefinitions";

                default: throw new ArgumentException();
            }
        }
    }
}



