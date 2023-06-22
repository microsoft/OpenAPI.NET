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
    public class OpenApiTagReferenceTest
    {
        private const string OpenApi = @"openapi: 3.0.3
info:
  title: Sample API
  version: 1.0.0

paths:
  /users/{userId}:
    get:
      summary: Returns a user by ID.
      parameters:
        - name: userId
          in: path
          required: true
          description: The ID of the user to return.
          schema:
            type: integer
      responses:
        '200':
          description: A user object.
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/User'
        '404':
          description: The user was not found.
      tags:
        - $ref: '#/tags/user'
components:
  schemas:
    User:
      type: object
      properties:
        id:
          type: integer
        name:
          type: string
tags:
  - name: user
    description: Operations about users.
";

        readonly OpenApiTagReference _openApiTagReference;

        public OpenApiTagReferenceTest()
        {
            var reader = new OpenApiStringReader();
            OpenApiDocument openApiDoc = reader.Read(OpenApi, out _);
            _openApiTagReference = new("user", openApiDoc)
            {
                Description = "Users operations"
            };
        }

        [Fact]
        public void ReferenceResolutionWorks()
        {
            // Assert
            Assert.Equal("user", _openApiTagReference.Name);
            Assert.Equal("Users operations", _openApiTagReference.Description);
        }
    }
}
