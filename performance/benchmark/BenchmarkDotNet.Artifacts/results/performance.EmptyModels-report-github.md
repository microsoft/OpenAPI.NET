```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8457/25H2/2025Update/HudsonValley2)
Snapdragon X 12-core X1E80100 3.40 GHz (Max: 3.42GHz), 1 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.300
  [Host]   : .NET 8.0.27 (8.0.27, 8.0.2726.22922), Arm64 RyuJIT armv8.0-a
  ShortRun : .NET 8.0.27 (8.0.27, 8.0.2726.22922), Arm64 RyuJIT armv8.0-a

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean       | Error      | StdDev    | Gen0   | Allocated |
|---------------------------- |-----------:|-----------:|----------:|-------:|----------:|
| EmptyApiCallback            |   1.517 ns |  0.9275 ns | 0.0508 ns | 0.0077 |      32 B |
| EmptyApiComponents          |   2.960 ns |  1.9855 ns | 0.1088 ns | 0.0249 |     104 B |
| EmptyApiContact             |   1.798 ns |  2.0807 ns | 0.1140 ns | 0.0115 |      48 B |
| EmptyApiDiscriminator       |   1.608 ns |  0.8198 ns | 0.0449 ns | 0.0096 |      40 B |
| EmptyDocument               | 270.449 ns | 48.5141 ns | 2.6592 ns | 0.2713 |    1136 B |
| EmptyApiEncoding            |   1.966 ns |  1.1698 ns | 0.0641 ns | 0.0134 |      56 B |
| EmptyApiExample             |   2.749 ns |  1.7176 ns | 0.0941 ns | 0.0134 |      56 B |
| EmptyApiExternalDocs        |   2.027 ns |  2.8392 ns | 0.1556 ns | 0.0096 |      40 B |
| EmptyApiHeader              |   2.738 ns |  8.1294 ns | 0.4456 ns | 0.0191 |      80 B |
| EmptyApiInfo                |   2.955 ns |  5.9220 ns | 0.3246 ns | 0.0191 |      80 B |
| EmptyApiLicense             |   2.158 ns |  2.1432 ns | 0.1175 ns | 0.0115 |      48 B |
| EmptyApiLink                |   2.505 ns |  1.1212 ns | 0.0615 ns | 0.0172 |      72 B |
| EmptyApiMediaType           |   2.094 ns |  3.4698 ns | 0.1902 ns | 0.0134 |      56 B |
| EmptyApiOAuthFlow           |   2.054 ns |  1.3889 ns | 0.0761 ns | 0.0134 |      56 B |
| EmptyApiOAuthFlows          |   2.301 ns |  2.0209 ns | 0.1108 ns | 0.0134 |      56 B |
| EmptyApiOperation           |  45.178 ns |  7.9592 ns | 0.4363 ns | 0.0899 |     376 B |
| EmptyApiParameter           |   3.169 ns |  5.4789 ns | 0.3003 ns | 0.0230 |      96 B |
| EmptyApiPathItem            |   2.275 ns |  0.6438 ns | 0.0353 ns | 0.0153 |      64 B |
| EmptyApiPaths               |  43.228 ns |  3.0199 ns | 0.1655 ns | 0.0592 |     248 B |
| EmptyApiRequestBody         |   1.970 ns |  0.0724 ns | 0.0040 ns | 0.0115 |      48 B |
| EmptyApiResponse            |   2.084 ns |  0.5726 ns | 0.0314 ns | 0.0134 |      56 B |
| EmptyApiResponses           |  43.235 ns | 11.7902 ns | 0.6463 ns | 0.0592 |     248 B |
| EmptyApiSchema              |   9.427 ns |  0.8926 ns | 0.0489 ns | 0.0995 |     416 B |
| EmptyApiSecurityRequirement |   7.208 ns |  1.1205 ns | 0.0614 ns | 0.0249 |     104 B |
| EmptyApiSecurityScheme      |   2.705 ns |  0.3761 ns | 0.0206 ns | 0.0210 |      88 B |
| EmptyApiServer              |   1.941 ns |  0.5668 ns | 0.0311 ns | 0.0115 |      48 B |
| EmptyApiServerVariable      |   1.943 ns |  0.8174 ns | 0.0448 ns | 0.0115 |      48 B |
| EmptyApiTag                 |   1.910 ns |  0.1369 ns | 0.0075 ns | 0.0115 |      48 B |
