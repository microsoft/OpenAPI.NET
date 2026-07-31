// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CS0618

using System;
using System.Collections.Generic;
#if NET
using System.Collections.Immutable;
#endif
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi;

internal sealed class OpenApiDeepCopyContext
{
    private readonly Dictionary<object, object> _items = new(ReferenceEqualityComparer.Instance);

    internal T Copy<T>(T source)
        where T : class
    {
        Utils.CheckArgumentNull(source);
        return CopyDynamic(source);
    }

    private T? GetExisting<T>(object source)
        where T : class
    {
        if (_items.TryGetValue(source, out var value))
        {
            return (T)value;
        }

        return null;
    }

    private void Add(object source, object target)
    {
        _items[source] = target;
    }

    internal OpenApiDocument Copy(OpenApiDocument source)
    {
        if (GetExisting<OpenApiDocument>(source) is { } existing)
        {
            return existing;
        }

        var target = new OpenApiDocument
        {
            Workspace = source.Workspace?.BaseUrl != null ? new(source.Workspace.BaseUrl) : source.Workspace != null ? new OpenApiWorkspace() : null,
            Info = Copy(source.Info),
            JsonSchemaDialect = source.JsonSchemaDialect,
            Self = source.Self,
            Servers = CopyList(source.Servers),
            Paths = Copy(source.Paths),
            Webhooks = CopyMap(source.Webhooks),
            Components = source.Components is not null ? Copy(source.Components) : null,
            Security = CopyList(source.Security),
            ExternalDocs = source.ExternalDocs is not null ? Copy(source.ExternalDocs) : null,
            Extensions = CopyExtensions(source.Extensions),
            Metadata = CopyMetadata(source.Metadata),
            BaseUri = source.BaseUri
        };
        Add(source, target);
        if (source.Tags is not null)
        {
            target.Tags = CopyTags(source.Tags);
        }
        SetReferenceHostDocument(target);
        target.RegisterComponents();
        return target;
    }

    internal OpenApiInfo Copy(OpenApiInfo source)
    {
        if (GetExisting<OpenApiInfo>(source) is { } existing)
        {
            return existing;
        }

        var target = new OpenApiInfo
        {
            Title = source.Title,
            Summary = source.Summary,
            Description = source.Description,
            Version = source.Version,
            TermsOfService = source.TermsOfService is not null ? CopyUri(source.TermsOfService) : null,
            Contact = source.Contact is not null ? Copy(source.Contact) : null,
            License = source.License is not null ? Copy(source.License) : null,
            Extensions = CopyExtensions(source.Extensions)
        };
        Add(source, target);
        return target;
    }

    internal OpenApiContact Copy(OpenApiContact source)
    {
        if (GetExisting<OpenApiContact>(source) is { } existing)
        {
            return existing;
        }

        var target = new OpenApiContact
        {
            Name = source.Name,
            Url = source.Url is not null ? CopyUri(source.Url) : null,
            Email = source.Email,
            Extensions = CopyExtensions(source.Extensions)
        };
        Add(source, target);
        return target;
    }

    internal OpenApiLicense Copy(OpenApiLicense source)
    {
        if (GetExisting<OpenApiLicense>(source) is { } existing)
        {
            return existing;
        }

        var target = new OpenApiLicense
        {
            Name = source.Name,
            Identifier = source.Identifier,
            Url = source.Url is not null ? CopyUri(source.Url) : null,
            Extensions = CopyExtensions(source.Extensions)
        };
        Add(source, target);
        return target;
    }

    internal OpenApiExternalDocs Copy(OpenApiExternalDocs source)
    {
        if (GetExisting<OpenApiExternalDocs>(source) is { } existing)
        {
            return existing;
        }

        var target = new OpenApiExternalDocs
        {
            Description = source.Description,
            Url = source.Url is not null ? CopyUri(source.Url) : null,
            Extensions = CopyExtensions(source.Extensions)
        };
        Add(source, target);
        return target;
    }

    internal OpenApiServer Copy(OpenApiServer source)
    {
        if (GetExisting<OpenApiServer>(source) is { } existing)
        {
            return existing;
        }

        var target = new OpenApiServer
        {
            Description = source.Description,
            Name = source.Name,
            Url = source.Url,
            Variables = CopyMap(source.Variables),
            Extensions = CopyExtensions(source.Extensions)
        };
        Add(source, target);
        return target;
    }

    internal OpenApiServerVariable Copy(OpenApiServerVariable source)
    {
        if (GetExisting<OpenApiServerVariable>(source) is { } existing)
        {
            return existing;
        }

        var target = new OpenApiServerVariable
        {
            Description = source.Description,
            Default = source.Default,
            Enum = source.Enum != null ? [.. source.Enum] : null,
            Extensions = CopyExtensions(source.Extensions)
        };
        Add(source, target);
        return target;
    }

    internal OpenApiPaths Copy(OpenApiPaths source)
    {
        if (GetExisting<OpenApiPaths>(source) is { } existing)
        {
            return existing;
        }

        var target = new OpenApiPaths { Extensions = CopyExtensions(source.Extensions) };
        Add(source, target);
        foreach (var item in source)
        {
            target.Add(item.Key, Copy(item.Value));
        }
        return target;
    }

    internal OpenApiComponents Copy(OpenApiComponents source)
    {
        if (GetExisting<OpenApiComponents>(source) is { } existing)
        {
            return existing;
        }

        var target = new OpenApiComponents();
        Add(source, target);
        target.Schemas = CopyMap(source.Schemas);
        target.Responses = CopyMap(source.Responses);
        target.Parameters = CopyMap(source.Parameters);
        target.Examples = CopyMap(source.Examples);
        target.RequestBodies = CopyMap(source.RequestBodies);
        target.Headers = CopyMap(source.Headers);
        target.SecuritySchemes = CopyMap(source.SecuritySchemes);
        target.Links = CopyMap(source.Links);
        target.Callbacks = CopyMap(source.Callbacks);
        target.PathItems = CopyMap(source.PathItems);
        target.MediaTypes = CopyMap(source.MediaTypes);
        target.Extensions = CopyExtensions(source.Extensions);
        return target;
    }

    internal OpenApiPathItem Copy(OpenApiPathItem source)
    {
        if (GetExisting<OpenApiPathItem>(source) is { } existing)
        {
            return existing;
        }

        var target = new OpenApiPathItem
        {
            Summary = source.Summary,
            Description = source.Description,
            Extensions = CopyExtensions(source.Extensions)
        };
        Add(source, target);
        target.Operations = source.Operations != null
            ? new Dictionary<HttpMethod, OpenApiOperation>(source.Operations.ToDictionary(static x => x.Key, x => Copy(x.Value)))
            : null;
        target.Servers = CopyList(source.Servers);
        target.Parameters = CopyList(source.Parameters);
        return target;
    }

    internal OpenApiOperation Copy(OpenApiOperation source)
    {
        if (GetExisting<OpenApiOperation>(source) is { } existing)
        {
            return existing;
        }

        var target = new OpenApiOperation
        {
            Summary = source.Summary,
            Description = source.Description,
            OperationId = source.OperationId,
            Deprecated = source.Deprecated,
            Extensions = CopyExtensions(source.Extensions),
            Metadata = CopyMetadata(source.Metadata)
        };
        Add(source, target);
        target.Tags = source.Tags != null ? new HashSet<OpenApiTagReference>(source.Tags.Select(t => Copy(t)), OpenApiTagComparer.Instance) : null;
        target.ExternalDocs = source.ExternalDocs is not null ? Copy(source.ExternalDocs) : null;
        target.Parameters = CopyList(source.Parameters);
        target.RequestBody = source.RequestBody is not null ? Copy(source.RequestBody) : null;
        target.Responses = source.Responses is not null ? Copy(source.Responses) : [];
        target.Callbacks = CopyMap(source.Callbacks);
        target.Security = CopyList(source.Security);
        target.Servers = CopyList(source.Servers);
        return target;
    }

    internal OpenApiResponses Copy(OpenApiResponses source)
    {
        if (GetExisting<OpenApiResponses>(source) is { } existing)
        {
            return existing;
        }

        var target = new OpenApiResponses { Extensions = CopyExtensions(source.Extensions) };
        Add(source, target);
        foreach (var item in source)
        {
            target.Add(item.Key, Copy(item.Value));
        }
        return target;
    }

    internal OpenApiSecurityRequirement Copy(OpenApiSecurityRequirement source)
    {
        if (GetExisting<OpenApiSecurityRequirement>(source) is { } existing)
        {
            return existing;
        }

        var target = new OpenApiSecurityRequirement();
        Add(source, target);
        foreach (var item in source)
        {
            target.Add(Copy(item.Key), item.Value != null ? [.. item.Value] : []);
        }
        return target;
    }

    internal IOpenApiPathItem Copy(IOpenApiPathItem source) => source switch
    {
        OpenApiPathItemReference reference => Copy(reference),
        OpenApiPathItem pathItem => Copy(pathItem),
        IDeepCopyable<IOpenApiPathItem> deepCopyable => deepCopyable.CreateDeepCopy(),
        _ => source.CreateShallowCopy()
    };

    internal IOpenApiCallback Copy(IOpenApiCallback source) => source switch
    {
        OpenApiCallbackReference reference => Copy(reference),
        OpenApiCallback callback => Copy(callback),
        IDeepCopyable<IOpenApiCallback> deepCopyable => deepCopyable.CreateDeepCopy(),
        _ => source.CreateShallowCopy()
    };

    internal OpenApiCallback Copy(OpenApiCallback source)
    {
        if (GetExisting<OpenApiCallback>(source) is { } existing)
        {
            return existing;
        }

        var target = new OpenApiCallback { Extensions = CopyExtensions(source.Extensions) };
        Add(source, target);
        target.PathItems = source.PathItems?.ToDictionary(static x => x.Key, x => Copy(x.Value));
        return target;
    }

    internal IOpenApiParameter Copy(IOpenApiParameter source) => source switch
    {
        OpenApiParameterReference reference => Copy(reference),
        OpenApiParameter parameter => Copy(parameter),
        IDeepCopyable<IOpenApiParameter> deepCopyable => deepCopyable.CreateDeepCopy(),
        _ => source.CreateShallowCopy()
    };

    internal OpenApiParameter Copy(OpenApiParameter source)
    {
        if (GetExisting<OpenApiParameter>(source) is { } existing)
        {
            return existing;
        }

        var target = new OpenApiParameter
        {
            Name = source.Name,
            In = source.In,
            Description = source.Description,
            Required = source.Required,
            Deprecated = source.Deprecated,
            AllowEmptyValue = source.AllowEmptyValue,
            Style = source.Style,
            Explode = source.Explode,
            AllowReserved = source.AllowReserved,
            Example = source.Example is not null ? CopyJsonNode(source.Example) : null,
            Extensions = CopyExtensions(source.Extensions)
        };
        Add(source, target);
        target.Schema = source.Schema is not null ? Copy(source.Schema) : null;
        target.Examples = CopyMap(source.Examples);
        target.Content = CopyMap(source.Content);
        return target;
    }

    internal IOpenApiRequestBody Copy(IOpenApiRequestBody source) => source switch
    {
        OpenApiRequestBodyReference reference => Copy(reference),
        OpenApiRequestBody requestBody => Copy(requestBody),
        IDeepCopyable<IOpenApiRequestBody> deepCopyable => deepCopyable.CreateDeepCopy(),
        _ => source.CreateShallowCopy()
    };

    internal OpenApiRequestBody Copy(OpenApiRequestBody source)
    {
        if (GetExisting<OpenApiRequestBody>(source) is { } existing)
        {
            return existing;
        }

        var target = new OpenApiRequestBody
        {
            Description = source.Description,
            Required = source.Required,
            Extensions = CopyExtensions(source.Extensions)
        };
        Add(source, target);
        target.Content = CopyMap(source.Content);
        return target;
    }

    internal IOpenApiResponse Copy(IOpenApiResponse source) => source switch
    {
        OpenApiResponseReference reference => Copy(reference),
        OpenApiResponse response => Copy(response),
        IDeepCopyable<IOpenApiResponse> deepCopyable => deepCopyable.CreateDeepCopy(),
        _ => source.CreateShallowCopy()
    };

    internal OpenApiResponse Copy(OpenApiResponse source)
    {
        if (GetExisting<OpenApiResponse>(source) is { } existing)
        {
            return existing;
        }

        var target = new OpenApiResponse
        {
            Summary = source.Summary,
            Description = source.Description,
            Extensions = CopyExtensions(source.Extensions)
        };
        Add(source, target);
        target.Headers = CopyMap(source.Headers);
        target.Content = CopyMap(source.Content);
        target.Links = CopyMap(source.Links);
        return target;
    }

    internal IOpenApiHeader Copy(IOpenApiHeader source) => source switch
    {
        OpenApiHeaderReference reference => Copy(reference),
        OpenApiHeader header => Copy(header),
        IDeepCopyable<IOpenApiHeader> deepCopyable => deepCopyable.CreateDeepCopy(),
        _ => source.CreateShallowCopy()
    };

    internal OpenApiHeader Copy(OpenApiHeader source)
    {
        if (GetExisting<OpenApiHeader>(source) is { } existing)
        {
            return existing;
        }

        var target = new OpenApiHeader(source.CreateShallowCopy())
        {
            Example = source.Example is not null ? CopyJsonNode(source.Example) : null,
            Extensions = CopyExtensions(source.Extensions)
        };
        Add(source, target);
        target.Schema = source.Schema is not null ? Copy(source.Schema) : null;
        target.Examples = CopyMap(source.Examples);
        target.Content = CopyMap(source.Content);
        return target;
    }

    internal IOpenApiMediaType Copy(IOpenApiMediaType source) => source switch
    {
        OpenApiMediaTypeReference reference => Copy(reference),
        OpenApiMediaType mediaType => Copy(mediaType),
        IDeepCopyable<IOpenApiMediaType> deepCopyable => deepCopyable.CreateDeepCopy(),
        _ => source.CreateShallowCopy()
    };

    internal OpenApiMediaType Copy(OpenApiMediaType source)
    {
        if (GetExisting<OpenApiMediaType>(source) is { } existing)
        {
            return existing;
        }

        var target = new OpenApiMediaType
        {
            Example = source.Example is not null ? CopyJsonNode(source.Example) : null,
            ItemEncoding = source.ItemEncoding is not null ? Copy(source.ItemEncoding) : null,
            Extensions = CopyExtensions(source.Extensions)
        };
        Add(source, target);
        target.Schema = source.Schema is not null ? Copy(source.Schema) : null;
        target.ItemSchema = source.ItemSchema is not null ? Copy(source.ItemSchema) : null;
        target.Examples = CopyMap(source.Examples);
        target.Encoding = CopyMap(source.Encoding);
        target.PrefixEncoding = CopyList(source.PrefixEncoding);
        return target;
    }

    internal OpenApiEncoding Copy(OpenApiEncoding source)
    {
        if (GetExisting<OpenApiEncoding>(source) is { } existing)
        {
            return existing;
        }

        var target = new OpenApiEncoding
        {
            ContentType = source.ContentType,
            Style = source.Style,
            Explode = source.Explode,
            AllowReserved = source.AllowReserved,
            Extensions = CopyExtensions(source.Extensions)
        };
        Add(source, target);
        target.Headers = CopyMap(source.Headers);
        target.Encoding = CopyMap(source.Encoding);
        target.ItemEncoding = source.ItemEncoding is not null ? Copy(source.ItemEncoding) : null;
        target.PrefixEncoding = CopyList(source.PrefixEncoding);
        return target;
    }

    internal IOpenApiExample Copy(IOpenApiExample source) => source switch
    {
        OpenApiExampleReference reference => Copy(reference),
        OpenApiExample example => Copy(example),
        IDeepCopyable<IOpenApiExample> deepCopyable => deepCopyable.CreateDeepCopy(),
        _ => source.CreateShallowCopy()
    };

    internal OpenApiExample Copy(OpenApiExample source)
    {
        if (GetExisting<OpenApiExample>(source) is { } existing)
        {
            return existing;
        }

        var target = new OpenApiExample
        {
            Summary = source.Summary,
            Description = source.Description,
            ExternalValue = source.ExternalValue,
            Value = source.Value is not null ? CopyJsonNode(source.Value) : null,
            DataValue = source.DataValue is not null ? CopyJsonNode(source.DataValue) : null,
            SerializedValue = source.SerializedValue,
            Extensions = CopyExtensions(source.Extensions)
        };
        Add(source, target);
        return target;
    }

    internal IOpenApiLink Copy(IOpenApiLink source) => source switch
    {
        OpenApiLinkReference reference => Copy(reference),
        OpenApiLink link => Copy(link),
        IDeepCopyable<IOpenApiLink> deepCopyable => deepCopyable.CreateDeepCopy(),
        _ => source.CreateShallowCopy()
    };

    internal OpenApiLink Copy(OpenApiLink source)
    {
        if (GetExisting<OpenApiLink>(source) is { } existing)
        {
            return existing;
        }

        var target = new OpenApiLink
        {
            OperationRef = source.OperationRef,
            OperationId = source.OperationId,
            RequestBody = source.RequestBody is not null ? Copy(source.RequestBody) : null,
            Description = source.Description,
            Server = source.Server is not null ? Copy(source.Server) : null,
            Extensions = CopyExtensions(source.Extensions)
        };
        Add(source, target);
        target.Parameters = CopyMap(source.Parameters);
        return target;
    }

    internal RuntimeExpressionAnyWrapper Copy(RuntimeExpressionAnyWrapper source)
    {
        if (GetExisting<RuntimeExpressionAnyWrapper>(source) is { } existing)
        {
            return existing;
        }

        var target = new RuntimeExpressionAnyWrapper(source);
        Add(source, target);
        return target;
    }

    internal IOpenApiSecurityScheme Copy(IOpenApiSecurityScheme source) => source switch
    {
        OpenApiSecuritySchemeReference reference => Copy(reference),
        OpenApiSecurityScheme securityScheme => Copy(securityScheme),
        IDeepCopyable<IOpenApiSecurityScheme> deepCopyable => deepCopyable.CreateDeepCopy(),
        _ => source.CreateShallowCopy()
    };

    internal OpenApiSecurityScheme Copy(OpenApiSecurityScheme source)
    {
        if (GetExisting<OpenApiSecurityScheme>(source) is { } existing)
        {
            return existing;
        }

        var target = new OpenApiSecurityScheme
        {
            Type = source.Type,
            Description = source.Description,
            Name = source.Name,
            In = source.In,
            Scheme = source.Scheme,
            BearerFormat = source.BearerFormat,
            OpenIdConnectUrl = source.OpenIdConnectUrl is not null ? CopyUri(source.OpenIdConnectUrl) : null,
            OAuth2MetadataUrl = source.OAuth2MetadataUrl is not null ? CopyUri(source.OAuth2MetadataUrl) : null,
            Deprecated = source.Deprecated,
            Extensions = CopyExtensions(source.Extensions)
        };
        Add(source, target);
        target.Flows = source.Flows is not null ? Copy(source.Flows) : null;
        return target;
    }

    internal OpenApiOAuthFlows Copy(OpenApiOAuthFlows source)
    {
        if (GetExisting<OpenApiOAuthFlows>(source) is { } existing)
        {
            return existing;
        }

        var target = new OpenApiOAuthFlows { Extensions = CopyExtensions(source.Extensions) };
        Add(source, target);
        target.Implicit = source.Implicit is not null ? Copy(source.Implicit) : null;
        target.Password = source.Password is not null ? Copy(source.Password) : null;
        target.ClientCredentials = source.ClientCredentials is not null ? Copy(source.ClientCredentials) : null;
        target.AuthorizationCode = source.AuthorizationCode is not null ? Copy(source.AuthorizationCode) : null;
        target.DeviceAuthorization = source.DeviceAuthorization is not null ? Copy(source.DeviceAuthorization) : null;
        return target;
    }

    internal OpenApiOAuthFlow Copy(OpenApiOAuthFlow source)
    {
        if (GetExisting<OpenApiOAuthFlow>(source) is { } existing)
        {
            return existing;
        }

        var target = new OpenApiOAuthFlow
        {
            AuthorizationUrl = source.AuthorizationUrl is not null ? CopyUri(source.AuthorizationUrl) : null,
            TokenUrl = source.TokenUrl is not null ? CopyUri(source.TokenUrl) : null,
            RefreshUrl = source.RefreshUrl is not null ? CopyUri(source.RefreshUrl) : null,
            DeviceAuthorizationUrl = source.DeviceAuthorizationUrl is not null ? CopyUri(source.DeviceAuthorizationUrl) : null,
            Scopes = source.Scopes != null ? new Dictionary<string, string>(source.Scopes) : null,
            Extensions = CopyExtensions(source.Extensions)
        };
        Add(source, target);
        return target;
    }

    internal IOpenApiTag Copy(IOpenApiTag source) => source switch
    {
        OpenApiTagReference reference => Copy(reference),
        OpenApiTag tag => Copy(tag),
        IDeepCopyable<IOpenApiTag> deepCopyable => deepCopyable.CreateDeepCopy(),
        _ => source.CreateShallowCopy()
    };

    internal OpenApiTag Copy(OpenApiTag source)
    {
        if (GetExisting<OpenApiTag>(source) is { } existing)
        {
            return existing;
        }

        var target = new OpenApiTag
        {
            Name = source.Name,
            Description = source.Description,
            Summary = source.Summary,
            Kind = source.Kind,
            Extensions = CopyExtensions(source.Extensions)
        };
        Add(source, target);
        target.ExternalDocs = source.ExternalDocs is not null ? Copy(source.ExternalDocs) : null;
        target.Parent = source.Parent is not null ? Copy(source.Parent) : null;
        return target;
    }

    internal IOpenApiSchema Copy(IOpenApiSchema source) => source switch
    {
        OpenApiSchemaReference reference => Copy(reference),
        OpenApiSchema schema => Copy(schema),
        IDeepCopyable<IOpenApiSchema> deepCopyable => deepCopyable.CreateDeepCopy(),
        _ => source.CreateShallowCopy()
    };

    internal OpenApiSchema Copy(OpenApiSchema source)
    {
        if (GetExisting<OpenApiSchema>(source) is { } existing)
        {
            return existing;
        }

        var target = new OpenApiSchema(source)
        {
            Vocabulary = source.Vocabulary != null ? new Dictionary<string, bool>(source.Vocabulary) : null,
            Default = source.Default is not null ? CopyJsonNode(source.Default) : null,
            Example = source.Example is not null ? CopyJsonNode(source.Example) : null,
            Examples = source.Examples?.Select(CopyJsonNode).ToList(),
            Enum = source.Enum?.Select(CopyJsonNode).ToList(),
            Discriminator = source.Discriminator is not null ? Copy(source.Discriminator) : null,
            ExternalDocs = source.ExternalDocs is not null ? Copy(source.ExternalDocs) : null,
            Xml = source.Xml is not null ? Copy(source.Xml) : null,
            Extensions = CopyExtensions(source.Extensions),
            Metadata = CopyMetadata(source.Metadata),
            UnrecognizedKeywords = source.UnrecognizedKeywords?.ToDictionary(static x => x.Key, x => CopyJsonNode(x.Value)),
            DependentRequired = source.DependentRequired?.ToDictionary(static x => x.Key, static x => new HashSet<string>(x.Value))
        };
        Add(source, target);
        target.Definitions = CopyMap(source.Definitions);
        if (source is IOpenApiSchemaMissingProperties missing)
        {
            target.Contains = missing.Contains is not null ? Copy(missing.Contains) : null;
            target.UnevaluatedPropertiesSchema = missing.UnevaluatedPropertiesSchema is not null ? Copy(missing.UnevaluatedPropertiesSchema) : null;
            target.ContentSchema = missing.ContentSchema is not null ? Copy(missing.ContentSchema) : null;
            target.PropertyNames = missing.PropertyNames is not null ? Copy(missing.PropertyNames) : null;
            target.DependentSchemas = CopyMap(missing.DependentSchemas);
            target.If = missing.If is not null ? Copy(missing.If) : null;
            target.Then = missing.Then is not null ? Copy(missing.Then) : null;
            target.Else = missing.Else is not null ? Copy(missing.Else) : null;
        }
        target.AllOf = CopyList(source.AllOf);
        target.OneOf = CopyList(source.OneOf);
        target.AnyOf = CopyList(source.AnyOf);
        target.Not = source.Not is not null ? Copy(source.Not) : null;
        target.Required = source.Required != null ? new HashSet<string>(source.Required) : null;
        target.Items = source.Items is not null ? Copy(source.Items) : null;
        target.Properties = CopyMap(source.Properties);
        target.PatternProperties = CopyMap(source.PatternProperties);
        target.AdditionalProperties = source.AdditionalProperties is not null ? Copy(source.AdditionalProperties) : null;
        return target;
    }

    internal OpenApiDiscriminator Copy(OpenApiDiscriminator source)
    {
        if (GetExisting<OpenApiDiscriminator>(source) is { } existing)
        {
            return existing;
        }

        var target = new OpenApiDiscriminator
        {
            PropertyName = source.PropertyName,
            Extensions = CopyExtensions(source.Extensions)
        };
        Add(source, target);
        target.Mapping = CopyMap(source.Mapping);
        target.DefaultMapping = source.DefaultMapping is not null ? Copy(source.DefaultMapping) : null;
        return target;
    }

    internal OpenApiXml Copy(OpenApiXml source)
    {
        if (GetExisting<OpenApiXml>(source) is { } existing)
        {
            return existing;
        }

        var target = new OpenApiXml
        {
            Name = source.Name,
            Namespace = source.Namespace is not null ? CopyUri(source.Namespace) : null,
            Prefix = source.Prefix,
            NodeType = source.NodeType,
            Extensions = CopyExtensions(source.Extensions)
        };
        Add(source, target);
        return target;
    }

    internal OpenApiSchemaReference Copy(OpenApiSchemaReference source)
    {
        if (GetExisting<OpenApiSchemaReference>(source) is { } existing)
        {
            return existing;
        }

        var target = (OpenApiSchemaReference)source.CreateShallowCopy();
        Add(source, target);
        CopyJsonSchemaReference(source.Reference, target.Reference);
        return target;
    }

    internal OpenApiCallbackReference Copy(OpenApiCallbackReference source) => CopyReference<OpenApiCallbackReference, IOpenApiCallback>(source);
    internal OpenApiExampleReference Copy(OpenApiExampleReference source) => CopyReference<OpenApiExampleReference, IOpenApiExample>(source);
    internal OpenApiHeaderReference Copy(OpenApiHeaderReference source) => CopyReference<OpenApiHeaderReference, IOpenApiHeader>(source);
    internal OpenApiLinkReference Copy(OpenApiLinkReference source) => CopyReference<OpenApiLinkReference, IOpenApiLink>(source);
    internal OpenApiMediaTypeReference Copy(OpenApiMediaTypeReference source) => CopyReference<OpenApiMediaTypeReference, IOpenApiMediaType>(source);
    internal OpenApiParameterReference Copy(OpenApiParameterReference source) => CopyReference<OpenApiParameterReference, IOpenApiParameter>(source);
    internal OpenApiPathItemReference Copy(OpenApiPathItemReference source) => CopyReference<OpenApiPathItemReference, IOpenApiPathItem>(source);
    internal OpenApiRequestBodyReference Copy(OpenApiRequestBodyReference source) => CopyReference<OpenApiRequestBodyReference, IOpenApiRequestBody>(source);
    internal OpenApiResponseReference Copy(OpenApiResponseReference source) => CopyReference<OpenApiResponseReference, IOpenApiResponse>(source);
    internal OpenApiSecuritySchemeReference Copy(OpenApiSecuritySchemeReference source) => CopyReference<OpenApiSecuritySchemeReference, IOpenApiSecurityScheme>(source);
    internal OpenApiTagReference Copy(OpenApiTagReference source) => CopyReference<OpenApiTagReference, IOpenApiTag>(source);

    private T CopyReference<T, TInterface>(T source)
        where T : class, IOpenApiReferenceable, IShallowCopyable<TInterface>
        where TInterface : class
    {
        if (GetExisting<T>(source) is { } existing)
        {
            return existing;
        }
        var target = (T)(object)source.CreateShallowCopy();
        Add(source, target);
        return target;
    }

    private void CopyJsonSchemaReference(JsonSchemaReference source, JsonSchemaReference target)
    {
        target.Default = source.Default is not null ? CopyJsonNode(source.Default) : null;
        target.Examples = source.Examples?.Select(CopyJsonNode).ToList();
        target.Extensions = CopyExtensions(source.Extensions);
        target.Vocabulary = source.Vocabulary != null ? new Dictionary<string, bool>(source.Vocabulary) : null;
        target.Definitions = CopyMap(source.Definitions);
        target.AllOf = CopyList(source.AllOf);
        target.OneOf = CopyList(source.OneOf);
        target.AnyOf = CopyList(source.AnyOf);
        target.Not = source.Not is not null ? Copy(source.Not) : null;
        target.Required = source.Required != null ? new HashSet<string>(source.Required) : null;
        target.Items = source.Items is not null ? Copy(source.Items) : null;
        target.Contains = source.Contains is not null ? Copy(source.Contains) : null;
        target.Properties = CopyMap(source.Properties);
        target.PatternProperties = CopyMap(source.PatternProperties);
        target.AdditionalProperties = source.AdditionalProperties is not null ? Copy(source.AdditionalProperties) : null;
        target.Discriminator = source.Discriminator is not null ? Copy(source.Discriminator) : null;
        target.Example = source.Example is not null ? CopyJsonNode(source.Example) : null;
        target.Enum = source.Enum?.Select(CopyJsonNode).ToList();
        target.UnevaluatedPropertiesSchema = source.UnevaluatedPropertiesSchema is not null ? Copy(source.UnevaluatedPropertiesSchema) : null;
        target.ExternalDocs = source.ExternalDocs is not null ? Copy(source.ExternalDocs) : null;
        target.Xml = source.Xml is not null ? Copy(source.Xml) : null;
        target.UnrecognizedKeywords = source.UnrecognizedKeywords?.ToDictionary(static x => x.Key, x => CopyJsonNode(x.Value));
        target.DependentRequired = source.DependentRequired?.ToDictionary(static x => x.Key, static x => new HashSet<string>(x.Value));
        target.ContentSchema = source.ContentSchema is not null ? Copy(source.ContentSchema) : null;
        target.PropertyNames = source.PropertyNames is not null ? Copy(source.PropertyNames) : null;
        target.DependentSchemas = CopyMap(source.DependentSchemas);
        target.If = source.If is not null ? Copy(source.If) : null;
        target.Then = source.Then is not null ? Copy(source.Then) : null;
        target.Else = source.Else is not null ? Copy(source.Else) : null;
    }

    private IList<T>? CopyList<T>(IEnumerable<T>? source)
        where T : class
        => source?.Select(x => CopyDynamic(x)).ToList();

    private Dictionary<string, T>? CopyMap<T>(IDictionary<string, T>? source)
        where T : class
        => source?.ToDictionary(static x => x.Key, x => CopyDynamic(x.Value));

    private ISet<OpenApiTag> CopyTags(ISet<OpenApiTag> source) => source switch
    {
        SortedSet<OpenApiTag> sortedSet => new SortedSet<OpenApiTag>(source.Select(Copy), sortedSet.Comparer),
#if NET
        ImmutableSortedSet<OpenApiTag> immutableSortedSet => ImmutableSortedSet.CreateRange(immutableSortedSet.KeyComparer, source.Select(Copy)),
#endif
        HashSet<OpenApiTag> hashSet => new HashSet<OpenApiTag>(source.Select(Copy), hashSet.Comparer),
        _ => new HashSet<OpenApiTag>(source.Select(Copy), OpenApiTagComparer.Instance)
    };

    private T CopyDynamic<T>(T source)
        where T : class
        => source switch
        {
            IOpenApiSchema schema => (T)Copy(schema),
            IOpenApiResponse response => (T)Copy(response),
            IOpenApiParameter parameter => (T)Copy(parameter),
            IOpenApiExample example => (T)Copy(example),
            IOpenApiRequestBody requestBody => (T)Copy(requestBody),
            IOpenApiHeader header => (T)Copy(header),
            IOpenApiMediaType mediaType => (T)Copy(mediaType),
            IOpenApiSecurityScheme securityScheme => (T)Copy(securityScheme),
            IOpenApiLink link => (T)Copy(link),
            IOpenApiCallback callback => (T)Copy(callback),
            IOpenApiPathItem pathItem => (T)Copy(pathItem),
            OpenApiServer server => (T)(object)Copy(server),
            OpenApiServerVariable serverVariable => (T)(object)Copy(serverVariable),
            OpenApiSecurityRequirement securityRequirement => (T)(object)Copy(securityRequirement),
            OpenApiEncoding encoding => (T)(object)Copy(encoding),
            RuntimeExpressionAnyWrapper wrapper => (T)(object)Copy(wrapper),
            OpenApiTag tag => (T)(object)Copy(tag),
            OpenApiTagReference tagReference => (T)(object)Copy(tagReference),
            _ => source
        };

    private static Uri CopyUri(Uri source) => new(source.OriginalString, UriKind.RelativeOrAbsolute);

    private static JsonNode CopyJsonNode(JsonNode source) => source.DeepClone();

    private Dictionary<string, IOpenApiExtension>? CopyExtensions(IDictionary<string, IOpenApiExtension>? source)
        => source?.ToDictionary(static x => x.Key, x => CopyExtension(x.Value));

    private IOpenApiExtension CopyExtension(IOpenApiExtension source) => source switch
    {
        JsonNodeExtension jsonNodeExtension => new JsonNodeExtension(CopyJsonNode(jsonNodeExtension.Node)),
        IDeepCopyable<IOpenApiExtension> deepCopyableExtension => deepCopyableExtension.CreateDeepCopy(),
        _ => source
    };

    private Dictionary<string, object>? CopyMetadata(IDictionary<string, object>? source)
        => source != null ? new Dictionary<string, object>(source) : null;

    private void SetReferenceHostDocument(OpenApiDocument hostDocument)
    {
        var resolver = new ReferenceHostDocumentSetter(hostDocument, overwriteExisting: true);
        var walker = new OpenApiWalker(resolver);
        walker.Walk(hostDocument);
    }
}

internal sealed class ReferenceEqualityComparer : IEqualityComparer<object>
{
    internal static readonly ReferenceEqualityComparer Instance = new();

    private ReferenceEqualityComparer()
    {
    }

    public new bool Equals(object? x, object? y) => ReferenceEquals(x, y);

    public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
}
