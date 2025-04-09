```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3476)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.408
  [Host]   : .NET 8.0.15 (8.0.1525.16413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  ShortRun : .NET 8.0.15 (8.0.1525.16413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean       | Error       | StdDev     | Gen0   | Gen1   | Allocated |
|---------------------------- |-----------:|------------:|-----------:|-------:|-------:|----------:|
| EmptyApiCallback            |   4.557 ns |   2.2807 ns |  0.1250 ns | 0.0051 |      - |      32 B |
| EmptyApiComponents          |   5.116 ns |   0.6915 ns |  0.0379 ns | 0.0166 |      - |     104 B |
| EmptyApiContact             |   3.759 ns |   3.1240 ns |  0.1712 ns | 0.0076 |      - |      48 B |
| EmptyApiDiscriminator       |   3.442 ns |   2.3747 ns |  0.1302 ns | 0.0064 |      - |      40 B |
| EmptyDocument               | 397.830 ns |  46.9921 ns |  2.5758 ns | 0.1807 | 0.0005 |    1136 B |
| EmptyApiEncoding            |   3.879 ns |   3.0270 ns |  0.1659 ns | 0.0089 |      - |      56 B |
| EmptyApiExample             |   4.045 ns |   6.0543 ns |  0.3319 ns | 0.0089 |      - |      56 B |
| EmptyApiExternalDocs        |   3.455 ns |   2.1233 ns |  0.1164 ns | 0.0064 |      - |      40 B |
| EmptyApiHeader              |   4.633 ns |   3.8933 ns |  0.2134 ns | 0.0127 |      - |      80 B |
| EmptyApiInfo                |   4.462 ns |   4.0279 ns |  0.2208 ns | 0.0127 |      - |      80 B |
| EmptyApiLicense             |   3.670 ns |   1.6839 ns |  0.0923 ns | 0.0076 |      - |      48 B |
| EmptyApiLink                |   4.388 ns |   1.9826 ns |  0.1087 ns | 0.0115 |      - |      72 B |
| EmptyApiMediaType           |   3.857 ns |   1.4731 ns |  0.0807 ns | 0.0089 |      - |      56 B |
| EmptyApiOAuthFlow           |   3.810 ns |   1.1359 ns |  0.0623 ns | 0.0089 |      - |      56 B |
| EmptyApiOAuthFlows          |   3.979 ns |   5.5181 ns |  0.3025 ns | 0.0089 |      - |      56 B |
| EmptyApiOperation           |  72.530 ns | 230.3314 ns | 12.6252 ns | 0.0599 | 0.0001 |     376 B |
| EmptyApiParameter           |   4.919 ns |   3.2142 ns |  0.1762 ns | 0.0153 |      - |      96 B |
| EmptyApiPathItem            |   3.966 ns |   0.7140 ns |  0.0391 ns | 0.0102 |      - |      64 B |
| EmptyApiPaths               |  56.222 ns |  32.3248 ns |  1.7718 ns | 0.0395 |      - |     248 B |
| EmptyApiRequestBody         |   3.683 ns |   2.3246 ns |  0.1274 ns | 0.0076 |      - |      48 B |
| EmptyApiResponse            |   3.864 ns |   0.9334 ns |  0.0512 ns | 0.0089 |      - |      56 B |
| EmptyApiResponses           |  49.325 ns |   7.2131 ns |  0.3954 ns | 0.0395 |      - |     248 B |
| EmptyApiSchema              |  12.565 ns |   2.0834 ns |  0.1142 ns | 0.0650 |      - |     408 B |
| EmptyApiSecurityRequirement |   8.411 ns |   1.5393 ns |  0.0844 ns | 0.0166 |      - |     104 B |
| EmptyApiSecurityScheme      |   4.719 ns |   3.8028 ns |  0.2084 ns | 0.0140 |      - |      88 B |
| EmptyApiServer              |   3.626 ns |   0.4928 ns |  0.0270 ns | 0.0076 |      - |      48 B |
| EmptyApiServerVariable      |   3.589 ns |   0.2983 ns |  0.0164 ns | 0.0076 |      - |      48 B |
| EmptyApiTag                 |   3.889 ns |   7.4113 ns |  0.4062 ns | 0.0076 |      - |      48 B |
