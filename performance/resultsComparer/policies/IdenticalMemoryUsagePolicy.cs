using resultsComparer.Models;

namespace resultsComparer.Policies;
internal sealed class IdenticalMemoryUsagePolicy : IBenchmarkComparisonPolicy
{
    public static IdenticalMemoryUsagePolicy Instance { get; } = new IdenticalMemoryUsagePolicy();
    public string Name => nameof(IdenticalMemoryUsagePolicy);
    public bool Equals(BenchmarkMemory? x, BenchmarkMemory? y)
    {
        return x?.AllocatedBytes == y?.AllocatedBytes;
    }

    public string GetErrorMessage(BenchmarkMemory? x, BenchmarkMemory? y)
    {
        return $"Allocated bytes differ: {x?.AllocatedBytes} != {y?.AllocatedBytes}";
    }

    public int GetHashCode(BenchmarkMemory obj)
    {
        return obj.AllocatedBytes.GetHashCode();
    }
}
