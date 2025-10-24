```

BenchmarkDotNet v0.15.4, Windows 11 (10.0.26200.6899)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.415
  [Host]   : .NET 8.0.21 (8.0.21, 8.0.2125.47513), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.21 (8.0.21, 8.0.2125.47513), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean         | Error         | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |-------------:|--------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |     407.7 μs |     134.55 μs |      7.38 μs |    58.5938 |     7.8125 |         - |     360.8 KB |
| PetStoreJson |     166.3 μs |      23.12 μs |      1.27 μs |    36.1328 |     6.8359 |         - |    222.95 KB |
| GHESYaml     | 896,578.2 μs | 138,441.39 μs |  7,588.44 μs | 60000.0000 | 23000.0000 | 4000.0000 |  345015.7 KB |
| GHESJson     | 432,991.2 μs | 243,041.11 μs | 13,321.90 μs | 33000.0000 | 12000.0000 | 2000.0000 | 206538.29 KB |
