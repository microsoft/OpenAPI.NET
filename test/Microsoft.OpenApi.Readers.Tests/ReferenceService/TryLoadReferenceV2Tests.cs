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
            //var referencedObject = document.ResolveReferenceTo<JsonSchema>(reference);

            //// Assert
            //referencedObject.Should().BeEquivalentTo(
            //    new JsonSchemaBuilder()
            //    .Required("id", "name")
            //    .Properties(
            //        ("id", new JsonSchemaBuilder().Type(SchemaValueType.Integer).Format("int64")),
            //        ("name", new JsonSchemaBuilder().Type(SchemaValueType.String)),
            //        ("tag", new JsonSchemaBuilder().Type(SchemaValueType.String)))
            //    .Ref("SampleObject"));
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
                    .Format("int32")
                    .Ref("skipParam")
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
            var referencedObject = document.ResolveReferenceTo<OpenApiSecurityScheme>(reference);

            // Assert
            referencedObject.Should().BeEquivalentTo(
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
            var referencedObject = document.ResolveReferenceTo<OpenApiResponse>(reference);

            // Assert
            referencedObject.Should().BeEquivalentTo(
                new OpenApiResponse
                {
                    Description = "Entity not found.",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.Response,
                        Id = "NotFound"
                    },
                    Content = new Dictionary<string, OpenApiMediaType>
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
            var referencedObject = document.ResolveReferenceTo<OpenApiResponse>(reference);

            // Assert
            referencedObject.Should().BeEquivalentTo(
                new OpenApiResponse
                {
                    Description = "General Error",
                    Content =
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Schema = new JsonSchemaBuilder()
                            .Description("Sample description")
                            .Required("name")
                            .Properties(
                                ("name", new JsonSchemaBuilder().Type(SchemaValueType.String)),
                                ("tag", new JsonSchemaBuilder().Type(SchemaValueType.String)))
                            .Ref("#/components/schemas/SampleObject2")
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
