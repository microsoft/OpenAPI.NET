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
| EmptyApiCallback            |   1.542 ns |  2.8290 ns | 0.1551 ns | 0.0077 |      32 B |
| EmptyApiComponents          |   2.539 ns |  0.1677 ns | 0.0092 ns | 0.0249 |     104 B |
| EmptyApiContact             |   1.941 ns |  0.6764 ns | 0.0371 ns | 0.0115 |      48 B |
| EmptyApiDiscriminator       |   1.780 ns |  0.7044 ns | 0.0386 ns | 0.0096 |      40 B |
| EmptyDocument               | 297.968 ns | 21.6042 ns | 1.1842 ns | 0.2751 |    1152 B |
| EmptyApiEncoding            |   2.146 ns |  1.7538 ns | 0.0961 ns | 0.0134 |      56 B |
| EmptyApiExample             |   1.876 ns |  6.4557 ns | 0.3539 ns | 0.0134 |      56 B |
| EmptyApiExternalDocs        |   1.729 ns |  0.3814 ns | 0.0209 ns | 0.0096 |      40 B |
| EmptyApiHeader              |   2.544 ns |  0.3848 ns | 0.0211 ns | 0.0191 |      80 B |
| EmptyApiInfo                |   2.562 ns |  1.2949 ns | 0.0710 ns | 0.0191 |      80 B |
| EmptyApiLicense             |   1.869 ns |  0.3055 ns | 0.0167 ns | 0.0115 |      48 B |
| EmptyApiLink                |   2.369 ns |  0.6096 ns | 0.0334 ns | 0.0172 |      72 B |
| EmptyApiMediaType           |   2.095 ns |  0.2404 ns | 0.0132 ns | 0.0134 |      56 B |
| EmptyApiOAuthFlow           |   2.013 ns |  0.2928 ns | 0.0160 ns | 0.0134 |      56 B |
| EmptyApiOAuthFlows          |   2.041 ns |  0.3166 ns | 0.0174 ns | 0.0134 |      56 B |
| EmptyApiOperation           |  47.686 ns |  6.2026 ns | 0.3400 ns | 0.0899 |     376 B |
| EmptyApiParameter           |   2.872 ns |  0.4280 ns | 0.0235 ns | 0.0230 |      96 B |
| EmptyApiPathItem            |   2.200 ns |  0.0822 ns | 0.0045 ns | 0.0153 |      64 B |
| EmptyApiPaths               |  42.168 ns |  4.2427 ns | 0.2326 ns | 0.0592 |     248 B |
| EmptyApiRequestBody         |   1.914 ns |  0.7504 ns | 0.0411 ns | 0.0115 |      48 B |
| EmptyApiResponse            |   2.069 ns |  0.5045 ns | 0.0277 ns | 0.0134 |      56 B |
| EmptyApiResponses           |  43.854 ns | 14.2551 ns | 0.7814 ns | 0.0592 |     248 B |
| EmptyApiSchema              |  10.891 ns |  0.3304 ns | 0.0181 ns | 0.1224 |     512 B |
| EmptyApiSecurityRequirement |   6.933 ns |  2.7969 ns | 0.1533 ns | 0.0249 |     104 B |
| EmptyApiSecurityScheme      |   2.687 ns |  0.3364 ns | 0.0184 ns | 0.0210 |      88 B |
| EmptyApiServer              |   1.906 ns |  0.2675 ns | 0.0147 ns | 0.0115 |      48 B |
| EmptyApiServerVariable      |   1.903 ns |  0.6081 ns | 0.0333 ns | 0.0115 |      48 B |
| EmptyApiTag                 |   1.871 ns |  0.0675 ns | 0.0037 ns | 0.0115 |      48 B |
