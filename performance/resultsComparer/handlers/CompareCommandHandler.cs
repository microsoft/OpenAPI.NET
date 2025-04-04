using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace resultsComparer.Handlers;

internal class CompareCommandHandler : AsyncCommandHandler
{
    public required Argument<string> OldResultsPath { get; set; }
    public required Argument<string> NewResultsPath { get; set; }
    public required Option<LogLevel> LogLevel { get; set; }

    public override Task<int> InvokeAsync(InvocationContext context)
    {
        var cancellationToken = context.BindingContext.GetRequiredService<CancellationToken>();
        var oldResultsPath = context.ParseResult.GetValueForArgument(OldResultsPath);
        var newResultsPath = context.ParseResult.GetValueForArgument(NewResultsPath);
        var logLevel = context.ParseResult.GetValueForOption(LogLevel);
        using var loggerFactory = Logger.ConfigureLogger(logLevel);
        var logger = loggerFactory.CreateLogger<CompareCommandHandler>();
        return CompareResultsAsync(oldResultsPath, newResultsPath, logger, cancellationToken);
    }
    private static async Task<int> CompareResultsAsync(string existingReportPath, string newReportPath, ILogger logger, CancellationToken cancellationToken = default) {

        var existingBenchmark = await GetBenchmarksAllocatedBytes(existingReportPath, cancellationToken);
        if (existingBenchmark is null)
        {
            logger.LogError("No existing benchmark data found.");
            return 1;
        }
        var newBenchmark = await GetBenchmarksAllocatedBytes(newReportPath, cancellationToken);
        if (newBenchmark is null)
        {
            logger.LogError("No new benchmark data found.");
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
                logger.LogError("No new benchmark result found for {existingBenchmarkResultKey}.", existingBenchmarkResult.Key);
                hasErrors = true;
            }
            foreach (var comparisonPolicy in comparisonPolicies)
            {
                if (!comparisonPolicy.Equals(existingBenchmarkResult.Value, newBenchmarkResult))
                {
                    logger.LogError("Benchmark result for {existingBenchmarkResultKey} does not match the existing benchmark result. {errorMessage}", existingBenchmarkResult.Key, comparisonPolicy.GetErrorMessage(existingBenchmarkResult.Value, newBenchmarkResult));
                    hasErrors = true;
                }
            }
        }

        if (newBenchmark.Keys.Where(x => !existingBenchmark.ContainsKey(x)).ToArray() is { Length: > 0 } missingKeys)
        {
            logger.LogError("New benchmark results found that do not exist in the existing benchmark results.");
            foreach (var missingKey in missingKeys)
            {
                logger.LogError("New benchmark result found: {missingKey}.", missingKey);
            }
            hasErrors = true;
        }
        logger.LogInformation("Benchmark comparison complete. {status}", hasErrors ? "Errors found" : "No errors found");
        return hasErrors ? 1 : 0;
    }

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
