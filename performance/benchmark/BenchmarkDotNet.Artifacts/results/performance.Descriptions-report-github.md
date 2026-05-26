```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.79GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.204
  [Host]   : .NET 8.0.27 (8.0.27, 8.0.2726.22922), X64 RyuJIT x86-64-v3
  ShortRun : .NET 8.0.27 (8.0.27, 8.0.2726.22922), X64 RyuJIT x86-64-v3

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean           | Error         | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |---------------:|--------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |       528.6 μs |     178.98 μs |      9.81 μs |    19.5313 |     3.9063 |         - |    363.54 KB |
| PetStoreJson |       232.1 μs |      12.90 μs |      0.71 μs |    13.6719 |     1.9531 |         - |    225.39 KB |
| GHESYaml     | 1,072,146.4 μs | 200,482.34 μs | 10,989.11 μs | 24000.0000 | 19000.0000 | 3000.0000 | 345905.05 KB |
| GHESJson     |   483,939.6 μs | 288,033.88 μs | 15,788.11 μs | 13000.0000 |  9000.0000 | 2000.0000 | 207426.91 KB |
| GHESNextYaml | 1,325,231.0 μs | 240,938.48 μs | 13,206.65 μs | 36000.0000 | 20000.0000 | 3000.0000 | 542041.88 KB |
| GHESNextJson |   699,920.8 μs |  85,692.01 μs |  4,697.07 μs | 25000.0000 | 11000.0000 | 2000.0000 | 407243.96 KB |
