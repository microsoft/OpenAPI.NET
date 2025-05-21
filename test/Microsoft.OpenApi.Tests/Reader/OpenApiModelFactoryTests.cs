using Xunit;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Models;
using System.Threading.Tasks;
using System.IO;
using System;

namespace Microsoft.OpenApi.Tests.Reader;

public class OpenApiModelFactoryTests
{
    [Fact]
    public async Task UsesSettingsBaseUrl()
    {
        var tempFilePathReferee = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
        await File.WriteAllTextAsync(tempFilePathReferee,
"""
{
    "openapi": "3.1.1",
    "info": {
        "title": "OData Service for namespace microsoft.graph",
        "description": "This OData service is located at https://graph.microsoft.com/v1.0",
        "version": "1.0.1"
    },
    "servers": [
        {
            "url": "https://graph.microsoft.com/v1.0"
        }
    ],
    "paths": {
        "/placeholder": {
            "get": {
                "responses": {
                    "200": {
                        "content": {
                            "application/json": {
                                "schema": {
                                    "type": "string"
                                }
                            }
                        }
                    }
                }
            }
        }
    },
    "components": {
        "schemas": {
            "MySchema": {
                "type": "object",
                "properties": {
                    "id": {
                        "type": "string"
                    }
                }
            }
        }
    }
}
""");
        var tempFilePathReferrer = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
        await File.WriteAllTextAsync(tempFilePathReferrer,
$$$"""
{
    "openapi": "3.1.1",
    "info": {
        "title": "OData Service for namespace microsoft.graph",
        "description": "This OData service is located at https://graph.microsoft.com/v1.0",
        "version": "1.0.1"
    },
    "servers": [
        {
            "url": "https://graph.microsoft.com/v1.0"
        }
    ],
    "paths": {
        "/placeholder": {
            "get": {
                "responses": {
                    "200": {
                        "content": {
                            "application/json": {
                                "schema": {
                                    "$ref": "./{{{Path.GetFileName(tempFilePathReferee)}}}#/components/schemas/MySchema"
                                }
                            }
                        }
                    }
                }
            }
        }
    },
    "components": {
        "schemas": {
            "MySchema": {
                "type": "object",
                "properties": {
                    "id": {
                        "type": "string"
                    }
                }
            }
        }
    }
}
""");
        // read referrer document to a memory stream
        using var stream = new MemoryStream();
        using var reader = new StreamReader(tempFilePathReferrer);
        await reader.BaseStream.CopyToAsync(stream);
        stream.Position = 0;
        var baseUri = new Uri(tempFilePathReferrer);
        var settings = new OpenApiReaderSettings
        {
            BaseUrl = baseUri, 
        };
        var readResult = await OpenApiDocument.LoadAsync(stream, settings: settings);
        Assert.NotNull(readResult.Document);
        Assert.NotNull(readResult.Document.Components);
        Assert.Equal(baseUri, readResult.Document.BaseUri);
    }
}
