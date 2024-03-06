using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.OpenApi.Diff.Output.Html;
using Microsoft.OpenApi.Diff.Output.Markdown;
using Microsoft.OpenApi.Diff.Tests._Base;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Diff.Tests.Tests
{
    public class OpenAPIDiffTest : BaseTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public OpenAPIDiffTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        private const string OpenAPIDoc1 = "Resources/petstore_v2_1.yaml";
        private const string OpenAPIDoc2 = "Resources/petstore_v2_2.yaml";
        private const string OpenAPIEmptyDoc = "Resources/petstore_v2_empty.yaml";

        [Fact]
        public void TestEqual()
        {
            TestUtils.AssertOpenAPIAreEquals(OpenAPIDoc2, OpenAPIDoc2);
        }

        [Fact]
        public async void TestNewAPI()
        {
            var changedOpenAPI = TestUtils.GetOpenAPICompare().FromLocations(OpenAPIEmptyDoc, OpenAPIDoc2);
            var newEndpoints = changedOpenAPI.NewEndpoints;
            var missingEndpoints = changedOpenAPI.MissingEndpoints;
            var changedEndPoints = changedOpenAPI.ChangedOperations;
            var html = await new HtmlRender().Render(changedOpenAPI);

            try
            {
                File.WriteAllText("testNewAPI.html", html);
            }
            catch (Exception e)
            {
                _testOutputHelper.WriteLine(e.ToString());
            }
            Assert.NotEmpty(newEndpoints);
            Assert.Empty(missingEndpoints);
            Assert.Empty(changedEndPoints);
        }

        [Fact]
        public async Task TestDeprecatedAPI()
        {
            var changedOpenAPI = TestUtils.GetOpenAPICompare().FromLocations(OpenAPIDoc1, OpenAPIEmptyDoc);
            var newEndpoints = changedOpenAPI.NewEndpoints;
            var missingEndpoints = changedOpenAPI.MissingEndpoints;
            var changedEndPoints = changedOpenAPI.ChangedOperations;
            var html = await new HtmlRender().Render(changedOpenAPI);

            try
            {
                File.WriteAllText("testDeprecatedAPI.html", html);
            }
            catch (Exception e)
            {
                _testOutputHelper.WriteLine(e.ToString());
            }
            Assert.Empty(newEndpoints);
            Assert.NotEmpty(missingEndpoints);
            Assert.Empty(changedEndPoints);
        }

        [Fact]
        public async Task TestDiff()
        {
            var changedOpenAPI = TestUtils.GetOpenAPICompare().FromLocations(OpenAPIDoc1, OpenAPIDoc2);
            var changedEndPoints = changedOpenAPI.ChangedOperations;
            var html = await new HtmlRender().Render(changedOpenAPI);
            try
            {
                File.WriteAllText("testDiff.html", html);
            }
            catch (Exception e)
            {
                _testOutputHelper.WriteLine(e.ToString());
            }
            Assert.NotEmpty(changedEndPoints);
        }

        [Fact]
        public async Task TestDiffAndMarkdown()
        {
            var changedOpenAPI = TestUtils.GetOpenAPICompare().FromLocations(OpenAPIDoc1, OpenAPIDoc2);
            var logger = _testOutputHelper.BuildLoggerFor<MarkdownRender>();
            var render = await new MarkdownRender(logger).Render(changedOpenAPI);
            try
            {
                File.WriteAllText("testDiff.md", render);

            }
            catch (Exception e)
            {
                _testOutputHelper.WriteLine(e.ToString());
            }
        }

        [Fact]
        public async Task TestDiffAndHtml()
        {
            var changedOpenAPI = TestUtils.GetOpenAPICompare().FromLocations(OpenAPIDoc1, OpenAPIDoc2);
            //var incompatibleChanges = TestUtils.GetChangesOfType(changedOpenAPI, DiffResultEnum.Incompatible);
            var render = await new HtmlRender().Render(changedOpenAPI);
            try
            {
                File.WriteAllText("testDiff.html", render);

            }
            catch (Exception e)
            {
                _testOutputHelper.WriteLine(e.ToString());
            }
        }
    }
}
