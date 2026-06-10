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
| EmptyApiCallback            |   2.105 ns |  0.2352 ns | 0.0129 ns | 0.0077 |      32 B |
| EmptyApiComponents          |   4.055 ns |  0.3474 ns | 0.0190 ns | 0.0249 |     104 B |
| EmptyApiContact             |   2.558 ns |  0.4430 ns | 0.0243 ns | 0.0115 |      48 B |
| EmptyApiDiscriminator       |   2.305 ns |  0.1176 ns | 0.0064 ns | 0.0096 |      40 B |
| EmptyDocument               | 400.660 ns | 15.3393 ns | 0.8408 ns | 0.2713 |    1136 B |
| EmptyApiEncoding            |   2.731 ns |  0.1507 ns | 0.0083 ns | 0.0134 |      56 B |
| EmptyApiExample             |   2.756 ns |  0.6696 ns | 0.0367 ns | 0.0134 |      56 B |
| EmptyApiExternalDocs        |   2.360 ns |  0.0652 ns | 0.0036 ns | 0.0096 |      40 B |
| EmptyApiHeader              |   3.345 ns |  0.2151 ns | 0.0118 ns | 0.0191 |      80 B |
| EmptyApiInfo                |   3.458 ns |  0.2971 ns | 0.0163 ns | 0.0191 |      80 B |
| EmptyApiLicense             |   2.545 ns |  0.4869 ns | 0.0267 ns | 0.0115 |      48 B |
| EmptyApiLink                |   3.196 ns |  0.2138 ns | 0.0117 ns | 0.0172 |      72 B |
| EmptyApiMediaType           |   2.722 ns |  0.1644 ns | 0.0090 ns | 0.0134 |      56 B |
| EmptyApiOAuthFlow           |   2.734 ns |  0.8261 ns | 0.0453 ns | 0.0134 |      56 B |
| EmptyApiOAuthFlows          |   2.821 ns |  0.7838 ns | 0.0430 ns | 0.0134 |      56 B |
| EmptyApiOperation           |  64.248 ns |  5.4103 ns | 0.2966 ns | 0.0899 |     376 B |
| EmptyApiParameter           |   4.088 ns |  2.8634 ns | 0.1570 ns | 0.0229 |      96 B |
| EmptyApiPathItem            |   3.053 ns |  1.4811 ns | 0.0812 ns | 0.0153 |      64 B |
| EmptyApiPaths               |  57.666 ns |  8.6003 ns | 0.4714 ns | 0.0592 |     248 B |
| EmptyApiRequestBody         |   2.562 ns |  0.4200 ns | 0.0230 ns | 0.0115 |      48 B |
| EmptyApiResponse            |   2.725 ns |  0.4427 ns | 0.0243 ns | 0.0134 |      56 B |
| EmptyApiResponses           |  58.058 ns |  8.6859 ns | 0.4761 ns | 0.0592 |     248 B |
| EmptyApiSchema              |  14.647 ns |  2.5004 ns | 0.1371 ns | 0.1224 |     512 B |
| EmptyApiSecurityRequirement |   9.232 ns |  1.5638 ns | 0.0857 ns | 0.0249 |     104 B |
| EmptyApiSecurityScheme      |   3.648 ns |  1.5011 ns | 0.0823 ns | 0.0210 |      88 B |
| EmptyApiServer              |   2.683 ns |  2.9452 ns | 0.1614 ns | 0.0115 |      48 B |
| EmptyApiServerVariable      |   2.520 ns |  0.0565 ns | 0.0031 ns | 0.0115 |      48 B |
| EmptyApiTag                 |   2.532 ns |  0.4817 ns | 0.0264 ns | 0.0115 |      48 B |
