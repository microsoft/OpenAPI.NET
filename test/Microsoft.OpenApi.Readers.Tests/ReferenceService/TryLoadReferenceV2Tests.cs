// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.ReferenceService
{
    [Collection("DefaultSettings")]
    public class TryLoadReferenceV2Tests
    {
        private const string SampleFolderPath = "ReferenceService/Samples/";

        public TryLoadReferenceV2Tests()
        {
            OpenApiReaderRegistry.RegisterReader("yaml", new OpenApiYamlReader());
        }

        [Fact]
        public async Task LoadParameterReference()
        {
            // Arrange
            var result = await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, "multipleReferences.v2.yaml"));
            var reference = new OpenApiParameterReference("skipParam", result.Document);

            // Assert
            Assert.Equivalent(
                new OpenApiParameter
                {
                    Name = "skip",
                    In = ParameterLocation.Query,
                    Description = "number of items to skip",
                    Required = true,
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Integer,
                        Format = "int32"
                    }
                    
                },
                reference
            );
        }

        [Fact]
        public async Task LoadSecuritySchemeReference()
        {
            var result = await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, "multipleReferences.v2.yaml"));

            var reference = new OpenApiSecuritySchemeReference("api_key_sample", result.Document);

            // Assert
            Assert.Equivalent(
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    Name = "api_key",
                    In = ParameterLocation.Header
                }, reference
            );
        }

        [Fact]
        public async Task LoadResponseReference()
        {
            var result = await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, "multipleReferences.v2.yaml"));

            var reference = new OpenApiResponseReference("NotFound", result.Document);

            // Assert
            Assert.Equivalent(
                new OpenApiResponse
                {
                    Description = "Entity not found.",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new()
                    }
                }, reference
            );
        }

        [Fact]
        public async Task LoadResponseAndSchemaReference()
        {
            var result = await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, "multipleReferences.v2.yaml"));
            var reference = new OpenApiResponseReference("GeneralError", result.Document);

            var expected = new OpenApiResponse
                {
                    Description = "General Error",
                    Content =
                    {
                        ["application/json"] = new()
                        {
                            Schema = new OpenApiSchemaReference(new OpenApiSchema()
                            {
                                Description = "Sample description",
                                Required = new HashSet<string> {"name" },
                                Properties = {
                                    ["name"] = new OpenApiSchema()
                                    {
                                        Type = JsonSchemaType.String
                                    },
                                    ["tag"] = new OpenApiSchema()
                                    {
                                        Type = JsonSchemaType.String
                                    }
                                },
                            }, "SampleObject2")
                        }
                    }
                };

            ((OpenApiSchemaReference)expected.Content["application/json"].Schema).Reference.EnsureHostDocumentIsSet(result.Document);
            var actual = reference.Target;

            // Assert
            Assert.Equivalent(expected, actual);
        }
    }
}
