```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]   : .NET 8.0.28 (8.0.28, 8.0.2826.26413), X64 RyuJIT x86-64-v3
  ShortRun : .NET 8.0.28 (8.0.28, 8.0.2826.26413), X64 RyuJIT x86-64-v3

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean         | Error     | StdDev    | Gen0   | Allocated |
|---------------------------- |-------------:|----------:|----------:|-------:|----------:|
| EmptyApiCallback            |    14.077 ns |  5.879 ns | 0.3223 ns | 0.0019 |      32 B |
| EmptyApiComponents          |    14.353 ns | 27.787 ns | 1.5231 ns | 0.0067 |     112 B |
| EmptyApiContact             |     8.820 ns |  4.774 ns | 0.2617 ns | 0.0029 |      48 B |
| EmptyApiDiscriminator       |    12.366 ns | 17.252 ns | 0.9456 ns | 0.0029 |      48 B |
| EmptyDocument               | 1,227.975 ns | 78.094 ns | 4.2806 ns | 0.0668 |    1144 B |
| EmptyApiEncoding            |    16.017 ns | 39.797 ns | 2.1814 ns | 0.0048 |      80 B |
| EmptyApiExample             |    13.024 ns |  4.635 ns | 0.2541 ns | 0.0043 |      72 B |
| EmptyApiExternalDocs        |    14.738 ns | 18.712 ns | 1.0257 ns | 0.0024 |      40 B |
| EmptyApiHeader              |    13.615 ns | 12.637 ns | 0.6927 ns | 0.0048 |      80 B |
| EmptyApiInfo                |    13.339 ns | 36.579 ns | 2.0050 ns | 0.0048 |      80 B |
| EmptyApiLicense             |    11.728 ns | 27.400 ns | 1.5019 ns | 0.0029 |      48 B |
| EmptyApiLink                |    14.362 ns | 51.417 ns | 2.8183 ns | 0.0043 |      72 B |
| EmptyApiMediaType           |    11.310 ns |  2.030 ns | 0.1113 ns | 0.0048 |      80 B |
| EmptyApiOAuthFlow           |     8.204 ns |  2.441 ns | 0.1338 ns | 0.0038 |      64 B |
| EmptyApiOAuthFlows          |    11.544 ns | 20.185 ns | 1.1064 ns | 0.0038 |      64 B |
| EmptyApiOperation           |    76.914 ns | 36.381 ns | 1.9942 ns | 0.0224 |     376 B |
| EmptyApiParameter           |    12.591 ns | 24.321 ns | 1.3331 ns | 0.0057 |      96 B |
| EmptyApiPathItem            |    15.782 ns | 25.659 ns | 1.4064 ns | 0.0038 |      64 B |
| EmptyApiPaths               |    60.798 ns | 12.323 ns | 0.6755 ns | 0.0148 |     248 B |
| EmptyApiRequestBody         |     9.189 ns | 14.175 ns | 0.7770 ns | 0.0029 |      48 B |
| EmptyApiResponse            |    14.850 ns | 20.664 ns | 1.1327 ns | 0.0038 |      64 B |
| EmptyApiResponses           |    64.813 ns | 29.839 ns | 1.6356 ns | 0.0148 |     248 B |
| EmptyApiSchema              |    22.176 ns |  8.769 ns | 0.4807 ns | 0.0306 |     512 B |
| EmptyApiSecurityRequirement |    16.838 ns | 11.238 ns | 0.6160 ns | 0.0062 |     104 B |
| EmptyApiSecurityScheme      |    11.860 ns | 15.234 ns | 0.8350 ns | 0.0062 |     104 B |
| EmptyApiServer              |     7.778 ns |  3.765 ns | 0.2063 ns | 0.0033 |      56 B |
| EmptyApiServerVariable      |    13.466 ns | 11.117 ns | 0.6093 ns | 0.0029 |      48 B |
| EmptyApiTag                 |     8.794 ns |  5.167 ns | 0.2832 ns | 0.0043 |      72 B |
