```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]   : .NET 8.0.28 (8.0.28, 8.0.2826.26413), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.28 (8.0.28, 8.0.2826.26413), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean       | Error     | StdDev    | Gen0   | Allocated |
|---------------------------- |-----------:|----------:|----------:|-------:|----------:|
| EmptyApiCallback            |   6.662 ns |  1.848 ns | 0.1013 ns | 0.0013 |      32 B |
| EmptyApiComponents          |  13.913 ns |  3.304 ns | 0.1811 ns | 0.0044 |     112 B |
| EmptyApiContact             |   8.218 ns |  1.824 ns | 0.1000 ns | 0.0019 |      48 B |
| EmptyApiDiscriminator       |   8.293 ns |  1.519 ns | 0.0833 ns | 0.0019 |      48 B |
| EmptyDocument               | 951.094 ns | 38.673 ns | 2.1198 ns | 0.0439 |    1144 B |
| EmptyApiEncoding            |  10.910 ns | 12.936 ns | 0.7091 ns | 0.0032 |      80 B |
| EmptyApiExample             |  11.240 ns |  2.607 ns | 0.1429 ns | 0.0029 |      72 B |
| EmptyApiExternalDocs        |   8.120 ns |  3.181 ns | 0.1743 ns | 0.0016 |      40 B |
| EmptyApiHeader              |  11.455 ns |  1.724 ns | 0.0945 ns | 0.0032 |      80 B |
| EmptyApiInfo                |  10.794 ns |  2.680 ns | 0.1469 ns | 0.0032 |      80 B |
| EmptyApiLicense             |   8.041 ns |  1.834 ns | 0.1005 ns | 0.0019 |      48 B |
| EmptyApiLink                |  10.987 ns |  7.384 ns | 0.4048 ns | 0.0029 |      72 B |
| EmptyApiMediaType           |  11.908 ns |  1.695 ns | 0.0929 ns | 0.0032 |      80 B |
| EmptyApiOAuthFlow           |   8.732 ns |  2.655 ns | 0.1455 ns | 0.0025 |      64 B |
| EmptyApiOAuthFlows          |   9.398 ns |  2.573 ns | 0.1410 ns | 0.0025 |      64 B |
| EmptyApiOperation           |  84.554 ns |  7.611 ns | 0.4172 ns | 0.0149 |     376 B |
| EmptyApiParameter           |  11.078 ns |  5.033 ns | 0.2759 ns | 0.0038 |      96 B |
| EmptyApiPathItem            |   8.602 ns |  1.244 ns | 0.0682 ns | 0.0025 |      64 B |
| EmptyApiPaths               |  73.217 ns |  8.493 ns | 0.4655 ns | 0.0098 |     248 B |
| EmptyApiRequestBody         |   9.255 ns |  1.496 ns | 0.0820 ns | 0.0019 |      48 B |
| EmptyApiResponse            |   9.733 ns |  2.739 ns | 0.1501 ns | 0.0025 |      64 B |
| EmptyApiResponses           |  76.603 ns |  7.301 ns | 0.4002 ns | 0.0098 |     248 B |
| EmptyApiSchema              |  35.221 ns | 46.877 ns | 2.5695 ns | 0.0204 |     512 B |
| EmptyApiSecurityRequirement |  18.422 ns |  3.708 ns | 0.2033 ns | 0.0041 |     104 B |
| EmptyApiSecurityScheme      |  11.513 ns |  4.587 ns | 0.2514 ns | 0.0041 |     104 B |
| EmptyApiServer              |   9.357 ns |  2.432 ns | 0.1333 ns | 0.0022 |      56 B |
| EmptyApiServerVariable      |   8.603 ns |  6.016 ns | 0.3297 ns | 0.0019 |      48 B |
| EmptyApiTag                 |   9.969 ns |  4.173 ns | 0.2288 ns | 0.0029 |      72 B |
