```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.417
  [Host]   : .NET 8.0.23 (8.0.23, 8.0.2325.60607), X64 RyuJIT x86-64-v3
  ShortRun : .NET 8.0.23 (8.0.23, 8.0.2325.60607), X64 RyuJIT x86-64-v3

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean           | Error         | StdDev      | Gen0       | Gen1       | Gen2      | Allocated   |
|------------- |---------------:|--------------:|------------:|-----------:|-----------:|----------:|------------:|
| PetStoreYaml |       506.2 μs |      95.21 μs |     5.22 μs |    19.5313 |     3.9063 |         - |   361.38 KB |
| PetStoreJson |       227.4 μs |     158.42 μs |     8.68 μs |    11.7188 |     1.9531 |         - |   223.24 KB |
| GHESYaml     | 1,077,570.1 μs |  87,375.79 μs | 4,789.36 μs | 24000.0000 | 19000.0000 | 3000.0000 | 345338.7 KB |
| GHESJson     |   478,591.0 μs | 114,328.37 μs | 6,266.72 μs | 13000.0000 |  9000.0000 | 2000.0000 | 206861.5 KB |
