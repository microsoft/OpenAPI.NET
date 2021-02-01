using Microsoft.OpenApi.Diff.Tests._Base;
using Xunit;

namespace Microsoft.OpenApi.Diff.Tests.Tests
{
    public class ReferenceDiffCacheTest : BaseTest
    {
        private const string OpenAPIDoc1 = "Resources/schema_diff_cache_1.yaml";

        [Fact]
        public void TestDiffSame()
        {
            TestUtils.AssertOpenAPIAreEquals(OpenAPIDoc1, OpenAPIDoc1);
        }
    }
}
