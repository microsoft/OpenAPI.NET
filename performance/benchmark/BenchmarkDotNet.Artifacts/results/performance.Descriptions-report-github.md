```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9711)
Snapdragon X 12-core X1E80100 3.40 GHz (Max: 3.42GHz), 1 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.404
  [Host]   : .NET 8.0.34 (8.0.34, 8.0.3426.52217), Arm64 RyuJIT armv8.0-a
  ShortRun : .NET 8.0.34 (8.0.34, 8.0.3426.52217), Arm64 RyuJIT armv8.0-a

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean         | Error         | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |-------------:|--------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |     415.7 μs |     152.24 μs |      8.34 μs |    78.1250 |          - |         - |    328.74 KB |
| PetStoreJson |     174.4 μs |      97.36 μs |      5.34 μs |    50.7813 |          - |         - |     210.9 KB |
| GHESYaml     | 707,562.0 μs | 277,871.52 μs | 15,231.08 μs | 47000.0000 | 14000.0000 | 3000.0000 | 267861.29 KB |
| GHESJson     | 364,989.6 μs |  44,023.13 μs |  2,413.06 μs | 23000.0000 | 11000.0000 | 2000.0000 | 141208.05 KB |
| GHESNextYaml | 705,198.1 μs | 217,842.24 μs | 11,940.67 μs | 50000.0000 | 15000.0000 | 3000.0000 | 282677.81 KB |
| GHESNextJson | 401,263.3 μs | 207,253.17 μs | 11,360.25 μs | 26000.0000 | 12000.0000 | 2000.0000 | 158421.13 KB |
