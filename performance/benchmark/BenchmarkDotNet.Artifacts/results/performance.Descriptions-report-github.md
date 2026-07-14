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
| PetStoreYaml |     352.2 μs |     299.63 μs |     16.42 μs |    89.8438 |    15.6250 |         - |    367.82 KB |
| PetStoreJson |     147.2 μs |      88.89 μs |      4.87 μs |    48.8281 |          - |         - |     202.1 KB |
| GHESYaml     | 673,081.0 μs | 446,393.95 μs | 24,468.36 μs | 54000.0000 | 20000.0000 | 3000.0000 | 309626.63 KB |
| GHESJson     | 329,204.5 μs | 228,948.96 μs | 12,549.47 μs | 24000.0000 | 13000.0000 | 3000.0000 | 139236.93 KB |
| GHESNextYaml | 863,940.5 μs | 152,558.37 μs |  8,362.24 μs | 91000.0000 | 21000.0000 | 3000.0000 |  511080.8 KB |
| GHESNextJson | 485,171.6 μs |  68,963.33 μs |  3,780.11 μs | 58000.0000 | 14000.0000 | 3000.0000 |  342912.2 KB |
