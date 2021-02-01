using Microsoft.OpenApi.Diff.Tests._Base;
using Xunit;

namespace Microsoft.OpenApi.Diff.Tests.Tests
{
    public class AddPropDiffTest : BaseTest
    {
        private const string OpenAPIDoc1 = "Resources/add-prop-1.yaml";
        private const string OpenAPIDoc2 = "Resources/add-prop-2.yaml";

        [Fact]
        public void TestDiffSame()
        {
            TestUtils.AssertOpenAPIAreEquals(OpenAPIDoc1, OpenAPIDoc1);
        }

        [Fact]
        public void TestDiffDifferent()
        {
            TestUtils.AssertOpenAPIBackwardIncompatible(OpenAPIDoc1, OpenAPIDoc2);
        }
    }
}
