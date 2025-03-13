// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Services;
using Microsoft.OpenApi.Tests.UtilityFiles;
using Microsoft.OpenApi.Writers;
using Moq;
using Xunit;

namespace Microsoft.OpenApi.Hidi.Tests
{
    public class OpenApiFilterServiceTests
    {
        private readonly OpenApiDocument _openApiDocumentMock;
        private readonly Mock<ILogger<OpenApiFilterServiceTests>> _mockLogger;
        private readonly ILogger<OpenApiFilterServiceTests> _logger;

        public OpenApiFilterServiceTests()
        {
            _openApiDocumentMock = OpenApiDocumentMock.CreateOpenApiDocument();
            _mockLogger = new();
            _logger = _mockLogger.Object;
        }

        [Theory]
        [InlineData("users.user.ListUser", null, 1)]
        [InlineData("users.user.GetUser", null, 1)]
        [InlineData("users.user.ListUser,users.user.GetUser", null, 2)]
        [InlineData("*", null, 12)]
        [InlineData("administrativeUnits.restore", null, 1)]
        [InlineData("graphService.GetGraphService", null, 1)]
        [InlineData(null, "users.user,applications.application", 3)]
        [InlineData(null, "^users\\.", 3)]
        [InlineData(null, "users.user", 2)]
        [InlineData(null, "applications.application", 1)]
        [InlineData(null, "reports.Functions", 2)]
        public void ReturnFilteredOpenApiDocumentBasedOnOperationIdsAndTags(string? operationIds, string? tags, int expectedPathCount)
        {
            // Act
            var predicate = OpenApiFilterService.CreatePredicate(operationIds, tags);
            var subsetOpenApiDocument = OpenApiFilterService.CreateFilteredDocument(_openApiDocumentMock, predicate);

            // Assert
            Assert.NotNull(subsetOpenApiDocument);
            Assert.NotEmpty(subsetOpenApiDocument.Paths);
            Assert.Equal(expectedPathCount, subsetOpenApiDocument.Paths.Count);
        }

        [Fact]
        public void ReturnFilteredOpenApiDocumentBasedOnPostmanCollection()
        {
            // Arrange
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UtilityFiles", "postmanCollection_ver2.json");
            var fileInput = new FileInfo(filePath);
            var stream = fileInput.OpenRead();

            // Act
            var requestUrls = OpenApiService.ParseJsonCollectionFile(stream, _logger);
            var predicate = OpenApiFilterService.CreatePredicate(requestUrls: requestUrls, source: _openApiDocumentMock);
            var subsetOpenApiDocument = OpenApiFilterService.CreateFilteredDocument(_openApiDocumentMock, predicate);

            // Assert
            Assert.NotNull(subsetOpenApiDocument);
            Assert.NotEmpty(subsetOpenApiDocument.Paths);
            Assert.Equal(3, subsetOpenApiDocument.Paths.Count);
        }

        // Create predicate based RequestUrls
        [Fact]
        public void TestPredicateFiltersUsingRelativeRequestUrls()
        {
            var openApiDocument = new OpenApiDocument
            {
                Info = new() { Title = "Test", Version = "1.0" },
                Servers = new List<OpenApiServer> { new() { Url = "https://localhost/" } },
                Paths = new()
                {
                    {"/foo", new OpenApiPathItem() {
                        Operations = new Dictionary<HttpMethod, OpenApiOperation>
                        {
                            { HttpMethod.Get, new() },
                            { HttpMethod.Patch, new() },
                            { HttpMethod.Post, new() }
                          }
                        }
                    }
                }
            };

            // Given a set of RequestUrls
            var requestUrls = new Dictionary<string, List<string>>
            {
                    {"/foo", new List<string> {"GET","POST"}}
            };

            // When
            var predicate = OpenApiFilterService.CreatePredicate(requestUrls: requestUrls, source: openApiDocument);

            // Then
            Assert.True(predicate("/foo", HttpMethod.Get, null!));
            Assert.True(predicate("/foo", HttpMethod.Post, null!));
            Assert.False(predicate("/foo", HttpMethod.Patch, null!));
        }

        [Fact]
        public void CreateFilteredDocumentUsingPredicateFromRequestUrl()
        {
            // Arrange
            var openApiDocument = new OpenApiDocument
            {
                Info = new() { Title = "Test", Version = "1.0" },
                Servers = new List<OpenApiServer> { new() { Url = "https://localhost/" } },
                Paths = new()
                {
                    ["/test/{id}"] = new OpenApiPathItem()
                    {
                        Operations = new Dictionary<HttpMethod, OpenApiOperation>
                        {
                            { HttpMethod.Get, new() },
                            { HttpMethod.Patch, new() }
                        },
                        Parameters =
                        [
                            new OpenApiParameter()
                            {
                                Name = "id",
                                In = ParameterLocation.Path,
                                Required = true,
                                Schema = new OpenApiSchema()
                                {
                                    Type = JsonSchemaType.String
                                }
                            }
                        ]
                    }


                }
            };

            var requestUrls = new Dictionary<string, List<string>>
            {
                    {"/test/{id}", new List<string> {"GET","PATCH"}}
            };

            // Act
            var predicate = OpenApiFilterService.CreatePredicate(requestUrls: requestUrls, source: openApiDocument);
            var subsetDoc = OpenApiFilterService.CreateFilteredDocument(openApiDocument, predicate);

            // Assert that there's only 1 parameter in the subset document
            Assert.NotNull(subsetDoc);
            Assert.NotEmpty(subsetDoc.Paths);
            Assert.Single(subsetDoc.Paths.First().Value.Parameters!);
        }

        [Fact]
        public void ShouldParseNestedPostmanCollection()
        {
            // Arrange
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UtilityFiles", "postmanCollection_ver3.json");
            var fileInput = new FileInfo(filePath);
            var stream = fileInput.OpenRead();

            // Act
            var requestUrls = OpenApiService.ParseJsonCollectionFile(stream, _logger);
            var pathCount = requestUrls.Count;

            // Assert
            Assert.NotNull(requestUrls);
            Assert.Equal(30, pathCount);
        }

        [Fact]
        public void ThrowsExceptionWhenUrlsInCollectionAreMissingFromSourceDocument()
        {
            // Arrange
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UtilityFiles", "postmanCollection_ver1.json");
            var fileInput = new FileInfo(filePath);
            var stream = fileInput.OpenRead();

            // Act
            var requestUrls = OpenApiService.ParseJsonCollectionFile(stream, _logger);

            // Assert
            var message = Assert.Throws<ArgumentException>(() =>
                OpenApiFilterService.CreatePredicate(requestUrls: requestUrls, source: _openApiDocumentMock)).Message;
            Assert.Equal("The urls in the Postman collection supplied could not be found.", message);
        }

        [Fact]
        public void ContinueProcessingWhenUrlsInCollectionAreMissingFromSourceDocument()
        {
            // Arrange
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UtilityFiles", "postmanCollection_ver4.json");
            var fileInput = new FileInfo(filePath);
            var stream = fileInput.OpenRead();

            // Act
            var requestUrls = OpenApiService.ParseJsonCollectionFile(stream, _logger);
            var pathCount = requestUrls.Count;
            var predicate = OpenApiFilterService.CreatePredicate(requestUrls: requestUrls, source: _openApiDocumentMock);
            var subsetOpenApiDocument = OpenApiFilterService.CreateFilteredDocument(_openApiDocumentMock, predicate);
            var subsetPathCount = subsetOpenApiDocument.Paths.Count;

            // Assert
            Assert.NotNull(subsetOpenApiDocument);
            Assert.NotEmpty(subsetOpenApiDocument.Paths);
            Assert.Equal(2, subsetPathCount);
            Assert.NotEqual(pathCount, subsetPathCount);
        }

        [Fact]
        public void ThrowsInvalidOperationExceptionInCreatePredicateWhenInvalidArgumentsArePassed()
        {
            // Act and Assert
            var message1 = Assert.Throws<InvalidOperationException>(() => OpenApiFilterService.CreatePredicate(null, null)).Message;
            Assert.Equal("Either operationId(s),tag(s) or Postman collection need to be specified.", message1);

            var message2 = Assert.Throws<InvalidOperationException>(() => OpenApiFilterService.CreatePredicate("users.user.ListUser", "users.user")).Message;
            Assert.Equal("Cannot specify both operationIds and tags at the same time.", message2);
        }

        [Fact]
        public async Task CopiesOverAllReferencedComponentsToTheSubsetDocumentCorrectly()
        {
            // Arrange
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UtilityFiles", "docWithReusableHeadersAndExamples.yaml");
            var operationIds = "getItems";

            // Act
            using var stream = File.OpenRead(filePath);
            var settings = new OpenApiReaderSettings();
            settings.AddYamlReader();
            var doc = (await OpenApiDocument.LoadAsync(stream, "yaml", settings)).Document;

            // validated the tags are read as references
            var openApiOperationTags = doc?.Paths["/items"].Operations?[HttpMethod.Get].Tags?.ToArray();
            Assert.NotNull(openApiOperationTags);
            Assert.Single(openApiOperationTags);
            Assert.True(openApiOperationTags[0].UnresolvedReference);

            var predicate = OpenApiFilterService.CreatePredicate(operationIds: operationIds);
            if (doc is not null)
            {
                var subsetOpenApiDocument = OpenApiFilterService.CreateFilteredDocument(doc, predicate);

                var response = subsetOpenApiDocument.Paths?["/items"].Operations?[HttpMethod.Get]?.Responses?["200"];
                var responseHeader = response?.Headers?["x-custom-header"];
                var mediaTypeExample = response?.Content?["application/json"]?.Examples?.First().Value;
                var targetHeaders = subsetOpenApiDocument.Components?.Headers;
                var targetExamples = subsetOpenApiDocument.Components?.Examples;

                // Assert
                Assert.Same(doc.Servers, subsetOpenApiDocument.Servers);
                var headerReference = Assert.IsType<OpenApiHeaderReference>(responseHeader);
                Assert.False(headerReference.UnresolvedReference);
                var exampleReference = Assert.IsType<OpenApiExampleReference>(mediaTypeExample);
                Assert.False(exampleReference?.UnresolvedReference);
                Assert.NotNull(targetHeaders);
                Assert.Single(targetHeaders);
                Assert.NotNull(targetExamples);
                Assert.Single(targetExamples);
                // validated the tags of the trimmed document are read as references
                var trimmedOpenApiOperationTags = subsetOpenApiDocument.Paths?["/items"].Operations?[HttpMethod.Get].Tags?.ToArray();
                Assert.NotNull(trimmedOpenApiOperationTags);
                Assert.Single(trimmedOpenApiOperationTags);
                Assert.True(trimmedOpenApiOperationTags[0].UnresolvedReference);

                // Finally try to write the trimmed document as v3 document
                var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
                var writer = new OpenApiJsonWriter(outputStringWriter)
                {
                    Settings = new OpenApiWriterSettings()
                    {
                        InlineExternalReferences = true,
                        InlineLocalReferences = true
                    }
                };
                subsetOpenApiDocument.SerializeAsV3(writer);
                await writer.FlushAsync();
                var result = outputStringWriter.ToString();
                Assert.NotEmpty(result);
            }            
        }

        [Theory]
        [InlineData("reports.getTeamsUserActivityUserDetail-a3f1", null)]
        [InlineData(null, "reports.Functions")]
        public void ReturnsPathParametersOnSlicingBasedOnOperationIdsOrTags(string? operationIds, string? tags)
        {
            // Act
            var predicate = OpenApiFilterService.CreatePredicate(operationIds, tags);
            var subsetOpenApiDocument = OpenApiFilterService.CreateFilteredDocument(_openApiDocumentMock, predicate);

            // Assert
            foreach (var pathItem in subsetOpenApiDocument.Paths)
            {
                Assert.True(pathItem.Value.Parameters!.Any());
                Assert.Single(pathItem.Value.Parameters!);
            }
        }
    }
}
