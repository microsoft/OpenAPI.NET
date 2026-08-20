using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Reader;

namespace performance;

[MemoryDiagnoser]
[JsonExporter]
[ShortRunJob]
public class Descriptions
{
    private enum DescriptionSource
    {
        PetStoreYaml,
        PetStoreJson,
        GHESYaml,
        GHESJson,
        GHESNextYaml,
        GHESNextJson
    }

    [Benchmark]
    public Task<OpenApiDocument> PetStoreYaml() => ParseDocumentAsync(DescriptionSource.PetStoreYaml);

    [Benchmark]
    public Task<OpenApiDocument> PetStoreJson() => ParseDocumentAsync(DescriptionSource.PetStoreJson, OpenApiConstants.Json);

    [Benchmark]
    public Task<OpenApiDocument> GHESYaml() => ParseDocumentAsync(DescriptionSource.GHESYaml);

    [Benchmark]
    public Task<OpenApiDocument> GHESJson() => ParseDocumentAsync(DescriptionSource.GHESJson, OpenApiConstants.Json);

    [Benchmark]
    public Task<OpenApiDocument> GHESNextYaml() => ParseDocumentAsync(DescriptionSource.GHESNextYaml);

    [Benchmark]
    public Task<OpenApiDocument> GHESNextJson() => ParseDocumentAsync(DescriptionSource.GHESNextJson, OpenApiConstants.Json);

    private readonly Dictionary<DescriptionSource, MemoryStream> _streams = new(capacity: 6);

    [GlobalSetup]
    public async Task GetAllDescriptions()
    {
        _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
        _readerSettings = new OpenApiReaderSettings { LeaveStreamOpen = true };
        _readerSettings.AddYamlReader();

        var results = await Task.WhenAll(
            LoadFromAssemblyAsync(DescriptionSource.PetStoreYaml, PetStoreYamlResourceName),
            LoadFromAssemblyAsync(DescriptionSource.PetStoreJson, PetStoreJsonResourceName),
            LoadFromUrlAsync(DescriptionSource.GHESYaml, GHESYamlDescriptionUrl),
            LoadFromUrlAsync(DescriptionSource.GHESJson, GHESJsonDescriptionUrl),
            LoadFromUrlAsync(DescriptionSource.GHESNextYaml, GHESNextYamlDescriptionUrl),
            LoadFromUrlAsync(DescriptionSource.GHESNextJson, GHESNextJsonDescriptionUrl)
        ).ConfigureAwait(false);

        foreach (var (source, stream) in results)
        {
            _streams.Add(source, stream);
        }
    }

    private OpenApiReaderSettings _readerSettings;

    private const string PetStoreYamlResourceName = "petStore.yaml";
    private const string PetStoreJsonResourceName = "petStore.json";

    private const string GHESRepoCommitSha = "aef5e31a2d10fdaab311ec6d18a453021a81383d";
    private const string GHESReleaseVersion = "ghes-3.16";
    private const string GHESDescriptionFileName = "ghes-3.16.2022-11-28";

    // Building the four GHES URLs from shared constants means the pinned commit only needs to
    // change in one place when the benchmark data set is refreshed, instead of four near-identical
    // literals that can silently drift out of sync with each other.
    private static string BuildGHESDescriptionUrl(string descriptionsFolder, string extension) =>
        $"https://raw.githubusercontent.com/github/rest-api-description/{GHESRepoCommitSha}/{descriptionsFolder}/{GHESReleaseVersion}/{GHESDescriptionFileName}.{extension}";

    private static readonly string GHESYamlDescriptionUrl = BuildGHESDescriptionUrl("descriptions", "yaml");
    private static readonly string GHESJsonDescriptionUrl = BuildGHESDescriptionUrl("descriptions", "json");
    private static readonly string GHESNextYamlDescriptionUrl = BuildGHESDescriptionUrl("descriptions-next", "yaml");
    private static readonly string GHESNextJsonDescriptionUrl = BuildGHESDescriptionUrl("descriptions-next", "json");

    private async Task<OpenApiDocument> ParseDocumentAsync(DescriptionSource source, string format = null)
    {
        format ??= OpenApiConstants.Yaml;
        var stream = _streams[source];
        stream.Seek(0, SeekOrigin.Begin);

        var (document, _) = await OpenApiDocument.LoadAsync(stream, format, _readerSettings).ConfigureAwait(false);
        return document;
    }

    private HttpClient _httpClient;

    private async Task<(DescriptionSource Source, MemoryStream Stream)> LoadFromUrlAsync(DescriptionSource source, string url)
    {
        using var response = await _httpClient.GetAsync(url).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var stream = new MemoryStream();
        await response.Content.CopyToAsync(stream).ConfigureAwait(false);
        stream.Seek(0, SeekOrigin.Begin);
        return (source, stream);
    }

    private static readonly Assembly _assembly = typeof(Descriptions).Assembly;

    private async Task<(DescriptionSource Source, MemoryStream Stream)> LoadFromAssemblyAsync(DescriptionSource source, string resourceFileName)
    {
        using var resourceStream = _assembly.GetManifestResourceStream($"PerformanceTests.{resourceFileName}");
        var stream = new MemoryStream(); 
        await resourceStream.CopyToAsync(stream).ConfigureAwait(false);
        stream.Seek(0, SeekOrigin.Begin);
        return (source, stream);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        foreach (var stream in _streams.Values)
        {
            stream.Dispose();
        }
        _streams.Clear();
        _httpClient.Dispose();
    }
}
