using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Nodes;
using Xunit;

namespace Microsoft.OpenApi.Tests.Walkers;

public class OpenApiWalkerRichDocumentTests
{
    [Fact]
    public void WalkTraversesRichDocumentsAcrossComponentsWebhooksAndExtensions()
    {
        var document = CreateDocument();
        var visitor = new RichWalkerVisitor();
        var walker = new OpenApiWalker(visitor);

        walker.Walk(document);

        Assert.Contains("#/servers/0/variables/tenant", visitor.Locations);
        Assert.Contains("#/paths/~1pets/post/callbacks/onData/$request.body#~1callbackUrl/post/responses/202", visitor.Locations);
        Assert.Contains("#/webhooks/petCreated/post/requestBody/content/application~1json/schema", visitor.Locations);
        Assert.Contains("#/components/requestBodies/PetBody/content/application~1json/schema/properties/name", visitor.Locations);
        Assert.Contains("#/components/headers/RateLimit/examples/detailed", visitor.Locations);
        Assert.Contains("#/components/links/NextPage/server", visitor.Locations);
        Assert.Contains("#/components/examples/PetExample", visitor.Locations);
        Assert.Contains("#/externalDocs", visitor.Locations);
        Assert.Contains("referenceAt: #/paths/~1pets/post/tags/0", visitor.Locations);
        Assert.Contains("referenceAt: #/paths/~1pets/post/tags/1", visitor.Locations);
        Assert.Contains("#/paths/~1pets/post/x-operation", visitor.Locations);
        Assert.Contains("referenceAt: #/paths/~1pets/post/requestBody", visitor.Locations);
        Assert.Contains("referenceAt: #/paths/~1pets/post/responses/200/headers/x-rate-limit", visitor.Locations);
        Assert.Contains("referenceAt: #/paths/~1pets/post/responses/200/content/application~1json", visitor.Locations);
        Assert.Contains("referenceAt: #/paths/~1pets/post/security/0", visitor.Locations);
    }

    private static OpenApiDocument CreateDocument()
    {
        var document = new OpenApiDocument
        {
            Info = new OpenApiInfo
            {
                Title = "Pets",
                Version = "1.0.0",
                Contact = new OpenApiContact { Name = "Contoso" },
                License = new OpenApiLicense { Name = "MIT" }
            },
            Servers =
            [
                new OpenApiServer
                {
                    Url = "https://{tenant}.contoso.com",
                    Variables = new Dictionary<string, OpenApiServerVariable>
                    {
                        ["tenant"] = new()
                        {
                            Default = "prod"
                        }
                    }
                }
            ],
            ExternalDocs = new OpenApiExternalDocs
            {
                Url = new Uri("https://contoso.test/docs")
            },
            Tags = new HashSet<OpenApiTag>
            {
                new() { Name = "pets" },
                new() { Name = "store" }
            },
            Paths = new OpenApiPaths
            {
                ["/pets"] = new OpenApiPathItem
                {
                    Operations = new Dictionary<HttpMethod, OpenApiOperation>
                    {
                        [HttpMethod.Post] = new()
                        {
                            RequestBody = new OpenApiRequestBodyReference("PetBody"),
                            Responses = new OpenApiResponses
                            {
                                ["200"] = new OpenApiResponse
                                {
                                    Headers = new Dictionary<string, IOpenApiHeader>
                                    {
                                        ["x-rate-limit"] = new OpenApiHeaderReference("RateLimit")
                                    },
                                    Content = new Dictionary<string, IOpenApiMediaType>
                                    {
                                        ["application/json"] = new OpenApiMediaTypeReference("PetMediaType")
                                    }
                                }
                            },
                            Callbacks = new Dictionary<string, IOpenApiCallback>
                            {
                                ["onData"] = new OpenApiCallback
                                {
                                    PathItems = new Dictionary<RuntimeExpression, IOpenApiPathItem>
                                    {
                                        [RuntimeExpression.Build("$request.body#/callbackUrl")] = new OpenApiPathItem
                                        {
                                            Operations = new Dictionary<HttpMethod, OpenApiOperation>
                                            {
                                                [HttpMethod.Post] = new()
                                                {
                                                    Responses = new OpenApiResponses
                                                    {
                                                        ["202"] = new OpenApiResponse
                                                        {
                                                            Description = "Accepted"
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            Security =
                            [
                                new OpenApiSecurityRequirement
                                {
                                    [new OpenApiSecuritySchemeReference("oauth")] = ["pets.read"]
                                }
                            ],
                            Extensions = new Dictionary<string, IOpenApiExtension>
                            {
                                ["x-operation"] = new JsonNodeExtension("tracked")
                            }
                        }
                    }
                }
            },
            Webhooks = new Dictionary<string, IOpenApiPathItem>
            {
                ["petCreated"] = new OpenApiPathItem
                {
                    Operations = new Dictionary<HttpMethod, OpenApiOperation>
                    {
                        [HttpMethod.Post] = new()
                        {
                            RequestBody = new OpenApiRequestBody
                            {
                                Content = new Dictionary<string, IOpenApiMediaType>
                                {
                                    ["application/json"] = new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Type = JsonSchemaType.String
                                        }
                                    }
                                }
                            },
                            Responses = new OpenApiResponses
                            {
                                ["204"] = new OpenApiResponse
                                {
                                    Description = "No content"
                                }
                            }
                        }
                    }
                }
            },
            Components = new OpenApiComponents
            {
                RequestBodies = new Dictionary<string, IOpenApiRequestBody>
                {
                    ["PetBody"] = new OpenApiRequestBody
                    {
                        Required = true,
                        Content = new Dictionary<string, IOpenApiMediaType>
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Schema = new OpenApiSchema
                                {
                                    Type = JsonSchemaType.Object,
                                    Properties = new Dictionary<string, IOpenApiSchema>
                                    {
                                        ["name"] = new OpenApiSchema { Type = JsonSchemaType.String }
                                    },
                                    AdditionalProperties = new OpenApiSchema { Type = JsonSchemaType.String },
                                    Not = new OpenApiSchema { Type = JsonSchemaType.Boolean },
                                    AllOf = [new OpenApiSchemaReference("Pet")],
                                    AnyOf = [new OpenApiSchemaReference("Pet")],
                                    OneOf = [new OpenApiSchemaReference("Cat")],
                                    Discriminator = new OpenApiDiscriminator
                                    {
                                        PropertyName = "kind",
                                        Mapping = new Dictionary<string, OpenApiSchemaReference>
                                        {
                                            ["cat"] = new("Cat")
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                Headers = new Dictionary<string, IOpenApiHeader>
                {
                    ["RateLimit"] = new OpenApiHeader
                    {
                        Schema = new OpenApiSchema { Type = JsonSchemaType.Integer },
                        Example = JsonValue.Create(100),
                        Examples = new Dictionary<string, IOpenApiExample>
                        {
                            ["detailed"] = new OpenApiExample
                            {
                                Value = JsonValue.Create(200)
                            }
                        }
                    }
                },
                Links = new Dictionary<string, IOpenApiLink>
                {
                    ["NextPage"] = new OpenApiLink
                    {
                        Server = new OpenApiServer
                        {
                            Url = "https://next.contoso.com"
                        }
                    }
                },
                Examples = new Dictionary<string, IOpenApiExample>
                {
                    ["PetExample"] = new OpenApiExample
                    {
                        Value = JsonNode.Parse("""{"name":"Fluffy"}""")
                    }
                },
                MediaTypes = new Dictionary<string, IOpenApiMediaType>
                {
                    ["PetMediaType"] = new OpenApiMediaTypeReference("PetMediaType")
                },
                SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
                {
                    ["oauth"] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2
                    }
                },
                Schemas = new Dictionary<string, IOpenApiSchema>
                {
                    ["Pet"] = new OpenApiSchema
                    {
                        Type = JsonSchemaType.Object
                    },
                    ["Cat"] = new OpenApiSchema
                    {
                        Type = JsonSchemaType.Object
                    }
                }
            }
        };

        document.Paths["/pets"].Operations[HttpMethod.Post].Tags = new HashSet<OpenApiTagReference>
        {
            new("pets", document),
            new("store", document)
        };

        return document;
    }

    private sealed class RichWalkerVisitor : OpenApiVisitorBase
    {
        public List<string> Locations { get; } = [];

        public override void Visit(OpenApiExternalDocs externalDocs) => Locations.Add(PathString);
        public override void Visit(OpenApiServer server) => Locations.Add(PathString);
        public override void Visit(OpenApiServerVariable serverVariable) => Locations.Add(PathString);
        public override void Visit(IOpenApiRequestBody requestBody) => Locations.Add(PathString);
        public override void Visit(IOpenApiResponse response) => Locations.Add(PathString);
        public override void Visit(IOpenApiMediaType mediaType) => Locations.Add(PathString);
        public override void Visit(IOpenApiSchema schema) => Locations.Add(PathString);
        public override void Visit(IOpenApiCallback callback) => Locations.Add(PathString);
        public override void Visit(IOpenApiLink link) => Locations.Add(PathString);
        public override void Visit(IOpenApiExample example) => Locations.Add(PathString);
        public override void Visit(IOpenApiExtension extension) => Locations.Add(PathString);
        public override void Visit(JsonNode node) => Locations.Add(PathString);
        public override void Visit(OpenApiTag tag) => Locations.Add(PathString);
        public override void Visit(OpenApiTagReference tag) => Locations.Add(PathString);
        public override void Visit(IOpenApiReferenceHolder referenceable) => Locations.Add("referenceAt: " + PathString);
    }
}
