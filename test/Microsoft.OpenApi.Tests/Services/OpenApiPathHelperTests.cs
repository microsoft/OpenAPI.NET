// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#nullable enable
#pragma warning disable OAI020 // Type is for evaluation purposes only
using Xunit;

namespace Microsoft.OpenApi.Tests.Services;

public class OpenApiPathHelperTests
{
    #region Identity (no transformation needed)

    [Theory]
    [InlineData("#/info")]
    [InlineData("#/info/title")]
    [InlineData("#/paths")]
    [InlineData("#/paths/~1items")]
    [InlineData("#/paths/~1items/get")]
    [InlineData("#/paths/~1items/get/responses")]
    [InlineData("#/paths/~1items/get/responses/200")]
    [InlineData("#/paths/~1items/get/responses/200/description")]
    [InlineData("#/tags")]
    [InlineData("#/externalDocs")]
    [InlineData("#/security")]
    public void V2_IdenticalPaths_ReturnedAsIs(string path)
    {
        var result = OpenApiPathHelper.GetVersionedPath(path, OpenApiSpecVersion.OpenApi2_0);
        Assert.Equal(path, result);
    }

    #endregion

    #region Null / empty / v3.2 passthrough

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void NullOrEmptyPath_ReturnsAsIs(string? path)
    {
        var result = OpenApiPathHelper.GetVersionedPath(path!, OpenApiSpecVersion.OpenApi2_0);
        Assert.Equal(path, result);
    }

    [Theory]
    [InlineData("#/paths/~1items/get/responses/200/content/application~1json/schema")]
    [InlineData("#/components/schemas/Pet")]
    [InlineData("#/servers/0")]
    [InlineData("#/webhooks/newPet/post")]
    public void V32_AllPaths_ReturnedAsIs(string path)
    {
        var result = OpenApiPathHelper.GetVersionedPath(path, OpenApiSpecVersion.OpenApi3_2);
        Assert.Equal(path, result);
    }

    [Theory]
    [InlineData("#/paths/~1items/get/responses/200/content/application~1json/schema")]
    [InlineData("#/components/schemas/Pet")]
    [InlineData("#/servers/0")]
    public void V31_AllPaths_ReturnedAsIs(string path)
    {
        var result = OpenApiPathHelper.GetVersionedPath(path, OpenApiSpecVersion.OpenApi3_1);
        Assert.Equal(path, result);
    }

    #endregion

    #region V3.0 unsupported paths

    [Theory]
    [InlineData("#/webhooks/newPet")]
    [InlineData("#/webhooks/newPet/post")]
    [InlineData("#/webhooks/newPet/post/responses/200")]
    public void V30_Webhooks_ReturnsNull(string path)
    {
        var result = OpenApiPathHelper.GetVersionedPath(path, OpenApiSpecVersion.OpenApi3_0);
        Assert.Null(result);
    }

    [Fact]
    public void V30_NonWebhookPaths_ReturnedAsIs()
    {
        var path = "#/paths/~1items/get/responses/200/content/application~1json/schema";
        var result = OpenApiPathHelper.GetVersionedPath(path, OpenApiSpecVersion.OpenApi3_0);
        Assert.Equal(path, result);
    }

    #endregion

    #region V2 unsupported paths (null returns)

    [Theory]
    [InlineData("#/servers")]
    [InlineData("#/servers/0")]
    [InlineData("#/servers/0/url")]
    public void V2_TopLevelServers_ReturnsNull(string path)
    {
        var result = OpenApiPathHelper.GetVersionedPath(path, OpenApiSpecVersion.OpenApi2_0);
        Assert.Null(result);
    }

    [Theory]
    [InlineData("#/paths/~1items/servers/0")]
    [InlineData("#/paths/~1items/get/servers/0")]
    public void V2_NestedServers_ReturnsNull(string path)
    {
        var result = OpenApiPathHelper.GetVersionedPath(path, OpenApiSpecVersion.OpenApi2_0);
        Assert.Null(result);
    }

    [Theory]
    [InlineData("#/webhooks/newPet")]
    [InlineData("#/webhooks/newPet/post")]
    [InlineData("#/webhooks/newPet/post/responses/200")]
    public void V2_Webhooks_ReturnsNull(string path)
    {
        var result = OpenApiPathHelper.GetVersionedPath(path, OpenApiSpecVersion.OpenApi2_0);
        Assert.Null(result);
    }

    [Theory]
    [InlineData("#/paths/~1items/get/callbacks/onEvent")]
    [InlineData("#/paths/~1items/get/callbacks/onEvent/~1callback/post")]
    public void V2_Callbacks_ReturnsNull(string path)
    {
        var result = OpenApiPathHelper.GetVersionedPath(path, OpenApiSpecVersion.OpenApi2_0);
        Assert.Null(result);
    }

    [Theory]
    [InlineData("#/paths/~1items/get/responses/200/links/GetItemById")]
    [InlineData("#/paths/~1items/get/responses/200/links/GetItemById/operationId")]
    public void V2_Links_ReturnsNull(string path)
    {
        var result = OpenApiPathHelper.GetVersionedPath(path, OpenApiSpecVersion.OpenApi2_0);
        Assert.Null(result);
    }

    [Theory]
    [InlineData("#/paths/~1items/post/requestBody", "#/paths/~1items/post/parameters/0")]
    [InlineData("#/paths/~1items/post/requestBody/content/application~1json/schema", "#/paths/~1items/post/parameters/0/schema")]
    [InlineData("#/paths/~1items/post/requestBody/content/application~1json/schema/properties/id", "#/paths/~1items/post/parameters/0/schema/properties/id")]
    [InlineData("#/paths/~1items/post/requestBody/description", "#/paths/~1items/post/parameters/0/description")]
    [InlineData("#/paths/~1items/post/requestBody/required", "#/paths/~1items/post/parameters/0/required")]
    public void V2_InlineRequestBody_MappedToBodyParameter(string input, string expected)
    {
        var result = OpenApiPathHelper.GetVersionedPath(input, OpenApiSpecVersion.OpenApi2_0);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("#/components/examples/FooExample")]
    [InlineData("#/components/headers/X-Rate-Limit")]
    [InlineData("#/components/pathItems/SharedItem")]
    [InlineData("#/components/links/GetItemById")]
    [InlineData("#/components/callbacks/onEvent")]
    [InlineData("#/components/requestBodies/PetBody")]
    [InlineData("#/components/mediaTypes/JsonMedia")]
    public void V2_UnsupportedComponentTypes_ReturnsNull(string path)
    {
        var result = OpenApiPathHelper.GetVersionedPath(path, OpenApiSpecVersion.OpenApi2_0);
        Assert.Null(result);
    }

    [Theory]
    [InlineData("#/paths/~1items/post/responses/200/content/application~1json/encoding/color")]
    public void V2_Encoding_ReturnsNull(string path)
    {
        var result = OpenApiPathHelper.GetVersionedPath(path, OpenApiSpecVersion.OpenApi2_0);
        Assert.Null(result);
    }

    #endregion

    #region V2 component renames

    [Theory]
    [InlineData("#/components/schemas/Pet", "#/definitions/Pet")]
    [InlineData("#/components/schemas/Pet/properties/name", "#/definitions/Pet/properties/name")]
    [InlineData("#/components/schemas/Pet~0Special", "#/definitions/Pet~0Special")]
    public void V2_ComponentsSchemas_RenamedToDefinitions(string input, string expected)
    {
        var result = OpenApiPathHelper.GetVersionedPath(input, OpenApiSpecVersion.OpenApi2_0);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("#/components/parameters/SkipParam", "#/parameters/SkipParam")]
    [InlineData("#/components/parameters/SkipParam/schema/type", "#/parameters/SkipParam/schema/type")]
    public void V2_ComponentsParameters_RenamedToParameters(string input, string expected)
    {
        var result = OpenApiPathHelper.GetVersionedPath(input, OpenApiSpecVersion.OpenApi2_0);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("#/components/responses/NotFound", "#/responses/NotFound")]
    [InlineData("#/components/responses/NotFound/description", "#/responses/NotFound/description")]
    public void V2_ComponentsResponses_RenamedToResponses(string input, string expected)
    {
        var result = OpenApiPathHelper.GetVersionedPath(input, OpenApiSpecVersion.OpenApi2_0);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("#/components/securitySchemes/ApiKeyAuth", "#/securityDefinitions/ApiKeyAuth")]
    [InlineData("#/components/securitySchemes/OAuth2/flows", "#/securityDefinitions/OAuth2/flows")]
    public void V2_ComponentsSecuritySchemes_RenamedToSecurityDefinitions(string input, string expected)
    {
        var result = OpenApiPathHelper.GetVersionedPath(input, OpenApiSpecVersion.OpenApi2_0);
        Assert.Equal(expected, result);
    }

    #endregion

    #region V2 response content schema unwrapping

    [Theory]
    [InlineData(
        "#/paths/~1items/get/responses/200/content/application~1json/schema",
        "#/paths/~1items/get/responses/200/schema")]
    [InlineData(
        "#/paths/~1items/get/responses/200/content/application~1octet-stream/schema",
        "#/paths/~1items/get/responses/200/schema")]
    [InlineData(
        "#/paths/~1items/get/responses/200/content/application~1json/schema/properties/name",
        "#/paths/~1items/get/responses/200/schema/properties/name")]
    [InlineData(
        "#/paths/~1items/get/responses/default/content/application~1json/schema",
        "#/paths/~1items/get/responses/default/schema")]
    public void V2_ResponseContentSchema_Unwrapped(string input, string expected)
    {
        var result = OpenApiPathHelper.GetVersionedPath(input, OpenApiSpecVersion.OpenApi2_0);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void V2_ComponentResponseWithContent_UnwrapsContentAndRenames()
    {
        // #/components/responses/NotFound/content/application~1json/schema
        // → first: #/responses/NotFound/content/application~1json/schema (component rename)
        // → then: #/responses/NotFound/schema (content unwrapping)
        var result = OpenApiPathHelper.GetVersionedPath(
            "#/components/responses/NotFound/content/application~1json/schema",
            OpenApiSpecVersion.OpenApi2_0);
        Assert.Equal("#/responses/NotFound/schema", result);
    }

    #endregion

    #region V2 header schema unwrapping

    [Theory]
    [InlineData(
        "#/paths/~1items/get/responses/200/headers/X-Rate-Limit/schema/type",
        "#/paths/~1items/get/responses/200/headers/X-Rate-Limit/type")]
    [InlineData(
        "#/paths/~1items/get/responses/200/headers/X-Rate-Limit/schema",
        "#/paths/~1items/get/responses/200/headers/X-Rate-Limit")]
    public void V2_HeaderSchema_Unwrapped(string input, string expected)
    {
        var result = OpenApiPathHelper.GetVersionedPath(input, OpenApiSpecVersion.OpenApi2_0);
        Assert.Equal(expected, result);
    }

    #endregion

    #region Issue 2806 reproduction

    [Fact]
    public void Issue2806_ResponseContentSchemaPath_ConvertedToV2()
    {
        // The exact scenario from the issue:
        // v3 walker produces: #/paths/~1items/get/responses/200/content/application~1octet-stream/schema
        // v2 document expects: #/paths/~1items/get/responses/200/schema
        var v3Path = "#/paths/~1items/get/responses/200/content/application~1octet-stream/schema";
        var v2Path = OpenApiPathHelper.GetVersionedPath(v3Path, OpenApiSpecVersion.OpenApi2_0);
        Assert.Equal("#/paths/~1items/get/responses/200/schema", v2Path);
    }

    #endregion

    #region V2 rooting / false-positive guards

    // V2ResponseContentUnwrappingPolicy false positives: "responses" and "content" are schema property names.
    [Theory]
    [InlineData("#/paths/~1items/get/responses/200/schema/properties/responses/200/content/application~1json")]
    [InlineData("#/x-ms-sneaky-extension/additionalReference/responses/data/content/0")]
    [InlineData("#/paths/~1items/get/parameters/0/schema/properties/responses/200/content/application~1json/schema")]
    public void V2_ResponsesContentInsideSchemaOrExtension_NotUnwrapped(string path)
    {
        var result = OpenApiPathHelper.GetVersionedPath(path, OpenApiSpecVersion.OpenApi2_0);
        Assert.Equal(path, result);
    }

    // V2RequestBodyToBodyParameterPolicy false positives: "requestBody" is a schema property name.
    [Theory]
    [InlineData("#/paths/~1items/get/responses/200/schema/properties/requestBody/content/application~1json/schema")]
    [InlineData("#/paths/~1items/get/responses/200/schema/properties/requestBody/description")]
    public void V2_RequestBodyInsideSchemaProperties_NotConvertedToBodyParameter(string path)
    {
        var result = OpenApiPathHelper.GetVersionedPath(path, OpenApiSpecVersion.OpenApi2_0);
        Assert.Equal(path, result);
    }

    // V2HeaderSchemaUnwrappingPolicy false positives: "headers" is a schema property name.
    [Theory]
    [InlineData("#/paths/~1items/get/responses/200/schema/properties/headers/X-My-Header/schema/type")]
    [InlineData("#/paths/~1items/get/responses/200/schema/properties/headers/X-My-Header/schema")]
    public void V2_HeaderSchemaInsideSchemaProperties_NotUnwrapped(string path)
    {
        var result = OpenApiPathHelper.GetVersionedPath(path, OpenApiSpecVersion.OpenApi2_0);
        Assert.Equal(path, result);
    }

    // V2UnsupportedPathPolicy false positives: unsupported keywords are schema property names.
    [Theory]
    [InlineData("#/paths/~1items/get/responses/200/schema/properties/servers/0")]
    [InlineData("#/paths/~1items/get/responses/200/schema/properties/callbacks/onEvent")]
    [InlineData("#/paths/~1items/get/responses/200/schema/properties/links/item")]
    public void V2_UnsupportedKeywordsInsideSchemaProperties_NotReturnedNull(string path)
    {
        var result = OpenApiPathHelper.GetVersionedPath(path, OpenApiSpecVersion.OpenApi2_0);
        Assert.Equal(path, result);
    }

    // V2ComponentRenamePolicy false positives: "content" after a property named "responses" in a schema.
    [Fact]
    public void V2_ComponentSchemaWithResponsesPropertyContainingContent_ContentNotUnwrapped()
    {
        // components/schemas/MyModel has a property "responses" whose value has a "content" property.
        // The inline content-unwrapping should NOT fire here.
        var path = "#/components/schemas/MyModel/properties/responses/properties/content/properties/foo";
        var result = OpenApiPathHelper.GetVersionedPath(path, OpenApiSpecVersion.OpenApi2_0);
        // Only the component rename applies (#/components/schemas → #/definitions).
        Assert.Equal("#/definitions/MyModel/properties/responses/properties/content/properties/foo", result);
    }

    [Fact]
    public void V2_ComponentSchemaWithRequestBodyPropertyInSchema_RequestBodyNotConvertedToBodyParameter()
    {
        // A component schema with a property named "requestBody" — component rename applies but
        // requestBody → body parameter conversion should NOT fire.
        var path = "#/components/schemas/Pet/properties/requestBody/content/application~1json";
        var result = OpenApiPathHelper.GetVersionedPath(path, OpenApiSpecVersion.OpenApi2_0);
        Assert.Equal("#/definitions/Pet/properties/requestBody/content/application~1json", result);
    }

    [Fact]
    public void V2_ComponentSchemaWithHeadersPropertyInSchema_HeaderSchemaNotUnwrapped()
    {
        // A component schema with a property named "headers" — component rename applies but
        // headers/schema unwrapping should NOT fire.
        var path = "#/components/schemas/Pet/properties/headers/Content-Type/schema";
        var result = OpenApiPathHelper.GetVersionedPath(path, OpenApiSpecVersion.OpenApi2_0);
        Assert.Equal("#/definitions/Pet/properties/headers/Content-Type/schema", result);
    }

    #endregion

    #region OpenApiValidatorError.GetVersionedPointer

    [Fact]
    public void ValidatorError_GetVersionedPointer_DelegatesToHelper()
    {
        var error = new OpenApiValidatorError(
            "TestRule",
            "#/paths/~1items/get/responses/200/content/application~1json/schema",
            "Test error message");

        var v2Pointer = error.GetVersionedPointer(OpenApiSpecVersion.OpenApi2_0);
        Assert.Equal("#/paths/~1items/get/responses/200/schema", v2Pointer);
    }

    [Fact]
    public void ValidatorError_GetVersionedPointer_NullForUnsupported()
    {
        var error = new OpenApiValidatorError(
            "TestRule",
            "#/servers/0",
            "Test error message");

        var v2Pointer = error.GetVersionedPointer(OpenApiSpecVersion.OpenApi2_0);
        Assert.Null(v2Pointer);
    }

    [Fact]
    public void ValidatorError_GetVersionedPointer_V32_ReturnsOriginal()
    {
        var error = new OpenApiValidatorError(
            "TestRule",
            "#/components/schemas/Pet",
            "Test error message");

        var v32Pointer = error.GetVersionedPointer(OpenApiSpecVersion.OpenApi3_2);
        Assert.Equal("#/components/schemas/Pet", v32Pointer);
    }

    #endregion
}
