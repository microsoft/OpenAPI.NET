// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    [UsesVerify]
    public class OpenApiExampleTests
    {
        public static OpenApiExample AdvancedExample = new OpenApiExample
        {
            Value = new OpenApiAny(new JsonObject
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
                                ["bytes"] = Convert.ToBase64String(new byte[] { 1, 2, 3 }),
                                ["binary"] = Convert.ToBase64String(Encoding.UTF8.GetBytes("Ñ😻😑♮Í☛oƞ♑😲☇éǋžŁ♻😟¥a´Ī♃ƠąøƩ"))
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
            })
        };

        public static OpenApiExample ReferencedExample = new OpenApiExample
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.Example,
                Id = "example1",
            },
            Value = new OpenApiAny(new JsonObject
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
            })
        };

        private readonly ITestOutputHelper _output;

        public OpenApiExampleTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeAdvancedExampleAsV3JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

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
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

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
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            // Act
            ReferencedExample.SerializeAsV3WithoutReference(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }
    }
}
