```

BenchmarkDotNet v0.15.8, macOS Tahoe 26.3 (25D125) [Darwin 25.3.0]
Apple M1 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK 8.0.418
  [Host]   : .NET 8.0.24 (8.0.24, 8.0.2426.7010), Arm64 RyuJIT armv8.0-a
  ShortRun : .NET 8.0.24 (8.0.24, 8.0.2426.7010), Arm64 RyuJIT armv8.0-a

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean       | Error      | StdDev    | Gen0   | Allocated |
|---------------------------- |-----------:|-----------:|----------:|-------:|----------:|
| EmptyApiCallback            |   2.736 ns |  2.0854 ns | 0.1143 ns | 0.0051 |      32 B |
| EmptyApiComponents          |   5.256 ns |  1.3034 ns | 0.0714 ns | 0.0179 |     112 B |
| EmptyApiContact             |   3.182 ns |  0.5211 ns | 0.0286 ns | 0.0076 |      48 B |
| EmptyApiDiscriminator       |   3.174 ns |  2.2139 ns | 0.1214 ns | 0.0076 |      48 B |
| EmptyDocument               | 569.692 ns | 64.9230 ns | 3.5586 ns | 0.1822 |    1144 B |
| EmptyApiEncoding            |   4.132 ns |  0.9468 ns | 0.0519 ns | 0.0127 |      80 B |
| EmptyApiExample             |   4.075 ns |  0.3257 ns | 0.0179 ns | 0.0115 |      72 B |
| EmptyApiExternalDocs        |   3.040 ns |  2.0834 ns | 0.1142 ns | 0.0064 |      40 B |
| EmptyApiHeader              |   4.314 ns |  2.7781 ns | 0.1523 ns | 0.0127 |      80 B |
| EmptyApiInfo                |   4.243 ns |  1.0393 ns | 0.0570 ns | 0.0127 |      80 B |
| EmptyApiLicense             |   3.259 ns |  1.5552 ns | 0.0852 ns | 0.0076 |      48 B |
| EmptyApiLink                |   3.964 ns |  1.4127 ns | 0.0774 ns | 0.0115 |      72 B |
| EmptyApiMediaType           |   4.257 ns |  1.9600 ns | 0.1074 ns | 0.0127 |      80 B |
| EmptyApiOAuthFlow           |   3.674 ns |  1.9886 ns | 0.1090 ns | 0.0102 |      64 B |
| EmptyApiOAuthFlows          |   3.733 ns |  1.9087 ns | 0.1046 ns | 0.0102 |      64 B |
| EmptyApiOperation           |  59.062 ns | 11.6699 ns | 0.6397 ns | 0.0598 |     376 B |
| EmptyApiParameter           |   4.813 ns |  1.5863 ns | 0.0869 ns | 0.0153 |      96 B |
| EmptyApiPathItem            |   3.777 ns |  1.4211 ns | 0.0779 ns | 0.0102 |      64 B |
| EmptyApiPaths               |  51.327 ns |  4.0622 ns | 0.2227 ns | 0.0395 |     248 B |
| EmptyApiRequestBody         |   3.281 ns |  0.0814 ns | 0.0045 ns | 0.0076 |      48 B |
| EmptyApiResponse            |   3.870 ns |  0.3077 ns | 0.0169 ns | 0.0102 |      64 B |
| EmptyApiResponses           |  50.940 ns | 23.4382 ns | 1.2847 ns | 0.0395 |     248 B |
| EmptyApiSchema              |  16.171 ns |  3.3551 ns | 0.1839 ns | 0.0663 |     416 B |
| EmptyApiSecurityRequirement |   8.834 ns |  2.7773 ns | 0.1522 ns | 0.0166 |     104 B |
| EmptyApiSecurityScheme      |   5.046 ns |  1.3737 ns | 0.0753 ns | 0.0166 |     104 B |
| EmptyApiServer              |   3.514 ns |  1.0344 ns | 0.0567 ns | 0.0089 |      56 B |
| EmptyApiServerVariable      |   3.307 ns |  1.9690 ns | 0.1079 ns | 0.0076 |      48 B |
| EmptyApiTag                 |   4.132 ns |  0.8657 ns | 0.0475 ns | 0.0115 |      72 B |
