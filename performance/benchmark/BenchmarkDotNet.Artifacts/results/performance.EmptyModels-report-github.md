```

BenchmarkDotNet v0.15.8, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
AMD EPYC 9V74 3.65GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.404
  [Host]   : .NET 8.0.34 (8.0.34, 8.0.3426.52217), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.34 (8.0.34, 8.0.3426.52217), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean         | Error       | StdDev    | Gen0   | Allocated |
|---------------------------- |-------------:|------------:|----------:|-------:|----------:|
| EmptyApiCallback            |     5.072 ns |   2.8978 ns | 0.1588 ns | 0.0019 |      32 B |
| EmptyApiComponents          |     6.942 ns |   3.9368 ns | 0.2158 ns | 0.0067 |     112 B |
| EmptyApiContact             |     5.572 ns |   2.2434 ns | 0.1230 ns | 0.0029 |      48 B |
| EmptyApiDiscriminator       |     5.292 ns |   1.3874 ns | 0.0761 ns | 0.0029 |      48 B |
| EmptyDocument               | 1,091.475 ns | 103.6479 ns | 5.6813 ns | 0.0687 |    1160 B |
| EmptyApiEncoding            |     6.379 ns |   0.9976 ns | 0.0547 ns | 0.0048 |      80 B |
| EmptyApiExample             |     6.206 ns |   2.0933 ns | 0.1147 ns | 0.0043 |      72 B |
| EmptyApiExternalDocs        |     5.639 ns |   2.9655 ns | 0.1625 ns | 0.0024 |      40 B |
| EmptyApiHeader              |     6.387 ns |   2.6349 ns | 0.1444 ns | 0.0048 |      80 B |
| EmptyApiInfo                |     6.571 ns |   0.6525 ns | 0.0358 ns | 0.0048 |      80 B |
| EmptyApiLicense             |     5.622 ns |   1.6026 ns | 0.0878 ns | 0.0029 |      48 B |
| EmptyApiLink                |     6.224 ns |   3.5286 ns | 0.1934 ns | 0.0043 |      72 B |
| EmptyApiMediaType           |     6.206 ns |   1.7429 ns | 0.0955 ns | 0.0048 |      80 B |
| EmptyApiOAuthFlow           |     6.756 ns |   2.7987 ns | 0.1534 ns | 0.0038 |      64 B |
| EmptyApiOAuthFlows          |     6.045 ns |   1.9014 ns | 0.1042 ns | 0.0038 |      64 B |
| EmptyApiOperation           |    57.901 ns |   8.0931 ns | 0.4436 ns | 0.0224 |     376 B |
| EmptyApiParameter           |     6.467 ns |   0.7373 ns | 0.0404 ns | 0.0057 |      96 B |
| EmptyApiPathItem            |     6.131 ns |   0.8645 ns | 0.0474 ns | 0.0038 |      64 B |
| EmptyApiPaths               |    51.495 ns |   8.4424 ns | 0.4628 ns | 0.0148 |     248 B |
| EmptyApiRequestBody         |     5.430 ns |   3.2939 ns | 0.1806 ns | 0.0029 |      48 B |
| EmptyApiResponse            |     5.933 ns |   1.2681 ns | 0.0695 ns | 0.0038 |      64 B |
| EmptyApiResponses           |    54.594 ns |   6.1854 ns | 0.3390 ns | 0.0148 |     248 B |
| EmptyApiSchema              |    14.557 ns |   0.6100 ns | 0.0334 ns | 0.0306 |     512 B |
| EmptyApiSecurityRequirement |    12.947 ns |   0.5780 ns | 0.0317 ns | 0.0062 |     104 B |
| EmptyApiSecurityScheme      |     6.503 ns |   2.3250 ns | 0.1274 ns | 0.0062 |     104 B |
| EmptyApiServer              |     5.583 ns |   3.6199 ns | 0.1984 ns | 0.0033 |      56 B |
| EmptyApiServerVariable      |     5.436 ns |   0.1179 ns | 0.0065 ns | 0.0029 |      48 B |
| EmptyApiTag                 |     6.271 ns |   1.1216 ns | 0.0615 ns | 0.0043 |      72 B |
