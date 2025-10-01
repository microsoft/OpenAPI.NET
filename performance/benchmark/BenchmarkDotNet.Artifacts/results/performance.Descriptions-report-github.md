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
| PetStoreYaml |     1,019.3 μs |       187.8 μs |     10.29 μs |    62.5000 |     7.8125 |         - |    387.49 KB |
| PetStoreJson |       237.6 μs |       548.4 μs |     30.06 μs |    40.0391 |     8.7891 |         - |    249.63 KB |
| GHESYaml     | 1,144,436.7 μs | 1,740,854.8 μs | 95,422.13 μs | 66000.0000 | 22000.0000 | 4000.0000 | 384520.48 KB |
| GHESJson     |   515,878.2 μs |    49,746.5 μs |  2,726.78 μs | 40000.0000 | 16000.0000 | 3000.0000 | 245991.74 KB |
