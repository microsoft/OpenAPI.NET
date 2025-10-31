// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiHeaderTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiHeader/";

        [Fact]
        public async Task ParseBasicHeaderShouldSucceed()
        {
            // Arrange
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "basicHeader.yaml"));

            // Act
            var header = await OpenApiModelFactory.LoadAsync<OpenApiHeader>(stream, OpenApiSpecVersion.OpenApi3_0, new(), settings: SettingsFixture.ReaderSettings);

            // Assert
            Assert.Equivalent(
                new OpenApiHeader
                {
                    Description = "The number of allowed requests in the current period",
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Integer
                    }
                }, header);
        }

        [Fact]
        public async Task ParseHeaderWithContentShouldSucceed()
        {
            // Arrange
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "headerWithContent.yaml"));

            // Act
            var header = await OpenApiModelFactory.LoadAsync<OpenApiHeader>(stream, OpenApiSpecVersion.OpenApi3_0, new(), settings: SettingsFixture.ReaderSettings);

            // Assert
            Assert.Equivalent(
                new OpenApiHeader
                {
                    Description = "A complex header with content",
                    Content = new Dictionary<string, IOpenApiMediaType>()
                    {
                        ["application/json"] = new OpenApiMediaType()
                        {
                           Schema = new OpenApiSchema()
                           {
                                Type = JsonSchemaType.Object,
                                Properties = new Dictionary<string, IOpenApiSchema>()
                                {
                                    ["timestamp"] = new OpenApiSchema()
                                    {
                                        Type = JsonSchemaType.String,
                                        Format = "date-time"
                                    },
                                    ["value"] = new OpenApiSchema()
                                    {
                                        Type = JsonSchemaType.Integer
                                    }
                                }
                           }
                        }
                    }
                }, header);
        }

        [Fact]
        public async Task ParseHeaderWithMultipleContentTypesShouldSucceed()
        {
            // Arrange
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "headerWithMultipleContentTypes.yaml"));

            // Act
            var header = await OpenApiModelFactory.LoadAsync<OpenApiHeader>(stream, OpenApiSpecVersion.OpenApi3_0, new(), settings: SettingsFixture.ReaderSettings);

            // Assert
            Assert.Equivalent(
                new OpenApiHeader
                {
                    Description = "A header that accepts multiple content types",
                    Content = new Dictionary<string, IOpenApiMediaType>()
                    {
                        ["application/json"] = new OpenApiMediaType()
                        {
                           Schema = new OpenApiSchema()
                           {
                                Type = JsonSchemaType.Object,
                                Properties = new Dictionary<string, IOpenApiSchema>()
                                {
                                    ["data"] = new OpenApiSchema()
                                    {
                                        Type = JsonSchemaType.String
                                    }
                                }
                           }
                        },
                        ["text/plain"] = new OpenApiMediaType()
                        {
                           Schema = new OpenApiSchema()
                           {
                                Type = JsonSchemaType.String
                           }
                        }
                    }
                }, header);
        }

        [Fact]
        public async Task ParseHeaderWithStyleAndContentShouldPreferContent()
        {
            // Arrange
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "headerWithStyleAndContent.yaml"));

            // Act
            var header = await OpenApiModelFactory.LoadAsync<OpenApiHeader>(stream, OpenApiSpecVersion.OpenApi3_0, new(), settings: SettingsFixture.ReaderSettings);

            // Assert
            // Both content and style can be present, content takes precedence for serialization behavior
            Assert.NotNull(header.Content);
            Assert.Single(header.Content);
            Assert.True(header.Content.ContainsKey("application/json"));
            Assert.Equal(ParameterStyle.Simple, header.Style); // Style can still be present
        }
    }
}
