The **OpenAPI.NET** SDK is made up of libraries which provide a useful object model for OpenAPI documents in .NET along with common serializers to extract raw OpenAPI JSON and YAML documents from the model. It consists of the following packages:
1. **Microsoft.OpenApi**
    - This package provides a single shared object model in .NET for OpenAPI descriptions as well as writers for both V2 and V3 specification formats.
2. **Microsoft.OpenApi.Readers**
    - Includes the most primitive Reader for ingesting OpenAPI JSON and YAML documents in both V2 and V3 formats and enables developers to create Readers that translate different data formats into OpenAPI descriptions
3. **Microsoft.OpenApi.Hidi**
    - A commandline tool that makes it easy to work with and transform OpenAPI documents. This tool enables you to validate and apply transformations to and from different file formats using various commands to do different actions on the files.

