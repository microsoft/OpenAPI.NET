// See https://aka.ms/new-console-template for more information
using System.CommandLine;
using Microsoft.Extensions.Logging;
using resultsComparer.Handlers;
using resultsComparer.Policies;

namespace resultsComparer;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var rootCommand = CreateRootCommand();
        return await rootCommand.InvokeAsync(args);
    }
    internal static RootCommand CreateRootCommand()
    {
        var rootCommand = new RootCommand { };

        var compareCommand = new Command("compare")
        {
            Description = "Compare the benchmark results."
        };
        var oldResultsPathArgument = new Argument<string>("existingReportPath", () => ExistingReportPath, "The path to the existing benchmark report.");
        compareCommand.AddArgument(oldResultsPathArgument);
        var newResultsPathArgument = new Argument<string>("newReportPath", () => ExistingReportPath, "The path to the new benchmark report.");
        compareCommand.AddArgument(newResultsPathArgument);
        var logLevelOption = new Option<LogLevel>(["--log-level", "-l"], () => LogLevel.Warning, "The log level to use.");
        compareCommand.AddOption(logLevelOption);
        var allPolicyNames = IBenchmarkComparisonPolicy.GetAllPolicies().Select(static p => p.Name).Order(StringComparer.OrdinalIgnoreCase).ToArray();
        var policiesOption = new Option<string[]>(["--policies", "-p"], () => ["all"], $"The policies to use for comparison: {string.Join(',', allPolicyNames)}.")
        {
            Arity = ArgumentArity.ZeroOrMore
        };
        compareCommand.AddOption(policiesOption);
        compareCommand.Handler = new CompareCommandHandler
        {
            OldResultsPath = oldResultsPathArgument,
            NewResultsPath = newResultsPathArgument,
            LogLevel = logLevelOption,
            Policies = policiesOption,
        };
        rootCommand.Add(compareCommand);
        return rootCommand;
    }
    private const string ExistingReportPath = "../benchmark/BenchmarkDotNet.Artifacts/results/performance.EmptyModels-report.json";
}
