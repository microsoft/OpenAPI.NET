```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3476)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 9.0.202
  [Host]   : .NET 8.0.14 (8.0.1425.11118), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  ShortRun : .NET 8.0.14 (8.0.1425.11118), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean           | Error        | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |---------------:|-------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |       448.7 μs |     326.6 μs |     17.90 μs |    58.5938 |    11.7188 |         - |    381.79 KB |
| PetStoreJson |       484.8 μs |     156.9 μs |      8.60 μs |    62.5000 |    15.6250 |         - |    389.28 KB |
| GHESYaml     | 1,008,349.6 μs | 565,392.0 μs | 30,991.04 μs | 66000.0000 | 23000.0000 | 4000.0000 |    382785 KB |
| GHESJson     | 1,039,447.0 μs | 267,501.0 μs | 14,662.63 μs | 67000.0000 | 23000.0000 | 4000.0000 | 389970.77 KB |
