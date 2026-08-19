```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.303
  [Host]   : .NET 8.0.30 (8.0.3026.36720), X64 RyuJIT AVX2
  ShortRun : .NET 8.0.30 (8.0.3026.36720), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean           | Error        | StdDev      | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |---------------:|-------------:|------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |       734.2 μs |     197.4 μs |    10.82 μs |    23.4375 |     7.8125 |         - |    440.21 KB |
| PetStoreJson |       836.3 μs |     297.6 μs |    16.31 μs |    23.4375 |     7.8125 |         - |    447.69 KB |
| GHESYaml     | 1,555,741.1 μs | 175,160.8 μs | 9,601.16 μs | 30000.0000 | 23000.0000 | 4000.0000 | 438630.55 KB |
| GHESJson     | 1,727,668.4 μs | 164,898.3 μs | 9,038.63 μs | 30000.0000 | 23000.0000 | 4000.0000 | 445814.75 KB |
