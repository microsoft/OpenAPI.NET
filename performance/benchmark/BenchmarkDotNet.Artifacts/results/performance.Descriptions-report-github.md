```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9168/25H2/2025Update/HudsonValley2)
Snapdragon X 12-core X1E80100 3.40 GHz (Max: 3.42GHz), 1 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.303
  [Host]   : .NET 8.0.30 (8.0.30, 8.0.3026.36720), Arm64 RyuJIT armv8.0-a
  ShortRun : .NET 8.0.30 (8.0.30, 8.0.3026.36720), Arm64 RyuJIT armv8.0-a

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean         | Error        | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |-------------:|-------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |     335.7 μs |     239.3 μs |     13.12 μs |    78.1250 |     7.8125 |         - |    327.37 KB |
| PetStoreJson |     131.8 μs |     106.7 μs |      5.85 μs |    50.7813 |    11.7188 |         - |    209.53 KB |
| GHESYaml     | 497,859.7 μs | 200,217.0 μs | 10,974.57 μs | 47000.0000 | 14000.0000 | 3000.0000 | 267512.44 KB |
| GHESJson     | 278,562.0 μs | 234,097.6 μs | 12,831.68 μs | 23000.0000 | 11000.0000 | 2000.0000 | 140859.51 KB |
| GHESNextYaml | 710,430.7 μs | 664,647.3 μs | 36,431.56 μs | 84000.0000 | 15000.0000 | 3000.0000 | 469450.01 KB |
| GHESNextJson | 491,725.9 μs | 233,616.5 μs | 12,805.31 μs | 59000.0000 | 13000.0000 | 3000.0000 | 345193.66 KB |
