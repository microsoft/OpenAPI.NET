```

BenchmarkDotNet v0.15.4, Windows 11 (10.0.26200.6584)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.414
  [Host]   : .NET 8.0.20 (8.0.20, 8.0.2025.41914), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.20 (8.0.20, 8.0.2025.41914), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method       | Mean           | Error          | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |---------------:|---------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |       979.3 μs |       375.3 μs |     20.57 μs |    62.5000 |     7.8125 |         - |    387.37 KB |
| PetStoreJson |       508.5 μs |       427.5 μs |     23.43 μs |    39.0625 |     7.8125 |         - |    249.51 KB |
| GHESYaml     | 1,587,861.1 μs | 1,076,577.3 μs | 59,010.84 μs | 66000.0000 | 22000.0000 | 4000.0000 | 384511.51 KB |
| GHESJson     |   599,442.5 μs |   843,317.6 μs | 46,225.08 μs | 40000.0000 | 16000.0000 | 3000.0000 | 245982.08 KB |
