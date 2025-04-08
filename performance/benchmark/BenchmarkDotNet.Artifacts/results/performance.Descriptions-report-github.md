```

BenchmarkDotNet v0.14.0, Ubuntu 22.04.5 LTS (Jammy Jellyfish) WSL
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.407
  [Host]   : .NET 8.0.14 (8.0.1425.11118), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  ShortRun : .NET 8.0.14 (8.0.1425.11118), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean           | Error          | StdDev        | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |---------------:|---------------:|--------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |       445.2 μs |       186.5 μs |      10.22 μs |    66.4063 |    15.6250 |         - |    424.68 KB |
| PetStoreJson |       228.3 μs |       372.2 μs |      20.40 μs |    45.8984 |    13.6719 |         - |    286.81 KB |
| GHESYaml     | 1,182,380.1 μs | 2,118,830.6 μs | 116,140.26 μs | 75000.0000 | 24000.0000 | 3000.0000 | 446194.44 KB |
| GHESJson     |   656,334.7 μs |   235,068.9 μs |  12,884.92 μs | 50000.0000 | 18000.0000 | 3000.0000 | 307664.51 KB |
