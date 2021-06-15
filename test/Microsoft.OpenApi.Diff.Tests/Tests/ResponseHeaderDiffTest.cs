using System.Linq;
using Microsoft.OpenApi.Diff.Tests._Base;
using Xunit;

namespace Microsoft.OpenApi.Diff.Tests.Tests
{
    public class ResponseHeaderDiffTest : BaseTest
    {
        private const string OpenapiDoc1 = "Resources/header_1.yaml";
        private const string OpenapiDoc2 = "Resources/header_2.yaml";

        [Fact]
        public void TestDiffDifferent()
        {
            var changedOpenAPI = TestUtils.GetOpenAPICompare().FromLocations(OpenapiDoc1, OpenapiDoc2);

            Assert.Empty(changedOpenAPI.NewEndpoints);
            Assert.Empty(changedOpenAPI.MissingEndpoints);
            Assert.NotEmpty(changedOpenAPI.ChangedOperations);

            var changedResponses = changedOpenAPI.ChangedOperations.FirstOrDefault()?.APIResponses.Changed;

            Assert.NotNull(changedResponses);
            Assert.NotEmpty(changedResponses);
            Assert.True(changedResponses.ContainsKey("200"));

            var changedHeaders = changedResponses["200"].Headers;
            Assert.True(changedHeaders.IsDifferent());
            Assert.Single(changedHeaders.Changed);
            Assert.Single(changedHeaders.Increased);
            Assert.Single(changedHeaders.Missing);
        }
    }
}
