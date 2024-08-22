// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiMediaTypeTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiMediaType/";

        public OpenApiMediaTypeTests()
        {
            OpenApiReaderRegistry.RegisterReader("yaml", new OpenApiYamlReader());
        }

        [Fact]
        public void ParseMediaTypeWithExampleShouldSucceed()
        {
            // Act
            var mediaType = OpenApiModelFactory.Load<OpenApiMediaType>(Path.Combine(SampleFolderPath, "mediaTypeWithExample.yaml"), OpenApiSpecVersion.OpenApi3_0, out var diagnostic);

            // Assert
            mediaType.Should().BeEquivalentTo(
                new OpenApiMediaType
                {
                    Example = 5,
                    Schema = new()
                    {
                        Type = "number",
                        Format = "float"
                    }
                }, options => options.IgnoringCyclicReferences()
                .Excluding(m => m.Example.Parent)
                );
        }

        [Fact]
        public void ParseMediaTypeWithExamplesShouldSucceed()
        {
            // Act
            var mediaType = OpenApiModelFactory.Load<OpenApiMediaType>(Path.Combine(SampleFolderPath, "mediaTypeWithExamples.yaml"), OpenApiSpecVersion.OpenApi3_0, out var diagnostic);

            // Assert
            mediaType.Should().BeEquivalentTo(
                new OpenApiMediaType
                {
                    Examples =
                    {
                        ["example1"] = new()
                        {
                            Value = 5
                        },
                        ["example2"] = new()
                        {
                            Value = 7.5
                        }
                    },
                    Schema = new()
                    {
                        Type = "number",
                        Format = "float"
                    }
                }, options => options.IgnoringCyclicReferences()
                .Excluding(m => m.Examples["example1"].Value.Parent)
                .Excluding(m => m.Examples["example2"].Value.Parent));
        }
    }
}
