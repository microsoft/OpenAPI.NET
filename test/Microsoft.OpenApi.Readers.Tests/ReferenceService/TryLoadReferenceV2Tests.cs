// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.V2;
using SharpYaml.Serialization;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.ReferenceService
{
    [Collection("DefaultSettings")]
    public class TryLoadReferenceV2Tests
    {
        private const string SampleFolderPath = "ReferenceService/Samples";

        [Fact]
        public void LoadSchemaReference()
        {
            // Arrange
            var context = new ParsingContext();
            var diagnostic = new OpenApiDiagnostic();

            using (var stream = File.OpenRead(Path.Combine(SampleFolderPath, "multipleReferences.v2.yaml")))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));
                var yamlDocument = yamlStream.Documents.First();

                context.RootNode = new RootNode(context, diagnostic, yamlDocument);
            }

            context.VersionService = new OpenApiV2VersionService();

            var reference = new OpenApiReference
            {
                Type = ReferenceType.Schema,
                Id = "SampleObject"
            };

            // Act
            context.VersionService.TryLoadReference(context, reference, out var referencedObject);

            // Assert
            referencedObject.ShouldBeEquivalentTo(
                new OpenApiSchema
                {
                    Required =
                    {
                        "id",
                        "name"
                    },
                    Properties =
                    {
                        ["id"] = new OpenApiSchema
                        {
                            Type = "integer",
                            Format = "int64"
                        },
                        ["name"] = new OpenApiSchema
                        {
                            Type = "string"
                        },
                        ["tag"] = new OpenApiSchema
                        {
                            Type = "string"
                        }
                    }
                }
            );
        }

        [Fact]
        public void LoadParameterReference()
        {
            // Arrange
            var context = new ParsingContext();
            var diagnostic = new OpenApiDiagnostic();

            using (var stream = File.OpenRead(Path.Combine(SampleFolderPath, "multipleReferences.v2.yaml")))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));
                var yamlDocument = yamlStream.Documents.First();

                context.RootNode = new RootNode(context, diagnostic, yamlDocument);
            }

            context.VersionService = new OpenApiV2VersionService();

            var reference = new OpenApiReference
            {
                Type = ReferenceType.Parameter,
                Id = "skipParam"
            };

            // Act
            context.VersionService.TryLoadReference(context, reference, out var referencedObject);

            // Assert
            referencedObject.ShouldBeEquivalentTo(
                new OpenApiParameter
                {
                    Name = "skip",
                    In = ParameterLocation.Query,
                    Description = "number of items to skip",
                    Required = true,
                    Schema = new OpenApiSchema
                    {
                        Type = "integer",
                        Format = "int32"
                    }
                }
            );
        }

        [Fact]
        public void LoadSecuritySchemeReference()
        {
            // Arrange
            var context = new ParsingContext();
            var diagnostic = new OpenApiDiagnostic();
            

            using (var stream = File.OpenRead(Path.Combine(SampleFolderPath, "multipleReferences.v2.yaml")))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));
                var yamlDocument = yamlStream.Documents.First();

                context.RootNode = new RootNode(context, diagnostic, yamlDocument);
            }

            context.VersionService = new OpenApiV2VersionService();
            var reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "api_key_sample"
            };

            // Act
            context.VersionService.TryLoadReference(context, reference, out var referencedObject);

            // Assert
            referencedObject.ShouldBeEquivalentTo(
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    Name = "api_key",
                    In = ParameterLocation.Header
                }
            );
        }

        [Fact]
        public void LoadResponseReference()
        {
            // Arrange
            var context = new ParsingContext();
            var diagnostic = new OpenApiDiagnostic();

            using (var stream = File.OpenRead(Path.Combine(SampleFolderPath, "multipleReferences.v2.yaml")))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));
                var yamlDocument = yamlStream.Documents.First();

                context.RootNode = new RootNode(context, diagnostic, yamlDocument);
            }

            context.VersionService = new OpenApiV2VersionService();

            var reference = new OpenApiReference
            {
                Type = ReferenceType.Response,
                Id = "NotFound"
            };

            // Act
            context.VersionService.TryLoadReference(context,reference, out var referencedObject);

            // Assert
            referencedObject.ShouldBeEquivalentTo(
                new OpenApiResponse
                {
                    Description = "Entity not found."
                }
            );
        }

        [Fact]
        public void LoadResponseAndSchemaReference()
        {
            // Arrange
            var context = new ParsingContext();
            var diagnostic = new OpenApiDiagnostic();

            using (var stream = File.OpenRead(Path.Combine(SampleFolderPath, "multipleReferences.v2.yaml")))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));
                var yamlDocument = yamlStream.Documents.First();

                context.RootNode = new RootNode(context, diagnostic, yamlDocument);
            }

            context.VersionService = new OpenApiV2VersionService();

            var reference = new OpenApiReference
            {
                Type = ReferenceType.Response,
                Id = "GeneralError"
            };

            // Act
            context.VersionService.TryLoadReference(context, reference, out var referencedObject);

            // Assert
            referencedObject.ShouldBeEquivalentTo(
                new OpenApiResponse
                {
                    Description = "General Error",
                    Content =
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.Schema,
                                    Id = "SampleObject2"
                                },
                                Description = "Sample description",
                                Required =
                                {
                                    "name"
                                },
                                Properties =
                                {
                                    ["name"] = new OpenApiSchema
                                    {
                                        Type = "string"
                                    },
                                    ["tag"] = new OpenApiSchema
                                    {
                                        Type = "string"
                                    }
                                }
                            }
                        }
                    }
                }
            );
        }
    }
}