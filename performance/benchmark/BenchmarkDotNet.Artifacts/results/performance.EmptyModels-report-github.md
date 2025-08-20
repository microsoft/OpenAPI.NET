```

BenchmarkDotNet v0.15.2, Linux Ubuntu 24.04.2 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.413
  [Host]   : .NET 8.0.19 (8.0.1925.36514), X64 RyuJIT AVX2
  ShortRun : .NET 8.0.19 (8.0.1925.36514), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean         | Error     | StdDev    | Gen0   | Allocated |
|---------------------------- |-------------:|----------:|----------:|-------:|----------:|
| EmptyApiCallback            |    16.112 ns |  5.784 ns | 0.3170 ns | 0.0019 |      32 B |
| EmptyApiComponents          |    19.110 ns | 22.757 ns | 1.2474 ns | 0.0062 |     104 B |
| EmptyApiContact             |     9.642 ns | 11.245 ns | 0.6164 ns | 0.0029 |      48 B |
| EmptyApiDiscriminator       |    12.330 ns |  5.798 ns | 0.3178 ns | 0.0024 |      40 B |
| EmptyDocument               | 1,151.986 ns | 65.900 ns | 3.6122 ns | 0.0668 |    1136 B |
| EmptyApiEncoding            |    10.202 ns |  4.448 ns | 0.2438 ns | 0.0033 |      56 B |
| EmptyApiExample             |    13.840 ns | 18.878 ns | 1.0348 ns | 0.0033 |      56 B |
| EmptyApiExternalDocs        |    13.485 ns | 20.898 ns | 1.1455 ns | 0.0024 |      40 B |
| EmptyApiHeader              |    14.911 ns | 21.281 ns | 1.1665 ns | 0.0048 |      80 B |
| EmptyApiInfo                |    17.285 ns | 11.383 ns | 0.6239 ns | 0.0048 |      80 B |
| EmptyApiLicense             |    15.353 ns | 15.752 ns | 0.8634 ns | 0.0029 |      48 B |
| EmptyApiLink                |    13.631 ns | 16.538 ns | 0.9065 ns | 0.0043 |      72 B |
| EmptyApiMediaType           |     8.334 ns |  3.227 ns | 0.1769 ns | 0.0033 |      56 B |
| EmptyApiOAuthFlow           |    10.103 ns |  8.885 ns | 0.4870 ns | 0.0033 |      56 B |
| EmptyApiOAuthFlows          |    11.514 ns |  6.548 ns | 0.3589 ns | 0.0033 |      56 B |
| EmptyApiOperation           |    85.834 ns |  6.257 ns | 0.3430 ns | 0.0224 |     376 B |
| EmptyApiParameter           |    18.951 ns | 23.964 ns | 1.3135 ns | 0.0057 |      96 B |
| EmptyApiPathItem            |    16.662 ns | 19.083 ns | 1.0460 ns | 0.0038 |      64 B |
| EmptyApiPaths               |    63.937 ns | 18.114 ns | 0.9929 ns | 0.0148 |     248 B |
| EmptyApiRequestBody         |    10.680 ns | 16.237 ns | 0.8900 ns | 0.0029 |      48 B |
| EmptyApiResponse            |    13.962 ns | 34.919 ns | 1.9140 ns | 0.0033 |      56 B |
| EmptyApiResponses           |    59.299 ns | 11.181 ns | 0.6129 ns | 0.0148 |     248 B |
| EmptyApiSchema              |    22.670 ns | 30.835 ns | 1.6902 ns | 0.0244 |     408 B |
| EmptyApiSecurityRequirement |    16.913 ns |  2.287 ns | 0.1254 ns | 0.0062 |     104 B |
| EmptyApiSecurityScheme      |    14.815 ns | 44.214 ns | 2.4235 ns | 0.0052 |      88 B |
| EmptyApiServer              |    14.207 ns | 19.311 ns | 1.0585 ns | 0.0029 |      48 B |
| EmptyApiServerVariable      |    13.801 ns | 35.161 ns | 1.9273 ns | 0.0029 |      48 B |
| EmptyApiTag                 |     8.223 ns |  4.013 ns | 0.2200 ns | 0.0029 |      48 B |
