using System.Text.Json.Serialization;

namespace resultsComparer.Models;
internal sealed record BenchmarkReport
{
    [JsonPropertyName("Benchmarks")]
    public Benchmark[] Benchmarks { get; init; } = [];
}
internal sealed record Benchmark
{
    [JsonPropertyName("Method")]
    public string Method { get; init; } = string.Empty;

    [JsonPropertyName("Memory")]
    public BenchmarkMemory Memory { get; init; } = new BenchmarkMemory();
}
internal sealed record BenchmarkMemory
{
    [JsonPropertyName("BytesAllocatedPerOperation")]
    public long AllocatedBytes { get; init; }
}
