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
| EmptyApiCallback            |   1.617 ns |  0.0672 ns | 0.0037 ns | 0.0077 |      32 B |
| EmptyApiComponents          |   3.296 ns |  0.6335 ns | 0.0347 ns | 0.0249 |     104 B |
| EmptyApiContact             |   1.877 ns |  0.2847 ns | 0.0156 ns | 0.0115 |      48 B |
| EmptyApiDiscriminator       |   1.729 ns |  0.3618 ns | 0.0198 ns | 0.0096 |      40 B |
| EmptyDocument               | 297.313 ns | 16.0613 ns | 0.8804 ns | 0.2713 |    1136 B |
| EmptyApiEncoding            |   2.119 ns |  0.8175 ns | 0.0448 ns | 0.0134 |      56 B |
| EmptyApiExample             |   2.420 ns |  9.4606 ns | 0.5186 ns | 0.0134 |      56 B |
| EmptyApiExternalDocs        |   1.622 ns |  3.7822 ns | 0.2073 ns | 0.0096 |      40 B |
| EmptyApiHeader              |   2.540 ns |  0.5043 ns | 0.0276 ns | 0.0191 |      80 B |
| EmptyApiInfo                |   2.538 ns |  0.4522 ns | 0.0248 ns | 0.0191 |      80 B |
| EmptyApiLicense             |   2.116 ns |  2.1212 ns | 0.1163 ns | 0.0115 |      48 B |
| EmptyApiLink                |   2.409 ns |  0.3301 ns | 0.0181 ns | 0.0172 |      72 B |
| EmptyApiMediaType           |   2.077 ns |  0.3947 ns | 0.0216 ns | 0.0134 |      56 B |
| EmptyApiOAuthFlow           |   2.099 ns |  1.1265 ns | 0.0617 ns | 0.0134 |      56 B |
| EmptyApiOAuthFlows          |   2.364 ns |  2.7114 ns | 0.1486 ns | 0.0134 |      56 B |
| EmptyApiOperation           |  46.196 ns |  8.4667 ns | 0.4641 ns | 0.0899 |     376 B |
| EmptyApiParameter           |   2.941 ns |  2.1841 ns | 0.1197 ns | 0.0230 |      96 B |
| EmptyApiPathItem            |   2.487 ns |  3.5335 ns | 0.1937 ns | 0.0153 |      64 B |
| EmptyApiPaths               |  43.691 ns | 11.9156 ns | 0.6531 ns | 0.0592 |     248 B |
| EmptyApiRequestBody         |   2.166 ns |  1.0253 ns | 0.0562 ns | 0.0115 |      48 B |
| EmptyApiResponse            |   2.176 ns |  0.4655 ns | 0.0255 ns | 0.0134 |      56 B |
| EmptyApiResponses           |  42.611 ns |  4.7641 ns | 0.2611 ns | 0.0592 |     248 B |
| EmptyApiSchema              |  10.504 ns |  0.2940 ns | 0.0161 ns | 0.1224 |     512 B |
| EmptyApiSecurityRequirement |   7.004 ns |  1.1125 ns | 0.0610 ns | 0.0249 |     104 B |
| EmptyApiSecurityScheme      |   2.702 ns |  0.6486 ns | 0.0356 ns | 0.0210 |      88 B |
| EmptyApiServer              |   2.022 ns |  0.6456 ns | 0.0354 ns | 0.0115 |      48 B |
| EmptyApiServerVariable      |   2.009 ns |  0.8402 ns | 0.0461 ns | 0.0115 |      48 B |
| EmptyApiTag                 |   2.042 ns |  1.0689 ns | 0.0586 ns | 0.0115 |      48 B |
