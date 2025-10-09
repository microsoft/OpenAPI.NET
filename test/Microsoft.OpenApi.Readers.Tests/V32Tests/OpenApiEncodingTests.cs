// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Threading.Tasks;
using Microsoft.OpenApi.Reader;
using System.Collections.Generic;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V32Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiEncodingTests
    {
        private const string SampleFolderPath = "V32Tests/Samples/OpenApiEncoding/";

        [Fact]
        public async Task ParseEncodingWithAllowReservedShouldSucceed()
        {
            // Act
            var encoding = await OpenApiModelFactory.LoadAsync<OpenApiEncoding>(Path.Combine(SampleFolderPath, "encodingWithAllowReserved.yaml"), OpenApiSpecVersion.OpenApi3_2, new(), SettingsFixture.ReaderSettings);

            // Assert
            Assert.Equivalent(
                new OpenApiEncoding
                {
                    ContentType = "application/x-www-form-urlencoded",
                    Style = ParameterStyle.Form,
                    Explode = true,
                    AllowReserved = true
                }, encoding);
        }

        [Fact]
        public async Task ParseEncodingWithNestedEncodingShouldSucceed()
        {
            // Act
            var encoding = await OpenApiModelFactory.LoadAsync<OpenApiEncoding>(Path.Combine(SampleFolderPath, "encodingWithNestedEncoding.yaml"), OpenApiSpecVersion.OpenApi3_2, new(), SettingsFixture.ReaderSettings);

            // Assert
            Assert.NotNull(encoding);
            Assert.Equal("application/json", encoding.ContentType);
            Assert.NotNull(encoding.Headers);
            Assert.Single(encoding.Headers);
            Assert.NotNull(encoding.Encoding);
            Assert.Equal(2, encoding.Encoding.Count);
            Assert.True(encoding.Encoding.ContainsKey("nestedField"));
            Assert.Equal("application/xml", encoding.Encoding["nestedField"].ContentType);
            Assert.Equal(ParameterStyle.Form, encoding.Encoding["nestedField"].Style);
            Assert.True(encoding.Encoding["nestedField"].Explode);
            Assert.True(encoding.Encoding.ContainsKey("anotherField"));
            Assert.Equal("text/plain", encoding.Encoding["anotherField"].ContentType);
        }
    }
}
