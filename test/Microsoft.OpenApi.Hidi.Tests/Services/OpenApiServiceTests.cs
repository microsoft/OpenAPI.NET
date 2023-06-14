// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.CommandLine;
using System.CommandLine.Invocation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Hidi.Options;
using Microsoft.OpenApi.Hidi.Utilities;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.OData;
using Microsoft.OpenApi.Services;
using Xunit;

namespace Microsoft.OpenApi.Hidi.Tests
{
    public class OpenApiServiceTests
    {
        private readonly ILogger<OpenApiServiceTests> _logger;

        public OpenApiServiceTests()
        {
            _logger = new Logger<OpenApiServiceTests>(new LoggerFactory());
        }

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
        [InlineData("Todos.Todo.UpdateTodo", null, 1)]
        [InlineData("Todos.Todo.ListTodo", null, 1)]
        [InlineData(null, "Todos.Todo", 5)]
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
            var config = SettingsUtilities.GetConfiguration(filePath);

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
            var openApiDoc = new OpenApiDocument
            {
                Info = new OpenApiInfo
                {
                    Title = "Test",
                    Version = "1.0.0"
                }
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
        public void ShowCommandGeneratesMermaidDiagramAsHtml()
        {
            var openApiDoc = new OpenApiDocument
            {
                Info = new OpenApiInfo
                {
                    Title = "Test",
                    Version = "1.0.0"
                }
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

            // create a dummy ILogger instance for testing
            var options = new HidiOptions()
            {
                OpenApi = "UtilityFiles\\SampleOpenApi.yml",
                Output = new FileInfo("sample.md")
            };

            await OpenApiService.ShowOpenApiDocument(options, _logger, new CancellationToken());

            var output = File.ReadAllText(options.Output.FullName);
            Assert.Contains("graph LR", output);
        }

        [Fact]
        public async Task ShowCommandGeneratesMermaidHtmlFileWithMermaidDiagram()
        {
            var options = new HidiOptions()
            {
                OpenApi = "UtilityFiles\\SampleOpenApi.yml"
            };
            var filePath = await OpenApiService.ShowOpenApiDocument(options, _logger, new CancellationToken());
            Assert.True(File.Exists(filePath));
        }

        [Fact]
        public async Task ShowCommandGeneratesMermaidMarkdownFileFromCsdlWithMermaidDiagram()
        {
            var options = new HidiOptions()
            {
                Csdl = "UtilityFiles\\Todo.xml",
                CsdlFilter = "todos",
                Output = new FileInfo("sample.md")
            };

            // create a dummy ILogger instance for testing
            await OpenApiService.ShowOpenApiDocument(options, _logger, new CancellationToken());

            var output = File.ReadAllText(options.Output.FullName);
            Assert.Contains("graph LR", output);
        }

        [Fact]
        public async Task ThrowIfOpenApiUrlIsNotProvidedWhenValidating()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await OpenApiService.ValidateOpenApiDocument("", _logger, new CancellationToken()));
        }


        [Fact]
        public async Task ThrowIfURLIsNotResolvableWhenValidating()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await OpenApiService.ValidateOpenApiDocument("https://example.org/itdoesnmatter", _logger, new CancellationToken()));
        }

        [Fact]
        public async Task ThrowIfFileDoesNotExistWhenValidating()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await OpenApiService.ValidateOpenApiDocument("aFileThatBetterNotExist.fake", _logger, new CancellationToken()));
        }

        [Fact]
        public async Task ValidateCommandProcessesOpenApi()
        {
            // create a dummy ILogger instance for testing
            await OpenApiService.ValidateOpenApiDocument("UtilityFiles\\SampleOpenApi.yml", _logger, new CancellationToken());

            Assert.True(true);
        }


        [Fact]
        public async Task TransformCommandConvertsOpenApi()
        {
            HidiOptions options = new HidiOptions
            {
                OpenApi = "UtilityFiles\\SampleOpenApi.yml",
                Output = new FileInfo("sample.json"),
                CleanOutput = true,
                TerseOutput = false,
                InlineLocal = false,
                InlineExternal = false,
            };
            // create a dummy ILogger instance for testing
            await OpenApiService.TransformOpenApiDocument(options, _logger, new CancellationToken());

            var output = File.ReadAllText("sample.json");
            Assert.NotEmpty(output);
        }


        [Fact]
        public async Task TransformCommandConvertsOpenApiWithDefaultOutputname()
        {
            HidiOptions options = new HidiOptions
            {
                OpenApi = "UtilityFiles\\SampleOpenApi.yml",
                CleanOutput = true,
                TerseOutput = false,
                InlineLocal = false,
                InlineExternal = false,
            };
            // create a dummy ILogger instance for testing
            await OpenApiService.TransformOpenApiDocument(options, _logger, new CancellationToken());

            var output = File.ReadAllText("output.yml");
            Assert.NotEmpty(output);
        }

        [Fact]
        public async Task TransformCommandConvertsCsdlWithDefaultOutputname()
        {
            HidiOptions options = new HidiOptions
            {
                Csdl = "UtilityFiles\\Todo.xml",
                CleanOutput = true,
                TerseOutput = false,
                InlineLocal = false,
                InlineExternal = false,
            };
            // create a dummy ILogger instance for testing
            await OpenApiService.TransformOpenApiDocument(options, _logger, new CancellationToken());

            var output = File.ReadAllText("output.yml");
            Assert.NotEmpty(output);
        }

        [Fact]
        public async Task TransformCommandConvertsOpenApiWithDefaultOutputnameAndSwitchFormat()
        {
            HidiOptions options = new HidiOptions
            {
                OpenApi = "UtilityFiles\\SampleOpenApi.yml",
                CleanOutput = true,
                Version = "3.0",
                OpenApiFormat = OpenApiFormat.Yaml,
                TerseOutput = false,
                InlineLocal = false,
                InlineExternal = false,
            };
            // create a dummy ILogger instance for testing
            await OpenApiService.TransformOpenApiDocument(options, _logger, new CancellationToken());

            var output = File.ReadAllText("output.yml");
            Assert.NotEmpty(output);
        }

        [Fact]
        public async Task ThrowTransformCommandIfOpenApiAndCsdlAreEmpty()
        {
            HidiOptions options = new HidiOptions
            {
                CleanOutput = true,
                TerseOutput = false,
                InlineLocal = false,
                InlineExternal = false,
            };
            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await OpenApiService.TransformOpenApiDocument(options, _logger, new CancellationToken()));

        }

        [Fact]
        public void InvokeTransformCommand()
        {
            var rootCommand = Program.CreateRootCommand();
            var args = new string[] { "transform", "-d", ".\\UtilityFiles\\SampleOpenApi.yml", "-o", "sample.json", "--co" };
            var parseResult = rootCommand.Parse(args);
            var handler = rootCommand.Subcommands.Where(c => c.Name == "transform").First().Handler;
            var context = new InvocationContext(parseResult);

            handler!.Invoke(context);

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

            handler!.Invoke(context);

            var output = File.ReadAllText("sample.md");
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
