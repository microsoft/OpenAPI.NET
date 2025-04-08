```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3476)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.407
  [Host]   : .NET 8.0.14 (8.0.1425.11118), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  ShortRun : .NET 8.0.14 (8.0.1425.11118), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean           | Error         | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |---------------:|--------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |       451.8 μs |      96.73 μs |      5.30 μs |    66.4063 |    15.6250 |         - |    424.68 KB |
| PetStoreJson |       192.1 μs |      24.97 μs |      1.37 μs |    45.8984 |    10.7422 |         - |    286.81 KB |
| GHESYaml     | 1,079,906.2 μs | 730,182.97 μs | 40,023.79 μs | 76000.0000 | 25000.0000 | 4000.0000 | 446194.88 KB |
| GHESJson     |   588,655.6 μs | 195,386.89 μs | 10,709.81 μs | 50000.0000 | 18000.0000 | 3000.0000 |  307664.5 KB |
