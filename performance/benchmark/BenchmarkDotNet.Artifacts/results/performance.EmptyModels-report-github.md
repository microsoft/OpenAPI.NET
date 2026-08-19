```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]   : .NET 8.0.30 (8.0.3026.36720), X64 RyuJIT AVX2
  ShortRun : .NET 8.0.30 (8.0.3026.36720), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean      | Error     | StdDev   | Gen0   | Allocated |
|---------------------------- |----------:|----------:|---------:|-------:|----------:|
| EmptyApiCallback            |  46.16 ns | 18.788 ns | 1.030 ns | 0.0124 |     208 B |
| EmptyApiComponents          | 214.80 ns | 37.548 ns | 2.058 ns | 0.0534 |     896 B |
| EmptyApiContact             |  26.27 ns |  3.484 ns | 0.191 ns | 0.0076 |     128 B |
| EmptyApiDiscriminator       |  25.48 ns |  3.211 ns | 0.176 ns | 0.0067 |     112 B |
| EmptyDocument               |  57.00 ns | 41.689 ns | 2.285 ns | 0.0162 |     272 B |
| EmptyApiEncoding            |  47.68 ns |  8.340 ns | 0.457 ns | 0.0129 |     216 B |
| EmptyApiExample             |  26.77 ns |  3.066 ns | 0.168 ns | 0.0091 |     152 B |
| EmptyApiExternalDocs        |  26.18 ns |  9.349 ns | 0.512 ns | 0.0072 |     120 B |
| EmptyApiHeader              |  70.34 ns |  7.145 ns | 0.392 ns | 0.0196 |     328 B |
| EmptyApiInfo                |  26.83 ns |  7.860 ns | 0.431 ns | 0.0091 |     152 B |
| EmptyApiLicense             |  27.63 ns |  2.343 ns | 0.128 ns | 0.0072 |     120 B |
| EmptyApiLink                |  49.25 ns |  5.517 ns | 0.302 ns | 0.0148 |     248 B |
| EmptyApiMediaType           |  68.63 ns |  1.123 ns | 0.062 ns | 0.0176 |     296 B |
| EmptyApiOAuthFlow           |  47.06 ns | 12.170 ns | 0.667 ns | 0.0129 |     216 B |
| EmptyApiOAuthFlows          |  26.24 ns |  6.435 ns | 0.353 ns | 0.0081 |     136 B |
| EmptyApiOperation           | 128.29 ns | 19.146 ns | 1.049 ns | 0.0348 |     584 B |
| EmptyApiParameter           |  70.68 ns |  8.696 ns | 0.477 ns | 0.0205 |     344 B |
| EmptyApiPathItem            |  60.18 ns | 11.229 ns | 0.616 ns | 0.0181 |     304 B |
| EmptyApiPaths               |  38.03 ns |  3.233 ns | 0.177 ns | 0.0100 |     168 B |
| EmptyApiRequestBody         |  47.98 ns | 28.401 ns | 1.557 ns | 0.0129 |     216 B |
| EmptyApiResponse            |  84.64 ns |  9.700 ns | 0.532 ns | 0.0234 |     392 B |
| EmptyApiResponses           |  39.10 ns |  8.984 ns | 0.492 ns | 0.0100 |     168 B |
| EmptyApiSchema              | 104.01 ns | 32.503 ns | 1.782 ns | 0.0416 |     696 B |
| EmptyApiSecurityRequirement |  14.65 ns |  6.529 ns | 0.358 ns | 0.0062 |     104 B |
| EmptyApiSecurityScheme      |  27.18 ns |  2.088 ns | 0.114 ns | 0.0105 |     176 B |
| EmptyApiServer              |  46.51 ns |  1.908 ns | 0.105 ns | 0.0124 |     208 B |
| EmptyApiServerVariable      |  25.89 ns |  2.527 ns | 0.138 ns | 0.0076 |     128 B |
| EmptyApiTag                 |  29.30 ns |  0.794 ns | 0.043 ns | 0.0086 |     144 B |
