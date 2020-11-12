// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

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
    public static class OpenApiReferencableExtensions
    {
        /// <summary>
        /// TODO: tmpDbg comment
        /// </summary>
        /// <param name="element">The referencable Open API element on which to apply the JSON pointer</param>
        /// <param name="jsonPointer">a JSON Pointer [RFC 6901](https://tools.ietf.org/html/rfc6901).</param>
        /// <returns>The element pointed to by the JSON pointer.</returns>
        public static IOpenApiReferenceable ResolveReference(this IOpenApiReferenceable element, string jsonPointer)
        {
            if (jsonPointer == "/")
            {
                return element;
            }
            if (string.IsNullOrEmpty(jsonPointer))
            {
                throw new OpenApiException(string.Format(SRResource.InvalidReferenceId, jsonPointer));
            }
            var jsonPointerTokens = jsonPointer.Split('/');
            var propertyName = jsonPointerTokens.ElementAtOrDefault(1);
            var mapKey = jsonPointerTokens.ElementAtOrDefault(2);
            try
            {
                if (element.GetType() == typeof(OpenApiHeader))
                {
                    return ResolveReferenceOnHeaderElement((OpenApiHeader)element, propertyName, mapKey, jsonPointer);
                }
                if (element.GetType() == typeof(OpenApiParameter))
                {
                    return ResolveReferenceOnParameterElement((OpenApiParameter)element, propertyName, mapKey, jsonPointer);
                }
                if (element.GetType() == typeof(OpenApiResponse))
                {
                    return ResolveReferenceOnResponseElement((OpenApiResponse)element, propertyName, mapKey, jsonPointer);
                }
            }
            catch (KeyNotFoundException)
            {
                throw new OpenApiException(string.Format(SRResource.InvalidReferenceId, jsonPointer));
            }
            throw new OpenApiException(string.Format(SRResource.InvalidReferenceId, jsonPointer));
        }

        private static IOpenApiReferenceable ResolveReferenceOnHeaderElement(
            OpenApiHeader headerElement,
            string propertyName,
            string mapKey,
            string jsonPointer)
        {
            switch (propertyName)
            {
                case OpenApiConstants.Schema:
                    return headerElement.Schema;
                case OpenApiConstants.Examples when mapKey != null:
                    return headerElement.Examples[mapKey];
                default:
                    throw new OpenApiException(string.Format(SRResource.InvalidReferenceId, jsonPointer));
            }
        }

        private static IOpenApiReferenceable ResolveReferenceOnParameterElement(
            OpenApiParameter parameterElement,
            string propertyName,
            string mapKey,
            string jsonPointer)
        {
            switch (propertyName)
            {
                case OpenApiConstants.Schema:
                    return parameterElement.Schema;
                case OpenApiConstants.Examples when mapKey != null:
                    return parameterElement.Examples[mapKey];
                default:
                    throw new OpenApiException(string.Format(SRResource.InvalidReferenceId, jsonPointer));
            }
        }

        private static IOpenApiReferenceable ResolveReferenceOnResponseElement(
            OpenApiResponse responseElement,
            string propertyName,
            string mapKey,
            string jsonPointer)
        {
            switch (propertyName)
            {
                case OpenApiConstants.Headers when mapKey != null:
                    return responseElement.Headers[mapKey];
                case OpenApiConstants.Links when mapKey != null:
                    return responseElement.Links[mapKey];
                default:
                    throw new OpenApiException(string.Format(SRResource.InvalidReferenceId, jsonPointer));
            }
        }
    }
}
