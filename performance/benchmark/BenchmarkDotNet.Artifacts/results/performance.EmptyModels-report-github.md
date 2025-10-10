```

BenchmarkDotNet v0.15.4, Windows 11 (10.0.26200.6584)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.414
  [Host]   : .NET 8.0.20 (8.0.20, 8.0.2025.41914), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.20 (8.0.20, 8.0.2025.41914), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean       | Error       | StdDev     | Median     | Gen0   | Allocated |
|---------------------------- |-----------:|------------:|-----------:|-----------:|-------:|----------:|
| EmptyApiCallback            |   8.932 ns |  25.7792 ns |  1.4130 ns |   9.648 ns | 0.0051 |      32 B |
| EmptyApiComponents          |  12.649 ns |  10.2075 ns |  0.5595 ns |  12.371 ns | 0.0179 |     112 B |
| EmptyApiContact             |   3.558 ns |  11.0229 ns |  0.6042 ns |   3.839 ns | 0.0076 |      48 B |
| EmptyApiDiscriminator       |   5.635 ns |  10.1181 ns |  0.5546 ns |   5.738 ns | 0.0076 |      48 B |
| EmptyDocument               | 550.270 ns | 481.6846 ns | 26.4028 ns | 554.942 ns | 0.1822 |    1144 B |
| EmptyApiEncoding            |   4.724 ns |   5.4908 ns |  0.3010 ns |   4.893 ns | 0.0127 |      80 B |
| EmptyApiExample             |   3.503 ns |   2.4258 ns |  0.1330 ns |   3.439 ns | 0.0115 |      72 B |
| EmptyApiExternalDocs        |   3.989 ns |  38.5188 ns |  2.1113 ns |   2.788 ns | 0.0064 |      40 B |
| EmptyApiHeader              |  10.137 ns |  28.3981 ns |  1.5566 ns |  10.276 ns | 0.0127 |      80 B |
| EmptyApiInfo                |   3.563 ns |   0.5973 ns |  0.0327 ns |   3.572 ns | 0.0127 |      80 B |
| EmptyApiLicense             |  10.870 ns |  82.6477 ns |  4.5302 ns |  12.523 ns | 0.0076 |      48 B |
| EmptyApiLink                |   3.845 ns |   6.1665 ns |  0.3380 ns |   3.936 ns | 0.0115 |      72 B |
| EmptyApiMediaType           |   8.664 ns |   3.7686 ns |  0.2066 ns |   8.577 ns | 0.0127 |      80 B |
| EmptyApiOAuthFlow           |   9.587 ns |  17.1593 ns |  0.9406 ns |   9.356 ns | 0.0102 |      64 B |
| EmptyApiOAuthFlows          |  13.459 ns |   9.2215 ns |  0.5055 ns |  13.355 ns | 0.0102 |      64 B |
| EmptyApiOperation           |  58.849 ns |  16.2113 ns |  0.8886 ns |  58.908 ns | 0.0598 |     376 B |
| EmptyApiParameter           |   5.985 ns |  14.1080 ns |  0.7733 ns |   5.581 ns | 0.0153 |      96 B |
| EmptyApiPathItem            |   7.139 ns |  21.9737 ns |  1.2045 ns |   6.748 ns | 0.0102 |      64 B |
| EmptyApiPaths               |  73.771 ns | 360.6736 ns | 19.7697 ns |  66.267 ns | 0.0395 |     248 B |
| EmptyApiRequestBody         |   3.278 ns |  20.0295 ns |  1.0979 ns |   2.676 ns | 0.0076 |      48 B |
| EmptyApiResponse            |   5.495 ns |  39.4834 ns |  2.1642 ns |   4.454 ns | 0.0102 |      64 B |
| EmptyApiResponses           |  65.789 ns | 106.1661 ns |  5.8193 ns |  62.600 ns | 0.0395 |     248 B |
| EmptyApiSchema              |  15.919 ns |  32.4069 ns |  1.7763 ns |  16.897 ns | 0.0650 |     408 B |
| EmptyApiSecurityRequirement |   8.337 ns |   0.5396 ns |  0.0296 ns |   8.349 ns | 0.0166 |     104 B |
| EmptyApiSecurityScheme      |   5.195 ns |   4.2028 ns |  0.2304 ns |   5.182 ns | 0.0153 |      96 B |
| EmptyApiServer              |   3.540 ns |   3.9717 ns |  0.2177 ns |   3.471 ns | 0.0089 |      56 B |
| EmptyApiServerVariable      |   6.720 ns |  21.2063 ns |  1.1624 ns |   6.274 ns | 0.0076 |      48 B |
| EmptyApiTag                 |   3.622 ns |   0.0612 ns |  0.0034 ns |   3.624 ns | 0.0115 |      72 B |
