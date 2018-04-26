// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
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
        private const string SampleFolderPath = "ReferenceService/Samples/";

        [Fact]
        public void LoadSchemaReference()
        {
            // Arrange
            OpenApiDocument document;
            var diagnostic = new OpenApiDiagnostic();

            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "multipleReferences.v2.yaml")))
            {
                document = new OpenApiStreamReader().Read(stream, out diagnostic);
            }

            var reference = new OpenApiReference
            {
                Type = ReferenceType.Schema,
                Id = "SampleObject"
            };

            // Act
            var referencedObject = document.ResolveReference(reference);

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
                    },
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.Schema,
                        Id = "SampleObject"
                    }
                }
            );
        }

        [Fact]
        public void LoadParameterReference()
        {
            // Arrange
            OpenApiDocument document;
            var diagnostic = new OpenApiDiagnostic();

            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "multipleReferences.v2.yaml")))
            {
                document = new OpenApiStreamReader().Read(stream, out diagnostic);
            }

            var reference = new OpenApiReference
            {
                Type = ReferenceType.Parameter,
                Id = "skipParam"
            };

            // Act
            var referencedObject = document.ResolveReference(reference);

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
                    },
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.Parameter,
                        Id = "skipParam"
                    }
                }
            );
        }

        [Fact]
        public void LoadSecuritySchemeReference()
        {
            // Arrange
            OpenApiDocument document;
            var diagnostic = new OpenApiDiagnostic();

            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "multipleReferences.v2.yaml")))
            {
                document = new OpenApiStreamReader().Read(stream, out diagnostic);
            }

            var reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "api_key_sample"
            };

            // Act
            var referencedObject = document.ResolveReference(reference);

            // Assert
            referencedObject.ShouldBeEquivalentTo(
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    Name = "api_key",
                    In = ParameterLocation.Header,
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "api_key_sample"
                    }
                }
            );
        }

        [Fact]
        public void LoadResponseReference()
        {
            // Arrange
            OpenApiDocument document;
            var diagnostic = new OpenApiDiagnostic();

            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "multipleReferences.v2.yaml")))
            {
                document = new OpenApiStreamReader().Read(stream, out diagnostic);
            }

            var reference = new OpenApiReference
            {
                Type = ReferenceType.Response,
                Id = "NotFound"
            };

            // Act
            var referencedObject = document.ResolveReference(reference);

            // Assert
            referencedObject.ShouldBeEquivalentTo(
                new OpenApiResponse
                {
                    Description = "Entity not found.",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.Response,
                        Id = "NotFound"
                    },
                    Content = new Dictionary<string,OpenApiMediaType>
                    {
                        ["application/json"] = new OpenApiMediaType()
                    }
                }
            );
        }

        [Fact]
        public void LoadResponseAndSchemaReference()
        {
            // Arrange
            OpenApiDocument document;
            var diagnostic = new OpenApiDiagnostic();

            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "multipleReferences.v2.yaml")))
            {
                document = new OpenApiStreamReader().Read(stream, out diagnostic);
            }

            var reference = new OpenApiReference
            {
                Type = ReferenceType.Response,
                Id = "GeneralError"
            };

            // Act
            var referencedObject = document.ResolveReference(reference);

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
                                Description = "Sample description",
                                Required = new HashSet<string> {"name" },
                                Properties = {
                                    ["name"] = new OpenApiSchema()
                                    {
                                        Type = "string"
                                    },
                                    ["tag"] = new OpenApiSchema()
                                    {
                                        Type = "string"
                                    }
                                },

                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.Schema,
                                    Id = "SampleObject2"
                                }
                            }
                        }
                    },
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.Response,
                        Id = "GeneralError"
                    }
                }
            );
        }
    }
}