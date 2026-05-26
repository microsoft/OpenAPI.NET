```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.79GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.204
  [Host]   : .NET 8.0.27 (8.0.27, 8.0.2726.22922), X64 RyuJIT x86-64-v3
  ShortRun : .NET 8.0.27 (8.0.27, 8.0.2726.22922), X64 RyuJIT x86-64-v3

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean         | Error       | StdDev     | Gen0   | Allocated |
|---------------------------- |-------------:|------------:|-----------:|-------:|----------:|
| EmptyApiCallback            |    10.557 ns |  15.5877 ns |  0.8544 ns | 0.0019 |      32 B |
| EmptyApiComponents          |    12.597 ns |   3.3570 ns |  0.1840 ns | 0.0062 |     104 B |
| EmptyApiContact             |    11.809 ns |  17.5456 ns |  0.9617 ns | 0.0029 |      48 B |
| EmptyApiDiscriminator       |     7.162 ns |   2.4094 ns |  0.1321 ns | 0.0024 |      40 B |
| EmptyDocument               | 1,263.504 ns | 264.1995 ns | 14.4817 ns | 0.0668 |    1136 B |
| EmptyApiEncoding            |    14.393 ns |  29.8567 ns |  1.6365 ns | 0.0033 |      56 B |
| EmptyApiExample             |    10.489 ns |   5.1373 ns |  0.2816 ns | 0.0033 |      56 B |
| EmptyApiExternalDocs        |     9.589 ns |   6.2059 ns |  0.3402 ns | 0.0024 |      40 B |
| EmptyApiHeader              |    17.724 ns |  28.2521 ns |  1.5486 ns | 0.0048 |      80 B |
| EmptyApiInfo                |     9.764 ns |   3.6053 ns |  0.1976 ns | 0.0048 |      80 B |
| EmptyApiLicense             |    12.353 ns |   9.5358 ns |  0.5227 ns | 0.0029 |      48 B |
| EmptyApiLink                |    13.246 ns |  17.5129 ns |  0.9599 ns | 0.0043 |      72 B |
| EmptyApiMediaType           |    14.324 ns |  68.8912 ns |  3.7762 ns | 0.0033 |      56 B |
| EmptyApiOAuthFlow           |    14.195 ns |  22.8188 ns |  1.2508 ns | 0.0033 |      56 B |
| EmptyApiOAuthFlows          |     9.845 ns |  10.0785 ns |  0.5524 ns | 0.0033 |      56 B |
| EmptyApiOperation           |    82.454 ns |  50.2855 ns |  2.7563 ns | 0.0224 |     376 B |
| EmptyApiParameter           |     9.103 ns |   4.0581 ns |  0.2224 ns | 0.0057 |      96 B |
| EmptyApiPathItem            |     9.306 ns |  10.7745 ns |  0.5906 ns | 0.0038 |      64 B |
| EmptyApiPaths               |    65.196 ns |  11.3144 ns |  0.6202 ns | 0.0148 |     248 B |
| EmptyApiRequestBody         |    14.324 ns |  32.1634 ns |  1.7630 ns | 0.0029 |      48 B |
| EmptyApiResponse            |     9.803 ns |  10.2912 ns |  0.5641 ns | 0.0033 |      56 B |
| EmptyApiResponses           |    64.377 ns |  26.6215 ns |  1.4592 ns | 0.0148 |     248 B |
| EmptyApiSchema              |    22.927 ns |  48.8483 ns |  2.6775 ns | 0.0249 |     416 B |
| EmptyApiSecurityRequirement |    17.021 ns |   8.9823 ns |  0.4924 ns | 0.0062 |     104 B |
| EmptyApiSecurityScheme      |    10.870 ns |  21.6150 ns |  1.1848 ns | 0.0052 |      88 B |
| EmptyApiServer              |    14.609 ns |  18.2451 ns |  1.0001 ns | 0.0029 |      48 B |
| EmptyApiServerVariable      |     7.938 ns |   0.3680 ns |  0.0202 ns | 0.0029 |      48 B |
| EmptyApiTag                 |     9.000 ns |   1.8078 ns |  0.0991 ns | 0.0029 |      48 B |
