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
| PetStoreYaml |     275.7 μs |      16.56 μs |      0.91 μs |    76.1719 |    13.6719 |         - |     313.5 KB |
| PetStoreJson |     120.8 μs |       3.75 μs |      0.21 μs |    42.9688 |    10.7422 |         - |    175.64 KB |
| GHESYaml     | 633,787.7 μs | 233,432.08 μs | 12,795.20 μs | 45000.0000 | 19000.0000 | 3000.0000 | 254632.89 KB |
| GHESJson     | 237,577.2 μs | 200,872.43 μs | 11,010.50 μs | 18000.0000 |  9000.0000 | 2000.0000 | 111804.38 KB |
| GHESNextYaml | 788,550.2 μs | 630,692.15 μs | 34,570.36 μs | 79000.0000 | 20000.0000 | 3000.0000 | 450741.24 KB |
| GHESNextJson | 482,486.5 μs | 298,619.77 μs | 16,368.36 μs | 53000.0000 | 13000.0000 | 3000.0000 | 312508.07 KB |
