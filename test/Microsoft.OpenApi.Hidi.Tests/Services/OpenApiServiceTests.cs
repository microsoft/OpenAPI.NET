// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.ApiManifest.OpenAI;
using Microsoft.OpenApi.Hidi.Options;
using Microsoft.OpenApi.Hidi.Utilities;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.OData;
using Microsoft.OpenApi.Services;
using Xunit;

namespace Microsoft.OpenApi.Hidi.Tests
{
    public sealed class OpenApiServiceTests : IDisposable
    {
        private readonly ILogger<OpenApiServiceTests> _logger;
        private readonly LoggerFactory _loggerFactory = new();

        public OpenApiServiceTests()
        {
            _logger = new Logger<OpenApiServiceTests>(_loggerFactory);
        }

        [Fact]
        public async Task ReturnConvertedCSDLFile()
        {
            // Arrange
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UtilityFiles", "Todo.xml");
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
        public async Task ReturnFilteredOpenApiDocBasedOnOperationIdsAndInputCsdlDocument(string? operationIds, string? tags, int expectedPathCount)
        {
            // Arrange
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UtilityFiles", "Todo.xml");
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
        public void ReturnOpenApiConvertSettingsWhenSettingsFileIsProvided(string? filePath)
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
                Info = new()
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
            Assert.Contains("graph LR", output, StringComparison.Ordinal);
        }

        [Fact]
        public void ShowCommandGeneratesMermaidDiagramAsHtml()
        {
            var openApiDoc = new OpenApiDocument
            {
                Info = new()
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
            Assert.Contains("graph LR", output, StringComparison.Ordinal);
        }

        [Fact]
        public async Task ShowCommandGeneratesMermaidMarkdownFileWithMermaidDiagram()
        {
            // create a dummy ILogger instance for testing
            var options = new HidiOptions
            {
                OpenApi = Path.Combine("UtilityFiles", "SampleOpenApi.yml"),
                Output = new("sample.md")
            };

            await OpenApiService.ShowOpenApiDocument(options, _logger);

            var output = await File.ReadAllTextAsync(options.Output.FullName);
            Assert.Contains("graph LR", output, StringComparison.Ordinal);
        }

        [Fact]
        public async Task ShowCommandGeneratesMermaidHtmlFileWithMermaidDiagram()
        {
            var options = new HidiOptions
            {
                OpenApi = Path.Combine("UtilityFiles", "SampleOpenApi.yml")
            };
            var filePath = await OpenApiService.ShowOpenApiDocument(options, _logger);
            Assert.True(File.Exists(filePath));
        }

        [Fact]
        public async Task ShowCommandGeneratesMermaidMarkdownFileFromCsdlWithMermaidDiagram()
        {
            var options = new HidiOptions
            {
                Csdl = Path.Combine("UtilityFiles", "Todo.xml"),
                CsdlFilter = "todos",
                Output = new("sample.md")
            };

            // create a dummy ILogger instance for testing
            await OpenApiService.ShowOpenApiDocument(options, _logger);

            var output = await File.ReadAllTextAsync(options.Output.FullName);
            Assert.Contains("graph LR", output, StringComparison.Ordinal);
        }

        [Fact]
        public Task ThrowIfOpenApiUrlIsNotProvidedWhenValidating()
        {
            return Assert.ThrowsAsync<ArgumentNullException>(() =>
                OpenApiService.ValidateOpenApiDocument("", _logger));
        }

        [Fact]
        public Task ThrowIfURLIsNotResolvableWhenValidating()
        {
            return Assert.ThrowsAsync<InvalidOperationException>(() =>
                OpenApiService.ValidateOpenApiDocument("https://example.org/itdoesnmatter", _logger));
        }

        [Fact]
        public Task ThrowIfFileDoesNotExistWhenValidating()
        {
            return Assert.ThrowsAsync<InvalidOperationException>(() =>
                OpenApiService.ValidateOpenApiDocument("aFileThatBetterNotExist.fake", _logger));
        }

        [Fact]
        public async Task ValidateCommandProcessesOpenApi()
        {
            // create a dummy ILogger instance for testing
            await OpenApiService.ValidateOpenApiDocument(Path.Combine("UtilityFiles", "SampleOpenApi.yml"), _logger);

            Assert.True(true);
        }

        [Fact]
        public async Task ValidFileReturnsTrue()
        {
            var isValid = await OpenApiService.ValidateOpenApiDocument(Path.Combine("UtilityFiles", "SampleOpenApi.yml"), _logger);

            Assert.True(isValid);
        }

        [Fact]
        public async Task InvalidFileReturnsFalse()
        {
            var isValid = await OpenApiService.ValidateOpenApiDocument(Path.Combine("UtilityFiles", "InvalidSampleOpenApi.yml"), _logger);

            Assert.False(isValid);
        }

        [Fact]
        public async Task CancellingValidationReturnsNull()
        {
            using var cts = new CancellationTokenSource();
            await cts.CancelAsync();
            var isValid = await OpenApiService.ValidateOpenApiDocument(Path.Combine("UtilityFiles", "SampleOpenApi.yml"), _logger, cts.Token);

            Assert.Null(isValid);
        }

        [Fact]
        public async Task TransformCommandConvertsOpenApi()
        {
            var options = new HidiOptions
            {
                OpenApi = Path.Combine("UtilityFiles", "SampleOpenApi.yml"),
                Output = new("sample.json"),
                CleanOutput = true,
                TerseOutput = false,
                InlineLocal = false,
                InlineExternal = false,
            };
            // create a dummy ILogger instance for testing
            await OpenApiService.TransformOpenApiDocument(options, _logger);

            var output = await File.ReadAllTextAsync("sample.json");
            Assert.NotEmpty(output);
        }


        [Fact]
        public async Task TransformCommandConvertsOpenApiWithDefaultOutputName()
        {
            var options = new HidiOptions
            {
                OpenApi = Path.Combine("UtilityFiles", "SampleOpenApi.yml"),
                CleanOutput = true,
                TerseOutput = false,
                InlineLocal = false,
                InlineExternal = false,
            };
            // create a dummy ILogger instance for testing
            await OpenApiService.TransformOpenApiDocument(options, _logger);

            var output = await File.ReadAllTextAsync("output.yml");
            Assert.NotEmpty(output);
        }

        [Fact]
        public async Task TransformCommandConvertsCsdlWithDefaultOutputName()
        {
            var options = new HidiOptions
            {
                Csdl = Path.Combine("UtilityFiles", "Todo.xml"),
                CleanOutput = true,
                TerseOutput = false,
                InlineLocal = false,
                InlineExternal = false,
            };
            // create a dummy ILogger instance for testing
            await OpenApiService.TransformOpenApiDocument(options, _logger);

            var output = await File.ReadAllTextAsync("output.yml");
            Assert.NotEmpty(output);
        }

        [Fact]
        public async Task TransformCommandConvertsOpenApiWithDefaultOutputNameAndSwitchFormat()
        {
            var options = new HidiOptions
            {
                OpenApi = Path.Combine("UtilityFiles", "SampleOpenApi.yml"),
                CleanOutput = true,
                Version = "3.0",
                OpenApiFormat = OpenApiFormat.Yaml,
                TerseOutput = false,
                InlineLocal = false,
                InlineExternal = false,
            };
            // create a dummy ILogger instance for testing
            await OpenApiService.TransformOpenApiDocument(options, _logger);

            var output = await File.ReadAllTextAsync("output.yml");
            Assert.NotEmpty(output);
        }

        [Fact]
        public Task ThrowTransformCommandIfOpenApiAndCsdlAreEmpty()
        {
            var options = new HidiOptions
            {
                CleanOutput = true,
                TerseOutput = false,
                InlineLocal = false,
                InlineExternal = false,
            };
            return Assert.ThrowsAsync<ArgumentException>(() =>
                OpenApiService.TransformOpenApiDocument(options, _logger));
        }

        [Fact]
        public async Task TransformToPowerShellCompliantOpenApi()
        {
            var settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UtilityFiles", "examplepowershellsettings.json");
            var options = new HidiOptions
            {
                OpenApi = Path.Combine("UtilityFiles", "SampleOpenApi.yml"),
                CleanOutput = true,
                Version = "3.0",
                OpenApiFormat = OpenApiFormat.Yaml,
                TerseOutput = false,
                InlineLocal = false,
                InlineExternal = false,
                SettingsConfig = SettingsUtilities.GetConfiguration(settingsPath)
            };
            // create a dummy ILogger instance for testing
            await OpenApiService.TransformOpenApiDocument(options, _logger);

            var output = await File.ReadAllTextAsync("output.yml");
            Assert.NotEmpty(output);
        }

        [Fact]
        public void InvokeTransformCommand()
        {
            var rootCommand = Program.CreateRootCommand();
            var openapi = Path.Combine(".", "UtilityFiles", "SampleOpenApi.yml");
            var args = new[] { "transform", "-d", openapi, "-o", "sample.json", "--co" };
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
            var openApi = Path.Combine(".", "UtilityFiles", "SampleOpenApi.yml");
            var args = new[] { "show", "-d", openApi, "-o", "sample.md" };
            var parseResult = rootCommand.Parse(args);
            var handler = rootCommand.Subcommands.Where(c => c.Name == "show").First().Handler;
            var context = new InvocationContext(parseResult);

            handler!.Invoke(context);

            var output = File.ReadAllText("sample.md");
            Assert.Contains("graph LR", output, StringComparison.Ordinal);
        }

        [Fact]
        public void InvokePluginCommand()
        {
            var rootCommand = Program.CreateRootCommand();
            var manifest = Path.Combine(".", "UtilityFiles", "exampleapimanifest.json");
            var args = new[] { "plugin", "-m", manifest, "--of", AppDomain.CurrentDomain.BaseDirectory };
            var parseResult = rootCommand.Parse(args);
            var handler = rootCommand.Subcommands.Where(c => c.Name == "plugin").First().Handler;
            var context = new InvocationContext(parseResult);

            handler!.Invoke(context);

            using var jsDoc = JsonDocument.Parse(File.ReadAllText("ai-plugin.json"));
            var openAiManifest = OpenAIPluginManifest.Load(jsDoc.RootElement);
            
            Assert.NotNull(openAiManifest);
            Assert.Equal("Mastodon - Subset", openAiManifest.NameForHuman);
            Assert.NotNull(openAiManifest.Api);
            Assert.Equal("openapi", openAiManifest.Api.Type);
            Assert.Equal("./openapi.json", openAiManifest.Api.Url);
        }


        // Relatively useless test to keep the code coverage metrics happy
        [Fact]
        public void CreateRootCommand()
        {
            var rootCommand = Program.CreateRootCommand();
            Assert.NotNull(rootCommand);
        }

        public void Dispose()
        {
            _loggerFactory.Dispose();
        }
    }
}
