```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3476)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.407
  [Host]     : .NET 8.0.14 (8.0.1425.11118), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  DefaultJob : .NET 8.0.14 (8.0.1425.11118), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI


```
| Method        | Mean        | Error      | StdDev      | Median      | Gen0   | Gen1   | Allocated |
|-------------- |------------:|-----------:|------------:|------------:|-------:|-------:|----------:|
| EmptyDocument | 601.6028 ns | 38.5381 ns | 109.3263 ns | 549.5873 ns | 0.2203 | 0.0010 |    1384 B |
| Scenario2     |   0.0045 ns |  0.0050 ns |   0.0044 ns |   0.0040 ns |      - |      - |         - |
