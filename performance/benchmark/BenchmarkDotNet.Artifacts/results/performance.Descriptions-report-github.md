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
| PetStoreYaml |     362.4 μs |      40.82 μs |      2.24 μs |    74.2188 |    15.6250 |         - |    307.15 KB |
| PetStoreJson |     151.2 μs |      17.70 μs |      0.97 μs |    41.0156 |     6.8359 |         - |    169.29 KB |
| GHESYaml     | 772,063.1 μs | 161,793.80 μs |  8,868.46 μs | 45000.0000 | 18000.0000 | 3000.0000 | 253280.85 KB |
| GHESJson     | 304,062.4 μs |  99,068.53 μs |  5,430.28 μs | 18000.0000 | 10000.0000 | 2000.0000 | 110452.47 KB |
| GHESNextYaml | 988,379.0 μs |  43,728.33 μs |  2,396.90 μs | 80000.0000 | 19000.0000 | 3000.0000 | 446980.96 KB |
| GHESNextJson | 558,548.3 μs | 292,614.67 μs | 16,039.20 μs | 52000.0000 | 13000.0000 | 3000.0000 | 308740.81 KB |
