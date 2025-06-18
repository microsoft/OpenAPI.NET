using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V31Tests
{
    public class RelativeReferenceTests
    {
        private const string SampleFolderPath = "V31Tests/ReferenceSamples";

        [Fact]
        public async Task ParseInlineLocalReferenceWorks()
        {
            // Arrange
            var filePath = Path.Combine(SampleFolderPath, "inlineLocalReference.yaml");

            // Act
            var actual = (await OpenApiDocument.LoadAsync(filePath, SettingsFixture.ReaderSettings)).Document;
            var schemaType = actual.Paths["/item"].Operations[HttpMethod.Get].Parameters[0].Schema.Type;

            // Assert
            Assert.Equal(JsonSchemaType.Number, schemaType);
        }

        [Fact]
        public async Task ParseInlineExternalReferenceWorks()
        {
            // Arrange
            var expected = new JsonArray
            {
                new JsonObject
                {
                    ["name"] = "thing",
                    ["description"] = "a thing"
                }
            };

            var path = Path.Combine(Directory.GetCurrentDirectory(), SampleFolderPath);

            var settings = new OpenApiReaderSettings
            {
                LoadExternalRefs = true,
                BaseUrl = new(path),
            };
            settings.AddYamlReader();

            // Act
            var actual = (await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, "inlineExternalReference.yaml"), settings)).Document;
            var exampleValue = actual.Paths["/items"].Operations[HttpMethod.Get].Responses["200"].Content["application/json"].Examples["item-list"].Value;

            // Assert
            Assert.NotNull(exampleValue);
            Assert.IsType<JsonArray>(exampleValue);
            Assert.Equal(expected.ToJsonString(), exampleValue.ToJsonString());
        }

        [Fact]
        public async Task ParseComponentExternalReferenceWorks()
        {
            // Arrange
            var path = Path.Combine(Directory.GetCurrentDirectory(), SampleFolderPath);
            var settings = new OpenApiReaderSettings
            {
                LoadExternalRefs = true,
                BaseUrl = new(path),
            };
            settings.AddYamlReader();

            // Act
            var actual = (await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, "componentExternalReference.yaml"), settings)).Document;
            var securitySchemeValue = actual.Components.SecuritySchemes["customapikey"];

            // Assert
            Assert.Equal("x-api-key", securitySchemeValue.Name);
        }

        [Fact]
        public async Task ParseRootInlineJsonSchemaReferenceWorks()
        {
            // Arrange
            var filePath = Path.Combine(SampleFolderPath, "rootInlineSchemaReference.yaml");

            // Act
            var actual = (await OpenApiDocument.LoadAsync(filePath, SettingsFixture.ReaderSettings)).Document;
            var schema = actual.Paths["/item"].Operations[HttpMethod.Get].Responses["200"].Content["application/json"].Schema;

            // Assert
            Assert.Equal(JsonSchemaType.Object, schema.Type);
        }

        [Fact]
        public async Task ParseSubschemaInlineJsonSchemaReferenceWorks()
        {
            // Arrange
            var filePath = Path.Combine(SampleFolderPath, "subschemaInlineSchemaReference.yaml");

            // Act
            var actual = (await OpenApiDocument.LoadAsync(filePath, SettingsFixture.ReaderSettings)).Document;
            var schema = actual.Paths["/items"].Operations[HttpMethod.Get].Responses["200"].Content["application/json"].Schema.Items;

            // Assert
            Assert.Equal(JsonSchemaType.Object, schema.Type);
        }

        [Fact]
        public async Task ParseRootComponentJsonSchemaReferenceWorks()
        {
            // Arrange
            var filePath = Path.Combine(SampleFolderPath, "rootComponentSchemaReference.yaml");

            // Act
            var actual = (await OpenApiDocument.LoadAsync(filePath, SettingsFixture.ReaderSettings)).Document;
            var schema = actual.Components.Schemas["specialitem"];

            // Assert
            Assert.Equal(JsonSchemaType.Object, schema.Type);
            Assert.Equal("Item", schema.Title);
        }

        [Fact]
        public async Task ParseSubschemaComponentJsonSchemaReferenceWorks()
        {
            // Arrange
            var filePath = Path.Combine(SampleFolderPath, "subschemaComponentSchemaReference.yaml");

            // Act
            var actual = (await OpenApiDocument.LoadAsync(filePath, SettingsFixture.ReaderSettings)).Document;
            var schema = actual.Components.Schemas["items"].Items;

            // Assert
            Assert.Equal(JsonSchemaType.Object, schema.Type);
        }

        [Fact]
        public async Task ParseInternalComponentSubschemaJsonSchemaReferenceWorks()
        {
            // Arrange
            var filePath = Path.Combine(SampleFolderPath, "internalComponentsSubschemaReference.yaml");

            // Act
            var actual = (await OpenApiDocument.LoadAsync(filePath, SettingsFixture.ReaderSettings)).Document;
            var addressSchema = actual.Paths["/person/{id}/address"].Operations[HttpMethod.Get].Responses["200"].Content["application/json"].Schema;
            var itemsSchema = actual.Paths["/human"].Operations[HttpMethod.Get].Responses["200"].Content["application/json"].Schema;

            // Assert
            Assert.Equal(JsonSchemaType.Object, addressSchema.Type);
            Assert.Equal(JsonSchemaType.Integer, itemsSchema.Type);
        }

        [Fact]
        public async Task ParseExternalComponentSubschemaJsonSchemaReferenceWorks()
        {
            // Arrange
            var path = Path.Combine(Directory.GetCurrentDirectory(), SampleFolderPath);
            var settings = new OpenApiReaderSettings
            {
                LoadExternalRefs = true,
                BaseUrl = new(path),
            };
            settings.AddYamlReader();

            // Act
            var actual = (await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, "externalComponentSubschemaReference.yaml"), settings)).Document;
            var schema = actual.Paths["/person/{id}"].Operations[HttpMethod.Get].Responses["200"].Content["application/json"].Schema;

            // Assert
            Assert.Equal(JsonSchemaType.Object, schema.Type);
        }

        [Fact]
        public async Task ParseReferenceToInternalComponentUsingDollarIdWorks()
        {
            // Arrange
            var filePath = Path.Combine(SampleFolderPath, "internalComponentReferenceUsingId.yaml");

            // Act
            var actual = (await OpenApiDocument.LoadAsync(filePath, SettingsFixture.ReaderSettings)).Document;
            var schema = actual.Paths["/person/{id}"].Operations[HttpMethod.Get].Responses["200"].Content["application/json"].Schema;

            // Assert
            Assert.Equal(JsonSchemaType.Object, schema.Type);
        }

        [Fact]
        public async Task ParseLocalReferenceToJsonSchemaResourceWorks()
        {
            // Arrange
            var filePath = Path.Combine(SampleFolderPath, "localReferenceToJsonSchemaResource.yaml");
            var stringWriter = new StringWriter();
            var writer = new OpenApiYamlWriter(stringWriter);

            // Act
            var actual = (await OpenApiDocument.LoadAsync(filePath, SettingsFixture.ReaderSettings)).Document;
            var schema = actual.Components.Schemas["a"].Properties["b"].Properties["c"].Properties["b"];
            schema.SerializeAsV31(writer);

            // Assert
            Assert.Equal(JsonSchemaType.Object | JsonSchemaType.Null, schema.Type);
        }

        [Fact]
        public void ResolveSubSchema_ShouldTraverseKnownKeywords()
        {
            var schema = new OpenApiSchema
            {
                Type = JsonSchemaType.Object,
                Properties = new Dictionary<string, IOpenApiSchema>
                {
                    ["a"] = new OpenApiSchema
                    {
                        Properties = new Dictionary<string, IOpenApiSchema>
                        {
                            ["b"] = new OpenApiSchema { Type = JsonSchemaType.String }
                        }
                    }
                }
            };

            var path = new[] { "properties", "a", "properties", "b" };

            var result = OpenApiWorkspace.ResolveSubSchema(schema, path, []);

            Assert.NotNull(result);
            Assert.Equal(JsonSchemaType.String, result!.Type);
        }

        public static IEnumerable<object[]> SubSchemaKeywordPropertyPaths =>
            [
                [new[] { "properties", "properties" }],
                [new[] { "properties", "allOf" }]
            ];


        [Theory]
        [MemberData(nameof(SubSchemaKeywordPropertyPaths))]
        public void ResolveSubSchema_ShouldHandleUserDefinedKeywordNamedProperty(string[] pathSegments)
        {
            var schema = new OpenApiSchema
            {
                Type = JsonSchemaType.Object,
                Properties = new Dictionary<string, IOpenApiSchema>
                {
                    ["properties"] = new OpenApiSchema { Type = JsonSchemaType.String },
                    ["allOf"] = new OpenApiSchema { Type = JsonSchemaType.String }
                }
            };

            var result = OpenApiWorkspace.ResolveSubSchema(schema, pathSegments, []);

            Assert.NotNull(result);
            Assert.Equal(JsonSchemaType.String, result!.Type);
        }

        [Fact]
        public void ResolveSubSchema_ShouldRecurseIntoAllOfComposition()
        {
            var schema = new OpenApiSchema
            {
                AllOf =
                [
                    new OpenApiSchema
                    {
                        Properties = new Dictionary<string, IOpenApiSchema>
                        {
                            ["x"] = new OpenApiSchema { Type = JsonSchemaType.Integer }
                        }
                    }
                ]
            };

            var path = new[] { "allOf", "0", "properties", "x" };

            var result = OpenApiWorkspace.ResolveSubSchema(schema, path, []);

            Assert.NotNull(result);
            Assert.Equal(JsonSchemaType.Integer, result!.Type);
        }
        [Fact]
        public async Task ShouldResolveRelativeSubReference()
        {
            // Arrange
            var filePath = Path.Combine(SampleFolderPath, "relativeSubschemaReference.json");

            // Act
            var (actual, _) = await OpenApiDocument.LoadAsync(filePath, SettingsFixture.ReaderSettings);

            var fooComponentSchema = actual.Components.Schemas["Foo"];
            var seq1Property = fooComponentSchema.Properties["seq1"];
            Assert.NotNull(seq1Property);
            var seq2Property = fooComponentSchema.Properties["seq2"];
            Assert.NotNull(seq2Property);
            Assert.Equal(JsonSchemaType.Array, seq2Property.Items.Type);
            Assert.Equal(JsonSchemaType.String, seq2Property.Items.Items.Type);
        }
        [Fact]
        public async Task ShouldResolveRelativeSubReferenceUsingParsingContext()
        {
            // Arrange
            var filePath = Path.Combine(SampleFolderPath, "relativeSubschemaReference.json");
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var jsonNode = await JsonNode.ParseAsync(fs);
            var schemaJsonNode = jsonNode["components"]?["schemas"]?["Foo"];
            Assert.NotNull(schemaJsonNode);
            var diagnostic = new OpenApiDiagnostic();
            var parsingContext = new ParsingContext(diagnostic);
            parsingContext.StartObject("components");
            parsingContext.StartObject("schemas");
            parsingContext.StartObject("Foo");
            var document = new OpenApiDocument();

            // Act
            var fooComponentSchema = parsingContext.ParseFragment<OpenApiSchema>(schemaJsonNode, OpenApiSpecVersion.OpenApi3_1, document);
            document.AddComponent("Foo", fooComponentSchema);
            var seq1Property = fooComponentSchema.Properties["seq1"];
            Assert.NotNull(seq1Property);
            var seq2Property = fooComponentSchema.Properties["seq2"];
            Assert.NotNull(seq2Property);
            Assert.Equal(JsonSchemaType.Array, seq2Property.Items.Type);
            Assert.Equal(JsonSchemaType.String, seq2Property.Items.Items.Type);
        }
        [Fact]
        public void ShouldFailToResolveRelativeSubReferenceFromTheObjectModel()
        {
            var document = new OpenApiDocument
            {
                Info = new OpenApiInfo { Title = "Test API", Version = "1.0.0" },
            };
            document.Components = new OpenApiComponents
            {
                Schemas = new Dictionary<string, IOpenApiSchema>
                {
                    ["Foo"] = new OpenApiSchema
                    {
                        Properties = new Dictionary<string, IOpenApiSchema>
                        {
                            ["seq1"] = new OpenApiSchema { Type = JsonSchemaType.Array | JsonSchemaType.Null, Items = new OpenApiSchema { Type = JsonSchemaType.Array, Items = new OpenApiSchema { Type = JsonSchemaType.String } } },
                            ["seq2"] = new OpenApiSchema { Type = JsonSchemaType.Array | JsonSchemaType.Null, Items = new OpenApiSchemaReference("#/properties/seq1/items", document) }
                        }
                    }
                }
            };
            document.RegisterComponents();

            var fooComponentSchema = document.Components.Schemas["Foo"];
            var seq1Property = fooComponentSchema.Properties["seq1"];
            Assert.NotNull(seq1Property);
            var seq2Property = fooComponentSchema.Properties["seq2"];
            Assert.NotNull(seq2Property);
            Assert.Throws<ArgumentException>(() => seq2Property.Items.Type);
            // it's impossible to resolve relative references from the object model only because we don't have a way to get to 
            // the parent object to build the full path for the reference.


            // #/properties/seq1/items
            // #/components/schemas/Foo/properties/seq1/items
        }
        [Fact]
        public void ShouldResolveAbsoluteSubReferenceFromTheObjectModel()
        {
            var document = new OpenApiDocument
            {
                Info = new OpenApiInfo { Title = "Test API", Version = "1.0.0" },
            };
            document.Components = new OpenApiComponents
            {
                Schemas = new Dictionary<string, IOpenApiSchema>
                {
                    ["Foo"] = new OpenApiSchema
                    {
                        Properties = new Dictionary<string, IOpenApiSchema>
                        {
                            ["seq1"] = new OpenApiSchema { Type = JsonSchemaType.Array | JsonSchemaType.Null, Items = new OpenApiSchema { Type = JsonSchemaType.Array, Items = new OpenApiSchema { Type = JsonSchemaType.String } } },
                            ["seq2"] = new OpenApiSchema { Type = JsonSchemaType.Array | JsonSchemaType.Null, Items = new OpenApiSchemaReference("#/components/schemas/Foo/properties/seq1/items", document) }
                        }
                    }
                }
            };
            document.RegisterComponents();

            var fooComponentSchema = document.Components.Schemas["Foo"];
            var seq1Property = fooComponentSchema.Properties["seq1"];
            Assert.NotNull(seq1Property);
            var seq2Property = fooComponentSchema.Properties["seq2"];
            Assert.NotNull(seq2Property);
            Assert.Equal(JsonSchemaType.Array, seq2Property.Items.Type);
            Assert.Equal(JsonSchemaType.String, seq2Property.Items.Items.Type);
        }
        [Fact]
        public async Task ShouldResolveRecursiveRelativeSubReference()
        {
            // Arrange
            var filePath = Path.Combine(SampleFolderPath, "recursiveRelativeSubschemaReference.json");

            // Act
            var (actual, _) = await OpenApiDocument.LoadAsync(filePath, SettingsFixture.ReaderSettings);

            var fooComponentSchema = actual.Components.Schemas["Foo"];
            var fooSchemaParentProperty = fooComponentSchema.Properties["parent"];
            Assert.NotNull(fooSchemaParentProperty);
            var fooSchemaParentPropertyTagsProperty = fooSchemaParentProperty.Properties["tags"];
            Assert.NotNull(fooSchemaParentPropertyTagsProperty);
            Assert.Equal(JsonSchemaType.Array | JsonSchemaType.Null, fooSchemaParentPropertyTagsProperty.Type);
            Assert.Equal(JsonSchemaType.Object, fooSchemaParentPropertyTagsProperty.Items.Type);

            var fooSchemaTagsProperty = fooComponentSchema.Properties["tags"];
            Assert.NotNull(fooSchemaTagsProperty);
            Assert.Equal(JsonSchemaType.Array | JsonSchemaType.Null, fooSchemaTagsProperty.Type);
            Assert.Equal(JsonSchemaType.Object, fooSchemaTagsProperty.Items.Type);
        }
        [Fact]
        public async Task ShouldResolveReferencesInSchemasFromSystemTextJson()
        {
            var filePath = Path.Combine(SampleFolderPath, "STJSchema.json");
            using var fs = File.OpenRead(filePath);
            var jsonNode = await JsonNode.ParseAsync(fs);

            var parsingContext = new ParsingContext(new OpenApiDiagnostic());
            var document = new OpenApiDocument();
            var schema = parsingContext.ParseFragment<OpenApiSchema>(jsonNode, OpenApiSpecVersion.OpenApi3_1, document);
            Assert.NotNull(schema);

            document.AddComponent("Foo", schema);
            var tagsProperty = Assert.IsType<OpenApiSchemaReference>(schema.Properties["tags"]);
            // this is the reference that is generated by STJ schema generator which does not have OAI in context.
            Assert.Equal("#/properties/parent/properties/tags", tagsProperty.Reference.ReferenceV3);
            // this is the reference that needs to be used in the document for components resolution.
            var absoluteReferenceId = $"#/components/schemas/Foo{tagsProperty.Reference.ReferenceV3.Replace("#", string.Empty)}";
            schema.Properties["tags"] = new OpenApiSchemaReference(absoluteReferenceId, document);
            var updatedTagsProperty = Assert.IsType<OpenApiSchemaReference>(schema.Properties["tags"]);
            Assert.Equal(absoluteReferenceId, updatedTagsProperty.Reference.ReferenceV3);
            Assert.Equal(JsonSchemaType.Array | JsonSchemaType.Null, updatedTagsProperty.Type);
            Assert.Equal(JsonSchemaType.Object, updatedTagsProperty.Items.Type);


            // doing the same for the parent property

            var parentProperty = Assert.IsType<OpenApiSchema>(schema.Properties["parent"]);
            var parentSubProperty = Assert.IsType<OpenApiSchemaReference>(parentProperty.Properties["parent"]);
            Assert.Equal("#/properties/parent", parentSubProperty.Reference.ReferenceV3);
            parentProperty.Properties["parent"] = new OpenApiSchemaReference($"#/components/schemas/Foo{parentSubProperty.Reference.ReferenceV3.Replace("#", string.Empty)}", document);
            var updatedParentSubProperty = Assert.IsType<OpenApiSchemaReference>(parentProperty.Properties["parent"]);
            Assert.Equal(JsonSchemaType.Object | JsonSchemaType.Null, updatedParentSubProperty.Type);

            var pathItem = new OpenApiPathItem
            {
                Operations = new Dictionary<HttpMethod, OpenApiOperation>
                {
                    [HttpMethod.Post] = new OpenApiOperation
                    {
                        Responses = new OpenApiResponses
                        {
                            ["200"] = new OpenApiResponse
                            {
                            }
                        },
                        RequestBody = new OpenApiRequestBody
                        {
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                ["application/json"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchemaReference("#/components/schemas/Foo", document)
                                }
                            }
                        }
                    }
                }
            };
            document.Paths.Add("/", pathItem);

            var requestBodySchema = pathItem.Operations[HttpMethod.Post].RequestBody.Content["application/json"].Schema;
            Assert.NotNull(requestBodySchema);
            var requestBodyTagsProperty = Assert.IsType<OpenApiSchemaReference>(requestBodySchema.Properties["tags"]);
            Assert.Equal(JsonSchemaType.Object, requestBodyTagsProperty.Items.Type);
        }

        [Fact]
        public void ExitsEarlyOnCyclicalReferences()
        {
            var document = new OpenApiDocument
            {
                Info = new OpenApiInfo { Title = "Test API", Version = "1.0.0" },
            };
            var categorySchema = new OpenApiSchema
            {
                Type = JsonSchemaType.Object,
                Properties = new Dictionary<string, IOpenApiSchema>
                {
                    ["name"] = new OpenApiSchema { Type = JsonSchemaType.String },
                    ["parent"] = new OpenApiSchemaReference("#/components/schemas/Category", document),
                    // this is intentionally wrong and cyclical reference
                    // it tests whether we're going in an infinite resolution loop
                    ["tags"] = new OpenApiSchemaReference("#/components/schemas/Category/properties/parent/properties/tags", document)
                }
            };
            document.AddComponent("Category", categorySchema);
            document.RegisterComponents();

            var tagsSchemaRef = Assert.IsType<OpenApiSchemaReference>(categorySchema.Properties["tags"]);
            Assert.Null(tagsSchemaRef.Items);
            Assert.Equal("#/components/schemas/Category/properties/parent/properties/tags", tagsSchemaRef.Reference.ReferenceV3);
            Assert.Null(tagsSchemaRef.Target);

            var parentSchemaRef = Assert.IsType<OpenApiSchemaReference>(categorySchema.Properties["parent"]);
            Assert.Equal("#/components/schemas/Category", parentSchemaRef.Reference.ReferenceV3);
            Assert.NotNull(parentSchemaRef.Target);
        }
    }
}
