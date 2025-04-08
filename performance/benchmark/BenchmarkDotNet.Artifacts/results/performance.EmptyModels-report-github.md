```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3476)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 9.0.202
  [Host]   : .NET 8.0.14 (8.0.1425.11118), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  ShortRun : .NET 8.0.14 (8.0.1425.11118), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean       | Error      | StdDev    | Gen0   | Gen1   | Allocated |
|---------------------------- |-----------:|-----------:|----------:|-------:|-------:|----------:|
| EmptyApiCallback            |  31.519 ns | 38.4672 ns | 2.1085 ns | 0.0331 |      - |     208 B |
| EmptyApiComponents          | 140.512 ns | 14.3041 ns | 0.7841 ns | 0.1428 | 0.0002 |     896 B |
| EmptyApiContact             |  16.055 ns |  1.3280 ns | 0.0728 ns | 0.0204 |      - |     128 B |
| EmptyApiDiscriminator       |  15.220 ns |  4.7792 ns | 0.2620 ns | 0.0179 |      - |     112 B |
| EmptyDocument               |  29.049 ns |  3.8422 ns | 0.2106 ns | 0.0433 |      - |     272 B |
| EmptyApiEncoding            |  33.103 ns | 57.7474 ns | 3.1653 ns | 0.0344 |      - |     216 B |
| EmptyApiExample             |  16.734 ns |  3.4981 ns | 0.1917 ns | 0.0242 |      - |     152 B |
| EmptyApiExternalDocs        |  16.954 ns |  1.2380 ns | 0.0679 ns | 0.0191 |      - |     120 B |
| EmptyApiHeader              |  43.507 ns |  2.5086 ns | 0.1375 ns | 0.0523 |      - |     328 B |
| EmptyApiInfo                |  16.481 ns |  0.3538 ns | 0.0194 ns | 0.0242 |      - |     152 B |
| EmptyApiLicense             |  17.546 ns |  3.3792 ns | 0.1852 ns | 0.0191 |      - |     120 B |
| EmptyApiLink                |  31.159 ns |  2.9456 ns | 0.1615 ns | 0.0395 |      - |     248 B |
| EmptyApiMediaType           |  48.673 ns | 40.1509 ns | 2.2008 ns | 0.0471 |      - |     296 B |
| EmptyApiOAuthFlow           |  31.660 ns |  6.6152 ns | 0.3626 ns | 0.0344 |      - |     216 B |
| EmptyApiOAuthFlows          |  16.520 ns |  8.7989 ns | 0.4823 ns | 0.0217 |      - |     136 B |
| EmptyApiOperation           |  74.102 ns |  5.9538 ns | 0.3263 ns | 0.0930 |      - |     584 B |
| EmptyApiParameter           |  44.586 ns | 25.6169 ns | 1.4041 ns | 0.0548 |      - |     344 B |
| EmptyApiPathItem            |  32.387 ns | 15.2578 ns | 0.8363 ns | 0.0485 |      - |     304 B |
| EmptyApiPaths               |  34.939 ns | 23.7764 ns | 1.3033 ns | 0.0268 |      - |     168 B |
| EmptyApiRequestBody         |  33.944 ns | 16.6503 ns | 0.9127 ns | 0.0344 |      - |     216 B |
| EmptyApiResponse            |  59.113 ns | 32.6796 ns | 1.7913 ns | 0.0625 |      - |     392 B |
| EmptyApiResponses           |  27.691 ns |  4.1005 ns | 0.2248 ns | 0.0268 |      - |     168 B |
| EmptyApiSchema              |  67.181 ns | 76.4118 ns | 4.1884 ns | 0.1109 | 0.0002 |     696 B |
| EmptyApiSecurityRequirement |   8.867 ns |  7.3926 ns | 0.4052 ns | 0.0166 |      - |     104 B |
| EmptyApiSecurityScheme      |  18.258 ns |  7.2938 ns | 0.3998 ns | 0.0280 |      - |     176 B |
| EmptyApiServer              |  30.928 ns |  6.3259 ns | 0.3467 ns | 0.0331 |      - |     208 B |
| EmptyApiServerVariable      |  17.401 ns |  1.4698 ns | 0.0806 ns | 0.0204 |      - |     128 B |
| EmptyApiTag                 |  17.470 ns | 17.4659 ns | 0.9574 ns | 0.0229 |      - |     144 B |
