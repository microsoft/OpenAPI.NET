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
| PetStoreYaml |     267.2 μs |      40.45 μs |      2.22 μs |    74.2188 |          - |         - |    308.02 KB |
| PetStoreJson |     112.4 μs |      21.93 μs |      1.20 μs |    41.5039 |     2.4414 |         - |    170.17 KB |
| GHESYaml     | 616,153.2 μs | 136,440.25 μs |  7,478.75 μs | 45000.0000 | 18000.0000 | 3000.0000 | 253472.06 KB |
| GHESJson     | 252,074.5 μs | 466,491.71 μs | 25,569.98 μs | 18000.0000 |  9000.0000 | 2000.0000 | 110643.88 KB |
| GHESNextYaml | 793,235.7 μs | 226,448.39 μs | 12,412.40 μs | 80000.0000 | 19000.0000 | 3000.0000 | 447183.69 KB |
| GHESNextJson | 460,463.3 μs | 239,580.50 μs | 13,132.22 μs | 53000.0000 | 13000.0000 | 3000.0000 | 308943.27 KB |
