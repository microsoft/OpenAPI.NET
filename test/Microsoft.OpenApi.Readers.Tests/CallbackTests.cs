// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Linq;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests
{
    public class CallbackTests
    {
        [Fact]
        public void LoadSimpleCallback()
        {
            var stream = GetType()
                .Assembly.GetManifestResourceStream(typeof(CallbackTests), "Samples.CallbackSample.yaml");
            var openApiDoc = new OpenApiStreamReader().Read(stream, out var context);

            var path = openApiDoc.Paths.First().Value;
            var subscribeOperation = path.Operations[OperationType.Post];

            var callback = subscribeOperation.Callbacks["mainHook"];
            var pathItem = callback.PathItems.First().Value;
            var operation = pathItem.Operations[OperationType.Post];

            operation.Should().NotBeNull();
        }

        [Fact]
        public void LoadSimpleCallbackWithRefs()
        {
            var stream = GetType()
                .Assembly.GetManifestResourceStream(typeof(CallbackTests), "Samples.CallbackSampleWithRef.yaml");

            var openApiDoc = new OpenApiStreamReader().Read(stream, out var context);

            var path = openApiDoc.Paths.First().Value;
            var operation = path.Operations.First().Value;

            var callbackPair = operation.Callbacks.First();
            callbackPair.Key.Should().Be("simplehook");

            var callback = callbackPair.Value;
            var pathItemPair = callback.PathItems.First();
            pathItemPair.Key.Expression.Should().Be("$request.body#/url");

            var pathItem = pathItemPair.Value;

            var operationPair = pathItem.Operations.First();
            operationPair.Key.Should().Be(OperationType.Post);

            callback.Should().NotBeNull();
        }
    }
}