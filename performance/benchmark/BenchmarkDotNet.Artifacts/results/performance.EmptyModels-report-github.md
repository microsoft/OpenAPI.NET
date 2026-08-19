```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]   : .NET 8.0.30 (8.0.3026.36720), X64 RyuJIT AVX2
  ShortRun : .NET 8.0.30 (8.0.3026.36720), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean      | Error     | StdDev   | Gen0   | Allocated |
|---------------------------- |----------:|----------:|---------:|-------:|----------:|
| EmptyApiCallback            |  48.71 ns | 13.702 ns | 0.751 ns | 0.0124 |     208 B |
| EmptyApiComponents          | 226.57 ns | 45.828 ns | 2.512 ns | 0.0534 |     896 B |
| EmptyApiContact             |  29.46 ns |  3.469 ns | 0.190 ns | 0.0076 |     128 B |
| EmptyApiDiscriminator       |  28.54 ns |  9.155 ns | 0.502 ns | 0.0067 |     112 B |
| EmptyDocument               |  60.29 ns | 16.830 ns | 0.923 ns | 0.0162 |     272 B |
| EmptyApiEncoding            |  51.10 ns | 14.399 ns | 0.789 ns | 0.0129 |     216 B |
| EmptyApiExample             |  31.99 ns |  7.352 ns | 0.403 ns | 0.0091 |     152 B |
| EmptyApiExternalDocs        |  32.39 ns |  5.356 ns | 0.294 ns | 0.0072 |     120 B |
| EmptyApiHeader              |  76.36 ns | 32.340 ns | 1.773 ns | 0.0196 |     328 B |
| EmptyApiInfo                |  30.61 ns |  4.565 ns | 0.250 ns | 0.0091 |     152 B |
| EmptyApiLicense             |  28.90 ns |  7.119 ns | 0.390 ns | 0.0072 |     120 B |
| EmptyApiLink                |  52.01 ns | 11.111 ns | 0.609 ns | 0.0148 |     248 B |
| EmptyApiMediaType           |  73.56 ns |  5.497 ns | 0.301 ns | 0.0176 |     296 B |
| EmptyApiOAuthFlow           |  50.64 ns | 22.684 ns | 1.243 ns | 0.0129 |     216 B |
| EmptyApiOAuthFlows          |  32.67 ns |  6.929 ns | 0.380 ns | 0.0081 |     136 B |
| EmptyApiOperation           | 136.86 ns | 58.824 ns | 3.224 ns | 0.0348 |     584 B |
| EmptyApiParameter           |  75.91 ns | 20.049 ns | 1.099 ns | 0.0205 |     344 B |
| EmptyApiPathItem            |  63.50 ns | 19.078 ns | 1.046 ns | 0.0181 |     304 B |
| EmptyApiPaths               |  42.65 ns | 15.564 ns | 0.853 ns | 0.0100 |     168 B |
| EmptyApiRequestBody         |  59.87 ns | 15.210 ns | 0.834 ns | 0.0129 |     216 B |
| EmptyApiResponse            |  92.62 ns | 27.756 ns | 1.521 ns | 0.0234 |     392 B |
| EmptyApiResponses           |  42.38 ns |  2.652 ns | 0.145 ns | 0.0100 |     168 B |
| EmptyApiSchema              | 124.10 ns | 46.460 ns | 2.547 ns | 0.0415 |     696 B |
| EmptyApiSecurityRequirement |  18.20 ns |  3.986 ns | 0.218 ns | 0.0062 |     104 B |
| EmptyApiSecurityScheme      |  32.71 ns |  3.952 ns | 0.217 ns | 0.0105 |     176 B |
| EmptyApiServer              |  55.11 ns | 29.756 ns | 1.631 ns | 0.0124 |     208 B |
| EmptyApiServerVariable      |  29.69 ns |  4.348 ns | 0.238 ns | 0.0076 |     128 B |
| EmptyApiTag                 |  30.84 ns | 15.907 ns | 0.872 ns | 0.0086 |     144 B |
