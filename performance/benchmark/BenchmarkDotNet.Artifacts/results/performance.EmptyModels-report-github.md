```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8655/25H2/2025Update/HudsonValley2)
Snapdragon X 12-core X1E80100 3.40 GHz (Max: 3.42GHz), 1 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.301
  [Host]   : .NET 8.0.28 (8.0.28, 8.0.2826.26413), Arm64 RyuJIT armv8.0-a
  ShortRun : .NET 8.0.28 (8.0.28, 8.0.2826.26413), Arm64 RyuJIT armv8.0-a

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean       | Error     | StdDev    | Gen0   | Allocated |
|---------------------------- |-----------:|----------:|----------:|-------:|----------:|
| EmptyApiCallback            |   1.631 ns | 0.4678 ns | 0.0256 ns | 0.0077 |      32 B |
| EmptyApiComponents          |   3.038 ns | 0.4529 ns | 0.0248 ns | 0.0249 |     104 B |
| EmptyApiContact             |   1.896 ns | 0.3129 ns | 0.0172 ns | 0.0115 |      48 B |
| EmptyApiDiscriminator       |   1.749 ns | 0.3228 ns | 0.0177 ns | 0.0096 |      40 B |
| EmptyDocument               | 299.380 ns | 9.4022 ns | 0.5154 ns | 0.2713 |    1136 B |
| EmptyApiEncoding            |   2.058 ns | 0.5317 ns | 0.0291 ns | 0.0134 |      56 B |
| EmptyApiExample             |   2.109 ns | 0.5283 ns | 0.0290 ns | 0.0134 |      56 B |
| EmptyApiExternalDocs        |   1.805 ns | 0.5617 ns | 0.0308 ns | 0.0096 |      40 B |
| EmptyApiHeader              |   2.603 ns | 0.2398 ns | 0.0131 ns | 0.0191 |      80 B |
| EmptyApiInfo                |   2.592 ns | 0.1590 ns | 0.0087 ns | 0.0191 |      80 B |
| EmptyApiLicense             |   1.943 ns | 0.1054 ns | 0.0058 ns | 0.0115 |      48 B |
| EmptyApiLink                |   2.421 ns | 0.4424 ns | 0.0242 ns | 0.0172 |      72 B |
| EmptyApiMediaType           |   2.090 ns | 0.4463 ns | 0.0245 ns | 0.0134 |      56 B |
| EmptyApiOAuthFlow           |   2.091 ns | 0.1712 ns | 0.0094 ns | 0.0134 |      56 B |
| EmptyApiOAuthFlows          |   2.145 ns | 1.3739 ns | 0.0753 ns | 0.0134 |      56 B |
| EmptyApiOperation           |  49.237 ns | 5.8974 ns | 0.3233 ns | 0.0899 |     376 B |
| EmptyApiParameter           |   2.901 ns | 0.1597 ns | 0.0088 ns | 0.0230 |      96 B |
| EmptyApiPathItem            |   2.268 ns | 0.3209 ns | 0.0176 ns | 0.0153 |      64 B |
| EmptyApiPaths               |  43.940 ns | 0.7996 ns | 0.0438 ns | 0.0592 |     248 B |
| EmptyApiRequestBody         |   2.018 ns | 1.0255 ns | 0.0562 ns | 0.0115 |      48 B |
| EmptyApiResponse            |   2.147 ns | 0.2978 ns | 0.0163 ns | 0.0134 |      56 B |
| EmptyApiResponses           |  44.493 ns | 8.3008 ns | 0.4550 ns | 0.0592 |     248 B |
| EmptyApiSchema              |  11.176 ns | 1.0596 ns | 0.0581 ns | 0.1224 |     512 B |
| EmptyApiSecurityRequirement |   6.937 ns | 0.9959 ns | 0.0546 ns | 0.0249 |     104 B |
| EmptyApiSecurityScheme      |   2.727 ns | 0.3873 ns | 0.0212 ns | 0.0210 |      88 B |
| EmptyApiServer              |   1.989 ns | 0.9299 ns | 0.0510 ns | 0.0115 |      48 B |
| EmptyApiServerVariable      |   1.925 ns | 0.5804 ns | 0.0318 ns | 0.0115 |      48 B |
| EmptyApiTag                 |   1.962 ns | 0.3806 ns | 0.0209 ns | 0.0115 |      48 B |
