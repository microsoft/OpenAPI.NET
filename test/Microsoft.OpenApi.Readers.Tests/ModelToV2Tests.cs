// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests
{
    // TODO: This needs to be moved to each individual model test in OpenApi.Tests
    public class ModelToV2Tests
    {
        [Fact]
        public void EmptyTest()
        {
            var openApiDoc = new OpenApiDocument
            {
                SpecVersion = OpenApiConstants.SwaggerVersion
            };

            var jObject = ExportV2ToJObject(openApiDoc);

            jObject["swagger"].ToString().Should().Be("2.0");
            jObject["info"].Should().NotBeNull();
        }

        private static JObject ExportV2ToJObject(OpenApiDocument openApiDoc)
        {
            var outputStream = new MemoryStream();
            openApiDoc.SerializeAsJson(outputStream, OpenApiSpecVersion.OpenApi2_0);
            outputStream.Position = 0;
            var json = new StreamReader(outputStream).ReadToEnd();
            var jObject = JObject.Parse(json);
            return jObject;
        }

        [Fact]
        public void HostTest()
        {
            var openApiDoc = new OpenApiDocument
            {
                Servers = new List<OpenApiServer>()
            };

            openApiDoc.Servers.Add(new OpenApiServer {Url = "http://example.org/api"});
            openApiDoc.Servers.Add(new OpenApiServer {Url = "https://example.org/api"});

            var jObject = ExportV2ToJObject(openApiDoc);

            ((string)jObject["host"]).Should().Be("example.org");
            ((string)jObject["basePath"]).Should().Be("/api");
            jObject["schemes"].ToObject<List<string>>().Should().Equal(new List<string> {"http", "https"});
        }

        [Fact]
        public void SimpleTest()
        {
            var stream = GetType().Assembly.GetManifestResourceStream(typeof(ModelToV2Tests), "Samples.Simplest.yaml");

            var openApiDoc = new OpenApiStreamReader().Read(stream, out var context);

            var outputStream = new MemoryStream();
            openApiDoc.SerializeAsJson(outputStream, OpenApiSpecVersion.OpenApi2_0);
        }

        [Fact]
        public void TestConsumes()
        {
            var openApiDoc = new OpenApiDocument();
            var pathItem = new OpenApiPathItem();
            var operation = new OpenApiOperation
            {
                RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        {
                            "application/vnd.collection+json", new OpenApiMediaType()
                        }
                    }
                },
                Responses = new OpenApiResponses
                {
                    {
                        "200", new OpenApiResponse
                        {
                            Description = "Success"
                        }
                    }
                }
            };
            pathItem.AddOperation(OperationType.Get, operation);
            openApiDoc.Paths.Add("/resource", pathItem);

            var jObject = ExportV2ToJObject(openApiDoc);

            ((string)jObject["paths"]["/resource"]["get"]["consumes"][0]).Should()
                .Be("application/vnd.collection+json");
        }

        [Fact]
        public void TestParameter()
        {
            var openApiDoc = new OpenApiDocument();
            var pathItem = new OpenApiPathItem();
            var operation = new OpenApiOperation
            {
                Parameters = new List<OpenApiParameter>
                {
                    new OpenApiParameter
                    {
                        Name = "param1",
                        In = ParameterLocation.Query,
                        Schema = new OpenApiSchema
                        {
                            Type = "string"
                        }
                    }
                },
                Responses = new OpenApiResponses
                {
                    {
                        "200", new OpenApiResponse
                        {
                            Description = "Success"
                        }
                    }
                }
            };
            pathItem.AddOperation(OperationType.Get, operation);
            openApiDoc.Paths.Add("/resource", pathItem);

            var jObject = ExportV2ToJObject(openApiDoc);

            ((string)jObject["paths"]["/resource"]["get"]["parameters"][0]["type"]).Should().Be("string");
        }

        [Fact]
        public void TestProduces()
        {
            var openApiDoc = new OpenApiDocument();
            var pathItem = new OpenApiPathItem();
            var operation = new OpenApiOperation
            {
                Responses = new OpenApiResponses
                {
                    {
                        "200", new OpenApiResponse
                        {
                            Description = "Success",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                {
                                    "application/vnd.collection+json", new OpenApiMediaType()
                                },
                                {"text/plain", null}
                            }
                        }
                    }
                }
            };
            pathItem.AddOperation(OperationType.Get, operation);
            openApiDoc.Paths.Add("/resource", pathItem);

            var jObject = ExportV2ToJObject(openApiDoc);

            ((string)jObject["paths"]["/resource"]["get"]["produces"][0])
                .Should()
                .Be("application/vnd.collection+json");
            ((string)jObject["paths"]["/resource"]["get"]["produces"][1]).Should().Be("text/plain");
        }

        [Fact]
        public void TestRequestBody()
        {
            var openApiDoc = new OpenApiDocument();
            var pathItem = new OpenApiPathItem();
            var operation = new OpenApiOperation
            {
                RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        {
                            "application/vnd.collection+json", new OpenApiMediaType
                            {
                                Schema = new OpenApiSchema
                                {
                                    Type = "string",
                                    MaxLength = 100
                                }
                            }
                        }
                    }
                },
                Responses = new OpenApiResponses
                {
                    {
                        "200", new OpenApiResponse
                        {
                            Description = "Success"
                        }
                    }
                }
            };

            pathItem.AddOperation(OperationType.Post, operation);
            openApiDoc.Paths.Add("/resource", pathItem);

            var jObject = ExportV2ToJObject(openApiDoc);

            var bodyparam = jObject["paths"]["/resource"]["post"]["parameters"][0];

            ((string)bodyparam["in"]).Should().Be("body");
            ((string)bodyparam["schema"]["type"]).Should().Be("string");
            ((string)bodyparam["schema"]["maxLength"]).Should().Be("100");
        }
    }
}