// See https://aka.ms/new-console-template for more information
using System.Text.Json;

namespace resultsComparer;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var existingBenchmark = await GetBenchmarksAllocatedBytes(ExistingReportPath);
        if (existingBenchmark is null)
        {
            await Console.Error.WriteLineAsync("No existing benchmark data found.");
            return 1;
        }
        var newBenchmark = await GetBenchmarksAllocatedBytes(ExistingReportPath);
        if (newBenchmark is null)
        {
            await Console.Error.WriteLineAsync("No new benchmark data found.");
            return 1;
        }
        IBenchmarkComparisonPolicy[] comparisonPolicies = [
            MemoryBenchmarkResultComparer.Instance
        ];
        var hasErrors = false;
        foreach(var existingBenchmarkResult in existingBenchmark)
        {
            if (!newBenchmark.TryGetValue(existingBenchmarkResult.Key, out var newBenchmarkResult))
            {
                await Console.Error.WriteLineAsync($"No new benchmark result found for {existingBenchmarkResult.Key}.");
                hasErrors = true;
            }
            foreach (var comparisonPolicy in comparisonPolicies)
            {
                if (!comparisonPolicy.Equals(existingBenchmarkResult.Value, newBenchmarkResult))
                {
                    await Console.Error.WriteLineAsync($"Benchmark result for {existingBenchmarkResult.Key} does not match the existing benchmark result. {comparisonPolicy.GetErrorMessage(existingBenchmarkResult.Value, newBenchmarkResult)}");
                    hasErrors = true;
                }
            }
        }

        if (newBenchmark.Keys.Where(x => !existingBenchmark.ContainsKey(x)).ToArray() is { Length: > 0 } missingKeys)
        {
            await Console.Error.WriteLineAsync("New benchmark results found that do not exist in the existing benchmark results.");
            foreach (var missingKey in missingKeys)
            {
                await Console.Error.WriteLineAsync($"New benchmark result found: {missingKey}.");
            }
            hasErrors = true;
        }
        return hasErrors ? 1 : 0;
    }
    private const string ExistingReportPath = "../benchmark/BenchmarkDotNet.Artifacts/results/performance.Benchmarks-report.json";

    private static async Task<Dictionary<string, BenchmarkResult>?> GetBenchmarksAllocatedBytes(string targetPath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(targetPath))
        {
            return null;
        }
        using var stream = new FileStream(targetPath, FileMode.Open, FileAccess.Read);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        var rootElement = document.RootElement;
        if (rootElement.ValueKind is not JsonValueKind.Object ||
            !rootElement.TryGetProperty("Benchmarks", out var benchmarksNode) ||
            benchmarksNode.ValueKind is not JsonValueKind.Array)
        {
            return null;
        }
        return benchmarksNode.EnumerateArray().Select(benchmarkNode => {
            if (benchmarkNode.ValueKind is not JsonValueKind.Object)
            {
                return default;
            }
            if (!benchmarkNode.TryGetProperty("Memory", out var memoryNode) ||
                memoryNode.ValueKind is not JsonValueKind.Object ||
                !memoryNode.TryGetProperty("BytesAllocatedPerOperation", out var allocatedBytesNode) ||
                allocatedBytesNode.ValueKind is not JsonValueKind.Number ||
                !allocatedBytesNode.TryGetInt64(out var allocatedBytes))
            {
                return default;
            }
            if (!benchmarkNode.TryGetProperty("Method", out var nameNode) ||
                nameNode.ValueKind is not JsonValueKind.String ||
                nameNode.GetString() is not string name)
            {
                return default;
            }
            return (name, new BenchmarkResult(allocatedBytes));
        })
        .Where(x => x.name is not null && x.Item2 is not null)
        .ToDictionary(x => x.name!, x => x.Item2!, StringComparer.OrdinalIgnoreCase);
    }
    private sealed record BenchmarkResult(long AllocatedBytes);
    private interface IBenchmarkComparisonPolicy : IEqualityComparer<BenchmarkResult>
    {
        string GetErrorMessage(BenchmarkResult? x, BenchmarkResult? y);
    }
    private sealed class MemoryBenchmarkResultComparer : IBenchmarkComparisonPolicy
    {
        public static MemoryBenchmarkResultComparer Instance { get; } = new MemoryBenchmarkResultComparer();
        public bool Equals(BenchmarkResult? x, BenchmarkResult? y)
        {
            return x?.AllocatedBytes == y?.AllocatedBytes;
        }

        public string GetErrorMessage(BenchmarkResult? x, BenchmarkResult? y)
        {
            return $"Allocated bytes differ: {x?.AllocatedBytes} != {y?.AllocatedBytes}";
        }

        public int GetHashCode(BenchmarkResult obj)
        {
            return obj.AllocatedBytes.GetHashCode();
        }
    }
}
