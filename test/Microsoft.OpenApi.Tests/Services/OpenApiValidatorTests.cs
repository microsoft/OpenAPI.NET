// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Services;
using Xunit;

namespace Microsoft.OpenApi.Tests.Services
{
    [Collection("DefaultSettings")]
    public class OpenApiValidatorTests
    {
        [Fact]
        public void ResponseMustHaveADescription()
        {
            var openApiDocument = new OpenApiDocument();

            openApiDocument.Paths.Add(
                "/test",
                new OpenApiPathItem
                {
                    Operations =
                    {
                        [OperationType.Get] = new OpenApiOperation
                        {
                            Responses =
                            {
                                ["200"] = new OpenApiResponse()
                            }
                        }
                    }
                });

            var validator = new OpenApiValidator();
            var walker = new OpenApiWalker(validator);
            walker.Walk(openApiDocument);

            validator.Exceptions.ShouldBeEquivalentTo(
                    new List<OpenApiException>
                    { 
                        new OpenApiException("Response must have a description")
                    });
        }
    }
}