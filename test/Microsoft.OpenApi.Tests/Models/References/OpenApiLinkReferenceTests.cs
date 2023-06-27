// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Linq;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Readers;
using VerifyXunit;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models.References
{
    [Collection("DefaultSettings")]
    [UsesVerify]
    public class OpenApiLinkReferenceTests
    {
        private const string OpenApi = @"
openapi: 3.0.0
info:
  version: 0.0.0
  title: Links example
paths:
  /users:
    post:
      summary: Creates a user and returns the user ID
      operationId: createUser
      requestBody:
        required: true
        description: A JSON object that contains the user name and age.
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/User'
      responses:
        '201':
          description: Created
          content:
            application/json:
              schema:
                type: object
                properties:
                  id:
                    type: integer
                    format: int64
                    description: ID of the created user.
          links:
            GetUserByUserId:
              $ref: '#/components/links/GetUserByUserId'   # <---- referencing the link here
components:
  links:
    GetUserByUserId:
      operationId: getUser
      parameters:
        userId: '$response.body#/id'
      description: >
        The `id` value returned in the response can be used as
        the `userId` parameter in `GET /users/{userId}`.
";

        OpenApiLinkReference _openApiLinkReference;

        public OpenApiLinkReferenceTests()
        {
            var reader = new OpenApiStringReader();
            OpenApiDocument openApiDoc = reader.Read(OpenApi, out _);
            _openApiLinkReference = new("GetUserByUserId", openApiDoc)
            {
                Description = "Use the id returned as the userId in `GET /users/{userId}`"
            };
        }

        [Fact]
        public void ReferenceResolutionWorks()
        {
            // Assert
            Assert.Equal("Use the id returned as the userId in `GET /users/{userId}`", _openApiLinkReference.Description);
            Assert.Equal("getUser", _openApiLinkReference.OperationId);
            Assert.Equal("userId", _openApiLinkReference.Parameters.First().Key);
        }
    }
}
