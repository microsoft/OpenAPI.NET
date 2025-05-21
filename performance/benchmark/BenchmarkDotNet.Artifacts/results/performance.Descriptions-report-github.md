```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3981)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.409
  [Host]   : .NET 8.0.16 (8.0.1625.21506), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  ShortRun : .NET 8.0.16 (8.0.1625.21506), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean         | Error         | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |-------------:|--------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |     470.3 μs |     138.05 μs |      7.57 μs |    58.5938 |    11.7188 |         - |    380.53 KB |
| PetStoreJson |     166.0 μs |      43.84 μs |      2.40 μs |    39.0625 |     8.7891 |         - |    242.67 KB |
| GHESYaml     | 915,406.4 μs | 714,492.62 μs | 39,163.75 μs | 68000.0000 | 22000.0000 | 4000.0000 | 395800.98 KB |
| GHESJson     | 470,609.4 μs | 264,698.88 μs | 14,509.04 μs | 42000.0000 | 15000.0000 | 3000.0000 | 257270.45 KB |
