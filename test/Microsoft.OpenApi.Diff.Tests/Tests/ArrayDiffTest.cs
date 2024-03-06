using Microsoft.OpenApi.Diff.Tests._Base;
using Xunit;

namespace Microsoft.OpenApi.Diff.Tests.Tests
{
    public class ArrayDiffTest : BaseTest
    {
        private const string OpenAPIDoc31 = "Resources/array_diff_1.yaml";
        private const string OpenAPIDoc32 = "Resources/array_diff_2.yaml";

        [Fact]
        public void TestArrayDiffDifferent()
        {
            TestUtils.AssertOpenAPIChangedEndpoints(OpenAPIDoc31, OpenAPIDoc32);
        }

        [Fact]
        public void TestArrayDiffSame()
        {
            TestUtils.AssertOpenAPIAreEquals(OpenAPIDoc31, OpenAPIDoc31);
        }
    }
}
