```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26200.9106) (Hyper-V)
AMD EPYC 7763, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.303
  [Host]   : .NET 8.0.30 (8.0.3026.36720), X64 RyuJIT AVX2
  ShortRun : .NET 8.0.30 (8.0.3026.36720), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean           | Error          | StdDev        | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |---------------:|---------------:|--------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |       941.6 μs |     1,888.8 μs |     103.53 μs |    23.4375 |     7.8125 |         - |    440.21 KB |
| PetStoreJson |       984.8 μs |       281.3 μs |      15.42 μs |    25.3906 |     1.9531 |         - |    446.38 KB |
| GHESYaml     | 2,171,893.7 μs |   478,607.6 μs |  26,234.09 μs | 30000.0000 | 23000.0000 | 4000.0000 |    438630 KB |
| GHESJson     | 3,011,015.9 μs | 7,499,811.5 μs | 411,089.98 μs | 30000.0000 | 23000.0000 | 4000.0000 | 445811.67 KB |
