```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8655/25H2/2025Update/HudsonValley2)
Snapdragon X 12-core X1E80100 3.40 GHz (Max: 3.42GHz), 1 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.301
  [Host]   : .NET 8.0.28 (8.0.28, 8.0.2826.26413), Arm64 RyuJIT armv8.0-a
  ShortRun : .NET 8.0.28 (8.0.28, 8.0.2826.26413), Arm64 RyuJIT armv8.0-a

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean         | Error         | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |-------------:|--------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |     275.1 μs |      37.61 μs |      2.06 μs |    76.1719 |    13.6719 |         - |    314.16 KB |
| PetStoreJson |     117.9 μs |       0.53 μs |      0.03 μs |    42.9688 |     0.4883 |         - |     176.3 KB |
| GHESYaml     | 634,726.6 μs |  63,613.02 μs |  3,486.84 μs | 45000.0000 | 19000.0000 | 3000.0000 | 254776.13 KB |
| GHESJson     | 255,639.6 μs | 104,996.42 μs |  5,755.21 μs | 18000.0000 |  9000.0000 | 2000.0000 | 111947.58 KB |
| GHESNextYaml | 809,785.9 μs | 250,759.05 μs | 13,744.95 μs | 80000.0000 | 20000.0000 | 3000.0000 | 450741.27 KB |
| GHESNextJson | 465,627.4 μs |  58,289.71 μs |  3,195.06 μs | 53000.0000 | 13000.0000 | 3000.0000 | 312505.27 KB |
