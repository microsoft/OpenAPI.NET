```

BenchmarkDotNet v0.15.4, Windows 11 (10.0.26200.6899)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.415
  [Host]   : .NET 8.0.21 (8.0.21, 8.0.2125.47513), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.21 (8.0.21, 8.0.2125.47513), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean       | Error       | StdDev     | Gen0   | Gen1   | Allocated |
|---------------------------- |-----------:|------------:|-----------:|-------:|-------:|----------:|
| EmptyApiCallback            |   4.123 ns |   6.9458 ns |  0.3807 ns | 0.0051 |      - |      32 B |
| EmptyApiComponents          |   5.984 ns |   8.2812 ns |  0.4539 ns | 0.0166 |      - |     104 B |
| EmptyApiContact             |   4.247 ns |   2.0411 ns |  0.1119 ns | 0.0076 |      - |      48 B |
| EmptyApiDiscriminator       |   3.737 ns |   3.2849 ns |  0.1801 ns | 0.0064 |      - |      40 B |
| EmptyDocument               | 378.175 ns |  28.4604 ns |  1.5600 ns | 0.1807 | 0.0005 |    1136 B |
| EmptyApiEncoding            |   3.957 ns |   0.4209 ns |  0.0231 ns | 0.0089 |      - |      56 B |
| EmptyApiExample             |   4.291 ns |  10.3278 ns |  0.5661 ns | 0.0089 |      - |      56 B |
| EmptyApiExternalDocs        |  13.830 ns |   3.4713 ns |  0.1903 ns | 0.0064 |      - |      40 B |
| EmptyApiHeader              |   5.588 ns |   0.2592 ns |  0.0142 ns | 0.0127 |      - |      80 B |
| EmptyApiInfo                |  20.057 ns |  15.6178 ns |  0.8561 ns | 0.0127 |      - |      80 B |
| EmptyApiLicense             |  15.371 ns |   2.4272 ns |  0.1330 ns | 0.0076 |      - |      48 B |
| EmptyApiLink                |  18.548 ns |   6.8904 ns |  0.3777 ns | 0.0115 |      - |      72 B |
| EmptyApiMediaType           |  12.316 ns |  75.4533 ns |  4.1359 ns | 0.0089 |      - |      56 B |
| EmptyApiOAuthFlow           |   7.727 ns |   3.5143 ns |  0.1926 ns | 0.0089 |      - |      56 B |
| EmptyApiOAuthFlows          |   4.888 ns |   5.8563 ns |  0.3210 ns | 0.0089 |      - |      56 B |
| EmptyApiOperation           |  52.321 ns |  22.6510 ns |  1.2416 ns | 0.0599 |      - |     376 B |
| EmptyApiParameter           |   5.761 ns |  10.3327 ns |  0.5664 ns | 0.0153 |      - |      96 B |
| EmptyApiPathItem            |   6.439 ns |   3.6625 ns |  0.2008 ns | 0.0102 |      - |      64 B |
| EmptyApiPaths               |  55.703 ns |  94.6718 ns |  5.1893 ns | 0.0395 |      - |     248 B |
| EmptyApiRequestBody         |  14.309 ns |  34.2430 ns |  1.8770 ns | 0.0076 |      - |      48 B |
| EmptyApiResponse            |   7.685 ns |  22.6218 ns |  1.2400 ns | 0.0089 |      - |      56 B |
| EmptyApiResponses           |  61.270 ns |  54.2391 ns |  2.9730 ns | 0.0395 |      - |     248 B |
| EmptyApiSchema              |  29.361 ns | 214.0627 ns | 11.7335 ns | 0.0650 |      - |     408 B |
| EmptyApiSecurityRequirement |  14.933 ns |  50.1256 ns |  2.7476 ns | 0.0166 |      - |     104 B |
| EmptyApiSecurityScheme      |  13.589 ns |  53.0533 ns |  2.9080 ns | 0.0140 |      - |      88 B |
| EmptyApiServer              |   5.909 ns |  37.7061 ns |  2.0668 ns | 0.0076 |      - |      48 B |
| EmptyApiServerVariable      |   6.311 ns |  29.6268 ns |  1.6239 ns | 0.0076 |      - |      48 B |
| EmptyApiTag                 |   6.111 ns |  35.5993 ns |  1.9513 ns | 0.0076 |      - |      48 B |
