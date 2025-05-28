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
    [Benchmark]
    public async Task<OpenApiDocument> PetStoreYaml()
    {
        return await ParseDocumentAsync(PetStoreYamlPath);
    }
    [Benchmark]
    public async Task<OpenApiDocument> PetStoreJson()
    {
        return await ParseDocumentAsync(PetStoreJsonPath, OpenApiConstants.Json);
    }
    [Benchmark]
    public async Task<OpenApiDocument> GHESYaml()
    {
        return await ParseDocumentAsync(GHESYamlDescriptionUrl);
    }
    [Benchmark]
    public async Task<OpenApiDocument> GHESJson()
    {
        return await ParseDocumentAsync(GHESJsonDescriptionUrl, OpenApiConstants.Json);
    }
    private readonly Dictionary<string, MemoryStream> _streams = new(StringComparer.OrdinalIgnoreCase);
    [GlobalSetup]
    public async Task GetAllDescriptions()
    {
        _httpClient = new HttpClient();
        readerSettings = new OpenApiReaderSettings
        {
            LeaveStreamOpen = true,
        };
        readerSettings.AddYamlReader();
        await LoadDocumentFromAssemblyIntoStreams(PetStoreYamlPath);
        await LoadDocumentFromAssemblyIntoStreams(PetStoreJsonPath);
        await LoadDocumentFromUrlIntoStreams(GHESYamlDescriptionUrl);
        await LoadDocumentFromUrlIntoStreams(GHESJsonDescriptionUrl);
    }
    private OpenApiReaderSettings readerSettings;
    private const string PetStoreYamlPath = @"petStore.yaml";
    private const string PetStoreJsonPath = @"petStore.json";
    private const string GHESYamlDescriptionUrl = @"https://raw.githubusercontent.com/github/rest-api-description/aef5e31a2d10fdaab311ec6d18a453021a81383d/descriptions/ghes-3.16/ghes-3.16.2022-11-28.yaml";
    private const string GHESJsonDescriptionUrl = @"https://raw.githubusercontent.com/github/rest-api-description/aef5e31a2d10fdaab311ec6d18a453021a81383d/descriptions/ghes-3.16/ghes-3.16.2022-11-28.json";
    private async Task<OpenApiDocument> ParseDocumentAsync(string fileName, string format = null)
    {
        format ??= OpenApiConstants.Yaml;
        var stream = _streams[fileName];
        stream.Seek(0, SeekOrigin.Begin);
        
        var (document, _) = await OpenApiDocument.LoadAsync(stream, format, readerSettings).ConfigureAwait(false);
        return document;
    }
    private HttpClient _httpClient;
    private async Task LoadDocumentFromUrlIntoStreams(string url)
    {
        var response = await _httpClient.GetAsync(url).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var stream = new MemoryStream(); // NOT disposed on purpose
        await response.Content.CopyToAsync(stream).ConfigureAwait(false);
        stream.Seek(0, SeekOrigin.Begin);
        _streams.Add(url, stream);
    }
    private static readonly Assembly assembly = typeof(Descriptions).GetTypeInfo().Assembly;
    private async Task LoadDocumentFromAssemblyIntoStreams(string fileName)
    {
        using var resource = assembly.GetManifestResourceStream($"PerformanceTests.{fileName}");
        var stream = new MemoryStream(); // NOT disposed on purpose
        await resource.CopyToAsync(stream).ConfigureAwait(false);
        stream.Seek(0, SeekOrigin.Begin);
        _streams.Add(fileName, stream);
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
