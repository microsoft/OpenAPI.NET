// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    [UsesVerify]
    public class OpenApiCallbackTests
    {
        public static OpenApiCallback AdvancedCallback = new OpenApiCallback
        {
            PathItems =
            {
                [RuntimeExpression.Build("$request.body#/url")]
                = new OpenApiPathItem
                {
                    Operations =
                    {
                        [OperationType.Post] =
                        new OpenApiOperation
                        {
                            RequestBody = new OpenApiRequestBody
                            {
                                Content =
                                {
                                    ["application/json"] = new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Type = "object"
                                        }
                                    }
                                }
                            },
                            Responses = new OpenApiResponses
                            {
                                ["200"] = new OpenApiResponse
                                {
                                    Description = "Success"
                                }
                            }
                        }
                    }
                }
            }
        };

        public static OpenApiCallback ReferencedCallback = new OpenApiCallback
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.Callback,
                Id = "simpleHook",
            },
            PathItems =
            {
                [RuntimeExpression.Build("$request.body#/url")]
                = new OpenApiPathItem
                {
                    Operations =
                    {
                        [OperationType.Post] =
                        new OpenApiOperation
                        {
                            RequestBody = new OpenApiRequestBody
                            {
                                Content =
                                {
                                    ["application/json"] = new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Type = "object"
                                        }
                                    }
                                }
                            },
                            Responses = new OpenApiResponses
                            {
                                ["200"] = new OpenApiResponse
                                {
                                    Description = "Success"
                                }
                            }
                        }
                    }
                }
            }
        };

        private readonly ITestOutputHelper _output;

        public OpenApiCallbackTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeAdvancedCallbackAsV3JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            // Act
            AdvancedCallback.SerializeAsV3(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            await Verifier.Verify(actual).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeReferencedCallbackAsV3JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            // Act
            ReferencedCallback.SerializeAsV3(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            await Verifier.Verify(actual).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeReferencedCallbackAsV3JsonWithoutReferenceWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            // Act
            ReferencedCallback.SerializeAsV3WithoutReference(writer, OpenApiSpecVersion.OpenApi3_0);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            await Verifier.Verify(actual).UseParameters(produceTerseOutput);
        }
    }
}
