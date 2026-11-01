// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Tests;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiSubschemaReferenceTests
    {
        private const string Document = @"
openapi: 3.0.3
info:
  title: Subschema references
  version: 1.0.0
paths: {}
components:
  schemas:
    User:
      type: object
      properties:
        name:
          type: string
        address:
          type: object
          properties:
            city:
              type: string
    name:
      type: integer
    UserName:
      $ref: '#/components/schemas/User/properties/name'
    UserCity:
      $ref: '#/components/schemas/User/properties/address/properties/city'
";

        [Fact]
        public void ReferenceToComponentPropertyResolvesToThatProperty()
        {
            var document = OpenApiDocument.Parse(Document, "yaml", SettingsFixture.ReaderSettings).Document;

            var userName = Assert.IsType<OpenApiSchemaReference>(document.Components.Schemas["UserName"]);

            Assert.NotNull(userName.Target);
            Assert.Equal(JsonSchemaType.String, userName.Target.Type);
            Assert.Equal("#/components/schemas/User/properties/name", userName.Reference.ReferenceV3);
        }

        [Fact]
        public void ReferenceToNestedComponentPropertyResolvesToThatProperty()
        {
            var document = OpenApiDocument.Parse(Document, "yaml", SettingsFixture.ReaderSettings).Document;

            var userCity = Assert.IsType<OpenApiSchemaReference>(document.Components.Schemas["UserCity"]);

            Assert.NotNull(userCity.Target);
            Assert.Equal(JsonSchemaType.String, userCity.Target.Type);
            Assert.Equal("#/components/schemas/User/properties/address/properties/city", userCity.Reference.ReferenceV3);
        }
    }
}
