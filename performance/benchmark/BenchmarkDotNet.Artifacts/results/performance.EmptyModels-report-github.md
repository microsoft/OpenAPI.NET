```

BenchmarkDotNet v0.15.4, Windows 11 (10.0.26200.6584)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.414
  [Host]   : .NET 8.0.20 (8.0.20, 8.0.2025.41914), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.20 (8.0.20, 8.0.2025.41914), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean       | Error         | StdDev      | Gen0   | Gen1   | Allocated |
|---------------------------- |-----------:|--------------:|------------:|-------:|-------:|----------:|
| EmptyApiCallback            |  10.878 ns |    19.9347 ns |   1.0927 ns | 0.0051 |      - |      32 B |
| EmptyApiComponents          |  22.201 ns |   103.1325 ns |   5.6530 ns | 0.0179 |      - |     112 B |
| EmptyApiContact             |   6.480 ns |    19.5579 ns |   1.0720 ns | 0.0076 |      - |      48 B |
| EmptyApiDiscriminator       |   3.720 ns |     7.4808 ns |   0.4100 ns | 0.0076 |      - |      48 B |
| EmptyDocument               | 609.429 ns | 1,856.4007 ns | 101.7556 ns | 0.1822 | 0.0005 |    1144 B |
| EmptyApiEncoding            |   8.411 ns |    31.0542 ns |   1.7022 ns | 0.0089 |      - |      56 B |
| EmptyApiExample             |   6.252 ns |     6.6823 ns |   0.3663 ns | 0.0115 |      - |      72 B |
| EmptyApiExternalDocs        |   4.488 ns |     7.6954 ns |   0.4218 ns | 0.0064 |      - |      40 B |
| EmptyApiHeader              |   5.854 ns |    24.3009 ns |   1.3320 ns | 0.0127 |      - |      80 B |
| EmptyApiInfo                |   8.092 ns |    40.7052 ns |   2.2312 ns | 0.0127 |      - |      80 B |
| EmptyApiLicense             |   4.119 ns |     4.6988 ns |   0.2576 ns | 0.0076 |      - |      48 B |
| EmptyApiLink                |   5.871 ns |    27.3828 ns |   1.5009 ns | 0.0115 |      - |      72 B |
| EmptyApiMediaType           |   6.413 ns |     7.4302 ns |   0.4073 ns | 0.0127 |      - |      80 B |
| EmptyApiOAuthFlow           |   5.674 ns |    13.8818 ns |   0.7609 ns | 0.0102 |      - |      64 B |
| EmptyApiOAuthFlows          |   5.290 ns |     7.3937 ns |   0.4053 ns | 0.0102 |      - |      64 B |
| EmptyApiOperation           |  65.442 ns |    21.1815 ns |   1.1610 ns | 0.0598 |      - |     376 B |
| EmptyApiParameter           |   5.995 ns |     3.5703 ns |   0.1957 ns | 0.0153 |      - |      96 B |
| EmptyApiPathItem            |   7.622 ns |    13.4673 ns |   0.7382 ns | 0.0102 |      - |      64 B |
| EmptyApiPaths               |  60.400 ns |    33.8603 ns |   1.8560 ns | 0.0395 |      - |     248 B |
| EmptyApiRequestBody         |   4.788 ns |     2.1215 ns |   0.1163 ns | 0.0076 |      - |      48 B |
| EmptyApiResponse            |   4.473 ns |     4.4310 ns |   0.2429 ns | 0.0102 |      - |      64 B |
| EmptyApiResponses           |  51.883 ns |    17.2049 ns |   0.9431 ns | 0.0395 |      - |     248 B |
| EmptyApiSchema              |  14.649 ns |    12.0213 ns |   0.6589 ns | 0.0650 |      - |     408 B |
| EmptyApiSecurityRequirement |  11.993 ns |    51.2809 ns |   2.8109 ns | 0.0166 |      - |     104 B |
| EmptyApiSecurityScheme      |  18.539 ns |    21.9011 ns |   1.2005 ns | 0.0153 |      - |      96 B |
| EmptyApiServer              |  11.425 ns |    73.5659 ns |   4.0324 ns | 0.0089 |      - |      56 B |
| EmptyApiServerVariable      |  16.196 ns |     0.6269 ns |   0.0344 ns | 0.0076 |      - |      48 B |
| EmptyApiTag                 |  19.659 ns |     6.4560 ns |   0.3539 ns | 0.0115 |      - |      72 B |
