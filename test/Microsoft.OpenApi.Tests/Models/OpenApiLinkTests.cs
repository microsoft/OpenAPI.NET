// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Globalization;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Writers;
using VerifyXunit;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiLinkTests
    {
        public static readonly OpenApiLink AdvancedLink = new()
        {
            OperationId = "operationId1",
            Parameters =
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

        public static readonly OpenApiLinkReference LinkReference = new(ReferencedLink, "example1");
        public static readonly OpenApiLink ReferencedLink = new()
        {
            Reference = new()
            {
                Type = ReferenceType.Link,
                Id = "example1",
            },
            OperationId = "operationId1",
            Parameters =
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
            writer.Flush();

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
            writer.Flush();

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
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Fact]
        public void LinkExtensionsSerializationWorks()
        {
            // Arrange
            var link = new OpenApiLink()
            {
                Extensions = {
                { "x-display", new OpenApiAny("Abc") }
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
