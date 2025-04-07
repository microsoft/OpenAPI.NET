using resultsComparer.Models;

namespace resultsComparer.Policies;

internal interface IBenchmarkComparisonPolicy : IEqualityComparer<BenchmarkMemory>
{
    string GetErrorMessage(BenchmarkMemory? x, BenchmarkMemory? y);
    string Name { get;}
    public static IEnumerable<IBenchmarkComparisonPolicy> GetSelectedPolicies(string[] names)
    {
        if (names is [])
        {
            yield break;
        }
        var allPolicies = GetAllPolicies();
        if (names is ["all"])
        {
            foreach (var policy in allPolicies)
            {
                yield return policy;
            }
        }
        var indexedNames = names.ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var policy in allPolicies.Where(x => indexedNames.Contains(x.Name)))
        {
            yield return policy;
        }
    }
    public static IBenchmarkComparisonPolicy[] GetAllPolicies()
    {
        return [
            IdenticalMemoryUsagePolicy.Instance,
            ZeroPointOnePercentDifferenceMemoryUsagePolicy.Instance,
            OnePercentDifferenceMemoryUsagePolicy.Instance,
            TwoPercentDifferenceMemoryUsagePolicy.Instance,
            FivePercentDifferenceMemoryUsagePolicy.Instance,
        ];
    }
}
