// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiDiscriminatorTests
    {
        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_2, "defaultMapping", "Pet", true)]
        [InlineData(OpenApiSpecVersion.OpenApi3_2, "defaultMapping", "#/components/schemas/Pet", true)]
        [InlineData(OpenApiSpecVersion.OpenApi3_2, "defaultMapping", "Pet", false)]
        [InlineData(OpenApiSpecVersion.OpenApi3_1, "x-oas-default-mapping", "Pet", true)]
        [InlineData(OpenApiSpecVersion.OpenApi3_1, "x-oas-default-mapping", "#/components/schemas/Pet", true)]
        [InlineData(OpenApiSpecVersion.OpenApi3_1, "x-oas-default-mapping", "Pet", false)]
        public async Task SerializeDefaultMappingAsReferenceString(OpenApiSpecVersion specVersion, string propertyName, string referenceId, bool useDocument)
        {
            // Arrange
            var document = useDocument ? new OpenApiDocument() : null;
            var discriminator = new OpenApiDiscriminator
            {
                PropertyName = "pet_type",
                DefaultMapping = new OpenApiSchemaReference(referenceId, document)
            };

            // Act
            var actual = JsonNode.Parse(await discriminator.SerializeAsJsonAsync(specVersion));

            // Assert
            Assert.NotNull(actual);
            Assert.Equal("#/components/schemas/Pet", actual[propertyName]?.GetValue<string>());
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        [InlineData(OpenApiSpecVersion.OpenApi3_1)]
        [InlineData(OpenApiSpecVersion.OpenApi3_2)]
        public async Task SerializeMappingValuesAsReferenceStrings(OpenApiSpecVersion specVersion)
        {
            // Arrange
            var document = new OpenApiDocument();
            var discriminator = new OpenApiDiscriminator
            {
                PropertyName = "pet_type",
                Mapping = new Dictionary<string, OpenApiSchemaReference>
                {
                    ["simple"] = new OpenApiSchemaReference("Pet", document),
                    ["explicit"] = new OpenApiSchemaReference("#/components/schemas/Pet", document),
                    ["noDocument"] = new OpenApiSchemaReference("Pet")
                }
            };

            // Act
            var actual = JsonNode.Parse(await discriminator.SerializeAsJsonAsync(specVersion));

            // Assert
            Assert.NotNull(actual);
            var mapping = Assert.IsAssignableFrom<JsonObject>(actual["mapping"]);
            AssertMappingValue(mapping, "simple");
            AssertMappingValue(mapping, "explicit");
            AssertMappingValue(mapping, "noDocument");
        }

        private static void AssertMappingValue(JsonObject mapping, string key)
        {
            var value = Assert.IsAssignableFrom<JsonValue>(mapping[key]);
            Assert.Equal("#/components/schemas/Pet", value.GetValue<string>());
        }
    }
}
