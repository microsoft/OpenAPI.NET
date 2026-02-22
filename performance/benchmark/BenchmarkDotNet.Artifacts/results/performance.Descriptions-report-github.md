```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7840/25H2/2025Update/HudsonValley2)
AMD Ryzen 7 7800X3D 4.20GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.418
  [Host]   : .NET 8.0.24 (8.0.24, 8.0.2426.7010), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.24 (8.0.24, 8.0.2426.7010), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean           | Error           | StdDev       | Gen0        | Gen1       | Gen2      | Allocated     |
|------------- |---------------:|----------------:|-------------:|------------:|-----------:|----------:|--------------:|
| PetStoreYaml |       279.0 μs |       272.39 μs |     14.93 μs |      5.8594 |          - |         - |     361.38 KB |
| PetStoreJson |       102.9 μs |        17.98 μs |      0.99 μs |      4.3945 |     0.9766 |         - |     223.52 KB |
| GHESYaml     |   635,487.8 μs |   279,254.76 μs | 15,306.90 μs |   9000.0000 |  8000.0000 | 2000.0000 |  345336.55 KB |
| GHESJson     |   277,064.1 μs |   164,049.38 μs |  8,992.10 μs |   4000.0000 |  3000.0000 | 1000.0000 |  206858.06 KB |
| GHESNextYaml | 4,479,297.6 μs | 1,561,306.63 μs | 85,580.48 μs | 191000.0000 | 11000.0000 | 3000.0000 | 9268440.43 KB |
| GHESNextJson | 3,305,679.1 μs |   679,209.31 μs | 37,229.75 μs | 186000.0000 | 17000.0000 | 1000.0000 |  9133635.6 KB |
