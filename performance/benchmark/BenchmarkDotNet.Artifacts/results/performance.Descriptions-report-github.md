```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7840/25H2/2025Update/HudsonValley2)
AMD Ryzen 7 7800X3D 4.20GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.418
  [Host]   : .NET 8.0.24 (8.0.24, 8.0.2426.7010), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.24 (8.0.24, 8.0.2426.7010), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean         | Error         | StdDev       | Gen0      | Gen1      | Gen2      | Allocated    |
|------------- |-------------:|--------------:|-------------:|----------:|----------:|----------:|-------------:|
| PetStoreYaml |     265.0 μs |     201.45 μs |     11.04 μs |    5.8594 |         - |         - |    361.38 KB |
| PetStoreJson |     106.2 μs |      32.80 μs |      1.80 μs |    4.3945 |    0.9766 |         - |    223.52 KB |
| GHESYaml     | 635,099.4 μs | 228,119.02 μs | 12,503.97 μs | 9000.0000 | 8000.0000 | 2000.0000 | 345336.55 KB |
| GHESJson     | 251,638.7 μs | 119,623.42 μs |  6,556.96 μs | 4000.0000 | 3000.0000 | 1000.0000 | 206858.06 KB |
