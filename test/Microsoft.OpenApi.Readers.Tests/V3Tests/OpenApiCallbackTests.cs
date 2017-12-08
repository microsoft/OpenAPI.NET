// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiCallbackTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiCallback";

        [Fact]
        public void ParseBasicCallbackShouldSucceed()
        {
            using (var stream = File.OpenRead(Path.Combine(SampleFolderPath, "basicCallback.yaml")))
            {
                var openApiDoc = new OpenApiStreamReader().Read(stream, out var context);

                var path = openApiDoc.Paths.First().Value;
                var subscribeOperation = path.Operations[OperationType.Post];

                var callback = subscribeOperation.Callbacks["mainHook"];

                context.ShouldBeEquivalentTo(new OpenApiDiagnostic());

                callback.ShouldBeEquivalentTo(
                    new OpenApiCallback
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
                                                ["application/json"] = null
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
                    });
            }
        }

        [Fact]
        public void ParseBasicCallbackWithReferenceShouldSucceed()
        {
            using (var stream = File.OpenRead(Path.Combine(SampleFolderPath, "basicCallbackWithReference.yaml")))
            {
                var openApiDoc = new OpenApiStreamReader().Read(stream, out var context);

                var path = openApiDoc.Paths.First().Value;
                var subscribeOperation = path.Operations[OperationType.Post];

                var callback = subscribeOperation.Callbacks["simpleHook"];

                context.ShouldBeEquivalentTo(new OpenApiDiagnostic());

                callback.ShouldBeEquivalentTo(
                    new OpenApiCallback
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
                    });
            }
        }
    }
}