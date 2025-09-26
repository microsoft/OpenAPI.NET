// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V32Tests
{
    public class OpenApiComponentsTests
    {
        [Theory]
        [InlineData("./FirstLevel/SecondLevel/ThridLevel/File.json#/components/schemas/ExternalRelativePathModel", "ExternalRelativePathModel", "./FirstLevel/SecondLevel/ThridLevel/File.json")]
        [InlineData("File.json#/components/schemas/ExternalSimpleRelativePathModel", "ExternalSimpleRelativePathModel", "File.json")]
        [InlineData("A:\\Dir\\File.json#/components/schemas/ExternalAbsWindowsPathModel", "ExternalAbsWindowsPathModel", "A:\\Dir\\File.json")]
        [InlineData("/Dir/File.json#/components/schemas/ExternalAbsUnixPathModel", "ExternalAbsUnixPathModel", "/Dir/File.json")]
        [InlineData("https://host.lan:1234/path/to/file/resource.json#/components/schemas/ExternalHttpsModel", "ExternalHttpsModel", "https://host.lan:1234/path/to/file/resource.json")]
        [InlineData("File.json", "File.json", null)]
        public void ParseExternalSchemaReferenceShouldSucceed(string reference, string referenceId, string externalResource)
        {
            var input = $@"{{
    ""schemas"": {{
        ""Model"": {{
            ""$ref"": ""{reference.Replace("\\", "\\\\")}""
        }}
    }}
}}
";
            var openApiDocument = new OpenApiDocument();

            // Act
            var components = OpenApiModelFactory.Parse<OpenApiComponents>(input, OpenApiSpecVersion.OpenApi3_2, openApiDocument, out _, "json");

            // Assert
            var schema = components.Schemas["Model"] as OpenApiSchemaReference;
            var expected = new OpenApiSchemaReference(referenceId, openApiDocument, externalResource);
            Assert.Equivalent(expected, schema);
        }
    }
}

