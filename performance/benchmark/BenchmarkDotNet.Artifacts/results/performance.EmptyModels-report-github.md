```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8655/25H2/2025Update/HudsonValley2)
Snapdragon X 12-core X1E80100 3.40 GHz (Max: 3.42GHz), 1 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.301
  [Host]   : .NET 8.0.28 (8.0.28, 8.0.2826.26413), Arm64 RyuJIT armv8.0-a
  ShortRun : .NET 8.0.28 (8.0.28, 8.0.2826.26413), Arm64 RyuJIT armv8.0-a

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean       | Error       | StdDev    | Gen0   | Allocated |
|---------------------------- |-----------:|------------:|----------:|-------:|----------:|
| EmptyApiCallback            |   8.441 ns |   0.9560 ns | 0.0524 ns | 0.0076 |      32 B |
| EmptyApiComponents          |   8.789 ns |   3.0390 ns | 0.1666 ns | 0.0268 |     112 B |
| EmptyApiContact             |   1.718 ns |   1.3157 ns | 0.0721 ns | 0.0115 |      48 B |
| EmptyApiDiscriminator       |   1.850 ns |   1.5513 ns | 0.0850 ns | 0.0115 |      48 B |
| EmptyDocument               | 275.419 ns | 113.2081 ns | 6.2053 ns | 0.2732 |    1144 B |
| EmptyApiEncoding            |   8.658 ns |   5.0614 ns | 0.2774 ns | 0.0191 |      80 B |
| EmptyApiExample             |   8.261 ns |   4.4939 ns | 0.2463 ns | 0.0172 |      72 B |
| EmptyApiExternalDocs        |   7.637 ns |   3.4662 ns | 0.1900 ns | 0.0096 |      40 B |
| EmptyApiHeader              |   2.212 ns |   3.2714 ns | 0.1793 ns | 0.0191 |      80 B |
| EmptyApiInfo                |   2.498 ns |   2.0242 ns | 0.1110 ns | 0.0191 |      80 B |
| EmptyApiLicense             |   1.935 ns |   1.7171 ns | 0.0941 ns | 0.0115 |      48 B |
| EmptyApiLink                |   2.201 ns |   0.8545 ns | 0.0468 ns | 0.0172 |      72 B |
| EmptyApiMediaType           |   8.823 ns |   5.9655 ns | 0.3270 ns | 0.0191 |      80 B |
| EmptyApiOAuthFlow           |   2.194 ns |   0.4027 ns | 0.0221 ns | 0.0153 |      64 B |
| EmptyApiOAuthFlows          |   9.184 ns |   2.3278 ns | 0.1276 ns | 0.0153 |      64 B |
| EmptyApiOperation           | 100.518 ns |  21.5626 ns | 1.1819 ns | 0.0899 |     376 B |
| EmptyApiParameter           |   9.626 ns |   0.7296 ns | 0.0400 ns | 0.0229 |      96 B |
| EmptyApiPathItem            |   2.333 ns |   0.0919 ns | 0.0050 ns | 0.0153 |      64 B |
| EmptyApiPaths               |  43.750 ns |   3.5441 ns | 0.1943 ns | 0.0592 |     248 B |
| EmptyApiRequestBody         |   8.899 ns |   0.9160 ns | 0.0502 ns | 0.0115 |      48 B |
| EmptyApiResponse            |   9.078 ns |   1.3136 ns | 0.0720 ns | 0.0153 |      64 B |
| EmptyApiResponses           |  84.375 ns |   9.9152 ns | 0.5435 ns | 0.0592 |     248 B |
| EmptyApiSchema              |  11.545 ns |   1.5733 ns | 0.0862 ns | 0.1224 |     512 B |
| EmptyApiSecurityRequirement |   7.186 ns |   0.6334 ns | 0.0347 ns | 0.0249 |     104 B |
| EmptyApiSecurityScheme      |   2.914 ns |   4.4421 ns | 0.2435 ns | 0.0249 |     104 B |
| EmptyApiServer              |   2.276 ns |   0.6819 ns | 0.0374 ns | 0.0134 |      56 B |
| EmptyApiServerVariable      |   2.324 ns |   0.6012 ns | 0.0330 ns | 0.0115 |      48 B |
| EmptyApiTag                 |   2.387 ns |   0.8556 ns | 0.0469 ns | 0.0172 |      72 B |
