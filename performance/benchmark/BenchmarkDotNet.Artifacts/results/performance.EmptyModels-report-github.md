```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8457/25H2/2025Update/HudsonValley2)
Snapdragon X 12-core X1E80100 3.40 GHz (Max: 3.42GHz), 1 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.300
  [Host]   : .NET 8.0.27 (8.0.27, 8.0.2726.22922), Arm64 RyuJIT armv8.0-a
  ShortRun : .NET 8.0.27 (8.0.27, 8.0.2726.22922), Arm64 RyuJIT armv8.0-a

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean       | Error      | StdDev    | Gen0   | Allocated |
|---------------------------- |-----------:|-----------:|----------:|-------:|----------:|
| EmptyApiCallback            |   2.184 ns |  1.0483 ns | 0.0575 ns | 0.0077 |      32 B |
| EmptyApiComponents          |   4.331 ns |  0.7695 ns | 0.0422 ns | 0.0268 |     112 B |
| EmptyApiContact             |   2.539 ns |  1.0312 ns | 0.0565 ns | 0.0115 |      48 B |
| EmptyApiDiscriminator       |   2.575 ns |  1.1737 ns | 0.0643 ns | 0.0115 |      48 B |
| EmptyDocument               | 403.466 ns | 10.0468 ns | 0.5507 ns | 0.2732 |    1144 B |
| EmptyApiEncoding            |   3.368 ns |  0.1769 ns | 0.0097 ns | 0.0191 |      80 B |
| EmptyApiExample             |   3.245 ns |  0.4273 ns | 0.0234 ns | 0.0172 |      72 B |
| EmptyApiExternalDocs        |   2.326 ns |  0.4220 ns | 0.0231 ns | 0.0096 |      40 B |
| EmptyApiHeader              |   3.342 ns |  0.1606 ns | 0.0088 ns | 0.0191 |      80 B |
| EmptyApiInfo                |   3.347 ns |  0.5774 ns | 0.0317 ns | 0.0191 |      80 B |
| EmptyApiLicense             |   2.707 ns |  0.5576 ns | 0.0306 ns | 0.0115 |      48 B |
| EmptyApiLink                |   3.301 ns |  1.8443 ns | 0.1011 ns | 0.0172 |      72 B |
| EmptyApiMediaType           |   3.659 ns |  1.9830 ns | 0.1087 ns | 0.0191 |      80 B |
| EmptyApiOAuthFlow           |   3.344 ns |  0.0587 ns | 0.0032 ns | 0.0153 |      64 B |
| EmptyApiOAuthFlows          |   3.312 ns |  2.9546 ns | 0.1620 ns | 0.0153 |      64 B |
| EmptyApiOperation           |  65.559 ns | 12.2926 ns | 0.6738 ns | 0.0899 |     376 B |
| EmptyApiParameter           |   4.012 ns |  0.6116 ns | 0.0335 ns | 0.0229 |      96 B |
| EmptyApiPathItem            |   3.155 ns |  1.7310 ns | 0.0949 ns | 0.0153 |      64 B |
| EmptyApiPaths               |  58.748 ns | 10.8771 ns | 0.5962 ns | 0.0592 |     248 B |
| EmptyApiRequestBody         |   2.643 ns |  0.4460 ns | 0.0244 ns | 0.0115 |      48 B |
| EmptyApiResponse            |   3.199 ns |  1.4671 ns | 0.0804 ns | 0.0153 |      64 B |
| EmptyApiResponses           |  58.041 ns |  8.1676 ns | 0.4477 ns | 0.0592 |     248 B |
| EmptyApiSchema              |  14.630 ns |  1.9349 ns | 0.1061 ns | 0.1224 |     512 B |
| EmptyApiSecurityRequirement |   9.473 ns |  4.8300 ns | 0.2647 ns | 0.0249 |     104 B |
| EmptyApiSecurityScheme      |   4.202 ns |  2.3228 ns | 0.1273 ns | 0.0249 |     104 B |
| EmptyApiServer              |   2.960 ns |  3.0384 ns | 0.1665 ns | 0.0134 |      56 B |
| EmptyApiServerVariable      |   2.755 ns |  1.2095 ns | 0.0663 ns | 0.0115 |      48 B |
| EmptyApiTag                 |   3.341 ns |  0.6456 ns | 0.0354 ns | 0.0172 |      72 B |
