```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8655/25H2/2025Update/HudsonValley2)
Snapdragon X 12-core X1E80100 3.40 GHz (Max: 3.42GHz), 1 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.301
  [Host]   : .NET 8.0.28 (8.0.28, 8.0.2826.26413), Arm64 RyuJIT armv8.0-a
  ShortRun : .NET 8.0.28 (8.0.28, 8.0.2826.26413), Arm64 RyuJIT armv8.0-a

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean       | Error      | StdDev    | Gen0   | Allocated |
|---------------------------- |-----------:|-----------:|----------:|-------:|----------:|
| EmptyApiCallback            |   1.646 ns |  0.6629 ns | 0.0363 ns | 0.0077 |      32 B |
| EmptyApiComponents          |   3.068 ns |  0.5393 ns | 0.0296 ns | 0.0249 |     104 B |
| EmptyApiContact             |   1.928 ns |  0.1985 ns | 0.0109 ns | 0.0115 |      48 B |
| EmptyApiDiscriminator       |   1.717 ns |  0.4121 ns | 0.0226 ns | 0.0096 |      40 B |
| EmptyDocument               | 299.059 ns | 22.2169 ns | 1.2178 ns | 0.2713 |    1136 B |
| EmptyApiEncoding            |   2.057 ns |  0.2635 ns | 0.0144 ns | 0.0134 |      56 B |
| EmptyApiExample             |   1.878 ns |  2.1182 ns | 0.1161 ns | 0.0134 |      56 B |
| EmptyApiExternalDocs        |   1.578 ns |  1.1661 ns | 0.0639 ns | 0.0096 |      40 B |
| EmptyApiHeader              |   2.258 ns |  2.2673 ns | 0.1243 ns | 0.0191 |      80 B |
| EmptyApiInfo                |   2.272 ns |  1.0395 ns | 0.0570 ns | 0.0191 |      80 B |
| EmptyApiLicense             |   1.711 ns |  1.5825 ns | 0.0867 ns | 0.0115 |      48 B |
| EmptyApiLink                |   2.251 ns |  2.9816 ns | 0.1634 ns | 0.0172 |      72 B |
| EmptyApiMediaType           |   1.821 ns |  1.1297 ns | 0.0619 ns | 0.0134 |      56 B |
| EmptyApiOAuthFlow           |   1.793 ns |  1.1581 ns | 0.0635 ns | 0.0134 |      56 B |
| EmptyApiOAuthFlows          |   2.447 ns |  0.5327 ns | 0.0292 ns | 0.0134 |      56 B |
| EmptyApiOperation           |  48.119 ns |  1.3899 ns | 0.0762 ns | 0.0899 |     376 B |
| EmptyApiParameter           |   2.861 ns |  3.9520 ns | 0.2166 ns | 0.0230 |      96 B |
| EmptyApiPathItem            |   2.214 ns |  0.2237 ns | 0.0123 ns | 0.0153 |      64 B |
| EmptyApiPaths               |  43.001 ns |  3.9765 ns | 0.2180 ns | 0.0592 |     248 B |
| EmptyApiRequestBody         |   1.877 ns |  0.4134 ns | 0.0227 ns | 0.0115 |      48 B |
| EmptyApiResponse            |   2.089 ns |  0.0827 ns | 0.0045 ns | 0.0134 |      56 B |
| EmptyApiResponses           |  43.530 ns |  0.3726 ns | 0.0204 ns | 0.0592 |     248 B |
| EmptyApiSchema              |  11.004 ns |  0.7435 ns | 0.0408 ns | 0.1224 |     512 B |
| EmptyApiSecurityRequirement |   7.082 ns |  1.8803 ns | 0.1031 ns | 0.0249 |     104 B |
| EmptyApiSecurityScheme      |   2.782 ns |  0.2327 ns | 0.0128 ns | 0.0210 |      88 B |
| EmptyApiServer              |   1.932 ns |  0.9701 ns | 0.0532 ns | 0.0115 |      48 B |
| EmptyApiServerVariable      |   1.967 ns |  0.2259 ns | 0.0124 ns | 0.0115 |      48 B |
| EmptyApiTag                 |   1.939 ns |  0.7970 ns | 0.0437 ns | 0.0115 |      48 B |
