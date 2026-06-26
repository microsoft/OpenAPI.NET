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
| PetStoreYaml |     276.5 μs |      30.06 μs |      1.65 μs |    74.2188 |     7.8125 |         - |    308.47 KB |
| PetStoreJson |     111.8 μs |      21.37 μs |      1.17 μs |    41.5039 |          - |         - |    170.61 KB |
| GHESYaml     | 622,060.6 μs |  86,657.16 μs |  4,749.97 μs | 45000.0000 | 18000.0000 | 3000.0000 | 253531.41 KB |
| GHESJson     | 261,208.7 μs |   8,228.39 μs |    451.03 μs | 18000.0000 |  9000.0000 | 2000.0000 | 110703.15 KB |
| GHESNextYaml | 837,135.5 μs | 595,784.29 μs | 32,656.95 μs | 80000.0000 | 19000.0000 | 3000.0000 |  447250.7 KB |
| GHESNextJson | 456,841.5 μs | 104,869.19 μs |  5,748.23 μs | 53000.0000 | 13000.0000 | 3000.0000 | 309010.66 KB |
