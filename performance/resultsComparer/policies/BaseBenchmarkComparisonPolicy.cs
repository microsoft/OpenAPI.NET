using resultsComparer.Models;

namespace resultsComparer.Policies;

internal abstract class BaseBenchmarkComparisonPolicy : IBenchmarkComparisonPolicy
{
    protected abstract string TypeName { get; }
    public string Name => TypeName[..^6]; // Remove "Policy" suffix

    public abstract bool Equals(BenchmarkMemory? x, BenchmarkMemory? y);
    public abstract string GetErrorMessage(BenchmarkMemory? x, BenchmarkMemory? y);

    public int GetHashCode(BenchmarkMemory obj)
    {
        throw new InvalidOperationException("This method should not be called. Use Equals instead.");
    }
}
