// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text;
using Castle.Core.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Hidi;
using Microsoft.OpenApi.Hidi.Handlers;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.OData;
using Microsoft.OpenApi.Services;
using Microsoft.VisualStudio.TestPlatform.Utilities;
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
        [InlineData("Todos.Todo.UpdateTodoById",null, 1)]
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
        
        [Theory]
        [InlineData("UtilityFiles/appsettingstest.json")]
        [InlineData(null)]
        public void ReturnOpenApiConvertSettingsWhenSettingsFileIsProvided(string filePath)
        {
            // Arrange
            var config = OpenApiService.GetConfiguration(filePath);

            // Act and Assert
            var settings = config.GetSection("OpenApiConvertSettings").Get<OpenApiConvertSettings>();

            if (filePath == null)
            {
                Assert.Null(settings);
            }
            else
            {
                Assert.NotNull(settings);
            }
        }


        [Fact]
        public void ShowCommandGeneratesMermaidDiagramAsMarkdown()
        {
            var openApiDoc = new OpenApiDocument();
            openApiDoc.Info = new OpenApiInfo
            {
                Title = "Test",
                Version = "1.0.0"
            };
            var stream = new MemoryStream();
            using var writer = new StreamWriter(stream);
            OpenApiService.WriteTreeDocumentAsMarkdown("https://example.org/openapi.json", openApiDoc, writer);
            writer.Flush();
            stream.Position = 0;
            using var reader = new StreamReader(stream);
            var output = reader.ReadToEnd();
            Assert.Contains("graph LR", output);
        }

        [Fact]
        public void ShowCommandGeneratesMermaidDiagramAsHtml ()
        {
            var openApiDoc = new OpenApiDocument();
            openApiDoc.Info = new OpenApiInfo
            {
                Title = "Test",
                Version = "1.0.0"
            };
            var stream = new MemoryStream();
            using var writer = new StreamWriter(stream);
            OpenApiService.WriteTreeDocumentAsHtml("https://example.org/openapi.json", openApiDoc, writer);
            writer.Flush();
            stream.Position = 0;
            using var reader = new StreamReader(stream);
            var output = reader.ReadToEnd();
            Assert.Contains("graph LR", output);
        }


        [Fact]
        public async Task ShowCommandGeneratesMermaidMarkdownFileWithMermaidDiagram()
        {
            var fileinfo = new FileInfo("sample.md");
            // create a dummy ILogger instance for testing
            await OpenApiService.ShowOpenApiDocument("UtilityFiles\\SampleOpenApi.yml", null, null, fileinfo, new Logger<OpenApiService>(new LoggerFactory()), new CancellationToken());

            var output = File.ReadAllText(fileinfo.FullName);
            Assert.Contains("graph LR", output);
        }


        [Fact]
        public async Task TransformCommandConvertsOpenApi()
        {
            var fileinfo = new FileInfo("sample.json");
            // create a dummy ILogger instance for testing
            await OpenApiService.TransformOpenApiDocument("UtilityFiles\\SampleOpenApi.yml",null, null,  fileinfo, true, null, null,false,null,false,false,null,null,null,new Logger<OpenApiService>(new LoggerFactory()), new CancellationToken());

            var output = File.ReadAllText("sample.json");
            Assert.NotEmpty(output);
        }

        [Fact]
        public void InvokeTransformCommand()
        {
            var rootCommand = Program.CreateRootCommand();
            var args = new string[] { "transform", "-d", ".\\UtilityFiles\\SampleOpenApi.yml", "-o", "sample.json" };
            var parseResult = rootCommand.Parse(args);
            var handler = rootCommand.Subcommands.Where(c => c.Name == "transform").First().Handler;
            var context = new InvocationContext(parseResult);

            handler.Invoke(context);

            var output = File.ReadAllText("sample.json");
            Assert.NotEmpty(output);
        }


        [Fact]
        public void InvokeShowCommand()
        {
            var rootCommand = Program.CreateRootCommand();
            var args = new string[] { "show", "-d", ".\\UtilityFiles\\SampleOpenApi.yml", "-o", "sample.md" };
            var parseResult = rootCommand.Parse(args);
            var handler = rootCommand.Subcommands.Where(c => c.Name == "show").First().Handler;
            var context = new InvocationContext(parseResult);

            handler.Invoke(context);

            var output = File.ReadAllText("sample.md");
            Assert.Contains("graph LR", output);
        }


        [Fact]
        public void InvokeShowCommandWithoutOutput()
        {
            var rootCommand = Program.CreateRootCommand();
            var args = new string[] { "show", "-d", ".\\UtilityFiles\\SampleOpenApi.yml" };
            var parseResult = rootCommand.Parse(args);
            var handler = rootCommand.Subcommands.Where(c => c.Name == "show").First().Handler;
            var context = new InvocationContext(parseResult);

            handler.Invoke(context);

            var output = File.ReadAllText(Path.Combine(Path.GetTempPath(), "apitree.html"));
            Assert.Contains("graph LR", output);
        }


        // Relatively useless test to keep the code coverage metrics happy
        [Fact]
        public void CreateRootCommand()
        {
            var rootCommand = Program.CreateRootCommand();
            Assert.NotNull(rootCommand);
        }
    }
}
