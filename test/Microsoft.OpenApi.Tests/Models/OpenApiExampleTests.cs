// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Writers;
using VerifyXunit;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiExampleTests
    {
        public static OpenApiExample AdvancedExample = new()
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
                                ["rel"] = "sampleRel1",
                                ["bytes"] = JsonSerializer.Serialize(new byte[] { 1, 2, 3 }),
                                ["binary"] = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes("Ñ😻😑♮Í☛oƞ♑😲☇éǋžŁ♻😟¥a´Ī♃ƠąøƩ"))
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

        public static OpenApiExampleReference OpenApiExampleReference = new(ReferencedExample, "example1");
        public static OpenApiExample ReferencedExample = new()
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
                },
                ["aDate"] = JsonSerializer.Serialize(DateTime.Parse("12/12/2022 00:00:00").ToString("yyyy-MM-dd"))
            }
        };

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeAdvancedExampleAsV3JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            AdvancedExample.SerializeAsV3(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeReferencedExampleAsV3JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            OpenApiExampleReference.SerializeAsV3(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeReferencedExampleAsV3JsonWithoutReferenceWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            ReferencedExample.SerializeAsV3(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }
    }
}
