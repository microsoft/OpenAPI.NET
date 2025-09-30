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
| PetStoreYaml |       677.8 μs |   3,027.4 μs |    165.94 μs |    62.5000 |    11.7188 |         - |    387.38 KB |
| PetStoreJson |       224.0 μs |     158.9 μs |      8.71 μs |    40.0391 |     8.7891 |         - |    249.52 KB |
| GHESYaml     | 1,064,084.8 μs | 345,144.4 μs | 18,918.53 μs | 65000.0000 | 21000.0000 | 3000.0000 | 384510.46 KB |
| GHESJson     |   597,184.6 μs | 692,020.9 μs | 37,932.00 μs | 40000.0000 | 16000.0000 | 3000.0000 | 245983.02 KB |
