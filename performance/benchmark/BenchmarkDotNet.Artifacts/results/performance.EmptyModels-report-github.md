```

BenchmarkDotNet v0.14.0, Ubuntu 22.04.5 LTS (Jammy Jellyfish) WSL
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.407
  [Host]   : .NET 8.0.14 (8.0.1425.11118), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  ShortRun : .NET 8.0.14 (8.0.1425.11118), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean      | Error      | StdDev    | Gen0   | Gen1   | Allocated |
|---------------------------- |----------:|-----------:|----------:|-------:|-------:|----------:|
| EmptyApiCallback            |  38.08 ns |  18.788 ns |  1.030 ns | 0.0306 |      - |     192 B |
| EmptyApiComponents          | 197.64 ns |  23.748 ns |  1.302 ns | 0.1566 | 0.0005 |     984 B |
| EmptyApiContact             |  20.72 ns |   5.135 ns |  0.281 ns | 0.0204 |      - |     128 B |
| EmptyApiDiscriminator       |  38.21 ns |   7.074 ns |  0.388 ns | 0.0318 |      - |     200 B |
| EmptyDocument               | 734.06 ns |  86.948 ns |  4.766 ns | 0.2203 | 0.0010 |    1384 B |
| EmptyApiEncoding            |  36.83 ns |  16.946 ns |  0.929 ns | 0.0344 |      - |     216 B |
| EmptyApiExample             |  25.31 ns |  54.053 ns |  2.963 ns | 0.0217 |      - |     136 B |
| EmptyApiExternalDocs        |  31.36 ns |  34.894 ns |  1.913 ns | 0.0191 |      - |     120 B |
| EmptyApiHeader              |  83.42 ns | 105.740 ns |  5.796 ns | 0.0509 |      - |     320 B |
| EmptyApiInfo                |  33.48 ns | 113.942 ns |  6.246 ns | 0.0255 |      - |     160 B |
| EmptyApiLicense             |  47.42 ns | 131.044 ns |  7.183 ns | 0.0204 |      - |     128 B |
| EmptyApiLink                |  59.54 ns | 128.628 ns |  7.051 ns | 0.0370 |      - |     232 B |
| EmptyApiMediaType           |  68.45 ns |  27.431 ns |  1.504 ns | 0.0471 |      - |     296 B |
| EmptyApiOAuthFlow           |  44.79 ns |  22.253 ns |  1.220 ns | 0.0344 |      - |     216 B |
| EmptyApiOAuthFlows          |  26.24 ns |  54.893 ns |  3.009 ns | 0.0216 |      - |     136 B |
| EmptyApiOperation           | 130.32 ns | 400.514 ns | 21.954 ns | 0.1006 |      - |     632 B |
| EmptyApiParameter           |  59.98 ns |  18.536 ns |  1.016 ns | 0.0535 |      - |     336 B |
| EmptyApiPathItem            |  57.97 ns | 100.637 ns |  5.516 ns | 0.0459 |      - |     288 B |
| EmptyApiPaths               |  62.88 ns | 153.718 ns |  8.426 ns | 0.0395 |      - |     248 B |
| EmptyApiRequestBody         |  46.17 ns | 167.895 ns |  9.203 ns | 0.0331 |      - |     208 B |
| EmptyApiResponse            |  78.70 ns |  44.209 ns |  2.423 ns | 0.0598 |      - |     376 B |
| EmptyApiResponses           |  49.13 ns |   2.420 ns |  0.133 ns | 0.0395 |      - |     248 B |
| EmptyApiSchema              | 138.20 ns |  37.314 ns |  2.045 ns | 0.1707 | 0.0005 |    1072 B |
| EmptyApiSecurityRequirement |  13.38 ns |   2.406 ns |  0.132 ns | 0.0166 |      - |     104 B |
| EmptyApiSecurityScheme      |  21.99 ns |   3.714 ns |  0.204 ns | 0.0268 |      - |     168 B |
| EmptyApiServer              |  37.40 ns |   3.161 ns |  0.173 ns | 0.0331 |      - |     208 B |
| EmptyApiServerVariable      |  23.73 ns |   5.836 ns |  0.320 ns | 0.0204 |      - |     128 B |
| EmptyApiTag                 |  23.21 ns |  31.131 ns |  1.706 ns | 0.0204 |      - |     128 B |
