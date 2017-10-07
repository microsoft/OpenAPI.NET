using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi;
using Xunit;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Writers;

namespace OpenApiTests
{
    public class DownGradeTests
    {
        public void SimpleTest()
        {
            var stream = this.GetType().Assembly.GetManifestResourceStream("OpenApiTests.Samples.Simplest.yaml");

            var openApiDoc = OpenApiParser.Parse(stream).OpenApiDocument;

            var outputStream = new MemoryStream();
            openApiDoc.Save(outputStream, new OpenApiV2Writer());


        }

        [Fact]
        public void EmptyTest()
        {
            var openApiDoc = new OpenApiDocument();

            JObject jObject = ExportV2ToJObject(openApiDoc);

            Assert.Equal("2.0", jObject["swagger"]);
            Assert.NotNull(jObject["info"]);

        }


        [Fact]
        public void HostTest()
        {
            var openApiDoc = new OpenApiDocument();
            openApiDoc.Servers.Add(new Server() { Url = "http://example.org/api" });
            openApiDoc.Servers.Add(new Server() { Url = "https://example.org/api" });

            JObject jObject = ExportV2ToJObject(openApiDoc);

            Assert.Equal("example.org", (string)jObject["host"]);
            Assert.Equal("/api", (string)jObject["basePath"]);

        }

        [Fact]
        public void TestConsumes()
        {
            var openApiDoc = new OpenApiDocument();
            var pathItem = new PathItem();
            var operation = new Operation
            {
                RequestBody = new RequestBody
                {
                    Content = new Dictionary<string,MediaType>() {
                        { "application/vnd.collection+json", new MediaType
                                {
                                }
                        }
                    }
                }, 
                Responses = new Dictionary<string,Response> { { "200", new Response() {
                    Description = "Success"
                } } }
            };
            pathItem.AddOperation(OperationType.Get, operation);
            openApiDoc.Paths.PathItems.Add("/resource", pathItem);

            JObject jObject = ExportV2ToJObject(openApiDoc);

            Assert.Equal("application/vnd.collection+json", (string)jObject["paths"]["/resource"]["get"]["consumes"][0]);

        }

        [Fact]
        public void TestRequestBody()
        {
            var openApiDoc = new OpenApiDocument();
            var pathItem = new PathItem();
            var operation = new Operation
            {
                RequestBody = new RequestBody
                {
                    Content = new Dictionary<string, MediaType>() {
                        { "application/vnd.collection+json", new MediaType
                                {
                                    Schema = new Schema
                                    {
                                        Type = "string",
                                        MaxLength = 100
                                    }
                                }
                        }
                    }
                },
                Responses = new Dictionary<string, Response> { { "200", new Response() {
                    Description = "Success"
                } } }
            };
            pathItem.AddOperation(OperationType.Post, operation);
            openApiDoc.Paths.PathItems.Add("/resource", pathItem);

            JObject jObject = ExportV2ToJObject(openApiDoc);

            var bodyparam = jObject["paths"]["/resource"]["post"]["parameters"][0];
            Assert.Equal("body", (string)bodyparam["in"]);
            Assert.Equal("string", (string)bodyparam["schema"]["type"]);
            Assert.Equal("100", (string)bodyparam["schema"]["maxLength"]);

        }

        [Fact]
        public void TestProduces()
        {
            var openApiDoc = new OpenApiDocument();
            var pathItem = new PathItem();
            var operation = new Operation
            {
                Responses = new Dictionary<string, Response> { { "200", new Response() {
                    Description = "Success",
                    Content = new Dictionary<string, MediaType>() {
                        { "application/vnd.collection+json", new MediaType
                                {
                                }
                        },
                        { "text/plain", null }
                    }

                } } }
            };
            pathItem.AddOperation(OperationType.Get, operation);
            openApiDoc.Paths.PathItems.Add("/resource", pathItem);

            JObject jObject = ExportV2ToJObject(openApiDoc);

            Assert.Equal("application/vnd.collection+json", (string)jObject["paths"]["/resource"]["get"]["produces"][0]);
            Assert.Equal("text/plain", (string)jObject["paths"]["/resource"]["get"]["produces"][1]);


        }

        [Fact]
        public void TestParameter()
        {
            var openApiDoc = new OpenApiDocument();
            var pathItem = new PathItem();
            var operation = new Operation
            {
                Parameters = new List<Parameter> {
                new Parameter {
                    Name = "param1",
                    In = InEnum.query,
                    Schema = new Schema
                    {
                        Type = "string"
                    }

                } },
                Responses = new Dictionary<string, Response> { { "200", new Response() {
                    Description = "Success"
                } } }
            };
            pathItem.AddOperation(OperationType.Get, operation);
            openApiDoc.Paths.AddPathItem("/resource", pathItem);

            JObject jObject = ExportV2ToJObject(openApiDoc);

            Assert.Equal("string", (string)jObject["paths"]["/resource"]["get"]["parameters"][0]["type"]);

        }

        private static JObject ExportV2ToJObject(OpenApiDocument openApiDoc)
        {
            var outputStream = new MemoryStream();
            openApiDoc.Save(outputStream, new OpenApiV2Writer((s) => new JsonParseNodeWriter(s)));
            outputStream.Position = 0;
            var json = new StreamReader(outputStream).ReadToEnd();
            var jObject = JObject.Parse(json);
            return jObject;
        }


    }
}
