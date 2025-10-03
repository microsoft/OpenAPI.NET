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
| PetStoreYaml |       525.7 μs |   1,392.5 μs |     76.33 μs |    62.5000 |    11.7188 |         - |    387.72 KB |
| PetStoreJson |       241.1 μs |     706.2 μs |     38.71 μs |    40.0391 |     8.7891 |         - |    249.86 KB |
| GHESYaml     | 1,126,435.6 μs | 361,261.8 μs | 19,801.98 μs | 66000.0000 | 22000.0000 | 4000.0000 | 384551.24 KB |
| GHESJson     |   970,225.6 μs | 187,943.8 μs | 10,301.84 μs | 40000.0000 | 16000.0000 | 3000.0000 | 246022.93 KB |
