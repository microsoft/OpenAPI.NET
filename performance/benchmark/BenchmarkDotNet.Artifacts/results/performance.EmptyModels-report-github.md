```

BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.417
  [Host]   : .NET 8.0.23 (8.0.23, 8.0.2325.60607), X64 RyuJIT x86-64-v3
  ShortRun : .NET 8.0.23 (8.0.23, 8.0.2325.60607), X64 RyuJIT x86-64-v3

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean         | Error       | StdDev    | Gen0   | Allocated |
|---------------------------- |-------------:|------------:|----------:|-------:|----------:|
| EmptyApiCallback            |     9.685 ns |  17.7937 ns | 0.9753 ns | 0.0019 |      32 B |
| EmptyApiComponents          |    11.627 ns |  10.7777 ns | 0.5908 ns | 0.0062 |     104 B |
| EmptyApiContact             |    14.086 ns |  14.8829 ns | 0.8158 ns | 0.0029 |      48 B |
| EmptyApiDiscriminator       |    11.069 ns |   6.1582 ns | 0.3376 ns | 0.0024 |      40 B |
| EmptyDocument               | 1,483.353 ns | 138.3191 ns | 7.5817 ns | 0.0668 |    1136 B |
| EmptyApiEncoding            |    11.392 ns |   8.8333 ns | 0.4842 ns | 0.0033 |      56 B |
| EmptyApiExample             |    11.386 ns |  18.8529 ns | 1.0334 ns | 0.0033 |      56 B |
| EmptyApiExternalDocs        |    12.680 ns |  19.1225 ns | 1.0482 ns | 0.0024 |      40 B |
| EmptyApiHeader              |     9.105 ns |   5.0905 ns | 0.2790 ns | 0.0048 |      80 B |
| EmptyApiInfo                |    10.826 ns |   4.2315 ns | 0.2319 ns | 0.0048 |      80 B |
| EmptyApiLicense             |    11.953 ns |  23.1808 ns | 1.2706 ns | 0.0029 |      48 B |
| EmptyApiLink                |     8.932 ns |   2.4237 ns | 0.1328 ns | 0.0043 |      72 B |
| EmptyApiMediaType           |    16.192 ns |   2.7983 ns | 0.1534 ns | 0.0033 |      56 B |
| EmptyApiOAuthFlow           |    12.778 ns |  12.4107 ns | 0.6803 ns | 0.0033 |      56 B |
| EmptyApiOAuthFlows          |     7.628 ns |   2.4289 ns | 0.1331 ns | 0.0033 |      56 B |
| EmptyApiOperation           |    73.545 ns |  34.5706 ns | 1.8949 ns | 0.0224 |     376 B |
| EmptyApiParameter           |    15.957 ns |  33.0382 ns | 1.8109 ns | 0.0057 |      96 B |
| EmptyApiPathItem            |     8.331 ns |   8.9214 ns | 0.4890 ns | 0.0038 |      64 B |
| EmptyApiPaths               |    62.495 ns |   0.3726 ns | 0.0204 ns | 0.0148 |     248 B |
| EmptyApiRequestBody         |    13.819 ns |  28.7503 ns | 1.5759 ns | 0.0029 |      48 B |
| EmptyApiResponse            |    14.121 ns |  12.2665 ns | 0.6724 ns | 0.0033 |      56 B |
| EmptyApiResponses           |    57.201 ns |   3.1653 ns | 0.1735 ns | 0.0148 |     248 B |
| EmptyApiSchema              |    27.815 ns |  19.2152 ns | 1.0532 ns | 0.0249 |     416 B |
| EmptyApiSecurityRequirement |    15.751 ns |  10.7910 ns | 0.5915 ns | 0.0062 |     104 B |
| EmptyApiSecurityScheme      |    11.171 ns |   3.6777 ns | 0.2016 ns | 0.0053 |      88 B |
| EmptyApiServer              |    13.892 ns |   9.7265 ns | 0.5331 ns | 0.0029 |      48 B |
| EmptyApiServerVariable      |     7.507 ns |   3.2572 ns | 0.1785 ns | 0.0029 |      48 B |
| EmptyApiTag                 |    11.666 ns |   9.0815 ns | 0.4978 ns | 0.0029 |      48 B |
