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
| EmptyApiCallback            |  49.88 ns | 29.348 ns | 1.609 ns | 0.0124 |     208 B |
| EmptyApiComponents          | 220.57 ns | 13.765 ns | 0.755 ns | 0.0534 |     896 B |
| EmptyApiContact             |  26.28 ns |  0.809 ns | 0.044 ns | 0.0076 |     128 B |
| EmptyApiDiscriminator       |  25.92 ns |  7.692 ns | 0.422 ns | 0.0067 |     112 B |
| EmptyDocument               |  56.50 ns | 52.254 ns | 2.864 ns | 0.0162 |     272 B |
| EmptyApiEncoding            |  47.16 ns | 31.896 ns | 1.748 ns | 0.0129 |     216 B |
| EmptyApiExample             |  27.31 ns |  3.156 ns | 0.173 ns | 0.0091 |     152 B |
| EmptyApiExternalDocs        |  26.66 ns |  5.050 ns | 0.277 ns | 0.0072 |     120 B |
| EmptyApiHeader              |  71.33 ns |  2.184 ns | 0.120 ns | 0.0196 |     328 B |
| EmptyApiInfo                |  27.21 ns |  6.589 ns | 0.361 ns | 0.0091 |     152 B |
| EmptyApiLicense             |  26.21 ns |  7.746 ns | 0.425 ns | 0.0072 |     120 B |
| EmptyApiLink                |  51.17 ns | 11.498 ns | 0.630 ns | 0.0148 |     248 B |
| EmptyApiMediaType           |  71.96 ns | 24.006 ns | 1.316 ns | 0.0176 |     296 B |
| EmptyApiOAuthFlow           |  64.53 ns |  4.598 ns | 0.252 ns | 0.0129 |     216 B |
| EmptyApiOAuthFlows          |  27.60 ns |  5.665 ns | 0.311 ns | 0.0081 |     136 B |
| EmptyApiOperation           | 135.29 ns | 16.660 ns | 0.913 ns | 0.0348 |     584 B |
| EmptyApiParameter           |  74.36 ns |  8.734 ns | 0.479 ns | 0.0205 |     344 B |
| EmptyApiPathItem            |  60.62 ns | 39.511 ns | 2.166 ns | 0.0181 |     304 B |
| EmptyApiPaths               |  39.49 ns |  1.490 ns | 0.082 ns | 0.0100 |     168 B |
| EmptyApiRequestBody         |  47.84 ns |  7.823 ns | 0.429 ns | 0.0129 |     216 B |
| EmptyApiResponse            |  87.35 ns |  4.081 ns | 0.224 ns | 0.0234 |     392 B |
| EmptyApiResponses           |  39.10 ns |  7.001 ns | 0.384 ns | 0.0100 |     168 B |
| EmptyApiSchema              | 110.38 ns | 14.431 ns | 0.791 ns | 0.0416 |     696 B |
| EmptyApiSecurityRequirement |  15.82 ns |  2.345 ns | 0.129 ns | 0.0062 |     104 B |
| EmptyApiSecurityScheme      |  27.36 ns |  2.313 ns | 0.127 ns | 0.0105 |     176 B |
| EmptyApiServer              |  46.04 ns |  6.024 ns | 0.330 ns | 0.0124 |     208 B |
| EmptyApiServerVariable      |  27.29 ns |  0.855 ns | 0.047 ns | 0.0076 |     128 B |
| EmptyApiTag                 |  27.43 ns |  5.298 ns | 0.290 ns | 0.0086 |     144 B |
