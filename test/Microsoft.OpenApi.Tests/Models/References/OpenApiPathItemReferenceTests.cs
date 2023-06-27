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
    public class OpenApiPathItemReferenceTests
    {
        private const string OpenApi = @"
openapi: 3.0.0
info:
  title: Sample API
  version: 1.0.0
paths:
  /users:
    $ref: '#/components/pathItems/userPathItem'

components:
  pathItems:
    userPathItem:
      get:
        summary: Get users
        responses:
          200:
            description: Successful operation
      post:
        summary: Create a user
        responses:
          201:
            description: User created successfully
      delete:
        summary: Delete a user
        responses:
          204:
            description: User deleted successfully
";

        readonly OpenApiPathItemReference _openApiPathItemReference;

        public OpenApiPathItemReferenceTests()
        {
            var reader = new OpenApiStringReader();
            OpenApiDocument openApiDoc = reader.Read(OpenApi, out _);
            _openApiPathItemReference = new OpenApiPathItemReference("userPathItem", openApiDoc)
            {
                Description = "User path item description"
            };
        }

        [Fact]
        public void ReferenceResolutionWorks()
        {
            // Assert
            Assert.Equal(3, _openApiPathItemReference.Operations.Count);
            Assert.Equal("User path item description", _openApiPathItemReference.Description);
            Assert.Equal(new OperationType[] { OperationType.Get, OperationType.Post, OperationType.Delete }, _openApiPathItemReference.Operations.Select(o => o.Key));            
        }
    }
}
