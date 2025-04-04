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
        if (names is ["all"])
        {
            foreach (var policy in GetAllPolicies())
            {
                yield return policy;
            }
        }
        var indexedNames = names.ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (indexedNames.Contains(nameof(IdenticalMemoryUsagePolicy)))
        {
            yield return IdenticalMemoryUsagePolicy.Instance;
        }
    }
    public static IBenchmarkComparisonPolicy[] GetAllPolicies()
    {
        return [IdenticalMemoryUsagePolicy.Instance];
    }
}
