```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9106/25H2/2025Update/HudsonValley2) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.303
  [Host]   : .NET 8.0.30 (8.0.30, 8.0.3026.36720), X64 RyuJIT x86-64-v3
  ShortRun : .NET 8.0.30 (8.0.30, 8.0.3026.36720), X64 RyuJIT x86-64-v3

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean       | Error      | StdDev     | Gen0   | Allocated |
|---------------------------- |-----------:|-----------:|-----------:|-------:|----------:|
| EmptyApiCallback            |   6.530 ns |  17.619 ns |  0.9658 ns | 0.0019 |      32 B |
| EmptyApiComponents          |  11.531 ns |  30.414 ns |  1.6671 ns | 0.0062 |     104 B |
| EmptyApiContact             |   6.755 ns |  20.237 ns |  1.1093 ns | 0.0029 |      48 B |
| EmptyApiDiscriminator       |   6.741 ns |  11.216 ns |  0.6148 ns | 0.0024 |      40 B |
| EmptyDocument               | 634.708 ns | 531.146 ns | 29.1139 ns | 0.0687 |    1152 B |
| EmptyApiEncoding            |   7.459 ns |   3.218 ns |  0.1764 ns | 0.0033 |      56 B |
| EmptyApiExample             |  11.100 ns |  18.163 ns |  0.9956 ns | 0.0033 |      56 B |
| EmptyApiExternalDocs        |   8.763 ns |  21.091 ns |  1.1561 ns | 0.0024 |      40 B |
| EmptyApiHeader              |   8.092 ns |   3.636 ns |  0.1993 ns | 0.0048 |      80 B |
| EmptyApiInfo                |   8.322 ns |   4.461 ns |  0.2445 ns | 0.0048 |      80 B |
| EmptyApiLicense             |   6.973 ns |   4.911 ns |  0.2692 ns | 0.0029 |      48 B |
| EmptyApiLink                |   8.480 ns |   5.549 ns |  0.3042 ns | 0.0043 |      72 B |
| EmptyApiMediaType           |   7.092 ns |   2.528 ns |  0.1386 ns | 0.0033 |      56 B |
| EmptyApiOAuthFlow           |   7.677 ns |   1.978 ns |  0.1084 ns | 0.0033 |      56 B |
| EmptyApiOAuthFlows          |   7.091 ns |   1.427 ns |  0.0782 ns | 0.0033 |      56 B |
| EmptyApiOperation           |  80.714 ns |  11.051 ns |  0.6057 ns | 0.0224 |     376 B |
| EmptyApiParameter           |   9.140 ns |  18.351 ns |  1.0059 ns | 0.0057 |      96 B |
| EmptyApiPathItem            |   9.527 ns |  39.170 ns |  2.1470 ns | 0.0038 |      64 B |
| EmptyApiPaths               | 103.358 ns |  85.122 ns |  4.6658 ns | 0.0148 |     248 B |
| EmptyApiRequestBody         |   7.907 ns |   4.309 ns |  0.2362 ns | 0.0029 |      48 B |
| EmptyApiResponse            |   8.404 ns |  13.667 ns |  0.7491 ns | 0.0033 |      56 B |
| EmptyApiResponses           | 112.791 ns |  87.315 ns |  4.7860 ns | 0.0148 |     248 B |
| EmptyApiSchema              |  36.972 ns |  70.509 ns |  3.8648 ns | 0.0306 |     512 B |
| EmptyApiSecurityRequirement |  21.192 ns |  41.436 ns |  2.2712 ns | 0.0062 |     104 B |
| EmptyApiSecurityScheme      |  13.446 ns |   6.500 ns |  0.3563 ns | 0.0052 |      88 B |
| EmptyApiServer              |   9.241 ns |   9.530 ns |  0.5224 ns | 0.0029 |      48 B |
| EmptyApiServerVariable      |   8.363 ns |  23.716 ns |  1.2999 ns | 0.0029 |      48 B |
| EmptyApiTag                 |   6.801 ns |   2.404 ns |  0.1318 ns | 0.0029 |      48 B |
