// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Writers;
using VerifyXunit;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiLinkTests
    {
        private static OpenApiLink AdvancedLink => new()
        {
            OperationId = "operationId1",
            Parameters = new Dictionary<string, RuntimeExpressionAnyWrapper>
            {
                ["parameter1"] = new()
                {
                    Expression = RuntimeExpression.Build("$request.path.id")
                }
            },
            RequestBody = new()
            {
                Any = new JsonObject
                {
                    ["property1"] = true
                }
            },
            Description = "description1",
            Server = new()
            {
                Description = "serverDescription1"
            }
        };

        private static OpenApiLinkReference LinkReference => new("example1");
        private static OpenApiLink ReferencedLink => new()
        {
            OperationId = "operationId1",
            Parameters = new Dictionary<string, RuntimeExpressionAnyWrapper>
            {
                ["parameter1"] = new()
                {
                    Expression = RuntimeExpression.Build("$request.path.id")
                }
            },
            RequestBody = new()
            {
                Any = new JsonObject
                {
                    ["property1"] = true
                }
            },
            Description = "description1",
            Server = new()
            {
                Description = "serverDescription1"
            }
        };

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeAdvancedLinkAsV3JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            AdvancedLink.SerializeAsV3(writer);
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeReferencedLinkAsV3JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            LinkReference.SerializeAsV3(writer);
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeReferencedLinkAsV3JsonWithoutReferenceWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            ReferencedLink.SerializeAsV3(writer);
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Fact]
        public void LinkExtensionsSerializationWorks()
        {
            // Arrange
            var link = new OpenApiLink()
            {
                Extensions = new()
                {
                    { "x-display", new JsonNodeExtension("Abc") 
                }
}
            };

            var expected =
                """
                {
                  "x-display": "Abc"
                }
                """;

            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = false });


            // Act
            link.SerializeAsV3(writer);

            // Assert
            var actual = outputStringWriter.ToString();
            Assert.Equal(expected.MakeLineBreaksEnvironmentNeutral(), actual.MakeLineBreaksEnvironmentNeutral());
        }
    }
}
