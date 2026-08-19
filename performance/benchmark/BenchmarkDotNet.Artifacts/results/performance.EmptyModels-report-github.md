```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
INTEL XEON PLATINUM 8573C 3.39GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.400
  [Host]   : .NET 8.0.30 (8.0.30, 8.0.3026.36720), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.30 (8.0.30, 8.0.3026.36720), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean       | Error       | StdDev    | Gen0   | Allocated |
|---------------------------- |-----------:|------------:|----------:|-------:|----------:|
| EmptyApiCallback            |   5.792 ns |   1.9141 ns | 0.1049 ns | 0.0004 |      32 B |
| EmptyApiComponents          |   8.615 ns |   2.3227 ns | 0.1273 ns | 0.0013 |     112 B |
| EmptyApiContact             |   6.746 ns |   6.7442 ns | 0.3697 ns | 0.0006 |      48 B |
| EmptyApiDiscriminator       |   5.808 ns |   6.7627 ns | 0.3707 ns | 0.0006 |      48 B |
| EmptyDocument               | 761.369 ns | 132.5084 ns | 7.2632 ns | 0.0134 |    1160 B |
| EmptyApiEncoding            |   6.299 ns |   2.6409 ns | 0.1448 ns | 0.0010 |      80 B |
| EmptyApiExample             |   6.907 ns |   1.2871 ns | 0.0706 ns | 0.0008 |      72 B |
| EmptyApiExternalDocs        |   5.874 ns |   2.9021 ns | 0.1591 ns | 0.0005 |      40 B |
| EmptyApiHeader              |   6.334 ns |   1.7008 ns | 0.0932 ns | 0.0010 |      80 B |
| EmptyApiInfo                |   7.055 ns |   1.5613 ns | 0.0856 ns | 0.0010 |      80 B |
| EmptyApiLicense             |   5.130 ns |   0.1562 ns | 0.0086 ns | 0.0006 |      48 B |
| EmptyApiLink                |   6.742 ns |   1.9763 ns | 0.1083 ns | 0.0008 |      72 B |
| EmptyApiMediaType           |   6.076 ns |   1.1051 ns | 0.0606 ns | 0.0010 |      80 B |
| EmptyApiOAuthFlow           |   6.654 ns |   1.2889 ns | 0.0707 ns | 0.0008 |      64 B |
| EmptyApiOAuthFlows          |   6.637 ns |   1.2926 ns | 0.0709 ns | 0.0008 |      64 B |
| EmptyApiOperation           |  61.301 ns |   8.5114 ns | 0.4665 ns | 0.0044 |     376 B |
| EmptyApiParameter           |   7.386 ns |   0.4687 ns | 0.0257 ns | 0.0011 |      96 B |
| EmptyApiPathItem            |   5.610 ns |   0.8653 ns | 0.0474 ns | 0.0008 |      64 B |
| EmptyApiPaths               |  55.377 ns |   9.4958 ns | 0.5205 ns | 0.0029 |     248 B |
| EmptyApiRequestBody         |   5.865 ns |   0.7877 ns | 0.0432 ns | 0.0006 |      48 B |
| EmptyApiResponse            |   6.373 ns |   1.0014 ns | 0.0549 ns | 0.0008 |      64 B |
| EmptyApiResponses           |  56.446 ns |  11.7569 ns | 0.6444 ns | 0.0029 |     248 B |
| EmptyApiSchema              |  20.843 ns |   3.7370 ns | 0.2048 ns | 0.0061 |     512 B |
| EmptyApiSecurityRequirement |  14.549 ns |   1.5578 ns | 0.0854 ns | 0.0012 |     104 B |
| EmptyApiSecurityScheme      |   7.002 ns |   0.3731 ns | 0.0205 ns | 0.0012 |     104 B |
| EmptyApiServer              |   5.490 ns |   1.2367 ns | 0.0678 ns | 0.0007 |      56 B |
| EmptyApiServerVariable      |   5.286 ns |   0.7700 ns | 0.0422 ns | 0.0006 |      48 B |
| EmptyApiTag                 |   6.148 ns |   0.7943 ns | 0.0435 ns | 0.0008 |      72 B |
