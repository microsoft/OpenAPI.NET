using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi;
using Xunit;
using Microsoft.OpenApi.Readers;

namespace OpenApiTests
{

    public class CallbackTests
    {
        [Fact]
        public void LoadSimpleCallback()
        {

            var stream = this.GetType().Assembly.GetManifestResourceStream("OpenApiTests.Samples.CallbackSample.yaml");
            var openApiDoc = OpenApiParser.Parse(stream).OpenApiDocument;

            PathItem path = openApiDoc.Paths.PathItems.First().Value;
            Operation subscribeOperation = path.Operations["post"];

            var callback = subscribeOperation.Callbacks["mainHook"];
            var pathItem = callback.PathItems.First().Value;
            var operation = pathItem.Operations["post"];

            Assert.NotNull(operation);

        }

        [Fact]
        public void LoadSimpleCallbackWithRefs()
        {

            var stream = this.GetType().Assembly.GetManifestResourceStream("OpenApiTests.Samples.CallbackSampleWithRef.yaml");

            var openApiDoc = OpenApiParser.Parse(stream).OpenApiDocument;

            var path = openApiDoc.Paths.PathItems.First().Value;
            var operation = path.Operations.First().Value;

            var callbackPair = operation.Callbacks.First();
            Assert.Equal("simplehook", callbackPair.Key);

            Callback callback = callbackPair.Value;
            var pathItemPair = callback.PathItems.First();
            Assert.Equal("$request.body(/url)", pathItemPair.Key.Expression);

            PathItem pathItem = pathItemPair.Value;

            var operationPair = pathItem.Operations.First();
            Operation cboperation = operationPair.Value;
            Assert.Equal("post", operationPair.Key);

            Assert.NotNull(callback);

        }

    }
}
