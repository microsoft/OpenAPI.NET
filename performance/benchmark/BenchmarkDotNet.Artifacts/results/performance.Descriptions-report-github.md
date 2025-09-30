```

BenchmarkDotNet v0.15.4, Windows 11 (10.0.26200.6584)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.414
  [Host]   : .NET 8.0.20 (8.0.20, 8.0.2025.41914), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.20 (8.0.20, 8.0.2025.41914), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean           | Error           | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |---------------:|----------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |       658.1 μs |     3,203.12 μs |    175.57 μs |    62.5000 |    11.7188 |         - |    387.37 KB |
| PetStoreJson |       234.4 μs |        95.66 μs |      5.24 μs |    39.0625 |     7.8125 |         - |    249.52 KB |
| GHESYaml     | 1,091,206.2 μs | 1,279,076.97 μs | 70,110.52 μs | 66000.0000 | 22000.0000 | 4000.0000 | 384511.66 KB |
| GHESJson     |   529,296.1 μs |   792,456.10 μs | 43,437.19 μs | 40000.0000 | 16000.0000 | 3000.0000 | 245982.38 KB |
