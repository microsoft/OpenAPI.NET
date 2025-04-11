using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace performance;
public class Program
{
    public static void Main(string[] args)
    {
        var config = DefaultConfig.Instance;
        BenchmarkRunner.Run<Descriptions>(config, args);
        BenchmarkRunner.Run<EmptyModels>(config, args);
    }
}
