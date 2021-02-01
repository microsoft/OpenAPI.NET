using Microsoft.OpenApi.Diff.Tests._Base;
using Xunit;

namespace Microsoft.OpenApi.Diff.Tests.Tests
{
    public class RecursiveSchemaTest : BaseTest
    {
        private const string OpenAPIDoc1 = "Resources/recursive_model_1.yaml";
        private const string OpenAPIDoc2 = "Resources/recursive_model_2.yaml";
        private const string OpenAPIDoc3 = "Resources/recursive_model_3.yaml";
        private const string OpenAPIDoc4 = "Resources/recursive_model_4.yaml";

        [Fact]
        public void TestDiffSame()
        {
            TestUtils.AssertOpenAPIAreEquals(OpenAPIDoc1, OpenAPIDoc1);
        }

        [Fact]
        public void TestDiffSameWithAllOf()
        {
            TestUtils.AssertOpenAPIAreEquals(OpenAPIDoc3, OpenAPIDoc3);
        }

        [Fact]
        public void TestDiffDifferent()
        {
            TestUtils.AssertOpenAPIBackwardIncompatible(OpenAPIDoc1, OpenAPIDoc2);
        }

        [Fact]
        public void TestDiffDifferentWithAllOf()
        {
            TestUtils.AssertOpenAPIBackwardIncompatible(OpenAPIDoc3, OpenAPIDoc4);
        }
    }
}
