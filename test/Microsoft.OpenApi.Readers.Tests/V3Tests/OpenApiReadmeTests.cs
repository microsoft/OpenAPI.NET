using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    public class OpenApiReadmeTests
    {

        [Fact]
        public void CreatingADocumentTest()
        {
            var document = new OpenApiDocument
            {
                Info = new OpenApiInfo
                {
                    Version = "1.0.0",
                    Title = "Swagger Petstore (Simple)",
                },
                Servers = new List<OpenApiServer>
                {
                    new OpenApiServer { Url = "http://petstore.swagger.io/api" }
                },
                Paths = new OpenApiPaths
                {
                    ["/pets"] = new OpenApiPathItem
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>
                        {
                            [OperationType.Get] = new OpenApiOperation
                            {
                                Description = "Returns all pets from the system that the user has access to",
                                Responses = new OpenApiResponses
                                {
                                    ["200"] = new OpenApiResponse
                                    {
                                        Description = "OK"
                                    }
                                }
                            }
                        }
                    }
                }
            };

            Assert.NotNull(document);            
        }

        [Fact]
        public async Task ReadingAndWritingADocument()
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://raw.githubusercontent.com/OAI/OpenAPI-Specification/")
            };

            var stream = await httpClient.GetStreamAsync("master/examples/v3.0/petstore.yaml");

            // Read V3 as YAML
            var openApiDocument = new OpenApiStreamReader().Read(stream, out var diagnostics);

            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);

            // Write V2 as JSON
            openApiDocument.SerializeAsV2(writer);

            outputStringWriter.Flush();
            var output = outputStringWriter.GetStringBuilder().ToString();

            Assert.Empty(diagnostics.Errors);
            Assert.NotNull(openApiDocument);
            Assert.NotEmpty(output);

        }
    }
}
