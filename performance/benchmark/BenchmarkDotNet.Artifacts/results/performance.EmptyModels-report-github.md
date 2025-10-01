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
| EmptyApiCallback            |   3.524 ns |   3.4381 ns |  0.1885 ns | 0.0051 |      - |      32 B |
| EmptyApiComponents          |   5.389 ns |   0.3327 ns |  0.0182 ns | 0.0166 |      - |     104 B |
| EmptyApiContact             |   5.001 ns |   1.0549 ns |  0.0578 ns | 0.0076 |      - |      48 B |
| EmptyApiDiscriminator       |   3.851 ns |   0.4329 ns |  0.0237 ns | 0.0076 |      - |      48 B |
| EmptyDocument               | 455.039 ns | 317.0640 ns | 17.3793 ns | 0.1822 | 0.0005 |    1144 B |
| EmptyApiEncoding            |   4.308 ns |   6.4158 ns |  0.3517 ns | 0.0089 |      - |      56 B |
| EmptyApiExample             |   4.830 ns |   7.7631 ns |  0.4255 ns | 0.0089 |      - |      56 B |
| EmptyApiExternalDocs        |   4.699 ns |  15.6593 ns |  0.8583 ns | 0.0064 |      - |      40 B |
| EmptyApiHeader              |   4.930 ns |   0.5490 ns |  0.0301 ns | 0.0127 |      - |      80 B |
| EmptyApiInfo                |   5.618 ns |  12.2402 ns |  0.6709 ns | 0.0127 |      - |      80 B |
| EmptyApiLicense             |   4.696 ns |   7.5172 ns |  0.4120 ns | 0.0076 |      - |      48 B |
| EmptyApiLink                |   3.960 ns |   4.3938 ns |  0.2408 ns | 0.0115 |      - |      72 B |
| EmptyApiMediaType           |   5.034 ns |  14.1757 ns |  0.7770 ns | 0.0102 |      - |      64 B |
| EmptyApiOAuthFlow           |   5.035 ns |   3.6306 ns |  0.1990 ns | 0.0102 |      - |      64 B |
| EmptyApiOAuthFlows          |   4.740 ns |  16.8177 ns |  0.9218 ns | 0.0089 |      - |      56 B |
| EmptyApiOperation           |  69.965 ns |  37.7463 ns |  2.0690 ns | 0.0598 |      - |     376 B |
| EmptyApiParameter           |   5.858 ns |   0.4876 ns |  0.0267 ns | 0.0153 |      - |      96 B |
| EmptyApiPathItem            |   6.403 ns |   3.0645 ns |  0.1680 ns | 0.0102 |      - |      64 B |
| EmptyApiPaths               |  64.103 ns |  34.9021 ns |  1.9131 ns | 0.0395 |      - |     248 B |
| EmptyApiRequestBody         |   4.769 ns |   5.1349 ns |  0.2815 ns | 0.0076 |      - |      48 B |
| EmptyApiResponse            |   6.539 ns |  17.8635 ns |  0.9792 ns | 0.0102 |      - |      64 B |
| EmptyApiResponses           |  73.277 ns | 228.2770 ns | 12.5126 ns | 0.0395 |      - |     248 B |
| EmptyApiSchema              |  17.984 ns |  29.4335 ns |  1.6133 ns | 0.0650 |      - |     408 B |
| EmptyApiSecurityRequirement |  11.393 ns |   7.8968 ns |  0.4329 ns | 0.0166 |      - |     104 B |
| EmptyApiSecurityScheme      |   6.162 ns |   4.5397 ns |  0.2488 ns | 0.0153 |      - |      96 B |
| EmptyApiServer              |   4.925 ns |   9.4233 ns |  0.5165 ns | 0.0089 |      - |      56 B |
| EmptyApiServerVariable      |   4.237 ns |   0.2664 ns |  0.0146 ns | 0.0076 |      - |      48 B |
| EmptyApiTag                 |   4.526 ns |   4.3139 ns |  0.2365 ns | 0.0076 |      - |      48 B |
