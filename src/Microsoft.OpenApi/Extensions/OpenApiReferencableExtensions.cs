// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Extensions
{
    /// <summary>
    /// Extension methods for resolving references on <see cref="IOpenApiReferenceable"/> elements.
    /// </summary>
    public static class OpenApiReferenceableExtensions
    {
        /// <summary>
        /// Resolves a JSON Pointer with respect to an element, returning the referenced element.
        /// </summary>
        /// <param name="element">The referencable Open API element on which to apply the JSON pointer</param>
        /// <param name="pointer">a JSON Pointer [RFC 6901](https://tools.ietf.org/html/rfc6901).</param>
        /// <returns>The element pointed to by the JSON pointer.</returns>
        public static IOpenApiReferenceable ResolveReference(this IOpenApiReferenceable element, JsonPointer pointer)
        {
            if (!pointer.Tokens.Any())
            {
                return element;
            }
            var propertyName = pointer.Tokens.FirstOrDefault();
            var mapKey = pointer.Tokens.ElementAtOrDefault(1);
            try
            {
                if (propertyName is not null && mapKey is not null)
                {
                    if (element is OpenApiHeader header)
                    {
                        return ResolveReferenceOnHeaderElement(header, propertyName, mapKey, pointer);
                    }
                    if (element is OpenApiParameter parameter)
                    {
                        return ResolveReferenceOnParameterElement(parameter, propertyName, mapKey, pointer);
                    }
                    if (element is OpenApiResponse response)
                    {
                        return ResolveReferenceOnResponseElement(response, propertyName, mapKey, pointer);
                    }
                }
            }
            catch (KeyNotFoundException)
            {
                throw new OpenApiException(string.Format(SRResource.InvalidReferenceId, pointer));
            }
            throw new OpenApiException(string.Format(SRResource.InvalidReferenceId, pointer));
        }

        private static IOpenApiReferenceable ResolveReferenceOnHeaderElement(
            OpenApiHeader headerElement,
            string propertyName,
            string mapKey,
            JsonPointer pointer)
        {
            if (OpenApiConstants.Examples.Equals(propertyName, StringComparison.Ordinal) &&
                !string.IsNullOrEmpty(mapKey) &&
                headerElement?.Examples != null &&
                headerElement.Examples.TryGetValue(mapKey, out var exampleElement))
            {
                return exampleElement;
            }
            throw new OpenApiException(string.Format(SRResource.InvalidReferenceId, pointer));
        }

        private static IOpenApiReferenceable ResolveReferenceOnParameterElement(
            OpenApiParameter parameterElement,
            string propertyName,
            string mapKey,
            JsonPointer pointer)
        {
            if (OpenApiConstants.Examples.Equals(propertyName, StringComparison.Ordinal) &&
                !string.IsNullOrEmpty(mapKey) &&
                parameterElement?.Examples != null &&
                parameterElement.Examples.TryGetValue(mapKey, out var exampleElement))
            {
                return exampleElement;
            }
            throw new OpenApiException(string.Format(SRResource.InvalidReferenceId, pointer));
        }

        private static IOpenApiReferenceable ResolveReferenceOnResponseElement(
            OpenApiResponse responseElement,
            string propertyName,
            string mapKey,
            JsonPointer pointer)
        {
            if (!string.IsNullOrEmpty(mapKey))
            {
                if (OpenApiConstants.Headers.Equals(propertyName, StringComparison.Ordinal) &&
                    responseElement?.Headers != null &&
                    responseElement.Headers.TryGetValue(mapKey, out var headerElement))
                {
                    return headerElement;
                }
                if (OpenApiConstants.Links.Equals(propertyName, StringComparison.Ordinal) &&
                    responseElement?.Links != null &&
                    responseElement.Links.TryGetValue(mapKey, out var linkElement))
                {
                    return linkElement;
                }
            }
            throw new OpenApiException(string.Format(SRResource.InvalidReferenceId, pointer));
        }
    }
}
