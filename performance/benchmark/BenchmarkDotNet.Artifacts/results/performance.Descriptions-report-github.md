```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7840/25H2/2025Update/HudsonValley2)
AMD Ryzen 7 7800X3D 4.20GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.418
  [Host]   : .NET 8.0.24 (8.0.24, 8.0.2426.7010), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.24 (8.0.24, 8.0.2426.7010), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean           | Error         | StdDev       | Gen0        | Gen1       | Gen2      | Allocated     |
|------------- |---------------:|--------------:|-------------:|------------:|-----------:|----------:|--------------:|
| PetStoreYaml |       285.1 μs |     467.14 μs |     25.61 μs |      5.8594 |          - |         - |     361.38 KB |
| PetStoreJson |       110.2 μs |      75.79 μs |      4.15 μs |      4.3945 |     0.9766 |         - |     223.52 KB |
| GHESYaml     |   687,947.0 μs | 659,775.92 μs | 36,164.54 μs |   9000.0000 |  8000.0000 | 2000.0000 |  345336.55 KB |
| GHESJson     |   270,583.7 μs | 134,378.11 μs |  7,365.72 μs |   4000.0000 |  3000.0000 | 1000.0000 |  206858.06 KB |
| GHESNextYaml | 3,443,318.8 μs | 127,519.37 μs |  6,989.77 μs | 160000.0000 | 11000.0000 | 3000.0000 | 7701975.75 KB |
| GHESNextJson | 2,954,715.0 μs | 327,737.12 μs | 17,964.38 μs | 155000.0000 | 19000.0000 | 1000.0000 | 7567167.19 KB |
