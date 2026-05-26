```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.300
  [Host]   : .NET 8.0.27 (8.0.27, 8.0.2726.22922), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.27 (8.0.27, 8.0.2726.22922), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean       | Error      | StdDev    | Gen0   | Allocated |
|---------------------------- |-----------:|-----------:|----------:|-------:|----------:|
| EmptyApiCallback            |   7.635 ns |  6.5021 ns | 0.3564 ns | 0.0013 |      32 B |
| EmptyApiComponents          |  14.535 ns |  6.7497 ns | 0.3700 ns | 0.0044 |     112 B |
| EmptyApiContact             |   7.196 ns |  4.1954 ns | 0.2300 ns | 0.0019 |      48 B |
| EmptyApiDiscriminator       |   7.639 ns |  1.8805 ns | 0.1031 ns | 0.0019 |      48 B |
| EmptyDocument               | 962.305 ns | 28.4905 ns | 1.5617 ns | 0.0439 |    1144 B |
| EmptyApiEncoding            |   8.806 ns |  3.8509 ns | 0.2111 ns | 0.0032 |      80 B |
| EmptyApiExample             |  10.131 ns |  2.7802 ns | 0.1524 ns | 0.0029 |      72 B |
| EmptyApiExternalDocs        |   7.945 ns |  3.4747 ns | 0.1905 ns | 0.0016 |      40 B |
| EmptyApiHeader              |  10.667 ns |  4.4840 ns | 0.2458 ns | 0.0032 |      80 B |
| EmptyApiInfo                |   9.665 ns |  7.0473 ns | 0.3863 ns | 0.0032 |      80 B |
| EmptyApiLicense             |   8.097 ns |  1.1216 ns | 0.0615 ns | 0.0019 |      48 B |
| EmptyApiLink                |   8.220 ns |  5.0956 ns | 0.2793 ns | 0.0029 |      72 B |
| EmptyApiMediaType           |   9.239 ns | 10.5225 ns | 0.5768 ns | 0.0032 |      80 B |
| EmptyApiOAuthFlow           |   9.344 ns |  1.9202 ns | 0.1053 ns | 0.0025 |      64 B |
| EmptyApiOAuthFlows          |   9.253 ns |  3.8718 ns | 0.2122 ns | 0.0025 |      64 B |
| EmptyApiOperation           |  90.086 ns | 78.6047 ns | 4.3086 ns | 0.0149 |     376 B |
| EmptyApiParameter           |   9.886 ns | 10.2885 ns | 0.5639 ns | 0.0038 |      96 B |
| EmptyApiPathItem            |   8.590 ns |  0.9367 ns | 0.0513 ns | 0.0025 |      64 B |
| EmptyApiPaths               |  73.841 ns | 44.9274 ns | 2.4626 ns | 0.0098 |     248 B |
| EmptyApiRequestBody         |   9.202 ns |  1.7608 ns | 0.0965 ns | 0.0019 |      48 B |
| EmptyApiResponse            |   9.330 ns |  3.0120 ns | 0.1651 ns | 0.0025 |      64 B |
| EmptyApiResponses           |  71.757 ns | 16.0925 ns | 0.8821 ns | 0.0098 |     248 B |
| EmptyApiSchema              |  30.900 ns | 27.4409 ns | 1.5041 ns | 0.0166 |     416 B |
| EmptyApiSecurityRequirement |  17.229 ns | 14.5380 ns | 0.7969 ns | 0.0041 |     104 B |
| EmptyApiSecurityScheme      |  10.194 ns |  4.5725 ns | 0.2506 ns | 0.0041 |     104 B |
| EmptyApiServer              |   7.765 ns |  5.1924 ns | 0.2846 ns | 0.0022 |      56 B |
| EmptyApiServerVariable      |   7.422 ns |  3.8244 ns | 0.2096 ns | 0.0019 |      48 B |
| EmptyApiTag                 |   9.067 ns |  3.0519 ns | 0.1673 ns | 0.0029 |      72 B |
