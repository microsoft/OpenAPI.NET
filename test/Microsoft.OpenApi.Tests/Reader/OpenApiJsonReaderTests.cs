// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Text;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Validations;
using Xunit;

namespace Microsoft.OpenApi.Tests.Reader;

public class OpenApiJsonReaderTests
{
    [Fact]
    public void ReadReturnsValidationErrorForSelfReferentialDiscriminatorSchema()
    {
        var reader = new OpenApiJsonReader();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(
            """
            {
              "openapi": "3.0.1",
              "info": {
                "title": "Sample",
                "version": "1.0.0"
              },
              "paths": {},
              "components": {
                "schemas": {
                  "Pet": {
                    "type": "object",
                    "discriminator": {
                      "propertyName": "kind"
                    },
                    "oneOf": [
                      {
                        "$ref": "#/components/schemas/Pet"
                      }
                    ]
                  }
                }
              }
            }
            """));

        var result = reader.Read(
            stream,
            new Uri("https://contoso.test/openapi.json"),
            new OpenApiReaderSettings());

        Assert.NotNull(result.Document);
        Assert.Contains(result.Diagnostic.Errors, error =>
            error is OpenApiValidatorError validatorError &&
            validatorError.RuleName == nameof(OpenApiSchemaRules.ValidateSchemaDiscriminator));
    }
}
