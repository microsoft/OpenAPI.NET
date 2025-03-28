using System;
using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using Microsoft.OpenApi.Models;

namespace performance
{
    [MemoryDiagnoser]
    [JsonExporter]
    public class Benchmarks
    {
        [Benchmark]
        public void EmptyDocument()
        {
            var document = new OpenApiDocument();
        }

        [Benchmark]
        public void Scenario2()
        {
            // Implement your benchmark here
        }
    }
}
