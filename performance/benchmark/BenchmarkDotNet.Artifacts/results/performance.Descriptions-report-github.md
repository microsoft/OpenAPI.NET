```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8457/25H2/2025Update/HudsonValley2)
Snapdragon X 12-core X1E80100 3.40 GHz (Max: 3.42GHz), 1 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.300
  [Host]   : .NET 8.0.27 (8.0.27, 8.0.2726.22922), Arm64 RyuJIT armv8.0-a
  ShortRun : .NET 8.0.27 (8.0.27, 8.0.2726.22922), Arm64 RyuJIT armv8.0-a

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean         | Error         | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |-------------:|--------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |     276.3 μs |      38.27 μs |      2.10 μs |    74.2188 |    11.7188 |         - |    305.91 KB |
| PetStoreJson |     112.8 μs |       2.80 μs |      0.15 μs |    41.0156 |     0.4883 |         - |    168.05 KB |
| GHESYaml     | 608,668.3 μs | 188,763.29 μs | 10,346.75 μs | 44000.0000 | 18000.0000 | 3000.0000 | 250121.85 KB |
| GHESJson     | 244,147.6 μs | 361,794.79 μs | 19,831.19 μs | 17000.0000 |  9000.0000 | 2000.0000 | 107293.42 KB |
| GHESNextYaml | 765,440.1 μs |  23,162.26 μs |  1,269.60 μs | 79000.0000 | 20000.0000 | 3000.0000 | 443655.46 KB |
| GHESNextJson | 435,329.2 μs | 241,612.89 μs | 13,243.62 μs | 51000.0000 | 11000.0000 | 2000.0000 | 305423.41 KB |
