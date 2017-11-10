// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Linq;
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

            Assert.NotNull(operation);
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
            Assert.Equal("simplehook", callbackPair.Key);

            var callback = callbackPair.Value;
            var pathItemPair = callback.PathItems.First();
            Assert.Equal("$request.body#/url", pathItemPair.Key.Expression);

            var pathItem = pathItemPair.Value;

            var operationPair = pathItem.Operations.First();
            var cboperation = operationPair.Value;
            Assert.Equal(OperationType.Post, operationPair.Key);

            Assert.NotNull(callback);
        }
    }
}