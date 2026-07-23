// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#pragma warning disable OPENAPI001

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Nodes;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models;

public class OpenApiDeepCopyTests
{
    [Fact]
    public void CreateDeepCopyClonesDocumentObjectGraph()
    {
        var document = CreateDocument();

        var copy = document.CreateDeepCopy();

        Assert.NotSame(document, copy);
        Assert.NotSame(document.Info, copy.Info);
        Assert.NotSame(document.Paths, copy.Paths);
        Assert.NotSame(document.Components, copy.Components);

        var originalPathItem = Assert.IsType<OpenApiPathItem>(document.Paths["/pets"]);
        var copiedPathItem = Assert.IsType<OpenApiPathItem>(copy.Paths["/pets"]);
        Assert.NotSame(originalPathItem, copiedPathItem);
        Assert.NotSame(originalPathItem.Operations, copiedPathItem.Operations);

        var originalOperations = originalPathItem.Operations;
        Assert.NotNull(originalOperations);
        var copiedOperations = copiedPathItem.Operations;
        Assert.NotNull(copiedOperations);
        var originalOperation = originalOperations[HttpMethod.Get];
        var copiedOperation = copiedOperations[HttpMethod.Get];
        Assert.NotSame(originalOperation, copiedOperation);
        Assert.NotSame(originalOperation.Responses, copiedOperation.Responses);

        var originalComponents = document.Components;
        Assert.NotNull(originalComponents);
        var originalSchemas = originalComponents.Schemas;
        Assert.NotNull(originalSchemas);

        var copiedComponents = copy.Components;
        Assert.NotNull(copiedComponents);
        var copiedSchemas = copiedComponents.Schemas;
        Assert.NotNull(copiedSchemas);
        
        var originalPetSchema = Assert.IsType<OpenApiSchema>(originalSchemas["Pet"]);
        var copiedPetSchema = Assert.IsType<OpenApiSchema>(copiedSchemas["Pet"]);
        Assert.NotSame(originalPetSchema, copiedPetSchema);
        var copiedProperties = copiedPetSchema.Properties;
        Assert.NotNull(copiedProperties);
        var originalProperties = originalPetSchema.Properties;
        Assert.NotNull(originalProperties);
        Assert.NotSame(originalProperties, copiedProperties);
        Assert.Same(copiedPetSchema, copiedProperties["friend"]);

        var copiedIdSchema = Assert.IsType<OpenApiSchema>(copiedProperties["id"]);
        copiedIdSchema.Description = "updated in copy";
        var originalIdSchema = Assert.IsType<OpenApiSchema>(originalProperties["id"]);
        Assert.Equal("original id", originalIdSchema.Description);
    }

    [Fact]
    public void CreateDeepCopyClonesJsonNodesAndExtensions()
    {
        var document = CreateDocument();

        var copy = document.CreateDeepCopy();

        var originalMediaType = GetJsonMediaType(document);
        var copiedMediaType = GetJsonMediaType(copy);

        var copiedExample = Assert.IsType<JsonObject>(copiedMediaType.Example);
        var originalExample = Assert.IsType<JsonObject>(originalMediaType.Example);
        Assert.NotSame(originalExample, copiedExample);
        copiedExample["name"] = "copy";
        var originalName = originalExample["name"];
        Assert.NotNull(originalName);
        Assert.Equal("original", originalName.GetValue<string>());

        var originalExtensions = document.Info.Extensions;
        Assert.NotNull(originalExtensions);
        var copiedExtensions = copy.Info.Extensions;
        Assert.NotNull(copiedExtensions);
        var originalExtension = Assert.IsType<JsonNodeExtension>(originalExtensions["x-info"]);
        var copiedExtension = Assert.IsType<JsonNodeExtension>(copiedExtensions["x-info"]);
        Assert.NotSame(originalExtension, copiedExtension);
        Assert.NotSame(originalExtension.Node, copiedExtension.Node);
    }

    [Fact]
    public void CreateDeepCopyRemapsCopiedReferencesToCopiedDocument()
    {
        var document = CreateDocument();

        var copy = document.CreateDeepCopy();

        var copiedMediaType = GetJsonMediaType(copy);

        var copiedSchemaReference = Assert.IsType<OpenApiSchemaReference>(copiedMediaType.Schema);
        Assert.Same(copy, copiedSchemaReference.Reference.HostDocument);
        var copiedComponents = copy.Components;
        Assert.NotNull(copiedComponents);
        var copiedSchemas = copiedComponents.Schemas;
        Assert.NotNull(copiedSchemas);
        var originalComponents = document.Components;
        Assert.NotNull(originalComponents);
        var originalSchemas = originalComponents.Schemas;
        Assert.NotNull(originalSchemas);
        Assert.Same(copiedSchemas["Pet"], copiedSchemaReference.Target);
        Assert.NotSame(originalSchemas["Pet"], copiedSchemaReference.Target);
    }

    [Fact]
    public void CreateDeepCopyPreservesOrderedDocumentTags()
    {
        var document = CreateDocument();
        document.Tags = new SortedSet<OpenApiTag>(new DescendingOpenApiTagComparer())
        {
            new() { Name = "tagB" },
            new() { Name = "tagA" },
            new() { Name = "tagC" },
        };

        var copy = document.CreateDeepCopy();

        var copiedTags = Assert.IsType<SortedSet<OpenApiTag>>(copy.Tags);
        Assert.Equal(["tagC", "tagB", "tagA"], copiedTags.Select(static t => t.Name));
        var originalTags = Assert.IsType<SortedSet<OpenApiTag>>(document.Tags);
        Assert.NotSame(originalTags.First(), copiedTags.First());
    }

    [Fact]
    public void CreateDeepCopyPreservesImmutableOrderedDocumentTags()
    {
        var document = CreateDocument();
        document.Tags = ImmutableSortedSet.Create(
            new DescendingOpenApiTagComparer(),
            [
                new OpenApiTag { Name = "tagB" },
                new OpenApiTag { Name = "tagA" },
                new OpenApiTag { Name = "tagC" },
            ]);

        var copy = document.CreateDeepCopy();

        var copiedTags = Assert.IsType<ImmutableSortedSet<OpenApiTag>>(copy.Tags);
        Assert.Equal(["tagC", "tagB", "tagA"], copiedTags.Select(static t => t.Name));
        var originalTags = Assert.IsType<ImmutableSortedSet<OpenApiTag>>(document.Tags);
        Assert.NotSame(originalTags.First(), copiedTags.First());
    }

    private static OpenApiMediaType GetJsonMediaType(OpenApiDocument document)
    {
        var pathItem = Assert.IsType<OpenApiPathItem>(document.Paths["/pets"]);
        var operations = pathItem.Operations;
        Assert.NotNull(operations);
        var operation = operations[HttpMethod.Get];
        var responses = operation.Responses;
        Assert.NotNull(responses);
        var response = Assert.IsType<OpenApiResponse>(responses["200"]);
        var content = response.Content;
        Assert.NotNull(content);
        return Assert.IsType<OpenApiMediaType>(content["application/json"]);
    }

    private static OpenApiDocument CreateDocument()
    {
        var document = new OpenApiDocument
        {
            Info = new OpenApiInfo
            {
                Title = "Pets",
                Version = "1.0.0",
                Extensions = new Dictionary<string, IOpenApiExtension>
                {
                    ["x-info"] = new JsonNodeExtension(new JsonObject { ["value"] = "original" })
                }
            },
            Components = new OpenApiComponents(),
            Paths = new OpenApiPaths()
        };

        var petSchema = new OpenApiSchema
        {
            Type = JsonSchemaType.Object,
            Properties = new Dictionary<string, IOpenApiSchema>
            {
                ["id"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description = "original id"
                }
            }
        };
        petSchema.Properties["friend"] = petSchema;
        document.Components.Schemas = new Dictionary<string, IOpenApiSchema>
        {
            ["Pet"] = petSchema
        };

        document.Paths.Add("/pets", new OpenApiPathItem
        {
            Operations = new Dictionary<HttpMethod, OpenApiOperation>
            {
                [HttpMethod.Get] = new OpenApiOperation
                {
                    Responses = new OpenApiResponses
                    {
                        ["200"] = new OpenApiResponse
                        {
                            Description = "ok",
                            Content = new Dictionary<string, IOpenApiMediaType>
                            {
                                ["application/json"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchemaReference("Pet", document),
                                    Example = new JsonObject { ["name"] = "original" }
                                }
                            }
                        }
                    }
                }
            }
        });

        document.SetReferenceHostDocument();
        return document;
    }

    private sealed class DescendingOpenApiTagComparer : IComparer<OpenApiTag>
    {
        public int Compare(OpenApiTag x, OpenApiTag y)
        {
            return string.Compare(y?.Name, x?.Name, System.StringComparison.Ordinal);
        }
    }
}
