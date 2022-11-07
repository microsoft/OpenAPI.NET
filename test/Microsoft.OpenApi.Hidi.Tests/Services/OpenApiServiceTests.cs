// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Hidi;
using Microsoft.OpenApi.OData;
using Microsoft.OpenApi.Services;
using Xunit;

namespace Microsoft.OpenApi.Tests.Services
{
    public class OpenApiServiceTests
    {
        [Fact]
        public async Task ReturnConvertedCSDLFile()
        {
            // Arrange
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UtilityFiles\\Todo.xml");
            var fileInput = new FileInfo(filePath);
            var csdlStream = fileInput.OpenRead();

            // Act
            var openApiDoc = await OpenApiService.ConvertCsdlToOpenApi(csdlStream);
            var expectedPathCount = 5;

            // Assert
            Assert.NotNull(openApiDoc);
            Assert.NotEmpty(openApiDoc.Paths);
            Assert.Equal(expectedPathCount, openApiDoc.Paths.Count);
        }
        
        [Theory]
        [InlineData("Todos.Todo.UpdateTodo",null, 1)]
        [InlineData("Todos.Todo.ListTodo",null, 1)]
        [InlineData(null, "Todos.Todo", 4)]
        public async Task ReturnFilteredOpenApiDocBasedOnOperationIdsAndInputCsdlDocument(string operationIds, string tags, int expectedPathCount)
        {
            // Arrange
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UtilityFiles\\Todo.xml");
            var fileInput = new FileInfo(filePath);
            var csdlStream = fileInput.OpenRead();

            // Act
            var openApiDoc = await OpenApiService.ConvertCsdlToOpenApi(csdlStream);
            var predicate = OpenApiFilterService.CreatePredicate(operationIds, tags);
            var subsetOpenApiDocument = OpenApiFilterService.CreateFilteredDocument(openApiDoc, predicate);

            // Assert
            Assert.NotNull(subsetOpenApiDocument);
            Assert.NotEmpty(subsetOpenApiDocument.Paths);
            Assert.Equal(expectedPathCount, subsetOpenApiDocument.Paths.Count);
        }
        
        [Fact]
        public void ReturnOpenApiConvertSettings()
        {
            // Arrange
            var filePath = "C:/Users/v-makim/source/repos/OpenAPI.NET/test/Microsoft.OpenApi.Hidi.Tests/UtilityFiles/appsettingstest.json";
            var config = OpenApiService.GetConfiguration(filePath);
            
            // Act
            var settings = config.GetSection("OpenApiConvertSettings").Get<OpenApiConvertSettings>();

            // Assert
            Assert.NotNull(settings);
        }
    }
}
