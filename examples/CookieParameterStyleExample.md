# Cookie Parameter Style Example

This example demonstrates the new `cookie` parameter style introduced in OpenAPI 3.2.

## Example OpenAPI 3.2 Document

```yaml
openapi: 3.2.0
info:
  title: Cookie Parameter Example API
  version: 1.0.0
paths:
  /secure-data:
    get:
      summary: Get secure data using cookie authentication
      parameters:
        - name: sessionToken
          in: cookie
          style: cookie
          required: true
          description: Session authentication token
          schema:
            type: string
        - name: userPreferences
          in: cookie
          # Note: style defaults to "form" for cookie parameters when not specified
          description: User preference settings
          schema:
            type: string
      responses:
        '200':
          description: Secure data retrieved successfully
        '401':
          description: Unauthorized - invalid session token
```

## Code Examples

### Creating a Cookie Parameter in C#

```csharp
// Create a cookie parameter with explicit cookie style
var cookieParameter = new OpenApiParameter
{
    Name = "sessionToken",
    In = ParameterLocation.Cookie,
    Style = ParameterStyle.Cookie,  // New in OpenAPI 3.2
    Required = true,
    Description = "Session authentication token",
    Schema = new OpenApiSchema { Type = JsonSchemaType.String }
};

// Create a cookie parameter with default form style
var preferencesParameter = new OpenApiParameter
{
    Name = "userPreferences",
    In = ParameterLocation.Cookie,
    // Style will default to ParameterStyle.Form for cookie location
    Description = "User preference settings",
    Schema = new OpenApiSchema { Type = JsonSchemaType.String }
};
```

### Serializing to Different OpenAPI Versions

```csharp
// Serialize to OpenAPI 3.2 - Cookie style is supported
var v32Json = await cookieParameter.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_2);
// Result: includes "style": "cookie"

// Attempting to serialize to earlier versions will throw an exception
try
{
    var v31Json = await cookieParameter.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1);
}
catch (OpenApiException ex)
{
    // Exception: "Parameter style 'cookie' is only supported in OpenAPI 3.2 and later versions"
}

// However, using default style (form) works in all versions
var v31JsonDefault = await preferencesParameter.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1);
// Works fine - uses default form style
```

### Version Compatibility

| Parameter Style | OpenAPI 2.0 | OpenAPI 3.0 | OpenAPI 3.1 | OpenAPI 3.2+ |
|-----------------|--------------|--------------|--------------|---------------|
| `cookie`        | ? Error     | ? Error     | ? Error     | ? Supported  |
| `form` (default)| ? Supported | ? Supported | ? Supported | ? Supported  |

### Default Behavior

- **Default Style**: Cookie parameters default to `form` style when not explicitly specified
- **Default Explode**: Cookie style parameters default to `explode: true` (same as form style)
- **Style Omission**: When a cookie parameter uses the default `form` style, the `style` property is not serialized to avoid redundancy

### Key Features

1. **Version Validation**: Automatic validation prevents using `cookie` style in unsupported OpenAPI versions
2. **Automatic Deserialization**: The library automatically recognizes and deserializes `cookie` style parameters
3. **Default Handling**: Smart default behavior ensures compatibility and reduces verbosity
4. **Round-trip Serialization**: Full support for serializing and deserializing cookie style parameters

This implementation follows the OpenAPI 3.2 specification while maintaining backward compatibility and providing clear error messages when version constraints are violated.