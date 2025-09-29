```

BenchmarkDotNet v0.15.4, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.414
  [Host]   : .NET 8.0.20 (8.0.20, 8.0.2025.41914), X64 RyuJIT x86-64-v3
  ShortRun : .NET 8.0.20 (8.0.20, 8.0.2025.41914), X64 RyuJIT x86-64-v3

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean         | Error     | StdDev    | Gen0   | Allocated |
|---------------------------- |-------------:|----------:|----------:|-------:|----------:|
| EmptyApiCallback            |     9.530 ns |  3.544 ns | 0.1942 ns | 0.0019 |      32 B |
| EmptyApiComponents          |    16.342 ns | 13.284 ns | 0.7281 ns | 0.0062 |     104 B |
| EmptyApiContact             |    14.826 ns |  7.392 ns | 0.4052 ns | 0.0029 |      48 B |
| EmptyApiDiscriminator       |    11.966 ns | 24.387 ns | 1.3367 ns | 0.0024 |      40 B |
| EmptyDocument               | 1,100.509 ns | 52.317 ns | 2.8677 ns | 0.0668 |    1136 B |
| EmptyApiEncoding            |     8.472 ns |  4.256 ns | 0.2333 ns | 0.0033 |      56 B |
| EmptyApiExample             |     8.204 ns |  2.671 ns | 0.1464 ns | 0.0033 |      56 B |
| EmptyApiExternalDocs        |    14.383 ns |  7.357 ns | 0.4033 ns | 0.0024 |      40 B |
| EmptyApiHeader              |     8.423 ns |  4.947 ns | 0.2712 ns | 0.0048 |      80 B |
| EmptyApiInfo                |    19.354 ns |  2.912 ns | 0.1596 ns | 0.0048 |      80 B |
| EmptyApiLicense             |     9.103 ns |  2.892 ns | 0.1585 ns | 0.0029 |      48 B |
| EmptyApiLink                |    15.344 ns |  8.632 ns | 0.4732 ns | 0.0043 |      72 B |
| EmptyApiMediaType           |    13.353 ns |  5.876 ns | 0.3221 ns | 0.0033 |      56 B |
| EmptyApiOAuthFlow           |    16.937 ns | 23.422 ns | 1.2839 ns | 0.0033 |      56 B |
| EmptyApiOAuthFlows          |    13.194 ns | 23.322 ns | 1.2783 ns | 0.0033 |      56 B |
| EmptyApiOperation           |    70.995 ns | 17.112 ns | 0.9380 ns | 0.0224 |     376 B |
| EmptyApiParameter           |    18.672 ns | 10.753 ns | 0.5894 ns | 0.0057 |      96 B |
| EmptyApiPathItem            |    18.164 ns |  8.096 ns | 0.4438 ns | 0.0038 |      64 B |
| EmptyApiPaths               |    63.171 ns |  1.055 ns | 0.0578 ns | 0.0148 |     248 B |
| EmptyApiRequestBody         |    13.143 ns |  8.962 ns | 0.4913 ns | 0.0029 |      48 B |
| EmptyApiResponse            |     7.853 ns |  1.278 ns | 0.0700 ns | 0.0038 |      64 B |
| EmptyApiResponses           |    62.085 ns |  8.772 ns | 0.4808 ns | 0.0148 |     248 B |
| EmptyApiSchema              |    19.546 ns |  4.037 ns | 0.2213 ns | 0.0244 |     408 B |
| EmptyApiSecurityRequirement |    15.817 ns |  4.579 ns | 0.2510 ns | 0.0062 |     104 B |
| EmptyApiSecurityScheme      |    13.738 ns | 25.244 ns | 1.3837 ns | 0.0052 |      88 B |
| EmptyApiServer              |    18.163 ns | 24.685 ns | 1.3531 ns | 0.0033 |      56 B |
| EmptyApiServerVariable      |    15.036 ns | 23.813 ns | 1.3053 ns | 0.0029 |      48 B |
| EmptyApiTag                 |    13.712 ns |  6.139 ns | 0.3365 ns | 0.0029 |      48 B |
