```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon 6973P-C, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.303
  [Host]   : .NET 8.0.30 (8.0.3026.36720), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  ShortRun : .NET 8.0.30 (8.0.3026.36720), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean           | Error         | StdDev       | Gen0      | Gen1      | Gen2      | Allocated    |
|------------- |---------------:|--------------:|-------------:|----------:|----------:|----------:|-------------:|
| PetStoreYaml |       450.4 μs |      70.34 μs |      3.86 μs |    3.9063 |         - |         - |    440.12 KB |
| PetStoreJson |       520.7 μs |      89.40 μs |      4.90 μs |    3.9063 |         - |         - |     447.6 KB |
| GHESYaml     | 1,265,574.2 μs | 396,012.10 μs | 21,706.76 μs | 6000.0000 | 5000.0000 | 1000.0000 | 438608.88 KB |
| GHESJson     | 1,329,832.7 μs | 375,414.26 μs | 20,577.72 μs | 6000.0000 | 5000.0000 | 1000.0000 | 445794.13 KB |
