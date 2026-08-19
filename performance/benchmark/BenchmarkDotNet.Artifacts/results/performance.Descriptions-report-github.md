```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
INTEL XEON PLATINUM 8573C 3.39GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.400
  [Host]   : .NET 8.0.30 (8.0.30, 8.0.3026.36720), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.30 (8.0.30, 8.0.3026.36720), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean         | Error         | StdDev       | Gen0      | Gen1      | Gen2      | Allocated    |
|------------- |-------------:|--------------:|-------------:|----------:|----------:|----------:|-------------:|
| PetStoreYaml |     441.7 μs |     119.48 μs |      6.55 μs |    3.9063 |         - |         - |     327.8 KB |
| PetStoreJson |     167.1 μs |      44.75 μs |      2.45 μs |    1.9531 |         - |         - |    209.67 KB |
| GHESYaml     | 639,148.3 μs | 104,953.08 μs |  5,752.83 μs | 4000.0000 | 3000.0000 | 1000.0000 | 267097.41 KB |
| GHESJson     | 183,609.4 μs | 234,899.82 μs | 12,875.65 μs | 1000.0000 |         - |         - | 140442.72 KB |
| GHESNextYaml | 877,586.8 μs | 245,901.99 μs | 13,478.72 μs | 6000.0000 | 4000.0000 | 1000.0000 |  469032.8 KB |
| GHESNextJson | 521,883.6 μs | 100,709.23 μs |  5,520.21 μs | 5000.0000 | 4000.0000 | 1000.0000 |  344770.8 KB |
