```

BenchmarkDotNet v0.15.8, macOS Tahoe 26.3 (25D125) [Darwin 25.3.0]
Apple M1 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK 8.0.418
  [Host]   : .NET 8.0.24 (8.0.24, 8.0.2426.7010), Arm64 RyuJIT armv8.0-a
  ShortRun : .NET 8.0.24 (8.0.24, 8.0.2426.7010), Arm64 RyuJIT armv8.0-a

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean         | Error         | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |-------------:|--------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |     305.2 μs |     102.37 μs |      5.61 μs |    58.5938 |    11.7188 |         - |    361.39 KB |
| PetStoreJson |     136.3 μs |      20.80 μs |      1.14 μs |    36.1328 |     7.8125 |         - |    223.26 KB |
| GHESYaml     | 784,491.3 μs | 271,693.72 μs | 14,892.45 μs | 63000.0000 | 21000.0000 | 8000.0000 | 345349.48 KB |
| GHESJson     | 368,975.3 μs |  38,641.31 μs |  2,118.06 μs | 36000.0000 | 14000.0000 | 5000.0000 | 223280.98 KB |
