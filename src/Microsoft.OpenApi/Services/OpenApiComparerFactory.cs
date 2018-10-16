// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Defines behavior for registering specific comparer instances and encapsulates default comparers.
    /// </summary>
    public class OpenApiComparerFactory
    {
        private static readonly Dictionary<Type, object> TypeToDefaultComparerMap = new Dictionary<Type, object>
        {
            {typeof(OpenApiPaths), new OpenApiPathsComparer()},
            {typeof(OpenApiPathItem), new OpenApiPathItemComparer()},
            {typeof(OpenApiOperation), new OpenApiOperationComparer()},
            {typeof(IDictionary<OperationType, OpenApiOperation>), new OpenApiOperationsComparer()},
            {typeof(IList<OpenApiParameter>), new OpenApiParametersComparer()},
            {typeof(OpenApiParameter), new OpenApiParameterComparer()},
            {typeof(OpenApiSchema), new OpenApiSchemaComparer()},
            {typeof(OpenApiMediaType), new OpenApiMediaTypeComparer()},
            {typeof(IDictionary<string, OpenApiMediaType>), new OpenApiDictionaryComparer<OpenApiMediaType>()},
            {typeof(IDictionary<string, OpenApiResponse>), new OpenApiDictionaryComparer<OpenApiResponse>()},
            {typeof(IDictionary<string, OpenApiHeader>), new OpenApiDictionaryComparer<OpenApiHeader>()},
            {typeof(IDictionary<string, OpenApiEncoding>), new OpenApiDictionaryComparer<OpenApiEncoding>()},
            {
                typeof(IDictionary<string, OpenApiServerVariable>),
                new OpenApiDictionaryComparer<OpenApiServerVariable>()
            },
            {typeof(IDictionary<string, OpenApiParameter>), new OpenApiDictionaryComparer<OpenApiParameter>()},
            {typeof(IDictionary<string, OpenApiRequestBody>), new OpenApiDictionaryComparer<OpenApiRequestBody>()},
            {typeof(IDictionary<string, OpenApiSchema>), new OpenApiDictionaryComparer<OpenApiSchema>()},
            {
                typeof(IDictionary<string, OpenApiSecurityScheme>),
                new OpenApiDictionaryComparer<OpenApiSecurityScheme>()
            },
            {typeof(OpenApiHeader), new OpenApiHeaderComparer()},
            {typeof(OpenApiRequestBody), new OpenApiRequestBodyComparer()},
            {typeof(OpenApiResponse), new OpenApiResponseComparer()},
            {typeof(OpenApiComponents), new OpenApiComponentsComparer()},
            {typeof(OpenApiEncoding), new OpenApiEncodingComparer()},
            {typeof(IList<OpenApiServer>), new OpenApiServersComparer()},
            {typeof(OpenApiServer), new OpenApiServerComparer()},
            {typeof(OpenApiServerVariable), new OpenApiServerVariableComparer()},
            {typeof(OpenApiOAuthFlow), new OpenApiOAuthFlowComparer()},
            {typeof(OpenApiOAuthFlows), new OpenApiOAuthFlowsComparer()},
            {typeof(OpenApiSecurityRequirement), new OpenApiSecurityRequirementComparer()},
            {typeof(OpenApiInfo), new OpenApiInfoComparer()},
            {typeof(OpenApiContact), new OpenApiContactComparer()},
            {typeof(OpenApiLicense), new OpenApiLicenseComparer()},
            {typeof(IList<OpenApiSecurityRequirement>), new OpenApiOrderedListComparer<OpenApiSecurityRequirement>()},
            {typeof(IList<OpenApiTag>), new OpenApiOrderedListComparer<OpenApiTag>()},
            {typeof(OpenApiExternalDocs), new OpenApiExternalDocsComparer()},
            {typeof(OpenApiTag), new OpenApiTagComparer()},
            {typeof(OpenApiSecurityScheme), new OpenApiSecuritySchemeComparer()},
            {typeof(OpenApiExample), new OpenApiExampleComparer()},
            {typeof(IDictionary<string, OpenApiExample>), new OpenApiDictionaryComparer<OpenApiExample>()},
            {typeof(IOpenApiAny), new OpenApiAnyComparer()}
        };

        private readonly Dictionary<Type, object> _typeToComparerMap = new Dictionary<Type, object>();

        /// <summary>
        /// Adds a comparer instance to this registry.
        /// </summary>
        /// <typeparam name="T">Type of the comparer instance.</typeparam>
        /// <param name="comparer">Instance of <see cref="OpenApiComparerBase{T}"/> to register.</param>
        protected void AddComparer<T>(OpenApiComparerBase<T> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            _typeToComparerMap.Add(typeof(T), comparer);
        }

        /// <summary>
        /// Gets a registered comparer instance for the requested type.
        /// </summary>
        /// <typeparam name="T">Type of the comparer.</typeparam>
        /// <returns>The comparer instance corresponding to the type requested.</returns>
        internal OpenApiComparerBase<T> GetComparer<T>()
        {
            var requestedComparerType = typeof(T);

            if (_typeToComparerMap.TryGetValue(requestedComparerType, out object comparerInstance))
            {
                return (OpenApiComparerBase<T>) comparerInstance;
            }

            if (!TypeToDefaultComparerMap.TryGetValue(requestedComparerType, out comparerInstance))
            {
                throw Error.NotSupported(
                    $"No comparer is registered for type {requestedComparerType.Name}.");
            }

            return (OpenApiComparerBase<T>) comparerInstance;
        }
    }
}