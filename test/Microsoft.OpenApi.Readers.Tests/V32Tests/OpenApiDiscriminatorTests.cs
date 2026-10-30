// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V32Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiDiscriminatorTests
    {
        private const string SampleFolderPath = "V32Tests/Samples/OpenApiDiscriminator/";

        [Fact]
        public async Task ParseBasicDiscriminatorShouldSucceed()
        {
            // Arrange
            using var stream = Resources.GetStream(Path.Join(SampleFolderPath, "basicDiscriminator.yaml"));
            // Copy stream to MemoryStream
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream, TestContext.Current.CancellationToken);
            memoryStream.Position = 0;

            // Act
            var openApiDocument = new OpenApiDocument();
            var discriminator = OpenApiModelFactory.Load<OpenApiDiscriminator>(memoryStream, OpenApiSpecVersion.OpenApi3_2, OpenApiConstants.Yaml, openApiDocument, out var diagnostic, SettingsFixture.ReaderSettings);

            // Assert
            Assert.Equivalent(
               new OpenApiDiscriminator
               {
                   PropertyName = "pet_type",
                   Mapping = new Dictionary<string, OpenApiSchemaReference>
                   {
                            ["puppy"] = new OpenApiSchemaReference("Dog", openApiDocument),
                            ["kitten"] = new OpenApiSchemaReference("Cat" , openApiDocument, "https://gigantic-server.com/schemas/animals.json"),
                            ["monster"] = new OpenApiSchemaReference("monster" , openApiDocument, "https://gigantic-server.com/schemas/Monster/schema.json")
                    },
                   DefaultMapping = new OpenApiSchemaReference("Animal", openApiDocument)
               }, discriminator);
        }

        [Fact]
        public void ParseDiscriminatorWithBareSchemaNameMappingResolvesLocalTarget()
        {
            var openApi = """
            {
              "openapi": "3.2.0",
              "info": { "title": "Repro", "version": "1" },
              "paths": {},
              "components": {
                "schemas": {
                  "Dog": {
                    "type": "object",
                    "properties": { "bark": { "type": "boolean" } }
                  },
                  "Pet": {
                    "type": "object",
                    "required": [ "petType" ],
                    "properties": {
                      "petType": { "type": "string" }
                    },
                    "discriminator": {
                      "propertyName": "petType",
                      "mapping": {
                        "dog": "Dog"
                      },
                      "defaultMapping": "Dog"
                    }
                  }
                }
              }
            }
            """;

            var readResult = OpenApiDocument.Parse(openApi, "json");
            var doc = readResult.Document;
            Assert.NotNull(doc);
            Assert.Empty(readResult.Diagnostic.Errors);

            var petSchema = Assert.IsType<OpenApiSchema>(doc.Components.Schemas["Pet"]);
            Assert.NotNull(petSchema.Discriminator);

            var dogMapping = petSchema.Discriminator.Mapping["dog"];
            Assert.False(dogMapping.Reference.IsExternal);
            Assert.Null(dogMapping.Reference.ExternalResource);
            Assert.Equal("Dog", dogMapping.Reference.Id);

            var dogSchema = Assert.IsType<OpenApiSchema>(doc.Components.Schemas["Dog"]);
            Assert.Same(dogSchema, dogMapping.Target);

            Assert.NotNull(petSchema.Discriminator.DefaultMapping);
            Assert.False(petSchema.Discriminator.DefaultMapping.Reference.IsExternal);
            Assert.Same(dogSchema, petSchema.Discriminator.DefaultMapping.Target);
        }
    }
}
