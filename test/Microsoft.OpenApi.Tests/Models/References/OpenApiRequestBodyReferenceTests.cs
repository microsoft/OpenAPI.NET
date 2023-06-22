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
    public class OpenApiRequestBodyReferenceTests
    {
        private const string OpenApi = @"
openapi: 3.0.3
info:
  title: Sample API
  version: 1.0.0

paths:
  /users:
    post:
      summary: Create a user
      requestBody:
        $ref: '#/components/requestBodies/UserRequest'  # <---- referencing the requestBody here
      responses:
        '201':
          description: User created
          
components:
  requestBodies:
    UserRequest:
      description: User creation request body
      content:
        application/json:
          schema:
            $ref: '#/components/schemas/UserSchema'

  schemas:
    UserSchema:
      type: object
      properties:
        name:
          type: string
        email:
          type: string
";

        OpenApiRequestBodyReference _openApiRequestBodyReference;

        public OpenApiRequestBodyReferenceTests()
        {
            var reader = new OpenApiStringReader();
            OpenApiDocument openApiDoc = reader.Read(OpenApi, out _);
            _openApiRequestBodyReference = new("", openApiDoc)
            {
                Description = "User request body"
            };
        }

        [Fact]
        public void ReferenceResolutionWorks()
        {
            // Assert
            Assert.Equal("User request body", _openApiRequestBodyReference.Description);
            Assert.Equal("application/json", _openApiRequestBodyReference.Content.First().Key);
        }
    }
}
