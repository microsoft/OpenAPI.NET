```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]   : .NET 8.0.29 (8.0.29, 8.0.2926.32403), X64 RyuJIT x86-64-v3
  ShortRun : .NET 8.0.29 (8.0.29, 8.0.2926.32403), X64 RyuJIT x86-64-v3

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean         | Error      | StdDev    | Gen0   | Allocated |
|---------------------------- |-------------:|-----------:|----------:|-------:|----------:|
| EmptyApiCallback            |     7.055 ns |  3.0013 ns | 0.1645 ns | 0.0019 |      32 B |
| EmptyApiComponents          |     8.371 ns |  0.2594 ns | 0.0142 ns | 0.0067 |     112 B |
| EmptyApiContact             |     6.418 ns |  2.1145 ns | 0.1159 ns | 0.0029 |      48 B |
| EmptyApiDiscriminator       |     7.079 ns |  1.5923 ns | 0.0873 ns | 0.0029 |      48 B |
| EmptyDocument               | 1,405.141 ns | 26.0523 ns | 1.4280 ns | 0.0687 |    1160 B |
| EmptyApiEncoding            |     8.256 ns |  3.1786 ns | 0.1742 ns | 0.0048 |      80 B |
| EmptyApiExample             |     6.972 ns |  1.1002 ns | 0.0603 ns | 0.0043 |      72 B |
| EmptyApiExternalDocs        |     6.532 ns |  2.1543 ns | 0.1181 ns | 0.0024 |      40 B |
| EmptyApiHeader              |     7.923 ns |  2.7927 ns | 0.1531 ns | 0.0048 |      80 B |
| EmptyApiInfo                |     7.500 ns |  0.1012 ns | 0.0055 ns | 0.0048 |      80 B |
| EmptyApiLicense             |     6.742 ns |  0.1901 ns | 0.0104 ns | 0.0029 |      48 B |
| EmptyApiLink                |     7.733 ns |  3.1664 ns | 0.1736 ns | 0.0043 |      72 B |
| EmptyApiMediaType           |     7.230 ns |  1.0756 ns | 0.0590 ns | 0.0048 |      80 B |
| EmptyApiOAuthFlow           |     7.297 ns |  0.3704 ns | 0.0203 ns | 0.0038 |      64 B |
| EmptyApiOAuthFlows          |     7.009 ns |  3.3255 ns | 0.1823 ns | 0.0038 |      64 B |
| EmptyApiOperation           |    73.739 ns |  6.8762 ns | 0.3769 ns | 0.0224 |     376 B |
| EmptyApiParameter           |     8.865 ns |  3.1255 ns | 0.1713 ns | 0.0057 |      96 B |
| EmptyApiPathItem            |     6.705 ns |  2.2071 ns | 0.1210 ns | 0.0038 |      64 B |
| EmptyApiPaths               |    60.599 ns |  1.2903 ns | 0.0707 ns | 0.0148 |     248 B |
| EmptyApiRequestBody         |     6.435 ns |  0.8979 ns | 0.0492 ns | 0.0029 |      48 B |
| EmptyApiResponse            |     6.886 ns |  1.2457 ns | 0.0683 ns | 0.0038 |      64 B |
| EmptyApiResponses           |    63.318 ns |  0.6100 ns | 0.0334 ns | 0.0148 |     248 B |
| EmptyApiSchema              |    18.465 ns |  1.9530 ns | 0.1071 ns | 0.0306 |     512 B |
| EmptyApiSecurityRequirement |    15.559 ns |  2.8850 ns | 0.1581 ns | 0.0062 |     104 B |
| EmptyApiSecurityScheme      |     7.990 ns |  4.1668 ns | 0.2284 ns | 0.0062 |     104 B |
| EmptyApiServer              |     6.713 ns |  3.6538 ns | 0.2003 ns | 0.0033 |      56 B |
| EmptyApiServerVariable      |     6.756 ns |  3.1240 ns | 0.1712 ns | 0.0029 |      48 B |
| EmptyApiTag                 |     7.248 ns |  3.7924 ns | 0.2079 ns | 0.0043 |      72 B |
