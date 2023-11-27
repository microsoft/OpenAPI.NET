using Json.Schema;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Hidi.Formatters;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Services;
using Xunit;
using Microsoft.OpenApi.Extensions;

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

            var testSchema = openApiDocument.Components.Schemas["TestSchema"];
            var averageAudioDegradationProperty = testSchema.GetProperties()?.GetValueOrDefault("averageAudioDegradation");
            var defaultPriceProperty = testSchema.GetProperties()?.GetValueOrDefault("defaultPrice");

            // Assert
            Assert.Null(averageAudioDegradationProperty?.GetAnyOf());
            Assert.Equal(SchemaValueType.Number, averageAudioDegradationProperty?.GetJsonType());
            Assert.Equal("float", averageAudioDegradationProperty?.GetFormat()?.Key);
            Assert.True(averageAudioDegradationProperty?.GetNullable());
            Assert.Null(defaultPriceProperty?.GetOneOf());
            Assert.Equal(SchemaValueType.Number, defaultPriceProperty?.GetJsonType());
            Assert.Equal("double", defaultPriceProperty?.GetFormat()?.Key);
            Assert.NotNull(testSchema.GetAdditionalProperties());
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

            var idsParameter = openApiDocument.Paths["/foo"].Operations[OperationType.Get].Parameters.Where(static p => p.Name == "ids").FirstOrDefault();

            // Assert
            Assert.Null(idsParameter?.Content);
            Assert.NotNull(idsParameter?.Schema);
            Assert.Equal(SchemaValueType.Array, idsParameter?.Schema.GetJsonType());
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
                                                            Schema = new JsonSchemaBuilder()
                                                                .Type(SchemaValueType.Array)
                                                                .Items(new JsonSchemaBuilder()
                                                                    .Type(SchemaValueType.String))
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
                    Schemas = new Dictionary<string, JsonSchema>
                    {
                        { "TestSchema",  new JsonSchemaBuilder()                       
                                .Type(SchemaValueType.Object)
                                .Properties(("averageAudioDegradation", new JsonSchemaBuilder()
                                            .AnyOf(
                                                    new JsonSchemaBuilder().Type(SchemaValueType.Number),
                                                    new JsonSchemaBuilder().Type(SchemaValueType.String))
                                            .Format("float")
                                            .Nullable(true)),
 
                                        ("defaultPrice", new JsonSchemaBuilder()
                                            .OneOf(
                                                new JsonSchemaBuilder().Type(SchemaValueType.Number).Format("double"),
                                                new JsonSchemaBuilder().Type(SchemaValueType.String))))
                        }
                    }
                }
            };
        }
    }
}
