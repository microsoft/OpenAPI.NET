```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.61GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]   : .NET 8.0.28 (8.0.28, 8.0.2826.26413), X64 RyuJIT x86-64-v3
  ShortRun : .NET 8.0.28 (8.0.28, 8.0.2826.26413), X64 RyuJIT x86-64-v3

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean           | Error         | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |---------------:|--------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |       490.9 μs |     282.59 μs |     15.49 μs |    19.5313 |     3.9063 |         - |    321.99 KB |
| PetStoreJson |       203.9 μs |      15.77 μs |      0.86 μs |    10.7422 |     1.9531 |         - |    183.85 KB |
| GHESYaml     |   884,972.5 μs |  24,370.58 μs |  1,335.83 μs | 18000.0000 | 17000.0000 | 3000.0000 | 258042.35 KB |
| GHESJson     |   284,350.7 μs |  61,542.17 μs |  3,373.33 μs |  8000.0000 |  7000.0000 | 2000.0000 | 115210.25 KB |
| GHESNextYaml | 1,133,321.0 μs | 304,160.14 μs | 16,672.04 μs | 30000.0000 | 18000.0000 | 3000.0000 | 454924.16 KB |
| GHESNextJson |   547,652.0 μs | 183,032.67 μs | 10,032.64 μs | 20000.0000 | 10000.0000 | 2000.0000 | 316687.77 KB |
