```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon 6973P-C, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.303
  [Host]   : .NET 8.0.30 (8.0.3026.36720), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  ShortRun : .NET 8.0.30 (8.0.3026.36720), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean      | Error     | StdDev   | Gen0   | Allocated |
|---------------------------- |----------:|----------:|---------:|-------:|----------:|
| EmptyApiCallback            |  45.03 ns | 17.933 ns | 0.983 ns | 0.0024 |     208 B |
| EmptyApiComponents          | 181.38 ns | 26.959 ns | 1.478 ns | 0.0105 |     896 B |
| EmptyApiContact             |  25.20 ns |  5.248 ns | 0.288 ns | 0.0015 |     128 B |
| EmptyApiDiscriminator       |  24.97 ns |  2.610 ns | 0.143 ns | 0.0013 |     112 B |
| EmptyDocument               |  58.46 ns | 18.062 ns | 0.990 ns | 0.0032 |     272 B |
| EmptyApiEncoding            |  44.82 ns |  4.547 ns | 0.249 ns | 0.0026 |     216 B |
| EmptyApiExample             |  29.20 ns |  7.338 ns | 0.402 ns | 0.0018 |     152 B |
| EmptyApiExternalDocs        |  24.02 ns |  3.101 ns | 0.170 ns | 0.0014 |     120 B |
| EmptyApiHeader              |  67.29 ns | 12.188 ns | 0.668 ns | 0.0038 |     328 B |
| EmptyApiInfo                |  32.28 ns |  5.773 ns | 0.316 ns | 0.0018 |     152 B |
| EmptyApiLicense             |  26.17 ns | 12.046 ns | 0.660 ns | 0.0014 |     120 B |
| EmptyApiLink                |  48.96 ns | 33.972 ns | 1.862 ns | 0.0029 |     248 B |
| EmptyApiMediaType           |  69.71 ns | 15.009 ns | 0.823 ns | 0.0035 |     296 B |
| EmptyApiOAuthFlow           |  47.82 ns | 17.756 ns | 0.973 ns | 0.0026 |     216 B |
| EmptyApiOAuthFlows          |  25.06 ns |  9.688 ns | 0.531 ns | 0.0016 |     136 B |
| EmptyApiOperation           | 119.69 ns | 43.270 ns | 2.372 ns | 0.0069 |     584 B |
| EmptyApiParameter           |  66.75 ns | 31.057 ns | 1.702 ns | 0.0041 |     344 B |
| EmptyApiPathItem            |  56.85 ns | 24.116 ns | 1.322 ns | 0.0036 |     304 B |
| EmptyApiPaths               |  35.76 ns |  5.519 ns | 0.302 ns | 0.0020 |     168 B |
| EmptyApiRequestBody         |  44.90 ns | 17.872 ns | 0.980 ns | 0.0026 |     216 B |
| EmptyApiResponse            |  84.26 ns | 45.778 ns | 2.509 ns | 0.0046 |     392 B |
| EmptyApiResponses           |  35.86 ns |  8.564 ns | 0.469 ns | 0.0020 |     168 B |
| EmptyApiSchema              | 110.51 ns | 32.027 ns | 1.755 ns | 0.0082 |     696 B |
| EmptyApiSecurityRequirement |  18.18 ns | 13.605 ns | 0.746 ns | 0.0012 |     104 B |
| EmptyApiSecurityScheme      |  27.14 ns | 39.022 ns | 2.139 ns | 0.0021 |     176 B |
| EmptyApiServer              |  40.33 ns | 11.650 ns | 0.639 ns | 0.0024 |     208 B |
| EmptyApiServerVariable      |  24.43 ns |  5.090 ns | 0.279 ns | 0.0015 |     128 B |
| EmptyApiTag                 |  29.14 ns | 26.493 ns | 1.452 ns | 0.0017 |     144 B |
