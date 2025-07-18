﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace Microsoft.OpenApi;
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
                AddSchemaToComponents(openApiSchemaReference.Target, openApiSchemaReference.Reference?.Id);
                break;
            case OpenApiSchema schema:
                AddSchemaToComponents(schema);
                break;
            case OpenApiParameterReference openApiParameterReference:
                AddParameterToComponents(openApiParameterReference.Target, openApiParameterReference.Reference?.Id);
                break;
            case OpenApiParameter parameter:
                AddParameterToComponents(parameter);
                break;
            case OpenApiResponseReference openApiResponseReference:
                AddResponseToComponents(openApiResponseReference.Target, openApiResponseReference.Reference?.Id);
                break;
            case OpenApiResponse response:
                AddResponseToComponents(response);
                break;
            case OpenApiRequestBodyReference openApiRequestBodyReference:
                AddRequestBodyToComponents(openApiRequestBodyReference.Target, openApiRequestBodyReference.Reference?.Id);
                break;
            case OpenApiRequestBody requestBody:
                AddRequestBodyToComponents(requestBody);
                break;
            case OpenApiExampleReference openApiExampleReference:
                AddExampleToComponents(openApiExampleReference.Target, openApiExampleReference.Reference?.Id);
                break;
            case OpenApiExample example:
                AddExampleToComponents(example);
                break;
            case OpenApiHeaderReference openApiHeaderReference:
                AddHeaderToComponents(openApiHeaderReference.Target, openApiHeaderReference.Reference?.Id);
                break;
            case OpenApiHeader header:
                AddHeaderToComponents(header);
                break;
            case OpenApiCallbackReference openApiCallbackReference:
                AddCallbackToComponents(openApiCallbackReference.Target, openApiCallbackReference.Reference?.Id);
                break;
            case OpenApiCallback callback:
                AddCallbackToComponents(callback);
                break;
            case OpenApiLinkReference openApiLinkReference:
                AddLinkToComponents(openApiLinkReference.Target, openApiLinkReference.Reference?.Id);
                break;
            case OpenApiLink link:
                AddLinkToComponents(link);
                break;
            case OpenApiSecuritySchemeReference openApiSecuritySchemeReference:
                AddSecuritySchemeToComponents(openApiSecuritySchemeReference.Target, openApiSecuritySchemeReference.Reference?.Id);
                break;
            case OpenApiSecurityScheme securityScheme:
                AddSecuritySchemeToComponents(securityScheme);
                break;
            case OpenApiPathItemReference openApiPathItemReference:
                AddPathItemToComponents(openApiPathItemReference.Target, openApiPathItemReference.Reference?.Id);
                break;
            case OpenApiPathItem pathItem:
                AddPathItemToComponents(pathItem);
                break;
            default:
                break;
        }

        base.Visit(referenceHolder);
    }

    private void AddSchemaToComponents(IOpenApiSchema? schema, string? referenceId = null)
    {
        EnsureComponentsExist();
        EnsureSchemasExist();
        if (referenceId is not null && schema is not null && !(Components.Schemas?.ContainsKey(referenceId) ?? false))
        {
            Components.Schemas ??= new Dictionary<string, IOpenApiSchema>();
            Components.Schemas.Add(referenceId, schema);
        }
    }

    private void AddParameterToComponents(IOpenApiParameter? parameter, string? referenceId = null)
    {
        EnsureComponentsExist();
        EnsureParametersExist();
        if (parameter is not null && referenceId is not null && !(Components.Parameters?.ContainsKey(referenceId) ?? false))
        {
            Components.Parameters ??= new Dictionary<string, IOpenApiParameter>();
            Components.Parameters.Add(referenceId, parameter);
        }
    }

    private void AddResponseToComponents(IOpenApiResponse? response, string? referenceId = null)
    {
        EnsureComponentsExist();
        EnsureResponsesExist();
        if (referenceId is not null && response is not null && !(Components.Responses?.ContainsKey(referenceId) ?? false))
        {
            Components.Responses ??= new Dictionary<string, IOpenApiResponse>();
            Components.Responses.Add(referenceId, response);
        }
    }
    private void AddRequestBodyToComponents(IOpenApiRequestBody? requestBody, string? referenceId = null)
    {
        EnsureComponentsExist();
        EnsureRequestBodiesExist();
        if (requestBody is not null && referenceId is not null && !(Components.RequestBodies?.ContainsKey(referenceId) ?? false))
        {
            Components.RequestBodies ??= new Dictionary<string, IOpenApiRequestBody>();
            Components.RequestBodies.Add(referenceId, requestBody);
        }
    }
    private void AddLinkToComponents(IOpenApiLink? link, string? referenceId = null)
    {
        EnsureComponentsExist();
        EnsureLinksExist();
        if (link is not null && referenceId is not null && !(Components.Links?.ContainsKey(referenceId) ?? false))
        {
            Components.Links ??= new Dictionary<string, IOpenApiLink>();
            Components.Links.Add(referenceId, link);
        }
    }
    private void AddCallbackToComponents(IOpenApiCallback? callback, string? referenceId = null)
    {
        EnsureComponentsExist();
        EnsureCallbacksExist();
        if (callback is not null && referenceId is not null && !(Components.Callbacks?.ContainsKey(referenceId) ?? false))
        {
            Components.Callbacks ??= new Dictionary<string, IOpenApiCallback>();
            Components.Callbacks.Add(referenceId, callback);
        }
    }
    private void AddHeaderToComponents(IOpenApiHeader? header, string? referenceId = null)
    {
        EnsureComponentsExist();
        EnsureHeadersExist();
        if (header is not null && referenceId is not null && !(Components.Headers?.ContainsKey(referenceId) ?? false))
        {
            Components.Headers ??= new Dictionary<string, IOpenApiHeader>();
            Components.Headers.Add(referenceId, header);
        }
    }
    private void AddExampleToComponents(IOpenApiExample? example, string? referenceId = null)
    {
        EnsureComponentsExist();
        EnsureExamplesExist();
        if (example is not null && referenceId is not null && !(Components.Examples?.ContainsKey(referenceId) ?? false))
        {
            Components.Examples ??= new Dictionary<string, IOpenApiExample>();
            Components.Examples.Add(referenceId, example);
        }
    }
    private void AddPathItemToComponents(IOpenApiPathItem? pathItem, string? referenceId = null)
    {
        EnsureComponentsExist();
        EnsurePathItemsExist();
        if (pathItem is not null && referenceId is not null && !(Components.PathItems?.ContainsKey(referenceId) ?? false))
        {
            Components.PathItems ??= new Dictionary<string, IOpenApiPathItem>();
            Components.PathItems.Add(referenceId, pathItem);
        }
    }
    private void AddSecuritySchemeToComponents(IOpenApiSecurityScheme? securityScheme, string? referenceId = null)
    {
        EnsureComponentsExist();
        EnsureSecuritySchemesExist();
        if (securityScheme is not null && referenceId is not null && !(Components.SecuritySchemes?.ContainsKey(referenceId) ?? false))
        {
            Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
            Components.SecuritySchemes.Add(referenceId, securityScheme);
        }
    }

    /// <inheritdoc/>
    public override void Visit(IOpenApiSchema schema)
    {
        // This is needed to handle schemas used in Responses in components
        if (schema is OpenApiSchemaReference openApiSchemaReference)
        {
            AddSchemaToComponents(openApiSchemaReference.Target, openApiSchemaReference.Reference?.Id);
        }
        base.Visit(schema);
    }

    private void EnsureComponentsExist()
    {
        _target.Components ??= new();
    }

    private void EnsureSchemasExist()
    {
        if (_target.Components is not null)
        {
            _target.Components.Schemas ??= new Dictionary<string, IOpenApiSchema>();
        }
    }

    private void EnsureParametersExist()
    {
        if (_target.Components is not null)
        {
            _target.Components.Parameters ??= new Dictionary<string, IOpenApiParameter>();
        }
    }

    private void EnsureResponsesExist()
    {
        if (_target.Components is not null)
        {
            _target.Components.Responses ??= new Dictionary<string, IOpenApiResponse>();
        }
    }

    private void EnsureRequestBodiesExist()
    {
        if (_target.Components is not null)
        {
            _target.Components.RequestBodies ??= new Dictionary<string, IOpenApiRequestBody>();
        }
    }

    private void EnsureExamplesExist()
    {
        if (_target.Components is not null)
        {
            _target.Components.Examples ??= new Dictionary<string, IOpenApiExample>();
        }
    }

    private void EnsureHeadersExist()
    {
        if (_target.Components is not null)
        {
            _target.Components.Headers ??= new Dictionary<string, IOpenApiHeader>();
        }
    }

    private void EnsureCallbacksExist()
    {
        if (_target.Components is not null)
        {
            _target.Components.Callbacks ??= new Dictionary<string, IOpenApiCallback>();
        }
    }

    private void EnsureLinksExist()
    {
        if (_target.Components is not null)
        {
            _target.Components.Links ??= new Dictionary<string, IOpenApiLink>();
        }
    }

    private void EnsureSecuritySchemesExist()
    {
        if (_target.Components is not null)
        {
            _target.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        }
    }
    private void EnsurePathItemsExist()
    {
        if (_target.Components is not null)
        {
            _target.Components.PathItems = new Dictionary<string, IOpenApiPathItem>();
        }
    }
}
