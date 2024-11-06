using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Hidi.Formatters;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Services;
using Xunit;

namespace Microsoft.OpenApi.Hidi.Tests.Formatters
{
    public class PowerShellFormatterTests
    {
        [Theory]
        [InlineData("drives.drive.ListDrive", "drive_ListDrive", OperationType.Get)]
        [InlineData("print.taskDefinitions.tasks.GetTrigger", "print.taskDefinition.task_GetTrigger", OperationType.Get)]
        [InlineData("groups.sites.termStore.groups.GetSets", "group.site.termStore.group_GetSet", OperationType.Get)]
        [InlineData("external.industryData.ListDataConnectors", "external.industryData_ListDataConnector", OperationType.Get)]
        [InlineData("applications.application.UpdateLogo", "application_SetLogo", OperationType.Put)]
        [InlineData("identityGovernance.lifecycleWorkflows.workflows.workflow.activate", "identityGovernance.lifecycleWorkflow.workflow_activate", OperationType.Post)]
        [InlineData("directory.GetDeletedItems.AsApplication", "directory_GetDeletedItemAsApplication", OperationType.Get)]
        [InlineData("education.users.GetCount-6be9", "education.user_GetCount", OperationType.Get)]
        public void FormatOperationIdsInOpenAPIDocument(string operationId, string expectedOperationId, OperationType operationType, string path = "/foo")
        {
            // Arrange
            var openApiDocument = new OpenApiDocument
            {
                Info = new() { Title = "Test", Version = "1.0" },
                Servers = new List<OpenApiServer> { new() { Url = "https://localhost/" } },
                Paths = new()
                {
                    { path, new() {
                        Operations = new Dictionary<OperationType, OpenApiOperation>
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
            Assert.Equal(expectedOperationId, openApiDocument.Paths[path].Operations[operationType].OperationId);
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

            var testSchema = openApiDocument.Components?.Schemas?["TestSchema"];
            var averageAudioDegradationProperty = testSchema?.Properties["averageAudioDegradation"];
            var defaultPriceProperty = testSchema?.Properties["defaultPrice"];

            // Assert
            Assert.NotNull(openApiDocument.Components);
            Assert.NotNull(openApiDocument.Components.Schemas);
            Assert.NotNull(testSchema);
            Assert.Null(averageAudioDegradationProperty?.AnyOf);
            Assert.Equal(JsonSchemaType.Number, averageAudioDegradationProperty?.Type);
            Assert.Equal("float", averageAudioDegradationProperty?.Format);
            Assert.True(averageAudioDegradationProperty?.Nullable);
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

            var idsParameter = openApiDocument.Paths["/foo"].Operations[OperationType.Get].Parameters?.Where(static p => p.Name == "ids").FirstOrDefault();

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
                Servers = new List<OpenApiServer> { new() { Url = "https://localhost/" } },
                Paths = new() {
                    { "/foo", new()
                        {
                            Operations = new Dictionary<OperationType, OpenApiOperation>
                            {
                                {
                                    OperationType.Get, new()
                                    {
                                        OperationId = "Foo.GetFoo",
                                        Parameters = new List<OpenApiParameter>
                                        {
                                            new()
                                            {
                                                Name = "ids",
                                                In = ParameterLocation.Query,
                                                Content = new Dictionary<string, OpenApiMediaType>
                                                {
                                                    {
                                                        "application/json",
                                                        new OpenApiMediaType
                                                        {
                                                            Schema = new()
                                                            {
                                                                Type = JsonSchemaType.Array,
                                                                Items = new()
                                                                {
                                                                    Type = JsonSchemaType.String
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        },
                                        Extensions = new Dictionary<string, IOpenApiExtension>
                                        {
                                            {
                                                "x-ms-docs-operation-type", new OpenApiAny("function")
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
                    Schemas = new Dictionary<string, OpenApiSchema>
                    {
                        { "TestSchema",  new OpenApiSchema
                            {
                                Type = JsonSchemaType.Object,
                                Properties = new Dictionary<string, OpenApiSchema>
                                {
                                    {
                                        "averageAudioDegradation", new OpenApiSchema
                                        {
                                            AnyOf = new List<OpenApiSchema>
                                            {
                                                new() { Type = JsonSchemaType.Number },
                                                new() { Type = JsonSchemaType.String }
                                            },
                                            Format = "float",
                                            Nullable = true
                                        }
                                    },
                                    {
                                        "defaultPrice", new OpenApiSchema
                                        {
                                            OneOf = new List<OpenApiSchema>
                                            {
                                                new() { Type = JsonSchemaType.Number, Format = "double" },
                                                new() { Type = JsonSchemaType.String }
                                            }
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
