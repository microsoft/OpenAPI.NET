```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
INTEL XEON PLATINUM 8573C 2.30GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]   : .NET 8.0.29 (8.0.29, 8.0.2926.32403), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.29 (8.0.29, 8.0.2926.32403), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean           | Error         | StdDev       | Gen0      | Gen1      | Gen2      | Allocated    |
|------------- |---------------:|--------------:|-------------:|----------:|----------:|----------:|-------------:|
| PetStoreYaml |       477.6 μs |     237.53 μs |     13.02 μs |    3.9063 |         - |         - |    375.67 KB |
| PetStoreJson |       193.3 μs |      29.03 μs |      1.59 μs |    1.9531 |         - |         - |    209.67 KB |
| GHESYaml     |   878,325.2 μs | 555,688.50 μs | 30,459.16 μs | 4000.0000 | 3000.0000 | 1000.0000 |  310814.2 KB |
| GHESJson     |   225,445.4 μs |  35,058.33 μs |  1,921.67 μs | 1000.0000 |         - |         - | 140426.65 KB |
| GHESNextYaml | 1,254,063.7 μs | 245,210.30 μs | 13,440.80 μs | 8000.0000 | 6000.0000 | 2000.0000 |  512928.9 KB |
| GHESNextJson |   588,121.5 μs |  83,680.85 μs |  4,586.83 μs | 5000.0000 | 4000.0000 | 1000.0000 |  344754.7 KB |
