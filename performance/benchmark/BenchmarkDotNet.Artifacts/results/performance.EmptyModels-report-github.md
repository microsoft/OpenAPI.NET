```

BenchmarkDotNet v0.15.4, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.414
  [Host]   : .NET 8.0.20 (8.0.20, 8.0.2025.41914), X64 RyuJIT x86-64-v3
  ShortRun : .NET 8.0.20 (8.0.20, 8.0.2025.41914), X64 RyuJIT x86-64-v3

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean       | Error       | StdDev     | Gen0   | Gen1   | Allocated |
|---------------------------- |-----------:|------------:|-----------:|-------:|-------:|----------:|
| EmptyApiCallback            |   3.424 ns |   2.0258 ns |  0.1110 ns | 0.0051 |      - |      32 B |
| EmptyApiComponents          |   4.780 ns |   4.3307 ns |  0.2374 ns | 0.0166 |      - |     104 B |
| EmptyApiContact             |   3.217 ns |   0.4872 ns |  0.0267 ns | 0.0076 |      - |      48 B |
| EmptyApiDiscriminator       |   3.313 ns |   0.8027 ns |  0.0440 ns | 0.0076 |      - |      48 B |
| EmptyDocument               | 388.528 ns | 238.6344 ns | 13.0804 ns | 0.1822 | 0.0005 |    1144 B |
| EmptyApiEncoding            |   3.468 ns |   0.4827 ns |  0.0265 ns | 0.0089 |      - |      56 B |
| EmptyApiExample             |   3.405 ns |   0.5600 ns |  0.0307 ns | 0.0089 |      - |      56 B |
| EmptyApiExternalDocs        |   3.073 ns |   1.5510 ns |  0.0850 ns | 0.0064 |      - |      40 B |
| EmptyApiHeader              |   3.993 ns |   1.8744 ns |  0.1027 ns | 0.0127 |      - |      80 B |
| EmptyApiInfo                |   4.089 ns |   1.8072 ns |  0.0991 ns | 0.0127 |      - |      80 B |
| EmptyApiLicense             |   3.300 ns |   1.3198 ns |  0.0723 ns | 0.0076 |      - |      48 B |
| EmptyApiLink                |   3.811 ns |   1.1633 ns |  0.0638 ns | 0.0115 |      - |      72 B |
| EmptyApiMediaType           |   3.400 ns |   0.5302 ns |  0.0291 ns | 0.0089 |      - |      56 B |
| EmptyApiOAuthFlow           |   3.413 ns |   1.5178 ns |  0.0832 ns | 0.0089 |      - |      56 B |
| EmptyApiOAuthFlows          |   4.672 ns |  23.4637 ns |  1.2861 ns | 0.0089 |      - |      56 B |
| EmptyApiOperation           |  53.248 ns |  35.8334 ns |  1.9641 ns | 0.0598 |      - |     376 B |
| EmptyApiParameter           |   5.174 ns |  13.7535 ns |  0.7539 ns | 0.0153 |      - |      96 B |
| EmptyApiPathItem            |   4.257 ns |   1.9101 ns |  0.1047 ns | 0.0102 |      - |      64 B |
| EmptyApiPaths               |  66.786 ns |  44.3418 ns |  2.4305 ns | 0.0395 |      - |     248 B |
| EmptyApiRequestBody         |   3.557 ns |   0.2645 ns |  0.0145 ns | 0.0076 |      - |      48 B |
| EmptyApiResponse            |   3.660 ns |   1.3775 ns |  0.0755 ns | 0.0102 |      - |      64 B |
| EmptyApiResponses           |  51.979 ns |   9.5132 ns |  0.5215 ns | 0.0395 |      - |     248 B |
| EmptyApiSchema              |  12.130 ns |   2.8643 ns |  0.1570 ns | 0.0650 |      - |     408 B |
| EmptyApiSecurityRequirement |   8.530 ns |   1.8944 ns |  0.1038 ns | 0.0166 |      - |     104 B |
| EmptyApiSecurityScheme      |   4.516 ns |   0.5682 ns |  0.0311 ns | 0.0153 |      - |      96 B |
| EmptyApiServer              |   3.540 ns |   1.4984 ns |  0.0821 ns | 0.0089 |      - |      56 B |
| EmptyApiServerVariable      |   2.888 ns |   1.5857 ns |  0.0869 ns | 0.0076 |      - |      48 B |
| EmptyApiTag                 |   3.102 ns |   7.2040 ns |  0.3949 ns | 0.0076 |      - |      48 B |
