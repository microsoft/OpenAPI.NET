using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Microsoft.OpenApi.Attributes;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
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
#pragma warning disable CS0618 // Type or member is obsolete, testing obsolete behavior
            Assert.Equal(expected, apiLevel.GetDisplayName());
#pragma warning restore CS0618 // Type or member is obsolete, testing obsolete behavior
        }

        [Fact]
        public void GetDisplayNameWorksForAllParameterStyle()
        {
            var enumValues = new List<ParameterStyle>(Enum.GetValues<ParameterStyle>());

            Assert.Equal("matrix", ParameterStyle.Matrix.GetDisplayName());
            Assert.True(enumValues.Remove(ParameterStyle.Matrix));

            Assert.Equal("label", ParameterStyle.Label.GetDisplayName());
            Assert.True(enumValues.Remove(ParameterStyle.Label));

            Assert.Equal("form", ParameterStyle.Form.GetDisplayName());
            Assert.True(enumValues.Remove(ParameterStyle.Form));

            Assert.Equal("simple", ParameterStyle.Simple.GetDisplayName());
            Assert.True(enumValues.Remove(ParameterStyle.Simple));

            Assert.Equal("spaceDelimited", ParameterStyle.SpaceDelimited.GetDisplayName());
            Assert.True(enumValues.Remove(ParameterStyle.SpaceDelimited));

            Assert.Equal("pipeDelimited", ParameterStyle.PipeDelimited.GetDisplayName());
            Assert.True(enumValues.Remove(ParameterStyle.PipeDelimited));

            Assert.Equal("deepObject", ParameterStyle.DeepObject.GetDisplayName());
            Assert.True(enumValues.Remove(ParameterStyle.DeepObject));

            Assert.Empty(enumValues);
        }

        [Fact]
        public void GetDisplayNameWorksForAllParameterLocation()
        {
            var enumValues = new List<ParameterLocation>(Enum.GetValues<ParameterLocation>());

            Assert.Equal("query", ParameterLocation.Query.GetDisplayName());
            Assert.True(enumValues.Remove(ParameterLocation.Query));

            Assert.Equal("header", ParameterLocation.Header.GetDisplayName());
            Assert.True(enumValues.Remove(ParameterLocation.Header));

            Assert.Equal("path", ParameterLocation.Path.GetDisplayName());
            Assert.True(enumValues.Remove(ParameterLocation.Path));

            Assert.Equal("cookie", ParameterLocation.Cookie.GetDisplayName());
            Assert.True(enumValues.Remove(ParameterLocation.Cookie));

            Assert.Empty(enumValues);
        }

        [Fact]
        public void GetDisplayNameWorksForAllReferenceType()
        {
            var enumValues = new List<ReferenceType>(Enum.GetValues<ReferenceType>());

            Assert.Equal("schemas", ReferenceType.Schema.GetDisplayName());
            Assert.True(enumValues.Remove(ReferenceType.Schema));

            Assert.Equal("responses", ReferenceType.Response.GetDisplayName());
            Assert.True(enumValues.Remove(ReferenceType.Response));

            Assert.Equal("parameters", ReferenceType.Parameter.GetDisplayName());
            Assert.True(enumValues.Remove(ReferenceType.Parameter));

            Assert.Equal("examples", ReferenceType.Example.GetDisplayName());
            Assert.True(enumValues.Remove(ReferenceType.Example));
            
            Assert.Equal("requestBodies", ReferenceType.RequestBody.GetDisplayName());
            Assert.True(enumValues.Remove(ReferenceType.RequestBody));

            Assert.Equal("headers", ReferenceType.Header.GetDisplayName());
            Assert.True(enumValues.Remove(ReferenceType.Header));

            Assert.Equal("securitySchemes", ReferenceType.SecurityScheme.GetDisplayName());
            Assert.True(enumValues.Remove(ReferenceType.SecurityScheme));

            Assert.Equal("links", ReferenceType.Link.GetDisplayName());
            Assert.True(enumValues.Remove(ReferenceType.Link));

            Assert.Equal("callbacks", ReferenceType.Callback.GetDisplayName());
            Assert.True(enumValues.Remove(ReferenceType.Callback));

            Assert.Equal("tags", ReferenceType.Tag.GetDisplayName());
            Assert.True(enumValues.Remove(ReferenceType.Tag));

            Assert.Equal("path", ReferenceType.Path.GetDisplayName());
            Assert.True(enumValues.Remove(ReferenceType.Path));

            Assert.Empty(enumValues);
        }

        [Fact]
        public void GetDisplayNameWorksForAllOperationTypes()
        {
            var enumValues = new List<OperationType>(Enum.GetValues<OperationType>());

            Assert.Equal("get", OperationType.Get.GetDisplayName());
            Assert.True(enumValues.Remove(OperationType.Get));

            Assert.Equal("put", OperationType.Put.GetDisplayName());
            Assert.True(enumValues.Remove(OperationType.Put));

            Assert.Equal("post", OperationType.Post.GetDisplayName());
            Assert.True(enumValues.Remove(OperationType.Post));

            Assert.Equal("delete", OperationType.Delete.GetDisplayName());
            Assert.True(enumValues.Remove(OperationType.Delete));

            Assert.Equal("options", OperationType.Options.GetDisplayName());
            Assert.True(enumValues.Remove(OperationType.Options));

            Assert.Equal("head", OperationType.Head.GetDisplayName());
            Assert.True(enumValues.Remove(OperationType.Head));

            Assert.Equal("patch", OperationType.Patch.GetDisplayName());
            Assert.True(enumValues.Remove(OperationType.Patch));

            Assert.Equal("trace", OperationType.Trace.GetDisplayName());
            Assert.True(enumValues.Remove(OperationType.Trace));

            Assert.Empty(enumValues);
        }

        [Fact]
        public void GetDisplayNameWorksForAllSecuritySchemeTypes()
        {
            var enumValues = new List<SecuritySchemeType>(Enum.GetValues<SecuritySchemeType>());

            Assert.Equal("apiKey", SecuritySchemeType.ApiKey.GetDisplayName());
            Assert.True(enumValues.Remove(SecuritySchemeType.ApiKey));

            Assert.Equal("http", SecuritySchemeType.Http.GetDisplayName());
            Assert.True(enumValues.Remove(SecuritySchemeType.Http));

            Assert.Equal("oauth2", SecuritySchemeType.OAuth2.GetDisplayName());
            Assert.True(enumValues.Remove(SecuritySchemeType.OAuth2));

            Assert.Equal("openIdConnect", SecuritySchemeType.OpenIdConnect.GetDisplayName());
            Assert.True(enumValues.Remove(SecuritySchemeType.OpenIdConnect));

            Assert.Empty(enumValues);
        }
    }
}
