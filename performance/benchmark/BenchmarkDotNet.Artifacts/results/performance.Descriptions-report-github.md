```

BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.417
  [Host]   : .NET 8.0.23 (8.0.23, 8.0.2325.60607), X64 RyuJIT x86-64-v3
  ShortRun : .NET 8.0.23 (8.0.23, 8.0.2325.60607), X64 RyuJIT x86-64-v3

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean         | Error         | StdDev      | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |-------------:|--------------:|------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |     501.7 μs |     528.15 μs |    28.95 μs |    19.5313 |     3.9063 |         - |    360.93 KB |
| PetStoreJson |     210.6 μs |      17.50 μs |     0.96 μs |    12.6953 |     3.9063 |         - |    222.79 KB |
| GHESYaml     | 949,343.9 μs | 130,962.66 μs | 7,178.51 μs | 24000.0000 | 19000.0000 | 3000.0000 | 345282.95 KB |
| GHESJson     | 416,644.6 μs | 110,860.35 μs | 6,076.63 μs | 13000.0000 |  9000.0000 | 2000.0000 | 206802.12 KB |
