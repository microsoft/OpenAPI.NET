// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Json.Schema;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.ReferenceService
{
    [Collection("DefaultSettings")]
    public class TryLoadReferenceV2Tests
    {
        private const string SampleFolderPath = "ReferenceService/Samples/";

        [Fact]
        public void LoadParameterReference()
        {
            // Arrange
            OpenApiDocument document;

            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "multipleReferences.v2.yaml")))
            {
                document = new OpenApiStreamReader().Read(stream, out var diagnostic);
            }

            var reference = new OpenApiReference
            {
                Type = ReferenceType.Parameter,
                Id = "skipParam"
            };

            // Act
            var referencedObject = document.ResolveReferenceTo<OpenApiParameter>(reference);

            // Assert
            referencedObject.Should().BeEquivalentTo(
                new OpenApiParameter
                {
                    Name = "skip",
                    In = ParameterLocation.Query,
                    Description = "number of items to skip",
                    Required = true,
                    Schema = new JsonSchemaBuilder()
                    .Type(SchemaValueType.Integer)
                    .Format("int32"),
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

            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "multipleReferences.v2.yaml")))
            {
                document = new OpenApiStreamReader().Read(stream, out var diagnostic);
            }

            var reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "api_key_sample"
            };

            // Act
            var referencedObject = document.ResolveReferenceTo<OpenApiSecurityScheme>(reference);

            // Assert
            referencedObject.Should().BeEquivalentTo(
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    Name = "api_key",
                    In = ParameterLocation.Header,
                    Reference = new()
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

            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "multipleReferences.v2.yaml")))
            {
                document = new OpenApiStreamReader().Read(stream, out var diagnostic);
            }

            var reference = new OpenApiReference
            {
                Type = ReferenceType.Response,
                Id = "NotFound"
            };

            // Act
            var referencedObject = document.ResolveReferenceTo<OpenApiResponse>(reference);

            // Assert
            referencedObject.Should().BeEquivalentTo(
                new OpenApiResponse
                {
                    Description = "Entity not found.",
                    Reference = new()
                    {
                        Type = ReferenceType.Response,
                        Id = "NotFound"
                    },
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new()
                    }
                }
            );
        }

        [Fact]
        public void LoadResponseAndSchemaReference()
        {
            // Arrange
            OpenApiDocument document;

            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "multipleReferences.v2.yaml")))
            {
                document = new OpenApiStreamReader().Read(stream, out var diagnostic);
            }

            var reference = new OpenApiReference
            {
                Type = ReferenceType.Response,
                Id = "GeneralError"
            };

            // Act
            var referencedObject = document.ResolveReferenceTo<OpenApiResponse>(reference);

            // Assert
            referencedObject.Should().BeEquivalentTo(
                new OpenApiResponse
                {
                    Description = "General Error",
                    Content =
                    {
                        ["application/json"] = new()
                        {
                            Schema = new JsonSchemaBuilder()
                            .Ref("#/definitions/SampleObject2")
                            .Description("Sample description")
                            .Required("name")
                            .Properties(
                                ("name", new JsonSchemaBuilder().Type(SchemaValueType.String)),
                                ("tag", new JsonSchemaBuilder().Type(SchemaValueType.String)))
                        }
                    },
                    Reference = new()
                    {
                        Type = ReferenceType.Response,
                        Id = "GeneralError"
                    }
                }
            );
        }
    }
}
