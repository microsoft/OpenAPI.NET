```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.417
  [Host]   : .NET 8.0.23 (8.0.23, 8.0.2325.60607), X64 RyuJIT x86-64-v3
  ShortRun : .NET 8.0.23 (8.0.23, 8.0.2325.60607), X64 RyuJIT x86-64-v3

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean         | Error      | StdDev     | Gen0   | Allocated |
|---------------------------- |-------------:|-----------:|-----------:|-------:|----------:|
| EmptyApiCallback            |    12.881 ns |   6.464 ns |  0.3543 ns | 0.0019 |      32 B |
| EmptyApiComponents          |    11.115 ns |   8.473 ns |  0.4644 ns | 0.0067 |     112 B |
| EmptyApiContact             |    15.612 ns |  43.722 ns |  2.3965 ns | 0.0029 |      48 B |
| EmptyApiDiscriminator       |    10.421 ns |   4.157 ns |  0.2278 ns | 0.0029 |      48 B |
| EmptyDocument               | 1,552.761 ns | 195.787 ns | 10.7317 ns | 0.0668 |    1144 B |
| EmptyApiEncoding            |    12.281 ns |  28.368 ns |  1.5550 ns | 0.0048 |      80 B |
| EmptyApiExample             |    12.103 ns |  17.069 ns |  0.9356 ns | 0.0043 |      72 B |
| EmptyApiExternalDocs        |    12.684 ns |  83.186 ns |  4.5597 ns | 0.0024 |      40 B |
| EmptyApiHeader              |    13.114 ns |  10.617 ns |  0.5819 ns | 0.0048 |      80 B |
| EmptyApiInfo                |    17.484 ns |  37.247 ns |  2.0416 ns | 0.0048 |      80 B |
| EmptyApiLicense             |     9.304 ns |   4.905 ns |  0.2689 ns | 0.0029 |      48 B |
| EmptyApiLink                |    13.260 ns |   7.497 ns |  0.4110 ns | 0.0043 |      72 B |
| EmptyApiMediaType           |    13.410 ns |  11.060 ns |  0.6063 ns | 0.0048 |      80 B |
| EmptyApiOAuthFlow           |    14.931 ns |  19.265 ns |  1.0560 ns | 0.0038 |      64 B |
| EmptyApiOAuthFlows          |    14.980 ns |  13.938 ns |  0.7640 ns | 0.0038 |      64 B |
| EmptyApiOperation           |    81.320 ns |  12.915 ns |  0.7079 ns | 0.0224 |     376 B |
| EmptyApiParameter           |    13.390 ns |  20.439 ns |  1.1204 ns | 0.0057 |      96 B |
| EmptyApiPathItem            |    12.827 ns |   3.747 ns |  0.2054 ns | 0.0038 |      64 B |
| EmptyApiPaths               |    64.713 ns |  18.742 ns |  1.0273 ns | 0.0148 |     248 B |
| EmptyApiRequestBody         |    14.251 ns |  13.790 ns |  0.7559 ns | 0.0029 |      48 B |
| EmptyApiResponse            |     8.811 ns |   3.772 ns |  0.2068 ns | 0.0038 |      64 B |
| EmptyApiResponses           |    63.846 ns |   7.887 ns |  0.4323 ns | 0.0148 |     248 B |
| EmptyApiSchema              |    26.385 ns |  49.316 ns |  2.7032 ns | 0.0249 |     416 B |
| EmptyApiSecurityRequirement |    17.544 ns |   2.492 ns |  0.1366 ns | 0.0062 |     104 B |
| EmptyApiSecurityScheme      |    10.306 ns |   9.709 ns |  0.5322 ns | 0.0057 |      96 B |
| EmptyApiServer              |    11.145 ns |  34.746 ns |  1.9045 ns | 0.0033 |      56 B |
| EmptyApiServerVariable      |    10.071 ns |  12.498 ns |  0.6850 ns | 0.0029 |      48 B |
| EmptyApiTag                 |    13.771 ns |  21.097 ns |  1.1564 ns | 0.0043 |      72 B |
