```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3476)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.407
  [Host]     : .NET 8.0.14 (8.0.1425.11118), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  DefaultJob : .NET 8.0.14 (8.0.1425.11118), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI


```
| Method                      | Mean      | Error     | StdDev    | Median    | Gen0   | Gen1   | Allocated |
|---------------------------- |----------:|----------:|----------:|----------:|-------:|-------:|----------:|
| EmptyApiCallback            |  35.02 ns |  0.759 ns |  1.774 ns |  34.41 ns | 0.0306 |      - |     192 B |
| EmptyApiComponents          | 163.68 ns |  3.310 ns |  3.679 ns | 164.19 ns | 0.1566 | 0.0005 |     984 B |
| EmptyApiContact             |  22.22 ns |  0.509 ns |  0.586 ns |  22.11 ns | 0.0204 |      - |     128 B |
| EmptyApiDiscriminator       |  34.50 ns |  0.681 ns |  1.852 ns |  34.00 ns | 0.0318 |      - |     200 B |
| EmptyDocument               | 495.48 ns |  9.284 ns |  7.753 ns | 495.55 ns | 0.2203 | 0.0010 |    1384 B |
| EmptyApiEncoding            |  41.21 ns |  1.649 ns |  4.759 ns |  39.99 ns | 0.0344 |      - |     216 B |
| EmptyApiExample             |  19.63 ns |  0.657 ns |  1.884 ns |  19.06 ns | 0.0217 |      - |     136 B |
| EmptyApiExternalDocs        |  18.69 ns |  0.635 ns |  1.801 ns |  18.02 ns | 0.0191 |      - |     120 B |
| EmptyApiHeader              |  61.05 ns |  3.524 ns |  9.939 ns |  57.82 ns | 0.0510 |      - |     320 B |
| EmptyApiInfo                |  23.43 ns |  1.709 ns |  4.735 ns |  21.61 ns | 0.0255 |      - |     160 B |
| EmptyApiLicense             |  22.52 ns |  1.199 ns |  3.382 ns |  21.72 ns | 0.0204 |      - |     128 B |
| EmptyApiLink                |  49.83 ns |  5.171 ns | 14.919 ns |  44.27 ns | 0.0370 |      - |     232 B |
| EmptyApiMediaType           |  58.59 ns |  2.946 ns |  8.064 ns |  55.92 ns | 0.0471 |      - |     296 B |
| EmptyApiOAuthFlow           |  33.41 ns |  1.019 ns |  2.856 ns |  32.05 ns | 0.0344 |      - |     216 B |
| EmptyApiOAuthFlows          |  20.27 ns |  1.067 ns |  3.045 ns |  19.07 ns | 0.0217 |      - |     136 B |
| EmptyApiOperation           | 109.92 ns |  3.557 ns | 10.031 ns | 107.70 ns | 0.1006 |      - |     632 B |
| EmptyApiParameter           |  60.59 ns |  1.676 ns |  4.616 ns |  59.36 ns | 0.0535 |      - |     336 B |
| EmptyApiPathItem            |  44.55 ns |  1.652 ns |  4.409 ns |  44.04 ns | 0.0459 |      - |     288 B |
| EmptyApiPaths               |  62.88 ns |  2.427 ns |  6.926 ns |  60.98 ns | 0.0395 |      - |     248 B |
| EmptyApiRequestBody         |  42.01 ns |  3.827 ns | 10.919 ns |  39.45 ns | 0.0331 |      - |     208 B |
| EmptyApiResponse            |  81.52 ns |  3.797 ns | 10.708 ns |  78.95 ns | 0.0598 |      - |     376 B |
| EmptyApiResponses           |  69.65 ns |  7.130 ns | 19.756 ns |  61.11 ns | 0.0395 |      - |     248 B |
| EmptyApiSchema              | 139.80 ns | 13.005 ns | 36.038 ns | 127.12 ns | 0.1707 | 0.0005 |    1072 B |
| EmptyApiSecurityRequirement |  11.51 ns |  0.888 ns |  2.474 ns |  11.12 ns | 0.0166 |      - |     104 B |
| EmptyApiSecurityScheme      |  28.91 ns |  2.571 ns |  6.994 ns |  27.50 ns | 0.0268 |      - |     168 B |
| EmptyApiServer              |  61.54 ns |  4.026 ns | 11.680 ns |  60.52 ns | 0.0331 |      - |     208 B |
| EmptyApiServerVariable      |  32.65 ns |  2.534 ns |  7.147 ns |  32.11 ns | 0.0204 |      - |     128 B |
| EmptyApiTag                 |  31.67 ns |  4.230 ns | 12.138 ns |  28.97 ns | 0.0204 |      - |     128 B |
