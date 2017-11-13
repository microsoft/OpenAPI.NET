// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests
{
    public class OpenApiExampleTests
    {
        private readonly HttpClient client;

        public OpenApiExampleTests()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(
                "https://raw.githubusercontent.com/OAI/OpenAPI-Specification/OpenAPI.next/examples/v3.0/");
        }

        [Fact(Skip = "Example is not updated yet")]
        public async Task ApiWithExamples()
        {
            var stream = await client.GetStreamAsync("api-with-examples.yaml");
            var openApiDoc = new OpenApiStreamReader().Read(stream, out var context);

            context.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task PetStoreExpandedExample()
        {
            var stream = await client.GetStreamAsync("petstore-expanded.yaml");
            var openApiDoc = new OpenApiStreamReader().Read(stream, out var context);

            context.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task SimplePetStore()
        {
            var stream = await client.GetStreamAsync("petstore.yaml");
            var openApiDoc = new OpenApiStreamReader().Read(stream, out var context);

            context.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task UberExample()
        {
            var stream = await client.GetStreamAsync("uber.yaml");
            var openApiDoc = new OpenApiStreamReader().Read(stream, out var context);

            context.Errors.Should().BeEmpty();
        }
    }
}