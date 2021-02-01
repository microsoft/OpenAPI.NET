using Microsoft.OpenApi.Diff.Tests._Base;
using Xunit;

namespace Microsoft.OpenApi.Diff.Tests.Tests
{
    public class ParameterDiffTest : BaseTest
    {
        private const string OpenAPIDoc1 = "Resources/parameters_diff_1.yaml";
        private const string OpenAPIDoc2 = "Resources/parameters_diff_2.yaml";

        [Fact]
        public void TestDiffDifferent()
        {
            TestUtils.AssertOpenAPIChangedEndpoints(OpenAPIDoc1, OpenAPIDoc2);
        }
    }
}
