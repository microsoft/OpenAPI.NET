---
title: Upgrade guide to OpenAPI.NET 2.1 
description: Learn how to upgrade your OpenAPI.NET version from 1.6 to 2.0 
author: rachit.malik
ms.author: malikrachit
ms.topic: conceptual
---

# Introduction

We are excited to announce the preview of a new version of the OpenAPI.NET library!  
OpenAPI.NET v2 is a major update to the OpenAPI.NET library. This release includes a number of performance improvements, API enhancements, and support for OpenAPI v3.1.

## The biggest update ever

Since the release of the first version of the OpenAPI.NET library in 2018, there has not been a major version update to the library. With the addition of support for OpenAPI v3.1 it was necessary to make some breaking changes. With this opportunity, we have taken the time to make some other improvements to the library, based on the experience we have gained supporting a large community of users for the last six years .

## Performance Improvements

One of the key features of OpenAPI.NET is its performance. This version makes it possible to parse JSON based OpenAPI descriptions even faster. OpenAPI.NET v1 relied on the excellent YamlSharp library for parsing both JSON and YAML files. With OpenAPI.NET v2 we are relying on System.Text.Json for parsing JSON files. For YAML files, we continue to use YamlSharp to parse YAML but then convert to JsonNodes for processing. This allows us to take advantage of the performance improvements in System.Text.Json while still supporting YAML files.

In v1, instances of `$ref` were resolved in a second pass of the document to ensure the target of the reference has been parsed before attempting to resolve it. In v2, reference targets are lazily resolved when reference objects are accessed. This improves load time performance for documents that make heavy use of references.

[How does this change the behaviour of external references?]

### Results

The following benchmark results outline an overall 50% reduction in processing time for the document parsing as well as 35% reduction in memory allocation when parsing JSON.
For YAML, the results between the different versions of the library are similar (some of the optimizations being compensated by the additional features).

#### 1.X

| Method       | Mean           | Error        | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |---------------:|-------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |       448.7 μs |     326.6 μs |     17.90 μs |    58.5938 |    11.7188 |         - |    381.79 KB |
| PetStoreJson |       484.8 μs |     156.9 μs |      8.60 μs |    62.5000 |    15.6250 |         - |    389.28 KB |
| GHESYaml     | 1,008,349.6 μs | 565,392.0 μs | 30,991.04 μs | 66000.0000 | 23000.0000 | 4000.0000 |    382785 KB |
| GHESJson     | 1,039,447.0 μs | 267,501.0 μs | 14,662.63 μs | 67000.0000 | 23000.0000 | 4000.0000 | 389970.77 KB |

#### 2.X

| Method       | Mean         | Error         | StdDev       | Gen0       | Gen1       | Gen2      | Allocated    |
|------------- |-------------:|--------------:|-------------:|-----------:|-----------:|----------:|-------------:|
| PetStoreYaml |     450.5 μs |      59.26 μs |      3.25 μs |    58.5938 |    11.7188 |         - |    377.15 KB |
| PetStoreJson |     172.8 μs |     123.46 μs |      6.77 μs |    39.0625 |     7.8125 |         - |    239.29 KB |
| GHESYaml     | 943,452.7 μs | 137,685.49 μs |  7,547.01 μs | 66000.0000 | 21000.0000 | 3000.0000 | 389463.91 KB |
| GHESJson     | 468,401.8 μs | 300,711.80 μs | 16,483.03 μs | 41000.0000 | 15000.0000 | 3000.0000 | 250934.62 KB |

### Asynchronous API surface

Any method which results in input/output access (memory, network, storage) is now Async and returns a `Task<Result>` to avoid any blocking calls an improve concurrency.

For example:

```csharp
var result = myOperation.SerializeAsJson(OpenApiSpecVersion.OpenApi2_0);
```

Is now:

```csharp
var result = await myOperation.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi2_0);
```

### Trimming support

To better support applications deployed in high performance environments or on devices which have limited compute available, any usage of reflection has been removed from the code base. This also brings support for trimming to the library. Any method relying on reflection has been removed or re-written.

> Note: as part of this change, the following types have been removed:
>
> - StringExtensions

### Collections are not initialized

To lower the memory footprint of the library, collections are now NOT initialized anymore when instantiating any of the models.

Example

```csharp
var mySchema = new OpenApiSchema();

// 1.6: works
// 2.X: if null reference types is enabled in the target application,
//      this will lead to a warning or error at compile time.
//      And fail at runtime with a null reference exception.
mySchema.AnyOf.Add(otherSchema);

// one solution
mySchema.AnyOf ??= [];
mySchema.AnyOf.Add(otherSchema);

// alternative
mySchema.AnyOf = [otherSchema];
```

## Reduced Dependencies

In OpenAPI v1, it was necessary to include the Microsoft.OpenApi.Readers library to be able to read OpenAPI descriptions in either YAML or JSON.  In OpenAPI.NET v2, the core Microsoft.OpenAPI library can both read and write JSON.  It is only necessary to use the newly renamed [Microsoft.OpenApi.YamlReader](https://www.nuget.org/packages/Microsoft.OpenApi.YamlReader/) library if you need YAML support. This allows teams who are only working in JSON to avoid the additional dependency and therefore eliminate all non-.NET library references.

Once the dependency is added, the reader needs to be added to the reader settings as demonstrated below

```csharp
var settings = new OpenApiReaderSettings();  
settings.AddYamlReader();  

var result = OpenApiDocument.LoadAsync(openApiString, settings: settings); 
```

## API Enhancements

### Loading the document

The v1 library attempted to mimic the pattern of `XmlTextReader` and `JsonTextReader` for the purpose of loading OpenAPI documents from strings, streams and text readers.

```csharp
var reader = new OpenApiStringReader();
var openApiDoc = reader.Read(stringOpenApiDoc, out var diagnostic);
```

The same pattern can be used for `OpenApiStreamReader` and `OpenApiTextReader`.  When we introduced the `ReadAsync` methods we eliminated the use of the `out` parameter. To improve code readability, we've added deconstruction support to `ReadResult`. The properties also have been renamed to avoid confusion with their types.

```csharp
var reader = new OpenApiStreamReader();
var (document, diagnostics) = await reader.ReadAsync(streamOpenApiDoc);
// or
var result = await reader.ReadAsync(streamOpenApiDoc);
var document = result.Document;
var diagnostics = result.Diagnostics;
```

A `ReadResult` object acts as a tuple of `OpenApiDocument` and `OpenApiDiagnostic`.

The challenge with this approach is that the reader classes are not very discoverable and the behaviour is not actually consistent with the `*TextReader` pattern that allows incrementally reading the document. This library does not support incrementally reading the OpenAPI Document. It only reads a complete document and returns an `OpenApiDocument` instance.

In the v2 library we are moving to the pattern used by classes like `XDocument` where a set of static `Load` and `Parse` methods are used as factory methods.

```csharp
public class OpenApiDocument {
    public static async Task<ReadResult> LoadAsync(string url, OpenApiReaderSettings settings = null) {}
    public static async Task<ReadResult> LoadAsync(Stream stream, string? format = null, OpenApiReaderSettings? settings = null) {}
    public static ReadResult Load(MemoryStream stream, string? format = null, OpenApiReaderSettings? settings = null) {}
    public static ReadResult Parse(string input, string? format = null, OpenApiReaderSettings? settings = null) {}
}
```

This API design allows a developer to use IDE autocomplete to present all the loading options by simply knowing the name of the `OpenApiDocument` class.  Each of these methods are layered on top of the more primitive methods to ensure consistent behaviour.

As the YAML format is only supported when including the `Microsoft.OpenApi.YamlReader` library it was decided not to use an enum for the `format` parameter.  We are considering implementing a more [strongly typed solution](https://github.com/microsoft/OpenAPI.NET/issues/1952) similar to the way that `HttpMethod` is implemented so that we have a strongly typed experience that is also extensible.

When the loading methods are used without a format parameter, we will attempt to parse the document using the default JSON reader.  If that fails and the YAML reader is registered, then we will attempt to read as YAML.  The goal is always to provide the fastest path with JSON but still maintain the convenience of not having to care whether a URL points to YAML or JSON if you need that flexibility.

### Additional exceptions

While parsing an OpenAPI description, the library will now throw the following new exceptions:

- `OpenApiReaderException` when the reader for the format cannot be found, the document cannot be parsed because it does not follow the format conventions, etc...
- `OpenApiUnsupportedSpecVersionException` when the document's version is not implemented by this version of the library and therefore cannot be parsed.

### Removing the OpenAPI Any classes

In the OpenAPI specification, there are a few properties that are defined as type `any`. This includes:

- the example property in the parameter, media type objects
- the value property in the example object
- the values in the link object's parameters dictionary and `requestBody` property
- all `x-` extension properties

In the v1 library, there are a set of classes that are derived from the `OpenApiAny` base class which is an abstract model which reflects the JSON data model plus some additional primitive types such as `decimal`, `float`, `datetime` etc.

In v2 we are removing this abstraction and relying on the `JsonNode` model to represent these inner types. In v1 we were not able to reliably identify the additional primitive types and it caused a significant amount of false negatives in error reporting as well as incorrectly parsed data values.

Due to `JsonNode` implicit operators, this makes initialization sometimes easier, instead of:

```csharp
new OpenApiParameter
{
    In = null,
    Name = "username",
    Description = "username to fetch",
    Example = new OpenApiFloat(5),
};
```

the assignment becomes simply,

```csharp
    Example = 0.5f,
```

For a more complex example, where the developer wants to create an extension that is an object they would do this in v1:

```csharp
var openApiObject = new OpenApiObject
{
    {"stringProp", new OpenApiString("stringValue1")},
    {"objProp", new OpenApiObject()},
    {
        "arrayProp",
        new OpenApiArray
        {
            new OpenApiBoolean(false)
        }
    }
};
var parameter = new OpenApiParameter();
parameter.Extensions.Add("x-foo", openApiObject);

```

In v2, the equivalent code would be,

```csharp
var openApiObject = new JsonObject
{
    {"stringProp", "stringValue1"},
    {"objProp", new JsonObject()},
    {
        "arrayProp",
        new JsonArray
        {
            false
        }
    }
};
var parameter = new OpenApiParameter();
parameter.Extensions.Add("x-foo", new JsonNodeExtension(openApiObject));

```

> Note: as part of this change, the following types have been removed from the library:
>
> - AnyType
> - IOpenApiAny
> - OpenApiAnyCloneHelper
> - OpenApiArray
> - OpenApiBinary
> - OpenApiBoolean
> - OpenApiByte
> - OpenApiDate
> - OpenApiDateTime
> - OpenApiDouble
> - OpenApiFloat
> - OpenApiInteger
> - OpenApiLong
> - OpenApiNull
> - OpenApiObject
> - OpenApiPassword
> - OpenApiPrimitive
> - OpenApiString
> - PrimitiveType

### Enable Null Reference Type Support

Version 2.0 preview 13 introduces support for null reference types, which improves type safety and reduces the likelihood of null reference exceptions.

**Example:**

```csharp
var document = new OpenApiDocument
{
    Components = null
};

// 1.X: no compilation error or warning, but fails with a null reference exception at runtime
// 2.X: compilation error or warning depending on the project configuration
var componentA = document.Components["A"];
```

### Collections are implementations

Any collection used by the model now documents using the implementation type instead of the interface. This facilitates the usage of new language features such as collections initialization.

```csharp
var schema = new OpenApiSchema();

// 1.X: does not compile due to the lack of implementation type
// 2.X: compiles successfully
schema.AnyOf = [];
// now a List<OpenApiSchema> instead of IList<OpenApiSchema>
```

### Ephemeral object properties are now in Metadata

In version 1.X applications could add ephemeral properties to some of the models from the libraries. These properties would be carried along in an "Annotations" property, but not serialized. This is especially helpful when building integrations that build document in multiple phases and need additional context to complete the work. The property is now named metadata to avoid any confusion with other terms. The parent interface has also been renamed from `IOpenApiAnnotatable` to `IMetadataContainer`.

```csharp
var schema = new OpenApiSchema();

// 1.X
var info = schema.Annotations["foo"];
// 2.X
var info = schema.Metadata["foo"];
```

### Updates to OpenApiSchema

The OpenAPI 3.1 specification changes significantly how it leverages JSON Schema.  In 3.0 and earlier, OpenAPI used a "subset, superset" of JSON Schema draft-4. This caused many problems for developers trying to use JSON Schema validation libraries with the JSON Schema in their OpenAPI descriptions.  In OpenAPI 3.1, the 2020-12 draft version of JSON Schema was adopted and a new JSON Schema vocabulary was adopted to support OpenAPI specific keywords.  All attempts to constrain what JSON Schema keywords could be used in OpenAPI were removed.

#### New keywords introduced in 2020-12

```csharp
/// $schema, a JSON Schema dialect identifier. Value must be a URI
public string Schema { get; set; }
/// $id - Identifies a schema resource with its canonical URI.
public string Id { get; set; }
/// $comment - reserves a location for comments from schema authors to readers or maintainers of the schema.
public string Comment { get; set; }
/// $vocabulary- used in meta-schemas to identify the vocabularies available for use in schemas described by that meta-schema.
public IDictionary<string, bool> Vocabulary { get; set; }
/// $dynamicRef - an applicator that allows for deferring the full resolution until runtime, at which point it is resolved each time it is encountered while evaluating an instance
public string DynamicRef { get; set; }
/// $dynamicAnchor - used to create plain name fragments that are not tied to any particular structural location for referencing purposes, which are taken into consideration for dynamic referencing.
public string DynamicAnchor { get; set; }
/// $defs - reserves a location for schema authors to inline re-usable JSON Schemas into a more general schema.
public IDictionary<string, OpenApiSchema> Definitions { get; set; }
public IDictionary<string, OpenApiSchema> PatternProperties { get; set; } = new Dictionary<string, OpenApiSchema>();
public bool UnevaluatedProperties { get; set;}

```

#### Changes to existing keywords

```csharp
public string? ExclusiveMaximum { get; set; }  // type changed to reflect the new version of JSON schema
public string? ExclusiveMinimum { get; set; } // type changed to reflect the new version of JSON schema
public JsonSchemaType? Type { get; set; }  // Was string, now flagged enum
public string? Maximum { get; set; }      // type changed to overcome double vs decimal issues
public string? Minimum { get; set; }       // type changed to overcome double vs decimal issues

public JsonNode Default { get; set; }  // Type matching no longer enforced. Was IOpenApiAny
public bool ReadOnly { get; set; }  // No longer has defined semantics in OpenAPI 3.1
public bool WriteOnly { get; set; }  // No longer has defined semantics in OpenAPI 3.1

public JsonNode Example { get; set; }  // No longer IOpenApiAny
public IList<JsonNode> Examples { get; set; }
public IList<JsonNode> Enum { get; set; }
public OpenApiExternalDocs ExternalDocs { get; set; }  // OpenApi Vocab
public bool Deprecated { get; set; }  // OpenApi Vocab
public OpenApiXml Xml { get; set; }  // OpenApi Vocab

public IDictionary<string, object> Metadata { get; set; }  // Custom property bag to be used by the application, used to be named annotations
```

#### OpenApiSchema methods

Other than the addition of `SerializeAsV31`, the methods have not changed.

```csharp
public class OpenApiSchema : IMetadataContainer, IOpenApiExtensible, IOpenApiReferenceable, IOpenApiSerializable
{
    public OpenApiSchema() { }
    public OpenApiSchema(OpenApiSchema schema) { }
    public void SerializeAsV31(IOpenApiWriter writer) { }
    public void SerializeAsV3(IOpenApiWriter writer) { }
    public void SerializeAsV2(IOpenApiWriter writer) { }
}

```

## OpenAPI v3.1 Support

There are a number of new features in OpenAPI v3.1 that are now supported in OpenAPI.NET.

### Webhooks

```csharp

public class OpenApiDocument  : IOpenApiSerializable, IOpenApiExtensible, IOpenApiMetadataContainer
{
    public IDictionary<string, OpenApiPathItem>? Webhooks { get; set; } = new Dictionary<string, OpenApiPathItem>();
}
```

### Summary in info object

```csharp
public class OpenApiInfo : IOpenApiSerializable, IOpenApiExtensible
{
    /// <summary>
    /// A short summary of the API.
    /// </summary>
    public string Summary { get; set; }
}
```

### License SPDX identifiers

```csharp
/// <summary>
/// License Object.
/// </summary>
public class OpenApiLicense : IOpenApiSerializable, IOpenApiExtensible
{
    /// <summary>
    /// An SPDX license expression for the API. The identifier field is mutually exclusive of the Url property.
    /// </summary>
    public string Identifier { get; set; }
}
```

### Reusable path items

```csharp
/// <summary>
/// Components Object.
/// </summary>
public class OpenApiComponents : IOpenApiSerializable, IOpenApiExtensible
{
    /// <summary>
    /// An object to hold reusable <see cref="OpenApiPathItem"/> Object.
    /// </summary>
    public IDictionary<string, OpenApiPathItem>? PathItems { get; set; } = new Dictionary<string, OpenApiPathItem>();
}
```

### Summary and Description alongside $ref

Through the use of proxy objects in order to represent references, it is now possible to set the Summary and Description property on an object that is a reference. This was previously not possible.

```csharp
var parameter = new OpenApiParameterReference("id", hostdocument)
{
    Description = "Customer Id"
};
```

Once serialized results in:

```yaml
$ref: id
description: Customer Id
```

### Use HTTP Method Object Instead of Enum

HTTP methods are now represented as objects instead of enums. This change enhances flexibility but requires updates to how HTTP methods are handled in your code.
Example:

```csharp
// Before (1.6)
OpenApiOperation operation = new OpenApiOperation
{
    HttpMethod = OperationType.Get
};

// After (2.0)
OpenApiOperation operation = new OpenApiOperation
{
    HttpMethod = new HttpMethod("GET") // or HttpMethod.Get
};
```

### References as Components

References can now be used as components, allowing for more modular and reusable OpenAPI documents.

**Example:**

```csharp
// Before (1.6)
OpenApiSchema schema = new OpenApiSchema
{
    Reference = new OpenApiReference
    {
        Type = ReferenceType.Schema,
        Id = "MySchema"
    }
};

// After (2.0)
OpenApiComponents components = new OpenApiComponents
{
    Schemas = new Dictionary<string, IOpenApiSchema>
    {
        ["MySchema"] = new OpenApiSchemaReference("MyOtherSchema")
        {
            Description = "Other reusable schema from initial schema"
        }
    }
};
```

### OpenApiDocument.SerializeAs()

The `SerializeAs()` method simplifies serialization scenarios, making it easier to convert OpenAPI documents to different formats.
**Example:**

```csharp
OpenApiDocument document = new OpenApiDocument();
string json = document.SerializeAs(OpenApiSpecVersion.OpenApi3_0, OpenApiConstants.Json);

```

### Use OpenApiConstants string Instead of OpenApiFormat Enum

OpenApiConstants are now used instead of OpenApiFormat enums.

**Example:**

```csharp
// Before (1.6)
var outputString = openApiDocument.Serialize(OpenApiSpecVersion.OpenApi2_0, OpenApiFormat.Json); 

// After (2.0)
var outputString = openApiDocument.Serialize(OpenApiSpecVersion.OpenApi2_0, OpenApiConstants.Json);
```

### Bug Fixes

## Serialization of References

Fixed a bug where references would not serialize summary or descriptions in OpenAPI 3.1.
**Example:**

```csharp
OpenApiSchemaReference schemaRef = new OpenApiSchemaReference("MySchema")
{
    Summary = "This is a summary",
    Description = "This is a description"
};
```

## Feedback

If you have any feedback please file a GitHub issue [here](https://github.com/microsoft/OpenAPI.NET/issues)
The team is looking forward to hear your experience trying the new version and we hope you have fun busting out your OpenAPI 3.1 descriptions.

## Todos

- Models now have matching interfaces + reference type + type assertion pattern + reference fields removed from the base model + Target + RecursiveTarget + removed OpenApiReferenceResolver.
- Workspace + component resolution.
- Visitor and Validator method now pass the interface model.
- Removed all the IEffective/GetEffective infrastructure.
- OpenApiSchema.Type is now a flag enum + bitwise operations.
- JsonSchemaDialect + BaseUri in document.
- Copy constructors are gone, use shallow copy method.
- Multiple methods that should have been internal have been changed from public to private link OpenApiLink.SerializeAsV3WithoutReference.
- duplicated _style property on parameter was removed.
- discriminator now uses references.
- ValidationRuleSet now accepts a key?