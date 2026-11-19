```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9711)
Snapdragon X 12-core X1E80100 3.40 GHz (Max: 3.42GHz), 1 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.403
  [Host]   : .NET 8.0.34 (8.0.34, 8.0.3426.52217), Arm64 RyuJIT armv8.0-a
  ShortRun : .NET 8.0.34 (8.0.34, 8.0.3426.52217), Arm64 RyuJIT armv8.0-a

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean       | Error      | StdDev    | Gen0   | Allocated |
|---------------------------- |-----------:|-----------:|----------:|-------:|----------:|
| EmptyApiCallback            |   1.845 ns |  0.1053 ns | 0.0058 ns | 0.0077 |      32 B |
| EmptyApiComponents          |   3.570 ns |  0.5665 ns | 0.0311 ns | 0.0268 |     112 B |
| EmptyApiContact             |   2.344 ns |  0.6267 ns | 0.0344 ns | 0.0115 |      48 B |
| EmptyApiDiscriminator       |   2.218 ns |  0.3551 ns | 0.0195 ns | 0.0115 |      48 B |
| EmptyDocument               | 334.798 ns | 21.1381 ns | 1.1586 ns | 0.2770 |    1160 B |
| EmptyApiEncoding            |   2.884 ns |  0.5880 ns | 0.0322 ns | 0.0191 |      80 B |
| EmptyApiExample             |   2.710 ns |  0.9338 ns | 0.0512 ns | 0.0172 |      72 B |
| EmptyApiExternalDocs        |   2.209 ns |  0.2993 ns | 0.0164 ns | 0.0096 |      40 B |
| EmptyApiHeader              |   3.315 ns |  1.2197 ns | 0.0669 ns | 0.0191 |      80 B |
| EmptyApiInfo                |   3.384 ns |  1.6441 ns | 0.0901 ns | 0.0191 |      80 B |
| EmptyApiLicense             |   2.436 ns |  1.7140 ns | 0.0940 ns | 0.0115 |      48 B |
| EmptyApiLink                |   3.212 ns |  1.0237 ns | 0.0561 ns | 0.0172 |      72 B |
| EmptyApiMediaType           |   3.344 ns |  0.6312 ns | 0.0346 ns | 0.0191 |      80 B |
| EmptyApiOAuthFlow           |   2.923 ns |  0.4818 ns | 0.0264 ns | 0.0153 |      64 B |
| EmptyApiOAuthFlows          |   2.698 ns |  0.1985 ns | 0.0109 ns | 0.0153 |      64 B |
| EmptyApiOperation           |  58.960 ns | 19.9311 ns | 1.0925 ns | 0.0899 |     376 B |
| EmptyApiParameter           |   3.491 ns |  1.5783 ns | 0.0865 ns | 0.0230 |      96 B |
| EmptyApiPathItem            |   2.683 ns |  1.3188 ns | 0.0723 ns | 0.0153 |      64 B |
| EmptyApiPaths               |  51.804 ns |  3.0372 ns | 0.1665 ns | 0.0592 |     248 B |
| EmptyApiRequestBody         |   2.452 ns |  0.1408 ns | 0.0077 ns | 0.0115 |      48 B |
| EmptyApiResponse            |   2.851 ns |  0.4788 ns | 0.0262 ns | 0.0153 |      64 B |
| EmptyApiResponses           |  52.826 ns |  1.9879 ns | 0.1090 ns | 0.0592 |     248 B |
| EmptyApiSchema              |  13.187 ns |  1.2161 ns | 0.0667 ns | 0.1224 |     512 B |
| EmptyApiSecurityRequirement |   8.793 ns |  0.7326 ns | 0.0402 ns | 0.0249 |     104 B |
| EmptyApiSecurityScheme      |   3.719 ns |  0.3875 ns | 0.0212 ns | 0.0249 |     104 B |
| EmptyApiServer              |   2.677 ns |  0.5607 ns | 0.0307 ns | 0.0134 |      56 B |
| EmptyApiServerVariable      |   2.489 ns |  1.0348 ns | 0.0567 ns | 0.0115 |      48 B |
| EmptyApiTag                 |   3.046 ns |  1.0251 ns | 0.0562 ns | 0.0172 |      72 B |
