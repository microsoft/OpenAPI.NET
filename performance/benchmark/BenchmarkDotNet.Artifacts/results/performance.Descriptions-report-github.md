```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]   : .NET 8.0.28 (8.0.28, 8.0.2826.26413), X64 RyuJIT x86-64-v3
  ShortRun : .NET 8.0.28 (8.0.28, 8.0.2826.26413), X64 RyuJIT x86-64-v3

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean           | Error         | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |---------------:|--------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |       491.4 μs |      72.31 μs |      3.96 μs |    18.5547 |     3.9063 |         - |    314.57 KB |
| PetStoreJson |       199.4 μs |      19.87 μs |      1.09 μs |    10.7422 |     2.9297 |         - |    176.42 KB |
| GHESYaml     |   939,520.0 μs | 187,454.80 μs | 10,275.03 μs | 18000.0000 | 17000.0000 | 3000.0000 | 256688.91 KB |
| GHESJson     |   282,084.8 μs | 100,501.66 μs |  5,508.84 μs |  6000.0000 |  5000.0000 | 1000.0000 | 113857.84 KB |
| GHESNextYaml | 1,152,354.7 μs |  45,814.56 μs |  2,511.25 μs | 30000.0000 | 18000.0000 | 3000.0000 |  452765.4 KB |
| GHESNextJson |   549,904.6 μs |  80,767.59 μs |  4,427.14 μs | 20000.0000 | 10000.0000 | 2000.0000 | 314529.52 KB |
