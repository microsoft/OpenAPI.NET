﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace Microsoft.OpenApi.Tests.Services
{
    public class OpenApiUrlTreeNodeTests
    {
        private OpenApiDocument OpenApiDocumentSample_1 => new()
        {
            Paths = new()
            {
                ["/"] = new OpenApiPathItem()
                {
                    Operations = new()
                    {
                        [HttpMethod.Get] = new(),
                    }
                },
                ["/houses"] = new OpenApiPathItem()
                {
                    Operations = new()
                    {
                        [HttpMethod.Get] = new(),
                        [HttpMethod.Post] = new()
                    }
                },
                ["/cars"] = new OpenApiPathItem()
                {
                    Operations = new()
                    {
                        [HttpMethod.Post] = new()
                    }
                }
            }
        };

        private OpenApiDocument OpenApiDocumentSample_2 => new()
        {
            Paths = new()
            {
                ["/"] = new OpenApiPathItem(),
                ["/hotels"] = new OpenApiPathItem(),
                ["/offices"] = new OpenApiPathItem()
            }
        };

        [Fact]
        public void CreateUrlSpaceWithoutOpenApiDocument()
        {
            var rootNode = OpenApiUrlTreeNode.Create();

            Assert.NotNull(rootNode);
        }

        [Fact]
        public void CreateSingleRootWorks()
        {
            var doc = new OpenApiDocument
            {
                Paths = new()
                {
                    ["/"] = new OpenApiPathItem()
                }
            };

            var label = "v1.0";
            var rootNode = OpenApiUrlTreeNode.Create(doc, label);

            Assert.NotNull(rootNode);
            Assert.NotNull(rootNode.PathItems);
            Assert.False(rootNode.HasOperations(label));
            Assert.Empty(rootNode.Children);
        }

        [Fact]
        public void CreatePathWithoutRootWorks()
        {
            var doc = new OpenApiDocument
            {
                Paths = new()
                {
                    ["/houses"] = new OpenApiPathItem()
                }
            };

            var label = "cabin";
            var rootNode = OpenApiUrlTreeNode.Create(doc, label);

            Assert.NotNull(rootNode);
            Assert.NotNull(rootNode.PathItems);
            Assert.Single(rootNode.Children);
            Assert.Equal("houses", rootNode.Children["houses"].Segment);
            Assert.NotNull(rootNode.Children["houses"].PathItems);
            Assert.False(rootNode.Children["houses"].HasOperations("cabin"));
        }

        [Fact]
        public void CreateMultiplePathsWorks()
        {
            var doc = OpenApiDocumentSample_1;

            var label = "assets";
            var rootNode = OpenApiUrlTreeNode.Create(doc, label);

            Assert.NotNull(rootNode);
            Assert.Equal(2, rootNode.Children.Count);
            Assert.Equal("houses", rootNode.Children["houses"].Segment);
            Assert.Equal("cars", rootNode.Children["cars"].Segment);
            Assert.True(rootNode.PathItems.ContainsKey(label));
            Assert.True(rootNode.Children["houses"].PathItems.ContainsKey(label));
            Assert.True(rootNode.Children["cars"].PathItems.ContainsKey(label));
        }

        [Fact]
        public void AttachDocumentWorks()
        {
            var doc1 = OpenApiDocumentSample_1;

            var doc2 = OpenApiDocumentSample_2;

            var label1 = "personal";
            var label2 = "business";
            var rootNode = OpenApiUrlTreeNode.Create(doc1, label1);
            rootNode.Attach(doc2, label2);

            Assert.NotNull(rootNode);
            Assert.Equal(4, rootNode.Children.Count);
            Assert.True(rootNode.Children["houses"].PathItems.ContainsKey(label1));
            Assert.True(rootNode.Children["offices"].PathItems.ContainsKey(label2));
        }

        [Fact]
        public void AttachPathWorks()
        {
            var doc = OpenApiDocumentSample_1;

            var label1 = "personal";
            var rootNode = OpenApiUrlTreeNode.Create(doc, label1);

            var pathItem1 = new OpenApiPathItem
            {
                Operations = new()
                {
                    {
                        HttpMethod.Get, new OpenApiOperation
                        {
                            OperationId = "motorcycles.ListMotorcycle",
                            Responses = new()
                            {
                                {
                                    "200", new OpenApiResponse()
                                    {
                                        Description = "Retrieved entities"
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var path1 = "/motorcycles";
            rootNode.Attach(path1, pathItem1, label1);

            var pathItem2 = new OpenApiPathItem
            {
                Operations = new()
                {
                    {
                        HttpMethod.Get, new OpenApiOperation
                        {
                            OperationId = "computers.ListComputer",
                            Responses = new()
                            {
                                {
                                    "200", new OpenApiResponse()
                                    {
                                        Description = "Retrieved entities"
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var path2 = "/computers";
            var label2 = "business";
            rootNode.Attach(path2, pathItem2, label2);

            Assert.Equal(4, rootNode.Children.Count);
            Assert.True(rootNode.Children.ContainsKey(path1[1..]));
            Assert.True(rootNode.Children.ContainsKey(path2[1..]));
            Assert.True(rootNode.Children[path1[1..]].PathItems.ContainsKey(label1));
            Assert.True(rootNode.Children[path2[1..]].PathItems.ContainsKey(label2));
        }

        [Fact]
        public void CreatePathsWithMultipleSegmentsWorks()
        {
            var doc = new OpenApiDocument
            {
                Paths = new()
                {
                    ["/"] = new OpenApiPathItem(),
                    ["/houses/apartments/{apartment-id}"] = new OpenApiPathItem(),
                    ["/cars/coupes"] = new OpenApiPathItem()
                }
            };

            var label = "assets";
            var rootNode = OpenApiUrlTreeNode.Create(doc, label);

            Assert.NotNull(rootNode);
            Assert.Equal(2, rootNode.Children.Count);
            Assert.NotNull(rootNode.Children["houses"].Children["apartments"].Children["{apartment-id}"].PathItems);
            Assert.True(rootNode.Children["houses"].Children["apartments"].Children["{apartment-id}"].PathItems.ContainsKey(label));
            Assert.True(rootNode.Children["cars"].Children["coupes"].PathItems.ContainsKey(label));
            Assert.True(rootNode.PathItems.ContainsKey(label));
            Assert.Equal("coupes", rootNode.Children["cars"].Children["coupes"].Segment);
        }

        [Fact]
        public void CreatePathsWithTrailingSlashWorks()
        {
            var doc = new OpenApiDocument
            {
                Paths = new()
                {
                    ["/"] = new OpenApiPathItem(),
                    ["/cars"] = new OpenApiPathItem(),
                    ["/cars/"] = new OpenApiPathItem(),
                    ["/cars/coupes"] = new OpenApiPathItem()
                }
            };

            var label = "assets";
            var rootNode = OpenApiUrlTreeNode.Create(doc, label);

            Assert.NotNull(rootNode);
            Assert.Single(rootNode.Children);
            var carsNode = rootNode.Children["cars"];
            Assert.Equal("cars", carsNode.Segment);
            Assert.Equal(2, carsNode.Children.Count);
            var emptyNode = carsNode.Children[""];
            Assert.Empty(emptyNode.Segment);
            Assert.Empty(emptyNode.Children);
            var coupesNode = carsNode.Children["coupes"];
            Assert.Equal("coupes", coupesNode.Segment);
            Assert.Empty(coupesNode.Children);
        }

        [Fact]
        public void CreatePathsWithTrailingSlashWorksInverted()
        {
            var doc = new OpenApiDocument
            {
                Paths = new()
                {
                    ["/"] = new OpenApiPathItem(),
                    ["/cars/"] = new OpenApiPathItem(),
                    ["/cars"] = new OpenApiPathItem(), // only difference from previous test, tests that node mapping is not order specific
                    ["/cars/coupes"] = new OpenApiPathItem()
                }
            };

            var label = "assets";
            var rootNode = OpenApiUrlTreeNode.Create(doc, label);

            Assert.NotNull(rootNode);
            Assert.Single(rootNode.Children);
            var carsNode = rootNode.Children["cars"];
            Assert.Equal("cars", carsNode.Segment);
            Assert.Equal(2, carsNode.Children.Count);
            var emptyNode = carsNode.Children[""];
            Assert.Empty(emptyNode.Segment);
            Assert.Empty(emptyNode.Children);
            var coupesNode = carsNode.Children["coupes"];
            Assert.Equal("coupes", coupesNode.Segment);
            Assert.Empty(coupesNode.Children);
        }

        [Fact]
        public void HasOperationsWorks()
        {
            var doc1 = new OpenApiDocument
            {
                Paths = new()
                {
                    ["/"] = new OpenApiPathItem(),
                    ["/houses"] = new OpenApiPathItem(),
                    ["/cars/{car-id}"] = new OpenApiPathItem()
                    {
                        Operations = new()
                        {
                            {
                                HttpMethod.Get, new OpenApiOperation
                                {
                                    OperationId = "cars.GetCar",
                                    Responses = new()
                                    {
                                        {
                                            "200", new OpenApiResponse()
                                            {
                                                Description = "Retrieved entity"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var doc2 = new OpenApiDocument
            {
                Paths = new()
                {
                    ["/cars/{car-id}"] = new OpenApiPathItem()
                    {
                        Operations = new()
                        {
                            {
                                HttpMethod.Get, new OpenApiOperation
                                {
                                    OperationId = "cars.GetCar",
                                    Responses = new()
                                    {
                                        {
                                            "200", new OpenApiResponse()
                                            {
                                                Description = "Retrieved entity"
                                            }
                                        }
                                    }
                                }
                            },
                            {
                                HttpMethod.Put, new OpenApiOperation
                                {
                                    OperationId = "cars.UpdateCar",
                                    Responses = new()
                                    {
                                        {
                                            "204", new OpenApiResponse()
                                            {
                                                Description = "Success."
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var label1 = "personal";
            var label2 = "business";
            var rootNode = OpenApiUrlTreeNode.Create(doc1, label1);
            rootNode.Attach(doc2, label2);

            Assert.NotNull(rootNode);

            Assert.Equal(2, rootNode.Children["cars"].Children["{car-id}"].PathItems.Count);
            Assert.True(rootNode.Children["cars"].Children["{car-id}"].PathItems.ContainsKey(label1));
            Assert.True(rootNode.Children["cars"].Children["{car-id}"].PathItems.ContainsKey(label2));

            Assert.False(rootNode.Children["houses"].HasOperations(label1));
            Assert.True(rootNode.Children["cars"].Children["{car-id}"].HasOperations(label1));
            Assert.True(rootNode.Children["cars"].Children["{car-id}"].HasOperations(label2));
            Assert.Single(rootNode.Children["cars"].Children["{car-id}"].PathItems[label1].Operations);
            Assert.Equal(2, rootNode.Children["cars"].Children["{car-id}"].PathItems[label2].Operations.Count);
        }

        [Fact]
        public void SegmentIsParameterWorks()
        {
            var doc = new OpenApiDocument
            {
                Paths = new()
                {
                    ["/"] = new OpenApiPathItem(),
                    ["/houses/apartments/{apartment-id}"] = new OpenApiPathItem()
                }
            };

            var label = "properties";
            var rootNode = OpenApiUrlTreeNode.Create(doc, label);

            Assert.NotNull(rootNode);
            Assert.Single(rootNode.Children);
            Assert.NotNull(rootNode.Children["houses"].Children["apartments"].Children["{apartment-id}"].PathItems);
            Assert.True(rootNode.Children["houses"].Children["apartments"].Children["{apartment-id}"].IsParameter);
            Assert.Equal("{apartment-id}", rootNode.Children["houses"].Children["apartments"].Children["{apartment-id}"].Segment);
        }

        [Fact]
        public void AdditionalDataWorks()
        {
            var doc = OpenApiDocumentSample_1;

            var label = "personal";
            var rootNode = OpenApiUrlTreeNode.Create(doc, label);

            var additionalData1 = new Dictionary<string, List<string>>
            {
                {"DatePurchased", new List<string> { "21st April 2021" } },
                {"Location", new List<string> { "Seattle, WA" } },
                {"TotalEstimateValue", new List<string> { "USD 2 Million" } },
                {"Owner", new List<string> { "John Doe, Mr" } }
            };
            rootNode.AddAdditionalData(additionalData1);

            var additionalData2 = new Dictionary<string, List<string>>
            {
                {"Bedrooms", new List<string> { "Five" } }
            };
            rootNode.Children["houses"].AddAdditionalData(additionalData2);

            var additionalData3 = new Dictionary<string, List<string>>
            {
                {"Categories", new List<string> { "Coupe", "Sedan", "Convertible" } }
            };
            rootNode.Children["cars"].AddAdditionalData(additionalData3);

            Assert.Equal(4, rootNode.AdditionalData.Count);
            Assert.True(rootNode.AdditionalData.ContainsKey("DatePurchased"));
            Assert.Collection(rootNode.AdditionalData["Location"],
                item => Assert.Equal("Seattle, WA", item));
            Assert.Collection(rootNode.Children["houses"].AdditionalData["Bedrooms"],
                item => Assert.Equal("Five", item));
            Assert.Collection(rootNode.Children["cars"].AdditionalData["Categories"],
                item => Assert.Equal("Coupe", item),
                item => Assert.Equal("Sedan", item),
                item => Assert.Equal("Convertible", item));
        }

        [Fact]
        public void ThrowsArgumentExceptionForDuplicateLabels()
        {
            var doc1 = OpenApiDocumentSample_1;

            var doc2 = OpenApiDocumentSample_2;

            var label1 = "personal";
            var rootNode = OpenApiUrlTreeNode.Create(doc1, label1);

            Assert.Throws<ArgumentException>(() => rootNode.Attach(doc2, label1));
        }

        [Fact]
        public void ThrowsArgumentNullExceptionForNullOrEmptyArgumentsInCreateMethod()
        {
            var doc = OpenApiDocumentSample_1;

            Assert.Throws<ArgumentNullException>(() => OpenApiUrlTreeNode.Create(doc, ""));
            Assert.Throws<ArgumentNullException>(() => OpenApiUrlTreeNode.Create(doc, null));
            Assert.Throws<ArgumentNullException>(() => OpenApiUrlTreeNode.Create(null, "beta"));
            Assert.Throws<ArgumentNullException>(() => OpenApiUrlTreeNode.Create(null, null));
            Assert.Throws<ArgumentNullException>(() => OpenApiUrlTreeNode.Create(null, ""));
        }

        [Fact]
        public void ThrowsArgumentNullExceptionForNullOrEmptyArgumentsInAttachMethod()
        {
            var doc1 = OpenApiDocumentSample_1;

            var doc2 = OpenApiDocumentSample_2;

            var label1 = "personal";
            var rootNode = OpenApiUrlTreeNode.Create(doc1, label1);

            Assert.Throws<ArgumentNullException>(() => rootNode.Attach(doc2, ""));
            Assert.Throws<ArgumentNullException>(() => rootNode.Attach(doc2, null));
            Assert.Throws<ArgumentNullException>(() => rootNode.Attach(null, "beta"));
            Assert.Throws<ArgumentNullException>(() => rootNode.Attach(null, null));
            Assert.Throws<ArgumentNullException>(() => rootNode.Attach(null, ""));
        }

        [Fact]
        public void ThrowsArgumentNullExceptionForNullOrEmptyArgumentInHasOperationsMethod()
        {
            var doc = OpenApiDocumentSample_1;

            var label = "personal";
            var rootNode = OpenApiUrlTreeNode.Create(doc, label);

            Assert.Throws<ArgumentNullException>(() => rootNode.HasOperations(null));
            Assert.Throws<ArgumentNullException>(() => rootNode.HasOperations(""));
        }

        [Fact]
        public void ThrowsArgumentNullExceptionForNullArgumentInAddAdditionalDataMethod()
        {
            var doc = OpenApiDocumentSample_1;

            var label = "personal";
            var rootNode = OpenApiUrlTreeNode.Create(doc, label);

            Assert.Throws<ArgumentNullException>(() => rootNode.AddAdditionalData(null));
        }

        [Fact]
        public async Task VerifyDiagramFromSampleOpenAPIAsync()
        {
            var doc1 = OpenApiDocumentSample_1;

            var label1 = "personal";
            var rootNode = OpenApiUrlTreeNode.Create(doc1, label1);

            var writer = new StringWriter();
            rootNode.WriteMermaid(writer);
            await writer.FlushAsync();
            var diagram = writer.GetStringBuilder().ToString();

            await Verifier.Verify(diagram);
        }

        public static TheoryData<string, string[], string> SupportsTrailingSlashesInPathData => new TheoryData<string, string[], string>
        {
            // Path, children up to second to leaf, last expected leaf node name, expected leaf node path
            { "/cars/{car-id}/build/", ["cars", "{car-id}", "build"], @"\cars\{car-id}\build\" },
            { "/cars/", ["cars"], @"\cars\" },
        };

        [Theory]
        [MemberData(nameof(SupportsTrailingSlashesInPathData))]
        public void SupportsTrailingSlashesInPath(string path, string[] childrenBeforeLastNode, string expectedLeafNodePath)
        {
            var openApiDocument = new OpenApiDocument
            {
                Paths = new()
                {
                    [path] = new OpenApiPathItem()
                }
            };

            var label = "trailing-slash";
            var rootNode = OpenApiUrlTreeNode.Create(openApiDocument, label);

            var secondToLeafNode = rootNode;
            foreach (var childName in childrenBeforeLastNode)
            {
                secondToLeafNode = secondToLeafNode.Children[childName];
            }

            Assert.True(secondToLeafNode.Children.TryGetValue(string.Empty, out var leafNode));
            Assert.Equal(expectedLeafNodePath, leafNode.Path);
            Assert.Empty(leafNode.Children);
        }
    }
}
