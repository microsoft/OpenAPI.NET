// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.IO;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Tests;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiExampleTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiExample/";

        [Fact]
        public async Task ParseAdvancedExampleShouldSucceed()
        {
            var example = await OpenApiModelFactory.LoadAsync<OpenApiExample>(Path.Join(SampleFolderPath, "advancedExample.yaml"), OpenApiSpecVersion.OpenApi3_0, new(), SettingsFixture.ReaderSettings, token: TestContext.Current.CancellationToken);
            var expected = new OpenApiExample
            {
                Value = new JsonObject
                {
                    ["versions"] = new JsonArray
                            {
                                new JsonObject
                                {
                                    ["status"] = "Status1",
                                    ["id"] = "v1",
                                    ["links"] = new JsonArray
                                    {
                                        new JsonObject
                                        {
                                            ["href"] = "http://example.com/1",
                                            ["rel"] = "sampleRel1"
                                        }
                                    }
                                },

                                new JsonObject
                                {
                                    ["status"] = "Status2",
                                    ["id"] = "v2",
                                    ["links"] = new JsonArray
                                    {
                                        new JsonObject
                                        {
                                            ["href"] = "http://example.com/2",
                                            ["rel"] = "sampleRel2"
                                        }
                                    }
                                }
                            }
                }
            };

            OpenApiTestAssert.Equivalent(expected, example, "Root");
        }

        [Fact]
        public async Task ParseExampleForcedStringSucceed()
        {
            var result = await OpenApiDocument.LoadAsync(Path.Join(SampleFolderPath, "explicitString.yaml"), SettingsFixture.ReaderSettings, token: TestContext.Current.CancellationToken);
            Assert.Empty(result.Diagnostic.Errors);
        }

        [Fact]
        public async Task ParseExampleWithDataValueExtensionShouldSucceed()
        {
            // Arrange & Act
            var example = await OpenApiModelFactory.LoadAsync<OpenApiExample>(
                Path.Join(SampleFolderPath, "exampleWithDataValue.yaml"), 
                OpenApiSpecVersion.OpenApi3_0, 
                new(), 
                SettingsFixture.ReaderSettings, token: TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(example);
            Assert.Equal("Example with dataValue (extension)", example.Summary);
            Assert.NotNull(example.DataValue);
            Assert.Equal("Alice Johnson", example.DataValue["name"].GetValue<string>());
            Assert.Equal(28, example.DataValue["age"].GetValue<decimal>());
        }

        [Fact]
        public async Task ParseExampleWithSerializedValueExtensionShouldSucceed()
        {
            // Arrange & Act
            var example = await OpenApiModelFactory.LoadAsync<OpenApiExample>(
                Path.Join(SampleFolderPath, "exampleWithSerializedValue.yaml"), 
                OpenApiSpecVersion.OpenApi3_0, 
                new(), 
                SettingsFixture.ReaderSettings, token: TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(example);
            Assert.Equal("Example with serializedValue (extension)", example.Summary);
            Assert.Equal("custom serialized string for V3", example.SerializedValue);
        }
    }
}
