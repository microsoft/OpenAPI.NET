using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.OpenApiWorkspaceTests
{
    public class OpenApiWorkspaceStreamTests
    {
        // Use OpenApiWorkspace to load a document and a referenced document

        [Theory]
        [InlineData(1, "json")]
        [InlineData(2, "json")]
        [InlineData(3, "yaml")]
        public async Task ExternalReferenceOnlyCyclesFailWithoutReloadingDocuments(int length, string format)
        {
            var loader = new RecordingLoader();
            for (var i = 0; i < length; i++)
            {
                loader.Documents[$"http://example.test/{i}.yaml"] =
                    Document($$$"""{"A":{"$ref":"{{{(i + 1) % length}}}.yaml#/components/schemas/A"}}""");
            }
            var settings = Settings(loader);
            settings.RuleSet = new ValidationRuleSet();
            var result = await Load(Document("""{"Main":{"$ref":"0.yaml#/components/schemas/A"}}"""), settings, format);

            Assert.Null(result.Document);
            Assert.Contains(result.Diagnostic.Errors, error =>
                error.Message.Contains("Circular external reference-only chain", StringComparison.Ordinal));
            Assert.Equal(length, loader.Requests.Count);
            Assert.Equal(length, loader.Requests.Distinct().Count());
            Assert.All(loader.Streams, stream => Assert.False(stream.CanRead));
        }

        [Fact]
        public async Task StreamBaseUrlDoesNotPretendToIdentifyTheRootDocument()
        {
            var loader = new RecordingLoader();
            loader.Documents["http://example.test/a.yaml"] = """
                openapi: 3.0.0
                info:
                  title: External
                  version: 1.0.0
                paths: {}
                components:
                  schemas:
                    A:
                      $ref: 'http://example.test/a.yaml#/components/schemas/A'
                """;
            var settings = Settings(loader);
            settings.BaseUrl = new("http://example.test/a.yaml");
            var result = await Load(Document("""{"Main":{"$ref":"http://example.test/a.yaml#/components/schemas/A"}}"""), settings, "yaml");

            Assert.Null(result.Document);
            Assert.Single(loader.Requests);
            Assert.Contains(result.Diagnostic.Errors, error => error.Message.Contains("Circular external", StringComparison.Ordinal));
        }

        [Theory]
        [InlineData(2, 1, false)]
        [InlineData(2, 2, true)]
        [InlineData(2, 3, true)]
        [InlineData(64, 65, true)]
        [InlineData(65, 64, false)]
        [InlineData(70, 80, true)]
        public async Task ExternalDepthIsCheckedBeforeFetchingBeyondTheLimit(int length, int limit, bool succeeds)
        {
            var loader = Chain(length);
            var settings = Settings(loader);
            settings.MaxExternalReferenceDepth = limit;
            var result = await Load(Document("""{"Main":{"$ref":"0.json#/components/schemas/A"}}"""), settings);

            Assert.Equal(succeeds, result.Document is not null);
            Assert.Equal(Math.Min(length, limit), loader.Requests.Count);
            if (succeeds)
            {
                var document = result.Document;
                Assert.NotNull(document);
                Assert.Empty(result.Diagnostic.Errors);
                var reference = Assert.IsType<OpenApiSchemaReference>(document.Components.Schemas["Main"]);
                Assert.Equal(JsonSchemaType.Object, reference.RecursiveTarget.Type);
            }
            else
            {
                Assert.Contains(result.Diagnostic.Errors, error =>
                    error.Message.Contains($"depth limit {limit}", StringComparison.Ordinal) &&
                    error.Message.Contains($"{limit}.json", StringComparison.Ordinal));
            }
        }

        [Fact]
        public async Task DefaultDepthStopsAt64ExternalDocuments()
        {
            var loader = Chain(65);
            var result = await Load(Document("""{"Main":{"$ref":"0.json#/components/schemas/A"}}"""), Settings(loader));
            Assert.Null(result.Document);
            Assert.Equal(64, loader.Requests.Count);
            Assert.Contains(result.Diagnostic.Errors, error => error.Message.Contains("depth limit 64", StringComparison.Ordinal));
        }

        [Fact]
        public async Task SharedDependenciesAndEquivalentUrisAreFetchedOnce()
        {
            var loader = new RecordingLoader();
            loader.Documents["http://example.test/a.json"] = Document("""
                {"A":{"$ref":"shared.json#/components/schemas/A"}}
                """);
            loader.Documents["http://example.test/b.json"] = Document("""
                {"A":{"$ref":"./shared.json#/components/schemas/B"}}
                """);
            loader.Documents["http://example.test/shared.json"] = Document("""
                {"A":{"type":"string"},"B":{"type":"integer"}}
                """);
            var result = await Load(Document("""
                {
                  "A":{"$ref":"a.json#/components/schemas/A"},
                  "B":{"$ref":"b.json#/components/schemas/A"},
                  "Other":{"$ref":"http://example.test/sub/../shared.json#/components/schemas/A"}
                }
                """), Settings(loader));

            Assert.NotNull(result.Document);
            Assert.Empty(result.Diagnostic.Errors);
            Assert.Equal(3, loader.Requests.Count);
            Assert.Equal(JsonSchemaType.String, result.Document.Components.Schemas["A"].Type);
            Assert.Equal(JsonSchemaType.Integer, result.Document.Components.Schemas["B"].Type);
        }

        [Fact]
        public async Task SameRelativeFilenameInDifferentDirectoriesResolvesIndependently()
        {
            var loader = new RecordingLoader();
            foreach (var directory in new[] { "left", "right" })
            {
                loader.Documents[$"http://example.test/{directory}/a.json"] =
                    Document("""{"A":{"$ref":"shared.json#/components/schemas/A"}}""");
                loader.Documents[$"http://example.test/{directory}/shared.json"] =
                    Document($$$"""{"A":{"type":"{{{(directory == "left" ? "string" : "integer")}}}"}}""");
            }
            var result = await Load(Document("""
                {"Left":{"$ref":"left/a.json#/components/schemas/A"},"Right":{"$ref":"right/a.json#/components/schemas/A"}}
                """), Settings(loader));

            Assert.NotNull(result.Document);
            Assert.Empty(result.Diagnostic.Errors);
            Assert.Equal(4, loader.Requests.Count);
            Assert.Equal(JsonSchemaType.String, result.Document.Components.Schemas["Left"].Type);
            Assert.Equal(JsonSchemaType.Integer, result.Document.Components.Schemas["Right"].Type);
        }

        [Fact]
        public async Task MultipleFragmentsAreAllCheckedForAliasCycles()
        {
            var loader = new RecordingLoader();
            loader.Documents["http://example.test/a.json"] = Document("""
                {"Valid":{"type":"string"},"Cycle":{"$ref":"a.json#/components/schemas/Cycle"}}
                """);
            var result = await Load(Document("""
                {"First":{"$ref":"a.json#/components/schemas/Valid"},"Second":{"$ref":"a.json#/components/schemas/Cycle"}}
                """), Settings(loader));
            Assert.Null(result.Document);
            Assert.Single(loader.Requests);
            Assert.Contains(result.Diagnostic.Errors, error => error.Message.Contains("Cycle", StringComparison.Ordinal));
        }

        [Fact]
        public async Task ValidRecursivePropertiesAcrossDocumentsArePreserved()
        {
            var loader = new RecordingLoader();
            loader.Documents["http://example.test/a.json"] = Document("""
                {"A":{"type":"object","properties":{"b":{"$ref":"b.json#/components/schemas/B"}}}}
                """);
            loader.Documents["http://example.test/b.json"] = Document("""
                {"B":{"type":"object","properties":{"a":{"$ref":"a.json#/components/schemas/A"}}}}
                """);
            var result = await Load(Document("""{"Main":{"$ref":"a.json#/components/schemas/A"}}"""), Settings(loader));

            Assert.NotNull(result.Document);
            Assert.Empty(result.Diagnostic.Errors);
            Assert.Equal(2, loader.Requests.Count);
            var schemaA = Assert.IsType<OpenApiSchemaReference>(result.Document.Components.Schemas["Main"]).RecursiveTarget;
            var schemaB = Assert.IsType<OpenApiSchemaReference>(schemaA.Properties["b"]).RecursiveTarget;
            Assert.Same(schemaA, Assert.IsType<OpenApiSchemaReference>(schemaB.Properties["a"]).RecursiveTarget);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task HttpLoadingReusesTheKnownRootForValidRecursiveReferences(bool overrideBaseUrl)
        {
            var loader = new RecordingLoader();
            loader.Documents["http://example.test/main.json"] = Document("""
                {"Root":{"type":"object","properties":{"a":{"$ref":"http://example.test/a.json#/components/schemas/A"}}}}
                """);
            loader.Documents["http://example.test/a.json"] = Document("""
                {"A":{"type":"object","properties":{"root":{"$ref":"main.json#/components/schemas/Root"}}}}
                """);
            using var client = new HttpClient(new RecordingHandler(loader));
            var settings = new OpenApiReaderSettings { HttpClient = client, LoadExternalRefs = true };
            if (overrideBaseUrl) settings.BaseUrl = new("http://example.test/context/main.json");
            var result = await OpenApiDocument.LoadAsync("http://example.test/main.json", settings, TestContext.Current.CancellationToken);

            Assert.NotNull(result.Document);
            Assert.Empty(result.Diagnostic.Errors);
            Assert.Equal(2, loader.Requests.Count);
            var root = result.Document.Components.Schemas["Root"];
            var schemaA = Assert.IsType<OpenApiSchemaReference>(root.Properties["a"]).RecursiveTarget;
            Assert.Same(root, Assert.IsType<OpenApiSchemaReference>(schemaA.Properties["root"]).RecursiveTarget);
        }

        [Fact]
        public async Task ResponseReferenceOnlyCyclesAlsoFail()
        {
            var loader = new RecordingLoader();
            var document = """
                {"openapi":"3.0.0","info":{"title":"Test","version":"1.0"},"paths":{},
                 "components":{"responses":{"A":{"$ref":"a.json#/components/responses/A"}}}}
                """;
            loader.Documents["http://example.test/a.json"] = document;
            var result = await Load(document, Settings(loader));
            Assert.Null(result.Document);
            Assert.Single(loader.Requests);
            Assert.Contains(result.Diagnostic.Errors, error => error.Message.Contains("Circular external", StringComparison.Ordinal));
        }

        [Fact]
        public async Task LocalOnlyAliasCyclesKeepTheirExistingWarnings()
        {
            var result = await Load(Document("""
                {"A":{"$ref":"#/components/schemas/B"},"B":{"$ref":"#/components/schemas/A"}}
                """), Settings(new RecordingLoader()));
            Assert.NotNull(result.Document);
            Assert.Empty(result.Diagnostic.Errors);
            Assert.Contains(result.Diagnostic.Warnings, warning => warning.Message.Contains("Circular reference", StringComparison.Ordinal));
        }

        [Fact]
        public async Task UriPathsAndQueriesRemainDistinctDocumentIdentities()
        {
            var loader = new RecordingLoader();
            loader.Documents["http://example.test/A.json"] = Document("""{"A":{"type":"string"}}""");
            loader.Documents["http://example.test/a.json"] = Document("""{"A":{"type":"integer"}}""");
            loader.Documents["http://example.test/a.json?version=1"] = Document("""{"A":{"type":"boolean"}}""");
            var result = await Load(Document("""
                {"Upper":{"$ref":"A.json#/components/schemas/A"},
                 "Lower":{"$ref":"a.json#/components/schemas/A"},
                 "Query":{"$ref":"a.json?version=1#/components/schemas/A"}}
                """), Settings(loader));
            Assert.NotNull(result.Document);
            Assert.Empty(result.Diagnostic.Errors);
            Assert.Equal(3, loader.Requests.Count);
            Assert.Equal(JsonSchemaType.String, result.Document.Components.Schemas["Upper"].Type);
            Assert.Equal(JsonSchemaType.Integer, result.Document.Components.Schemas["Lower"].Type);
            Assert.Equal(JsonSchemaType.Boolean, result.Document.Components.Schemas["Query"].Type);
        }

        [Theory]
        [InlineData("3.1.0")]
        [InlineData("3.2.0")]
        public async Task AuthoredSchemaSiblingsArePreservedAndTheirReferencesAreLoaded(string version)
        {
            var loader = new RecordingLoader();
            loader.Documents["http://example.test/a.json"] = Document("""
                {"A":{"$ref":"a.json#/components/schemas/A","type":"object","properties":{"child":{"$ref":"child.json#/components/schemas/A"}}}}
                """, version);
            loader.Documents["http://example.test/child.json"] = Document("""{"A":{"type":"string"}}""", version);
            var result = await Load(Document("""{"Main":{"$ref":"a.json#/components/schemas/A"}}""", version), Settings(loader));

            Assert.NotNull(result.Document);
            Assert.Empty(result.Diagnostic.Errors);
            Assert.Equal(2, loader.Requests.Count);
            Assert.Equal(JsonSchemaType.Object, result.Document.Components.Schemas["Main"].Type);
            Assert.Equal(JsonSchemaType.String, result.Document.Components.Schemas["Main"].Properties["child"].Type);
        }

        [Theory]
        [InlineData("$defs", false)]
        [InlineData("$defs", true)]
        [InlineData("patternProperties", true)]
        [InlineData("contains", true)]
        public async Task ExternalReferencesBeneathConcreteSchemaChildrenAreLoaded(string keyword, bool hasReferenceSibling)
        {
            var loader = new RecordingLoader();
            var childReference = new Dictionary<string, string> { ["$ref"] = "child.json#/components/schemas/A" };
            var wrapper = new Dictionary<string, object>
            {
                [keyword] = keyword == "contains" ? childReference :
                    new Dictionary<string, object> { ["Child"] = childReference }
            };
            var schema = new Dictionary<string, object>
            {
                ["type"] = "object",
                ["properties"] = new Dictionary<string, object> { ["wrapper"] = wrapper }
            };
            if (hasReferenceSibling)
            {
                schema["$ref"] = "a.json#/components/schemas/A";
            }
            loader.Documents["http://example.test/a.json"] =
                Document(JsonSerializer.Serialize(new Dictionary<string, object> { ["A"] = schema }), "3.1.0");
            loader.Documents["http://example.test/child.json"] = Document("""{"A":{"type":"string"}}""", "3.1.0");
            var result = await Load(Document("""{"Main":{"$ref":"a.json#/components/schemas/A"}}""", "3.1.0"), Settings(loader));

            var document = result.Document;
            Assert.NotNull(document);
            Assert.Empty(result.Diagnostic.Errors);
            Assert.Equal(2, loader.Requests.Count);
            var wrapperSchema = document.Components.Schemas["Main"].Properties["wrapper"];
            var childSchema = keyword switch
            {
                "$defs" => wrapperSchema.Definitions["Child"],
                "patternProperties" => wrapperSchema.PatternProperties["Child"],
                _ => ((IOpenApiSchemaMissingProperties)wrapperSchema).Contains
            };
            Assert.Equal(JsonSchemaType.String, Assert.IsType<OpenApiSchemaReference>(childSchema).RecursiveTarget.Type);
        }

        [Theory]
        [InlineData("properties")]
        [InlineData("allOf")]
        [InlineData("allOf/-1")]
        [InlineData("anyOf/-1")]
        [InlineData("oneOf/-1")]
        public async Task MalformedExternalSubschemaPointersReturnDiagnostics(string pointer)
        {
            var loader = new RecordingLoader();
            loader.Documents["http://example.test/a.json"] = Document("""
                {"A":{"type":"object","properties":{"name":{"type":"string"}},
                      "allOf":[{"type":"object"}],"anyOf":[{"type":"object"}],"oneOf":[{"type":"object"}]}}
                """, "3.1.0");
            var settings = Settings(loader);
            settings.RuleSet = new ValidationRuleSet();
            var result = await Load(Document($$$"""{"Main":{"$ref":"a.json#/components/schemas/A/{{{pointer}}}"}}""", "3.1.0"), settings);

            Assert.Null(result.Document);
            Assert.Single(loader.Requests);
            Assert.Contains(result.Diagnostic.Errors, error =>
                error.Message.Contains("Could not resolve external reference chain", StringComparison.Ordinal) &&
                error.Message.Contains(pointer, StringComparison.Ordinal));
        }

        [Fact]
        public async Task EmbeddedSchemaResourcesAreNotFetchedAsExternalDocuments()
        {
            var loader = new RecordingLoader();
            var result = await Load(Document("""
                {"Embedded":{"$id":"http://example.test/embedded","$anchor":"value","type":"string"},
                 "Main":{"$ref":"http://example.test/embedded#value"}}
                """, "3.1.0"), Settings(loader));

            var document = Assert.IsType<OpenApiDocument>(result.Document);
            Assert.Empty(result.Diagnostic.Errors);
            Assert.Empty(loader.Requests);
            Assert.Equal(JsonSchemaType.String, document.Components.Schemas["Main"].Type);
        }

        [Fact]
        public async Task SlashContainingOperationTagsDoNotUseComponentReferenceResolution()
        {
            var loader = new RecordingLoader();
            var result = await Load("""
                {"openapi":"3.0.0","info":{"title":"Test","version":"1.0"},
                 "tags":[{"name":"users/admin","description":"Admin users"}],
                 "paths":{"/users":{"get":{"tags":["users/admin"],"responses":{"200":{"description":"OK"}}}}}}
                """, Settings(loader));

            var document = Assert.IsType<OpenApiDocument>(result.Document);
            Assert.Empty(result.Diagnostic.Errors);
            Assert.Empty(loader.Requests);
            var tag = Assert.Single(document.Paths["/users"].Operations[HttpMethod.Get].Tags);
            Assert.Equal("Admin users", tag.Description);
        }

        [Theory]
        [InlineData("properties")]
        [InlineData("allOf/-1")]
        public async Task MalformedLocalSubschemaPointersPreserveLoadingBehavior(string pointer)
        {
            var loader = new RecordingLoader();
            var settings = Settings(loader);
            settings.RuleSet = new ValidationRuleSet();
            var result = await Load(Document($$$"""
                {"Main":{"$ref":"#/components/schemas/A/{{{pointer}}}"},
                 "A":{"type":"object","properties":{"name":{"type":"string"}},"allOf":[{"type":"object"}]}}
                """, "3.1.0"), settings);

            Assert.NotNull(result.Document);
            Assert.Empty(result.Diagnostic.Errors);
            Assert.Empty(loader.Requests);
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public async Task AuthoredDiscriminatorReferencesAreLoaded(bool hasReferenceSibling, bool useDefaultMapping)
        {
            var loader = new RecordingLoader();
            loader.Documents["http://example.test/child.json"] = Document("""{"A":{"type":"string"}}""", "3.2.0");
            var discriminator = useDefaultMapping
                ? """{"propertyName":"kind","defaultMapping":"child.json#/components/schemas/A"}"""
                : """{"propertyName":"kind","mapping":{"child":"child.json#/components/schemas/A"}}""";
            var schema = hasReferenceSibling
                ? $$$"""{"$ref":"#/components/schemas/Base","discriminator":{{{discriminator}}}}"""
                : $$$$"""{"$defs":{"Child":{"type":"object","discriminator":{{{{discriminator}}}}}}}""";
            var result = await Load(Document($$$"""{"Base":{"type":"object"},"Main":{{{schema}}}}""", "3.2.0"), Settings(loader));

            var document = Assert.IsType<OpenApiDocument>(result.Document);
            Assert.Empty(result.Diagnostic.Errors);
            Assert.Single(loader.Requests);
            var main = document.Components.Schemas["Main"];
            var authoredDiscriminator = hasReferenceSibling
                ? Assert.IsType<OpenApiSchemaReference>(main).Reference.Discriminator
                : main.Definitions["Child"].Discriminator;
            Assert.NotNull(authoredDiscriminator);
            var mapping = useDefaultMapping ? authoredDiscriminator.DefaultMapping : authoredDiscriminator.Mapping["child"];
            Assert.Equal(JsonSchemaType.String, mapping.RecursiveTarget.Type);
        }

        [Fact]
        public async Task WideGraphsDoNotConsumeTheDepthBudgetAcrossSiblings()
        {
            var loader = new RecordingLoader();
            var schemas = new Dictionary<string, object>();
            for (var i = 0; i < 70; i++)
            {
                loader.Documents[$"http://example.test/{i}.json"] = Document("""{"A":{"type":"object"}}""");
                schemas[$"Schema{i}"] = new Dictionary<string, string> { ["$ref"] = $"{i}.json#/components/schemas/A" };
            }
            var settings = Settings(loader);
            settings.MaxExternalReferenceDepth = 1;
            var result = await Load(Document(JsonSerializer.Serialize(schemas)), settings);

            Assert.NotNull(result.Document);
            Assert.Empty(result.Diagnostic.Errors);
            Assert.Equal(70, loader.Requests.Count);
        }

        [Fact]
        public async Task DisabledExternalLoadingMakesNoRequests()
        {
            var loader = new RecordingLoader();
            var settings = Settings(loader);
            settings.LoadExternalRefs = false;
            var result = await Load(Document("""{"Main":{"$ref":"a.json#/components/schemas/A"}}"""), settings);
            Assert.NotNull(result.Document);
            Assert.Empty(loader.Requests);
        }

        [Fact]
        public async Task ReusingSettingsDoesNotShareLoadedDocuments()
        {
            var loader = Chain(1);
            var settings = Settings(loader);
            var root = Document("""{"Main":{"$ref":"0.json#/components/schemas/A"}}""");
            var results = await Task.WhenAll(Load(root, settings), Load(root, settings));
            Assert.All(results, result => Assert.NotNull(result.Document));
            Assert.NotSame(results[0].Document.Workspace, results[1].Document.Workspace);
            Assert.Equal(2, loader.Requests.Count);
        }

        [Theory]
        [InlineData(false, 1)]
        [InlineData(true, 1)]
        [InlineData(false, 2)]
        [InlineData(true, 2)]
        public async Task StreamsAreDisposedAccordingToOwnership(bool leaveOpen, int length)
        {
            var loader = Chain(length);
            var settings = Settings(loader);
            settings.MaxExternalReferenceDepth = 1;
            settings.LeaveStreamOpen = leaveOpen;
            using var root = new MemoryStream(Encoding.UTF8.GetBytes(Document("""{"Main":{"$ref":"0.json#/components/schemas/A"}}""")));
            var result = await OpenApiDocument.LoadAsync(root, "json", settings, TestContext.Current.CancellationToken);

            Assert.Equal(length == 1, result.Document is not null);
            Assert.Equal(leaveOpen, root.CanRead);
            Assert.All(loader.Streams, stream => Assert.False(stream.CanRead));
        }

        [Fact]
        public async Task CancellationDisposesStreamsAndDoesNotBecomeADiagnostic()
        {
            var loader = Chain(2);
            using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
            loader.OnLoad = () =>
            {
                if (loader.Requests.Count == 2) cancellation.Cancel();
            };
            using var root = new MemoryStream(Encoding.UTF8.GetBytes(Document("""{"Main":{"$ref":"0.json#/components/schemas/A"}}""")));
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
                OpenApiDocument.LoadAsync(root, "json", Settings(loader), cancellation.Token));
            Assert.False(root.CanRead);
            Assert.All(loader.Streams, stream => Assert.False(stream.CanRead));
        }

        private static string Document(string schemas, string version = "3.0.0") => $$$"""
            {"openapi":"{{{version}}}","info":{"title":"Test","version":"1.0"},"paths":{},"components":{"schemas":{{{schemas}}}}}
            """;

        private static OpenApiReaderSettings Settings(RecordingLoader loader)
        {
            var settings = new OpenApiReaderSettings
            {
                LoadExternalRefs = true,
                CustomExternalLoader = loader,
                BaseUrl = new("http://example.test/main.json")
            };
            settings.AddYamlReader();
            return settings;
        }

        private static async Task<ReadResult> Load(string document, OpenApiReaderSettings settings, string format = "json")
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(document));
            return await OpenApiDocument.LoadAsync(stream, format, settings, TestContext.Current.CancellationToken);
        }

        private static RecordingLoader Chain(int length)
        {
            var loader = new RecordingLoader();
            for (var i = 0; i < length; i++)
            {
                loader.Documents[$"http://example.test/{i}.json"] = Document(i + 1 < length ?
                    $$$"""{"A":{"$ref":"{{{i + 1}}}.json#/components/schemas/A"}}""" : """{"A":{"type":"object"}}""");
            }
            return loader;
        }

        private sealed class RecordingLoader : IStreamLoader
        {
            public Dictionary<string, string> Documents { get; } = new();
            public List<Uri> Requests { get; } = new();
            public List<MemoryStream> Streams { get; } = new();
            public Action OnLoad { get; set; }

            public Task<Stream> LoadAsync(Uri baseUrl, Uri uri, CancellationToken cancellationToken = default)
            {
                lock (Requests)
                {
                    if (Requests.Count >= 1000) throw new InvalidOperationException("Test request budget exceeded.");
                    var absolute = new Uri(baseUrl, uri);
                    Requests.Add(absolute);
                    OnLoad?.Invoke();
                    cancellationToken.ThrowIfCancellationRequested();
                    var stream = new MemoryStream(Encoding.UTF8.GetBytes(Documents[absolute.AbsoluteUri]));
                    Streams.Add(stream);
                    return Task.FromResult<Stream>(stream);
                }
            }
        }

        private sealed class RecordingHandler(RecordingLoader loader) : HttpMessageHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var stream = await loader.LoadAsync(new Uri("http://example.test/"), request.RequestUri, cancellationToken);
                var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = new StreamContent(stream) };
                response.Content.Headers.ContentType = new("application/json");
                return response;
            }
        }

        [Fact]
        public async Task LoadingDocumentWithResolveAllReferencesShouldLoadDocumentIntoWorkspaceAsync()
        {
            // Create a reader that will resolve all references
            var settings = new OpenApiReaderSettings
            {
                LoadExternalRefs = true,
                CustomExternalLoader = new MockLoader(),
                BaseUrl = new("file://c:\\")
            };
            settings.AddYamlReader();

            var stream = new MemoryStream();
            var doc = """
                      openapi: 3.0.0
                      info:
                        title: foo
                        version: 1.0.0
                      paths: {}
                      """;
            var wr = new StreamWriter(stream);
            await wr.WriteAsync(doc);
            await wr.FlushAsync(TestContext.Current.CancellationToken);
            stream.Position = 0;

            var result = await OpenApiDocument.LoadAsync(stream, OpenApiConstants.Yaml, settings: settings, cancellationToken: TestContext.Current.CancellationToken);

            Assert.NotNull(result.Document.Workspace);
        }

        [Fact]
        public async Task LoadDocumentWithExternalReferenceShouldLoadBothDocumentsIntoWorkspaceAsync()
        {
            // Create a reader that will resolve all references
            var settings = new OpenApiReaderSettings
            {
                LoadExternalRefs = true,
                CustomExternalLoader = new ResourceLoader(),
                BaseUrl = new("file://c:\\"),
            };
            settings.AddYamlReader();

            ReadResult result;
            result = await OpenApiDocument.LoadAsync("V3Tests/Samples/OpenApiWorkspace/TodoMain.yaml", settings, token: TestContext.Current.CancellationToken);

            var externalDocBaseUri = result.Document.Workspace.GetDocumentId("./TodoComponents.yaml");
            var schemasPath = "#/components/schemas/";
            var parametersPath = "#/components/parameters/";

            Assert.NotNull(externalDocBaseUri);
            Assert.True(result.Document.Workspace.Contains(externalDocBaseUri + schemasPath + "todo"));
            Assert.True(result.Document.Workspace.Contains(externalDocBaseUri + schemasPath + "entity"));
            Assert.True(result.Document.Workspace.Contains(externalDocBaseUri + parametersPath + "filter"));
        }

        [Fact]
        public async Task LoadDocumentWithExternalReferencesInSubDirectories()
        {
            var sampleFolderPath = $"V3Tests/Samples/OpenApiWorkspace/ExternalReferencesInSubDirectories";
            var referenceBaseUri = "file://" + Path.GetFullPath(sampleFolderPath);

            // Create a reader that will resolve all references also of documents located in the non-root directory
            var settings = new OpenApiReaderSettings()
            {
                LoadExternalRefs = true,
            };
            settings.AddYamlReader();

            // Act
            var result = await OpenApiDocument.LoadAsync($"{sampleFolderPath}/Root.yaml", settings, token: TestContext.Current.CancellationToken);
            var document = result.Document;
            var workspace = result.Document.Workspace;

            // Assert
            Assert.True(workspace.Contains($"{Path.Join(referenceBaseUri, "Directory", "PetsPage.yaml")}#/components/schemas/PetsPage"));
            Assert.True(workspace.Contains($"{Path.Join(referenceBaseUri, "Directory", "Pets.yaml")}#/components/schemas/Pets"));
            Assert.True(workspace.Contains($"{Path.Join(referenceBaseUri, "Directory", "Pets.yaml")}#/components/schemas/Pet"));

            var operationResponseSchema = document.Paths["/pets"].Operations[HttpMethod.Get].Responses["200"].Content["application/json"].Schema;
            Assert.IsType<OpenApiSchemaReference>(operationResponseSchema);
            
            var petsSchema = operationResponseSchema.Properties["pets"];
            Assert.IsType<OpenApiSchemaReference>(petsSchema);
            Assert.Equal(JsonSchemaType.Array, petsSchema.Type);
                        
            var petSchema = petsSchema.Items;
            var petSchemaReference = Assert.IsType<OpenApiSchemaReference>(petSchema);
            var petSchemaTarget = petSchemaReference.RecursiveTarget;
            Assert.NotNull(petSchemaTarget);

            Assert.Equivalent(new OpenApiSchema
            {
                Required = new HashSet<string> { "id", "name" },
                Properties = new Dictionary<string, IOpenApiSchema>
                {
                    ["id"] = new OpenApiSchema
                    {
                        Type =  JsonSchemaType.Integer,
                        Format = "int64"
                    },
                    ["name"] = new OpenApiSchema
                    {
                        Type = JsonSchemaType.String
                    },
                    ["tag"] = new OpenApiSchema
                    {
                        Type = JsonSchemaType.String
                    }
                }
            }, petSchemaTarget);
        }
    }

    public class MockLoader : IStreamLoader
    {
        public Stream Load(Uri uri)
        {
            return null;
        }

        public Task<Stream> LoadAsync(Uri baseUrl, Uri uri, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<Stream>(null);
        }
    }

    public class ResourceLoader : IStreamLoader
    {
        public Stream Load(Uri uri)
        {
            return null;
        }

        public Task<Stream> LoadAsync(Uri baseUrl, Uri uri, CancellationToken cancellationToken = default)
        {
            var path = new Uri(new("http://example.org/V3Tests/Samples/OpenApiWorkspace/"), uri).AbsolutePath;
            path = path[1..]; // remove leading slash
            return Task.FromResult(Resources.GetStream(path));
        }
    }
}
