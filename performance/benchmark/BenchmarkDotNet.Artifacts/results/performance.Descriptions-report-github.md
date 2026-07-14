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
| PetStoreYaml |     295.0 μs |      11.08 μs |      0.61 μs |    91.7969 |    17.5781 |         - |    375.25 KB |
| PetStoreJson |     145.9 μs |      27.23 μs |      1.49 μs |    50.7813 |    11.7188 |         - |    209.53 KB |
| GHESYaml     | 673,002.8 μs | 442,503.72 μs | 24,255.12 μs | 54000.0000 | 20000.0000 | 3000.0000 | 310978.47 KB |
| GHESJson     | 289,915.7 μs |  48,490.16 μs |  2,657.91 μs | 23000.0000 | 11000.0000 | 2000.0000 |    140588 KB |
| GHESNextYaml | 831,243.5 μs | 195,694.95 μs | 10,726.70 μs | 91000.0000 | 21000.0000 | 3000.0000 | 513236.66 KB |
| GHESNextJson | 499,654.1 μs | 154,061.41 μs |  8,444.63 μs | 58000.0000 | 14000.0000 | 3000.0000 | 345064.25 KB |
