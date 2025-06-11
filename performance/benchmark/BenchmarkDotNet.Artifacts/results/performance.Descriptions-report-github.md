```

BenchmarkDotNet v0.15.1, Windows 11 (10.0.26100.4270/24H2/2024Update/HudsonValley)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.411
  [Host]   : .NET 8.0.17 (8.0.1725.26602), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  ShortRun : .NET 8.0.17 (8.0.1725.26602), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean         | Error         | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |-------------:|--------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |     447.1 μs |      43.25 μs |      2.37 μs |    66.4063 |    15.6250 |         - |    429.16 KB |
| PetStoreJson |     240.7 μs |     476.72 μs |     26.13 μs |    40.0391 |     8.7891 |         - |    249.35 KB |
| GHESYaml     | 959,693.0 μs | 430,630.36 μs | 23,604.30 μs | 75000.0000 | 24000.0000 | 4000.0000 |  438325.2 KB |
| GHESJson     | 545,959.8 μs | 771,081.18 μs | 42,265.56 μs | 43000.0000 | 16000.0000 | 3000.0000 | 261563.62 KB |
