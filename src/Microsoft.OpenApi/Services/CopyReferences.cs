// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Json.Schema;
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
                case JsonSchema schema:
                    EnsureComponentsExists();
                    EnsureSchemasExists();
                    if (!Components.Schemas.ContainsKey(schema.GetRef().OriginalString))
                    {
                        Components.Schemas.Add(schema.GetRef().OriginalString, schema);
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

                default:
                    break;
            }
            base.Visit(referenceable);
        }

        /// <summary>
        /// Visits <see cref="JsonSchema"/>
        /// </summary>
        /// <param name="schema">The OpenApiSchema to be visited.</param>
        public override void Visit(ref JsonSchema schema)
        {
            // This is needed to handle schemas used in Responses in components
            if (schema.GetRef() != null)
            {
                EnsureComponentsExists();
                EnsureSchemasExists();
                if (!Components.Schemas.ContainsKey(schema.GetRef().OriginalString))
                {
                    Components.Schemas.Add(schema.GetRef().OriginalString, schema);
                }
            }
            base.Visit(ref schema);
        }

        private void EnsureComponentsExists()
        {
            if (_target.Components == null)
            {
                _target.Components = new OpenApiComponents();
            }
        }

        private void EnsureSchemasExists()
        {
            if (_target.Components.Schemas == null)
            {
                _target.Components.Schemas = new Dictionary<string, JsonSchema>();
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
    }
}
