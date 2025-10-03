// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiEncodingTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiEncoding/";

        [Fact]
        public async Task ParseBasicEncodingShouldSucceed()
        {
            // Act
            var encoding = await OpenApiModelFactory.LoadAsync<OpenApiEncoding>(Path.Combine(SampleFolderPath, "basicEncoding.yaml"), OpenApiSpecVersion.OpenApi3_0, new(), SettingsFixture.ReaderSettings);

            // Assert
            Assert.Equivalent(
                new OpenApiEncoding
                {
                    ContentType = "application/xml; charset=utf-8"
                }, encoding);
        }

        [Fact]
        public async Task ParseAdvancedEncodingShouldSucceed()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "advancedEncoding.yaml"));

            // Act
            var encoding = await OpenApiModelFactory.LoadAsync<OpenApiEncoding>(stream, OpenApiSpecVersion.OpenApi3_0, new(), settings: SettingsFixture.ReaderSettings);

            // Assert
            Assert.Equivalent(
                new OpenApiEncoding
                {
                    ContentType = "image/png, image/jpeg",
                    Headers = new Dictionary<string, IOpenApiHeader>
                    {
                        ["X-Rate-Limit-Limit"] =
                            new OpenApiHeader()
                            {
                                Description = "The number of allowed requests in the current period",
                                Schema = new OpenApiSchema()
                                { 
                                    Type = JsonSchemaType.Integer
                                }
                            }
                    }
                }, encoding);
        }

        [Fact]
        public async Task ParseEncodingWithAllowReservedShouldSucceed()
        {
            // Act
            var encoding = await OpenApiModelFactory.LoadAsync<OpenApiEncoding>(Path.Combine(SampleFolderPath, "encodingWithAllowReserved.yaml"), OpenApiSpecVersion.OpenApi3_0, new(), SettingsFixture.ReaderSettings);

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
    }
}
