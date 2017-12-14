![Category overview screenshot](docs/images/oainet.png "Microsoft + OpenAPI = Love")

# OpenAPI.NET [Preview]
[ Disclaimer: This repository is in a preview state. Expect to see some iterating as we work towards the final release candidate slated for early 2018. Feedback is welcome! ]

The **OpenAPI.NET** SDK contains a useful object model for OpenAPI documents in .NET along with common serializers to extract raw OpenAPI JSON and YAML documents from the model.

**See more information on the OpenAPI spec and its history here: <a href="https://www.openapis.org">Open API Initiative</a>**

Project Objectives 

- Provide a single shared object model in .NET for OpenAPI descriptions.
- Include the most primitive Reader for ingesting OpenAPI JSON and YAML documents in both V2 and V3 formats.
- Provide OpenAPI description writers for both V2 and V3 specification formats.
- Enable developers to create Readers that translate different data formats into OpenAPI descriptions. 

# Readers
The OpenAPI.NET project holds the base object model for representing OpenAPI descriptions as .NET objects. Translation for different data types into this object model is handled by reading raw JSON/YAML or from individual "Readers", a number of which are in the works.

The base JSON and YAML Readers are built into this project. Below is the list of supported "reader" projects.

- .NET Comment Reader: [Coming Soon]

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

var outputStringWriter = new StringWriter();
var writer = new OpenApiJsonWriter(outputStringWriter);

// Write V2 as JSON
openApiDocument.SerializeAsV2(writer);

outputStringWriter.Flush();
var output = outputStringWriter.GetStringBuilder().ToString();

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

To provide feedback and ask questions you can use StackOverflow with the [OpenApi.net](https://stackoverflow.com/questions/tagged/openapi.net) tag or use the OpenApi.net Slack channel which you can join by registering for the httpapis team at http://slack.httpapis.com 
