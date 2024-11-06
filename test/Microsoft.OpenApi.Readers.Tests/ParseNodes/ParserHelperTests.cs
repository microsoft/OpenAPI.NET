// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Globalization;
using Microsoft.OpenApi.Reader.ParseNodes;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.ParseNodes
{
    [Collection("DefaultSettings")]
    public class ParserHelperTests
    {
        [Fact]
        public void ParseDecimalWithFallbackOnOverflow_ReturnsParsedValue()
        {
            Assert.Equal(23434, ParserHelper.ParseDecimalWithFallbackOnOverflow("23434", 10));
        }

        [Fact]
        public void ParseDecimalWithFallbackOnOverflow_Overflows_ReturnsFallback()
        {
            Assert.Equal(10, ParserHelper.ParseDecimalWithFallbackOnOverflow(double.MaxValue.ToString(CultureInfo.InvariantCulture), 10));
        }
    }
}
