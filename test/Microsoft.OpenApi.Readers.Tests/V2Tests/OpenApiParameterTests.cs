// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.IO;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.V2;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V2Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiParameterTests
    {
        private const string SampleFolderPath = "V2Tests/Samples/OpenApiParameter";

        [Fact]
        public void ParseBodyParameterShouldSucceed()
        {
            MapNode node;
            // Arrange
            using (var stream = File.OpenRead(Path.Combine(SampleFolderPath, "bodyParameter.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var parameter = OpenApiV2Deserializer.LoadParameter(node);

            // Assert
            // Body parameter is currently not translated via LoadParameter.
            // This design may be revisited and this unit test may likely change.
            parameter.ShouldBeEquivalentTo(null);
        }

        [Fact]
        public void ParsePathParameterShouldSucceed()
        {
            MapNode node;
            // Arrange
            using (var stream = File.OpenRead(Path.Combine(SampleFolderPath, "pathParameter.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var parameter = OpenApiV2Deserializer.LoadParameter(node);

            // Assert
            parameter.ShouldBeEquivalentTo(
                new OpenApiParameter
                {
                    In = ParameterLocation.Path,
                    Name = "username",
                    Description = "username to fetch",
                    Required = true,
                    Schema = new OpenApiSchema
                    {
                        Type = "string"
                    }
                });
        }

        [Fact]
        public void ParseQueryParameterShouldSucceed()
        {
            MapNode node;
            // Arrange
            using (var stream = File.OpenRead(Path.Combine(SampleFolderPath, "queryParameter.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }
            // Act
            var parameter = OpenApiV2Deserializer.LoadParameter(node);

            // Assert
            parameter.ShouldBeEquivalentTo(
                new OpenApiParameter
                {
                    In = ParameterLocation.Query,
                    Name = "id",
                    Description = "ID of the object to fetch",
                    Required = false,
                    Schema = new OpenApiSchema
                    {
                        Type = "array",
                        Items = new OpenApiSchema
                        {
                            Type = "string"
                        }
                    },
                    Style = ParameterStyle.Form,
                    Explode = true
                });
        }

        [Fact]
        public void ParseFormDataParameterShouldSucceed()
        {
            MapNode node;
            // Arrange
            using (var stream = File.OpenRead(Path.Combine(SampleFolderPath, "formDataParameter.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var parameter = OpenApiV2Deserializer.LoadParameter(node);

            // Assert
            // Form data parameter is currently not translated via LoadParameter.
            // This design may be revisited and this unit test may likely change.
            parameter.ShouldBeEquivalentTo(null);
        }

        [Fact]
        public void ParseHeaderParameterShouldSucceed()
        {
            MapNode node;
            // Arrange
            using (var stream = File.OpenRead(Path.Combine(SampleFolderPath, "headerParameter.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var parameter = OpenApiV2Deserializer.LoadParameter(node);

            // Assert
            parameter.ShouldBeEquivalentTo(
                new OpenApiParameter
                {
                    In = ParameterLocation.Header,
                    Name = "token",
                    Description = "token to be passed as a header",
                    Required = true,
                    Style = ParameterStyle.Simple,
                    Schema = new OpenApiSchema
                    {
                        Type = "array",
                        Items = new OpenApiSchema
                        {
                            Type = "integer",
                            Format = "int64"
                        }
                    }
                });
        }
    }
}