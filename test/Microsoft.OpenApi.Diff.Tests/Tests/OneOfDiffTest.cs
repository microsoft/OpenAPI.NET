using Microsoft.OpenApi.Diff.Tests._Base;
using Xunit;

namespace Microsoft.OpenApi.Diff.Tests.Tests
{
    public class OneOfDiffTest : BaseTest
    {
        private const string OpenAPIDoc1 = "Resources/oneOf_diff_1.yaml";
        private const string OpenAPIDoc2 = "Resources/oneOf_diff_2.yaml";
        private const string OpenAPIDoc3 = "Resources/oneOf_diff_3.yaml";
        private const string OpenAPIDoc4 = "Resources/composed_schema_1.yaml";
        private const string OpenAPIDoc5 = "Resources/composed_schema_2.yaml";
        private const string OpenAPIDoc6 = "Resources/oneOf_discriminator-changed_1.yaml";
        private const string OpenAPIDoc7 = "Resources/oneOf_discriminator-changed_2.yaml";

        [Fact]
        public void TestDiffSame()
        {
            TestUtils.AssertOpenAPIAreEquals(OpenAPIDoc1, OpenAPIDoc1);
        }

        [Fact]
        public void TestDiffDifferentMapping()
        {
            TestUtils.AssertOpenAPIChangedEndpoints(OpenAPIDoc1, OpenAPIDoc2);
        }

        [Fact]
        public void testDiffSameWithOneOf()
        {
            TestUtils.AssertOpenAPIAreEquals(OpenAPIDoc2, OpenAPIDoc3);
        }

        [Fact]
        public void TestComposedSchema()
        {
            TestUtils.AssertOpenAPIBackwardIncompatible(OpenAPIDoc4, OpenAPIDoc5);
        }

        [Fact]
        public void TestOneOfDiscriminatorChanged()
        {
            // The oneOf 'discriminator' changed: 'realtype' -> 'othertype':
            TestUtils.AssertOpenAPIBackwardIncompatible(OpenAPIDoc6, OpenAPIDoc7);
        }
    }
}
