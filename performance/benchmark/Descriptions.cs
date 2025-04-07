using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;

namespace performance;

[MemoryDiagnoser]
[JsonExporter]
[ShortRunJob]
public class Descriptions
{
    [Benchmark]
    public async Task<OpenApiDocument> PetStore()
    {
        return await ParseDocumentAsync(PetStorePath);
    }
    private readonly Dictionary<string, MemoryStream> _streams = new(StringComparer.OrdinalIgnoreCase);
    [GlobalSetup]
    public async Task GetAllDescriptions()
    {
        readerSettings = new OpenApiReaderSettings
        {
            LeaveStreamOpen = true,
        };
        readerSettings.AddYamlReader();
        await LoadDocumentIntoStreams(PetStorePath);
    }
    private OpenApiReaderSettings readerSettings;
    private const string PetStorePath = @"petStore.yaml";
    private async Task<OpenApiDocument> ParseDocumentAsync(string fileName)
    {
        var stream = _streams[fileName];
        stream.Seek(0, SeekOrigin.Begin);
        
        var (document, _) = await OpenApiDocument.LoadAsync(stream, OpenApiConstants.Yaml, readerSettings).ConfigureAwait(false);
        return document;
    }
    private static readonly Assembly assembly = typeof(Descriptions).GetTypeInfo().Assembly;
    private async Task LoadDocumentIntoStreams(string fileName)
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
    }
}
