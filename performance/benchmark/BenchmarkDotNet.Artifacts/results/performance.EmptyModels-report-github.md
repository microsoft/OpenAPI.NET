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
| EmptyApiCallback            |   1.639 ns |  0.2293 ns | 0.0126 ns | 0.0077 |      32 B |
| EmptyApiComponents          |   9.982 ns |  1.1676 ns | 0.0640 ns | 0.0268 |     112 B |
| EmptyApiContact             |   1.983 ns |  0.1871 ns | 0.0103 ns | 0.0115 |      48 B |
| EmptyApiDiscriminator       |   2.100 ns |  0.6646 ns | 0.0364 ns | 0.0115 |      48 B |
| EmptyDocument               | 312.325 ns | 25.4404 ns | 1.3945 ns | 0.2732 |    1144 B |
| EmptyApiEncoding            |   8.220 ns |  5.4137 ns | 0.2967 ns | 0.0191 |      80 B |
| EmptyApiExample             |   2.224 ns |  1.9925 ns | 0.1092 ns | 0.0172 |      72 B |
| EmptyApiExternalDocs        |   1.702 ns |  2.5621 ns | 0.1404 ns | 0.0096 |      40 B |
| EmptyApiHeader              |   2.481 ns |  0.9789 ns | 0.0537 ns | 0.0191 |      80 B |
| EmptyApiInfo                |   2.507 ns |  1.1456 ns | 0.0628 ns | 0.0191 |      80 B |
| EmptyApiLicense             |   1.773 ns |  1.7535 ns | 0.0961 ns | 0.0115 |      48 B |
| EmptyApiLink                |   2.128 ns |  0.6084 ns | 0.0333 ns | 0.0172 |      72 B |
| EmptyApiMediaType           |   8.070 ns |  1.8399 ns | 0.1009 ns | 0.0191 |      80 B |
| EmptyApiOAuthFlow           |   2.282 ns |  0.5407 ns | 0.0296 ns | 0.0153 |      64 B |
| EmptyApiOAuthFlows          |   8.983 ns |  1.1142 ns | 0.0611 ns | 0.0153 |      64 B |
| EmptyApiOperation           |  96.716 ns |  3.0416 ns | 0.1667 ns | 0.0899 |     376 B |
| EmptyApiParameter           |   9.601 ns |  1.4536 ns | 0.0797 ns | 0.0229 |      96 B |
| EmptyApiPathItem            |   2.261 ns |  0.3447 ns | 0.0189 ns | 0.0153 |      64 B |
| EmptyApiPaths               |  43.543 ns |  2.7691 ns | 0.1518 ns | 0.0592 |     248 B |
| EmptyApiRequestBody         |   2.123 ns |  0.8041 ns | 0.0441 ns | 0.0115 |      48 B |
| EmptyApiResponse            |   2.316 ns |  0.0985 ns | 0.0054 ns | 0.0153 |      64 B |
| EmptyApiResponses           |  82.569 ns |  3.5980 ns | 0.1972 ns | 0.0592 |     248 B |
| EmptyApiSchema              |  11.077 ns |  0.6623 ns | 0.0363 ns | 0.1224 |     512 B |
| EmptyApiSecurityRequirement |   7.046 ns |  1.5013 ns | 0.0823 ns | 0.0249 |     104 B |
| EmptyApiSecurityScheme      |   3.054 ns |  0.4315 ns | 0.0237 ns | 0.0249 |     104 B |
| EmptyApiServer              |   8.729 ns |  0.2324 ns | 0.0127 ns | 0.0134 |      56 B |
| EmptyApiServerVariable      |   1.940 ns |  0.3199 ns | 0.0175 ns | 0.0115 |      48 B |
| EmptyApiTag                 |   2.415 ns |  0.2616 ns | 0.0143 ns | 0.0172 |      72 B |
