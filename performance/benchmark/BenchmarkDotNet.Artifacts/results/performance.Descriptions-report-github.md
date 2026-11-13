```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9711)
Snapdragon X 12-core X1E80100 3.40 GHz (Max: 3.42GHz), 1 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.403
  [Host]   : .NET 8.0.34 (8.0.34, 8.0.3426.52217), Arm64 RyuJIT armv8.0-a
  ShortRun : .NET 8.0.34 (8.0.34, 8.0.3426.52217), Arm64 RyuJIT armv8.0-a

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean         | Error         | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |-------------:|--------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |     345.8 μs |      69.85 μs |      3.83 μs |    78.1250 |    11.7188 |         - |    329.19 KB |
| PetStoreJson |     155.7 μs |       1.06 μs |      0.06 μs |    50.7813 |     9.7656 |         - |    211.35 KB |
| GHESYaml     | 608,273.7 μs |  86,098.51 μs |  4,719.35 μs | 47000.0000 | 14000.0000 | 3000.0000 | 267921.01 KB |
| GHESJson     | 325,277.9 μs |  83,983.75 μs |  4,603.43 μs | 23000.0000 | 11000.0000 | 2000.0000 | 141267.53 KB |
| GHESNextYaml | 641,025.5 μs | 220,286.78 μs | 12,074.66 μs | 50000.0000 | 15000.0000 | 3000.0000 |  282745.1 KB |
| GHESNextJson | 362,873.4 μs | 136,951.76 μs |  7,506.79 μs | 26000.0000 | 12000.0000 | 2000.0000 | 158487.98 KB |
