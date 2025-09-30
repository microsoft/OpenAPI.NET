```

BenchmarkDotNet v0.15.4, Windows 11 (10.0.26200.6584)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.414
  [Host]   : .NET 8.0.20 (8.0.20, 8.0.2025.41914), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.20 (8.0.20, 8.0.2025.41914), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean         | Error       | StdDev      | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |-------------:|------------:|------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |     442.4 μs |    110.3 μs |     6.05 μs |    62.5000 |    11.7188 |         - |    387.38 KB |
| PetStoreJson |     199.5 μs |    106.8 μs |     5.85 μs |    40.0391 |     8.7891 |         - |    249.52 KB |
| GHESYaml     | 959,128.1 μs | 77,233.5 μs | 4,233.43 μs | 66000.0000 | 22000.0000 | 4000.0000 | 384512.09 KB |
| GHESJson     | 459,066.4 μs | 43,160.0 μs | 2,365.75 μs | 40000.0000 | 16000.0000 | 3000.0000 |  245982.3 KB |
