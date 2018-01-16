// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using FluentAssertions;
using Microsoft.OpenApi.Readers.Exceptions;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.OpenApiReaderTests
{
    [Collection("DefaultSettings")]
    public class UnsupportedSpecVersionTests
    {
        [Fact]
        public void ThrowOpenApiUnsupportedSpecVersionException()
        {
            using (var stream = Resources.GetStream("OpenApiReaderTests/Samples/unsupported.v1.yaml"))
            {
                Action act = () => new OpenApiStreamReader().Read(stream, out var diagnostic);
                act.ShouldThrow<OpenApiUnsupportedSpecVersionException>();
            }
        }
    }
}