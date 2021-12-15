// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    [UsesVerify]
    public class OpenApiLinkTests
    {
        public static OpenApiLink AdvancedLink = new OpenApiLink
        {
            OperationId = "operationId1",
            Parameters =
            {
                ["parameter1"] = new RuntimeExpressionAnyWrapper
                {
                    Expression = RuntimeExpression.Build("$request.path.id")
                }
            },
            RequestBody = new RuntimeExpressionAnyWrapper
            {
                Any = new OpenApiObject
                {
                    ["property1"] = new OpenApiBoolean(true)
                }
            },
            Description = "description1",
            Server = new OpenApiServer
            {
                Description = "serverDescription1"
            }
        };

        public static OpenApiLink ReferencedLink = new OpenApiLink
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.Link,
                Id = "example1",
            },
            OperationId = "operationId1",
            Parameters =
            {
                ["parameter1"] = new RuntimeExpressionAnyWrapper
                {
                    Expression = RuntimeExpression.Build("$request.path.id")
                }
            },
            RequestBody = new RuntimeExpressionAnyWrapper
            {
                Any = new OpenApiObject
                {
                    ["property1"] = new OpenApiBoolean(true)
                }
            },
            Description = "description1",
            Server = new OpenApiServer
            {
                Description = "serverDescription1"
            }
        };

        private readonly ITestOutputHelper _output;

        public OpenApiLinkTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeAdvancedLinkAsV3JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            // Act
            AdvancedLink.SerializeAsV3(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            await Verifier.Verify(actual).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeReferencedLinkAsV3JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            // Act
            ReferencedLink.SerializeAsV3(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            await Verifier.Verify(actual).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeReferencedLinkAsV3JsonWithoutReferenceWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            // Act
            ReferencedLink.SerializeAsV3WithoutReference(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            await Verifier.Verify(actual).UseParameters(produceTerseOutput);
        }
    }
}
