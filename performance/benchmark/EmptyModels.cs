using System;
using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using Microsoft.OpenApi.Models;

namespace performance;
[MemoryDiagnoser]
[JsonExporter]
[ShortRunJob]
// [SimpleJob(launchCount: 1, warmupCount: 30, iterationCount: 50, invocationCount:1000)]
public class EmptyModels
{
    [Benchmark]
    public OpenApiCallback EmptyApiCallback()
    {
        return new OpenApiCallback();
    }
    [Benchmark]
    public OpenApiComponents EmptyApiComponents()
    {
        return new OpenApiComponents();
    }
    [Benchmark]
    public OpenApiContact EmptyApiContact()
    {
        return new OpenApiContact();
    }
    [Benchmark]
    public OpenApiDiscriminator EmptyApiDiscriminator()
    {
        return new OpenApiDiscriminator();
    }
    [Benchmark]
    public OpenApiDocument EmptyDocument()
    {
        return new OpenApiDocument();
    }
    [Benchmark]
    public OpenApiEncoding EmptyApiEncoding()
    {
        return new OpenApiEncoding();
    }
    [Benchmark]
    public OpenApiExample EmptyApiExample()
    {
        return new OpenApiExample();
    }
    [Benchmark]
    public OpenApiExternalDocs EmptyApiExternalDocs()
    {
        return new OpenApiExternalDocs();
    }
    [Benchmark]
    public OpenApiHeader EmptyApiHeader()
    {
        return new OpenApiHeader();
    }
    [Benchmark]
    public OpenApiInfo EmptyApiInfo()
    {
        return new OpenApiInfo();
    }
    [Benchmark]
    public OpenApiLicense EmptyApiLicense()
    {
        return new OpenApiLicense();
    }
    [Benchmark]
    public OpenApiLink EmptyApiLink()
    {
        return new OpenApiLink();
    }
    [Benchmark]
    public OpenApiMediaType EmptyApiMediaType()
    {
        return new OpenApiMediaType();
    }
    [Benchmark]
    public OpenApiOAuthFlow EmptyApiOAuthFlow()
    {
        return new OpenApiOAuthFlow();
    }
    [Benchmark]
    public OpenApiOAuthFlows EmptyApiOAuthFlows()
    {
        return new OpenApiOAuthFlows();
    }
    [Benchmark]
    public OpenApiOperation EmptyApiOperation()
    {
        return new OpenApiOperation();
    }
    [Benchmark]
    public OpenApiParameter EmptyApiParameter()
    {
        return new OpenApiParameter();
    }
    [Benchmark]
    public OpenApiPathItem EmptyApiPathItem()
    {
        return new OpenApiPathItem();
    }
    [Benchmark]
    public OpenApiPaths EmptyApiPaths()
    {
        return new OpenApiPaths();
    }
    [Benchmark]
    public OpenApiRequestBody EmptyApiRequestBody()
    {
        return new OpenApiRequestBody();
    }
    [Benchmark]
    public OpenApiResponse EmptyApiResponse()
    {
        return new OpenApiResponse();
    }
    [Benchmark]
    public OpenApiResponses EmptyApiResponses()
    {
        return new OpenApiResponses();
    }
    [Benchmark]
    public OpenApiSchema EmptyApiSchema()
    {
        return new OpenApiSchema();
    }
    [Benchmark]
    public OpenApiSecurityRequirement EmptyApiSecurityRequirement()
    {
        return new OpenApiSecurityRequirement();
    }
    [Benchmark]
    public OpenApiSecurityScheme EmptyApiSecurityScheme()
    {
        return new OpenApiSecurityScheme();
    }
    [Benchmark]
    public OpenApiServer EmptyApiServer()
    {
        return new OpenApiServer();
    }
    [Benchmark]
    public OpenApiServerVariable EmptyApiServerVariable()
    {
        return new OpenApiServerVariable();
    }
    [Benchmark]
    public OpenApiTag EmptyApiTag()
    {
        return new OpenApiTag();
    }
}
