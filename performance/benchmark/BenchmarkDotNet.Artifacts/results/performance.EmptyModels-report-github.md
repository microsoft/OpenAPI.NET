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
| EmptyApiCallback            |   1.751 ns |  0.2856 ns | 0.0157 ns | 0.0077 |      32 B |
| EmptyApiComponents          |   3.097 ns |  0.9298 ns | 0.0510 ns | 0.0249 |     104 B |
| EmptyApiContact             |   2.013 ns |  1.0469 ns | 0.0574 ns | 0.0115 |      48 B |
| EmptyApiDiscriminator       |   1.777 ns |  0.5696 ns | 0.0312 ns | 0.0096 |      40 B |
| EmptyDocument               | 287.564 ns | 10.2224 ns | 0.5603 ns | 0.2713 |    1136 B |
| EmptyApiEncoding            |   2.097 ns |  1.1678 ns | 0.0640 ns | 0.0134 |      56 B |
| EmptyApiExample             |   2.050 ns |  0.7205 ns | 0.0395 ns | 0.0134 |      56 B |
| EmptyApiExternalDocs        |   1.774 ns |  0.4241 ns | 0.0232 ns | 0.0096 |      40 B |
| EmptyApiHeader              |   2.462 ns |  1.3315 ns | 0.0730 ns | 0.0191 |      80 B |
| EmptyApiInfo                |   2.572 ns |  2.5960 ns | 0.1423 ns | 0.0191 |      80 B |
| EmptyApiLicense             |   1.982 ns |  0.1459 ns | 0.0080 ns | 0.0115 |      48 B |
| EmptyApiLink                |   2.504 ns |  0.1151 ns | 0.0063 ns | 0.0172 |      72 B |
| EmptyApiMediaType           |   2.158 ns |  0.0825 ns | 0.0045 ns | 0.0134 |      56 B |
| EmptyApiOAuthFlow           |   2.239 ns |  1.2433 ns | 0.0681 ns | 0.0134 |      56 B |
| EmptyApiOAuthFlows          |   2.104 ns |  0.4341 ns | 0.0238 ns | 0.0134 |      56 B |
| EmptyApiOperation           |  46.730 ns |  5.2571 ns | 0.2882 ns | 0.0899 |     376 B |
| EmptyApiParameter           |   2.879 ns |  0.6769 ns | 0.0371 ns | 0.0230 |      96 B |
| EmptyApiPathItem            |   2.173 ns |  0.3015 ns | 0.0165 ns | 0.0153 |      64 B |
| EmptyApiPaths               |  41.833 ns |  4.7467 ns | 0.2602 ns | 0.0592 |     248 B |
| EmptyApiRequestBody         |   1.980 ns |  0.7034 ns | 0.0386 ns | 0.0115 |      48 B |
| EmptyApiResponse            |   2.033 ns |  0.5751 ns | 0.0315 ns | 0.0134 |      56 B |
| EmptyApiResponses           |  41.616 ns |  2.3761 ns | 0.1302 ns | 0.0592 |     248 B |
| EmptyApiSchema              |  10.349 ns |  4.5393 ns | 0.2488 ns | 0.1224 |     512 B |
| EmptyApiSecurityRequirement |   6.803 ns |  1.1485 ns | 0.0630 ns | 0.0249 |     104 B |
| EmptyApiSecurityScheme      |   2.651 ns |  0.4362 ns | 0.0239 ns | 0.0210 |      88 B |
| EmptyApiServer              |   1.881 ns |  1.1119 ns | 0.0609 ns | 0.0115 |      48 B |
| EmptyApiServerVariable      |   1.904 ns |  1.3084 ns | 0.0717 ns | 0.0115 |      48 B |
| EmptyApiTag                 |   1.917 ns |  0.5099 ns | 0.0280 ns | 0.0115 |      48 B |
