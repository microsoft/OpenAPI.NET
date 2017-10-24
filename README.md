<!---
category: OpenAPI REST Swagger
-->

![Category overview screenshot](docs/images/oainet.png "Microsoft + OpenAPI = Love")

# OpenAPI.Net

The **OpenAPI.NET** SDK contains a useful object model for OpenAPI documents in .NET along with common serializers to extract raw OAI JSON and YAML documents from the model.

**See more information on the Open API spec and its history here: <a href="https://www.openapis.org">Open API Initiative</a>**

Project Objectives

- Provide a single shared object model in .NET for Open API documents.
- Include the most primitive Reader for ingesting OAI JSON and YAML documents.
- Enable developers to create Readers that translate different data formats into Open API documents. 

# Readers
The OpenAPI.NET project holds the base object model for representing OAI documents as .NET objects. Translation for different data types into this object model is handled by individual "Readers".

The base JSON and YAML Readers are built into this project as documented "here". Below are a few other custom Readers developed externally. 

- EMD/OData Reader Repo: <a href="https://github.com/Microsoft/OpenAPI.NET.OData.Reader">Link</a>
- .NET Comment Reader Repo: <a href="https://github.com/Microsoft/OpenAPI.NET.CSharpComment.Reader">Link</a>

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
