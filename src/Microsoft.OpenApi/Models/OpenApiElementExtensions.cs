// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Extension methods for Open API elements.
    /// </summary>
    public static class OpenApiElementExtensions
    {
        /// <summary>
        /// Add a <see cref="OpenApiPathItem"/> into the <see cref="OpenApiDocument"/>.
        /// </summary>
        /// <param name="document">The Open API document.</param>
        /// <param name="key">The path item name.</param>
        /// <param name="configure">The path item configuration action.</param>
        public static void AddPathItem(this OpenApiDocument document, string name, Action<OpenApiPathItem> configure)
        {
            if (document == null)
            {
                throw Error.ArgumentNull(nameof(document));
            }

            if (String.IsNullOrWhiteSpace(name))
            {
                throw Error.ArgumentNullOrWhiteSpace(nameof(name));
            }

            if (configure == null)
            {
                throw Error.ArgumentNull(nameof(configure));
            }

            var pathItem = new OpenApiPathItem();
            configure(pathItem);

            if (document.Paths == null)
            {
                document.Paths = new OpenApiPaths();
            }

            document.Paths.Add(name, pathItem);
        }

        /// <summary>
        /// Add a <see cref="OpenApiOperation"/> into the <see cref="OpenApiPathItem"/>.
        /// </summary>
        /// <param name="pathItem">The Open API path item.</param>
        /// <param name="operationType">The operation type kind.</param>
        /// <param name="configure">The operation configuration action.</param>
        public static void AddOperation(this OpenApiPathItem pathItem, OperationType operationType, Action<OpenApiOperation> configure)
        {
            if (pathItem == null)
            {
                throw Error.ArgumentNull(nameof(pathItem));
            }

            if (configure == null)
            {
                throw Error.ArgumentNull(nameof(configure));
            }

            var operation = new OpenApiOperation();

            configure(operation);

            pathItem.AddOperation(operationType, operation);
        }

        /// <summary>
        /// Add a <see cref="OpenApiResponse"/> into the <see cref="OpenApiOperation"/>.
        /// </summary>
        /// <param name="operation">The Open API operation.</param>
        /// <param name="name">The response name.</param>
        /// <param name="configure">The response configuration action.</param>
        public static void AddResponse(this OpenApiOperation operation, string name, Action<OpenApiResponse> configure)
        {
            if (operation == null)
            {
                throw Error.ArgumentNull(nameof(operation));
            }

            if (String.IsNullOrWhiteSpace(name))
            {
                throw Error.ArgumentNullOrWhiteSpace(nameof(name));
            }

            if (configure == null)
            {
                throw Error.ArgumentNull(nameof(configure));
            }

            var response = new OpenApiResponse();
            configure(response);

            if (operation.Responses == null)
            {
                operation.Responses = new OpenApiResponses();
            }

            operation.Responses.Add(name, response);
        }

        /// <summary>
        /// Add extension into the Extensions
        /// </summary>
        /// <typeparam name="T"><see cref="IOpenApiExtension"/>.</typeparam>
        /// <param name="element">The extensible Open API element. </param>
        /// <param name="name">The extension name.</param>
        /// <param name="any">The extension value.</param>
        public static void AddExtension<T>(this T element, string name, IOpenApiAny any)
            where T : IOpenApiExtension
        {
            if (element == null)
            {
                throw Error.ArgumentNull(nameof(element));
            }

            VerifyExtensionName(name);

            if (element.Extensions == null)
            {
                element.Extensions = new Dictionary<string, IOpenApiAny>();
            }

            element.Extensions[name] = any ?? throw Error.ArgumentNull(nameof(any));
        }

        private static void VerifyExtensionName(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw Error.ArgumentNullOrWhiteSpace(nameof(name));
            }

            if (!name.StartsWith(OpenApiConstants.OpenApiDocExtensionFieldNamePrefix))
            {
                throw new OpenApiException(String.Format(SRResource.ExtensionFieldNameMustBeginWithXDash, name));
            }
        }
    }
}
