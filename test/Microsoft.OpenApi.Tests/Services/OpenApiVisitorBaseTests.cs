using System.Collections.Generic;
using Xunit;

namespace Microsoft.OpenApi.Tests.Services;

public class OpenApiVisitorBaseTests
{
    [Fact]
    public void EncodesReservedCharacters()
    {
        // Given
        var openApiDocument = new OpenApiDocument
        {
            Info = new()
            {
                Title = "foo",
                Version = "1.2.2"
            },
            Paths = new()
            {
            },
            Components = new()
            {
                Schemas = new Dictionary<string, IOpenApiSchema>()
                {
                    ["Pet~"] = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Object
                    },
                    ["Pet/"] = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Object
                    },
                }
            }
        };
        var visitor = new LocatorVisitor();

        // When
        visitor.Visit(openApiDocument);

        // Then
        Assert.Equivalent(
            new List<string>
            {
                "#/components/schemas/Pet~0",
                "#/components/schemas/Pet~1"
            }, visitor.Locations);
    }

    private class LocatorVisitor : OpenApiVisitorBase
    {
        public List<string> Locations { get; } = new List<string>();

        public override void Visit(IOpenApiSchema openApiSchema)
        {
            Locations.Add(this.PathString);
        }
        public override void Visit(OpenApiComponents components)
        {
            Enter("schemas");
            if (components.Schemas != null)
            {
                foreach (var schemaKvp in components.Schemas)
                {
                    Enter(schemaKvp.Key);
                    this.Visit(schemaKvp.Value);
                    Exit();
                }
            }
            Exit();
        }
        public override void Visit(OpenApiDocument doc)
        {
            Enter("components");
            Visit(doc.Components);
            Exit();
        }
    }
}
