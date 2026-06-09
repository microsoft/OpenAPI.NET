```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8457/25H2/2025Update/HudsonValley2)
Snapdragon X 12-core X1E80100 3.40 GHz (Max: 3.42GHz), 1 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.300
  [Host]   : .NET 8.0.27 (8.0.27, 8.0.2726.22922), Arm64 RyuJIT armv8.0-a
  ShortRun : .NET 8.0.27 (8.0.27, 8.0.2726.22922), Arm64 RyuJIT armv8.0-a

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean           | Error         | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |---------------:|--------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |       371.7 μs |      35.71 μs |      1.96 μs |    74.2188 |    15.6250 |         - |    307.59 KB |
| PetStoreJson |       155.8 μs |      27.95 μs |      1.53 μs |    41.0156 |     2.9297 |         - |    169.74 KB |
| GHESYaml     |   820,515.0 μs | 271,578.81 μs | 14,886.15 μs | 45000.0000 | 18000.0000 | 3000.0000 | 253340.42 KB |
| GHESJson     |   302,067.9 μs | 133,906.46 μs |  7,339.86 μs | 18000.0000 | 10000.0000 | 2000.0000 | 110511.77 KB |
| GHESNextYaml | 1,023,253.0 μs | 242,683.77 μs | 13,302.32 μs | 80000.0000 | 19000.0000 | 3000.0000 | 447044.99 KB |
| GHESNextJson |   577,121.9 μs | 340,214.97 μs | 18,648.33 μs | 52000.0000 | 13000.0000 | 3000.0000 | 308806.54 KB |
