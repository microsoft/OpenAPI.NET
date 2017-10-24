// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.IO;
using Microsoft.OpenApi.Writers;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests
{
    public class YamlWriterTests
    {
        [Fact]
        public void WriteList()
        {
            var outputString = new StringWriter();
            var writer = new OpenApiYamlWriter(outputString);

            writer.WriteStartArray();
            writer.WriteStartObject();
            writer.WriteEndObject();
            writer.WriteEndArray();

            //Assert.Equal(0, debug.StackState.Count);
            //Assert.Equal("", debug.Indent);
        }

        [Fact]
        public void WriteMap()
        {
            var outputString = new StringWriter();
            var writer = new OpenApiYamlWriter(outputString);

            writer.WriteStartObject();
            writer.WriteEndObject();

            //Assert.Equal(0, debug.StackState.Count);
            //Assert.Equal("", debug.Indent);
        }
    }
}