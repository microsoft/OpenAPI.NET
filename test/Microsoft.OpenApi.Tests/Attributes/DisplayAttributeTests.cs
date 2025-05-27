﻿using System;
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

    [Flags]
    public enum UserType
    {
        [DisplayAttribute("admin")]
        Admin = 1,
        [DisplayAttribute("editor")]
        Editor = 2,
        [DisplayAttribute("publisher")]
        Publisher = 3,
        [DisplayAttribute("all")]
        All = Admin | Editor | Publisher
    }

    public class DisplayAttributeTests
    {
        [Theory]
        [InlineData(ApiLevel.Private, "private")]
        [InlineData(ApiLevel.Public, "public")]
        [InlineData(ApiLevel.Corporate, "corporate")]
        public void GetDisplayNameExtensionShouldUseDisplayAttribute(ApiLevel apiLevel, string expected)
        {
            Assert.Equal(expected, apiLevel.GetDisplayName());
        }

        [Theory]
        [InlineData(ApiLevel.Private,"private")]
        [InlineData(ApiLevel.Public, "public")]
        [InlineData(ApiLevel.Corporate, "corporate")]
        public void GetEnumFromDisplayNameShouldReturnEnumValue(ApiLevel expected, string displayName)
        {
            displayName.TryGetEnumFromDisplayName<ApiLevel>(out var result);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(UserType.Admin,"admin")]
        [InlineData(UserType.Publisher, "publisher")]
        [InlineData(UserType.Editor, "editor")]
        public void GetEnumFromDisplayNameShouldReturnEnumValueForFlagsEnum(UserType expected, string displayName)
        {
            displayName.TryGetEnumFromDisplayName<UserType>(out var result);
            Assert.Equal(expected, result);
        }
    }
}
