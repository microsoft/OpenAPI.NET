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
| EmptyApiCallback            |   1.609 ns |  0.2332 ns | 0.0128 ns | 0.0077 |      32 B |
| EmptyApiComponents          |   3.057 ns |  0.1980 ns | 0.0109 ns | 0.0249 |     104 B |
| EmptyApiContact             |   1.873 ns |  0.2182 ns | 0.0120 ns | 0.0115 |      48 B |
| EmptyApiDiscriminator       |   1.701 ns |  0.3282 ns | 0.0180 ns | 0.0096 |      40 B |
| EmptyDocument               | 295.365 ns | 25.8942 ns | 1.4193 ns | 0.2751 |    1152 B |
| EmptyApiEncoding            |   2.055 ns |  0.3608 ns | 0.0198 ns | 0.0134 |      56 B |
| EmptyApiExample             |   2.007 ns |  0.2155 ns | 0.0118 ns | 0.0134 |      56 B |
| EmptyApiExternalDocs        |   1.709 ns |  0.2845 ns | 0.0156 ns | 0.0096 |      40 B |
| EmptyApiHeader              |   2.515 ns |  0.1279 ns | 0.0070 ns | 0.0191 |      80 B |
| EmptyApiInfo                |   2.493 ns |  0.2505 ns | 0.0137 ns | 0.0191 |      80 B |
| EmptyApiLicense             |   1.867 ns |  0.1831 ns | 0.0100 ns | 0.0115 |      48 B |
| EmptyApiLink                |   1.769 ns |  0.2593 ns | 0.0142 ns | 0.0172 |      72 B |
| EmptyApiMediaType           |   2.018 ns |  0.3384 ns | 0.0186 ns | 0.0134 |      56 B |
| EmptyApiOAuthFlow           |   2.061 ns |  0.2944 ns | 0.0161 ns | 0.0134 |      56 B |
| EmptyApiOAuthFlows          |   2.044 ns |  0.2322 ns | 0.0127 ns | 0.0134 |      56 B |
| EmptyApiOperation           |  47.464 ns |  6.6225 ns | 0.3630 ns | 0.0899 |     376 B |
| EmptyApiParameter           |   2.861 ns |  0.2335 ns | 0.0128 ns | 0.0230 |      96 B |
| EmptyApiPathItem            |   2.195 ns |  0.2627 ns | 0.0144 ns | 0.0153 |      64 B |
| EmptyApiPaths               |  42.303 ns |  3.6356 ns | 0.1993 ns | 0.0592 |     248 B |
| EmptyApiRequestBody         |   1.859 ns |  0.3941 ns | 0.0216 ns | 0.0115 |      48 B |
| EmptyApiResponse            |   2.036 ns |  0.1630 ns | 0.0089 ns | 0.0134 |      56 B |
| EmptyApiResponses           |  43.046 ns |  2.1392 ns | 0.1173 ns | 0.0592 |     248 B |
| EmptyApiSchema              |  10.918 ns |  0.3190 ns | 0.0175 ns | 0.1224 |     512 B |
| EmptyApiSecurityRequirement |   6.793 ns |  0.8479 ns | 0.0465 ns | 0.0249 |     104 B |
| EmptyApiSecurityScheme      |   2.128 ns |  0.2049 ns | 0.0112 ns | 0.0210 |      88 B |
| EmptyApiServer              |   1.850 ns |  0.2318 ns | 0.0127 ns | 0.0115 |      48 B |
| EmptyApiServerVariable      |   2.002 ns |  0.6545 ns | 0.0359 ns | 0.0115 |      48 B |
| EmptyApiTag                 |   1.873 ns |  0.1548 ns | 0.0085 ns | 0.0115 |      48 B |
