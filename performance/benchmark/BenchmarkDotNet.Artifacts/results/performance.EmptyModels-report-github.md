```

BenchmarkDotNet v0.15.4, Windows 11 (10.0.26200.6584)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.414
  [Host]   : .NET 8.0.20 (8.0.20, 8.0.2025.41914), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.20 (8.0.20, 8.0.2025.41914), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean       | Error      | StdDev     | Gen0   | Allocated |
|---------------------------- |-----------:|-----------:|-----------:|-------:|----------:|
| EmptyApiCallback            |   4.828 ns |   6.435 ns |  0.3527 ns | 0.0051 |      32 B |
| EmptyApiComponents          |   6.520 ns |   9.088 ns |  0.4981 ns | 0.0166 |     104 B |
| EmptyApiContact             |   4.413 ns |  10.508 ns |  0.5760 ns | 0.0076 |      48 B |
| EmptyApiDiscriminator       |   4.401 ns |   3.087 ns |  0.1692 ns | 0.0064 |      40 B |
| EmptyDocument               | 528.141 ns | 519.009 ns | 28.4486 ns | 0.1822 |    1144 B |
| EmptyApiEncoding            |   5.046 ns |  14.821 ns |  0.8124 ns | 0.0089 |      56 B |
| EmptyApiExample             |   5.190 ns |   2.976 ns |  0.1631 ns | 0.0089 |      56 B |
| EmptyApiExternalDocs        |   4.430 ns |   8.570 ns |  0.4698 ns | 0.0064 |      40 B |
| EmptyApiHeader              |   6.005 ns |  12.071 ns |  0.6617 ns | 0.0127 |      80 B |
| EmptyApiInfo                |   6.108 ns |   9.581 ns |  0.5252 ns | 0.0127 |      80 B |
| EmptyApiLicense             |   5.106 ns |   5.930 ns |  0.3251 ns | 0.0076 |      48 B |
| EmptyApiLink                |   5.468 ns |   5.850 ns |  0.3207 ns | 0.0115 |      72 B |
| EmptyApiMediaType           |   5.382 ns |   7.645 ns |  0.4190 ns | 0.0089 |      56 B |
| EmptyApiOAuthFlow           |  10.300 ns |  70.612 ns |  3.8705 ns | 0.0089 |      56 B |
| EmptyApiOAuthFlows          |   6.232 ns |  12.654 ns |  0.6936 ns | 0.0089 |      56 B |
| EmptyApiOperation           | 112.000 ns | 177.327 ns |  9.7199 ns | 0.0598 |     376 B |
| EmptyApiParameter           |   8.392 ns |  26.414 ns |  1.4478 ns | 0.0153 |      96 B |
| EmptyApiPathItem            |   8.074 ns |  43.179 ns |  2.3668 ns | 0.0102 |      64 B |
| EmptyApiPaths               | 100.150 ns | 108.226 ns |  5.9322 ns | 0.0395 |     248 B |
| EmptyApiRequestBody         |   4.110 ns |   1.149 ns |  0.0630 ns | 0.0076 |      48 B |
| EmptyApiResponse            |   4.860 ns |   1.099 ns |  0.0603 ns | 0.0102 |      64 B |
| EmptyApiResponses           |  93.849 ns | 294.979 ns | 16.1688 ns | 0.0395 |     248 B |
| EmptyApiSchema              |  20.206 ns |  20.946 ns |  1.1481 ns | 0.0650 |     408 B |
| EmptyApiSecurityRequirement |  14.111 ns |  34.042 ns |  1.8660 ns | 0.0166 |     104 B |
| EmptyApiSecurityScheme      |   8.667 ns |  14.076 ns |  0.7715 ns | 0.0153 |      96 B |
| EmptyApiServer              |   7.689 ns |  46.200 ns |  2.5324 ns | 0.0089 |      56 B |
| EmptyApiServerVariable      |   4.385 ns |   4.847 ns |  0.2657 ns | 0.0076 |      48 B |
| EmptyApiTag                 |   4.780 ns |   5.955 ns |  0.3264 ns | 0.0076 |      48 B |
