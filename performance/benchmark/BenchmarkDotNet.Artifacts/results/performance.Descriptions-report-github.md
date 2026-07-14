```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]   : .NET 8.0.28 (8.0.28, 8.0.2826.26413), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.28 (8.0.28, 8.0.2826.26413), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean           | Error        | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |---------------:|-------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |       528.1 μs |     177.6 μs |      9.74 μs |    11.7188 |          - |         - |    368.25 KB |
| PetStoreJson |       244.1 μs |     258.7 μs |     14.18 μs |     7.8125 |          - |         - |    202.25 KB |
| GHESYaml     | 1,015,062.7 μs | 189,974.3 μs | 10,413.13 μs | 15000.0000 | 14000.0000 | 3000.0000 |  309638.7 KB |
| GHESJson     |   348,239.6 μs | 155,668.6 μs |  8,532.72 μs |  6000.0000 |  5000.0000 | 1000.0000 | 139242.43 KB |
| GHESNextYaml | 1,236,771.2 μs |  88,165.1 μs |  4,832.63 μs | 23000.0000 | 14000.0000 | 3000.0000 |  511139.5 KB |
| GHESNextJson |   613,831.6 μs | 120,635.4 μs |  6,612.43 μs | 15000.0000 |  8000.0000 | 2000.0000 | 342968.27 KB |
