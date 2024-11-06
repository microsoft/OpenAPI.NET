// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.IO;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Writers;
using Xunit;

namespace Microsoft.OpenApi.OData.OpenApiExtensions.Tests;

public class OpenApiPrimaryErrorMessageExtensionTests
{
    [Fact]
    public void ExtensionNameMatchesExpected()
    {
        // Act
        var name = MicrosoftExtensions.OpenApiPrimaryErrorMessageExtension.Name;
        var expectedName = "x-ms-primary-error-message";

        // Assert
        Assert.Equal(expectedName, name);
    }

    [Fact]
    public void WritesValue()
    {
        // Arrange
        Microsoft.OpenApi.MicrosoftExtensions.OpenApiPrimaryErrorMessageExtension extension = new()
        {
            IsPrimaryErrorMessage = true
        };
        using TextWriter sWriter = new StringWriter();
        OpenApiJsonWriter writer = new(sWriter);

        // Act
        extension.Write(writer, OpenApiSpecVersion.OpenApi3_0);
        var result = sWriter.ToString();

        // Assert
        Assert.True(extension.IsPrimaryErrorMessage);
        Assert.Equal("true", result);
    }

    [Fact]
    public void ParsesValue()
    {
        // Arrange
        var value = true;

        // Act
        var extension = MicrosoftExtensions.OpenApiPrimaryErrorMessageExtension.Parse(value);

        // Assert
        Assert.True(extension.IsPrimaryErrorMessage);
    }
}
