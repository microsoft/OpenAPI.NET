// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Writers;
using VerifyXunit;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiCallbackTests
    {
        private static OpenApiCallback AdvancedCallback => new()
        {
            PathItems = new Dictionary<RuntimeExpression, IOpenApiPathItem>
            {
                [RuntimeExpression.Build("$request.body#/url")]
                = new OpenApiPathItem()
                {
                    Operations = new()
                    {
                        [HttpMethod.Post] =
                        new()
                        {
                            RequestBody = new OpenApiRequestBody()
                            {
                                Content = new()
                                {
                                    ["application/json"] = new()
                                    {
                                        Schema = new OpenApiSchema()
                                        {
                                            Type = JsonSchemaType.Object
                                        }
                                    }
                                }
                            },
                            Responses = new()
                            {
                                ["200"] = new OpenApiResponse()
                                {
                                    Description = "Success"
                                }
                            }
                        }
                    }
                }
            }
        };

        private static OpenApiCallbackReference CallbackProxy => new("simpleHook");

        private static OpenApiCallback ReferencedCallback => new()
        {
            PathItems = new Dictionary<RuntimeExpression, IOpenApiPathItem>
            {
                [RuntimeExpression.Build("$request.body#/url")]
                = new OpenApiPathItem()
                {
                    Operations = new()
                    {
                        [HttpMethod.Post] =
                        new()
                        {
                            RequestBody = new OpenApiRequestBody()
                            {
                                Content = new()
                                {
                                    ["application/json"] = new()
                                    {
                                        Schema = new OpenApiSchema()
                                        {
                                            Type = JsonSchemaType.Object
                                        }
                                    }
                                }
                            },
                            Responses = new()
                            {
                                ["200"] = new OpenApiResponse()
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
        public async Task SerializeAdvancedCallbackAsV3JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            AdvancedCallback.SerializeAsV3(writer);
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeReferencedCallbackAsV3JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            CallbackProxy.SerializeAsV3(writer);
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeReferencedCallbackAsV3JsonWithoutReferenceWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            ReferencedCallback.SerializeAsV3(writer);
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }
    }
}
