// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiResponsesTests
    {
        [Fact]
        public void CopyConstructorCopiesExtensions()
        {
            // Arrange
            var responses = new OpenApiResponses
            {
                ["200"] = new OpenApiResponse { Description = "OK" },
                Extensions = new Dictionary<string, IOpenApiExtension>
                {
                    ["x-response-notes"] = new JsonNodeExtension("all-responses-note"),
                    ["x-default-code"] = new JsonNodeExtension("200"),
                }
            };

            // Act
            var copy = new OpenApiResponses(responses);

            // Assert
            Assert.NotNull(copy.Extensions);
            Assert.Equal(2, copy.Extensions.Count);
            Assert.True(copy.Extensions.ContainsKey("x-response-notes"));
            Assert.True(copy.Extensions.ContainsKey("x-default-code"));
        }

        [Fact]
        public void CopyConstructorIsolatesExtensionsDictionary()
        {
            // Arrange
            var responses = new OpenApiResponses
            {
                Extensions = new Dictionary<string, IOpenApiExtension>
                {
                    ["x-original"] = new JsonNodeExtension("original")
                }
            };

            // Act
            var copy = new OpenApiResponses(responses);
            copy.Extensions["x-copy-only"] = new JsonNodeExtension("copy");

            // Assert
            Assert.Equal(2, copy.Extensions.Count);
            Assert.Single(responses.Extensions);
            Assert.False(responses.Extensions.ContainsKey("x-copy-only"));
        }

        [Fact]
        public void CopyConstructorWithNullResponsesInitializesEmptyExtensions()
        {
            // Act
            var copy = new OpenApiResponses(null!);

            // Assert
            Assert.NotNull(copy.Extensions);
            Assert.Empty(copy.Extensions);
            Assert.Empty(copy);
        }

        [Fact]
        public void CopyConstructorWithNullExtensionsInitializesEmptyExtensions()
        {
            // Arrange
            var responses = new OpenApiResponses { Extensions = null };

            // Act
            var copy = new OpenApiResponses(responses);

            // Assert
            Assert.NotNull(copy.Extensions);
            Assert.Empty(copy.Extensions);
        }

        [Fact]
        public void CopyConstructorWithEmptyExtensionsInitializesEmptyExtensions()
        {
            // Arrange
            var responses = new OpenApiResponses
            {
                Extensions = new Dictionary<string, IOpenApiExtension>()
            };

            // Act
            var copy = new OpenApiResponses(responses);

            // Assert
            Assert.NotNull(copy.Extensions);
            Assert.Empty(copy.Extensions);
        }
    }
}
