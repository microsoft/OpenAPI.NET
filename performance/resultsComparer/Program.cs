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
        var parseResult = rootCommand.Parse(args);
        return await parseResult.InvokeAsync();
    }
    internal static RootCommand CreateRootCommand()
    {
        var rootCommand = new RootCommand { };

        var compareCommand = new Command("compare")
        {
            Description = "Compare the benchmark results."
        };
        var oldResultsPathArgument = new Argument<string>("existingReportPath")
        {
            DefaultValueFactory = (_) => ExistingReportPath,
            Description = "The path to the existing benchmark report.",
        };
        compareCommand.Arguments.Add(oldResultsPathArgument);
        var newResultsPathArgument = new Argument<string>("newReportPath")
        {
            DefaultValueFactory = (_) => ExistingReportPath,
            Description = "The path to the new benchmark report.",
        };
        compareCommand.Arguments.Add(newResultsPathArgument);
        var logLevelOption = new Option<LogLevel>("--log-level", "-l")
        {
            DefaultValueFactory = (_) => LogLevel.Warning,
            Description = "The log level to use.",
        };
        compareCommand.Options.Add(logLevelOption);
        var allPolicyNames = IBenchmarkComparisonPolicy.GetAllPolicies().Select(static p => p.Name).Order(StringComparer.OrdinalIgnoreCase).ToArray();
        var policiesOption = new Option<string[]>("--policies", "-p")
        {
            Arity = ArgumentArity.ZeroOrMore,
            DefaultValueFactory = (_) => ["all"],
            Description = $"The policies to use for comparison: {string.Join(',', allPolicyNames)}.",
        };
        compareCommand.Options.Add(policiesOption);
        var compareCommandHandler = new CompareCommandHandler
        {
            OldResultsPath = oldResultsPathArgument,
            NewResultsPath = newResultsPathArgument,
            LogLevel = logLevelOption,
            Policies = policiesOption,
        };
        compareCommand.SetAction(compareCommandHandler.InvokeAsync);
        rootCommand.Add(compareCommand);
        return rootCommand;
    }
    private const string ExistingReportPath = "../benchmark/BenchmarkDotNet.Artifacts/results/performance.EmptyModels-report.json";
}
