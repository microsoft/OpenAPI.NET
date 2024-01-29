// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
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
            Value = new OpenApiObject
            {
                ["versions"] = new OpenApiArray
                {
                    new OpenApiObject
                    {
                        ["status"] = new OpenApiString("Status1"),
                        ["id"] = new OpenApiString("v1"),
                        ["links"] = new OpenApiArray
                        {
                            new OpenApiObject
                            {
                                ["href"] = new OpenApiString("http://example.com/1"),
                                ["rel"] = new OpenApiString("sampleRel1"),
                                ["bytes"] = new OpenApiByte(new byte[] { 1, 2, 3 }),
                                ["binary"] = new OpenApiBinary(Encoding.UTF8.GetBytes("Ñ😻😑♮Í☛oƞ♑😲☇éǋžŁ♻😟¥a´Ī♃ƠąøƩ"))
                            }
                        }
                    },

                    new OpenApiObject
                    {
                        ["status"] = new OpenApiString("Status2"),
                        ["id"] = new OpenApiString("v2"),
                        ["links"] = new OpenApiArray
                        {
                            new OpenApiObject
                            {
                                ["href"] = new OpenApiString("http://example.com/2"),
                                ["rel"] = new OpenApiString("sampleRel2")
                            }
                        }
                    }
                }
            }
        };

        public static OpenApiExample ReferencedExample = new()
        {
            Reference = new()
            {
                Type = ReferenceType.Example,
                Id = "example1",
            },
            Value = new OpenApiObject
            {
                ["versions"] = new OpenApiArray
                {
                    new OpenApiObject
                    {
                        ["status"] = new OpenApiString("Status1"),
                        ["id"] = new OpenApiString("v1"),
                        ["links"] = new OpenApiArray
                        {
                            new OpenApiObject
                            {
                                ["href"] = new OpenApiString("http://example.com/1"),
                                ["rel"] = new OpenApiString("sampleRel1")
                            }
                        }
                    },

                    new OpenApiObject
                    {
                        ["status"] = new OpenApiString("Status2"),
                        ["id"] = new OpenApiString("v2"),
                        ["links"] = new OpenApiArray
                        {
                            new OpenApiObject
                            {
                                ["href"] = new OpenApiString("http://example.com/2"),
                                ["rel"] = new OpenApiString("sampleRel2")
                            }
                        }
                    }
                },
                ["aDate"] = new OpenApiDate(DateTime.Parse("12/12/2022 00:00:00"))
            }
        };

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeAdvancedExampleAsV3JsonWorks(bool produceTerseOutput)
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
        public async Task SerializeReferencedExampleAsV3JsonWorks(bool produceTerseOutput)
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

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeReferencedExampleAsV3JsonWithoutReferenceWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            ReferencedExample.SerializeAsV3WithoutReference(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }
    }
}
