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
| EmptyApiCallback            |   1.781 ns |  0.4829 ns | 0.0265 ns | 0.0077 |      32 B |
| EmptyApiComponents          |   4.957 ns |  2.9026 ns | 0.1591 ns | 0.0268 |     112 B |
| EmptyApiContact             |   2.865 ns |  1.4412 ns | 0.0790 ns | 0.0115 |      48 B |
| EmptyApiDiscriminator       |   2.670 ns |  0.6316 ns | 0.0346 ns | 0.0115 |      48 B |
| EmptyDocument               | 416.732 ns | 39.3570 ns | 2.1573 ns | 0.2732 |    1144 B |
| EmptyApiEncoding            |   3.503 ns |  0.4499 ns | 0.0247 ns | 0.0191 |      80 B |
| EmptyApiExample             |   3.548 ns |  0.8868 ns | 0.0486 ns | 0.0172 |      72 B |
| EmptyApiExternalDocs        |   2.652 ns |  0.8060 ns | 0.0442 ns | 0.0096 |      40 B |
| EmptyApiHeader              |   3.482 ns |  0.6217 ns | 0.0341 ns | 0.0191 |      80 B |
| EmptyApiInfo                |   3.512 ns |  0.6497 ns | 0.0356 ns | 0.0191 |      80 B |
| EmptyApiLicense             |   2.706 ns |  1.6023 ns | 0.0878 ns | 0.0115 |      48 B |
| EmptyApiLink                |   3.270 ns |  0.8580 ns | 0.0470 ns | 0.0172 |      72 B |
| EmptyApiMediaType           |   3.494 ns |  0.6535 ns | 0.0358 ns | 0.0191 |      80 B |
| EmptyApiOAuthFlow           |   3.094 ns |  0.9549 ns | 0.0523 ns | 0.0153 |      64 B |
| EmptyApiOAuthFlows          |   3.264 ns |  1.2645 ns | 0.0693 ns | 0.0153 |      64 B |
| EmptyApiOperation           |  64.758 ns |  8.5491 ns | 0.4686 ns | 0.0899 |     376 B |
| EmptyApiParameter           |   4.144 ns |  1.7628 ns | 0.0966 ns | 0.0229 |      96 B |
| EmptyApiPathItem            |   3.110 ns |  0.9364 ns | 0.0513 ns | 0.0153 |      64 B |
| EmptyApiPaths               |  58.693 ns | 15.0339 ns | 0.8241 ns | 0.0592 |     248 B |
| EmptyApiRequestBody         |   3.252 ns |  9.8887 ns | 0.5420 ns | 0.0115 |      48 B |
| EmptyApiResponse            |   3.152 ns |  1.1094 ns | 0.0608 ns | 0.0153 |      64 B |
| EmptyApiResponses           |  58.916 ns | 12.7100 ns | 0.6967 ns | 0.0592 |     248 B |
| EmptyApiSchema              |  14.711 ns |  4.1025 ns | 0.2249 ns | 0.1167 |     488 B |
| EmptyApiSecurityRequirement |   9.836 ns |  3.5845 ns | 0.1965 ns | 0.0249 |     104 B |
| EmptyApiSecurityScheme      |   4.153 ns |  1.5510 ns | 0.0850 ns | 0.0249 |     104 B |
| EmptyApiServer              |   2.913 ns |  1.0017 ns | 0.0549 ns | 0.0134 |      56 B |
| EmptyApiServerVariable      |   2.684 ns |  0.4622 ns | 0.0253 ns | 0.0115 |      48 B |
| EmptyApiTag                 |   3.467 ns |  2.0488 ns | 0.1123 ns | 0.0172 |      72 B |
