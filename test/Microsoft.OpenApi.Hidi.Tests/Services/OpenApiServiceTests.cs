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
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Readers;
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
            OpenApiReaderRegistry.RegisterReader(OpenApiConstants.Yml, new OpenApiYamlReader());
            OpenApiReaderRegistry.RegisterReader(OpenApiConstants.Yaml, new OpenApiYamlReader());
        }

        [Fact]
        public void CreateFilteredDocumentOnMinimalOpenApi()
        {
            // Arrange

            // We create a minimal OpenApiDocument with a single path and operation.
            var openApiDoc = new OpenApiDocument
            {
                Info = new()
                {
                    Title = "Test",
                    Version = "1.0.0"
                },
                Paths = new()
                {
                    ["/test"] = new OpenApiPathItem()
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>
                        {
                            [OperationType.Get] = new OpenApiOperation()
                        }
                    }
                }
            };

            // Act
            var requestUrls = new Dictionary<string, List<string>>()
            {
                { "/test", ["GET"] }
            };
            var filterPredicate = OpenApiFilterService.CreatePredicate(null, null, requestUrls, openApiDoc);
            var filteredDocument = OpenApiFilterService.CreateFilteredDocument(openApiDoc, filterPredicate);

            // Assert
            Assert.NotNull(filteredDocument);
            Assert.NotNull(filteredDocument.Paths);
            Assert.Single(filteredDocument.Paths);
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
        public async Task ShowCommandGeneratesMermaidMarkdownFileWithMermaidDiagramAsync()
        {
            // create a dummy ILogger instance for testing
            var options = new HidiOptions
            {
                OpenApi = Path.Combine("UtilityFiles", "SampleOpenApi.yml"),
                Output = new("sample.md")
            };

            await OpenApiService.ShowOpenApiDocumentAsync(options, _logger);

            var output = await File.ReadAllTextAsync(options.Output.FullName);
            Assert.Contains("graph LR", output, StringComparison.Ordinal);
        }

        [Fact]
        public async Task ShowCommandGeneratesMermaidHtmlFileWithMermaidDiagramAsync()
        {
            var options = new HidiOptions
            {
                OpenApi = Path.Combine("UtilityFiles", "SampleOpenApi.yml")
            };
            var filePath = await OpenApiService.ShowOpenApiDocumentAsync(options, _logger);
            Assert.True(File.Exists(filePath));
        }

        [Fact]
        public Task ThrowIfOpenApiUrlIsNotProvidedWhenValidatingAsync()
        {
            return Assert.ThrowsAsync<ArgumentNullException>(() =>
                OpenApiService.ValidateOpenApiDocumentAsync("", _logger));
        }

        [Fact]
        public Task ThrowIfURLIsNotResolvableWhenValidatingAsync()
        {
            return Assert.ThrowsAsync<InvalidOperationException>(() =>
                OpenApiService.ValidateOpenApiDocumentAsync("https://example.org/itdoesnmatter", _logger));
        }

        [Fact]
        public Task ThrowIfFileDoesNotExistWhenValidatingAsync()
        {
            return Assert.ThrowsAsync<InvalidOperationException>(() =>
                OpenApiService.ValidateOpenApiDocumentAsync("aFileThatBetterNotExist.fake", _logger));
        }

        [Fact]
        public async Task ValidateCommandProcessesOpenApiAsync()
        {
            // create a dummy ILogger instance for testing
            await OpenApiService.ValidateOpenApiDocumentAsync(Path.Combine("UtilityFiles", "SampleOpenApi.yml"), _logger);

            Assert.True(true);
        }

        [Fact]
        public async Task ValidFileReturnsTrueAsync()
        {
            var isValid = await OpenApiService.ValidateOpenApiDocumentAsync(Path.Combine("UtilityFiles", "SampleOpenApi.yml"), _logger);

            Assert.True(isValid);
        }

        [Fact]
        public async Task InvalidFileReturnsFalseAsync()
        {
            var isValid = await OpenApiService.ValidateOpenApiDocumentAsync(Path.Combine("UtilityFiles", "InvalidSampleOpenApi.yml"), _logger);

            Assert.False(isValid);
        }

        [Fact]
        public async Task CancellingValidationReturnsNullAsync()
        {
            using var cts = new CancellationTokenSource();
            await cts.CancelAsync();
            var isValid = await OpenApiService.ValidateOpenApiDocumentAsync(Path.Combine("UtilityFiles", "SampleOpenApi.yml"), _logger, cts.Token);

            Assert.Null(isValid);
        }

        [Fact]
        public async Task TransformCommandConvertsOpenApiAsync()
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
            await OpenApiService.TransformOpenApiDocumentAsync(options, _logger);

            var output = await File.ReadAllTextAsync("sample.json");
            Assert.NotEmpty(output);
        }


        [Fact]
        public async Task TransformCommandConvertsOpenApiWithDefaultOutputNameAsync()
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
            await OpenApiService.TransformOpenApiDocumentAsync(options, _logger);

            var output = await File.ReadAllTextAsync("output.yml");
            Assert.NotEmpty(output);
        }

        [Fact]
        public async Task TransformCommandConvertsOpenApiWithDefaultOutputNameAndSwitchFormatAsync()
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
            await OpenApiService.TransformOpenApiDocumentAsync(options, _logger);

            var output = await File.ReadAllTextAsync("output.yml");
            Assert.NotEmpty(output);
        }

        [Fact]
        public Task ThrowTransformCommandIfOpenApiAndCsdlAreEmptyAsync()
        {
            var options = new HidiOptions
            {
                CleanOutput = true,
                TerseOutput = false,
                InlineLocal = false,
                InlineExternal = false,
            };
            return Assert.ThrowsAsync<ArgumentException>(() =>
                OpenApiService.TransformOpenApiDocumentAsync(options, _logger));
        }

        [Fact]
        public async Task TransformToPowerShellCompliantOpenApiAsync()
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
            await OpenApiService.TransformOpenApiDocumentAsync(options, _logger);

            var output = await File.ReadAllTextAsync("output.yaml");
            Assert.NotEmpty(output);
        }

        [Fact]
        public async Task InvokeTransformCommandAsync()
        {
            var rootCommand = Program.CreateRootCommand();
            var openapi = Path.Combine(".", "UtilityFiles", "SampleOpenApi.yml");
            var args = new[] { "transform", "-d", openapi, "-o", "sample.json", "--co" };
            var parseResult = rootCommand.Parse(args);
            var handler = rootCommand.Subcommands.First(c => c.Name == "transform").Handler;
            var context = new InvocationContext(parseResult);

            await handler!.InvokeAsync(context);

            var output = await File.ReadAllTextAsync("sample.json");
            Assert.NotEmpty(output);
        }


        [Fact]
        public async Task InvokeShowCommandAsync()
        {
            var rootCommand = Program.CreateRootCommand();
            var openApi = Path.Combine(".", "UtilityFiles", "SampleOpenApi.yml");
            var args = new[] { "show", "-d", openApi, "-o", "sample.md" };
            var parseResult = rootCommand.Parse(args);
            var handler = rootCommand.Subcommands.Where(c => c.Name == "show").First().Handler;
            var context = new InvocationContext(parseResult);

            await handler!.InvokeAsync(context);

            var output = await File.ReadAllTextAsync("sample.md");
            Assert.Contains("graph LR", output, StringComparison.Ordinal);
        }

        [Fact]
        public async Task InvokePluginCommandAsync()
        {
            var rootCommand = Program.CreateRootCommand();
            var manifest = Path.Combine(".", "UtilityFiles", "exampleapimanifest.json");
            var args = new[] { "plugin", "-m", manifest, "--of", AppDomain.CurrentDomain.BaseDirectory };
            var parseResult = rootCommand.Parse(args);
            var handler = rootCommand.Subcommands.Where(c => c.Name == "plugin").First().Handler;
            var context = new InvocationContext(parseResult);

            await handler!.InvokeAsync(context);

            using var jsDoc = JsonDocument.Parse(await File.ReadAllTextAsync("ai-plugin.json"));
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
