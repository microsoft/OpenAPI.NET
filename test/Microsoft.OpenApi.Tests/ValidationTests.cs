// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using FluentAssertions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Services;
using Xunit;

namespace Microsoft.OpenApi.Tests
{
    public class ValidationTests
    {
        [Fact]
        public void ResponseMustHaveADescription()
        {
            var openApiDocument = new OpenApiDocument();
            openApiDocument.AddPathItem(
                "/test",
                p => p.AddOperation(
                    OperationType.Get,
                    o => o.AddResponse(
                        "200",
                        r =>
                        {
                        }
                    )
                )
            );
            var validator = new OpenApiValidator();
            var walker = new OpenApiWalker(validator);
            walker.Walk(openApiDocument);

            validator.Exceptions.Should().HaveCount(1);
        }
    }
}