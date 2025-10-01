```

BenchmarkDotNet v0.15.4, Windows 11 (10.0.26200.6584)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.414
  [Host]   : .NET 8.0.20 (8.0.20, 8.0.2025.41914), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.20 (8.0.20, 8.0.2025.41914), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean         | Error        | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |-------------:|-------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |     439.2 μs |     322.1 μs |     17.65 μs |    62.5000 |    11.7188 |         - |    387.38 KB |
| PetStoreJson |     197.7 μs |     181.5 μs |      9.95 μs |    40.0391 |     8.7891 |         - |    249.52 KB |
| GHESYaml     | 912,587.8 μs | 748,677.7 μs | 41,037.55 μs | 66000.0000 | 22000.0000 | 4000.0000 | 384511.62 KB |
| GHESJson     | 476,268.8 μs | 199,447.2 μs | 10,932.38 μs | 40000.0000 | 16000.0000 | 3000.0000 |  245982.3 KB |
