```

BenchmarkDotNet v0.15.4, Windows 11 (10.0.26200.6899)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.415
  [Host]   : .NET 8.0.21 (8.0.21, 8.0.2125.47513), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.21 (8.0.21, 8.0.2125.47513), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean           | Error          | StdDev        | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |---------------:|---------------:|--------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |       913.5 μs |     3,348.1 μs |     183.52 μs |    58.5938 |    11.7188 |         - |    361.25 KB |
| PetStoreJson |       425.1 μs |       327.5 μs |      17.95 μs |    35.1563 |     5.8594 |         - |    223.39 KB |
| GHESYaml     | 1,765,825.8 μs | 5,036,336.6 μs | 276,058.60 μs | 60000.0000 | 23000.0000 | 4000.0000 | 345082.98 KB |
| GHESJson     |   848,197.4 μs | 1,381,723.6 μs |  75,736.93 μs | 33000.0000 | 12000.0000 | 2000.0000 | 206597.63 KB |
