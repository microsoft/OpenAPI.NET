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
| PetStoreYaml |       582.2 μs |     208.61 μs |     11.43 μs |    19.5313 |     3.9063 |         - |    375.68 KB |
| PetStoreJson |       252.3 μs |      29.46 μs |      1.61 μs |    11.7188 |     1.9531 |         - |    209.67 KB |
| GHESYaml     | 1,005,560.1 μs | 211,112.24 μs | 11,571.77 μs | 22000.0000 | 18000.0000 | 3000.0000 | 311035.48 KB |
| GHESJson     |   369,047.2 μs |  94,799.88 μs |  5,196.30 μs |  9000.0000 |  8000.0000 | 2000.0000 | 140644.21 KB |
| GHESNextYaml | 1,241,432.6 μs | 106,390.00 μs |  5,831.60 μs | 34000.0000 | 19000.0000 | 3000.0000 | 513301.23 KB |
| GHESNextJson |   626,499.0 μs |  70,802.74 μs |  3,880.94 μs | 22000.0000 | 10000.0000 | 2000.0000 | 345119.94 KB |
