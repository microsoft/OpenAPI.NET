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
| PetStoreYaml |     475.0 μs |     448.4 μs |     24.58 μs |    62.5000 |    11.7188 |         - |    387.38 KB |
| PetStoreJson |     202.9 μs |     127.2 μs |      6.97 μs |    40.0391 |     8.7891 |         - |    249.52 KB |
| GHESYaml     | 948,429.3 μs | 329,938.7 μs | 18,085.05 μs | 66000.0000 | 22000.0000 | 4000.0000 | 384511.61 KB |
| GHESJson     | 543,072.2 μs | 750,614.8 μs | 41,143.73 μs | 40000.0000 | 16000.0000 | 3000.0000 | 245982.33 KB |
