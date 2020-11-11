// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;
using Microsoft.OpenApi.Services;
using Microsoft.OpenApi.Validations;

namespace Microsoft.OpenApi.Extensions
{
    /// <summary>
    /// TODO: tmpDbg comment
    /// </summary>
    public static class OpenApiReferencableExtensions
    {
        /// <summary>
        /// TODO: tmpDbg comment
        /// </summary>
        /// <param name="element">Element to validate</param>
        /// <param name="ruleSet">Optional set of rules to use for validation</param>
        /// <returns>An IEnumerable of errors.  This function will never return null.</returns>
        public static IOpenApiReferenceable ResolveReference(this IOpenApiReferenceable element, string jsonPointer)
        {
            if (jsonPointer == "/")
                return element;
            try
            {
                if (element.GetType() == typeof(OpenApiHeader))
                {
                    return ResolveReferenceOnHeaderElement((OpenApiHeader)element, jsonPointer);
                }
                if (element.GetType() == typeof(OpenApiParameter))
                {
                    return ResolveReferenceOnParameterElement((OpenApiParameter)element, jsonPointer);
                }
            }
            catch (KeyNotFoundException)
            {
                throw new OpenApiException(string.Format(SRResource.InvalidReferenceId, jsonPointer));
            }
            throw new NotImplementedException();
        }

        private static IOpenApiReferenceable ResolveReferenceOnHeaderElement(OpenApiHeader headerElement, string jsonPointer)
        {
            var jsonPointerTokens = jsonPointer.Split('/');
            var propertyName = jsonPointerTokens.ElementAtOrDefault(1);
            switch (propertyName)
            {
                case OpenApiConstants.Schema:
                    return headerElement.Schema;
                case OpenApiConstants.Examples:
                    {
                        var mapKey = jsonPointerTokens.ElementAtOrDefault(2);
                        return headerElement.Examples[mapKey];
                    }
                default:
                    throw new OpenApiException(string.Format(SRResource.InvalidReferenceId, jsonPointer));
            }
        }

        private static IOpenApiReferenceable ResolveReferenceOnParameterElement(OpenApiParameter parameterElement, string jsonPointer)
        {
            var jsonPointerTokens = jsonPointer.Split('/');
            var propertyName = jsonPointerTokens.ElementAtOrDefault(1);
            switch (propertyName)
            {
                case OpenApiConstants.Schema:
                    return parameterElement.Schema;
                case OpenApiConstants.Examples:
                    {
                        var mapKey = jsonPointerTokens.ElementAtOrDefault(2);
                        return parameterElement.Examples[mapKey];
                    }
                default:
                    throw new OpenApiException(string.Format(SRResource.InvalidReferenceId, jsonPointer));
            }
        }
    }
}
