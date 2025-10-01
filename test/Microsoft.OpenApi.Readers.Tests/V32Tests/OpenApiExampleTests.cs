// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V32Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiExampleTests
    {
        private const string SampleFolderPath = "V32Tests/Samples/OpenApiExample/";

        [Fact]
        public async Task ParseExampleWithDataValueShouldSucceed()
        {
            // Arrange & Act
            var example = await OpenApiModelFactory.LoadAsync<OpenApiExample>(
                Path.Combine(SampleFolderPath, "exampleWithDataValue.yaml"), 
                OpenApiSpecVersion.OpenApi3_2, 
                new(), 
                SettingsFixture.ReaderSettings);

            // Assert
            example.Should().NotBeNull();
            example.Summary.Should().Be("Example with dataValue");
            example.DataValue.Should().NotBeNull();
            example.DataValue["name"].GetValue<string>().Should().Be("John Doe");
            example.DataValue["age"].GetValue<decimal>().Should().Be(30);
        }

        [Fact]
        public async Task ParseExampleWithSerializedValueShouldSucceed()
        {
            // Arrange & Act
            var example = await OpenApiModelFactory.LoadAsync<OpenApiExample>(
                Path.Combine(SampleFolderPath, "exampleWithSerializedValue.yaml"), 
                OpenApiSpecVersion.OpenApi3_2, 
                new(), 
                SettingsFixture.ReaderSettings);

            // Assert
            example.Should().NotBeNull();
            example.Summary.Should().Be("Example with serializedValue");
            example.SerializedValue.Should().Be("custom serialized string");
        }
    }
}
