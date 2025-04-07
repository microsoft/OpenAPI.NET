```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3476)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.407
  [Host]   : .NET 8.0.14 (8.0.1425.11118), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  ShortRun : .NET 8.0.14 (8.0.1425.11118), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean      | Error        | StdDev     | Median    | Gen0   | Gen1   | Allocated |
|---------------------------- |----------:|-------------:|-----------:|----------:|-------:|-------:|----------:|
| EmptyApiCallback            |  44.07 ns |   146.637 ns |   8.038 ns |  45.60 ns | 0.0306 |      - |     192 B |
| EmptyApiComponents          | 205.97 ns |   471.086 ns |  25.822 ns | 193.80 ns | 0.1566 | 0.0005 |     984 B |
| EmptyApiContact             |  30.27 ns |    39.655 ns |   2.174 ns |  31.32 ns | 0.0204 |      - |     128 B |
| EmptyApiDiscriminator       |  33.92 ns |   100.165 ns |   5.490 ns |  31.57 ns | 0.0318 |      - |     200 B |
| EmptyDocument               | 779.70 ns | 2,985.749 ns | 163.659 ns | 722.56 ns | 0.2203 | 0.0010 |    1384 B |
| EmptyApiEncoding            |  40.32 ns |    42.717 ns |   2.341 ns |  41.62 ns | 0.0344 |      - |     216 B |
| EmptyApiExample             |  16.48 ns |     4.664 ns |   0.256 ns |  16.45 ns | 0.0217 |      - |     136 B |
| EmptyApiExternalDocs        |  31.46 ns |   437.448 ns |  23.978 ns |  18.05 ns | 0.0191 |      - |     120 B |
| EmptyApiHeader              |  61.24 ns |   147.696 ns |   8.096 ns |  57.06 ns | 0.0509 |      - |     320 B |
| EmptyApiInfo                |  23.52 ns |    35.018 ns |   1.919 ns |  22.45 ns | 0.0255 |      - |     160 B |
| EmptyApiLicense             |  18.59 ns |    27.731 ns |   1.520 ns |  18.09 ns | 0.0204 |      - |     128 B |
| EmptyApiLink                |  54.08 ns |   310.850 ns |  17.039 ns |  44.79 ns | 0.0370 |      - |     232 B |
| EmptyApiMediaType           |  65.53 ns |   201.353 ns |  11.037 ns |  63.49 ns | 0.0471 |      - |     296 B |
| EmptyApiOAuthFlow           |  95.07 ns |   541.832 ns |  29.700 ns |  96.14 ns | 0.0344 |      - |     216 B |
| EmptyApiOAuthFlows          |  31.02 ns |   119.015 ns |   6.524 ns |  33.25 ns | 0.0217 |      - |     136 B |
| EmptyApiOperation           | 127.49 ns |   239.810 ns |  13.145 ns | 128.04 ns | 0.1006 |      - |     632 B |
| EmptyApiParameter           |  72.82 ns |   123.982 ns |   6.796 ns |  70.44 ns | 0.0535 |      - |     336 B |
| EmptyApiPathItem            |  44.87 ns |    40.567 ns |   2.224 ns |  44.30 ns | 0.0459 |      - |     288 B |
| EmptyApiPaths               |  84.99 ns |   401.839 ns |  22.026 ns |  93.58 ns | 0.0395 |      - |     248 B |
| EmptyApiRequestBody         |  30.96 ns |     5.391 ns |   0.296 ns |  31.09 ns | 0.0331 |      - |     208 B |
| EmptyApiResponse            |  67.54 ns |    36.492 ns |   2.000 ns |  67.42 ns | 0.0598 |      - |     376 B |
| EmptyApiResponses           |  51.79 ns |    61.598 ns |   3.376 ns |  53.54 ns | 0.0395 |      - |     248 B |
| EmptyApiSchema              | 107.45 ns |    52.352 ns |   2.870 ns | 107.19 ns | 0.1707 | 0.0005 |    1072 B |
| EmptyApiSecurityRequirement |  19.42 ns |    93.507 ns |   5.125 ns |  17.83 ns | 0.0166 |      - |     104 B |
| EmptyApiSecurityScheme      |  38.73 ns |   332.810 ns |  18.242 ns |  31.34 ns | 0.0268 |      - |     168 B |
| EmptyApiServer              |  41.63 ns |    44.977 ns |   2.465 ns |  41.52 ns | 0.0331 |      - |     208 B |
| EmptyApiServerVariable      |  23.72 ns |    54.900 ns |   3.009 ns |  22.66 ns | 0.0204 |      - |     128 B |
| EmptyApiTag                 |  23.99 ns |    30.096 ns |   1.650 ns |  23.74 ns | 0.0204 |      - |     128 B |
