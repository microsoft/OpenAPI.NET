```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8457/25H2/2025Update/HudsonValley2)
Snapdragon X 12-core X1E80100 3.40 GHz (Max: 3.42GHz), 1 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.300
  [Host]   : .NET 8.0.27 (8.0.27, 8.0.2726.22922), Arm64 RyuJIT armv8.0-a
  ShortRun : .NET 8.0.27 (8.0.27, 8.0.2726.22922), Arm64 RyuJIT armv8.0-a

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean         | Error         | StdDev      | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |-------------:|--------------:|------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |     371.5 μs |      35.60 μs |     1.95 μs |    74.2188 |    11.7188 |         - |    307.17 KB |
| PetStoreJson |     155.7 μs |      10.23 μs |     0.56 μs |    41.0156 |     6.8359 |         - |    169.31 KB |
| GHESYaml     | 771,340.7 μs |  72,493.09 μs | 3,973.59 μs | 44000.0000 | 18000.0000 | 3000.0000 | 252535.98 KB |
| GHESJson     | 308,100.8 μs | 132,615.87 μs | 7,269.12 μs | 17000.0000 |  9000.0000 | 2000.0000 | 109706.91 KB |
| GHESNextYaml | 999,238.5 μs | 116,421.98 μs | 6,381.48 μs | 80000.0000 | 20000.0000 | 3000.0000 | 446197.67 KB |
| GHESNextJson | 565,582.8 μs |  54,146.09 μs | 2,967.93 μs | 52000.0000 | 14000.0000 | 3000.0000 | 307956.73 KB |
