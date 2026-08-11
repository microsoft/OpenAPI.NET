```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
INTEL XEON PLATINUM 8573C 2.30GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]   : .NET 8.0.29 (8.0.29, 8.0.2926.32403), X64 RyuJIT x86-64-v4
  ShortRun : .NET 8.0.29 (8.0.29, 8.0.2926.32403), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean       | Error      | StdDev    | Gen0   | Allocated |
|---------------------------- |-----------:|-----------:|----------:|-------:|----------:|
| EmptyApiCallback            |   6.207 ns |   2.846 ns | 0.1560 ns | 0.0004 |      32 B |
| EmptyApiComponents          |  12.866 ns |   6.322 ns | 0.3465 ns | 0.0013 |     112 B |
| EmptyApiContact             |   7.801 ns |   3.348 ns | 0.1835 ns | 0.0006 |      48 B |
| EmptyApiDiscriminator       |   9.036 ns |   1.814 ns | 0.0994 ns | 0.0006 |      48 B |
| EmptyDocument               | 949.884 ns | 131.775 ns | 7.2230 ns | 0.0134 |    1160 B |
| EmptyApiEncoding            |  10.494 ns |  10.483 ns | 0.5746 ns | 0.0010 |      80 B |
| EmptyApiExample             |   9.508 ns |   4.205 ns | 0.2305 ns | 0.0008 |      72 B |
| EmptyApiExternalDocs        |   7.966 ns |   2.534 ns | 0.1389 ns | 0.0005 |      40 B |
| EmptyApiHeader              |   9.715 ns |   2.184 ns | 0.1197 ns | 0.0010 |      80 B |
| EmptyApiInfo                |   9.847 ns |   1.993 ns | 0.1092 ns | 0.0010 |      80 B |
| EmptyApiLicense             |   6.927 ns |   1.396 ns | 0.0765 ns | 0.0006 |      48 B |
| EmptyApiLink                |   9.883 ns |   3.282 ns | 0.1799 ns | 0.0008 |      72 B |
| EmptyApiMediaType           |   9.296 ns |   5.169 ns | 0.2833 ns | 0.0010 |      80 B |
| EmptyApiOAuthFlow           |   8.998 ns |   3.281 ns | 0.1798 ns | 0.0008 |      64 B |
| EmptyApiOAuthFlows          |   8.175 ns |   3.971 ns | 0.2177 ns | 0.0008 |      64 B |
| EmptyApiOperation           |  81.671 ns |   8.280 ns | 0.4539 ns | 0.0044 |     376 B |
| EmptyApiParameter           |  10.677 ns |   1.481 ns | 0.0812 ns | 0.0011 |      96 B |
| EmptyApiPathItem            |   7.852 ns |   1.813 ns | 0.0994 ns | 0.0008 |      64 B |
| EmptyApiPaths               |  71.228 ns |  15.296 ns | 0.8384 ns | 0.0029 |     248 B |
| EmptyApiRequestBody         |   8.993 ns |   2.229 ns | 0.1222 ns | 0.0006 |      48 B |
| EmptyApiResponse            |   8.971 ns |   8.853 ns | 0.4853 ns | 0.0008 |      64 B |
| EmptyApiResponses           |  72.011 ns |  11.671 ns | 0.6397 ns | 0.0029 |     248 B |
| EmptyApiSchema              |  40.348 ns |  65.932 ns | 3.6139 ns | 0.0061 |     512 B |
| EmptyApiSecurityRequirement |  20.542 ns |  13.190 ns | 0.7230 ns | 0.0012 |     104 B |
| EmptyApiSecurityScheme      |  12.220 ns |   6.753 ns | 0.3702 ns | 0.0012 |     104 B |
| EmptyApiServer              |   7.975 ns |   6.627 ns | 0.3633 ns | 0.0007 |      56 B |
| EmptyApiServerVariable      |   8.220 ns |   2.321 ns | 0.1272 ns | 0.0006 |      48 B |
| EmptyApiTag                 |  10.240 ns |   7.962 ns | 0.4364 ns | 0.0008 |      72 B |
