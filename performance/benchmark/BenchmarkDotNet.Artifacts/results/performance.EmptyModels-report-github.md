```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9168/25H2/2025Update/HudsonValley2)
Snapdragon X 12-core X1E80100 3.40 GHz (Max: 3.42GHz), 1 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.303
  [Host]   : .NET 8.0.30 (8.0.30, 8.0.3026.36720), Arm64 RyuJIT armv8.0-a
  ShortRun : .NET 8.0.30 (8.0.30, 8.0.3026.36720), Arm64 RyuJIT armv8.0-a

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean       | Error       | StdDev     | Gen0   | Allocated |
|---------------------------- |-----------:|------------:|-----------:|-------:|----------:|
| EmptyApiCallback            |   1.797 ns |   1.1305 ns |  0.0620 ns | 0.0077 |      32 B |
| EmptyApiComponents          |   3.212 ns |   5.2799 ns |  0.2894 ns | 0.0249 |     104 B |
| EmptyApiContact             |   2.200 ns |   2.2648 ns |  0.1241 ns | 0.0115 |      48 B |
| EmptyApiDiscriminator       |   1.810 ns |   0.7037 ns |  0.0386 ns | 0.0096 |      40 B |
| EmptyDocument               | 320.239 ns | 270.6002 ns | 14.8325 ns | 0.2751 |    1152 B |
| EmptyApiEncoding            |   2.187 ns |   1.7488 ns |  0.0959 ns | 0.0134 |      56 B |
| EmptyApiExample             |   2.389 ns |   3.4710 ns |  0.1903 ns | 0.0134 |      56 B |
| EmptyApiExternalDocs        |   1.633 ns |   0.2642 ns |  0.0145 ns | 0.0096 |      40 B |
| EmptyApiHeader              |   2.405 ns |   2.1406 ns |  0.1173 ns | 0.0191 |      80 B |
| EmptyApiInfo                |   2.546 ns |   4.5880 ns |  0.2515 ns | 0.0191 |      80 B |
| EmptyApiLicense             |   1.694 ns |   1.0651 ns |  0.0584 ns | 0.0115 |      48 B |
| EmptyApiLink                |   2.304 ns |   1.4758 ns |  0.0809 ns | 0.0172 |      72 B |
| EmptyApiMediaType           |   2.088 ns |   1.8371 ns |  0.1007 ns | 0.0134 |      56 B |
| EmptyApiOAuthFlow           |   1.895 ns |   2.3751 ns |  0.1302 ns | 0.0134 |      56 B |
| EmptyApiOAuthFlows          |   1.819 ns |   1.2829 ns |  0.0703 ns | 0.0134 |      56 B |
| EmptyApiOperation           |  46.668 ns |   6.3921 ns |  0.3504 ns | 0.0899 |     376 B |
| EmptyApiParameter           |   2.525 ns |   2.7709 ns |  0.1519 ns | 0.0230 |      96 B |
| EmptyApiPathItem            |   2.071 ns |   0.3674 ns |  0.0201 ns | 0.0153 |      64 B |
| EmptyApiPaths               |  39.858 ns |  10.4440 ns |  0.5725 ns | 0.0592 |     248 B |
| EmptyApiRequestBody         |   1.873 ns |   1.5603 ns |  0.0855 ns | 0.0115 |      48 B |
| EmptyApiResponse            |   1.988 ns |   2.8827 ns |  0.1580 ns | 0.0134 |      56 B |
| EmptyApiResponses           |  40.284 ns |  19.2653 ns |  1.0560 ns | 0.0592 |     248 B |
| EmptyApiSchema              |   9.927 ns |   1.7206 ns |  0.0943 ns | 0.1224 |     512 B |
| EmptyApiSecurityRequirement |   6.373 ns |   1.6557 ns |  0.0908 ns | 0.0249 |     104 B |
| EmptyApiSecurityScheme      |   2.564 ns |   1.6803 ns |  0.0921 ns | 0.0210 |      88 B |
| EmptyApiServer              |   1.833 ns |   1.3469 ns |  0.0738 ns | 0.0115 |      48 B |
| EmptyApiServerVariable      |   1.792 ns |   0.4778 ns |  0.0262 ns | 0.0115 |      48 B |
| EmptyApiTag                 |   1.843 ns |   0.9756 ns |  0.0535 ns | 0.0115 |      48 B |
