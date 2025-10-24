```

BenchmarkDotNet v0.15.4, Windows 11 (10.0.26200.6899)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.415
  [Host]   : .NET 8.0.21 (8.0.21, 8.0.2125.47513), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.21 (8.0.21, 8.0.2125.47513), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean       | Error         | StdDev     | Gen0   | Gen1   | Allocated |
|---------------------------- |-----------:|--------------:|-----------:|-------:|-------:|----------:|
| EmptyApiCallback            |   9.857 ns |     9.4413 ns |  0.5175 ns | 0.0051 |      - |      32 B |
| EmptyApiComponents          |   5.810 ns |     8.1726 ns |  0.4480 ns | 0.0179 |      - |     112 B |
| EmptyApiContact             |   4.056 ns |     6.2561 ns |  0.3429 ns | 0.0076 |      - |      48 B |
| EmptyApiDiscriminator       |   3.774 ns |     4.3627 ns |  0.2391 ns | 0.0076 |      - |      48 B |
| EmptyDocument               | 455.827 ns | 1,324.8541 ns | 72.6197 ns | 0.1822 | 0.0005 |    1144 B |
| EmptyApiEncoding            |   4.464 ns |     5.8213 ns |  0.3191 ns | 0.0127 |      - |      80 B |
| EmptyApiExample             |   4.049 ns |     6.4281 ns |  0.3523 ns | 0.0115 |      - |      72 B |
| EmptyApiExternalDocs        |   3.554 ns |     9.2144 ns |  0.5051 ns | 0.0064 |      - |      40 B |
| EmptyApiHeader              |   5.744 ns |     5.7874 ns |  0.3172 ns | 0.0127 |      - |      80 B |
| EmptyApiInfo                |   4.899 ns |     0.2540 ns |  0.0139 ns | 0.0127 |      - |      80 B |
| EmptyApiLicense             |   3.276 ns |     5.2349 ns |  0.2869 ns | 0.0076 |      - |      48 B |
| EmptyApiLink                |   4.808 ns |     5.0966 ns |  0.2794 ns | 0.0115 |      - |      72 B |
| EmptyApiMediaType           |   5.524 ns |     3.1665 ns |  0.1736 ns | 0.0127 |      - |      80 B |
| EmptyApiOAuthFlow           |   4.655 ns |     2.4297 ns |  0.1332 ns | 0.0102 |      - |      64 B |
| EmptyApiOAuthFlows          |   4.611 ns |     2.6531 ns |  0.1454 ns | 0.0102 |      - |      64 B |
| EmptyApiOperation           |  68.632 ns |    30.0777 ns |  1.6487 ns | 0.0598 |      - |     376 B |
| EmptyApiParameter           |   5.697 ns |     3.3660 ns |  0.1845 ns | 0.0153 |      - |      96 B |
| EmptyApiPathItem            |   4.366 ns |     1.1628 ns |  0.0637 ns | 0.0102 |      - |      64 B |
| EmptyApiPaths               |  56.296 ns |    39.6902 ns |  2.1756 ns | 0.0395 |      - |     248 B |
| EmptyApiRequestBody         |   3.891 ns |     0.5843 ns |  0.0320 ns | 0.0076 |      - |      48 B |
| EmptyApiResponse            |   4.411 ns |     1.4575 ns |  0.0799 ns | 0.0102 |      - |      64 B |
| EmptyApiResponses           |  52.932 ns |    11.5133 ns |  0.6311 ns | 0.0395 |      - |     248 B |
| EmptyApiSchema              |  15.148 ns |     2.4846 ns |  0.1362 ns | 0.0650 |      - |     408 B |
| EmptyApiSecurityRequirement |  12.187 ns |    25.9656 ns |  1.4233 ns | 0.0166 |      - |     104 B |
| EmptyApiSecurityScheme      |   6.508 ns |    33.8480 ns |  1.8553 ns | 0.0153 |      - |      96 B |
| EmptyApiServer              |  16.896 ns |    22.6705 ns |  1.2426 ns | 0.0089 |      - |      56 B |
| EmptyApiServerVariable      |   5.436 ns |    20.6472 ns |  1.1317 ns | 0.0076 |      - |      48 B |
| EmptyApiTag                 |   6.426 ns |     3.3776 ns |  0.1851 ns | 0.0115 |      - |      72 B |
