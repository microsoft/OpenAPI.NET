using Microsoft.OpenApi.Diff.Tests._Base;
using Xunit;

namespace Microsoft.OpenApi.Diff.Tests.Tests
{
    public class ContentDiffTest : BaseTest
    {
        private const string OpenAPIDoc1 = "Resources/content_diff_1.yaml";
        private const string OpenAPIDoc2 = "Resources/content_diff_2.yaml";

        [Fact]
        public void TestContentDiffWithOneEmptyMediaType()
        {
            var changedOpenAPI = TestUtils.GetOpenAPICompare().FromLocations(OpenAPIDoc1, OpenAPIDoc2);
            Assert.True(changedOpenAPI.IsIncompatible());
        }

        [Fact]
        public void TestContentDiffWithEmptyMediaTypes()
        {
            var changedOpenAPI = TestUtils.GetOpenAPICompare().FromLocations(OpenAPIDoc1, OpenAPIDoc1);
            Assert.True(changedOpenAPI.IsUnchanged());
        }

        [Fact]
        public void TestSameContentDiff()
        {
            var changedOpenAPI = TestUtils.GetOpenAPICompare().FromLocations(OpenAPIDoc2, OpenAPIDoc2);
            Assert.True(changedOpenAPI.IsUnchanged());
        }
    }
}
