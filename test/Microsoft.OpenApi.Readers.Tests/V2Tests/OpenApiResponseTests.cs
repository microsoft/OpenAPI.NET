// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.V2;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V2Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiResponseTests
    {
        private const string SampleFolderPath = "V2Tests/Samples/OpenApiResponse/";

        [Fact]
        public void ParseResponseWithExamplesShouldSucceed()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "responseWithExamples.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            node.Context.SetTempStorage(TempStorageKeys.OperationProduces, new List<string>()
            {
                "application/json"
            });

            // Act
            var response = OpenApiV2Deserializer.LoadResponse(node);

            // Assert
            response.ShouldBeEquivalentTo(
                new OpenApiResponse()
                {
                    Description = "An array of float response",
                    Content =
                    {
                        ["application/json"] = new OpenApiMediaType()
                        {
                            Schema = new OpenApiSchema()
                            {
                                Type = "array",
                                Items = new OpenApiSchema()
                                {
                                    Type = "number",
                                    Format = "float"
                                }
                            },
                            Example = new OpenApiArray()
                            {
                                new OpenApiFloat(5),
                                new OpenApiFloat(6),
                                new OpenApiFloat(7),
                            }
                        }
                    }
                });
        }
    }
}