// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    internal class CopyReferences : OpenApiVisitorBase
    {
        private readonly OpenApiDocument _target;
        public OpenApiComponents Components = new();

        public CopyReferences(OpenApiDocument target)
        {
            _target = target;
        }

        /// <summary>
        /// Visits IOpenApiReferenceable instances that are references and not in components.
        /// </summary>
        /// <param name="referenceable"> An IOpenApiReferenceable object.</param>
        public override void Visit(IOpenApiReferenceable referenceable)
        {
            switch (referenceable)
            {
                case OpenApiSchema schema:
                    EnsureComponentsExist();
                    EnsureSchemasExist();
                    if (!Components.Schemas.ContainsKey(schema.Reference.Id))
                    {
                        Components.Schemas.Add(schema.Reference.Id, schema);
                    }
                    break;

                case OpenApiParameter parameter:
                    EnsureComponentsExist();
                    EnsureParametersExist();
                    if (!Components.Parameters.ContainsKey(parameter.Reference.Id))
                    {
                        Components.Parameters.Add(parameter.Reference.Id, parameter);
                    }
                    break;

                case OpenApiResponse response:
                    EnsureComponentsExist();
                    EnsureResponsesExist();
                    if (!Components.Responses.ContainsKey(response.Reference.Id))
                    {
                        Components.Responses.Add(response.Reference.Id, response);
                    }
                    break;

                case OpenApiRequestBody requestBody:
                    EnsureComponentsExist();
                    EnsureResponsesExist();
                    EnsureRequestBodiesExist();
                    if (!Components.RequestBodies.ContainsKey(requestBody.Reference.Id))
                    {
                        Components.RequestBodies.Add(requestBody.Reference.Id, requestBody);
                    }
                    break;

                case OpenApiExample example:
                    EnsureComponentsExist();
                    EnsureExamplesExist();
                    if (!Components.Examples.ContainsKey(example.Reference.Id))
                    {
                        Components.Examples.Add(example.Reference.Id, example);
                    }
                    break;

                case OpenApiHeader header:
                    EnsureComponentsExist();
                    EnsureHeadersExist();
                    if (!Components.Headers.ContainsKey(header.Reference.Id))
                    {
                        Components.Headers.Add(header.Reference.Id, header);
                    }
                    break;

                case OpenApiCallback callback:
                    EnsureComponentsExist();
                    EnsureCallbacksExist();
                    if (!Components.Callbacks.ContainsKey(callback.Reference.Id))
                    {
                        Components.Callbacks.Add(callback.Reference.Id, callback);
                    }
                    break;

                case OpenApiLink link:
                    EnsureComponentsExist();
                    EnsureLinksExist();
                    if (!Components.Links.ContainsKey(link.Reference.Id))
                    {
                        Components.Links.Add(link.Reference.Id, link);
                    }
                    break;

                case OpenApiSecurityScheme securityScheme:
                    EnsureComponentsExist();
                    EnsureSecuritySchemesExist();
                    if (!Components.SecuritySchemes.ContainsKey(securityScheme.Reference.Id))
                    {
                        Components.SecuritySchemes.Add(securityScheme.Reference.Id, securityScheme);
                    }
                    break;

                default:
                    break;
            }

            base.Visit(referenceable);
        }

        /// <summary>
        /// Visits <see cref="OpenApiSchema"/>
        /// </summary>
        /// <param name="schema">The OpenApiSchema to be visited.</param>
        public override void Visit(OpenApiSchema schema)
        {
            // This is needed to handle schemas used in Responses in components
            if (schema.Reference != null)
            {
                EnsureComponentsExist();
                EnsureSchemasExist();
                if (!Components.Schemas.ContainsKey(schema.Reference.Id))
                {
                    Components.Schemas.Add(schema.Reference.Id, schema);
                }
            }
            base.Visit(schema);
        }

        private void EnsureComponentsExist()
        {
            _target.Components ??= new();
        }

        private void EnsureSchemasExist()
        {
            _target.Components.Schemas ??= new Dictionary<string, OpenApiSchema>();
        }

        private void EnsureParametersExist()
        {
            _target.Components.Parameters ??= new Dictionary<string, OpenApiParameter>();
        }

        private void EnsureResponsesExist()
        {
            _target.Components.Responses ??= new Dictionary<string, OpenApiResponse>();
        }

        private void EnsureRequestBodiesExist()
        {
            _target.Components.RequestBodies ??= new Dictionary<string, OpenApiRequestBody>();
        }

        private void EnsureExamplesExist()
        {
            _target.Components.Examples ??= new Dictionary<string, OpenApiExample>();
        }

        private void EnsureHeadersExist()
        {
            _target.Components.Headers ??= new Dictionary<string, OpenApiHeader>();
        }

        private void EnsureCallbacksExist()
        {
            _target.Components.Callbacks ??= new Dictionary<string, OpenApiCallback>();
        }

        private void EnsureLinksExist()
        {
            _target.Components.Links ??= new Dictionary<string, OpenApiLink>();
        }

        private void EnsureSecuritySchemesExist()
        {
            _target.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();
        }
    }
}
