using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using resultsComparer.Models;
using resultsComparer.Policies;

namespace resultsComparer.Handlers;

internal class CompareCommandHandler : AsynchronousCommandLineAction
{
    public required Argument<string> OldResultsPath { get; set; }
    public required Argument<string> NewResultsPath { get; set; }
    public required Option<LogLevel> LogLevel { get; set; }
    public required Option<string[]> Policies { get; set; }

    public override Task<int> InvokeAsync(ParseResult parseResult, CancellationToken cancellationToken = default)
    {
        var oldResultsPath = parseResult.GetValue(OldResultsPath);
        var newResultsPath = parseResult.GetValue(NewResultsPath);
        var policyNames = parseResult.GetValue(Policies) ?? [];
        var policies = IBenchmarkComparisonPolicy.GetSelectedPolicies(policyNames).ToArray();
        var logLevel = parseResult.GetValue(LogLevel);
        using var loggerFactory = Logger.ConfigureLogger(logLevel);
        var logger = loggerFactory.CreateLogger<CompareCommandHandler>();
        if (string.IsNullOrWhiteSpace(oldResultsPath))
        {
            logger.LogError("Old results path is required.");
            return Task.FromResult(1);
        }
        if (string.IsNullOrWhiteSpace(newResultsPath))
        {
            logger.LogError("New results path is required.");
            return Task.FromResult(1);
        }
        return CompareResultsAsync(oldResultsPath, newResultsPath, logger, policies, cancellationToken);
    }
    private static async Task<int> CompareResultsAsync(string existingReportPath, string newReportPath, ILogger logger, IBenchmarkComparisonPolicy[] comparisonPolicies, CancellationToken cancellationToken = default)
    {

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
        var hasErrors = false;
        foreach (var existingBenchmarkResult in existingBenchmark)
        {
            if (!newBenchmark.TryGetValue(existingBenchmarkResult.Key, out var newBenchmarkResult))
            {
                logger.LogError("No new benchmark result found for {ExistingBenchmarkResultKey}.", existingBenchmarkResult.Key);
                hasErrors = true;
            }
            foreach (var comparisonPolicy in comparisonPolicies)
            {
                if (!comparisonPolicy.Equals(existingBenchmarkResult.Value, newBenchmarkResult))
                {
                    logger.LogError("Benchmark result for {ExistingBenchmarkResultKey} does not match the existing benchmark result. {ErrorMessage}", existingBenchmarkResult.Key, comparisonPolicy.GetErrorMessage(existingBenchmarkResult.Value, newBenchmarkResult));
                    hasErrors = true;
                }
            }
        }

        if (newBenchmark.Keys.Where(x => !existingBenchmark.ContainsKey(x)).ToArray() is { Length: > 0 } missingKeys)
        {
            logger.LogError("New benchmark results found that do not exist in the existing benchmark results.");
            foreach (var missingKey in missingKeys)
            {
                logger.LogError("New benchmark result found: {MissingKey}.", missingKey);
            }
            hasErrors = true;
        }
        logger.LogInformation("Benchmark comparison complete. {Status}", hasErrors ? "Errors found" : "No errors found");
        return hasErrors ? 1 : 0;
    }

    private static async Task<Dictionary<string, BenchmarkMemory>?> GetBenchmarksAllocatedBytes(string targetPath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(targetPath))
        {
            return null;
        }
        using var stream = new FileStream(targetPath, FileMode.Open, FileAccess.Read);
        var report = (await JsonSerializer.DeserializeAsync(stream, BenchmarkSourceGenerationContext.Default.BenchmarkReport, cancellationToken: cancellationToken))
                        ?? throw new InvalidOperationException($"Failed to deserialize {targetPath}.");
        return report.Benchmarks
                .Where(x => x.Memory is not null && x.Method is not null)
            .ToDictionary(x => x.Method!, x => x.Memory!, StringComparer.OrdinalIgnoreCase);
    }
}

[JsonSerializable(typeof(BenchmarkReport))]
internal partial class BenchmarkSourceGenerationContext : JsonSerializerContext
{
}
