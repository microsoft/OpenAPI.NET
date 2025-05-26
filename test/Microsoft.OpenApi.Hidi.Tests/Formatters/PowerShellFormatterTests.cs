using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Hidi.Formatters;
using Microsoft.OpenApi.Services;
using Xunit;

namespace Microsoft.OpenApi.Hidi.Tests.Formatters
{
    public class PowerShellFormatterTests
    {
        public static IEnumerable<object[]> TestCases
        {
            get
            {
                yield return new object[] { "drives.drive.ListDrive", "drive_ListDrive", HttpMethod.Get };
                yield return new object[] { "print.taskDefinitions.tasks.GetTrigger", "print.taskDefinition.task_GetTrigger", HttpMethod.Get };
                yield return new object[] { "groups.sites.termStore.groups.GetSets", "group.site.termStore.group_GetSet", HttpMethod.Get };
                yield return new object[] { "external.industryData.ListDataConnectors", "external.industryData_ListDataConnector", HttpMethod.Get };
                yield return new object[] { "applications.application.UpdateLogo", "application_SetLogo", HttpMethod.Put };
                yield return new object[] { "identityGovernance.lifecycleWorkflows.workflows.workflow.activate", "identityGovernance.lifecycleWorkflow.workflow_activate", HttpMethod.Post };
                yield return new object[] { "directory.GetDeletedItems.AsApplication", "directory_GetDeletedItemAsApplication", HttpMethod.Get };
                yield return new object[] { "education.users.GetCount-6be9", "education.user_GetCount", HttpMethod.Get };
            }
        }
        [Theory]
        [MemberData(nameof(TestCases))]
        public void FormatOperationIdsInOpenAPIDocument(string operationId, string expectedOperationId, HttpMethod operationType, string path = "/foo")
        {
            // Arrange
            var openApiDocument = new OpenApiDocument
            {
                Info = new() { Title = "Test", Version = "1.0" },
                Servers = [new() { Url = "https://localhost/" }],
                Paths = new()
                {
                    { path, new OpenApiPathItem() {
                        Operations = new()
                        {
                            { operationType, new() { OperationId = operationId } }
                          }
                        }
                    }
                }
            };

            // Act
            var powerShellFormatter = new PowerShellFormatter();
            var walker = new OpenApiWalker(powerShellFormatter);
            walker.Walk(openApiDocument);

            // Assert
            Assert.Equal(expectedOperationId, openApiDocument.Paths[path].Operations?[operationType].OperationId);
        }

        [Fact]
        public void RemoveAnyOfAndOneOfFromSchema()
        {
            // Arrange
            var openApiDocument = GetSampleOpenApiDocument();

            // Act
            var powerShellFormatter = new PowerShellFormatter();
            var walker = new OpenApiWalker(powerShellFormatter);
            walker.Walk(openApiDocument);

            Assert.NotNull(openApiDocument.Components);
            Assert.NotNull(openApiDocument.Components.Schemas);
            var testSchema = openApiDocument.Components.Schemas["TestSchema"];
            var averageAudioDegradationProperty = testSchema.Properties?["averageAudioDegradation"];
            var defaultPriceProperty = testSchema.Properties?["defaultPrice"];

            // Assert
            Assert.NotNull(openApiDocument.Components);
            Assert.NotNull(openApiDocument.Components.Schemas);
            Assert.NotNull(testSchema);
            Assert.Null(averageAudioDegradationProperty?.AnyOf);
            Assert.Equal(JsonSchemaType.Number | JsonSchemaType.Null, averageAudioDegradationProperty?.Type);
            Assert.Equal("float", averageAudioDegradationProperty?.Format);
            Assert.Equal(JsonSchemaType.Null, averageAudioDegradationProperty?.Type & JsonSchemaType.Null);
            Assert.Null(defaultPriceProperty?.OneOf);
            Assert.Equal(JsonSchemaType.Number, defaultPriceProperty?.Type);
            Assert.Equal("double", defaultPriceProperty?.Format);
            Assert.NotNull(testSchema.AdditionalProperties);
        }

        [Fact]
        public void ResolveFunctionParameters()
        {
            // Arrange
            var openApiDocument = GetSampleOpenApiDocument();

            // Act
            var powerShellFormatter = new PowerShellFormatter();
            var walker = new OpenApiWalker(powerShellFormatter);
            walker.Walk(openApiDocument);

            var idsParameter = openApiDocument.Paths["/foo"].Operations?[HttpMethod.Get].Parameters?.FirstOrDefault(static p => p.Name == "ids");

            // Assert
            Assert.Null(idsParameter?.Content);
            Assert.NotNull(idsParameter?.Schema);
            Assert.Equal(JsonSchemaType.Array, idsParameter?.Schema.Type);
        }

        private static OpenApiDocument GetSampleOpenApiDocument()
        {
            return new()
            {
                Info = new() { Title = "Test", Version = "1.0" },
                Servers = [new() { Url = "https://localhost/" }],
                Paths = new() {
                    { "/foo", new OpenApiPathItem()
                        {
                            Operations = new()
                            {
                                {
                                    HttpMethod.Get, new()
                                    {
                                        OperationId = "Foo.GetFoo",
                                        Parameters =
                                        [
                                            new OpenApiParameter()
                                            {
                                                Name = "ids",
                                                In = ParameterLocation.Query,
                                                Content = new()
                                                {
                                                    {
                                                        "application/json",
                                                        new OpenApiMediaType
                                                        {
                                                            Schema = new OpenApiSchema()
                                                            {
                                                                Type = JsonSchemaType.Array,
                                                                Items = new OpenApiSchema()
                                                                {
                                                                    Type = JsonSchemaType.String
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        ],
                                        Extensions = new()
                                        {
                                            {
                                                "x-ms-docs-operation-type", new JsonNodeExtension("function")
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                Components = new()
                {
                    Schemas = new()
                    {
                        { "TestSchema",  new OpenApiSchema
                            {
                                Type = JsonSchemaType.Object,
                                Properties = new()
                                {
                                    {
                                        "averageAudioDegradation", new OpenApiSchema
                                        {
                                            AnyOf =
                                            [
                                                new OpenApiSchema() { Type = JsonSchemaType.Number | JsonSchemaType.Null },
                                                new OpenApiSchema() { Type = JsonSchemaType.String }
                                            ],
                                            Format = "float",
                                        }
                                    },
                                    {
                                        "defaultPrice", new OpenApiSchema
                                        {
                                            OneOf =
                                            [
                                                new OpenApiSchema() { Type = JsonSchemaType.Number, Format = "double" },
                                                new OpenApiSchema() { Type = JsonSchemaType.String }
                                            ]
                                        }
                                    }
                                }
                            }
                        } 
                    }
                }
            };
        }
    }
}
