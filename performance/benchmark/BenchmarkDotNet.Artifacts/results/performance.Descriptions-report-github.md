```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.3 LTS (Noble Numbat) (container)
Intel Xeon Platinum 8370C CPU 2.80GHz (Max: 3.39GHz), 1 CPU, 2 logical cores and 1 physical core
.NET SDK 8.0.418
  [Host]   : .NET 8.0.24 (8.0.24, 8.0.2426.7010), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.24 (8.0.24, 8.0.2426.7010), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean           | Error        | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |---------------:|-------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |       594.9 μs |   1,121.5 μs |     61.47 μs |    11.7188 |          - |         - |    361.24 KB |
| PetStoreJson |       329.4 μs |   1,905.6 μs |    104.45 μs |     7.8125 |     1.9531 |         - |     223.1 KB |
| GHESYaml     | 1,164,634.7 μs | 868,047.1 μs | 47,580.59 μs | 17000.0000 | 14000.0000 | 3000.0000 | 345072.18 KB |
| GHESJson     |   462,077.5 μs | 294,835.8 μs | 16,160.94 μs |  8000.0000 |  6000.0000 | 1000.0000 | 206591.14 KB |
