using System.Collections.Generic;
using System.Net.Http;
using Xunit;

namespace Microsoft.OpenApi.Tests.Services;

public class CopyReferencesTests
{
    [Fact]
    public void VisitCopiesResolvedReferenceTargetsIntoMatchingComponentCollections()
    {
        var callback = new OpenApiCallback
        {
            PathItems = new Dictionary<RuntimeExpression, IOpenApiPathItem>
            {
                [RuntimeExpression.Build("{$request.body#/callbackUrl}")] = new OpenApiPathItem
                {
                    Operations = new Dictionary<HttpMethod, OpenApiOperation>
                    {
                        [HttpMethod.Post] = new()
                        {
                            Responses = new OpenApiResponses
                            {
                                ["200"] = new OpenApiResponse { Description = "ok" }
                            }
                        }
                    }
                }
            }
        };
        var link = new OpenApiLink { OperationId = "getUser" };
        var requestBody = new OpenApiRequestBody
        {
            Required = true,
            Content = new Dictionary<string, IOpenApiMediaType>
            {
                ["application/json"] = new OpenApiMediaType()
            }
        };
        var securityScheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            Name = "api-key",
            In = ParameterLocation.Header
        };

        var source = new OpenApiDocument
        {
            Components = new OpenApiComponents
            {
                Callbacks = new Dictionary<string, IOpenApiCallback> { ["callback"] = callback },
                Links = new Dictionary<string, IOpenApiLink> { ["link"] = link },
                RequestBodies = new Dictionary<string, IOpenApiRequestBody> { ["body"] = requestBody },
                SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme> { ["scheme"] = securityScheme }
            }
        };
        source.RegisterComponents();
        var target = new OpenApiDocument();
        var visitor = new CopyReferences(target);

        visitor.Visit((IOpenApiReferenceHolder)new OpenApiCallbackReference("callback", source));
        visitor.Visit((IOpenApiReferenceHolder)new OpenApiLinkReference("link", source));
        visitor.Visit((IOpenApiReferenceHolder)new OpenApiRequestBodyReference("body", source));
        visitor.Visit((IOpenApiReferenceHolder)new OpenApiSecuritySchemeReference("scheme", source));

        Assert.Same(callback, Assert.Single(visitor.Components.Callbacks).Value);
        Assert.Same(link, Assert.Single(visitor.Components.Links).Value);
        Assert.Same(requestBody, Assert.Single(visitor.Components.RequestBodies).Value);
        Assert.Same(securityScheme, Assert.Single(visitor.Components.SecuritySchemes).Value);

        Assert.NotNull(target.Components);
        Assert.NotNull(target.Components.Callbacks);
        Assert.NotNull(target.Components.Links);
        Assert.NotNull(target.Components.RequestBodies);
        Assert.NotNull(target.Components.SecuritySchemes);
    }

    [Fact]
    public void VisitCopiesSchemaTargetsOnceAndIgnoresMissingReferences()
    {
        var schema = new OpenApiSchema { Type = JsonSchemaType.Object };
        var source = new OpenApiDocument
        {
            Components = new OpenApiComponents
            {
                Schemas = new Dictionary<string, IOpenApiSchema> { ["Pet"] = schema }
            }
        };
        source.RegisterComponents();
        var visitor = new CopyReferences(new OpenApiDocument());

        visitor.Visit((IOpenApiReferenceHolder)new OpenApiLinkReference("missing", source));

        var schemaReference = new OpenApiSchemaReference("Pet", source);
        visitor.Visit((IOpenApiSchema)schemaReference);
        visitor.Visit((IOpenApiSchema)schemaReference);

        Assert.Null(visitor.Components.Links);
        var copiedSchema = Assert.Single(visitor.Components.Schemas);
        Assert.Equal("Pet", copiedSchema.Key);
        Assert.Same(schema, copiedSchema.Value);
    }
}
