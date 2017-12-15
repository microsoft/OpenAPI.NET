// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Validations.Visitors
{
    /// <summary>
    /// Class to cache the <see cref="IVisitor"/>.
    /// </summary>
    internal static class OpenApiVisitorSet
    {
        private static IDictionary<Type, IVisitor> _visitorCache = new Dictionary<Type, IVisitor>
        {
            { typeof(OpenApiCallback), new CallbackVisitor() },
            { typeof(OpenApiComponents), new ComponentsVisitor() },
            { typeof(OpenApiContact), new ContactVisitor() },
            { typeof(OpenApiDiscriminator), new DiscriminatorVisitor() },
            { typeof(OpenApiDocument), new DocumentVisitor() },
            { typeof(OpenApiEncoding), new EncodingVisitor() },
            { typeof(OpenApiExample), new ExampleVisitor() },
            { typeof(OpenApiExternalDocs), new ExternalDocsVisitor() },
            { typeof(OpenApiHeader), new HeaderVisitor() },
            { typeof(OpenApiInfo), new InfoVisitor() },
            { typeof(OpenApiLicense), new LicenseVisitor() },
            { typeof(OpenApiLink), new LinkVisitor() },
            { typeof(OpenApiMediaType), new MediaTypeVisitor() },
            { typeof(OpenApiOAuthFlows), new OAuthFlowsVisitor() },
            { typeof(OpenApiOAuthFlow), new OAuthFlowVisitor() },
            { typeof(OpenApiOperation), new OperationVisitor() },
            { typeof(OpenApiParameter), new ParameterVisitor() },
            { typeof(OpenApiPathItem), new PathItemVisitor() },
            { typeof(OpenApiPaths), new PathsVisitor() },
            { typeof(OpenApiRequestBody), new RequestBodyVisitor() },
            { typeof(OpenApiResponses), new ResponsesVisitor() },
            { typeof(OpenApiResponse), new ResponseVisitor() },
            { typeof(OpenApiSchema), new SchemaVisitor() },
            { typeof(OpenApiSecurityRequirement), new SecurityRequirementVisitor() },
            { typeof(OpenApiSecurityScheme), new SecuritySchemeVisitor() },
            { typeof(OpenApiServerVariable), new ServerVariableVisitor() },
            { typeof(OpenApiServer), new ServerVisitor() },
            { typeof(OpenApiTag), new TagVisitor() },
            { typeof(OpenApiXml), new XmlVisitor() }
        };

        /// <summary>
        /// Get the element visitor.
        /// </summary>
        /// <param name="elementType">The element type.</param>
        /// <returns>The element visitor or null.</returns>
        public static IVisitor GetVisitor(Type elementType)
        {
            IVisitor visitor;
            if (_visitorCache.TryGetValue(elementType, out visitor))
            {
                return visitor;
            }

            throw new OpenApiException(String.Format(SRResource.UnknownVisitorType, elementType.FullName));
        }
    }
}
