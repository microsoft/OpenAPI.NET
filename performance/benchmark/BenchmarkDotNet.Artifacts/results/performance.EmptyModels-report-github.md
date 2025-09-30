```

BenchmarkDotNet v0.15.4, Windows 11 (10.0.26200.6584)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.414
  [Host]   : .NET 8.0.20 (8.0.20, 8.0.2025.41914), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.20 (8.0.20, 8.0.2025.41914), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean       | Error       | StdDev     | Gen0   | Gen1   | Allocated |
|---------------------------- |-----------:|------------:|-----------:|-------:|-------:|----------:|
| EmptyApiCallback            |   3.396 ns |   1.7552 ns |  0.0962 ns | 0.0051 |      - |      32 B |
| EmptyApiComponents          |   4.727 ns |   0.7294 ns |  0.0400 ns | 0.0166 |      - |     104 B |
| EmptyApiContact             |   3.272 ns |   2.0487 ns |  0.1123 ns | 0.0076 |      - |      48 B |
| EmptyApiDiscriminator       |   4.100 ns |   7.0684 ns |  0.3874 ns | 0.0076 |      - |      48 B |
| EmptyDocument               | 501.551 ns | 376.7537 ns | 20.6511 ns | 0.1822 |      - |    1144 B |
| EmptyApiEncoding            |   4.488 ns |  13.8132 ns |  0.7571 ns | 0.0089 |      - |      56 B |
| EmptyApiExample             |   3.041 ns |   1.3738 ns |  0.0753 ns | 0.0089 |      - |      56 B |
| EmptyApiExternalDocs        |   4.714 ns |   8.8977 ns |  0.4877 ns | 0.0064 |      - |      40 B |
| EmptyApiHeader              |   3.737 ns |   2.0447 ns |  0.1121 ns | 0.0127 |      - |      80 B |
| EmptyApiInfo                |   4.398 ns |   6.2465 ns |  0.3424 ns | 0.0127 |      - |      80 B |
| EmptyApiLicense             |   3.502 ns |   3.5672 ns |  0.1955 ns | 0.0076 |      - |      48 B |
| EmptyApiLink                |   4.083 ns |   1.6907 ns |  0.0927 ns | 0.0115 |      - |      72 B |
| EmptyApiMediaType           |   3.810 ns |   4.7174 ns |  0.2586 ns | 0.0089 |      - |      56 B |
| EmptyApiOAuthFlow           |   3.782 ns |   2.7240 ns |  0.1493 ns | 0.0102 |      - |      64 B |
| EmptyApiOAuthFlows          |   3.668 ns |   2.0959 ns |  0.1149 ns | 0.0089 |      - |      56 B |
| EmptyApiOperation           |  52.984 ns |  23.7491 ns |  1.3018 ns | 0.0599 | 0.0001 |     376 B |
| EmptyApiParameter           |   4.942 ns |  10.4632 ns |  0.5735 ns | 0.0153 |      - |      96 B |
| EmptyApiPathItem            |   3.756 ns |   3.5941 ns |  0.1970 ns | 0.0102 |      - |      64 B |
| EmptyApiPaths               |  46.153 ns |  20.7899 ns |  1.1396 ns | 0.0395 |      - |     248 B |
| EmptyApiRequestBody         |   3.261 ns |   0.8516 ns |  0.0467 ns | 0.0076 |      - |      48 B |
| EmptyApiResponse            |   5.048 ns |  12.9863 ns |  0.7118 ns | 0.0102 |      - |      64 B |
| EmptyApiResponses           |  60.960 ns |  37.1342 ns |  2.0355 ns | 0.0395 |      - |     248 B |
| EmptyApiSchema              |  12.058 ns |   8.7459 ns |  0.4794 ns | 0.0650 |      - |     408 B |
| EmptyApiSecurityRequirement |   8.466 ns |   4.5188 ns |  0.2477 ns | 0.0166 |      - |     104 B |
| EmptyApiSecurityScheme      |   4.509 ns |   6.6819 ns |  0.3663 ns | 0.0153 |      - |      96 B |
| EmptyApiServer              |   3.402 ns |   0.8242 ns |  0.0452 ns | 0.0089 |      - |      56 B |
| EmptyApiServerVariable      |   3.164 ns |   1.0524 ns |  0.0577 ns | 0.0076 |      - |      48 B |
| EmptyApiTag                 |   3.230 ns |   0.6518 ns |  0.0357 ns | 0.0076 |      - |      48 B |
