using System;
using Microsoft.OpenApi.Diff.Tests._Base;
using Xunit;

namespace Microsoft.OpenApi.Diff.Tests.Tests
{
    public class PathDiffTest : BaseTest
    {
        private const string OpenAPIPath1 = "Resources/path_1.yaml";
        private const string OpenAPIPath2 = "Resources/path_2.yaml";
        private const string OpenAPIPath3 = "Resources/path_3.yaml";

        [Fact]
        public void TestEqual()
        {
            TestUtils.AssertOpenAPIAreEquals(OpenAPIPath1, OpenAPIPath2);
        }

        [Fact]
        public void TestMultiplePathWithSameSignature()
        {
            Assert.Throws<ArgumentException>(() => TestUtils.AssertOpenAPIAreEquals(OpenAPIPath3, OpenAPIPath3));
        }
    }
}
