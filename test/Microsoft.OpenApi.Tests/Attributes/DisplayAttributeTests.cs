using Microsoft.OpenApi.Attributes;
using Microsoft.OpenApi.Extensions;
using Xunit;

namespace Microsoft.OpenApi.Tests.Attributes
{
    public enum ApiLevel
    {
        [DisplayAttribute("private")]
        Private = 1,
        [DisplayAttribute("public")]
        Public = 2,
        [DisplayAttribute("corporate")]
        Corporate = 3
    }

    public class DisplayAttributeTests
    {
        [Theory]
        [InlineData(ApiLevel.Private,"private")]
        [InlineData(ApiLevel.Public, "public")]
        [InlineData(ApiLevel.Corporate, "corporate")]
        public void GetDisplayNameExtensionShouldUseDisplayAttribute(ApiLevel apiLevel, string expected)
        {
            Assert.Equal(expected, apiLevel.GetDisplayName());          
        }
    }
}
