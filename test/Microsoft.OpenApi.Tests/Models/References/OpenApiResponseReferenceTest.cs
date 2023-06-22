using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Readers;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models.References
{
    public class OpenApiResponseReferenceTest
    {
        private const string OpenApi = @"
openapi: 3.0.3
info:
  title: Sample API
  version: 1.0.0

paths:
  /ping:
    get:
      responses:
        '200':
          $ref: '#/components/responses/OkResponse'

components:
  responses:
    OkResponse:
      description: OK
      content:
        text/plain:
          schema:
            $ref: '#/components/schemas/Pong'
";

        OpenApiResponseReference _openApiResponseReference;

        public OpenApiResponseReferenceTest()
        {
            var reader = new OpenApiStringReader();
            OpenApiDocument openApiDoc = reader.Read(OpenApi, out _);
            _openApiResponseReference = new("OkResponse", openApiDoc)
            {
                Description = "OK response"
            };
        }

        [Fact]
        public void ReferenceResolutionWorks()
        {
            // Assert
            Assert.Equal("OK response", _openApiResponseReference.Description);
            Assert.Equal("text/plain", _openApiResponseReference.Content.First().Key);
        }
    }
}
