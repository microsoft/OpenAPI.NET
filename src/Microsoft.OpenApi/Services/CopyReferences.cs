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

    private void AddSchemaToComponents(OpenApiSchema? schema, string? referenceId = null)
    {
        EnsureComponentsExist();
        EnsureSchemasExist();
        if (Components.Schemas is not null && referenceId is not null && schema is not null && !Components.Schemas.ContainsKey(referenceId))
        {
            Components.Schemas.Add(referenceId, schema);
        }
    }

    private void AddParameterToComponents(OpenApiParameter? parameter, string? referenceId = null)
    {
        EnsureComponentsExist();
        EnsureParametersExist();
        if (Components.Parameters is not null && parameter is not null && referenceId is not null && !Components.Parameters.ContainsKey(referenceId))
        {
            Components.Parameters.Add(referenceId, parameter);
        }
    }

    private void AddResponseToComponents(OpenApiResponse? response, string? referenceId = null)
    {
        EnsureComponentsExist();
        EnsureResponsesExist();
        if (Components.Responses is not null && referenceId is not null && response is not null && !Components.Responses.ContainsKey(referenceId))
        {
            Components.Responses.Add(referenceId, response);
        }
    }
    private void AddRequestBodyToComponents(OpenApiRequestBody? requestBody, string? referenceId = null)
    {
        EnsureComponentsExist();
        EnsureRequestBodiesExist();
        if (Components.RequestBodies is not null && requestBody is not null && referenceId is not null && !Components.RequestBodies.ContainsKey(referenceId))
        {
            Components.RequestBodies.Add(referenceId, requestBody);
        }
    }
    private void AddLinkToComponents(OpenApiLink? link, string? referenceId = null)
    {
        EnsureComponentsExist();
        EnsureLinksExist();
        if (Components.Links is not null && link is not null && referenceId is not null && !Components.Links.ContainsKey(referenceId))
        {
            Components.Links.Add(referenceId, link);
        }
    }
    private void AddCallbackToComponents(OpenApiCallback? callback, string? referenceId = null)
    {
        EnsureComponentsExist();
        EnsureCallbacksExist();
        if (Components.Callbacks is not null && callback is not null && referenceId is not null && !Components.Callbacks.ContainsKey(referenceId))
        {
            Components.Callbacks.Add(referenceId, callback);
        }
    }
    private void AddHeaderToComponents(OpenApiHeader? header, string? referenceId = null)
    {
        EnsureComponentsExist();
        EnsureHeadersExist();
        if (Components.Headers is not null && header is not null && referenceId is not null && !Components.Headers.ContainsKey(referenceId))
        {
            Components.Headers.Add(referenceId, header);
        }
    }
    private void AddExampleToComponents(OpenApiExample? example, string? referenceId = null)
    {
        EnsureComponentsExist();
        EnsureExamplesExist();
        if (Components.Examples is not null && example is not null && referenceId is not null && !Components.Examples.ContainsKey(referenceId))
        {
            Components.Examples.Add(referenceId, example);
        }
    }
    private void AddPathItemToComponents(OpenApiPathItem? pathItem, string? referenceId = null)
    {
        EnsureComponentsExist();
        EnsurePathItemsExist();
        if (Components.PathItems is not null && pathItem is not null && referenceId is not null && !Components.PathItems.ContainsKey(referenceId))
        {
            Components.PathItems.Add(referenceId, pathItem);
        }
    }
    private void AddSecuritySchemeToComponents(OpenApiSecurityScheme? securityScheme, string? referenceId = null)
    {
        EnsureComponentsExist();
        EnsureSecuritySchemesExist();
        if (Components.SecuritySchemes is not null && securityScheme is not null && referenceId is not null && !Components.SecuritySchemes.ContainsKey(referenceId))
        {
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
