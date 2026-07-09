```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8655/25H2/2025Update/HudsonValley2)
Snapdragon X 12-core X1E80100 3.40 GHz (Max: 3.42GHz), 1 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.301
  [Host]   : .NET 8.0.28 (8.0.28, 8.0.2826.26413), Arm64 RyuJIT armv8.0-a
  ShortRun : .NET 8.0.28 (8.0.28, 8.0.2826.26413), Arm64 RyuJIT armv8.0-a

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean         | Error         | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |-------------:|--------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |     232.0 μs |      56.09 μs |      3.07 μs |    76.1719 |     9.7656 |         - |    314.13 KB |
| PetStoreJson |     117.3 μs |      10.56 μs |      0.58 μs |    42.9688 |     7.3242 |         - |    176.27 KB |
| GHESYaml     | 621,674.1 μs | 152,892.95 μs |  8,380.58 μs | 45000.0000 | 19000.0000 | 3000.0000 | 256630.98 KB |
| GHESJson     | 264,154.1 μs | 490,035.63 μs | 26,860.51 μs | 18000.0000 | 10000.0000 | 2000.0000 | 113802.94 KB |
| GHESNextYaml | 777,958.3 μs | 269,413.33 μs | 14,767.45 μs | 80000.0000 | 20000.0000 | 3000.0000 | 452706.23 KB |
| GHESNextJson | 432,900.8 μs | 561,774.69 μs | 30,792.77 μs | 54000.0000 | 14000.0000 | 3000.0000 | 314472.13 KB |
