// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Linq;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests
{
    public class OperationTests
    {
        private readonly OpenApiDocument _PetStoreDoc;
        //      private Operation _PostOperation;

        public OperationTests()
        {
            var stream = GetType()
                .Assembly.GetManifestResourceStream(typeof(OperationTests), "Samples.petstore30.yaml");
            _PetStoreDoc = new OpenApiStreamReader().Read(stream, out var context);
            //_PostOperation = _PetStoreDoc.Paths.PathMap.Where(pm=>pm.Key == "/pets").Value
            //    .Operations.Where()
        }

        [Fact]
        public void CheckPetStoreFirstOperation()
        {
            var firstPath = _PetStoreDoc.Paths.First().Value;
            var firstOperation = firstPath.Operations.First();
            Assert.Equal("get", firstOperation.Key);
            Assert.Equal("findPets", firstOperation.Value.OperationId);
            Assert.Equal(2, firstOperation.Value.Parameters.Count);
        }

        [Fact]
        public void CheckPetStoreFirstOperationParameters()
        {
            var firstPath = _PetStoreDoc.Paths.First().Value;
            var firstOperation = firstPath.Operations.First().Value;
            var firstParameter = firstOperation.Parameters.First();

            Assert.Equal("tags", firstParameter.Name);
            Assert.Equal(ParameterLocation.Query, firstParameter.In);
            Assert.Equal("tags to filter by", firstParameter.Description);
        }

        [Fact]
        public void CheckPetStoreFirstOperationRequest()
        {
            var firstPath = _PetStoreDoc.Paths.First().Value;
            var firstOperation = firstPath.Operations.First().Value;
            var requestBody = firstOperation.RequestBody;

            Assert.Null(firstOperation.RequestBody);
        }

        [Fact]
        public void DoesAPathExist()
        {
            Assert.Equal(2, _PetStoreDoc.Paths.Count());
            Assert.NotNull(_PetStoreDoc.Paths["/pets"]);
        }

        [Fact]
        public void GetPostOperation()
        {
            var postOperation = _PetStoreDoc.Paths["/pets"].Operations["post"];

            Assert.Equal("addPet", postOperation.OperationId);

            Assert.NotNull(postOperation);
        }

        [Fact]
        public void GetResponses()
        {
            var getOperation = _PetStoreDoc.Paths["/pets"].Operations["get"];

            var responses = getOperation.Responses;

            Assert.Equal(2, responses["200"].Content.Values.Count());
            var response = getOperation.Responses["200"];
            var content = response.Content["application/json"];

            Assert.NotNull(content.Schema);
        }
    }
}