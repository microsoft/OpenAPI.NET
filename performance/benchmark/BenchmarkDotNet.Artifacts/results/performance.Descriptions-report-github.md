```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9106/25H2/2025Update/HudsonValley2) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.303
  [Host]   : .NET 8.0.30 (8.0.30, 8.0.3026.36720), X64 RyuJIT x86-64-v3
  ShortRun : .NET 8.0.30 (8.0.30, 8.0.3026.36720), X64 RyuJIT x86-64-v3

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean           | Error          | StdDev        | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |---------------:|---------------:|--------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |       955.1 μs |     2,072.5 μs |     113.60 μs |    15.6250 |          - |         - |    327.36 KB |
| PetStoreJson |       299.0 μs |       145.4 μs |       7.97 μs |    11.7188 |     1.9531 |         - |    209.52 KB |
| GHESYaml     | 1,198,752.5 μs | 3,307,785.9 μs | 181,310.91 μs | 18000.0000 | 10000.0000 | 2000.0000 | 267040.88 KB |
| GHESJson     |   615,836.5 μs |   496,026.6 μs |  27,188.89 μs |  9000.0000 |  8000.0000 | 2000.0000 | 140387.58 KB |
| GHESNextYaml | 1,379,670.6 μs | 1,464,340.6 μs |  80,265.45 μs | 30000.0000 | 11000.0000 | 2000.0000 | 468968.06 KB |
| GHESNextJson |   950,071.7 μs |   425,730.9 μs |  23,335.75 μs | 22000.0000 | 10000.0000 | 2000.0000 | 344709.98 KB |
