// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.IO;
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
        public void LoadParameterReference()
        {
            // Arrange
            var result = OpenApiDocument.Load(Path.Combine(SampleFolderPath, "multipleReferences.v2.yaml"));
            var reference = new OpenApiParameterReference("skipParam", result.Document);

            // Assert
            reference.Should().BeEquivalentTo(
                new OpenApiParameter
                {
                    Name = "skip",
                    In = ParameterLocation.Query,
                    Description = "number of items to skip",
                    Required = true,
                    Schema = new()
                    {
                        Type = JsonSchemaType.Integer,
                        Format = "int32"
                    }
                    
                }, options => options.Excluding(x => x.Reference)
            );
        }

        [Fact]
        public void LoadSecuritySchemeReference()
        {
            var result = OpenApiDocument.Load(Path.Combine(SampleFolderPath, "multipleReferences.v2.yaml"));

            var reference = new OpenApiSecuritySchemeReference("api_key_sample", result.Document);

            // Assert
            reference.Should().BeEquivalentTo(
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    Name = "api_key",
                    In = ParameterLocation.Header
                }, options => options.Excluding(x => x.Reference)
            );
        }

        [Fact]
        public void LoadResponseReference()
        {
            var result = OpenApiDocument.Load(Path.Combine(SampleFolderPath, "multipleReferences.v2.yaml"));

            var reference = new OpenApiResponseReference("NotFound", result.Document);

            // Assert
            reference.Should().BeEquivalentTo(
                new OpenApiResponse
                {
                    Description = "Entity not found.",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new()
                    }
                }, options => options.Excluding(x => x.Reference)
            );
        }

        [Fact]
        public void LoadResponseAndSchemaReference()
        {
            var result = OpenApiDocument.Load(Path.Combine(SampleFolderPath, "multipleReferences.v2.yaml"));
            var reference = new OpenApiResponseReference("GeneralError", result.Document);

            // Assert
            reference.Should().BeEquivalentTo(
                new OpenApiResponse
                {
                    Description = "General Error",
                    Content =
                    {
                        ["application/json"] = new()
                        {
                            Schema = new()
                            {
                                Description = "Sample description",
                                Required = new HashSet<string> {"name" },
                                Properties = {
                                    ["name"] = new()
                                    {
                                        Type = JsonSchemaType.String
                                    },
                                    ["tag"] = new()
                                    {
                                        Type = JsonSchemaType.String
                                    }
                                },

                                Reference = new()
                                {
                                    Type = ReferenceType.Schema,
                                    Id = "SampleObject2",
                                    HostDocument = result.Document
                                }
                            }
                        }
                    },
                    Reference = new()
                    {
                        Type = ReferenceType.Response,
                        Id = "GeneralError"
                    }
                }, options => options.Excluding(x => x.Reference)
            );
        }
    }
}
