```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.400
  [Host]   : .NET 8.0.30 (8.0.30, 8.0.3026.36720), X64 RyuJIT x86-64-v3
  ShortRun : .NET 8.0.30 (8.0.30, 8.0.3026.36720), X64 RyuJIT x86-64-v3

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean         | Error     | StdDev    | Gen0   | Allocated |
|---------------------------- |-------------:|----------:|----------:|-------:|----------:|
| EmptyApiCallback            |    13.883 ns | 31.303 ns | 1.7158 ns | 0.0019 |      32 B |
| EmptyApiComponents          |    11.486 ns |  8.012 ns | 0.4391 ns | 0.0067 |     112 B |
| EmptyApiContact             |    12.119 ns | 29.862 ns | 1.6368 ns | 0.0029 |      48 B |
| EmptyApiDiscriminator       |    14.477 ns | 16.530 ns | 0.9061 ns | 0.0029 |      48 B |
| EmptyDocument               | 1,159.023 ns | 41.293 ns | 2.2634 ns | 0.0687 |    1160 B |
| EmptyApiEncoding            |    14.032 ns | 13.067 ns | 0.7163 ns | 0.0048 |      80 B |
| EmptyApiExample             |    14.136 ns |  9.627 ns | 0.5277 ns | 0.0043 |      72 B |
| EmptyApiExternalDocs        |    13.615 ns |  5.768 ns | 0.3161 ns | 0.0024 |      40 B |
| EmptyApiHeader              |    15.168 ns | 23.385 ns | 1.2818 ns | 0.0048 |      80 B |
| EmptyApiInfo                |    11.988 ns |  8.455 ns | 0.4634 ns | 0.0048 |      80 B |
| EmptyApiLicense             |    15.973 ns | 11.512 ns | 0.6310 ns | 0.0029 |      48 B |
| EmptyApiLink                |    11.352 ns | 20.723 ns | 1.1359 ns | 0.0043 |      72 B |
| EmptyApiMediaType           |    12.567 ns | 17.009 ns | 0.9323 ns | 0.0048 |      80 B |
| EmptyApiOAuthFlow           |    15.797 ns | 20.149 ns | 1.1044 ns | 0.0038 |      64 B |
| EmptyApiOAuthFlows          |    15.136 ns | 55.718 ns | 3.0541 ns | 0.0038 |      64 B |
| EmptyApiOperation           |    80.422 ns | 10.276 ns | 0.5633 ns | 0.0224 |     376 B |
| EmptyApiParameter           |    11.119 ns | 12.830 ns | 0.7032 ns | 0.0057 |      96 B |
| EmptyApiPathItem            |    15.189 ns | 55.661 ns | 3.0510 ns | 0.0038 |      64 B |
| EmptyApiPaths               |    68.082 ns | 11.260 ns | 0.6172 ns | 0.0148 |     248 B |
| EmptyApiRequestBody         |    10.824 ns | 10.037 ns | 0.5501 ns | 0.0029 |      48 B |
| EmptyApiResponse            |    14.045 ns | 39.447 ns | 2.1622 ns | 0.0038 |      64 B |
| EmptyApiResponses           |    64.353 ns | 13.642 ns | 0.7478 ns | 0.0148 |     248 B |
| EmptyApiSchema              |    25.053 ns |  3.922 ns | 0.2150 ns | 0.0306 |     512 B |
| EmptyApiSecurityRequirement |    17.377 ns |  2.781 ns | 0.1524 ns | 0.0062 |     104 B |
| EmptyApiSecurityScheme      |    10.360 ns |  1.932 ns | 0.1059 ns | 0.0062 |     104 B |
| EmptyApiServer              |    15.258 ns | 20.720 ns | 1.1357 ns | 0.0033 |      56 B |
| EmptyApiServerVariable      |     8.365 ns |  1.520 ns | 0.0833 ns | 0.0029 |      48 B |
| EmptyApiTag                 |    14.267 ns | 10.334 ns | 0.5665 ns | 0.0043 |      72 B |
