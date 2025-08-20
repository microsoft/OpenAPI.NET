```

BenchmarkDotNet v0.15.2, Linux Ubuntu 24.04.2 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.413
  [Host]   : .NET 8.0.19 (8.0.1925.36514), X64 RyuJIT AVX2
  ShortRun : .NET 8.0.19 (8.0.1925.36514), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean           | Error         | StdDev      | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |---------------:|--------------:|------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |       529.5 μs |      62.50 μs |     3.43 μs |    23.4375 |     3.9063 |         - |    387.26 KB |
| PetStoreJson |       240.8 μs |      15.69 μs |     0.86 μs |    13.6719 |     1.9531 |         - |     249.1 KB |
| GHESYaml     | 1,097,576.6 μs | 100,584.42 μs | 5,513.37 μs | 26000.0000 | 20000.0000 | 3000.0000 | 384492.38 KB |
| GHESJson     |   516,328.2 μs |  87,964.22 μs | 4,821.62 μs | 16000.0000 |  9000.0000 | 2000.0000 |  245957.5 KB |
