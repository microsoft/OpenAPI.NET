// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiDiscriminatorTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiDiscriminator/";

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
            var discriminator = OpenApiModelFactory.Load<OpenApiDiscriminator>(memoryStream, OpenApiSpecVersion.OpenApi3_0, OpenApiConstants.Yaml, openApiDocument, out var diagnostic, SettingsFixture.ReaderSettings);

            // Assert
            Assert.Equivalent(
               new OpenApiDiscriminator
               {
                   PropertyName = "pet_type",
                   Mapping =
                    {
                            ["puppy"] = new OpenApiSchemaReference("Dog", openApiDocument),
                            ["monster"] = new OpenApiSchemaReference("schema.json" , openApiDocument, "https://gigantic-server.com/schemas/Monster/schema.json")
                    }
               }, discriminator);
        }
    }
}
