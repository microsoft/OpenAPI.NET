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
| EmptyApiCallback            |   4.294 ns |   1.9361 ns |  0.1061 ns | 0.0051 |      - |      32 B |
| EmptyApiComponents          |   4.775 ns |   4.8246 ns |  0.2645 ns | 0.0166 |      - |     104 B |
| EmptyApiContact             |   3.904 ns |   2.1044 ns |  0.1153 ns | 0.0076 |      - |      48 B |
| EmptyApiDiscriminator       |   4.255 ns |   4.3111 ns |  0.2363 ns | 0.0076 |      - |      48 B |
| EmptyDocument               | 391.727 ns | 188.6621 ns | 10.3412 ns | 0.1822 | 0.0005 |    1144 B |
| EmptyApiEncoding            |   3.463 ns |   3.3919 ns |  0.1859 ns | 0.0089 |      - |      56 B |
| EmptyApiExample             |   5.624 ns |  15.7475 ns |  0.8632 ns | 0.0115 |      - |      72 B |
| EmptyApiExternalDocs        |   6.269 ns |  28.9202 ns |  1.5852 ns | 0.0064 |      - |      40 B |
| EmptyApiHeader              |   4.063 ns |  13.3302 ns |  0.7307 ns | 0.0127 |      - |      80 B |
| EmptyApiInfo                |   3.998 ns |   2.2530 ns |  0.1235 ns | 0.0127 |      - |      80 B |
| EmptyApiLicense             |   3.751 ns |   8.2254 ns |  0.4509 ns | 0.0076 |      - |      48 B |
| EmptyApiLink                |   5.253 ns |  36.8053 ns |  2.0174 ns | 0.0115 |      - |      72 B |
| EmptyApiMediaType           |   3.085 ns |   9.6165 ns |  0.5271 ns | 0.0089 |      - |      56 B |
| EmptyApiOAuthFlow           |   3.887 ns |   7.4877 ns |  0.4104 ns | 0.0102 |      - |      64 B |
| EmptyApiOAuthFlows          |   3.904 ns |   8.2115 ns |  0.4501 ns | 0.0102 |      - |      64 B |
| EmptyApiOperation           |  57.296 ns |  68.6982 ns |  3.7656 ns | 0.0598 |      - |     376 B |
| EmptyApiParameter           |   5.075 ns |   0.7385 ns |  0.0405 ns | 0.0153 |      - |      96 B |
| EmptyApiPathItem            |   4.123 ns |   7.1330 ns |  0.3910 ns | 0.0102 |      - |      64 B |
| EmptyApiPaths               |  52.181 ns |  31.9033 ns |  1.7487 ns | 0.0395 |      - |     248 B |
| EmptyApiRequestBody         |   3.900 ns |   8.9583 ns |  0.4910 ns | 0.0076 |      - |      48 B |
| EmptyApiResponse            |   3.322 ns |   5.0137 ns |  0.2748 ns | 0.0102 |      - |      64 B |
| EmptyApiResponses           |  52.908 ns |  67.4863 ns |  3.6992 ns | 0.0395 |      - |     248 B |
| EmptyApiSchema              |  13.516 ns |  28.8353 ns |  1.5806 ns | 0.0650 |      - |     408 B |
| EmptyApiSecurityRequirement |  10.592 ns |  11.2899 ns |  0.6188 ns | 0.0166 |      - |     104 B |
| EmptyApiSecurityScheme      |   5.906 ns |  17.7076 ns |  0.9706 ns | 0.0153 |      - |      96 B |
| EmptyApiServer              |   4.192 ns |  11.7503 ns |  0.6441 ns | 0.0089 |      - |      56 B |
| EmptyApiServerVariable      |   4.884 ns |  10.7917 ns |  0.5915 ns | 0.0076 |      - |      48 B |
| EmptyApiTag                 |   4.237 ns |  12.8794 ns |  0.7060 ns | 0.0076 |      - |      48 B |
