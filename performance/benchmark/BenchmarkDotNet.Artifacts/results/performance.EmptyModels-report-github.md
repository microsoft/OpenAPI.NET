```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9711)
Snapdragon X 12-core X1E80100 3.40 GHz (Max: 3.42GHz), 1 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.404
  [Host]   : .NET 8.0.34 (8.0.34, 8.0.3426.52217), Arm64 RyuJIT armv8.0-a
  ShortRun : .NET 8.0.34 (8.0.34, 8.0.3426.52217), Arm64 RyuJIT armv8.0-a

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean       | Error      | StdDev    | Gen0   | Allocated |
|---------------------------- |-----------:|-----------:|----------:|-------:|----------:|
| EmptyApiCallback            |   2.176 ns |  1.3477 ns | 0.0739 ns | 0.0077 |      32 B |
| EmptyApiComponents          |   4.014 ns |  1.4239 ns | 0.0780 ns | 0.0249 |     104 B |
| EmptyApiContact             |   2.392 ns |  1.4431 ns | 0.0791 ns | 0.0115 |      48 B |
| EmptyApiDiscriminator       |   2.243 ns |  1.2297 ns | 0.0674 ns | 0.0096 |      40 B |
| EmptyDocument               | 366.289 ns | 29.1850 ns | 1.5997 ns | 0.2751 |    1152 B |
| EmptyApiEncoding            |   2.467 ns |  0.0524 ns | 0.0029 ns | 0.0134 |      56 B |
| EmptyApiExample             |   2.444 ns |  0.3290 ns | 0.0180 ns | 0.0134 |      56 B |
| EmptyApiExternalDocs        |   2.123 ns |  0.0742 ns | 0.0041 ns | 0.0096 |      40 B |
| EmptyApiHeader              |   3.009 ns |  0.1701 ns | 0.0093 ns | 0.0191 |      80 B |
| EmptyApiInfo                |   3.374 ns |  0.8706 ns | 0.0477 ns | 0.0191 |      80 B |
| EmptyApiLicense             |   2.369 ns |  0.4418 ns | 0.0242 ns | 0.0115 |      48 B |
| EmptyApiLink                |   2.767 ns |  0.9702 ns | 0.0532 ns | 0.0172 |      72 B |
| EmptyApiMediaType           |   2.649 ns |  1.1204 ns | 0.0614 ns | 0.0134 |      56 B |
| EmptyApiOAuthFlow           |   2.856 ns |  0.4533 ns | 0.0248 ns | 0.0134 |      56 B |
| EmptyApiOAuthFlows          |   2.754 ns |  0.3797 ns | 0.0208 ns | 0.0134 |      56 B |
| EmptyApiOperation           |  60.166 ns |  3.2769 ns | 0.1796 ns | 0.0899 |     376 B |
| EmptyApiParameter           |   3.466 ns |  0.0490 ns | 0.0027 ns | 0.0230 |      96 B |
| EmptyApiPathItem            |   2.700 ns |  0.7611 ns | 0.0417 ns | 0.0153 |      64 B |
| EmptyApiPaths               |  50.570 ns |  4.9646 ns | 0.2721 ns | 0.0592 |     248 B |
| EmptyApiRequestBody         |   2.426 ns |  0.3368 ns | 0.0185 ns | 0.0115 |      48 B |
| EmptyApiResponse            |   2.472 ns |  0.0846 ns | 0.0046 ns | 0.0134 |      56 B |
| EmptyApiResponses           |  50.297 ns | 13.1541 ns | 0.7210 ns | 0.0592 |     248 B |
| EmptyApiSchema              |  12.174 ns |  0.5459 ns | 0.0299 ns | 0.1224 |     512 B |
| EmptyApiSecurityRequirement |   8.101 ns |  1.3602 ns | 0.0746 ns | 0.0249 |     104 B |
| EmptyApiSecurityScheme      |   3.700 ns |  2.2623 ns | 0.1240 ns | 0.0210 |      88 B |
| EmptyApiServer              |   2.674 ns |  0.8604 ns | 0.0472 ns | 0.0115 |      48 B |
| EmptyApiServerVariable      |   2.502 ns |  1.6387 ns | 0.0898 ns | 0.0115 |      48 B |
| EmptyApiTag                 |   2.528 ns |  3.0444 ns | 0.1669 ns | 0.0115 |      48 B |
