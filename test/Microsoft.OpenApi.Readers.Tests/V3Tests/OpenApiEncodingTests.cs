﻿// Copyright (c) Microsoft Corporation. All rights reserved.
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
    public class OpenApiEncodingTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiEncoding/";

        public OpenApiEncodingTests()
        {
            OpenApiReaderRegistry.RegisterReader(OpenApiConstants.Yaml, new OpenApiYamlReader());
        }

        [Fact]
        public async Task ParseBasicEncodingShouldSucceed()
        {
            // Act
            var encoding = await OpenApiModelFactory.LoadAsync<OpenApiEncoding>(Path.Combine(SampleFolderPath, "basicEncoding.yaml"), OpenApiSpecVersion.OpenApi3_0);

            // Assert
            encoding.Element.Should().BeEquivalentTo(
                new OpenApiEncoding
                {
                    ContentType = "application/xml; charset=utf-8"
                });
        }

        [Fact]
        public async Task ParseAdvancedEncodingShouldSucceed()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "advancedEncoding.yaml"));

            // Act
            var encoding = await OpenApiModelFactory.LoadAsync<OpenApiEncoding>(stream, OpenApiSpecVersion.OpenApi3_0);

            // Assert
            encoding.Element.Should().BeEquivalentTo(
                new OpenApiEncoding
                {
                    ContentType = "image/png, image/jpeg",
                    Headers =
                    {
                        ["X-Rate-Limit-Limit"] =
                            new()
                            {
                                Description = "The number of allowed requests in the current period",
                                Schema = new()
                                { 
                                    Type = JsonSchemaType.Integer
                                }
                            }
                    }
                });
        }
    }
}
