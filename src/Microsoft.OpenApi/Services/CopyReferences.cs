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
                    EnsureComponentsExists();
                    EnsureSchemasExists();
                    if (!Components.Schemas.ContainsKey(schema.Reference.Id))
                    {
                        Components.Schemas.Add(schema.Reference.Id, schema);
                    }
                    break;

                case OpenApiParameter parameter:
                    EnsureComponentsExists();
                    EnsureParametersExists();
                    if (!Components.Parameters.ContainsKey(parameter.Reference.Id))
                    {
                        Components.Parameters.Add(parameter.Reference.Id, parameter);
                    }
                    break;

                case OpenApiResponse response:
                    EnsureComponentsExists();
                    EnsureResponsesExists();
                    if (!Components.Responses.ContainsKey(response.Reference.Id))
                    {
                        Components.Responses.Add(response.Reference.Id, response);
                    }
                    break;

                case OpenApiRequestBody requestBody:
                    EnsureComponentsExists();
                    EnsureResponsesExists();
                    EnsurRequestBodiesExists();
                    if (!Components.RequestBodies.ContainsKey(requestBody.Reference.Id))
                    {
                        Components.RequestBodies.Add(requestBody.Reference.Id, requestBody);
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
                EnsureComponentsExists();
                EnsureSchemasExists();
                if (!Components.Schemas.ContainsKey(schema.Reference.Id))
                {
                    Components.Schemas.Add(schema.Reference.Id, schema);
                }
            }
            base.Visit(schema);
        }

        private void EnsureComponentsExists()
        {
            if (_target.Components == null)
            {
                _target.Components = new();
            }
        }

        private void EnsureSchemasExists()
        {
            if (_target.Components.Schemas == null)
            {
                _target.Components.Schemas = new Dictionary<string, OpenApiSchema>();
            }
        }

        private void EnsureParametersExists()
        {
            if (_target.Components.Parameters == null)
            {
                _target.Components.Parameters = new Dictionary<string, OpenApiParameter>();
            }
        }

        private void EnsureResponsesExists()
        {
            if (_target.Components.Responses == null)
            {
                _target.Components.Responses = new Dictionary<string, OpenApiResponse>();
            }
        }

        private void EnsurRequestBodiesExists()
        {
            _target.Components.RequestBodies ??= new Dictionary<string, OpenApiRequestBody>();
        }
    }
}
