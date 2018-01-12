// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;
using Microsoft.OpenApi.Services;
using Microsoft.OpenApi.Validations;
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
            openApiDocument.Info = new OpenApiInfo()
            {
                Title = "foo",
                Version = "1.2.2"
            };
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

            validator.Errors.ShouldBeEquivalentTo(
                    new List<ValidationError>
                    {
                        new ValidationError(ErrorReason.Required, "#/paths/~1test/get/responses/200/description",
                            String.Format(SRResource.Validation_FieldIsRequired, "description", "response"))
        });
        }
    }
}