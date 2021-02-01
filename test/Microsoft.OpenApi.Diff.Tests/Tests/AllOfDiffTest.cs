using Microsoft.OpenApi.Diff.Tests._Base;
using Xunit;

namespace Microsoft.OpenApi.Diff.Tests.Tests
{
    public class AllOfDiffTest : BaseTest
    {
        private const string OpenAPIDoc1 = "Resources/allOf_diff_1.yaml";
        private const string OpenAPIDoc2 = "Resources/allOf_diff_2.yaml";
        private const string OpenAPIDoc3 = "Resources/allOf_diff_3.yaml";
        private const string OpenAPIDoc4 = "Resources/allOf_diff_4.yaml";

        [Fact]
        public void TestDiffSame()
        {
            TestUtils.AssertOpenAPIAreEquals(OpenAPIDoc1, OpenAPIDoc1);
        }

        [Fact]
        public void TestDiffSameWithAllOf()
        {
            TestUtils.AssertOpenAPIAreEquals(OpenAPIDoc1, OpenAPIDoc2);
        }

        [Fact]
        public void TestDiffDifferent1()
        {
            TestUtils.AssertOpenAPIChangedEndpoints(OpenAPIDoc1, OpenAPIDoc3);
        }

        [Fact]
        public void TestDiffDifferent2()
        {
            TestUtils.AssertOpenAPIChangedEndpoints(OpenAPIDoc1, OpenAPIDoc4);
        }
    }
}
