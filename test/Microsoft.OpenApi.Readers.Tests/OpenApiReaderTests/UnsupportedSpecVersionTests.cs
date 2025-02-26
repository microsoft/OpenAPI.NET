// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Threading.Tasks;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.OpenApiReaderTests
{
    [Collection("DefaultSettings")]
    public class UnsupportedSpecVersionTests
    {
        [Fact]
        public async Task ThrowOpenApiUnsupportedSpecVersionException()
        {
            try
            {
                _ = await OpenApiDocument.LoadAsync("OpenApiReaderTests/Samples/unsupported.v1.yaml", SettingsFixture.ReaderSettings);
            }
            catch (OpenApiUnsupportedSpecVersionException exception)
            {
                Assert.Equal("1.0.0", exception.SpecificationVersion);
            }
        }
    }
}
