```

BenchmarkDotNet v0.15.8, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
AMD EPYC 9V74 3.65GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.404
  [Host]   : .NET 8.0.34 (8.0.34, 8.0.3426.52217), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.34 (8.0.34, 8.0.3426.52217), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean         | Error         | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |-------------:|--------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |     483.5 μs |     464.94 μs |     25.48 μs |    19.5313 |          - |         - |    327.73 KB |
| PetStoreJson |     178.7 μs |      18.75 μs |      1.03 μs |    12.6953 |     2.9297 |         - |     209.6 KB |
| GHESYaml     | 738,204.1 μs | 118,424.49 μs |  6,491.25 μs | 18000.0000 | 10000.0000 | 2000.0000 | 267536.02 KB |
| GHESJson     | 375,952.3 μs |  95,669.76 μs |  5,243.98 μs |  9000.0000 |  8000.0000 | 2000.0000 | 140882.55 KB |
| GHESNextYaml | 772,822.5 μs | 156,298.41 μs |  8,567.24 μs | 19000.0000 | 10000.0000 | 2000.0000 |  282327.3 KB |
| GHESNextJson | 423,406.4 μs | 189,929.70 μs | 10,410.69 μs | 10000.0000 |  9000.0000 | 2000.0000 | 158070.03 KB |
