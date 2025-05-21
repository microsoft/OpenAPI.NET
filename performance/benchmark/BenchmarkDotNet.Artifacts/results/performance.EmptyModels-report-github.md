```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3981)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.409
  [Host]   : .NET 8.0.16 (8.0.1625.21506), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  ShortRun : .NET 8.0.16 (8.0.1625.21506), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean       | Error       | StdDev     | Median     | Gen0   | Allocated |
|---------------------------- |-----------:|------------:|-----------:|-----------:|-------:|----------:|
| EmptyApiCallback            |   4.120 ns |   1.7973 ns |  0.0985 ns |   4.170 ns | 0.0051 |      32 B |
| EmptyApiComponents          |  11.965 ns |  56.2456 ns |  3.0830 ns |  12.490 ns | 0.0166 |     104 B |
| EmptyApiContact             |  11.843 ns |  66.9375 ns |  3.6691 ns |  12.069 ns | 0.0076 |      48 B |
| EmptyApiDiscriminator       |   5.461 ns |  41.2864 ns |  2.2630 ns |   4.184 ns | 0.0064 |      40 B |
| EmptyDocument               | 447.453 ns | 307.9616 ns | 16.8804 ns | 440.505 ns | 0.1802 |    1136 B |
| EmptyApiEncoding            |   4.933 ns |  10.6453 ns |  0.5835 ns |   5.094 ns | 0.0089 |      56 B |
| EmptyApiExample             |   4.753 ns |   8.8503 ns |  0.4851 ns |   4.660 ns | 0.0089 |      56 B |
| EmptyApiExternalDocs        |   4.035 ns |   2.4540 ns |  0.1345 ns |   4.079 ns | 0.0064 |      40 B |
| EmptyApiHeader              |   5.239 ns |   5.4576 ns |  0.2991 ns |   5.067 ns | 0.0127 |      80 B |
| EmptyApiInfo                |   5.183 ns |   5.2973 ns |  0.2904 ns |   5.125 ns | 0.0127 |      80 B |
| EmptyApiLicense             |   4.250 ns |   3.7202 ns |  0.2039 ns |   4.236 ns | 0.0076 |      48 B |
| EmptyApiLink                |   4.837 ns |   3.6203 ns |  0.1984 ns |   4.853 ns | 0.0115 |      72 B |
| EmptyApiMediaType           |   4.388 ns |   2.8437 ns |  0.1559 ns |   4.341 ns | 0.0089 |      56 B |
| EmptyApiOAuthFlow           |   4.318 ns |   2.7240 ns |  0.1493 ns |   4.249 ns | 0.0089 |      56 B |
| EmptyApiOAuthFlows          |   4.473 ns |   4.8273 ns |  0.2646 ns |   4.339 ns | 0.0089 |      56 B |
| EmptyApiOperation           |  63.983 ns | 208.7371 ns | 11.4416 ns |  60.687 ns | 0.0599 |     376 B |
| EmptyApiParameter           |   4.958 ns |   0.7540 ns |  0.0413 ns |   4.975 ns | 0.0153 |      96 B |
| EmptyApiPathItem            |   5.436 ns |   7.9092 ns |  0.4335 ns |   5.383 ns | 0.0102 |      64 B |
| EmptyApiPaths               |  81.391 ns | 292.1156 ns | 16.0118 ns |  82.553 ns | 0.0395 |     248 B |
| EmptyApiRequestBody         |   4.576 ns |   7.3893 ns |  0.4050 ns |   4.512 ns | 0.0076 |      48 B |
| EmptyApiResponse            |   4.349 ns |   1.1732 ns |  0.0643 ns |   4.319 ns | 0.0089 |      56 B |
| EmptyApiResponses           |  53.510 ns |  87.6669 ns |  4.8053 ns |  54.947 ns | 0.0395 |     248 B |
| EmptyApiSchema              |  14.447 ns |  21.8422 ns |  1.1972 ns |  14.301 ns | 0.0650 |     408 B |
| EmptyApiSecurityRequirement |   8.445 ns |   5.3829 ns |  0.2951 ns |   8.298 ns | 0.0166 |     104 B |
| EmptyApiSecurityScheme      |   6.039 ns |  12.4925 ns |  0.6848 ns |   5.960 ns | 0.0140 |      88 B |
| EmptyApiServer              |   4.640 ns |   3.9936 ns |  0.2189 ns |   4.747 ns | 0.0076 |      48 B |
| EmptyApiServerVariable      |   5.058 ns |   3.8031 ns |  0.2085 ns |   4.972 ns | 0.0076 |      48 B |
| EmptyApiTag                 |   4.763 ns |   8.7140 ns |  0.4776 ns |   4.711 ns | 0.0076 |      48 B |
