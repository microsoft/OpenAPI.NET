```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.3 LTS (Noble Numbat) (container)
Intel Xeon Platinum 8370C CPU 2.80GHz (Max: 3.39GHz), 1 CPU, 2 logical cores and 1 physical core
.NET SDK 8.0.418
  [Host]   : .NET 8.0.24 (8.0.24, 8.0.2426.7010), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.24 (8.0.24, 8.0.2426.7010), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean       | Error      | StdDev     | Gen0   | Allocated |
|---------------------------- |-----------:|-----------:|-----------:|-------:|----------:|
| EmptyApiCallback            |   6.964 ns |   5.291 ns |  0.2900 ns | 0.0013 |      32 B |
| EmptyApiComponents          |  19.173 ns |  19.324 ns |  1.0592 ns | 0.0044 |     112 B |
| EmptyApiContact             |   8.183 ns |  16.338 ns |  0.8955 ns | 0.0019 |      48 B |
| EmptyApiDiscriminator       |   7.510 ns |   2.406 ns |  0.1319 ns | 0.0019 |      48 B |
| EmptyDocument               | 939.863 ns | 691.622 ns | 37.9102 ns | 0.0439 |    1144 B |
| EmptyApiEncoding            |  11.170 ns |  52.319 ns |  2.8678 ns | 0.0032 |      80 B |
| EmptyApiExample             |   8.631 ns |  12.824 ns |  0.7029 ns | 0.0029 |      72 B |
| EmptyApiExternalDocs        |   7.436 ns |  19.221 ns |  1.0536 ns | 0.0016 |      40 B |
| EmptyApiHeader              |   9.342 ns |   3.838 ns |  0.2104 ns | 0.0032 |      80 B |
| EmptyApiInfo                |   9.722 ns |  20.326 ns |  1.1141 ns | 0.0032 |      80 B |
| EmptyApiLicense             |   7.440 ns |   8.454 ns |  0.4634 ns | 0.0019 |      48 B |
| EmptyApiLink                |   9.082 ns |   8.480 ns |  0.4648 ns | 0.0029 |      72 B |
| EmptyApiMediaType           |   9.425 ns |  16.352 ns |  0.8963 ns | 0.0032 |      80 B |
| EmptyApiOAuthFlow           |   9.522 ns |  19.971 ns |  1.0947 ns | 0.0025 |      64 B |
| EmptyApiOAuthFlows          |   9.292 ns |   8.211 ns |  0.4501 ns | 0.0025 |      64 B |
| EmptyApiOperation           |  85.778 ns |  54.791 ns |  3.0033 ns | 0.0149 |     376 B |
| EmptyApiParameter           |  10.846 ns |   5.570 ns |  0.3053 ns | 0.0038 |      96 B |
| EmptyApiPathItem            |   9.145 ns |  24.879 ns |  1.3637 ns | 0.0025 |      64 B |
| EmptyApiPaths               |  84.455 ns |  72.890 ns |  3.9953 ns | 0.0098 |     248 B |
| EmptyApiRequestBody         |   8.446 ns |  21.137 ns |  1.1586 ns | 0.0019 |      48 B |
| EmptyApiResponse            |   9.328 ns |   6.467 ns |  0.3545 ns | 0.0025 |      64 B |
| EmptyApiResponses           |  74.717 ns |  75.055 ns |  4.1140 ns | 0.0098 |     248 B |
| EmptyApiSchema              |  27.212 ns |  68.454 ns |  3.7522 ns | 0.0162 |     408 B |
| EmptyApiSecurityRequirement |  18.828 ns |  22.964 ns |  1.2587 ns | 0.0041 |     104 B |
| EmptyApiSecurityScheme      |  10.762 ns |  12.076 ns |  0.6619 ns | 0.0041 |     104 B |
| EmptyApiServer              |   8.832 ns |  11.038 ns |  0.6050 ns | 0.0022 |      56 B |
| EmptyApiServerVariable      |   8.136 ns |   7.553 ns |  0.4140 ns | 0.0019 |      48 B |
| EmptyApiTag                 |  10.186 ns |   8.503 ns |  0.4661 ns | 0.0029 |      72 B |
