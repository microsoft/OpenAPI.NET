// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Services;
using Xunit;

namespace Microsoft.OpenApi.Tests.Services
{
    public class OpenApiUrlTreeNodeTests
    {
        [Fact]
        public void CreateEmptyUrlTreeNode()
        {
            var doc = new OpenApiDocument() { };

            var rootNode = OpenApiUrlTreeNode.Create(doc);
            var rootNode1 = OpenApiUrlTreeNode.Create(null);

            Assert.NotNull(rootNode);
            Assert.NotNull(rootNode1);
        }

        [Fact]
        public void CreateSingleRootWorks()
        {
            var doc = new OpenApiDocument()
            {
                Paths = new OpenApiPaths()
                {
                    ["/"] = new OpenApiPathItem()
                }
            };

            var rootNode = OpenApiUrlTreeNode.Create(doc);

            Assert.NotNull(rootNode);
            Assert.NotNull(rootNode.PathItem);
            Assert.Equal(0, rootNode.Children.Count);
        }

        [Fact]
        public void CreatePathWithoutRootWorks()
        {
            var doc = new OpenApiDocument()
            {
                Paths = new OpenApiPaths()
                {
                    ["/home"] = new OpenApiPathItem()
                }
            };

            var rootNode = OpenApiUrlTreeNode.Create(doc);

            Assert.NotNull(rootNode);
            Assert.Null(rootNode.PathItem);
            Assert.Equal(1, rootNode.Children.Count);
            Assert.Equal("home", rootNode.Children["home"].Segment);
            Assert.NotNull(rootNode.Children["home"].PathItem);
        }

        [Fact]
        public void CreateMultiplePathsWorks()
        {
            var doc = new OpenApiDocument()
            {
                Paths = new OpenApiPaths()
                {
                    ["/"] = new OpenApiPathItem(),
                    ["/home"] = new OpenApiPathItem(),
                    ["/start"] = new OpenApiPathItem()
                }
            };

            var rootNode = OpenApiUrlTreeNode.Create(doc, "Ensuite");

            Assert.NotNull(rootNode);
            Assert.Equal(2, rootNode.Children.Count);
            Assert.Equal("home", rootNode.Children["home"].Segment);
            Assert.Equal("start", rootNode.Children["start"].Segment);
            Assert.Equal("Ensuite", rootNode.Label);
            Assert.Equal("Ensuite", rootNode.Children["home"].Label);
            Assert.Equal("Ensuite", rootNode.Children["start"].Label);
        }

        [Fact]
        public void AttachDocumentWorks()
        {
            var doc1 = new OpenApiDocument()
            {
                Paths = new OpenApiPaths()
                {
                    ["/"] = new OpenApiPathItem(),
                    ["/home"] = new OpenApiPathItem(),
                    ["/car"] = new OpenApiPathItem()
                }
            };

            var doc2 = new OpenApiDocument()
            {
                Paths = new OpenApiPaths()
                {
                    ["/"] = new OpenApiPathItem(),
                    ["/hotel"] = new OpenApiPathItem(),
                    ["/car"] = new OpenApiPathItem()
                }
            };

            var rootNode = OpenApiUrlTreeNode.Create(doc1, "Penthouse");
            rootNode.Attach(doc2, "Five-star");

            Assert.NotNull(rootNode);
            Assert.Equal(3, rootNode.Children.Count);
            Assert.Equal("Penthouse", rootNode.Children["home"].Label);
            Assert.Equal("Five-star", rootNode.Children["hotel"].Label);
        }

        [Fact]
        public void CreatePathsWithMultipleSegmentsWorks()
        {
            var doc = new OpenApiDocument()
            {
                Paths = new OpenApiPaths()
                {
                    ["/"] = new OpenApiPathItem(),
                    ["/home/sweet/home"] = new OpenApiPathItem(),
                    ["/start/end"] = new OpenApiPathItem()
                }
            };

            var rootNode = OpenApiUrlTreeNode.Create(doc);

            Assert.NotNull(rootNode);
            Assert.Equal(2, rootNode.Children.Count);
            Assert.NotNull(rootNode.Children["home"].Children["sweet"].Children["home"].PathItem);
            Assert.Equal("end", rootNode.Children["start"].Children["end"].Segment);
        }

        [Fact]
        public void HasOperationsWorks()
        {
            var doc = new OpenApiDocument()
            {
                Paths = new OpenApiPaths()
                {
                    ["/"] = new OpenApiPathItem(),
                    ["/home"] = new OpenApiPathItem(),
                    ["/car"] = new OpenApiPathItem()
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>
                        {
                            {
                                OperationType.Get, new OpenApiOperation
                                {
                                    OperationId = "car.GetCar",
                                    Responses = new OpenApiResponses()
                                    {
                                        {
                                            "200", new OpenApiResponse()
                                            {
                                                Description = "OK"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var rootNode = OpenApiUrlTreeNode.Create(doc);
            Assert.NotNull(rootNode);
            Assert.False(rootNode.Children["home"].HasOperations());
            Assert.True(rootNode.Children["car"].HasOperations());
        }

        [Fact]
        public void SegmentIsParameterWorks()
        {
            var doc = new OpenApiDocument()
            {
                Paths = new OpenApiPaths()
                {
                    ["/"] = new OpenApiPathItem(),
                    ["/home/bedroom/{bedroom-id}"] = new OpenApiPathItem()
                }
            };

            var rootNode = OpenApiUrlTreeNode.Create(doc);

            Assert.NotNull(rootNode);
            Assert.Equal(1, rootNode.Children.Count);
            Assert.NotNull(rootNode.Children["home"].Children["bedroom"].Children["{bedroom-id}"].PathItem);
            Assert.True(rootNode.Children["home"].Children["bedroom"].Children["{bedroom-id}"].IsParameter);
            Assert.Equal("{bedroom-id}", rootNode.Children["home"].Children["bedroom"].Children["{bedroom-id}"].Segment);
        }
    }
}
