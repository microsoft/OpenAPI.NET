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
        public OpenApiDocument EmptyDocument()
        {
            return new OpenApiDocument();
        }

        [Benchmark]
        public void Scenario2()
        {
            // Implement your benchmark here
        }
    }
}
