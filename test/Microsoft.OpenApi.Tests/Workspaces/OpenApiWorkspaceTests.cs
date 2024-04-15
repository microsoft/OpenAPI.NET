// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using Json.Schema;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Services;
using Xunit;

namespace Microsoft.OpenApi.Tests
{
    public class OpenApiWorkspaceTests
    {
        [Fact]
        public void OpenApiWorkspacesCanAddComponentsFromAnotherDocument()
        {
            var doc = new OpenApiDocument()
            {
                Paths = new OpenApiPaths()
                {
                    ["/"] = new()
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>()
                        {
                            [OperationType.Get] = new OpenApiOperation()
                            {
                                Responses = new OpenApiResponses()
                                {
                                    ["200"] = new()
                                    {
                                        Content = new Dictionary<string, OpenApiMediaType>()
                                        {
                                            ["application/json"] = new OpenApiMediaType()
                                            {
                                                Schema = new JsonSchemaBuilder().Ref("test").Build()
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var doc2 = new OpenApiDocument()
            {
                Components = new OpenApiComponents()
                {
                    Schemas = {
                        ["test"] = new JsonSchemaBuilder().Type(SchemaValueType.String).Description("The referenced one").Build()
                    }
                }
            };

            doc.Workspace.RegisterComponents(doc2);
                                    
            Assert.Equal(1, doc.Workspace.ComponentsCount());
        }

        [Fact]
        public void OpenApiWorkspacesCanResolveExternalReferences()
        {
            var refUri = new Uri("https://everything.json/common#/components/schemas/test");
            var workspace = new OpenApiWorkspace();
            var externalDoc = CreateCommonDocument();
                       
            workspace.RegisterComponent<IBaseDocument>("https://everything.json/common#/components/schemas/test", externalDoc.Components.Schemas["test"]);

            var schema = workspace.ResolveReference<JsonSchema>("https://everything.json/common#/components/schemas/test");
           
            Assert.NotNull(schema);
            Assert.Equal("The referenced one", schema.GetDescription());
        }

        [Fact]
        public void OpenApiWorkspacesCanResolveReferencesToDocumentFragments()
        {
            // Arrange
            var workspace = new OpenApiWorkspace();
            var schemaFragment = new JsonSchemaBuilder().Type(SchemaValueType.String).Description("Schema from a fragment").Build();
            workspace.RegisterComponent<IBaseDocument>("common#/components/schemas/test", schemaFragment);

            // Act
            var schema = workspace.ResolveReference<JsonSchema>("common#/components/schemas/test");

            // Assert
            Assert.NotNull(schema);
            Assert.Equal("Schema from a fragment", schema.GetDescription());
        }

        [Fact]
        public void OpenApiWorkspacesCanResolveReferencesToDocumentFragmentsWithJsonPointers()
        {
            // Arrange
            var workspace = new OpenApiWorkspace();
            var responseFragment = new OpenApiResponse
            {
                Headers = new Dictionary<string, OpenApiHeader>
                {
                    { "header1", new OpenApiHeader() }
                }
            };

            workspace.RegisterComponent("headers/header1", responseFragment);

            // Act
            var resolvedElement = workspace.ResolveReference<OpenApiResponse>("headers/header1");

            // Assert
            Assert.Same(responseFragment.Headers["header1"], resolvedElement.Headers["header1"]);
        }

        // Test artifacts
        private static OpenApiDocument CreateCommonDocument()
        {
            var doc =  new OpenApiDocument()
            {
                Components = new()
                {
                    Schemas = {
                        ["test"] = new JsonSchemaBuilder().Type(SchemaValueType.String).Description("The referenced one").Build()
                    }
                }
            };

            return doc;
        }
    }

    public static class OpenApiFactoryExtensions
    {

        public static OpenApiDocument CreatePathItem(this OpenApiDocument document, string path, Action<OpenApiPathItem> config)
        {
            var pathItem = new OpenApiPathItem();
            config(pathItem);
            document.Paths = new OpenApiPaths();
            document.Paths.Add(path, pathItem);
            return document;
        }

        public static OpenApiPathItem CreateOperation(this OpenApiPathItem parent, OperationType opType, Action<OpenApiOperation> config)
        {
            var child = new OpenApiOperation();
            config(child);
            parent.Operations.Add(opType, child);
            return parent;
        }

        public static OpenApiOperation CreateResponse(this OpenApiOperation parent, string status, Action<OpenApiResponse> config)
        {
            var child = new OpenApiResponse();
            config(child);
            parent.Responses.Add(status, child);
            return parent;
        }

        public static OpenApiResponse CreateContent(this OpenApiResponse parent, string mediaType, Action<OpenApiMediaType> config)
        {
            var child = new OpenApiMediaType();
            config(child);
            parent.Content.Add(mediaType, child);
            return parent;
        }

    }
}
