```

BenchmarkDotNet v0.15.4, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.414
  [Host]   : .NET 8.0.20 (8.0.20, 8.0.2025.41914), X64 RyuJIT x86-64-v3
  ShortRun : .NET 8.0.20 (8.0.20, 8.0.2025.41914), X64 RyuJIT x86-64-v3

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean           | Error         | StdDev      | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |---------------:|--------------:|------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |       513.8 μs |     220.40 μs |    12.08 μs |    23.4375 |     3.9063 |         - |    387.37 KB |
| PetStoreJson |       235.7 μs |      19.54 μs |     1.07 μs |    13.6719 |     1.9531 |         - |    249.22 KB |
| GHESYaml     | 1,008,778.7 μs |  50,002.60 μs | 2,740.81 μs | 26000.0000 | 20000.0000 | 3000.0000 | 384508.01 KB |
| GHESJson     |   469,189.2 μs | 144,923.46 μs | 7,943.74 μs | 16000.0000 |  9000.0000 | 2000.0000 |  245977.2 KB |
