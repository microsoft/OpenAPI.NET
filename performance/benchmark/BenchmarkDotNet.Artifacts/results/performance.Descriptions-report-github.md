```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.300
  [Host]   : .NET 8.0.27 (8.0.27, 8.0.2726.22922), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.27 (8.0.27, 8.0.2726.22922), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean           | Error         | StdDev      | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |---------------:|--------------:|------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |       480.2 μs |      50.16 μs |     2.75 μs |    11.7188 |          - |         - |    363.98 KB |
| PetStoreJson |       215.4 μs |      27.68 μs |     1.52 μs |     8.7891 |     1.9531 |         - |    225.84 KB |
| GHESYaml     | 1,034,639.0 μs |  99,755.13 μs | 5,467.92 μs | 17000.0000 | 14000.0000 | 3000.0000 | 345962.32 KB |
| GHESJson     |   414,562.3 μs |  57,822.66 μs | 3,169.46 μs |  8000.0000 |  6000.0000 | 1000.0000 | 207483.96 KB |
| GHESNextYaml | 1,233,870.1 μs | 138,213.19 μs | 7,575.93 μs | 25000.0000 | 15000.0000 | 3000.0000 | 542594.95 KB |
| GHESNextJson |   649,688.9 μs |  72,965.05 μs | 3,999.46 μs | 16000.0000 |  8000.0000 | 1000.0000 | 407786.32 KB |
