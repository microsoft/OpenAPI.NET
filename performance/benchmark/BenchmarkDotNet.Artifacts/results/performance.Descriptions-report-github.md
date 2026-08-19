```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.400
  [Host]   : .NET 8.0.30 (8.0.30, 8.0.3026.36720), X64 RyuJIT x86-64-v3
  ShortRun : .NET 8.0.30 (8.0.30, 8.0.3026.36720), X64 RyuJIT x86-64-v3

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean           | Error         | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |---------------:|--------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |       569.0 μs |     108.17 μs |      5.93 μs |    19.5313 |          - |         - |     327.8 KB |
| PetStoreJson |       250.8 μs |      20.85 μs |      1.14 μs |    11.7188 |     1.9531 |         - |    209.67 KB |
| GHESYaml     |   831,169.7 μs | 217,393.05 μs | 11,916.05 μs | 18000.0000 | 10000.0000 | 2000.0000 |  267570.8 KB |
| GHESJson     |   367,711.4 μs | 135,955.63 μs |  7,452.19 μs |  9000.0000 |  8000.0000 | 2000.0000 | 140917.06 KB |
| GHESNextYaml | 1,040,243.2 μs | 167,595.29 μs |  9,186.46 μs | 30000.0000 | 11000.0000 | 2000.0000 | 469507.05 KB |
| GHESNextJson |   615,173.6 μs | 143,051.39 μs |  7,841.13 μs | 22000.0000 | 10000.0000 | 2000.0000 | 345247.91 KB |
