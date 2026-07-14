```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]   : .NET 8.0.28 (8.0.28, 8.0.2826.26413), X64 RyuJIT x86-64-v3
  ShortRun : .NET 8.0.28 (8.0.28, 8.0.2826.26413), X64 RyuJIT x86-64-v3

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean         | Error       | StdDev     | Gen0   | Allocated |
|---------------------------- |-------------:|------------:|-----------:|-------:|----------:|
| EmptyApiCallback            |    13.559 ns |  18.4262 ns |  1.0100 ns | 0.0019 |      32 B |
| EmptyApiComponents          |    16.536 ns |  46.6608 ns |  2.5576 ns | 0.0067 |     112 B |
| EmptyApiContact             |    13.787 ns |  33.7542 ns |  1.8502 ns | 0.0029 |      48 B |
| EmptyApiDiscriminator       |    10.289 ns |   7.6409 ns |  0.4188 ns | 0.0029 |      48 B |
| EmptyDocument               | 1,212.262 ns | 228.0732 ns | 12.5015 ns | 0.0687 |    1160 B |
| EmptyApiEncoding            |    16.019 ns |   8.9064 ns |  0.4882 ns | 0.0048 |      80 B |
| EmptyApiExample             |     7.675 ns |   0.0519 ns |  0.0028 ns | 0.0043 |      72 B |
| EmptyApiExternalDocs        |    11.095 ns |  17.5104 ns |  0.9598 ns | 0.0024 |      40 B |
| EmptyApiHeader              |    12.965 ns |  15.0039 ns |  0.8224 ns | 0.0048 |      80 B |
| EmptyApiInfo                |     9.089 ns |   2.0775 ns |  0.1139 ns | 0.0048 |      80 B |
| EmptyApiLicense             |    15.303 ns |  25.4293 ns |  1.3939 ns | 0.0029 |      48 B |
| EmptyApiLink                |    11.312 ns |   9.0124 ns |  0.4940 ns | 0.0043 |      72 B |
| EmptyApiMediaType           |    13.833 ns |  20.3302 ns |  1.1144 ns | 0.0048 |      80 B |
| EmptyApiOAuthFlow           |    10.090 ns |  17.9927 ns |  0.9862 ns | 0.0038 |      64 B |
| EmptyApiOAuthFlows          |     8.289 ns |   5.9660 ns |  0.3270 ns | 0.0038 |      64 B |
| EmptyApiOperation           |    77.781 ns |  16.9606 ns |  0.9297 ns | 0.0224 |     376 B |
| EmptyApiParameter           |    15.831 ns |  20.4125 ns |  1.1189 ns | 0.0057 |      96 B |
| EmptyApiPathItem            |    10.735 ns |   4.5651 ns |  0.2502 ns | 0.0038 |      64 B |
| EmptyApiPaths               |    61.156 ns |  10.0591 ns |  0.5514 ns | 0.0148 |     248 B |
| EmptyApiRequestBody         |    10.578 ns |  12.3590 ns |  0.6774 ns | 0.0029 |      48 B |
| EmptyApiResponse            |    12.502 ns |   3.6616 ns |  0.2007 ns | 0.0038 |      64 B |
| EmptyApiResponses           |    62.856 ns |  27.1422 ns |  1.4878 ns | 0.0148 |     248 B |
| EmptyApiSchema              |    23.426 ns |   8.6128 ns |  0.4721 ns | 0.0306 |     512 B |
| EmptyApiSecurityRequirement |    16.151 ns |  20.2796 ns |  1.1116 ns | 0.0062 |     104 B |
| EmptyApiSecurityScheme      |    16.169 ns |  35.9051 ns |  1.9681 ns | 0.0062 |     104 B |
| EmptyApiServer              |     8.241 ns |   7.0772 ns |  0.3879 ns | 0.0033 |      56 B |
| EmptyApiServerVariable      |    13.641 ns |  52.1485 ns |  2.8584 ns | 0.0029 |      48 B |
| EmptyApiTag                 |    12.151 ns |  25.0675 ns |  1.3740 ns | 0.0043 |      72 B |
