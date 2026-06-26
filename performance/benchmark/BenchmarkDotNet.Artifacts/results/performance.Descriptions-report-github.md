```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8655/25H2/2025Update/HudsonValley2)
Snapdragon X 12-core X1E80100 3.40 GHz (Max: 3.42GHz), 1 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.301
  [Host]   : .NET 8.0.28 (8.0.28, 8.0.2826.26413), Arm64 RyuJIT armv8.0-a
  ShortRun : .NET 8.0.28 (8.0.28, 8.0.2826.26413), Arm64 RyuJIT armv8.0-a

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean          | Error          | StdDev        | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |--------------:|---------------:|--------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |     281.26 μs |      87.951 μs |      4.821 μs |    74.2188 |    15.6250 |         - |    313.05 KB |
| PetStoreJson |      99.37 μs |       7.974 μs |      0.437 μs |    42.4805 |     8.3008 |         - |     175.2 KB |
| GHESYaml     | 558,007.40 μs | 245,240.510 μs | 13,442.460 μs | 45000.0000 | 19000.0000 | 3000.0000 | 254573.63 KB |
| GHESJson     | 263,444.90 μs | 586,112.802 μs | 32,126.821 μs | 18000.0000 |  9000.0000 | 2000.0000 | 111745.13 KB |
| GHESNextYaml | 785,094.03 μs | 214,946.431 μs | 11,781.939 μs | 80000.0000 | 20000.0000 | 3000.0000 |  450674.2 KB |
| GHESNextJson | 461,067.00 μs |  50,227.814 μs |  2,753.156 μs | 53000.0000 | 13000.0000 | 3000.0000 | 312434.25 KB |
