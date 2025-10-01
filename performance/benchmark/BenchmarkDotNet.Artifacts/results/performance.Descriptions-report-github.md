```

BenchmarkDotNet v0.15.4, Windows 11 (10.0.26200.6584)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.414
  [Host]   : .NET 8.0.20 (8.0.20, 8.0.2025.41914), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.20 (8.0.20, 8.0.2025.41914), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean           | Error         | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |---------------:|--------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |       452.0 μs |     488.11 μs |     26.76 μs |    62.5000 |    11.7188 |         - |    387.38 KB |
| PetStoreJson |       199.6 μs |      83.24 μs |      4.56 μs |    40.0391 |     8.7891 |         - |    249.52 KB |
| GHESYaml     | 1,001,468.3 μs | 766,455.68 μs | 42,012.02 μs | 65000.0000 | 21000.0000 | 3000.0000 | 384523.34 KB |
| GHESJson     |   505,102.8 μs | 143,429.27 μs |  7,861.84 μs | 40000.0000 | 16000.0000 | 3000.0000 | 245995.15 KB |
