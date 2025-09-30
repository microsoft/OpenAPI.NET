```

BenchmarkDotNet v0.15.4, Windows 11 (10.0.26200.6584)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.414
  [Host]   : .NET 8.0.20 (8.0.20, 8.0.2025.41914), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.20 (8.0.20, 8.0.2025.41914), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean       | Error         | StdDev      | Gen0   | Allocated |
|---------------------------- |-----------:|--------------:|------------:|-------:|----------:|
| EmptyApiCallback            |   4.611 ns |     0.2382 ns |   0.0131 ns | 0.0051 |      32 B |
| EmptyApiComponents          |  13.590 ns |   110.3530 ns |   6.0488 ns | 0.0166 |     104 B |
| EmptyApiContact             |   3.928 ns |     0.2265 ns |   0.0124 ns | 0.0076 |      48 B |
| EmptyApiDiscriminator       |   4.299 ns |     4.3753 ns |   0.2398 ns | 0.0064 |      40 B |
| EmptyDocument               | 788.076 ns | 3,490.5215 ns | 191.3273 ns | 0.1802 |    1136 B |
| EmptyApiEncoding            |   4.699 ns |    17.2697 ns |   0.9466 ns | 0.0089 |      56 B |
| EmptyApiExample             |   4.683 ns |     9.1504 ns |   0.5016 ns | 0.0089 |      56 B |
| EmptyApiExternalDocs        |   3.800 ns |     0.6940 ns |   0.0380 ns | 0.0064 |      40 B |
| EmptyApiHeader              |   5.206 ns |     4.1726 ns |   0.2287 ns | 0.0127 |      80 B |
| EmptyApiInfo                |   5.543 ns |     8.5164 ns |   0.4668 ns | 0.0127 |      80 B |
| EmptyApiLicense             |   4.144 ns |     0.4410 ns |   0.0242 ns | 0.0076 |      48 B |
| EmptyApiLink                |   4.812 ns |     1.1268 ns |   0.0618 ns | 0.0115 |      72 B |
| EmptyApiMediaType           |   4.947 ns |    16.1280 ns |   0.8840 ns | 0.0089 |      56 B |
| EmptyApiOAuthFlow           |   4.350 ns |     2.3894 ns |   0.1310 ns | 0.0089 |      56 B |
| EmptyApiOAuthFlows          |   4.997 ns |    11.8078 ns |   0.6472 ns | 0.0089 |      56 B |
| EmptyApiOperation           |  70.028 ns |    67.7802 ns |   3.7153 ns | 0.0598 |     376 B |
| EmptyApiParameter           |   5.850 ns |     3.8821 ns |   0.2128 ns | 0.0153 |      96 B |
| EmptyApiPathItem            |   4.779 ns |     3.1709 ns |   0.1738 ns | 0.0102 |      64 B |
| EmptyApiPaths               |  68.199 ns |    16.6646 ns |   0.9134 ns | 0.0395 |     248 B |
| EmptyApiRequestBody         |   4.329 ns |     3.7170 ns |   0.2037 ns | 0.0076 |      48 B |
| EmptyApiResponse            |   5.995 ns |    28.4117 ns |   1.5573 ns | 0.0102 |      64 B |
| EmptyApiResponses           |  61.406 ns |    18.1159 ns |   0.9930 ns | 0.0395 |     248 B |
| EmptyApiSchema              |  19.435 ns |    34.0416 ns |   1.8659 ns | 0.0650 |     408 B |
| EmptyApiSecurityRequirement |  13.501 ns |    14.6594 ns |   0.8035 ns | 0.0166 |     104 B |
| EmptyApiSecurityScheme      |   6.749 ns |     6.3253 ns |   0.3467 ns | 0.0153 |      96 B |
| EmptyApiServer              |   5.764 ns |     9.8387 ns |   0.5393 ns | 0.0089 |      56 B |
| EmptyApiServerVariable      |   4.275 ns |     1.0822 ns |   0.0593 ns | 0.0076 |      48 B |
| EmptyApiTag                 |   4.191 ns |     1.4517 ns |   0.0796 ns | 0.0076 |      48 B |
