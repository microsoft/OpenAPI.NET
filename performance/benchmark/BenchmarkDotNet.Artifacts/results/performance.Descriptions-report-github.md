```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]   : .NET 8.0.30 (8.0.3026.36720), X64 RyuJIT AVX2
  ShortRun : .NET 8.0.30 (8.0.3026.36720), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean           | Error        | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |---------------:|-------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |       656.5 μs |     215.4 μs |     11.81 μs |    23.4375 |     3.9063 |         - |    438.94 KB |
| PetStoreJson |       749.0 μs |     150.6 μs |      8.25 μs |    25.3906 |     1.9531 |         - |    446.38 KB |
| GHESYaml     | 1,604,748.4 μs | 199,004.5 μs | 10,908.11 μs | 30000.0000 | 23000.0000 | 4000.0000 | 438632.27 KB |
| GHESJson     | 1,770,229.0 μs | 636,403.8 μs | 34,883.44 μs | 30000.0000 | 23000.0000 | 4000.0000 | 445808.88 KB |
