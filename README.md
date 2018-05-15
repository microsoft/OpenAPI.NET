![Category overview screenshot](docs/images/oainet.png "Microsoft + OpenAPI = Love")

# OpenAPI.NET

The **OpenAPI.NET** SDK contains a useful object model for OpenAPI documents in .NET along with common serializers to extract raw OpenAPI JSON and YAML documents from the model.

**See more information on the OpenAPI specification and its history here: <a href="https://www.openapis.org">Open API Initiative</a>**

Project Objectives 

- Provide a single shared object model in .NET for OpenAPI descriptions.
- Include the most primitive Reader for ingesting OpenAPI JSON and YAML documents in both V2 and V3 formats.
- Provide OpenAPI description writers for both V2 and V3 specification formats.
- Enable developers to create Readers that translate different data formats into OpenAPI descriptions. 

# Processors
The OpenAPI.NET project holds the base object model for representing OpenAPI documents as .NET objects. Some developers have found the need to write processors that convert other data formats into this OpenAPI.NET object model. We'd like to curate that list of processors in this section of the readme. 

The base JSON and YAML processors are built into this project. Below is the list of the other supported processor projects.

- [**C# Comment / Annotation Processor**](https://github.com/Microsoft/OpenAPI.NET.CSharpAnnotations) : Converts standard .NET annotations ( /// comments ) emitted from your build (MSBuild.exe) into OpenAPI.NET document object. 

# Example Usage

Creating an OpenAPI Document

```C#
var document = new OpenApiDocument
{
    Info = new OpenApiInfo
    {
        Version = "1.0.0",
        Title = "Swagger Petstore (Simple)",
    },
    Servers = new List<OpenApiServer>
    {
        new OpenApiServer { Url = "http://petstore.swagger.io/api" }
    },
    Paths = new OpenApiPaths
    {
        ["/pets"] = new OpenApiPathItem
        {
            Operations = new Dictionary<OperationType, OpenApiOperation>
            {
                [OperationType.Get] = new OpenApiOperation
                {
                    Description = "Returns all pets from the system that the user has access to",
                    Responses = new OpenApiResponses
                    {
                        ["200"] = new OpenApiResponse
                        {
                            Description = "OK"
                        }
                    }
                }
            }
        }
    }
};
```

Reading and writing a OpenAPI description

```C#
var httpClient = new HttpClient
{
    BaseAddress = new Uri("https://raw.githubusercontent.com/OAI/OpenAPI-Specification/")
};

var stream = await httpClient.GetStreamAsync("master/examples/v3.0/petstore.yaml");

// Read V3 as YAML
var openApiDocument = new OpenApiStreamReader().Read(stream, out var diagnostic);

// Write V2 as JSON
var outputString = openApiDocument.Serialize(OpenApiSpecVersion.OpenApi2_0, OpenApiFormat.Json);

```

# Build Status

|**master**|
|--|
|[![Build status](https://ci.appveyor.com/api/projects/status/9l6hly3vjeu0tmtx/branch/master?svg=true)](https://ci.appveyor.com/project/MicrosoftOpenAPINETAdmin/openapi-net-54e7i/branch/master)|

# Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

To provide feedback and ask questions you can use Stack Overflow with the [OpenAPI.NET](https://stackoverflow.com/questions/tagged/openapi.net) tag or use the OpenAPI.NET Slack channel which you can join by registering for the HTTP APIs team at http://slack.httpapis.com.
