# Upgrade guide to OpenAPI.NET 3.0

## Introduction

We are excited to announce OpenAPI.NET v3.0! This major update introduces support for OpenAPI v3.2 specification along with several API enhancements and refinements to the existing model architecture.

> [!WARNING]
> This is a major version update that includes breaking changes. Please review this guide carefully before upgrading.

## Integrations with ASP.NET

If you are using this library with [AspNetCore OpenAPI versions](https://www.nuget.org/packages/Microsoft.AspNetCore.OpenApi) `< 10.0` then you must remain on version `1.x` as it's not compatible.

If you are using this library with [AspNetCore OpenAPI versions](https://www.nuget.org/packages/Microsoft.AspNetCore.OpenApi) `= 10.0` then you must remain on version `2.x` as it's not compatible.

If you are using this library with [Swashbuckle.AspNetCore version](https://www.nuget.org/packages/Swashbuckle.AspNetCore/) `< 10.0` then you must remain on version `1.x` as it's not compatible.

If you are using this library with [Swashbuckle.AspNetCore version](https://www.nuget.org/packages/Swashbuckle.AspNetCore/) `< 10.0` then you must remain on version `2.x` as it's not compatible.

The latest support policy information for this library, and integration with ASP.NET can be found on [the contributing documentation](https://github.com/microsoft/OpenAPI.NET/blob/main/CONTRIBUTING.md#branches-and-support-policy).

## OpenAPI v3.2 Support

The primary focus of OpenAPI.NET v3.0 is adding comprehensive support for OpenAPI specification v3.2. This includes new serialization methods, enhanced model properties, and expanded functionality across the entire API surface.

### New Serialization Methods

All serializable components now include a `SerializeAsV32` method alongside the existing serialization methods:

```csharp
// v2.x
document.SerializeAsV31(writer);

// v3.0
document.SerializeAsV31(writer);
document.SerializeAsV32(writer); // New!
```

### OpenApiSpecVersion Enum Update

A new version constant has been added:

```csharp
public enum OpenApiSpecVersion
{
    OpenApi2_0 = 0,
    OpenApi3_0 = 1,
    OpenApi3_1 = 2,
    OpenApi3_2 = 3,  // New!
}
```

## Enhanced Media Type Support

### IOpenApiMediaType Interface

Media types are now represented by the `IOpenApiMediaType` interface, providing better abstraction and consistency across the API:

```csharp
// v2.x
public IDictionary<string, OpenApiMediaType>? Content { get; set; }

// v3.0
public IDictionary<string, IOpenApiMediaType>? Content { get; set; }
```

### New Media Type Properties

The `IOpenApiMediaType` interface includes additional properties for enhanced functionality:

```csharp
public interface IOpenApiMediaType
{
    IDictionary<string, OpenApiEncoding>? Encoding { get; }
    JsonNode? Example { get; }
    IDictionary<string, IOpenApiExample>? Examples { get; }
    OpenApiEncoding? ItemEncoding { get; }        // New!
    IOpenApiSchema? ItemSchema { get; }           // New!
    IList<OpenApiEncoding>? PrefixEncoding { get; } // New!
    IOpenApiSchema? Schema { get; }
}
```

### MediaTypes in Components

Components now support reusable media types:

```csharp
public class OpenApiComponents
{
    public IDictionary<string, IOpenApiMediaType>? MediaTypes { get; set; } // New!
    // ... other existing properties
}
```

## Enhanced Example Support

### New Example Properties

Example objects now support additional data representation options:

```csharp
public class OpenApiExample : IOpenApiExample
{
    public JsonNode? DataValue { get; set; }      // New!
    public string? SerializedValue { get; set; }  // New!
    public string? ExternalValue { get; set; }
    public JsonNode? Value { get; set; }
    // ... other properties
}
```

## Security Scheme Enhancements

### Deprecated Property

Security schemes now support a deprecated flag:

```csharp
public interface IOpenApiSecurityScheme
{
    bool Deprecated { get; }  // New!
    // ... other existing properties
}
```

### Device Authorization Flow

OAuth flows now support device authorization:

```csharp
public class OpenApiOAuthFlows
{
    public OpenApiOAuthFlow? DeviceAuthorization { get; set; }  // New!
    // ... other existing flows
}

public class OpenApiOAuthFlow
{
    public Uri? DeviceAuthorizationUrl { get; set; }  // New!
    // ... other existing properties
}
```

## Tag System Improvements

### Hierarchical Tags

Tags now support hierarchical organization with enhanced metadata:

```csharp
public interface IOpenApiTag
{
    string? Kind { get; }                    // New!
    string? Summary { get; }                 // New!
    OpenApiTagReference? Parent { get; }     // New!
    string? Name { get; }
    // ... other existing properties
}
```

## Response Enhancements

### Summary Support

Responses now implement `IOpenApiSummarizedElement` and support summary text:

```csharp
// v2.x
public interface IOpenApiResponse : IOpenApiDescribedElement, ...
{
    // No summary support
}

// v3.0
public interface IOpenApiResponse : IOpenApiDescribedElement, 
    IOpenApiSummarizedElement, ...  // New!
{
    // Inherits Summary property from IOpenApiSummarizedElement
}
```

## XML Improvements

### Enhanced Node Type Support

The XML object has been refactored to use a more flexible node type system:

```csharp
// v2.x
public class OpenApiXml
{
    public bool Attribute { get; set; }  // Removed!
    public bool Wrapped { get; set; }    // Removed!
}

// v3.0
public class OpenApiXml
{
    public OpenApiXmlNodeType? NodeType { get; set; }  // New!
}

public enum OpenApiXmlNodeType
{
    Element = 0,
    Attribute = 1,
    Text = 2,
    Cdata = 3,
    None = 4,
}
```

## Discriminator Enhancements

### Default Mapping Support

Discriminators now support default mapping scenarios:

```csharp
public class OpenApiDiscriminator
{
    public OpenApiSchemaReference? DefaultMapping { get; set; }  // New!
    // ... other existing properties
}
```

## Document Enhancements

### Self-Reference Support

Documents can now specify their own identity URI:

```csharp
public class OpenApiDocument
{
    public Uri? Self { get; set; }  // New!
    // ... other existing properties
}
```

## Parameter Location Enhancements

### New Parameter Locations

Additional parameter locations are now supported:

```csharp
public enum ParameterLocation
{
    Query = 0,
    Header = 1,
    Path = 2,
    Cookie = 3,
    QueryString = 4,  // New!
}
```

### New Parameter Styles

```csharp
public enum ParameterStyle
{
    Matrix = 0,
    Label = 1,
    Form = 2,
    Simple = 3,
    SpaceDelimited = 4,
    PipeDelimited = 5,
    DeepObject = 6,
    Cookie = 7,  // New!
}
```

## Reference Type Enhancements

### New Reference Types

```csharp
public enum ReferenceType
{
    Schema = 0,
    Response = 1,
    Parameter = 2,
    Example = 3,
    RequestBody = 4,
    Header = 5,
    SecurityScheme = 6,
    Link = 7,
    Callback = 8,
    Tag = 9,
    PathItem = 10,
    MediaType = 11,  // New!
}
```

## Visitor Pattern Updates

### Interface-Based Visiting

The visitor pattern now works with interface types for better abstraction:

```csharp
// v2.x
public virtual void Visit(OpenApiMediaType mediaType) { }

// v3.0
public virtual void Visit(IOpenApiMediaType mediaType) { }
```

## Version Detection

### New Version Detection Method

```csharp
public static class VersionService
{
    public static bool is3_2(this string version) { }  // New!
    // ... other existing version methods
}
```

## Migration Steps

### 1. Update Media Type References

Replace concrete `OpenApiMediaType` references with `IOpenApiMediaType`:

```csharp
// Before
public IDictionary<string, OpenApiMediaType>? Content { get; set; }

// After
public IDictionary<string, IOpenApiMediaType>? Content { get; set; }
```

### 2. Update XML Object Usage

Replace boolean properties with the new enum-based approach:

```csharp
// Before
var xml = new OpenApiXml
{
    Attribute = true,
    Wrapped = false
};

// After
var xml = new OpenApiXml
{
    NodeType = OpenApiXmlNodeType.Attribute
};
```

### 3. Update Visitor Implementations

Update visitor methods to use interface types:

```csharp
// Before
public override void Visit(OpenApiMediaType mediaType) { /* ... */ }

// After
public override void Visit(IOpenApiMediaType mediaType) { /* ... */ }
```

### 4. Add v3.2 Serialization Support

If you have custom serialization logic, add support for v3.2:

```csharp
public void SerializeAsV32(IOpenApiWriter writer)
{
    // Implement v3.2 specific serialization
    SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_2);
}
```

## Breaking Changes Summary

1. **Media Type Abstraction**: `OpenApiMediaType` replaced with `IOpenApiMediaType` interface in collection properties
2. **XML Object Refactoring**: `Attribute` and `Wrapped` boolean properties replaced with `NodeType` enum
3. **Visitor Pattern**: Methods now accept interface types instead of concrete types
4. **Response Interface**: `IOpenApiResponse` now extends `IOpenApiSummarizedElement`

## New Features Summary

1. **OpenAPI v3.2 Support**: Full serialization and model support
2. **Enhanced Media Types**: New properties for encoding and schema support
3. **Hierarchical Tags**: Support for tag organization with kind, summary, and parent relationships
4. **Security Enhancements**: Deprecated flag and device authorization flow support
5. **Enhanced Examples**: New data value and serialized value properties
6. **Document Self-Reference**: Support for document identity URIs
7. **Extended Parameter Support**: New locations and styles for parameters

## Feedback

If you have any feedback please file [a new GitHub issue](https://github.com/microsoft/OpenAPI.NET/issues). The team looks forward to hearing about your experience with OpenAPI.NET v3.0 and OpenAPI v3.2 support!
