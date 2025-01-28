// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Models.References;

namespace Microsoft.OpenApi.Services;
internal class CopyReferences(OpenApiDocument target) : OpenApiVisitorBase
{
    private readonly OpenApiDocument _target = target;
    public OpenApiComponents Components = new();

    /// <inheritdoc/>
    public override void Visit(IOpenApiReferenceHolder referenceHolder)
    {
        switch (referenceHolder)
        {
            case OpenApiSchemaReference openApiSchemaReference:
                AddSchemaToComponents(openApiSchemaReference.Target, openApiSchemaReference.Reference.Id);
                break;
            case OpenApiSchema schema:
                AddSchemaToComponents(schema);
                break;
            case OpenApiParameterReference openApiParameterReference:
                AddParameterToComponents(openApiParameterReference.Target, openApiParameterReference.Reference.Id);
                break;
            case OpenApiParameter parameter:
                AddParameterToComponents(parameter);
                break;
            case OpenApiResponseReference openApiResponseReference:
                AddResponseToComponents(openApiResponseReference.Target, openApiResponseReference.Reference.Id);
                break;
            case OpenApiResponse response:
                AddResponseToComponents(response);
                break;
            case OpenApiRequestBodyReference openApiRequestBodyReference:
                AddRequestBodyToComponents(openApiRequestBodyReference.Target, openApiRequestBodyReference.Reference.Id);
                break;
            case OpenApiRequestBody requestBody:
                AddRequestBodyToComponents(requestBody);
                break;
            case OpenApiExampleReference openApiExampleReference:
                AddExampleToComponents(openApiExampleReference.Target, openApiExampleReference.Reference.Id);
                break;
            case OpenApiExample example:
                AddExampleToComponents(example);
                break;
            case OpenApiHeaderReference openApiHeaderReference:
                AddHeaderToComponents(openApiHeaderReference.Target, openApiHeaderReference.Reference.Id);
                break;
            case OpenApiHeader header:
                AddHeaderToComponents(header);
                break;
            case OpenApiCallbackReference openApiCallbackReference:
                AddCallbackToComponents(openApiCallbackReference.Target, openApiCallbackReference.Reference.Id);
                break;
            case OpenApiCallback callback:
                AddCallbackToComponents(callback);
                break;
            case OpenApiLinkReference openApiLinkReference:
                AddLinkToComponents(openApiLinkReference.Target, openApiLinkReference.Reference.Id);
                break;
            case OpenApiLink link:
                AddLinkToComponents(link);
                break;
            case OpenApiSecuritySchemeReference openApiSecuritySchemeReference:
                AddSecuritySchemeToComponents(openApiSecuritySchemeReference.Target, openApiSecuritySchemeReference.Reference.Id);
                break;
            case OpenApiSecurityScheme securityScheme:
                AddSecuritySchemeToComponents(securityScheme);
                break;
            case OpenApiPathItemReference openApiPathItemReference:
                AddPathItemToComponents(openApiPathItemReference.Target, openApiPathItemReference.Reference.Id);
                break;
            case OpenApiPathItem pathItem:
                AddPathItemToComponents(pathItem);
                break;
            default:
                break;
        }

        base.Visit(referenceHolder);
    }

    private void AddSchemaToComponents(OpenApiSchema schema, string referenceId = null)
    {
        EnsureComponentsExist();
        EnsureSchemasExist();
        if (!Components.Schemas.ContainsKey(referenceId ?? schema.Reference.Id))
        {
            Components.Schemas.Add(referenceId ?? schema.Reference.Id, schema);
        }
    }

    private void AddParameterToComponents(OpenApiParameter parameter, string referenceId = null)
    {
        EnsureComponentsExist();
        EnsureParametersExist();
        if (!Components.Parameters.ContainsKey(referenceId ?? parameter.Reference.Id))
        {
            Components.Parameters.Add(referenceId ?? parameter.Reference.Id, parameter);
        }
    }

    private void AddResponseToComponents(OpenApiResponse response, string referenceId = null)
    {
        EnsureComponentsExist();
        EnsureResponsesExist();
        if (!Components.Responses.ContainsKey(referenceId ?? response.Reference.Id))
        {
            Components.Responses.Add(referenceId ?? response.Reference.Id, response);
        }
    }
    private void AddRequestBodyToComponents(OpenApiRequestBody requestBody, string referenceId = null)
    {
        EnsureComponentsExist();
        EnsureRequestBodiesExist();
        if (!Components.RequestBodies.ContainsKey(referenceId ?? requestBody.Reference.Id))
        {
            Components.RequestBodies.Add(referenceId ?? requestBody.Reference.Id, requestBody);
        }
    }
    private void AddLinkToComponents(OpenApiLink link, string referenceId = null)
    {
        EnsureComponentsExist();
        EnsureLinksExist();
        if (!Components.Links.ContainsKey(referenceId))
        {
            Components.Links.Add(referenceId, link);
        }
    }
    private void AddCallbackToComponents(OpenApiCallback callback, string referenceId = null)
    {
        EnsureComponentsExist();
        EnsureCallbacksExist();
        if (!Components.Callbacks.ContainsKey(referenceId))
        {
            Components.Callbacks.Add(referenceId, callback);
        }
    }
    private void AddHeaderToComponents(OpenApiHeader header, string referenceId = null)
    {
        EnsureComponentsExist();
        EnsureHeadersExist();
        if (!Components.Headers.ContainsKey(referenceId))
        {
            Components.Headers.Add(referenceId, header);
        }
    }
    private void AddExampleToComponents(OpenApiExample example, string referenceId = null)
    {
        EnsureComponentsExist();
        EnsureExamplesExist();
        if (!Components.Examples.ContainsKey(referenceId))
        {
            Components.Examples.Add(referenceId, example);
        }
    }
    private void AddPathItemToComponents(OpenApiPathItem pathItem, string referenceId = null)
    {
        EnsureComponentsExist();
        EnsurePathItemsExist();
        if (!Components.PathItems.ContainsKey(referenceId ?? pathItem.Reference.Id))
        {
            Components.PathItems.Add(referenceId ?? pathItem.Reference.Id, pathItem);
        }
    }
    private void AddSecuritySchemeToComponents(OpenApiSecurityScheme securityScheme, string referenceId = null)
    {
        EnsureComponentsExist();
        EnsureSecuritySchemesExist();
        if (!Components.SecuritySchemes.ContainsKey(referenceId ?? securityScheme.Reference.Id))
        {
            Components.SecuritySchemes.Add(referenceId ?? securityScheme.Reference.Id, securityScheme);
        }
    }

    /// <inheritdoc/>
    public override void Visit(OpenApiSchema schema)
    {
        // This is needed to handle schemas used in Responses in components
        if (schema is OpenApiSchemaReference openApiSchemaReference)
        {
            AddSchemaToComponents(openApiSchemaReference.Target, openApiSchemaReference.Reference.Id);
        }
        else if (schema.Reference != null)
        {
            AddSchemaToComponents(schema);
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
        _target.Components.Examples ??= new Dictionary<string, IOpenApiExample>();
    }

    private void EnsureHeadersExist()
    {
        _target.Components.Headers ??= new Dictionary<string, IOpenApiHeader>();
    }

    private void EnsureCallbacksExist()
    {
        _target.Components.Callbacks ??= new Dictionary<string, IOpenApiCallback>();
    }

    private void EnsureLinksExist()
    {
        _target.Components.Links ??= new Dictionary<string, IOpenApiLink>();
    }

    private void EnsureSecuritySchemesExist()
    {
        _target.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();
    }
    private void EnsurePathItemsExist()
    {
        _target.Components.PathItems ??= new Dictionary<string, OpenApiPathItem>();
    }
}
