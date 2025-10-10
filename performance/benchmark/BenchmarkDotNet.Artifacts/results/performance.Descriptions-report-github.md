```

BenchmarkDotNet v0.15.4, Windows 11 (10.0.26200.6584)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.414
  [Host]   : .NET 8.0.20 (8.0.20, 8.0.2025.41914), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.20 (8.0.20, 8.0.2025.41914), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean           | Error          | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |---------------:|---------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |       524.4 μs |       176.0 μs |      9.65 μs |    62.5000 |    11.7188 |         - |    387.72 KB |
| PetStoreJson |       214.4 μs |       531.5 μs |     29.13 μs |    40.0391 |     8.7891 |         - |    249.86 KB |
| GHESYaml     | 1,118,727.3 μs | 1,445,393.2 μs | 79,226.88 μs | 66000.0000 | 22000.0000 | 4000.0000 | 384551.19 KB |
| GHESJson     |   586,923.9 μs |   475,182.0 μs | 26,046.33 μs | 40000.0000 | 16000.0000 | 3000.0000 | 246021.88 KB |
