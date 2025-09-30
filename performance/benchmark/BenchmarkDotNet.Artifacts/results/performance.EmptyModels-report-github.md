```

BenchmarkDotNet v0.15.4, Windows 11 (10.0.26200.6584)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.414
  [Host]   : .NET 8.0.20 (8.0.20, 8.0.2025.41914), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.20 (8.0.20, 8.0.2025.41914), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean       | Error        | StdDev      | Gen0   | Gen1   | Allocated |
|---------------------------- |-----------:|-------------:|------------:|-------:|-------:|----------:|
| EmptyApiCallback            |   9.710 ns |    22.445 ns |   1.2303 ns | 0.0051 |      - |      32 B |
| EmptyApiComponents          |  33.694 ns |     9.447 ns |   0.5178 ns | 0.0166 |      - |     104 B |
| EmptyApiContact             |   3.538 ns |    13.416 ns |   0.7354 ns | 0.0076 |      - |      48 B |
| EmptyApiDiscriminator       |   5.351 ns |    13.152 ns |   0.7209 ns | 0.0076 |      - |      48 B |
| EmptyDocument               | 989.854 ns | 7,871.537 ns | 431.4655 ns | 0.1807 | 0.0005 |    1136 B |
| EmptyApiEncoding            |  20.098 ns |     5.012 ns |   0.2747 ns | 0.0089 |      - |      56 B |
| EmptyApiExample             |  10.358 ns |    23.866 ns |   1.3082 ns | 0.0089 |      - |      56 B |
| EmptyApiExternalDocs        |   9.184 ns |    24.199 ns |   1.3264 ns | 0.0064 |      - |      40 B |
| EmptyApiHeader              |  10.480 ns |     9.031 ns |   0.4950 ns | 0.0127 |      - |      80 B |
| EmptyApiInfo                |   5.795 ns |    19.555 ns |   1.0719 ns | 0.0127 |      - |      80 B |
| EmptyApiLicense             |   5.994 ns |    30.220 ns |   1.6565 ns | 0.0076 |      - |      48 B |
| EmptyApiLink                |   6.139 ns |    23.835 ns |   1.3065 ns | 0.0115 |      - |      72 B |
| EmptyApiMediaType           |   4.752 ns |     3.409 ns |   0.1869 ns | 0.0089 |      - |      56 B |
| EmptyApiOAuthFlow           |   5.794 ns |    37.420 ns |   2.0511 ns | 0.0089 |      - |      56 B |
| EmptyApiOAuthFlows          |   4.567 ns |     5.439 ns |   0.2982 ns | 0.0089 |      - |      56 B |
| EmptyApiOperation           |  73.728 ns |   216.735 ns |  11.8800 ns | 0.0598 |      - |     376 B |
| EmptyApiParameter           |   6.209 ns |    18.846 ns |   1.0330 ns | 0.0153 |      - |      96 B |
| EmptyApiPathItem            |   5.201 ns |     3.199 ns |   0.1753 ns | 0.0102 |      - |      64 B |
| EmptyApiPaths               |  71.769 ns |    58.754 ns |   3.2205 ns | 0.0395 |      - |     248 B |
| EmptyApiRequestBody         |   3.880 ns |     7.973 ns |   0.4370 ns | 0.0076 |      - |      48 B |
| EmptyApiResponse            |   4.554 ns |     5.639 ns |   0.3091 ns | 0.0102 |      - |      64 B |
| EmptyApiResponses           |  70.786 ns |   204.827 ns |  11.2273 ns | 0.0395 |      - |     248 B |
| EmptyApiSchema              |  16.257 ns |    13.982 ns |   0.7664 ns | 0.0650 |      - |     408 B |
| EmptyApiSecurityRequirement |  12.712 ns |    34.240 ns |   1.8768 ns | 0.0166 |      - |     104 B |
| EmptyApiSecurityScheme      |   5.492 ns |     7.509 ns |   0.4116 ns | 0.0140 |      - |      88 B |
| EmptyApiServer              |   3.386 ns |     1.694 ns |   0.0928 ns | 0.0089 |      - |      56 B |
| EmptyApiServerVariable      |   3.657 ns |     7.319 ns |   0.4012 ns | 0.0076 |      - |      48 B |
| EmptyApiTag                 |   3.887 ns |     3.857 ns |   0.2114 ns | 0.0076 |      - |      48 B |
