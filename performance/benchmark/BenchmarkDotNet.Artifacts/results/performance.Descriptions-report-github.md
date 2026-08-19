```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]   : .NET 8.0.30 (8.0.3026.36720), X64 RyuJIT AVX2
  ShortRun : .NET 8.0.30 (8.0.3026.36720), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean           | Error         | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |---------------:|--------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |       676.6 μs |      54.31 μs |      2.98 μs |    23.4375 |     3.9063 |         - |    438.94 KB |
| PetStoreJson |       784.0 μs |     176.52 μs |      9.68 μs |    25.3906 |     1.9531 |         - |    446.38 KB |
| GHESYaml     | 1,828,295.9 μs | 199,328.43 μs | 10,925.86 μs | 30000.0000 | 23000.0000 | 4000.0000 | 438627.23 KB |
| GHESJson     | 1,983,974.4 μs | 338,736.59 μs | 18,567.30 μs | 30000.0000 | 23000.0000 | 4000.0000 | 445808.38 KB |
