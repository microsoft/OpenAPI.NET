```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8457/25H2/2025Update/HudsonValley2)
Snapdragon X 12-core X1E80100 3.40 GHz (Max: 3.42GHz), 1 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.300
  [Host]   : .NET 8.0.27 (8.0.27, 8.0.2726.22922), Arm64 RyuJIT armv8.0-a
  ShortRun : .NET 8.0.27 (8.0.27, 8.0.2726.22922), Arm64 RyuJIT armv8.0-a

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean         | Error         | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |-------------:|--------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |     273.1 μs |     100.13 μs |      5.49 μs |    74.2188 |    15.6250 |         - |    305.46 KB |
| PetStoreJson |     107.3 μs |      73.59 μs |      4.03 μs |    41.0156 |     7.3242 |         - |     167.6 KB |
| GHESYaml     | 579,750.4 μs | 566,587.31 μs | 31,056.56 μs | 44000.0000 | 18000.0000 | 3000.0000 | 250062.54 KB |
| GHESJson     | 239,218.6 μs | 182,294.47 μs |  9,992.18 μs | 17000.0000 |  9000.0000 | 2000.0000 |    107234 KB |
| GHESNextYaml | 701,412.2 μs | 280,955.53 μs | 15,400.12 μs | 80000.0000 | 20000.0000 | 3000.0000 | 443588.54 KB |
| GHESNextJson | 388,845.2 μs | 160,463.67 μs |  8,795.56 μs | 51000.0000 | 11000.0000 | 2000.0000 | 305346.16 KB |
