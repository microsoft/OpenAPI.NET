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
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "basicDiscriminator.yaml"));
            // Copy stream to MemoryStream
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
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
    }
}
