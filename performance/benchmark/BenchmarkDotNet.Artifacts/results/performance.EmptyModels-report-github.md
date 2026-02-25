```

BenchmarkDotNet v0.15.6, Windows 11 (10.0.26200.7781)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.418
  [Host]   : .NET 8.0.24 (8.0.24, 8.0.2426.7010), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.24 (8.0.24, 8.0.2426.7010), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean       | Error      | StdDev    | Gen0   | Gen1   | Allocated |
|---------------------------- |-----------:|-----------:|----------:|-------:|-------:|----------:|
| EmptyApiCallback            |   4.394 ns |  7.7398 ns | 0.4242 ns | 0.0051 |      - |      32 B |
| EmptyApiComponents          |   6.145 ns |  3.1822 ns | 0.1744 ns | 0.0166 |      - |     104 B |
| EmptyApiContact             |   4.473 ns |  1.8228 ns | 0.0999 ns | 0.0076 |      - |      48 B |
| EmptyApiDiscriminator       |   4.484 ns |  5.5910 ns | 0.3065 ns | 0.0064 |      - |      40 B |
| EmptyDocument               | 441.153 ns | 74.1464 ns | 4.0642 ns | 0.1807 | 0.0005 |    1136 B |
| EmptyApiEncoding            |   4.876 ns |  1.9665 ns | 0.1078 ns | 0.0089 |      - |      56 B |
| EmptyApiExample             |   4.982 ns |  0.8982 ns | 0.0492 ns | 0.0089 |      - |      56 B |
| EmptyApiExternalDocs        |   4.224 ns |  2.6263 ns | 0.1440 ns | 0.0064 |      - |      40 B |
| EmptyApiHeader              |   5.617 ns |  7.1086 ns | 0.3896 ns | 0.0127 |      - |      80 B |
| EmptyApiInfo                |   5.978 ns |  8.8949 ns | 0.4876 ns | 0.0127 |      - |      80 B |
| EmptyApiLicense             |   5.116 ns |  2.2648 ns | 0.1241 ns | 0.0076 |      - |      48 B |
| EmptyApiLink                |   5.791 ns |  6.7182 ns | 0.3682 ns | 0.0115 |      - |      72 B |
| EmptyApiMediaType           |   5.533 ns |  2.5596 ns | 0.1403 ns | 0.0089 |      - |      56 B |
| EmptyApiOAuthFlow           |   5.220 ns |  0.5278 ns | 0.0289 ns | 0.0089 |      - |      56 B |
| EmptyApiOAuthFlows          |   5.915 ns |  5.5310 ns | 0.3032 ns | 0.0089 |      - |      56 B |
| EmptyApiOperation           |  67.753 ns |  7.6830 ns | 0.4211 ns | 0.0598 |      - |     376 B |
| EmptyApiParameter           |   7.363 ns | 20.7768 ns | 1.1388 ns | 0.0153 |      - |      96 B |
| EmptyApiPathItem            |   5.958 ns |  8.9121 ns | 0.4885 ns | 0.0102 |      - |      64 B |
| EmptyApiPaths               |  60.404 ns | 11.7043 ns | 0.6416 ns | 0.0395 |      - |     248 B |
| EmptyApiRequestBody         |   4.739 ns |  5.1058 ns | 0.2799 ns | 0.0076 |      - |      48 B |
| EmptyApiResponse            |   5.912 ns | 17.1013 ns | 0.9374 ns | 0.0089 |      - |      56 B |
| EmptyApiResponses           |  58.344 ns | 31.8750 ns | 1.7472 ns | 0.0395 |      - |     248 B |
| EmptyApiSchema              |  15.576 ns |  8.9314 ns | 0.4896 ns | 0.0663 |      - |     416 B |
| EmptyApiSecurityRequirement |  11.522 ns |  0.8412 ns | 0.0461 ns | 0.0166 |      - |     104 B |
| EmptyApiSecurityScheme      |   6.179 ns |  9.7926 ns | 0.5368 ns | 0.0140 |      - |      88 B |
| EmptyApiServer              |   4.518 ns |  1.1882 ns | 0.0651 ns | 0.0076 |      - |      48 B |
| EmptyApiServerVariable      |   4.984 ns | 12.5509 ns | 0.6880 ns | 0.0076 |      - |      48 B |
| EmptyApiTag                 |   5.830 ns | 36.9629 ns | 2.0261 ns | 0.0076 |      - |      48 B |
