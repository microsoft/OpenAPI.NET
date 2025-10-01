```

BenchmarkDotNet v0.15.4, Windows 11 (10.0.26200.6584)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.414
  [Host]   : .NET 8.0.20 (8.0.20, 8.0.2025.41914), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.20 (8.0.20, 8.0.2025.41914), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean       | Error       | StdDev    | Gen0   | Gen1   | Allocated |
|---------------------------- |-----------:|------------:|----------:|-------:|-------:|----------:|
| EmptyApiCallback            |   4.263 ns |   5.1657 ns | 0.2832 ns | 0.0051 |      - |      32 B |
| EmptyApiComponents          |   4.945 ns |   8.8646 ns | 0.4859 ns | 0.0166 |      - |     104 B |
| EmptyApiContact             |   3.131 ns |   1.0396 ns | 0.0570 ns | 0.0076 |      - |      48 B |
| EmptyApiDiscriminator       |   3.090 ns |   0.9009 ns | 0.0494 ns | 0.0076 |      - |      48 B |
| EmptyDocument               | 417.799 ns | 144.2693 ns | 7.9079 ns | 0.1822 | 0.0005 |    1144 B |
| EmptyApiEncoding            |   3.312 ns |   0.5217 ns | 0.0286 ns | 0.0089 |      - |      56 B |
| EmptyApiExample             |   3.431 ns |   0.8258 ns | 0.0453 ns | 0.0089 |      - |      56 B |
| EmptyApiExternalDocs        |   3.347 ns |   2.2879 ns | 0.1254 ns | 0.0064 |      - |      40 B |
| EmptyApiHeader              |   3.990 ns |   2.3010 ns | 0.1261 ns | 0.0127 |      - |      80 B |
| EmptyApiInfo                |   3.849 ns |   1.0172 ns | 0.0558 ns | 0.0127 |      - |      80 B |
| EmptyApiLicense             |   3.171 ns |   1.3064 ns | 0.0716 ns | 0.0076 |      - |      48 B |
| EmptyApiLink                |   3.867 ns |   4.9749 ns | 0.2727 ns | 0.0115 |      - |      72 B |
| EmptyApiMediaType           |   4.039 ns |   9.6571 ns | 0.5293 ns | 0.0089 |      - |      56 B |
| EmptyApiOAuthFlow           |   3.685 ns |   2.0429 ns | 0.1120 ns | 0.0102 |      - |      64 B |
| EmptyApiOAuthFlows          |   5.205 ns |  25.2772 ns | 1.3855 ns | 0.0102 |      - |      64 B |
| EmptyApiOperation           |  59.714 ns |  62.1008 ns | 3.4040 ns | 0.0598 |      - |     376 B |
| EmptyApiParameter           |   5.936 ns |  26.5846 ns | 1.4572 ns | 0.0153 |      - |      96 B |
| EmptyApiPathItem            |   4.574 ns |   6.6880 ns | 0.3666 ns | 0.0102 |      - |      64 B |
| EmptyApiPaths               |  53.614 ns |  17.9613 ns | 0.9845 ns | 0.0395 |      - |     248 B |
| EmptyApiRequestBody         |  11.345 ns |  30.1172 ns | 1.6508 ns | 0.0076 |      - |      48 B |
| EmptyApiResponse            |   4.667 ns |   5.8182 ns | 0.3189 ns | 0.0102 |      - |      64 B |
| EmptyApiResponses           | 145.557 ns | 107.4049 ns | 5.8872 ns | 0.0395 |      - |     248 B |
| EmptyApiSchema              |  41.699 ns |  45.7900 ns | 2.5099 ns | 0.0650 |      - |     408 B |
| EmptyApiSecurityRequirement |  30.686 ns |   2.9648 ns | 0.1625 ns | 0.0166 |      - |     104 B |
| EmptyApiSecurityScheme      |   4.313 ns |   8.6152 ns | 0.4722 ns | 0.0153 |      - |      96 B |
| EmptyApiServer              |   3.847 ns |   2.8613 ns | 0.1568 ns | 0.0089 |      - |      56 B |
| EmptyApiServerVariable      |   4.164 ns |   6.3423 ns | 0.3476 ns | 0.0076 |      - |      48 B |
| EmptyApiTag                 |   3.564 ns |   1.5630 ns | 0.0857 ns | 0.0076 |      - |      48 B |
