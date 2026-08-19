```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.303
  [Host]   : .NET 8.0.30 (8.0.3026.36720), X64 RyuJIT AVX2
  ShortRun : .NET 8.0.30 (8.0.3026.36720), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean      | Error     | StdDev   | Gen0   | Allocated |
|---------------------------- |----------:|----------:|---------:|-------:|----------:|
| EmptyApiCallback            |  43.42 ns |  2.656 ns | 0.146 ns | 0.0124 |     208 B |
| EmptyApiComponents          | 202.82 ns | 23.462 ns | 1.286 ns | 0.0534 |     896 B |
| EmptyApiContact             |  25.77 ns |  3.655 ns | 0.200 ns | 0.0076 |     128 B |
| EmptyApiDiscriminator       |  25.86 ns |  8.292 ns | 0.454 ns | 0.0067 |     112 B |
| EmptyDocument               |  56.04 ns |  8.461 ns | 0.464 ns | 0.0162 |     272 B |
| EmptyApiEncoding            |  49.09 ns | 11.972 ns | 0.656 ns | 0.0129 |     216 B |
| EmptyApiExample             |  29.03 ns | 10.463 ns | 0.574 ns | 0.0091 |     152 B |
| EmptyApiExternalDocs        |  26.78 ns |  9.102 ns | 0.499 ns | 0.0072 |     120 B |
| EmptyApiHeader              |  69.01 ns |  4.668 ns | 0.256 ns | 0.0196 |     328 B |
| EmptyApiInfo                |  33.38 ns |  9.033 ns | 0.495 ns | 0.0091 |     152 B |
| EmptyApiLicense             |  27.36 ns | 18.241 ns | 1.000 ns | 0.0072 |     120 B |
| EmptyApiLink                |  49.22 ns | 31.989 ns | 1.753 ns | 0.0148 |     248 B |
| EmptyApiMediaType           |  70.68 ns | 11.715 ns | 0.642 ns | 0.0176 |     296 B |
| EmptyApiOAuthFlow           |  46.59 ns |  9.360 ns | 0.513 ns | 0.0129 |     216 B |
| EmptyApiOAuthFlows          |  27.41 ns | 18.088 ns | 0.991 ns | 0.0081 |     136 B |
| EmptyApiOperation           | 122.79 ns | 25.296 ns | 1.387 ns | 0.0348 |     584 B |
| EmptyApiParameter           |  75.89 ns | 42.069 ns | 2.306 ns | 0.0205 |     344 B |
| EmptyApiPathItem            |  62.86 ns |  6.705 ns | 0.368 ns | 0.0181 |     304 B |
| EmptyApiPaths               |  40.92 ns |  4.704 ns | 0.258 ns | 0.0100 |     168 B |
| EmptyApiRequestBody         |  48.94 ns | 30.826 ns | 1.690 ns | 0.0129 |     216 B |
| EmptyApiResponse            |  83.56 ns | 15.695 ns | 0.860 ns | 0.0234 |     392 B |
| EmptyApiResponses           |  40.10 ns |  1.899 ns | 0.104 ns | 0.0100 |     168 B |
| EmptyApiSchema              | 105.34 ns | 14.707 ns | 0.806 ns | 0.0416 |     696 B |
| EmptyApiSecurityRequirement |  25.67 ns |  3.945 ns | 0.216 ns | 0.0062 |     104 B |
| EmptyApiSecurityScheme      |  28.96 ns |  5.686 ns | 0.312 ns | 0.0105 |     176 B |
| EmptyApiServer              |  44.73 ns |  8.193 ns | 0.449 ns | 0.0124 |     208 B |
| EmptyApiServerVariable      |  30.25 ns | 10.108 ns | 0.554 ns | 0.0076 |     128 B |
| EmptyApiTag                 |  27.62 ns |  1.200 ns | 0.066 ns | 0.0086 |     144 B |
