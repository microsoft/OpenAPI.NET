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
| EmptyApiCallback            |   5.645 ns |   7.8741 ns |  0.4316 ns | 0.0051 |      - |      32 B |
| EmptyApiComponents          |   6.006 ns |   5.2597 ns |  0.2883 ns | 0.0166 |      - |     104 B |
| EmptyApiContact             |   4.089 ns |   0.5878 ns |  0.0322 ns | 0.0076 |      - |      48 B |
| EmptyApiDiscriminator       |   4.782 ns |   8.0499 ns |  0.4412 ns | 0.0076 |      - |      48 B |
| EmptyDocument               | 433.420 ns | 187.8471 ns | 10.2965 ns | 0.1822 | 0.0005 |    1144 B |
| EmptyApiEncoding            |   4.530 ns |   5.1398 ns |  0.2817 ns | 0.0089 |      - |      56 B |
| EmptyApiExample             |   7.305 ns |  43.4382 ns |  2.3810 ns | 0.0115 |      - |      72 B |
| EmptyApiExternalDocs        |   3.674 ns |   1.4908 ns |  0.0817 ns | 0.0064 |      - |      40 B |
| EmptyApiHeader              |   4.717 ns |   2.2016 ns |  0.1207 ns | 0.0127 |      - |      80 B |
| EmptyApiInfo                |   4.111 ns |   2.9213 ns |  0.1601 ns | 0.0127 |      - |      80 B |
| EmptyApiLicense             |   6.578 ns |  20.7582 ns |  1.1378 ns | 0.0076 |      - |      48 B |
| EmptyApiLink                |   4.809 ns |   7.0818 ns |  0.3882 ns | 0.0115 |      - |      72 B |
| EmptyApiMediaType           |   8.162 ns |  22.2763 ns |  1.2210 ns | 0.0127 |      - |      80 B |
| EmptyApiOAuthFlow           |   4.741 ns |   9.7498 ns |  0.5344 ns | 0.0102 |      - |      64 B |
| EmptyApiOAuthFlows          |   4.431 ns |   3.7652 ns |  0.2064 ns | 0.0102 |      - |      64 B |
| EmptyApiOperation           |  54.763 ns |  39.4861 ns |  2.1644 ns | 0.0599 | 0.0001 |     376 B |
| EmptyApiParameter           |   5.538 ns |   5.7246 ns |  0.3138 ns | 0.0153 |      - |      96 B |
| EmptyApiPathItem            |   4.709 ns |   4.4763 ns |  0.2454 ns | 0.0102 |      - |      64 B |
| EmptyApiPaths               |  45.287 ns |  13.3268 ns |  0.7305 ns | 0.0395 |      - |     248 B |
| EmptyApiRequestBody         |   7.107 ns |  30.9082 ns |  1.6942 ns | 0.0076 |      - |      48 B |
| EmptyApiResponse            |   4.630 ns |   4.9979 ns |  0.2740 ns | 0.0102 |      - |      64 B |
| EmptyApiResponses           |  51.217 ns |  91.3082 ns |  5.0049 ns | 0.0395 |      - |     248 B |
| EmptyApiSchema              |  13.945 ns |   4.5336 ns |  0.2485 ns | 0.0650 |      - |     408 B |
| EmptyApiSecurityRequirement |   8.539 ns |   2.4608 ns |  0.1349 ns | 0.0166 |      - |     104 B |
| EmptyApiSecurityScheme      |   5.019 ns |   5.0371 ns |  0.2761 ns | 0.0153 |      - |      96 B |
| EmptyApiServer              |   4.910 ns |   6.7044 ns |  0.3675 ns | 0.0089 |      - |      56 B |
| EmptyApiServerVariable      |   4.658 ns |   9.8425 ns |  0.5395 ns | 0.0076 |      - |      48 B |
| EmptyApiTag                 |   4.635 ns |   2.2431 ns |  0.1230 ns | 0.0115 |      - |      72 B |
