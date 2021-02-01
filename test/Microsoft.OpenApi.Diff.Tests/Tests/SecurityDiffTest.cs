using System;
using System.Linq;
using Microsoft.OpenApi.Diff.Tests._Base;
using Microsoft.OpenApi.Readers;
using Xunit;

namespace Microsoft.OpenApi.Diff.Tests.Tests
{
    public class SecurityDiffTest : BaseTest
    {
        private const string OpenapiDoc1 = "Resources/security_diff_1.yaml";
        private const string OpenapiDoc2 = "Resources/security_diff_2.yaml";
        private const string OpenapiDoc3 = "Resources/security_diff_3.yaml";

        [Fact]
        public void TestDiffDifferent()
        {
            var changedOpenAPI = TestUtils.GetOpenAPICompare().FromLocations(OpenapiDoc1, OpenapiDoc2);
            Assert.Equal(3, changedOpenAPI.ChangedOperations.Count);

            var changedOperation1 = changedOpenAPI
                .ChangedOperations
                .FirstOrDefault(x => x.PathUrl.Equals("/pet/{petId}"));
            Assert.NotNull(changedOperation1);
            Assert.False(changedOperation1.IsCompatible());

            var changedSecurityRequirements1 = changedOperation1.SecurityRequirements;
            Assert.NotNull(changedSecurityRequirements1);
            Assert.False(changedSecurityRequirements1.IsCompatible());
            Assert.Single(changedSecurityRequirements1.Increased);
            Assert.Single(changedSecurityRequirements1.Changed);

            var changedSecurityRequirement1 = changedSecurityRequirements1.Changed.FirstOrDefault();

            Assert.NotNull(changedSecurityRequirement1);
            Assert.Single(changedSecurityRequirement1.Changed);

            var changedScopes1 =
                changedSecurityRequirement1.Changed.First().ChangedScopes;
            Assert.NotNull(changedScopes1);
            Assert.Single(changedScopes1.Increased);
            Assert.Equal("read:pets", changedScopes1.Increased.First());

            var changedOperation2 =
                    changedOpenAPI
                        .ChangedOperations.FirstOrDefault(x => x.PathUrl == "/pet3");
            Assert.NotNull(changedOperation2);
            Assert.False(changedOperation2.IsCompatible());

            var changedSecurityRequirements2 =
                changedOperation2.SecurityRequirements;
            Assert.NotNull(changedSecurityRequirements2);
            Assert.False(changedSecurityRequirements2.IsCompatible());
            Assert.Single(changedSecurityRequirements2.Changed);

            var changedSecurityRequirement2 =
                changedSecurityRequirements2.Changed.First();
            Assert.Single(changedSecurityRequirement2.Changed);

            var changedImplicitOAuthFlow2 =
                changedSecurityRequirement2.Changed.First().OAuthFlows.ImplicitOAuthFlow;
            Assert.NotNull(changedImplicitOAuthFlow2);
            Assert.True(changedImplicitOAuthFlow2.ChangedAuthorizationUrl);

            var changedOperation3 =
                    changedOpenAPI
                        .ChangedOperations
                        .FirstOrDefault(x => x.PathUrl == "/pet/findByStatus2");
            Assert.NotNull(changedOperation3);
            Assert.True(changedOperation3.IsCompatible());

            var changedSecurityRequirements3 =
                changedOperation3.SecurityRequirements;
            Assert.NotNull(changedSecurityRequirements3);
            Assert.Single(changedSecurityRequirements3.Increased);

            var securityRequirement3 = changedSecurityRequirements3.Increased.First();
            Assert.Single(securityRequirement3);
            Assert.All(securityRequirement3.Keys, x => Assert.Equal("petstore_auth", x.Reference.ReferenceV3));
            Assert.Equal(2, securityRequirement3.First().Value.Count);
        }

        [Fact]
        public void TestWithUnknownSecurityScheme()
        {
            var settings = new OpenApiReaderSettings
            {
                ReferenceResolution = ReferenceResolutionSetting.DoNotResolveReferences
            };
            Assert.Throws<ArgumentException>(() => TestUtils.GetOpenAPICompare().FromLocations(OpenapiDoc3, OpenapiDoc3, settings));
        }
    }
}
