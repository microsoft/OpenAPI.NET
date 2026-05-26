```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8457/25H2/2025Update/HudsonValley2)
Snapdragon X 12-core X1E80100 3.40 GHz (Max: 3.42GHz), 1 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.300
  [Host]   : .NET 8.0.27 (8.0.27, 8.0.2726.22922), Arm64 RyuJIT armv8.0-a
  ShortRun : .NET 8.0.27 (8.0.27, 8.0.2726.22922), Arm64 RyuJIT armv8.0-a

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean       | Error       | StdDev    | Gen0   | Allocated |
|---------------------------- |-----------:|------------:|----------:|-------:|----------:|
| EmptyApiCallback            |   1.640 ns |   0.8103 ns | 0.0444 ns | 0.0077 |      32 B |
| EmptyApiComponents          |   3.288 ns |   0.2120 ns | 0.0116 ns | 0.0268 |     112 B |
| EmptyApiContact             |   1.952 ns |   1.2597 ns | 0.0690 ns | 0.0115 |      48 B |
| EmptyApiDiscriminator       |   1.901 ns |   0.3793 ns | 0.0208 ns | 0.0115 |      48 B |
| EmptyDocument               | 258.154 ns | 168.3657 ns | 9.2287 ns | 0.2732 |    1144 B |
| EmptyApiEncoding            |   2.205 ns |   0.8682 ns | 0.0476 ns | 0.0191 |      80 B |
| EmptyApiExample             |   2.153 ns |   1.2697 ns | 0.0696 ns | 0.0172 |      72 B |
| EmptyApiExternalDocs        |   1.521 ns |   0.6641 ns | 0.0364 ns | 0.0096 |      40 B |
| EmptyApiHeader              |   2.550 ns |   3.3121 ns | 0.1815 ns | 0.0191 |      80 B |
| EmptyApiInfo                |   2.671 ns |   1.0929 ns | 0.0599 ns | 0.0191 |      80 B |
| EmptyApiLicense             |   1.939 ns |   0.6162 ns | 0.0338 ns | 0.0115 |      48 B |
| EmptyApiLink                |   2.898 ns |  13.0612 ns | 0.7159 ns | 0.0172 |      72 B |
| EmptyApiMediaType           |   2.561 ns |   0.3901 ns | 0.0214 ns | 0.0191 |      80 B |
| EmptyApiOAuthFlow           |   2.129 ns |   4.0837 ns | 0.2238 ns | 0.0153 |      64 B |
| EmptyApiOAuthFlows          |   2.460 ns |   1.0232 ns | 0.0561 ns | 0.0153 |      64 B |
| EmptyApiOperation           |  45.987 ns |   5.3100 ns | 0.2911 ns | 0.0899 |     376 B |
| EmptyApiParameter           |   2.960 ns |   1.5296 ns | 0.0838 ns | 0.0230 |      96 B |
| EmptyApiPathItem            |   2.370 ns |   0.5273 ns | 0.0289 ns | 0.0153 |      64 B |
| EmptyApiPaths               |  44.908 ns |   8.3727 ns | 0.4589 ns | 0.0592 |     248 B |
| EmptyApiRequestBody         |   1.938 ns |   0.2641 ns | 0.0145 ns | 0.0115 |      48 B |
| EmptyApiResponse            |   2.235 ns |   0.5333 ns | 0.0292 ns | 0.0153 |      64 B |
| EmptyApiResponses           |  43.842 ns |  11.2607 ns | 0.6172 ns | 0.0592 |     248 B |
| EmptyApiSchema              |   9.161 ns |   1.1866 ns | 0.0650 ns | 0.0995 |     416 B |
| EmptyApiSecurityRequirement |   6.938 ns |   0.4257 ns | 0.0233 ns | 0.0249 |     104 B |
| EmptyApiSecurityScheme      |   2.985 ns |   0.3369 ns | 0.0185 ns | 0.0249 |     104 B |
| EmptyApiServer              |   2.085 ns |   0.7051 ns | 0.0386 ns | 0.0134 |      56 B |
| EmptyApiServerVariable      |   1.887 ns |   0.3896 ns | 0.0214 ns | 0.0115 |      48 B |
| EmptyApiTag                 |   2.359 ns |   0.5921 ns | 0.0325 ns | 0.0172 |      72 B |
