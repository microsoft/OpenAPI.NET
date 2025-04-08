```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3476)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.407
  [Host]   : .NET 8.0.14 (8.0.1425.11118), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  ShortRun : .NET 8.0.14 (8.0.1425.11118), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean       | Error      | StdDev     | Median     | Gen0   | Gen1   | Allocated |
|---------------------------- |-----------:|-----------:|-----------:|-----------:|-------:|-------:|----------:|
| EmptyApiCallback            |  43.413 ns |  64.615 ns |  3.5418 ns |  41.429 ns | 0.0306 |      - |     192 B |
| EmptyApiComponents          | 174.428 ns | 177.831 ns |  9.7475 ns | 178.768 ns | 0.1566 | 0.0005 |     984 B |
| EmptyApiContact             |  17.993 ns |  19.112 ns |  1.0476 ns |  17.865 ns | 0.0204 |      - |     128 B |
| EmptyApiDiscriminator       |  32.343 ns |  24.299 ns |  1.3319 ns |  32.988 ns | 0.0318 |      - |     200 B |
| EmptyDocument               | 556.709 ns | 374.450 ns | 20.5249 ns | 550.393 ns | 0.2203 | 0.0010 |    1384 B |
| EmptyApiEncoding            |  34.972 ns |  22.225 ns |  1.2182 ns |  34.710 ns | 0.0344 |      - |     216 B |
| EmptyApiExample             |  17.051 ns |  19.571 ns |  1.0728 ns |  16.492 ns | 0.0217 |      - |     136 B |
| EmptyApiExternalDocs        |  21.698 ns |  52.592 ns |  2.8827 ns |  20.859 ns | 0.0191 |      - |     120 B |
| EmptyApiHeader              |  51.813 ns |  71.591 ns |  3.9242 ns |  49.602 ns | 0.0510 |      - |     320 B |
| EmptyApiInfo                |  18.290 ns |   7.732 ns |  0.4238 ns |  18.079 ns | 0.0255 |      - |     160 B |
| EmptyApiLicense             |  19.421 ns |  45.906 ns |  2.5163 ns |  20.804 ns | 0.0204 |      - |     128 B |
| EmptyApiLink                |  32.448 ns |  11.779 ns |  0.6456 ns |  32.340 ns | 0.0370 |      - |     232 B |
| EmptyApiMediaType           |  61.900 ns | 160.639 ns |  8.8052 ns |  59.521 ns | 0.0471 |      - |     296 B |
| EmptyApiOAuthFlow           |  38.086 ns |  57.540 ns |  3.1540 ns |  37.072 ns | 0.0344 |      - |     216 B |
| EmptyApiOAuthFlows          |  17.813 ns |  26.116 ns |  1.4315 ns |  17.183 ns | 0.0217 |      - |     136 B |
| EmptyApiOperation           | 284.797 ns |  90.805 ns |  4.9773 ns | 285.265 ns | 0.1007 | 0.0001 |     632 B |
| EmptyApiParameter           | 113.771 ns | 621.746 ns | 34.0800 ns | 111.246 ns | 0.0534 |      - |     336 B |
| EmptyApiPathItem            | 106.226 ns | 177.486 ns |  9.7286 ns | 102.109 ns | 0.0458 |      - |     288 B |
| EmptyApiPaths               | 134.175 ns |  18.198 ns |  0.9975 ns | 134.283 ns | 0.0393 |      - |     248 B |
| EmptyApiRequestBody         | 105.229 ns |  16.300 ns |  0.8935 ns | 105.228 ns | 0.0331 |      - |     208 B |
| EmptyApiResponse            | 195.390 ns |  23.409 ns |  1.2831 ns | 195.320 ns | 0.0598 |      - |     376 B |
| EmptyApiResponses           |  80.140 ns | 260.612 ns | 14.2850 ns |  76.470 ns | 0.0393 |      - |     248 B |
| EmptyApiSchema              | 196.306 ns | 987.242 ns | 54.1141 ns | 186.016 ns | 0.1707 | 0.0005 |    1072 B |
| EmptyApiSecurityRequirement |   9.601 ns |   1.589 ns |  0.0871 ns |   9.649 ns | 0.0166 |      - |     104 B |
| EmptyApiSecurityScheme      |  30.086 ns | 342.099 ns | 18.7516 ns |  19.844 ns | 0.0268 |      - |     168 B |
| EmptyApiServer              |  33.843 ns |  20.069 ns |  1.1001 ns |  33.641 ns | 0.0331 |      - |     208 B |
| EmptyApiServerVariable      |  17.171 ns |   9.809 ns |  0.5377 ns |  17.034 ns | 0.0204 |      - |     128 B |
| EmptyApiTag                 |  16.494 ns |   5.025 ns |  0.2754 ns |  16.402 ns | 0.0204 |      - |     128 B |
