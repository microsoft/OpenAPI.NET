// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Services;
using Xunit;

namespace Microsoft.OpenApi.Tests
{

    public class OpenApiWorkspaceTests
    {
        [Fact]
        public void OpenApiWorkspaceCanHoldMultipleDocuments()
        {
            var workspace = new OpenApiWorkspace();

            workspace.AddDocument("root", new OpenApiDocument());
            workspace.AddDocument("common", new OpenApiDocument());

            Assert.Equal(2, workspace.Documents.Count());
        }

        [Fact]
        public void OpenApiWorkspacesAllowDocumentsToReferenceEachOther()
        {
            var workspace = new OpenApiWorkspace();

            workspace.AddDocument("root", new OpenApiDocument() {
                Paths = new OpenApiPaths()
                {
                    ["/"] = new OpenApiPathItem()
                    {
                        Operations  = new Dictionary<OperationType, OpenApiOperation>()
                        {
                            [OperationType.Get] = new OpenApiOperation() {
                                Responses = new OpenApiResponses()
                                {
                                    ["200"] = new OpenApiResponse()
                                    {
                                       Content = new Dictionary<string,OpenApiMediaType>()
                                       {
                                           ["application/json"] = new OpenApiMediaType()
                                           {
                                               Schema = new OpenApiSchema()
                                               {
                                                   Reference = new OpenApiReference()
                                                   {
                                                       Id = "test",
                                                       Type = ReferenceType.Schema
                                                   }
                                               }
                                           }
                                       }
                                    }
                                }
                            }
                        }
                    }
                }
            });
            workspace.AddDocument("common", new OpenApiDocument() {
                Components = new OpenApiComponents()
                {
                    Schemas = {
                        ["test"] = new OpenApiSchema() {
                            Type = "string",
                            Description = "The referenced one"
                        }
                    }
                }
            });

            Assert.Equal(2, workspace.Documents.Count());
        }

        [Fact]
        public void OpenApiWorkspacesCanResolveExternalReferences()
        {
            var workspace = new OpenApiWorkspace();
            workspace.AddDocument("common", CreateCommonDocument());
            var schema = workspace.ResolveReference(new OpenApiReference()
            {
                Id = "test",
                Type = ReferenceType.Schema,
                ExternalResource ="common"
            }) as OpenApiSchema;

            Assert.NotNull(schema);
            Assert.Equal("The referenced one", schema.Description);
        }

        [Fact]
        public void OpenApiWorkspacesAllowDocumentsToReferenceEachOther_short()
        {
            var workspace = new OpenApiWorkspace();

            var doc = new OpenApiDocument();
            doc.CreatePathItem("/", p =>
            {
                p.Description = "Consumer";
                p.CreateOperation(OperationType.Get, op =>
                  op.CreateResponse("200", re =>
                  {
                      re.Description = "Success";
                      re.CreateContent("application/json", co =>
                          co.Schema = new OpenApiSchema()
                          {
                              Reference = new OpenApiReference()  // Reference
                              {
                                  Id = "test",
                                  Type = ReferenceType.Schema,
                                  ExternalResource = "common"
                              },
                              UnresolvedReference = true
                          }
                      );
                  })
                );
            });

            workspace.AddDocument("root", doc);
            workspace.AddDocument("common", CreateCommonDocument());
            var errors = doc.ResolveReferences();
            Assert.Empty(errors);

            var schema = doc.Paths["/"].Operations[OperationType.Get].Responses["200"].Content["application/json"].Schema;
            var effectiveSchema = schema.GetEffective(doc);
            Assert.False(effectiveSchema.UnresolvedReference);
        }

        [Fact]
        public void OpenApiWorkspacesShouldNormalizeDocumentLocations()
        {
            var workspace = new OpenApiWorkspace();
            workspace.AddDocument("hello", new OpenApiDocument());
            workspace.AddDocument("hi", new OpenApiDocument());

            Assert.True(workspace.Contains("./hello"));
            Assert.True(workspace.Contains("./foo/../hello"));
            Assert.True(workspace.Contains("file://" + Environment.CurrentDirectory + "/./foo/../hello"));

            Assert.False(workspace.Contains("./goodbye"));
        }

        // Enable Workspace to load from any reader, not just streams.

        // Test fragments
        internal void OpenApiWorkspacesShouldLoadDocumentFragments()
        {
            Assert.True(false);
        }

        [Fact]
        public void OpenApiWorkspacesCanResolveReferencesToDocumentFragments()
        {
            // Arrange
            var workspace = new OpenApiWorkspace();
            var schemaFragment = new OpenApiSchema { Type = "string", Description = "Schema from a fragment" };
            workspace.AddFragment("fragment", schemaFragment);

            // Act
            var schema = workspace.ResolveReference(new OpenApiReference()
            {
                ExternalResource = "fragment"
            }) as OpenApiSchema;

            // Assert
            Assert.NotNull(schema);
            Assert.Equal("Schema from a fragment", schema.Description);
        }

        [Fact]
        public void OpenApiWorkspacesCanResolveReferencesToDocumentFragmentsWithJsonPointers()
        {
            // Arrange
            var workspace = new OpenApiWorkspace();
            var responseFragment = new OpenApiResponse()
            {
                Headers = new Dictionary<string, OpenApiHeader>
                {
                    { "header1", new OpenApiHeader() }
                }
            };
            workspace.AddFragment("fragment", responseFragment);

            // Act
            var resolvedElement = workspace.ResolveReference(new OpenApiReference()
            {
                Id = "headers/header1",
                ExternalResource = "fragment"
            });

            // Assert
            Assert.Same(responseFragment.Headers["header1"], resolvedElement);
        }


        // Test artifacts

        private static OpenApiDocument CreateCommonDocument()
        {
            return new OpenApiDocument()
            {
                Components = new OpenApiComponents()
                {
                    Schemas = {
                        ["test"] = new OpenApiSchema() {
                            Type = "string",
                            Description = "The referenced one"
                        }
                    }
                }
            };
        }
    }

    public static class OpenApiFactoryExtensions {

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
