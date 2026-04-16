# Experimental APIs

The Microsoft.OpenApi library includes a set of experimental APIs that are available for evaluation.
These APIs are subject to change or removal in future versions without following the usual deprecation process.

Using an experimental API will produce a compiler diagnostic that must be explicitly suppressed
to acknowledge the experimental nature of the API.

## Suppressing Experimental API Diagnostics

To use an experimental API, suppress the corresponding diagnostic in your project:

### Per call site

```csharp
#pragma warning disable OAI020
var v2Path = OpenApiPathHelper.GetVersionedPath(path, OpenApiSpecVersion.OpenApi2_0);
#pragma warning restore OAI020
```

### Per project (in `.csproj`)

```xml
<PropertyGroup>
  <NoWarn>$(NoWarn);OAI020</NoWarn>
</PropertyGroup>
```

---

## OAI020 — Path Version Conversion

| Diagnostic ID | Applies to | Since |
|---|---|---|
| `OAI020` | `OpenApiPathHelper`, `OpenApiValidatorError.GetVersionedPointer` | v3.6.0 |

### Overview

The path version conversion APIs translate JSON Pointer paths produced by the `OpenApiWalker`
(which always uses the v3 document model) into their equivalents for a specified OpenAPI
specification version.

This is useful when validation errors or walker paths need to be reported relative to
the original document version (e.g., Swagger v2) rather than the internal v3 representation.

### APIs

#### `OpenApiPathHelper.GetVersionedPath(string path, OpenApiSpecVersion targetVersion)`

Converts a v3-style JSON Pointer path to its equivalent for the target specification version.

**Parameters:**

- `path` — The v3-style JSON Pointer (e.g., `#/paths/~1items/get/responses/200/content/application~1json/schema`).
- `targetVersion` — The target OpenAPI specification version.

**Returns:** The equivalent path in the target version, the original path unchanged if no
transformation is needed, or `null` if the construct has no equivalent in the target version.

**Example:**

```csharp
#pragma warning disable OAI020
// v3 path from the walker
var v3Path = "#/paths/~1items/get/responses/200/content/application~1octet-stream/schema";

// Convert to v2 equivalent
var v2Path = OpenApiPathHelper.GetVersionedPath(v3Path, OpenApiSpecVersion.OpenApi2_0);
// Result: "#/paths/~1items/get/responses/200/schema"

// Convert to v3.2 (no transformation needed)
var v32Path = OpenApiPathHelper.GetVersionedPath(v3Path, OpenApiSpecVersion.OpenApi3_2);
// Result: "#/paths/~1items/get/responses/200/content/application~1octet-stream/schema"

// v3-only construct with no v2 equivalent
var serversPath = "#/servers/0";
var v2Result = OpenApiPathHelper.GetVersionedPath(serversPath, OpenApiSpecVersion.OpenApi2_0);
// Result: null
#pragma warning restore OAI020
```

#### `OpenApiValidatorError.GetVersionedPointer(OpenApiSpecVersion targetVersion)`

A convenience method on validation errors that translates the error's `Pointer` property to
the equivalent path for the target specification version.

**Example:**

```csharp
var validator = new OpenApiValidator(ValidationRuleSet.GetDefaultRuleSet());
var walker = new OpenApiWalker(validator);
walker.Walk(document);

foreach (var error in validator.Errors)
{
    if (error is OpenApiValidatorError validatorError)
    {
#pragma warning disable OAI020
        var v2Pointer = validatorError.GetVersionedPointer(OpenApiSpecVersion.OpenApi2_0);
#pragma warning restore OAI020
        if (v2Pointer is not null)
        {
            Console.WriteLine($"Error at {v2Pointer}: {validatorError.Message}");
        }
    }
}
```

### Supported Transformations (v2 target)

| v3 Path Pattern | v2 Equivalent |
|---|---|
| `#/components/schemas/{name}/**` | `#/definitions/{name}/**` |
| `#/components/parameters/{name}/**` | `#/parameters/{name}/**` |
| `#/components/responses/{name}/**` | `#/responses/{name}/**` |
| `#/components/securitySchemes/{name}/**` | `#/securityDefinitions/{name}/**` |
| `.../responses/{code}/content/{mediaType}/schema/**` | `.../responses/{code}/schema/**` |
| `.../headers/{name}/schema/**` | `.../headers/{name}/**` |

### Paths With No v2 Equivalent (returns `null`)

- `#/servers/**`
- `#/webhooks/**`
- `.../callbacks/**`
- `.../links/**`
- `.../requestBody/**`
- `.../content/{mediaType}/encoding/**`
- `#/components/examples/**`, `#/components/headers/**`, `#/components/pathItems/**`,
  `#/components/links/**`, `#/components/callbacks/**`, `#/components/requestBodies/**`,
  `#/components/mediaTypes/**`

### Why This Is Experimental

The set of path transformations may evolve as edge cases are discovered and additional
specification versions are released. The API surface and behavior may change in future versions
based on community feedback.
