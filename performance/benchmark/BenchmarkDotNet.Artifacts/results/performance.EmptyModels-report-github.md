```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.303
  [Host]   : .NET 8.0.30 (8.0.3026.36720), X64 RyuJIT AVX2
  ShortRun : .NET 8.0.30 (8.0.3026.36720), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean      | Error     | StdDev   | Gen0   | Allocated |
|---------------------------- |----------:|----------:|---------:|-------:|----------:|
| EmptyApiCallback            |  44.87 ns | 10.770 ns | 0.590 ns | 0.0124 |     208 B |
| EmptyApiComponents          | 216.48 ns | 24.892 ns | 1.364 ns | 0.0534 |     896 B |
| EmptyApiContact             |  26.06 ns |  5.204 ns | 0.285 ns | 0.0076 |     128 B |
| EmptyApiDiscriminator       |  25.93 ns |  6.535 ns | 0.358 ns | 0.0067 |     112 B |
| EmptyDocument               |  55.15 ns |  5.381 ns | 0.295 ns | 0.0162 |     272 B |
| EmptyApiEncoding            |  48.22 ns |  5.799 ns | 0.318 ns | 0.0129 |     216 B |
| EmptyApiExample             |  26.79 ns |  2.322 ns | 0.127 ns | 0.0091 |     152 B |
| EmptyApiExternalDocs        |  26.06 ns | 11.687 ns | 0.641 ns | 0.0072 |     120 B |
| EmptyApiHeader              |  70.64 ns |  6.176 ns | 0.339 ns | 0.0196 |     328 B |
| EmptyApiInfo                |  26.86 ns |  5.582 ns | 0.306 ns | 0.0091 |     152 B |
| EmptyApiLicense             |  25.97 ns |  2.916 ns | 0.160 ns | 0.0072 |     120 B |
| EmptyApiLink                |  47.76 ns | 10.516 ns | 0.576 ns | 0.0148 |     248 B |
| EmptyApiMediaType           |  67.82 ns |  2.106 ns | 0.115 ns | 0.0176 |     296 B |
| EmptyApiOAuthFlow           |  47.46 ns |  9.129 ns | 0.500 ns | 0.0129 |     216 B |
| EmptyApiOAuthFlows          |  26.28 ns |  1.983 ns | 0.109 ns | 0.0081 |     136 B |
| EmptyApiOperation           | 126.73 ns | 11.801 ns | 0.647 ns | 0.0348 |     584 B |
| EmptyApiParameter           |  70.00 ns |  5.170 ns | 0.283 ns | 0.0205 |     344 B |
| EmptyApiPathItem            |  57.68 ns | 16.673 ns | 0.914 ns | 0.0181 |     304 B |
| EmptyApiPaths               |  38.43 ns |  1.234 ns | 0.068 ns | 0.0100 |     168 B |
| EmptyApiRequestBody         |  47.15 ns | 11.232 ns | 0.616 ns | 0.0129 |     216 B |
| EmptyApiResponse            |  85.97 ns | 10.296 ns | 0.564 ns | 0.0234 |     392 B |
| EmptyApiResponses           |  38.69 ns |  4.161 ns | 0.228 ns | 0.0100 |     168 B |
| EmptyApiSchema              | 107.59 ns | 59.013 ns | 3.235 ns | 0.0416 |     696 B |
| EmptyApiSecurityRequirement |  15.03 ns |  2.621 ns | 0.144 ns | 0.0062 |     104 B |
| EmptyApiSecurityScheme      |  27.64 ns |  8.290 ns | 0.454 ns | 0.0105 |     176 B |
| EmptyApiServer              |  47.55 ns | 10.684 ns | 0.586 ns | 0.0124 |     208 B |
| EmptyApiServerVariable      |  26.72 ns |  6.635 ns | 0.364 ns | 0.0076 |     128 B |
| EmptyApiTag                 |  26.26 ns |  4.015 ns | 0.220 ns | 0.0086 |     144 B |
