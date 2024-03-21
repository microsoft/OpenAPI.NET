# Feature Specification: Overlays Support

## Objective
Support use of Overlays for enabling developers to enhance an existing OpenAPI document without changing the original file.

## Problem Statement
OpenAPI documents are crucial for enhancing AI models. They enable the models to interact with web services through plugins and GPTs which in turn elevate the user experience when users are interacting with AI models. For these documents to be effective in providing a robust extensibility to the AI models they often require specific adjustments to existing properties or additional properties. Direct edits to the original documents is usually not feasible or is undisirable. Additionally, teams managing these OpenAPI documents across diverse environments such as development, staging, and production or for different audiences face significant challenges in that maintenance of multiple OpenAPI document versions leads to inefficiency, inconsistency, and increased overhead. This complexity is intensified when modifications or exclusions of certain properties or endpoints are necessary, especially in adapting content for public consumption without exposing sensitive information.

## Solution Overview
Overlays provide a flexible mechanism for modifying OpenAPI documents without directly modifying the original files. This allows necessary properties to be added, existing values modified, and the OpenAPI document tailored for a specific use case, for example developing a plugin manifest or adopting the document for a specific environment or a select audience group, while leaving the original openAPI document intact.

## Functional Requirements
1. **Overlay File Format Support**
   - Support overlay files in accordance with the Overlay Specification (v1.0.0), available at [Overlay Specification](https://github.com/OAI/Overlay-Specification/blob/3f398c6/versions/1.0.0.md).
   - Support both YAML and JSON formats for overlay files.
   - The target OpenAPI document may be specified in the Overlay file or may be provided separately.

2. **Overlay Application**
   - Apply overlay file to an existing OpenAPI document to produce a hybrid of the overlay file and the original OpenAPI document without changing the original document.
   - Ensure the resulting hybrid OpenAPI document remain valid according to the OpenAPI Specification.

3. **Overlay Operations**
   - Support addition, modification, and removal of properties to the target OpenAPI document.

## Example: Applying Overlay to Modify OpenAPI Description File

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

**Overlay File (overlay.yaml)**

```yaml
overlay: 1.0.0
info:
  title: Sample Overlay
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
paths:
  /users:
    get:
      summary: Retrieve all users  # Updated summary from overlay
      responses:
        '200':
          description: Successful response
  /users/{id}:
    get:
      summary: Get a user by ID  # Unchanged summary from original description
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
