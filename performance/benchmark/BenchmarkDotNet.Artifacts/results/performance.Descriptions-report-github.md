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
| PetStoreYaml |       833.4 μs |   1,308.6 μs |     71.73 μs |    62.5000 |     7.8125 |         - |    387.49 KB |
| PetStoreJson |       223.4 μs |     175.2 μs |      9.60 μs |    40.0391 |     8.7891 |         - |    249.63 KB |
| GHESYaml     | 1,249,667.2 μs | 733,749.3 μs | 40,219.28 μs | 66000.0000 | 22000.0000 | 4000.0000 | 384533.09 KB |
| GHESJson     |   653,588.2 μs | 729,353.4 μs | 39,978.32 μs | 40000.0000 | 16000.0000 | 3000.0000 | 246004.59 KB |
