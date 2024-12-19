// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiDiscriminatorTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiDiscriminator/";

        public OpenApiDiscriminatorTests()
        {
            OpenApiReaderRegistry.RegisterReader(OpenApiConstants.Yaml, new OpenApiYamlReader());
        }

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
            var discriminator = OpenApiModelFactory.Load<OpenApiDiscriminator>(memoryStream, OpenApiSpecVersion.OpenApi3_0, OpenApiConstants.Yaml, out var diagnostic);

            // Assert
            discriminator.Should().BeEquivalentTo(
                new OpenApiDiscriminator
                {
                    PropertyName = "pet_type",
                    Mapping =
                    {
                            ["puppy"] = "#/components/schemas/Dog",
                            ["kitten"] = "Cat"
                    }
                });
        }
    }
}
