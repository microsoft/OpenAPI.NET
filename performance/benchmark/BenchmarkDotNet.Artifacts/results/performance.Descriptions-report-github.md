```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7781/25H2/2025Update/HudsonValley2)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.418
  [Host]   : .NET 8.0.24 (8.0.24, 8.0.2426.7010), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.24 (8.0.24, 8.0.2426.7010), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean         | Error         | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |-------------:|--------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |     405.2 μs |     162.42 μs |      8.90 μs |    58.5938 |    11.7188 |         - |    361.39 KB |
| PetStoreJson |     173.0 μs |      23.21 μs |      1.27 μs |    36.1328 |     6.8359 |         - |    223.53 KB |
| GHESYaml     | 951,103.8 μs | 629,262.79 μs | 34,492.02 μs | 60000.0000 | 22000.0000 | 4000.0000 | 345343.16 KB |
| GHESJson     | 435,425.1 μs |  88,170.37 μs |  4,832.92 μs | 33000.0000 | 12000.0000 | 2000.0000 | 206865.71 KB |
