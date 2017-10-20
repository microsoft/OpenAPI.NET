using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi;
using Xunit;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Readers.YamlReaders;

namespace Microsoft.OpenApi.Readers.Tests
{

    public class CallbackTests
    {
        [Fact]
        public void LoadSimpleCallback()
        {

            var stream = this.GetType().Assembly.GetManifestResourceStream(typeof(CallbackTests),"Samples.CallbackSample.yaml");
            var openApiDoc = new OpenApiStreamReader().Read(stream, out var context);

            OpenApiPathItem path = openApiDoc.Paths.First().Value;
            OpenApiOperation subscribeOperation = path.Operations["post"];

            var callback = subscribeOperation.Callbacks["mainHook"];
            var pathItem = callback.PathItems.First().Value;
            var operation = pathItem.Operations["post"];

            Assert.NotNull(operation);

        }

        [Fact]
        public void LoadSimpleCallbackWithRefs()
        {

            var stream = this.GetType().Assembly.GetManifestResourceStream(typeof(CallbackTests), "Samples.CallbackSampleWithRef.yaml");

            var openApiDoc = new OpenApiStreamReader().Read(stream, out var context);

            var path = openApiDoc.Paths.First().Value;
            var operation = path.Operations.First().Value;

            var callbackPair = operation.Callbacks.First();
            Assert.Equal("simplehook", callbackPair.Key);

            OpenApiCallback callback = callbackPair.Value;
            var pathItemPair = callback.PathItems.First();
            Assert.Equal("$request.body(/url)", pathItemPair.Key.Expression);

            OpenApiPathItem pathItem = pathItemPair.Value;

            var operationPair = pathItem.Operations.First();
            OpenApiOperation cboperation = operationPair.Value;
            Assert.Equal("post", operationPair.Key);

            Assert.NotNull(callback);

        }

    }
}
