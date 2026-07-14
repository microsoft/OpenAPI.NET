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
| PetStoreYaml |     286.0 μs |      73.19 μs |      4.01 μs |    78.1250 |     3.9063 |         - |    321.56 KB |
| PetStoreJson |     138.4 μs |     281.66 μs |     15.44 μs |    44.9219 |     0.9766 |         - |     183.7 KB |
| GHESYaml     | 634,204.7 μs | 753,841.96 μs | 41,320.62 μs | 46000.0000 | 19000.0000 | 3000.0000 | 258029.89 KB |
| GHESJson     | 276,364.3 μs | 194,356.19 μs | 10,653.32 μs | 19000.0000 | 10000.0000 | 2000.0000 | 115201.33 KB |
| GHESNextYaml | 826,374.3 μs |  43,744.86 μs |  2,397.80 μs | 81000.0000 | 20000.0000 | 3000.0000 | 454862.14 KB |
| GHESNextJson | 492,571.5 μs |  71,304.93 μs |  3,908.46 μs | 54000.0000 | 14000.0000 | 3000.0000 | 316622.34 KB |
