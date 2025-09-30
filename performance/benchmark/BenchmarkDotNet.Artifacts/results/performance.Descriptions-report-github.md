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
| PetStoreYaml |       519.5 μs |     807.9 μs |     44.29 μs |    62.5000 |    11.7188 |         - |    387.37 KB |
| PetStoreJson |       234.0 μs |     166.2 μs |      9.11 μs |    40.0391 |     7.8125 |         - |    249.52 KB |
| GHESYaml     | 1,120,391.4 μs | 912,897.7 μs | 50,039.00 μs | 65000.0000 | 21000.0000 | 3000.0000 | 384510.39 KB |
| GHESJson     |   585,492.8 μs | 734,663.2 μs | 40,269.37 μs | 40000.0000 | 16000.0000 | 3000.0000 | 245982.27 KB |
