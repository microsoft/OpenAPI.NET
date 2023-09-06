// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
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
		string name = OpenApiPrimaryErrorMessageExtension.Name;
		string expectedName = "x-ms-primary-error-message";

		// Assert
		Assert.Equal(expectedName, name);
	}
	[Fact]
	public void WritesValue()
	{
		// Arrange
		OpenApiPrimaryErrorMessageExtension extension = new() {
			IsPrimaryErrorMessage = true
		};
		using TextWriter sWriter = new StringWriter();
		OpenApiJsonWriter writer = new(sWriter);

		// Act
		extension.Write(writer, OpenApiSpecVersion.OpenApi3_0);
		string result = sWriter.ToString();

		// Assert
		Assert.True(extension.IsPrimaryErrorMessage);
		Assert.Equal("true", result);
	}
	[Fact]
	public void ParsesValue()
	{
		// Arrange
		var value = new OpenApiBoolean(true);

		// Act
		var extension = OpenApiPrimaryErrorMessageExtension.Parse(value);

		// Assert
		Assert.True(extension.IsPrimaryErrorMessage);
	}
}
