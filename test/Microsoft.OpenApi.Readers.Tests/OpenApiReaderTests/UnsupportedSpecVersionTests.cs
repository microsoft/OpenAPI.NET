// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using FluentAssertions;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.OpenApiReaderTests
{
    [Collection("DefaultSettings")]
    public class UnsupportedSpecVersionTests
    {
        [Fact]
        public void ThrowOpenApiUnsupportedSpecVersionException()
        {
            try
            {
                _ = OpenApiDocument.Load("OpenApiReaderTests/Samples/unsupported.v1.yaml");
            }
            catch (OpenApiUnsupportedSpecVersionException exception)
            {
                exception.SpecificationVersion.Should().Be("1.0.0");
            }
        }
    }
}
