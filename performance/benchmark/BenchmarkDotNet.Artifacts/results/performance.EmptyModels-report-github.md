```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7840/25H2/2025Update/HudsonValley2)
AMD Ryzen 7 7800X3D 4.20GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.418
  [Host]   : .NET 8.0.24 (8.0.24, 8.0.2426.7010), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.24 (8.0.24, 8.0.2426.7010), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean       | Error       | StdDev    | Gen0   | Allocated |
|---------------------------- |-----------:|------------:|----------:|-------:|----------:|
| EmptyApiCallback            |   2.544 ns |   2.0548 ns | 0.1126 ns | 0.0006 |      32 B |
| EmptyApiComponents          |   3.670 ns |   1.0618 ns | 0.0582 ns | 0.0022 |     112 B |
| EmptyApiContact             |   2.780 ns |   1.5098 ns | 0.0828 ns | 0.0010 |      48 B |
| EmptyApiDiscriminator       |   2.866 ns |   2.0794 ns | 0.1140 ns | 0.0010 |      48 B |
| EmptyDocument               | 288.287 ns | 164.5128 ns | 9.0175 ns | 0.0224 |    1144 B |
| EmptyApiEncoding            |   3.279 ns |   1.7411 ns | 0.0954 ns | 0.0016 |      80 B |
| EmptyApiExample             |   3.833 ns |   5.7132 ns | 0.3132 ns | 0.0014 |      72 B |
| EmptyApiExternalDocs        |   2.612 ns |   0.8470 ns | 0.0464 ns | 0.0008 |      40 B |
| EmptyApiHeader              |   3.371 ns |   2.0284 ns | 0.1112 ns | 0.0016 |      80 B |
| EmptyApiInfo                |   3.496 ns |   4.4784 ns | 0.2455 ns | 0.0016 |      80 B |
| EmptyApiLicense             |   3.192 ns |   5.8244 ns | 0.3193 ns | 0.0010 |      48 B |
| EmptyApiLink                |   3.317 ns |   1.5361 ns | 0.0842 ns | 0.0014 |      72 B |
| EmptyApiMediaType           |   3.531 ns |   3.6109 ns | 0.1979 ns | 0.0016 |      80 B |
| EmptyApiOAuthFlow           |   3.090 ns |   2.4010 ns | 0.1316 ns | 0.0013 |      64 B |
| EmptyApiOAuthFlows          |   3.251 ns |   3.1758 ns | 0.1741 ns | 0.0013 |      64 B |
| EmptyApiOperation           |  44.999 ns |  91.2661 ns | 5.0026 ns | 0.0075 |     376 B |
| EmptyApiParameter           |   3.871 ns |   5.3081 ns | 0.2910 ns | 0.0019 |      96 B |
| EmptyApiPathItem            |   3.152 ns |   2.3126 ns | 0.1268 ns | 0.0013 |      64 B |
| EmptyApiPaths               |  39.383 ns |  23.3311 ns | 1.2789 ns | 0.0049 |     248 B |
| EmptyApiRequestBody         |   3.048 ns |   2.9421 ns | 0.1613 ns | 0.0010 |      48 B |
| EmptyApiResponse            |   3.166 ns |   4.2306 ns | 0.2319 ns | 0.0013 |      64 B |
| EmptyApiResponses           |  37.056 ns |  17.1438 ns | 0.9397 ns | 0.0049 |     248 B |
| EmptyApiSchema              |   8.117 ns |   5.2729 ns | 0.2890 ns | 0.0083 |     416 B |
| EmptyApiSecurityRequirement |   7.151 ns |   6.2278 ns | 0.3414 ns | 0.0021 |     104 B |
| EmptyApiSecurityScheme      |   3.708 ns |   5.3268 ns | 0.2920 ns | 0.0021 |     104 B |
| EmptyApiServer              |   2.923 ns |   0.4437 ns | 0.0243 ns | 0.0011 |      56 B |
| EmptyApiServerVariable      |   2.916 ns |   3.7008 ns | 0.2029 ns | 0.0010 |      48 B |
| EmptyApiTag                 |   3.470 ns |   4.5358 ns | 0.2486 ns | 0.0014 |      72 B |
