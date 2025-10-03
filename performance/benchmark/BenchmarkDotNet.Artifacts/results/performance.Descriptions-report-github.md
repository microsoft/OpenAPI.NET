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
| PetStoreYaml |       492.5 μs |   1,328.0 μs |     72.79 μs |    62.5000 |    11.7188 |         - |    387.71 KB |
| PetStoreJson |       197.1 μs |     127.1 μs |      6.97 μs |    40.0391 |     8.7891 |         - |    249.85 KB |
| GHESYaml     | 1,134,963.6 μs | 681,713.6 μs | 37,367.02 μs | 66000.0000 | 22000.0000 | 4000.0000 | 384551.49 KB |
| GHESJson     |   750,469.7 μs | 851,637.3 μs | 46,681.12 μs | 40000.0000 | 15000.0000 | 3000.0000 | 246021.99 KB |
