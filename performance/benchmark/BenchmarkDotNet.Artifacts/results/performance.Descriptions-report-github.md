```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3476)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.407
  [Host]   : .NET 8.0.14 (8.0.1425.11118), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  ShortRun : .NET 8.0.14 (8.0.1425.11118), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method   | Mean           | Error         | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|--------- |---------------:|--------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStore |       462.3 μs |      31.21 μs |      1.71 μs |    66.4063 |    15.6250 |         - |    424.68 KB |
| GHES     | 1,138,799.2 μs | 948,023.83 μs | 51,964.39 μs | 77000.0000 | 26000.0000 | 5000.0000 | 446195.81 KB |
