# Feature Specification: Overlays Support

## Objective
The objective of this feature is to empower users of the OpenAPI.NET library with capability to enahance existing OpenAPI documents without altering the original document.

## Problem Statement
OpenAPI documents are crucial for enhancing AI models. They enable the models to **interact with web services through plugins and GPTs** which in turn **elevate the user experience** when users are interacting with AI models. For these documents to be effective in providing a **robust extensibility to the AI models** they often require specific adjustments to existing properties or additional properties. **Direct edits** to the original documents is usually **not feasible or is undisirable**. Additionally, teams managing these OpenAPI documents across **diverse environments such as development, staging, and production** or for different audiences face significant challenges in that **maintenance of multiple OpenAPI document versions leads to inefficiency, inconsistency, and increased overhead**. This complexity is intensified when modifications or exclusions of certain properties or endpoints are necessary, especially in adapting content for public consumption without exposing sensitive information.

## Solution Overview
Overlays provide a flexible mechanism for modifying OpenAPI documents without directly modifying the original files. This allows necessary properties to be added, existing values modified, and the OpenAPI document tailored for a specific use case, for example developing a plugin manifest or adopting the document for a specific environment or a select audience group, while leaving the original openAPI document intact.

## Functional Requirements
1. **Overlay Support Feature**
   - Support overlay files in accordance with the Overlay Specification (v1.0.0), available at [Overlay Specification](https://github.com/OAI/Overlay-Specification/blob/3f398c6/versions/1.0.0.md).
   - Support both YAML and JSON formats for overlay files.
   - Support addition, modification, and removal of properties to the target OpenAPI document.
   - The target OpenAPI document should be provided separately in either JSON or YAML format.
   - Support use of structured overlay file to support merging the overlay at the root of the target OpenAPI document and use of targeted merging where only specific paths are to be modified. See example in the example section.
   - Apply overlay file to an existing OpenAPI document to produce a hybrid of the overlay file and the original OpenAPI document without changing the original document.
   - Ensure the resulting hybrid OpenAPI document remain valid according to the OpenAPI Specification.
   - Overlay support feature will be added to the OpenAPI.NET library project as a new class. This will allow compatibility with current and future versions of OpenAPI.NET. This way developers enjoy the feature without adding their dependency footprint.
   - Since OpenAPI.NET already supports both JSON and YAML, developers can use either format for their OpenAPI documents and overlays.
   - The OverlayService class will validate the overlay file against OAI Overlay Specification and handle the modification of the OpenApiDocument by applying the overlay file. The modified objects will be used by the downstream logics as per the developer's needs.
       
  2. **User Journey**
     1. Start by loading an existing OpenAPI document using OpenApiDocument.Load to load JSON/Yaml file with Overlay referenced in readerSettings.

     2. Load Yaml and translate to JSONNodes or just load JSONNodes.
     3. Load Overlay as JsonNodes.
     4. Apply Overlay to OpenAPI JsonNodes
        
         ```csharp
        
         var overlay = await Overlay.LoadAsync(stream);
        var jsonElement = await JsonDocument.ParseAsync(stream).RootElement;
        var newJsonElement = overlay.Apply(jsonElement);
        

     6. Pass "overlayed" JsonNodes to rest of OpenAPI Parser for its intended applications, such as AI-Plugin manifest generation, client SDK generation or other use cases.
        
     7. Optionally, serialize the enhanced OpenAPI document into JSON or YAML format for storage, distribution, or further processing. This can be done using the serialization capabilities provided by the OpenAPI.NET library, ensuring that the document can be easily shared or integrated into other tools and workflows.

## Example: Applying Overlay to Modify OpenAPI Description File

In this example, we delve into the strategic use of overlay files to enrich the semantic clarity of OpenAPI description files.

Consider a sample API with two primary operations: one retrieves a comprehensive list of users, and the other fetches a single user by their unique identifier. Initially, the operation to retrieve all users is summarized as "Get a list of users." By applying an overlay, we enhance its semantic clarity by updating the summary to "Retrieve all users." This subtle modification makes the description more natural and aligned with conversational language and increases its interpretability by AI, aiding in tasks such as automatic API invocation or documentation generation.
Similarly, for the operation that retrieves a user by ID, we refine the parameter description from a generic "User ID" to a more descriptive "Unique identifier of the user." This precise terminology improves AI's comprehension by explicitly defining the parameter's purpose and significance. Additionally, using the overlay, we introduce a new response scenario in the 'responses' property to account for situations when a user is not found, further enhancing the API's robustness and the AI's ability to handle different response types effectively.

**Original OpenAPI Description File**

```yaml
openapi: 3.0.0
info:
  title: Sample API
  version: 1.0.0
paths:
  /users:
    get:
      summary: Get a list of users
      responses:
        '200':
          description: Successful response
  /users/{id}:
    get:
      summary: Get a user by ID
      parameters:
        - name: id
          in: path
          required: true
          description: User ID
      responses:
        '200':
          description: Successful response
```

**Structured Overlay File (overlay.yaml)**

This is a structured overlay file. It follows the structure of the target OpenAPI document. It applies a merge at the root of the target document similar to how JSON merge would work.

```yaml
overlay: 1.0.0
info:
  title: Sample Structured Overlay
  version: 1.0.0
actions:
  - target: "$"
    update:
      info:
        x-overlay-applied: sample-overlay
      paths:
        "/users":
          get:
            summary: "Retrieve all users"
        "/users/{id}":
          get:
            parameters:
              - name: id
                description: "Unique identifier of the user"
            responses:
              '404':
                description: "User not found"
```

**Resulting OpenAPI document**

```yaml
openapi: 3.0.0
info:
  title: Sample API
  version: 1.0.0
  x-overlay-applied: sample-structured-overlay
paths:
  /users:
    get:
      summary: Retrieve all users  # Updated summary from overlay
      responses:
        '200':
          description: Successful response
  /users/{id}:
    get:
      summary: Get a user by ID
      parameters:
        - name: id
          in: path
          required: true
          description: Unique identifier of the user  # Updated description from overlay
      responses:
        '200':
          description: Successful response
        '404':
          description: User not found  # New response added from overlay
```

To illustrate the use of targeted overlay, we take the resulting OpenAPI document and add operationIDs to the two operations supported by the API. OperationID property provide unique identifiers for each operation, making them more easily referenced and thus enahance usability and maintainability.

**Targeted Overlay File (overlay.yaml)**

```yaml
overlay: 1.0.0
info:
  title: Sample Overlay
  version: 1.0.0
actions:
  - target: "$"
    update:
      info:
        x-overlay-applied: sample-targeted-overlay # Updated summary from overlay
  - target: "$.paths.'/users'.get"
    update:
      operationId: "getUsers" # Updated summary from overlay
  - target: "$.paths.'/users/{id}'.get"
    update:
      operationId: "getUserById"
      parameters:
        - name: id
          in: path
          required: true
          description: "Unique identifier of the user"
      responses:
        '404':
          description: "User not found"
```
**Resulting OpenAPI document**

```yaml
openapi: 3.0.0
info:
  title: Sample API
  version: 1.0.0
  x-overlay-applied: sample-overlay # 
paths:
  /users:
    get:
      summary: Retrieve all users
      operationId: getUsers # This line is added by the overlay
      responses:
        '200':
          description: Successful response
  /users/{id}:
    get:
      summary: Get a user by ID
      operationId: getUserById # This line is added by the overlay
      parameters:
        - name: id
          in: path
          required: true
          description: Unique identifier of the user
      responses:
        '200':
          description: Successful response
        '404':
          description: User not found
```

