```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7781/25H2/2025Update/HudsonValley2)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.418
  [Host]   : .NET 8.0.24 (8.0.24, 8.0.2426.7010), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.24 (8.0.24, 8.0.2426.7010), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean       | Error         | StdDev     | Median     | Gen0   | Allocated |
|---------------------------- |-----------:|--------------:|-----------:|-----------:|-------:|----------:|
| EmptyApiCallback            |   7.107 ns |    27.5677 ns |  1.5111 ns |   6.733 ns | 0.0051 |      32 B |
| EmptyApiComponents          |   7.575 ns |    28.6866 ns |  1.5724 ns |   7.197 ns | 0.0179 |     112 B |
| EmptyApiContact             |   4.512 ns |     7.1450 ns |  0.3916 ns |   4.737 ns | 0.0076 |      48 B |
| EmptyApiDiscriminator       |   5.057 ns |     9.8280 ns |  0.5387 ns |   5.188 ns | 0.0076 |      48 B |
| EmptyDocument               | 512.914 ns | 1,179.4105 ns | 64.6475 ns | 490.147 ns | 0.1822 |    1144 B |
| EmptyApiEncoding            |   5.553 ns |     4.0716 ns |  0.2232 ns |   5.558 ns | 0.0127 |      80 B |
| EmptyApiExample             |   7.453 ns |    35.3590 ns |  1.9381 ns |   7.280 ns | 0.0115 |      72 B |
| EmptyApiExternalDocs        |   4.335 ns |     8.2705 ns |  0.4533 ns |   4.391 ns | 0.0064 |      40 B |
| EmptyApiHeader              |   4.809 ns |     1.0010 ns |  0.0549 ns |   4.783 ns | 0.0127 |      80 B |
| EmptyApiInfo                |   6.335 ns |     7.8701 ns |  0.4314 ns |   6.497 ns | 0.0127 |      80 B |
| EmptyApiLicense             |   4.646 ns |    11.5202 ns |  0.6315 ns |   4.866 ns | 0.0076 |      48 B |
| EmptyApiLink                |   5.075 ns |    11.0742 ns |  0.6070 ns |   4.839 ns | 0.0115 |      72 B |
| EmptyApiMediaType           |   5.995 ns |    37.5428 ns |  2.0578 ns |   4.808 ns | 0.0127 |      80 B |
| EmptyApiOAuthFlow           |   5.399 ns |     8.7086 ns |  0.4773 ns |   5.297 ns | 0.0102 |      64 B |
| EmptyApiOAuthFlows          |   5.644 ns |    17.9537 ns |  0.9841 ns |   5.634 ns | 0.0102 |      64 B |
| EmptyApiOperation           |  62.189 ns |     5.8945 ns |  0.3231 ns |  62.114 ns | 0.0598 |     376 B |
| EmptyApiParameter           |   5.664 ns |     3.3125 ns |  0.1816 ns |   5.668 ns | 0.0153 |      96 B |
| EmptyApiPathItem            |   5.656 ns |    18.7748 ns |  1.0291 ns |   5.920 ns | 0.0102 |      64 B |
| EmptyApiPaths               |  54.952 ns |    15.4066 ns |  0.8445 ns |  54.857 ns | 0.0395 |     248 B |
| EmptyApiRequestBody         |   4.035 ns |     1.9281 ns |  0.1057 ns |   4.044 ns | 0.0076 |      48 B |
| EmptyApiResponse            |   6.326 ns |    43.8105 ns |  2.4014 ns |   4.964 ns | 0.0102 |      64 B |
| EmptyApiResponses           |  57.107 ns |    72.4663 ns |  3.9721 ns |  55.247 ns | 0.0395 |     248 B |
| EmptyApiSchema              |  15.773 ns |    53.2697 ns |  2.9199 ns |  14.163 ns | 0.0663 |     416 B |
| EmptyApiSecurityRequirement |  11.263 ns |    25.3799 ns |  1.3912 ns |  11.019 ns | 0.0166 |     104 B |
| EmptyApiSecurityScheme      |   5.531 ns |     0.7452 ns |  0.0408 ns |   5.544 ns | 0.0166 |     104 B |
| EmptyApiServer              |   4.612 ns |     6.7112 ns |  0.3679 ns |   4.789 ns | 0.0089 |      56 B |
| EmptyApiServerVariable      |   4.004 ns |     1.7788 ns |  0.0975 ns |   4.049 ns | 0.0076 |      48 B |
| EmptyApiTag                 |   4.742 ns |     2.3792 ns |  0.1304 ns |   4.752 ns | 0.0115 |      72 B |
