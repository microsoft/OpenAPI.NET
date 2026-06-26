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
| EmptyApiCallback            |   1.834 ns |  0.4138 ns | 0.0227 ns | 0.0077 |      32 B |
| EmptyApiComponents          |   3.273 ns |  0.1576 ns | 0.0086 ns | 0.0268 |     112 B |
| EmptyApiContact             |   1.876 ns |  1.5257 ns | 0.0836 ns | 0.0115 |      48 B |
| EmptyApiDiscriminator       |   2.055 ns |  1.3792 ns | 0.0756 ns | 0.0115 |      48 B |
| EmptyDocument               | 301.687 ns | 18.8858 ns | 1.0352 ns | 0.2732 |    1144 B |
| EmptyApiEncoding            |   2.619 ns |  0.5789 ns | 0.0317 ns | 0.0191 |      80 B |
| EmptyApiExample             |   2.991 ns |  0.5519 ns | 0.0303 ns | 0.0172 |      72 B |
| EmptyApiExternalDocs        |   1.774 ns |  0.2712 ns | 0.0149 ns | 0.0096 |      40 B |
| EmptyApiHeader              |   2.640 ns |  0.5179 ns | 0.0284 ns | 0.0191 |      80 B |
| EmptyApiInfo                |   2.687 ns |  0.5028 ns | 0.0276 ns | 0.0191 |      80 B |
| EmptyApiLicense             |   1.955 ns |  0.3474 ns | 0.0190 ns | 0.0115 |      48 B |
| EmptyApiLink                |   2.455 ns |  0.5000 ns | 0.0274 ns | 0.0172 |      72 B |
| EmptyApiMediaType           |   2.886 ns | 10.2101 ns | 0.5597 ns | 0.0191 |      80 B |
| EmptyApiOAuthFlow           |   2.220 ns |  0.2202 ns | 0.0121 ns | 0.0153 |      64 B |
| EmptyApiOAuthFlows          |   2.223 ns |  0.2445 ns | 0.0134 ns | 0.0153 |      64 B |
| EmptyApiOperation           |  47.706 ns |  5.9139 ns | 0.3242 ns | 0.0899 |     376 B |
| EmptyApiParameter           |   2.872 ns |  0.3867 ns | 0.0212 ns | 0.0230 |      96 B |
| EmptyApiPathItem            |   2.226 ns |  0.4628 ns | 0.0254 ns | 0.0153 |      64 B |
| EmptyApiPaths               |  43.370 ns |  6.6182 ns | 0.3628 ns | 0.0592 |     248 B |
| EmptyApiRequestBody         |   1.923 ns |  0.1979 ns | 0.0108 ns | 0.0115 |      48 B |
| EmptyApiResponse            |   2.232 ns |  0.3409 ns | 0.0187 ns | 0.0153 |      64 B |
| EmptyApiResponses           |  43.110 ns |  1.1852 ns | 0.0650 ns | 0.0592 |     248 B |
| EmptyApiSchema              |  10.957 ns |  1.0671 ns | 0.0585 ns | 0.1224 |     512 B |
| EmptyApiSecurityRequirement |   6.967 ns |  0.5461 ns | 0.0299 ns | 0.0249 |     104 B |
| EmptyApiSecurityScheme      |   3.090 ns |  0.2228 ns | 0.0122 ns | 0.0249 |     104 B |
| EmptyApiServer              |   2.070 ns |  0.5205 ns | 0.0285 ns | 0.0134 |      56 B |
| EmptyApiServerVariable      |   1.919 ns |  0.6760 ns | 0.0371 ns | 0.0115 |      48 B |
| EmptyApiTag                 |   2.404 ns |  0.2185 ns | 0.0120 ns | 0.0172 |      72 B |
