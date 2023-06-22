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
    public class OpenApiExampleReferenceTests
    {
        private const string OpenApi = @"
openapi: 3.0.0
info:
  title: Sample API
  version: 1.0.0
paths:
  /users:
    get:
      summary: Get users
      responses:
        '200':
          description: Successful operation
          content:
            application/json:
              schema:
                type: array
                items:
                  $ref: '#/components/schemas/User'
              examples:
                - $ref: '#/components/examples/UserExample'
components:
  schemas:
    User:
      type: object
      properties:
        id:
          type: integer
        name:
          type: string
  examples:
    UserExample:
       summary: Example of a user
       value:
        - id: 1
          name: John Doe
";

        readonly OpenApiExampleReference _openApiExampleReference;

        public OpenApiExampleReferenceTests()
        {
            var reader = new OpenApiStringReader();
            OpenApiDocument openApiDoc = reader.Read(OpenApi, out _);
            _openApiExampleReference = new OpenApiExampleReference("UserExample", openApiDoc)
            {
                Summary = "Example of an admin user"
            };
        }

        [Fact]
        public void ReferenceResolutionWorks()
        {
            // Assert
            Assert.Equal("Example of an admin user", _openApiExampleReference.Summary);
            Assert.NotNull(_openApiExampleReference.Value);            
        }
    }    
}
