// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using SharpYaml.Serialization;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests
{
    public class OpenApiExampleTests
    {

        [Fact()]
        public void ApiWithExamples()
        {
            using (var stream = GetType().Assembly.GetManifestResourceStream(GetType(), "Samples.api-with-examples.yaml"))
            {
                var openApiDoc = new OpenApiStreamReader().Read(stream, out var context);

                Assert.Empty(context.Errors);
            }

        }

        [Fact]
        public void PetStoreExpandedExample()
        {
            using (var stream = GetType().Assembly.GetManifestResourceStream(GetType(), "Samples.petstore-expanded.yaml"))
            {
                var openApiDoc = new OpenApiStreamReader().Read(stream, out var context);

                Assert.Empty(context.Errors);
            }
        }

        [Fact]
        public void SimplePetStore()
        {
            using (var stream = GetType().Assembly.GetManifestResourceStream(GetType(), "Samples.petstore30.yaml"))
            {
                var openApiDoc = new OpenApiStreamReader().Read(stream, out var context);

                Assert.Empty(context.Errors);
            }
        }

        [Fact]
        public void UberExample()
        {
            using (var stream = GetType().Assembly.GetManifestResourceStream(GetType(), "Samples.uber.yaml"))
            {
                var openApiDoc = new OpenApiStreamReader().Read(stream, out var context);

                Assert.Empty(context.Errors);
            }
        }
    }
}