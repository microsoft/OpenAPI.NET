```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.61GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]   : .NET 8.0.28 (8.0.28, 8.0.2826.26413), X64 RyuJIT x86-64-v3
  ShortRun : .NET 8.0.28 (8.0.28, 8.0.2826.26413), X64 RyuJIT x86-64-v3

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean         | Error      | StdDev    | Gen0   | Allocated |
|---------------------------- |-------------:|-----------:|----------:|-------:|----------:|
| EmptyApiCallback            |     6.153 ns |  1.5524 ns | 0.0851 ns | 0.0019 |      32 B |
| EmptyApiComponents          |    13.051 ns | 12.0445 ns | 0.6602 ns | 0.0067 |     112 B |
| EmptyApiContact             |     7.470 ns |  6.2102 ns | 0.3404 ns | 0.0029 |      48 B |
| EmptyApiDiscriminator       |    12.672 ns | 13.5456 ns | 0.7425 ns | 0.0029 |      48 B |
| EmptyDocument               | 1,125.095 ns | 84.1354 ns | 4.6117 ns | 0.0687 |    1160 B |
| EmptyApiEncoding            |    11.062 ns |  4.7529 ns | 0.2605 ns | 0.0048 |      80 B |
| EmptyApiExample             |     7.975 ns |  1.2050 ns | 0.0661 ns | 0.0043 |      72 B |
| EmptyApiExternalDocs        |    13.252 ns | 10.5544 ns | 0.5785 ns | 0.0024 |      40 B |
| EmptyApiHeader              |    14.924 ns |  7.4238 ns | 0.4069 ns | 0.0048 |      80 B |
| EmptyApiInfo                |    15.859 ns | 17.8903 ns | 0.9806 ns | 0.0048 |      80 B |
| EmptyApiLicense             |     9.133 ns | 27.3655 ns | 1.5000 ns | 0.0029 |      48 B |
| EmptyApiLink                |    15.451 ns | 14.1755 ns | 0.7770 ns | 0.0043 |      72 B |
| EmptyApiMediaType           |    10.330 ns | 10.0944 ns | 0.5533 ns | 0.0048 |      80 B |
| EmptyApiOAuthFlow           |    11.289 ns | 35.2986 ns | 1.9348 ns | 0.0038 |      64 B |
| EmptyApiOAuthFlows          |     8.992 ns |  7.2445 ns | 0.3971 ns | 0.0038 |      64 B |
| EmptyApiOperation           |    71.454 ns | 10.5799 ns | 0.5799 ns | 0.0224 |     376 B |
| EmptyApiParameter           |    14.482 ns | 13.1230 ns | 0.7193 ns | 0.0057 |      96 B |
| EmptyApiPathItem            |    10.780 ns | 20.0935 ns | 1.1014 ns | 0.0038 |      64 B |
| EmptyApiPaths               |    64.184 ns |  7.0932 ns | 0.3888 ns | 0.0148 |     248 B |
| EmptyApiRequestBody         |     7.130 ns |  1.7039 ns | 0.0934 ns | 0.0029 |      48 B |
| EmptyApiResponse            |    10.824 ns | 31.3218 ns | 1.7169 ns | 0.0038 |      64 B |
| EmptyApiResponses           |    57.075 ns |  2.8416 ns | 0.1558 ns | 0.0148 |     248 B |
| EmptyApiSchema              |    22.121 ns |  6.6296 ns | 0.3634 ns | 0.0306 |     512 B |
| EmptyApiSecurityRequirement |    15.320 ns |  1.8307 ns | 0.1003 ns | 0.0062 |     104 B |
| EmptyApiSecurityScheme      |    14.596 ns | 36.8580 ns | 2.0203 ns | 0.0062 |     104 B |
| EmptyApiServer              |     7.890 ns |  2.0641 ns | 0.1131 ns | 0.0033 |      56 B |
| EmptyApiServerVariable      |     8.140 ns |  0.7516 ns | 0.0412 ns | 0.0029 |      48 B |
| EmptyApiTag                 |    10.418 ns | 11.4961 ns | 0.6301 ns | 0.0043 |      72 B |
