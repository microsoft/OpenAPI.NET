using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.V32;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V32Tests;

// NOTE: These tests are currently disabled because media type reference deserialization 
// support needs to be implemented in the parser. The tests are kept here as a specification
// of the expected behavior once parser support is added.
public class OpenApiMediaTypeReferenceDeserializerTests
{
    /*
    [Fact]
    public void ShouldDeserializeMediaTypeReferenceInRequestBodyContent()
    {
        var json =
        """
        {
            "openapi": "3.2.0",
            "info": {
                "title": "Test API",
                "version": "1.0.0"
            },
            "paths": {
                "/test": {
                    "post": {
                        "requestBody": {
                            "content": {
                                "application/json": {
                                    "$ref": "#/components/mediaTypes/application~1json"
                                }
                            }
                        },
                        "responses": {
                            "200": {
                                "description": "OK"
                            }
                        }
                    }
                }
            },
            "components": {
                "mediaTypes": {
                    "application/json": {
                        "schema": {
                            "type": "object",
                            "properties": {
                                "name": {
                                    "type": "string"
                                }
                            }
                        }
                    }
                }
            }
        }
        """;

        var readResult = OpenApiDocument.Parse(json, "json");
        var document = readResult.Document;

        Assert.NotNull(document);
        Assert.Empty(readResult.Diagnostic.Errors);

        var requestBody = document.Paths["/test"].Operations[HttpMethod.Post].RequestBody;
        Assert.NotNull(requestBody);
        Assert.NotNull(requestBody.Content);
        Assert.True(requestBody.Content.ContainsKey("application/json"));

        var mediaType = requestBody.Content["application/json"];
        Assert.IsType<OpenApiMediaTypeReference>(mediaType);
        var mediaTypeRef = (OpenApiMediaTypeReference)mediaType;

        Assert.NotNull(mediaTypeRef.Target);
        Assert.NotNull(mediaTypeRef.Schema);
        Assert.Equal(JsonSchemaType.Object, mediaTypeRef.Schema.Type);
    }

    [Fact]
    public void ShouldDeserializeMediaTypeReferenceInResponseBodyContent()
    {
        var json =
        """
        {
            "openapi": "3.2.0",
            "info": {
                "title": "Test API",
                "version": "1.0.0"
            },
            "paths": {
                "/test": {
                    "get": {
                        "responses": {
                            "200": {
                                "description": "OK",
                                "content": {
                                    "application/json": {
                                        "$ref": "#/components/mediaTypes/application~1json"
                                    }
                                }
                            }
                        }
                    }
                }
            },
            "components": {
                "mediaTypes": {
                    "application/json": {
                        "schema": {
                            "type": "object",
                            "properties": {
                                "id": {
                                    "type": "integer"
                                }
                            }
                        }
                    }
                }
            }
        }
        """;

        var readResult = OpenApiDocument.Parse(json, "json");
        var document = readResult.Document;

        Assert.NotNull(document);
        Assert.Empty(readResult.Diagnostic.Errors);

        var response = document.Paths["/test"].Operations[HttpMethod.Get].Responses["200"];
        Assert.NotNull(response);
        Assert.NotNull(response.Content);
        Assert.True(response.Content.ContainsKey("application/json"));

        var mediaType = response.Content["application/json"];
        Assert.IsType<OpenApiMediaTypeReference>(mediaType);
        var mediaTypeRef = (OpenApiMediaTypeReference)mediaType;

        Assert.NotNull(mediaTypeRef.Target);
        Assert.NotNull(mediaTypeRef.Schema);
        Assert.Equal(JsonSchemaType.Object, mediaTypeRef.Schema.Type);
    }

    [Fact]
    public void ShouldDeserializeMediaTypeReferenceInParameterContent()
    {
        var json =
        """
        {
            "openapi": "3.2.0",
            "info": {
                "title": "Test API",
                "version": "1.0.0"
            },
            "paths": {
                "/test": {
                    "get": {
                        "parameters": [
                            {
                                "name": "filter",
                                "in": "query",
                                "content": {
                                    "application/json": {
                                        "$ref": "#/components/mediaTypes/application~1json"
                                    }
                                }
                            }
                        ],
                        "responses": {
                            "200": {
                                "description": "OK"
                            }
                        }
                    }
                }
            },
            "components": {
                "mediaTypes": {
                    "application/json": {
                        "schema": {
                            "type": "string"
                        }
                    }
                }
            }
        }
        """;

        var readResult = OpenApiDocument.Parse(json, "json");
        var document = readResult.Document;

        Assert.NotNull(document);
        Assert.Empty(readResult.Diagnostic.Errors);

        var parameters = document.Paths["/test"].Operations[HttpMethod.Get].Parameters;
        Assert.NotNull(parameters);
        Assert.Single(parameters);

        var parameter = parameters[0];
        Assert.NotNull(parameter.Content);
        Assert.True(parameter.Content.ContainsKey("application/json"));

        var mediaType = parameter.Content["application/json"];
        Assert.IsType<OpenApiMediaTypeReference>(mediaType);
        var mediaTypeRef = (OpenApiMediaTypeReference)mediaType;

        Assert.NotNull(mediaTypeRef.Target);
        Assert.NotNull(mediaTypeRef.Schema);
        Assert.Equal(JsonSchemaType.String, mediaTypeRef.Schema.Type);
    }

    [Fact]
    public void ShouldDeserializeMediaTypeReferenceInHeaderContent()
    {
        var json =
        """
        {
            "openapi": "3.2.0",
            "info": {
                "title": "Test API",
                "version": "1.0.0"
            },
            "paths": {
                "/test": {
                    "get": {
                        "responses": {
                            "200": {
                                "description": "OK",
                                "headers": {
                                    "X-Custom-Header": {
                                        "description": "Custom header",
                                        "content": {
                                            "application/json": {
                                                "$ref": "#/components/mediaTypes/application~1json"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            "components": {
                "mediaTypes": {
                    "application/json": {
                        "schema": {
                            "type": "array",
                            "items": {
                                "type": "string"
                            }
                        }
                    }
                }
            }
        }
        """;

        var readResult = OpenApiDocument.Parse(json, "json");
        var document = readResult.Document;

        Assert.NotNull(document);
        Assert.Empty(readResult.Diagnostic.Errors);

        var headers = document.Paths["/test"].Operations[HttpMethod.Get].Responses["200"].Headers;
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Custom-Header"));

        var header = headers["X-Custom-Header"];
        Assert.NotNull(header.Content);
        Assert.True(header.Content.ContainsKey("application/json"));

        var mediaType = header.Content["application/json"];
        Assert.IsType<OpenApiMediaTypeReference>(mediaType);
        var mediaTypeRef = (OpenApiMediaTypeReference)mediaType;

        Assert.NotNull(mediaTypeRef.Target);
        Assert.NotNull(mediaTypeRef.Schema);
        Assert.Equal(JsonSchemaType.Array, mediaTypeRef.Schema.Type);
    }
    */
}
