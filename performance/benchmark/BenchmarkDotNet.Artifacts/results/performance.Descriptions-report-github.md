```

BenchmarkDotNet v0.15.4, Windows 11 (10.0.26200.6584)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.414
  [Host]   : .NET 8.0.20 (8.0.20, 8.0.2025.41914), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.20 (8.0.20, 8.0.2025.41914), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean           | Error        | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |---------------:|-------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |       499.0 μs |     459.9 μs |     25.21 μs |    62.5000 |    11.7188 |         - |    387.71 KB |
| PetStoreJson |       240.7 μs |     638.0 μs |     34.97 μs |    40.0391 |     8.7891 |         - |    249.85 KB |
| GHESYaml     | 1,055,965.3 μs | 311,428.8 μs | 17,070.46 μs | 66000.0000 | 22000.0000 | 4000.0000 | 384550.33 KB |
| GHESJson     |   540,193.4 μs | 107,223.7 μs |  5,877.29 μs | 40000.0000 | 16000.0000 | 3000.0000 | 246021.04 KB |
