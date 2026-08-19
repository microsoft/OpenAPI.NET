```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.303
  [Host]   : .NET 8.0.30 (8.0.3026.36720), X64 RyuJIT AVX2
  ShortRun : .NET 8.0.30 (8.0.3026.36720), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean           | Error        | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |---------------:|-------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |       663.6 μs |     146.8 μs |      8.05 μs |    23.4375 |     3.9063 |         - |    438.94 KB |
| PetStoreJson |       832.4 μs |     607.9 μs |     33.32 μs |    23.4375 |     7.8125 |         - |    447.69 KB |
| GHESYaml     | 1,621,961.7 μs | 451,664.6 μs | 24,757.26 μs | 30000.0000 | 23000.0000 | 4000.0000 | 438632.07 KB |
| GHESJson     | 1,809,092.3 μs | 187,494.5 μs | 10,277.20 μs | 30000.0000 | 23000.0000 | 4000.0000 | 445806.78 KB |
