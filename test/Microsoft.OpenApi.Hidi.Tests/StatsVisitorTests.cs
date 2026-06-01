using System;
using System.Collections.Generic;
using System.Net.Http;
using Xunit;

namespace Microsoft.OpenApi.Hidi.Tests;

public class StatsVisitorTests
{
    [Fact]
    public void GetStatisticsReportReflectsVisitedElements()
    {
        var document = new OpenApiDocument
        {
            Paths = new()
            {
                ["/pets"] = new OpenApiPathItem
                {
                    Operations = new()
                    {
                        [HttpMethod.Post] = new OpenApiOperation
                        {
                            Parameters =
                            [
                                new OpenApiParameter
                                {
                                    Name = "expand",
                                    In = ParameterLocation.Query,
                                    Schema = new OpenApiSchema { Type = JsonSchemaType.String }
                                }
                            ],
                            RequestBody = new OpenApiRequestBody
                            {
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
                                            }
                                        }
                                    }
                                }
                            },
                            Responses = new OpenApiResponses
                            {
                                ["200"] = new OpenApiResponse
                                {
                                    Headers = new Dictionary<string, IOpenApiHeader>
                                    {
                                        ["x-rate-limit"] = new OpenApiHeader
                                        {
                                            Schema = new OpenApiSchema { Type = JsonSchemaType.Integer }
                                        }
                                    },
                                    Links = new Dictionary<string, IOpenApiLink>
                                    {
                                        ["next"] = new OpenApiLink()
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
                                            Operations = new()
                                            {
                                                [HttpMethod.Post] = new OpenApiOperation
                                                {
                                                    Responses = new OpenApiResponses
                                                    {
                                                        ["202"] = new OpenApiResponse { Description = "Accepted" }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        var visitor = new StatsVisitor();
        new OpenApiWalker(visitor).Walk(document);
        var report = visitor.GetStatisticsReport();

        Assert.Equal(2, visitor.PathItemCount);
        Assert.Equal(2, visitor.OperationCount);
        Assert.Equal(1, visitor.ParameterCount);
        Assert.Equal(1, visitor.RequestBodyCount);
        Assert.Equal(2, visitor.ResponseCount);
        Assert.Equal(1, visitor.LinkCount);
        Assert.Equal(1, visitor.CallbackCount);
        Assert.Equal(4, visitor.SchemaCount);
        Assert.Contains("Path Items: 2", report, StringComparison.Ordinal);
        Assert.Contains("Callbacks: 1", report, StringComparison.Ordinal);
        Assert.Contains("Schemas: 4", report, StringComparison.Ordinal);
    }
}
