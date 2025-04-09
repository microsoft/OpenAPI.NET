```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3476)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.408
  [Host]   : .NET 8.0.15 (8.0.1525.16413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  ShortRun : .NET 8.0.15 (8.0.1525.16413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean         | Error         | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |-------------:|--------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |     450.5 μs |      59.26 μs |      3.25 μs |    58.5938 |    11.7188 |         - |    377.15 KB |
| PetStoreJson |     172.8 μs |     123.46 μs |      6.77 μs |    39.0625 |     7.8125 |         - |    239.29 KB |
| GHESYaml     | 943,452.7 μs | 137,685.49 μs |  7,547.01 μs | 66000.0000 | 21000.0000 | 3000.0000 | 389463.91 KB |
| GHESJson     | 468,401.8 μs | 300,711.80 μs | 16,483.03 μs | 41000.0000 | 15000.0000 | 3000.0000 | 250934.62 KB |
