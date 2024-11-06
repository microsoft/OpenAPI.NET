// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Writers;
using VerifyXunit;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiRequestBodyTests
    {
        public static OpenApiRequestBody AdvancedRequestBody = new()
        {
            Description = "description",
            Required = true,
            Content =
            {
                ["application/json"] = new()
                {
                    Schema = new()
                    {
                        Type = JsonSchemaType.String
                    }
                }
            }
        };

        public static OpenApiRequestBodyReference OpenApiRequestBodyReference = new(ReferencedRequestBody, "example1");
        public static OpenApiRequestBody ReferencedRequestBody = new()
        {
            Description = "description",
            Required = true,
            Content =
            {
                ["application/json"] = new()
                {
                    Schema = new()
                    {
                        Type = JsonSchemaType.String
                    }
                }
            }
        };

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeAdvancedRequestBodyAsV3JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            AdvancedRequestBody.SerializeAsV3(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeReferencedRequestBodyAsV3JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            OpenApiRequestBodyReference.SerializeAsV3(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeReferencedRequestBodyAsV3JsonWithoutReferenceWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            ReferencedRequestBody.SerializeAsV3(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }
    }
}
