
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Services;

namespace Microsoft.OpenApi.Validations
{
    /// <summary>
    /// Class containing dispatchers to execute validation rules on for Open API document.
    /// </summary>
    public class OpenApiValidator : OpenApiVisitorBase, IValidationContext
    {
        private readonly ValidationRuleSet _ruleSet;
        private readonly IList<OpenApiValidatorError> _errors = new List<OpenApiValidatorError>();
        private readonly IList<OpenApiValidatorWarning> _warnings = new List<OpenApiValidatorWarning>();

        /// <summary>
        /// Create a visitor that will validate an OpenAPIDocument
        /// </summary>
        /// <param name="ruleSet"></param>
        public OpenApiValidator(ValidationRuleSet ruleSet)
        {
            _ruleSet = ruleSet;
        }

        /// <summary>
        /// Gets the validation errors.
        /// </summary>
        public IEnumerable<OpenApiValidatorError> Errors { get => _errors; }

        /// <summary>
        /// Gets the validation warnings.
        /// </summary>
        public IEnumerable<OpenApiValidatorWarning> Warnings { get => _warnings; }

        /// <summary>
        /// Register an error with the validation context.
        /// </summary>
        /// <param name="error">Error to register.</param>
        public void AddError(OpenApiValidatorError error)
        {
            Utils.CheckArgumentNull(error);

            _errors.Add(error);
        }

        /// <summary>
        /// Register an error with the validation context.
        /// </summary>
        /// <param name="warning">Error to register.</param>
        public void AddWarning(OpenApiValidatorWarning warning)
        {
            Utils.CheckArgumentNull(warning);

            _warnings.Add(warning);
        }

        /// <inheritdoc/>
        public override void Visit(OpenApiDocument doc) => Validate(doc);

        /// <inheritdoc/>
        public override void Visit(OpenApiInfo info) => Validate(info);

        /// <inheritdoc/>
        public override void Visit(OpenApiContact contact) => Validate(contact);

        /// <inheritdoc/>
        public override void Visit(OpenApiComponents components) => Validate(components);

        /// <inheritdoc/>
        public override void Visit(IOpenApiHeader header) => Validate(header);

        /// <inheritdoc/>
        public override void Visit(IOpenApiResponse response) => Validate(response);

        /// <inheritdoc/>
        public override void Visit(OpenApiMediaType mediaType) => Validate(mediaType);

        /// <inheritdoc/>
        public override void Visit(OpenApiResponses response) => Validate(response);

        /// <inheritdoc/>
        public override void Visit(OpenApiExternalDocs externalDocs) => Validate(externalDocs);

        /// <inheritdoc/>
        public override void Visit(OpenApiLicense license) => Validate(license);

        /// <inheritdoc/>
        public override void Visit(OpenApiOAuthFlow openApiOAuthFlow) => Validate(openApiOAuthFlow);

        /// <inheritdoc/>
        public override void Visit(OpenApiTag tag) => Validate(tag);

        /// <inheritdoc/>
        public override void Visit(IOpenApiParameter parameter) => Validate(parameter);

        /// <inheritdoc/>
        public override void Visit(IOpenApiSchema schema) => Validate(schema);

        /// <inheritdoc/>
        public override void Visit(OpenApiServer server) => Validate(server);

        /// <inheritdoc/>
        public override void Visit(OpenApiEncoding encoding) => Validate(encoding);

        /// <inheritdoc/>
        public override void Visit(IOpenApiCallback callback) => Validate(callback);

        /// <inheritdoc/>
        public override void Visit(IOpenApiExtensible openApiExtensible) => Validate(openApiExtensible);

        /// <inheritdoc/>
        public override void Visit(IOpenApiExtension openApiExtension) => Validate(openApiExtension, openApiExtension.GetType());

        /// <inheritdoc/>
        public override void Visit(IList<IOpenApiExample> example) => Validate(example, example.GetType());

        /// <inheritdoc/>
        public override void Visit(IOpenApiPathItem pathItem) => Validate(pathItem);

        /// <inheritdoc/>
        public override void Visit(OpenApiServerVariable serverVariable) => Validate(serverVariable);

        /// <inheritdoc/>
        public override void Visit(IOpenApiSecurityScheme securityScheme) => Validate(securityScheme);

        /// <inheritdoc/>
        public override void Visit(OpenApiSecurityRequirement securityRequirement) => Validate(securityRequirement);

        /// <inheritdoc/>
        public override void Visit(IOpenApiRequestBody requestBody) => Validate(requestBody);

        /// <inheritdoc/>
        public override void Visit(OpenApiPaths paths) => Validate(paths);

        /// <inheritdoc/>
        public override void Visit(IOpenApiLink link) => Validate(link);

        /// <inheritdoc/>
        public override void Visit(IOpenApiExample example) => Validate(example);

        /// <inheritdoc/>
        public override void Visit(OpenApiOperation operation) => Validate(operation);
        /// <inheritdoc/>
        public override void Visit(IDictionary<OperationType, OpenApiOperation> operations) => Validate(operations, operations.GetType());
        /// <inheritdoc/>
        public override void Visit(IDictionary<string, IOpenApiHeader> headers) => Validate(headers, headers.GetType());
        /// <inheritdoc/>
        public override void Visit(IDictionary<string, IOpenApiCallback> callbacks) => Validate(callbacks, callbacks.GetType());
        /// <inheritdoc/>
        public override void Visit(IDictionary<string, OpenApiMediaType> content) => Validate(content, content.GetType());
        /// <inheritdoc/>
        public override void Visit(IDictionary<string, IOpenApiExample> examples) => Validate(examples, examples.GetType());
        /// <inheritdoc/>
        public override void Visit(IDictionary<string, IOpenApiLink> links) => Validate(links, links.GetType());
        /// <inheritdoc/>
        public override void Visit(IDictionary<string, OpenApiServerVariable> serverVariables) => Validate(serverVariables, serverVariables.GetType());
        /// <inheritdoc/>
        public override void Visit(IDictionary<string, OpenApiEncoding> encodings) => Validate(encodings, encodings.GetType());

        private void Validate<T>(T item)
        {
            var type = typeof(T);

            Validate(item, type);
        }

        /// <summary>
        /// This overload allows applying rules based on actual object type, rather than matched interface.  This is
        /// needed for validating extensions.
        /// </summary>
        private void Validate(object? item, Type type)
        {
            if (item == null)
            {
                return;  // Required fields should be checked by higher level objects
            }

            // Validate unresolved references as references
            if (item is IOpenApiReferenceHolder { UnresolvedReference: true })
            {
                type = typeof(IOpenApiReferenceHolder);
            }

            var rules = _ruleSet.FindRules(type);
            if (rules is not null)
            {
                foreach (var rule in rules)
                {
                    rule.Evaluate(this, item);
                }
            }            
        }
    }
}
