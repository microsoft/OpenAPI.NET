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
    public class OpenApiHeaderReferenceTests
    {
        private const string OpenApi= @"
openapi: 3.0.0
info:
  title: Sample API
  version: 1.0.0
paths:
  /users:
    post:
      summary: Create a post
      responses:
        '201':
          description: Post created successfully
          headers:
            Location:
              $ref: '#/components/headers/LocationHeader'
components:
  headers:
    LocationHeader:
      description: The URL of the newly created post
      schema:
        type: string
";

        readonly OpenApiHeaderReference _openApiHeaderReference;

        public OpenApiHeaderReferenceTests()
        {
            var reader = new OpenApiStringReader();
            OpenApiDocument openApiDoc = reader.Read(OpenApi, out _);
            _openApiHeaderReference = new OpenApiHeaderReference("LocationHeader", openApiDoc)
            {
                Description = "Location of the created post"
            };
        }

        [Fact]
        public void ReferenceResolutionWorks()
        {
            // Assert
            Assert.Equal("Location of the created post", _openApiHeaderReference.Description);
        }
    }
}
