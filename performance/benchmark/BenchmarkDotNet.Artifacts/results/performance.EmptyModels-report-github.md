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
| EmptyApiCallback            |   6.212 ns |   3.627 ns |  0.1988 ns | 0.0051 |      32 B |
| EmptyApiComponents          |   6.707 ns |   2.423 ns |  0.1328 ns | 0.0166 |     104 B |
| EmptyApiContact             |   6.376 ns |  10.314 ns |  0.5653 ns | 0.0076 |      48 B |
| EmptyApiDiscriminator       |   6.735 ns |   3.089 ns |  0.1693 ns | 0.0064 |      40 B |
| EmptyDocument               | 726.496 ns | 517.997 ns | 28.3932 ns | 0.1793 |    1136 B |
| EmptyApiEncoding            |   6.933 ns |  25.121 ns |  1.3770 ns | 0.0089 |      56 B |
| EmptyApiExample             |  10.714 ns |  24.903 ns |  1.3650 ns | 0.0089 |      56 B |
| EmptyApiExternalDocs        |   6.494 ns |   5.991 ns |  0.3284 ns | 0.0064 |      40 B |
| EmptyApiHeader              |   6.513 ns |   3.221 ns |  0.1766 ns | 0.0127 |      80 B |
| EmptyApiInfo                |   9.555 ns |   6.141 ns |  0.3366 ns | 0.0127 |      80 B |
| EmptyApiLicense             |   6.116 ns |  13.500 ns |  0.7400 ns | 0.0076 |      48 B |
| EmptyApiLink                |   7.932 ns |  12.038 ns |  0.6598 ns | 0.0115 |      72 B |
| EmptyApiMediaType           |   9.712 ns |  38.293 ns |  2.0990 ns | 0.0089 |      56 B |
| EmptyApiOAuthFlow           |   5.992 ns |  16.501 ns |  0.9044 ns | 0.0089 |      56 B |
| EmptyApiOAuthFlows          |   5.611 ns |   6.006 ns |  0.3292 ns | 0.0089 |      56 B |
| EmptyApiOperation           |  73.808 ns | 105.669 ns |  5.7921 ns | 0.0598 |     376 B |
| EmptyApiParameter           |   9.987 ns |  52.942 ns |  2.9019 ns | 0.0153 |      96 B |
| EmptyApiPathItem            |   7.261 ns |  47.687 ns |  2.6139 ns | 0.0102 |      64 B |
| EmptyApiPaths               |  71.777 ns | 153.840 ns |  8.4325 ns | 0.0395 |     248 B |
| EmptyApiRequestBody         |   6.336 ns |  15.158 ns |  0.8309 ns | 0.0076 |      48 B |
| EmptyApiResponse            |   6.722 ns |  25.357 ns |  1.3899 ns | 0.0102 |      64 B |
| EmptyApiResponses           |  69.793 ns |  73.649 ns |  4.0369 ns | 0.0395 |     248 B |
| EmptyApiSchema              |  15.572 ns |  12.622 ns |  0.6918 ns | 0.0650 |     408 B |
| EmptyApiSecurityRequirement |  14.110 ns |  49.363 ns |  2.7058 ns | 0.0166 |     104 B |
| EmptyApiSecurityScheme      |   5.967 ns |   2.491 ns |  0.1366 ns | 0.0140 |      88 B |
| EmptyApiServer              |   8.528 ns |  34.590 ns |  1.8960 ns | 0.0076 |      48 B |
| EmptyApiServerVariable      |   7.021 ns |  18.579 ns |  1.0184 ns | 0.0076 |      48 B |
| EmptyApiTag                 |   6.213 ns |  17.442 ns |  0.9561 ns | 0.0076 |      48 B |
