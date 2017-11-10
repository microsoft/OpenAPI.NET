// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.OpenApi.Commons;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Interface;
using Microsoft.OpenApi.Readers.Properties;
using Microsoft.OpenApi.Readers.ParseNodes;

namespace Microsoft.OpenApi.Readers.V3
{
    /// <summary>
    /// The reference service for the Open API V3.0.
    /// </summary>
    internal class OpenApiV3ReferenceService : OpenApiReferenceServiceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiV3ReferenceService"/> class.
        /// </summary>
        /// <param name="rootNode">The document root node.</param>
        public OpenApiV3ReferenceService(RootNode rootNode)
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

            if (reference.ReferenceType == ReferenceType.Tag)
            {
                return "#/tags/" + reference.Name;
            }
            else
            {
                return "#/components/" + reference.ReferenceType.GetDisplayName() + "/" + reference.Name;
            }
        }

        /// <inheritdoc />
        protected override IOpenApiReference LoadReference(OpenApiReference reference, ParseNode node)
        {
            if (reference == null || (node == null && reference.ReferenceType != ReferenceType.Tag))
            {
                return null;
            }

            IOpenApiReference referencedObject = null;
            switch (reference.ReferenceType)
            {
                case ReferenceType.Schema:
                    referencedObject = OpenApiV3Deserializer.LoadSchema(node);
                    break;

                case ReferenceType.Response:
                    referencedObject = OpenApiV3Deserializer.LoadResponse(node);
                    break;

                case ReferenceType.Parameter:
                    referencedObject = OpenApiV3Deserializer.LoadParameter(node);
                    break;

                case ReferenceType.Example:
                    referencedObject = OpenApiV3Deserializer.LoadExample(node);
                    break;

                case ReferenceType.RequestBody:
                    referencedObject = OpenApiV3Deserializer.LoadRequestBody(node);
                    break;

                case ReferenceType.Header:
                    referencedObject = OpenApiV3Deserializer.LoadHeader(node);
                    break;

                case ReferenceType.SecurityScheme:
                    referencedObject = OpenApiV3Deserializer.LoadSecurityScheme(node);
                    break;

                case ReferenceType.Link:
                    referencedObject = OpenApiV3Deserializer.LoadLink(node);
                    break;

                case ReferenceType.Callback:
                    referencedObject = OpenApiV3Deserializer.LoadCallback(node);
                    break;

                case ReferenceType.Tag:
                    var list = (ListNode)node;
                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            var tag = OpenApiV3Deserializer.LoadTag(item);

                            if (tag.Name == reference.Name)
                            {
                                referencedObject = tag;
                            }
                        }
                    }
                    else
                    {
                        referencedObject = new OpenApiTag {Name = reference.Name };
                    }

                    break;

                default:
                    throw new OpenApiException(String.Format(SRResource.ReferenceHasInvalidValue, reference.ReferenceType, GetLocalPointer(reference).ToString()));
            }

            return referencedObject;
        }

        /// <inheritdoc />
        protected override JsonPointer GetLocalPointer(OpenApiReference reference)
        {
            return new JsonPointer(ToString(reference));
        }

        /// <inheritdoc />
        protected override OpenApiReference ParseLocalPointer(string localPointer)
        {
            if (String.IsNullOrWhiteSpace(localPointer))
            {
                throw new ArgumentException(String.Format(SRResource.ArgumentNullOrWhiteSpace, nameof(localPointer)));
            }

            var segments = localPointer.Split('/');
            if (segments.Length == 3) // /tags/Pet
            {
                ReferenceType referenceType = segments[1].GetEnumFromDisplayName<ReferenceType>();
                if (referenceType == ReferenceType.Tag)
                {
                    return new OpenApiReference(referenceType, segments[2]);
                }
            }
            else if (segments.Length == 4) // /components/{type}/pet
            {
                if (segments[1] == "components")
                {
                    ReferenceType referenceType = segments[2].GetEnumFromDisplayName<ReferenceType>();
                    return new OpenApiReference(referenceType, segments[3]);
                }
            }

            throw new OpenApiException(String.Format(SRResource.ReferenceHasInvalidFormat, localPointer));
        }
    }
}
