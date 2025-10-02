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
| EmptyApiCallback            |   4.729 ns |   4.165 ns |  0.2283 ns | 0.0051 |      32 B |
| EmptyApiComponents          |   6.363 ns |   4.104 ns |  0.2250 ns | 0.0166 |     104 B |
| EmptyApiContact             |   4.935 ns |   4.592 ns |  0.2517 ns | 0.0076 |      48 B |
| EmptyApiDiscriminator       |   4.489 ns |   2.375 ns |  0.1302 ns | 0.0076 |      48 B |
| EmptyDocument               | 500.015 ns | 516.293 ns | 28.2997 ns | 0.1822 |    1144 B |
| EmptyApiEncoding            |   4.969 ns |   4.077 ns |  0.2235 ns | 0.0089 |      56 B |
| EmptyApiExample             |   6.696 ns |  11.729 ns |  0.6429 ns | 0.0115 |      72 B |
| EmptyApiExternalDocs        |   4.406 ns |   2.431 ns |  0.1333 ns | 0.0064 |      40 B |
| EmptyApiHeader              |   8.554 ns |  32.020 ns |  1.7551 ns | 0.0127 |      80 B |
| EmptyApiInfo                |   7.712 ns |   8.397 ns |  0.4603 ns | 0.0127 |      80 B |
| EmptyApiLicense             |   6.327 ns |  24.406 ns |  1.3378 ns | 0.0076 |      48 B |
| EmptyApiLink                |   9.752 ns |  35.128 ns |  1.9255 ns | 0.0115 |      72 B |
| EmptyApiMediaType           |   6.677 ns |  16.501 ns |  0.9045 ns | 0.0127 |      80 B |
| EmptyApiOAuthFlow           |   6.861 ns |   9.665 ns |  0.5298 ns | 0.0102 |      64 B |
| EmptyApiOAuthFlows          |   6.438 ns |  19.570 ns |  1.0727 ns | 0.0102 |      64 B |
| EmptyApiOperation           |  83.363 ns |  10.816 ns |  0.5929 ns | 0.0598 |     376 B |
| EmptyApiParameter           |   8.253 ns |  22.120 ns |  1.2125 ns | 0.0153 |      96 B |
| EmptyApiPathItem            |   5.618 ns |   5.996 ns |  0.3286 ns | 0.0102 |      64 B |
| EmptyApiPaths               |  74.034 ns | 156.861 ns |  8.5981 ns | 0.0395 |     248 B |
| EmptyApiRequestBody         |   6.145 ns |  12.296 ns |  0.6740 ns | 0.0076 |      48 B |
| EmptyApiResponse            |  10.106 ns |  49.190 ns |  2.6963 ns | 0.0102 |      64 B |
| EmptyApiResponses           |  78.267 ns |  89.568 ns |  4.9095 ns | 0.0395 |     248 B |
| EmptyApiSchema              |  18.320 ns |   4.933 ns |  0.2704 ns | 0.0650 |     408 B |
| EmptyApiSecurityRequirement |  12.997 ns |  16.363 ns |  0.8969 ns | 0.0166 |     104 B |
| EmptyApiSecurityScheme      |   8.740 ns |   9.697 ns |  0.5315 ns | 0.0153 |      96 B |
| EmptyApiServer              |   5.493 ns |   6.821 ns |  0.3739 ns | 0.0089 |      56 B |
| EmptyApiServerVariable      |   5.511 ns |  11.169 ns |  0.6122 ns | 0.0076 |      48 B |
| EmptyApiTag                 |   5.088 ns |   3.306 ns |  0.1812 ns | 0.0076 |      48 B |
