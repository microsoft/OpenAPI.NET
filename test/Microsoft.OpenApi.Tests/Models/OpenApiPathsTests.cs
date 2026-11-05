// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiPathsTests
    {
        [Fact]
        public void CopyConstructorCopiesExtensions()
        {
            // Arrange
            var paths = new OpenApiPaths
            {
                ["/users"] = new OpenApiPathItem(),
                Extensions = new Dictionary<string, IOpenApiExtension>
                {
                    ["x-custom-path"] = new JsonNodeExtension("custom-value"),
                    ["x-rate-limit"] = new JsonNodeExtension(100),
                }
            };

            // Act
            var copy = new OpenApiPaths(paths);

            // Assert
            Assert.NotNull(copy.Extensions);
            Assert.Equal(2, copy.Extensions.Count);
            Assert.True(copy.Extensions.ContainsKey("x-custom-path"));
            Assert.True(copy.Extensions.ContainsKey("x-rate-limit"));
        }

        [Fact]
        public void CopyConstructorIsolatesExtensionsDictionary()
        {
            // Arrange
            var paths = new OpenApiPaths
            {
                Extensions = new Dictionary<string, IOpenApiExtension>
                {
                    ["x-original"] = new JsonNodeExtension("original")
                }
            };

            // Act
            var copy = new OpenApiPaths(paths);
            copy.Extensions["x-copy-only"] = new JsonNodeExtension("copy");

            // Assert
            Assert.Equal(2, copy.Extensions.Count);
            Assert.Single(paths.Extensions);
            Assert.False(paths.Extensions.ContainsKey("x-copy-only"));
        }
    }
}
