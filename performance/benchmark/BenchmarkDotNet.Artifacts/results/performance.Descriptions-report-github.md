```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]   : .NET 8.0.29 (8.0.29, 8.0.2926.32403), X64 RyuJIT x86-64-v3
  ShortRun : .NET 8.0.29 (8.0.29, 8.0.2926.32403), X64 RyuJIT x86-64-v3

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean           | Error         | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |---------------:|--------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |       529.6 μs |     159.05 μs |      8.72 μs |    19.5313 |     3.9063 |         - |    375.68 KB |
| PetStoreJson |       211.9 μs |      12.09 μs |      0.66 μs |    12.6953 |     2.9297 |         - |    209.67 KB |
| GHESYaml     |   961,450.4 μs | 493,674.21 μs | 27,059.95 μs | 22000.0000 | 18000.0000 | 3000.0000 | 310249.58 KB |
| GHESJson     |   341,906.3 μs |  72,259.82 μs |  3,960.80 μs |  9000.0000 |  8000.0000 | 2000.0000 | 139861.88 KB |
| GHESNextYaml | 1,140,795.3 μs |  75,343.64 μs |  4,129.84 μs | 34000.0000 | 19000.0000 | 3000.0000 | 512935.25 KB |
| GHESNextJson |   545,592.7 μs |  36,360.40 μs |  1,993.04 μs | 22000.0000 | 10000.0000 | 2000.0000 | 344761.41 KB |
