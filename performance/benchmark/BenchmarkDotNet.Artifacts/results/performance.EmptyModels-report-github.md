```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26200.9106) (Hyper-V)
AMD EPYC 7763, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.303
  [Host]   : .NET 8.0.30 (8.0.3026.36720), X64 RyuJIT AVX2
  ShortRun : .NET 8.0.30 (8.0.3026.36720), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean      | Error      | StdDev    | Gen0   | Allocated |
|---------------------------- |----------:|-----------:|----------:|-------:|----------:|
| EmptyApiCallback            |  49.40 ns | 104.721 ns |  5.740 ns | 0.0124 |     208 B |
| EmptyApiComponents          | 302.23 ns | 903.447 ns | 49.521 ns | 0.0534 |     896 B |
| EmptyApiContact             |  26.61 ns |  15.989 ns |  0.876 ns | 0.0076 |     128 B |
| EmptyApiDiscriminator       |  28.03 ns |  11.415 ns |  0.626 ns | 0.0067 |     112 B |
| EmptyDocument               |  50.32 ns |  43.092 ns |  2.362 ns | 0.0162 |     272 B |
| EmptyApiEncoding            |  50.27 ns |  28.520 ns |  1.563 ns | 0.0129 |     216 B |
| EmptyApiExample             |  26.33 ns |   7.773 ns |  0.426 ns | 0.0091 |     152 B |
| EmptyApiExternalDocs        |  27.36 ns |  23.617 ns |  1.295 ns | 0.0072 |     120 B |
| EmptyApiHeader              |  73.84 ns |  78.669 ns |  4.312 ns | 0.0196 |     328 B |
| EmptyApiInfo                |  29.99 ns |  91.424 ns |  5.011 ns | 0.0091 |     152 B |
| EmptyApiLicense             |  32.02 ns |  63.269 ns |  3.468 ns | 0.0072 |     120 B |
| EmptyApiLink                |  59.86 ns | 122.819 ns |  6.732 ns | 0.0148 |     248 B |
| EmptyApiMediaType           |  77.93 ns | 145.131 ns |  7.955 ns | 0.0176 |     296 B |
| EmptyApiOAuthFlow           |  53.89 ns |  70.496 ns |  3.864 ns | 0.0129 |     216 B |
| EmptyApiOAuthFlows          |  47.32 ns |   5.775 ns |  0.317 ns | 0.0081 |     136 B |
| EmptyApiOperation           | 185.84 ns | 621.224 ns | 34.051 ns | 0.0348 |     584 B |
| EmptyApiParameter           |  79.19 ns |  78.560 ns |  4.306 ns | 0.0205 |     344 B |
| EmptyApiPathItem            |  58.45 ns |  72.595 ns |  3.979 ns | 0.0181 |     304 B |
| EmptyApiPaths               |  45.18 ns |  60.829 ns |  3.334 ns | 0.0100 |     168 B |
| EmptyApiRequestBody         |  49.70 ns |  30.429 ns |  1.668 ns | 0.0129 |     216 B |
| EmptyApiResponse            |  93.13 ns |  46.379 ns |  2.542 ns | 0.0234 |     392 B |
| EmptyApiResponses           |  43.03 ns |  36.408 ns |  1.996 ns | 0.0100 |     168 B |
| EmptyApiSchema              | 104.26 ns |  25.354 ns |  1.390 ns | 0.0416 |     696 B |
| EmptyApiSecurityRequirement |  16.28 ns |  23.985 ns |  1.315 ns | 0.0062 |     104 B |
| EmptyApiSecurityScheme      |  31.86 ns |  40.830 ns |  2.238 ns | 0.0105 |     176 B |
| EmptyApiServer              |  49.09 ns |  62.291 ns |  3.414 ns | 0.0124 |     208 B |
| EmptyApiServerVariable      |  25.27 ns |   8.671 ns |  0.475 ns | 0.0076 |     128 B |
| EmptyApiTag                 |  27.82 ns |  10.429 ns |  0.572 ns | 0.0086 |     144 B |
