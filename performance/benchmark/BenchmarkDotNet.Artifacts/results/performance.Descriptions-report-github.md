```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7840/25H2/2025Update/HudsonValley2)
AMD Ryzen 7 7800X3D 4.20GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.418
  [Host]   : .NET 8.0.24 (8.0.24, 8.0.2426.7010), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.24 (8.0.24, 8.0.2426.7010), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean         | Error         | StdDev       | Gen0       | Gen1      | Gen2      | Allocated    |
|------------- |-------------:|--------------:|-------------:|-----------:|----------:|----------:|-------------:|
| PetStoreYaml |     261.1 μs |     105.89 μs |      5.80 μs |     5.8594 |         - |         - |    361.38 KB |
| PetStoreJson |     101.9 μs |      48.20 μs |      2.64 μs |     4.3945 |    0.9766 |         - |    223.52 KB |
| GHESYaml     | 602,932.7 μs | 170,410.86 μs |  9,340.79 μs |  9000.0000 | 8000.0000 | 2000.0000 | 345336.55 KB |
| GHESJson     | 254,976.7 μs | 111,875.43 μs |  6,132.27 μs |  4000.0000 | 3000.0000 | 1000.0000 | 206858.06 KB |
| GHESNextYaml | 729,602.0 μs | 357,122.29 μs | 19,575.08 μs | 13000.0000 | 9000.0000 | 2000.0000 | 541566.37 KB |
| GHESNextJson | 378,208.4 μs | 109,458.45 μs |  5,999.79 μs |  8000.0000 | 5000.0000 | 1000.0000 | 406762.41 KB |
