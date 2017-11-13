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
            firstOperation.Key.Should().Be(OperationType.Get);
            firstOperation.Value.OperationId.Should().Be("findPets");
            firstOperation.Value.Parameters.Count.Should().Be(2);
        }

        [Fact]
        public void CheckPetStoreFirstOperationParameters()
        {
            var firstPath = _PetStoreDoc.Paths.First().Value;
            var firstOperation = firstPath.Operations.First().Value;
            var firstParameter = firstOperation.Parameters.First();

            firstParameter.Name.Should().Be("tags");
            firstParameter.In.Should().Be(ParameterLocation.Query);
            firstParameter.Description.Should().Be("tags to filter by");
        }

        [Fact]
        public void CheckPetStoreFirstOperationRequest()
        {
            var firstPath = _PetStoreDoc.Paths.First().Value;
            var firstOperation = firstPath.Operations.First().Value;
            var requestBody = firstOperation.RequestBody;

            firstOperation.RequestBody.Should().BeNull();
        }

        [Fact]
        public void DoesAPathExist()
        {
            _PetStoreDoc.Paths.Count().Should().Be(2);
            _PetStoreDoc.Paths["/pets"].Should().NotBeNull();
        }

        [Fact]
        public void GetPostOperation()
        {
            var postOperation = _PetStoreDoc.Paths["/pets"].Operations[OperationType.Post];

            postOperation.OperationId.Should().Be("addPet");

            postOperation.Should().NotBeNull();
        }

        [Fact]
        public void GetResponses()
        {
            var getOperation = _PetStoreDoc.Paths["/pets"].Operations[OperationType.Get];

            var responses = getOperation.Responses;

            responses["200"].Content.Values.Count().Should().Be(2);
            var response = getOperation.Responses["200"];
            var content = response.Content["application/json"];

            content.Schema.Should().NotBeNull();
        }
    }
}