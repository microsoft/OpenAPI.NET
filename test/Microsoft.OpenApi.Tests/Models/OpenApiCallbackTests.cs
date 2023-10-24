﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using VerifyXunit;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    [UsesVerify]
    public class OpenApiCallbackTests
    {
        public static OpenApiCallback AdvancedCallback = new()
        {
            PathItems =
            {
                [RuntimeExpression.Build("$request.body#/url")]
                = new()
                {
                    Operations =
                    {
                        [OperationType.Post] =
                        new()
                        {
                            RequestBody = new()
                            {
                                Content =
                                {
                                    ["application/json"] = new()
                                    {
                                        Schema = new()
                                        {
                                            Type = "object"
                                        }
                                    }
                                }
                            },
                            Responses = new()
                            {
                                ["200"] = new()
                                {
                                    Description = "Success"
                                }
                            }
                        }
                    }
                }
            }
        };

        public static OpenApiCallback ReferencedCallback = new()
        {
            Reference = new()
            {
                Type = ReferenceType.Callback,
                Id = "simpleHook",
            },
            PathItems =
            {
                [RuntimeExpression.Build("$request.body#/url")]
                = new()
                {
                    Operations =
                    {
                        [OperationType.Post] =
                        new()
                        {
                            RequestBody = new()
                            {
                                Content =
                                {
                                    ["application/json"] = new()
                                    {
                                        Schema = new()
                                        {
                                            Type = "object"
                                        }
                                    }
                                }
                            },
                            Responses = new()
                            {
                                ["200"] = new()
                                {
                                    Description = "Success"
                                }
                            }
                        }
                    }
                }
            }
        };

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeAdvancedCallbackAsV3JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            AdvancedCallback.SerializeAsV3(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeReferencedCallbackAsV3JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            ReferencedCallback.SerializeAsV3(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeReferencedCallbackAsV3JsonWithoutReferenceWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            ReferencedCallback.SerializeAsV3WithoutReference(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }
    }
}
