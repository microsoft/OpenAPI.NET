```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3476)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.407
  [Host]   : .NET 8.0.14 (8.0.1425.11118), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  ShortRun : .NET 8.0.14 (8.0.1425.11118), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean           | Error          | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |---------------:|---------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |       482.0 μs |       237.7 μs |     13.03 μs |    66.4063 |    15.6250 |         - |    424.68 KB |
| PetStoreJson |       259.2 μs |       431.6 μs |     23.66 μs |    45.8984 |    10.7422 |         - |    286.81 KB |
| GHESYaml     | 1,146,422.3 μs | 1,225,300.5 μs | 67,162.85 μs | 77000.0000 | 26000.0000 | 5000.0000 |  446195.8 KB |
| GHESJson     |   655,219.8 μs |   747,448.5 μs | 40,970.17 μs | 50000.0000 | 18000.0000 | 3000.0000 | 307664.52 KB |
