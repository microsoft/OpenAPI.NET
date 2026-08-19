```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]   : .NET 8.0.30 (8.0.3026.36720), X64 RyuJIT AVX2
  ShortRun : .NET 8.0.30 (8.0.3026.36720), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean           | Error        | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |---------------:|-------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |       750.3 μs |     320.4 μs |     17.56 μs |    23.4375 |     3.9063 |         - |    438.94 KB |
| PetStoreJson |       875.7 μs |     167.0 μs |      9.15 μs |    25.3906 |     1.9531 |         - |    446.38 KB |
| GHESYaml     | 1,775,302.8 μs | 137,608.1 μs |  7,542.76 μs | 30000.0000 | 23000.0000 | 4000.0000 | 438624.06 KB |
| GHESJson     | 1,924,124.8 μs | 514,004.3 μs | 28,174.31 μs | 30000.0000 | 23000.0000 | 4000.0000 | 445806.12 KB |
