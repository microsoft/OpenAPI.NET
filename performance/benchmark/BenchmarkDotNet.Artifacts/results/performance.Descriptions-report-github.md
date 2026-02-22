```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7840/25H2/2025Update/HudsonValley2)
AMD Ryzen 7 7800X3D 4.20GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.418
  [Host]   : .NET 8.0.24 (8.0.24, 8.0.2426.7010), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.24 (8.0.24, 8.0.2426.7010), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean         | Error           | StdDev       | Gen0       | Gen1      | Gen2      | Allocated    |
|------------- |-------------:|----------------:|-------------:|-----------:|----------:|----------:|-------------:|
| PetStoreYaml |     266.8 μs |       423.33 μs |     23.20 μs |     5.8594 |         - |         - |    361.38 KB |
| PetStoreJson |     104.9 μs |        38.83 μs |      2.13 μs |     4.3945 |    0.9766 |         - |    223.52 KB |
| GHESYaml     | 627,326.7 μs |   525,719.23 μs | 28,816.45 μs |  9000.0000 | 8000.0000 | 2000.0000 | 345336.55 KB |
| GHESJson     | 256,258.8 μs |   156,676.06 μs |  8,587.94 μs |  4000.0000 | 3000.0000 | 1000.0000 | 206858.38 KB |
| GHESNextYaml | 840,612.7 μs | 1,063,489.38 μs | 58,293.44 μs | 20000.0000 | 9000.0000 | 2000.0000 | 908819.18 KB |
| GHESNextJson | 478,093.3 μs |    98,044.22 μs |  5,374.13 μs | 16000.0000 | 7000.0000 | 1000.0000 | 774015.55 KB |
