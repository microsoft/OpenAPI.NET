// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using Microsoft.OpenApi.Readers.YamlReaders;
using Microsoft.OpenApi.Writers;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests
{
    public class DowngradeTests
    {
        [Fact]
        public void EmptyTest()
        {
            var openApiDoc = new OpenApiDocument();

            var jObject = ExportV2ToJObject(openApiDoc);

            Assert.Equal("2.0", jObject["swagger"]);
            Assert.NotNull(jObject["info"]);
        }

        private static JObject ExportV2ToJObject(OpenApiDocument openApiDoc)
        {
            var outputStream = new MemoryStream();
            openApiDoc.Save(outputStream, new OpenApiV2Writer(s => new OpenApiJsonWriter(new StreamWriter(s))));
            outputStream.Position = 0;
            var json = new StreamReader(outputStream).ReadToEnd();
            var jObject = JObject.Parse(json);
            return jObject;
        }

        [Fact]
        public void HostTest()
        {
            var openApiDoc = new OpenApiDocument();
            openApiDoc.Servers.Add(new OpenApiServer {Url = "http://example.org/api"});
            openApiDoc.Servers.Add(new OpenApiServer {Url = "https://example.org/api"});

            var jObject = ExportV2ToJObject(openApiDoc);

            Assert.Equal("example.org", (string)jObject["host"]);
            Assert.Equal("/api", (string)jObject["basePath"]);
        }

        [Fact]
        public void SimpleTest()
        {
            var stream = GetType().Assembly.GetManifestResourceStream(typeof(DowngradeTests), "Samples.Simplest.yaml");

            var openApiDoc = new OpenApiStreamReader().Read(stream, out var context);

            var outputStream = new MemoryStream();
            openApiDoc.Save(outputStream, new OpenApiV2Writer());
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
                Responses = new Dictionary<string, OpenApiResponse>
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

            Assert.Equal(
                "application/vnd.collection+json",
                (string)jObject["paths"]["/resource"]["get"]["consumes"][0]);
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
                        In = ParameterLocation.query,
                        Schema = new OpenApiSchema
                        {
                            Type = "string"
                        }
                    }
                },
                Responses = new Dictionary<string, OpenApiResponse>
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

            Assert.Equal("string", (string)jObject["paths"]["/resource"]["get"]["parameters"][0]["type"]);
        }

        [Fact]
        public void TestProduces()
        {
            var openApiDoc = new OpenApiDocument();
            var pathItem = new OpenApiPathItem();
            var operation = new OpenApiOperation
            {
                Responses = new Dictionary<string, OpenApiResponse>
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

            Assert.Equal(
                "application/vnd.collection+json",
                (string)jObject["paths"]["/resource"]["get"]["produces"][0]);
            Assert.Equal("text/plain", (string)jObject["paths"]["/resource"]["get"]["produces"][1]);
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
                Responses = new Dictionary<string, OpenApiResponse>
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
            Assert.Equal("body", (string)bodyparam["in"]);
            Assert.Equal("string", (string)bodyparam["schema"]["type"]);
            Assert.Equal("100", (string)bodyparam["schema"]["maxLength"]);
        }
    }
}