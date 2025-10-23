using resultsComparer.Models;

namespace resultsComparer.Policies;

internal sealed class ZeroPointOnePercentDifferenceMemoryUsagePolicy : PercentageMemoryUsagePolicy
{
    public static ZeroPointOnePercentDifferenceMemoryUsagePolicy Instance { get; } = new ZeroPointOnePercentDifferenceMemoryUsagePolicy();
    protected override string TypeName => nameof(ZeroPointOnePercentDifferenceMemoryUsagePolicy);
    public ZeroPointOnePercentDifferenceMemoryUsagePolicy():base(0.1f) {}
}

internal sealed class ZeroPointTwoPercentDifferenceMemoryUsagePolicy : PercentageMemoryUsagePolicy
{
    public static ZeroPointTwoPercentDifferenceMemoryUsagePolicy Instance { get; } = new ZeroPointTwoPercentDifferenceMemoryUsagePolicy();
    protected override string TypeName => nameof(ZeroPointTwoPercentDifferenceMemoryUsagePolicy);
    public ZeroPointTwoPercentDifferenceMemoryUsagePolicy():base(0.2f) {}
}

internal sealed class OnePercentDifferenceMemoryUsagePolicy : PercentageMemoryUsagePolicy
{
    public static OnePercentDifferenceMemoryUsagePolicy Instance { get; } = new OnePercentDifferenceMemoryUsagePolicy();
    protected override string TypeName => nameof(OnePercentDifferenceMemoryUsagePolicy);
    public OnePercentDifferenceMemoryUsagePolicy():base(1) {}
}

internal sealed class TwoPercentDifferenceMemoryUsagePolicy : PercentageMemoryUsagePolicy
{
    public static TwoPercentDifferenceMemoryUsagePolicy Instance { get; } = new TwoPercentDifferenceMemoryUsagePolicy();
    protected override string TypeName => nameof(TwoPercentDifferenceMemoryUsagePolicy);
    public TwoPercentDifferenceMemoryUsagePolicy():base(2) {}
}

internal sealed class FivePercentDifferenceMemoryUsagePolicy : PercentageMemoryUsagePolicy
{
    public static FivePercentDifferenceMemoryUsagePolicy Instance { get; } = new FivePercentDifferenceMemoryUsagePolicy();
    protected override string TypeName => nameof(FivePercentDifferenceMemoryUsagePolicy);
    public FivePercentDifferenceMemoryUsagePolicy():base(1) {}
}

internal abstract class PercentageMemoryUsagePolicy(float tolerancePercentagePoints) : BaseBenchmarkComparisonPolicy
{
    private float TolerancePercentagePoints { get; } = Math.Abs(tolerancePercentagePoints);
    public override bool Equals(BenchmarkMemory? x, BenchmarkMemory? y)
    {
        if (x is null && y is null)
        {
            return true;
        }
        if (x is null || y is null)
        {
            return false;
        }
        var forwardRatio = GetPercentageDifference(x, y);
        var backwardRatio = GetPercentageDifference(y, x);
        return forwardRatio <= TolerancePercentagePoints && backwardRatio <= TolerancePercentagePoints;
    }
    private static double GetPercentageDifference(BenchmarkMemory x, BenchmarkMemory y)
    {
        return Math.Truncate(Math.Abs(GetRatio(x, y)) * 10000) / 100;
    }
    private static double GetRatio(BenchmarkMemory x, BenchmarkMemory y)
    {
        return (double)(x.AllocatedBytes - y.AllocatedBytes) / x.AllocatedBytes;
    }
    public override string GetErrorMessage(BenchmarkMemory? x, BenchmarkMemory? y)
    {
        if (x is null || y is null)
        {
            return "One of the benchmarks is null.";
        }
        return $"Allocated bytes differ: {x.AllocatedBytes} != {y.AllocatedBytes}, Ratio: {GetRatio(x, y)}, Allowed: {TolerancePercentagePoints}%";
    }
}
