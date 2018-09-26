// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Services;
using Xunit;

namespace Microsoft.OpenApi.Tests.Walkers
{
    public class WalkerLocationTests
    {

        [Fact]
        public void LocateTopLevelObjects()
        {
            var doc = new OpenApiDocument();
            
            var locator = new LocatorVisitor();
            var walker = new OpenApiWalker(locator);
            walker.Walk(doc);

            locator.Locations.ShouldBeEquivalentTo(new List<string> {
                "#/servers",
                "#/tags"
            });
        }

        [Fact]
        public void LocateTopLevelArrayItems()
        {
            var doc = new OpenApiDocument()
            {
                Servers = new List<OpenApiServer>() {
                    new OpenApiServer(),
                    new OpenApiServer()
                },
                Paths = new OpenApiPaths(),
                Tags = new List<OpenApiTag>()
                {
                    new OpenApiTag()
                }
            };

            var locator = new LocatorVisitor();
            var walker = new OpenApiWalker(locator);
            walker.Walk(doc);

            locator.Locations.ShouldBeEquivalentTo(new List<string> {
                "#/servers",
                "#/servers/0",
                "#/servers/1",
                "#/paths",
                "#/tags",
                "#/tags/0"
            });
        }

        [Fact]
        public void LocatePathOperationContentSchema()
        {
            var doc = new OpenApiDocument();
            doc.Paths = new OpenApiPaths();
            doc.Paths.Add("/test", new OpenApiPathItem()
            {
                Operations = new Dictionary<OperationType, OpenApiOperation>()
                {
                    { OperationType.Get, new OpenApiOperation()
                    {
                        Responses = new OpenApiResponses()
                        {
                            { "200", new OpenApiResponse() {
                                    Content = new Dictionary<string,OpenApiMediaType>
                                    {
                                        { "application/json", new OpenApiMediaType {
                                                Schema = new OpenApiSchema
                                                {
                                                    Type = "string"
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

            var locator = new LocatorVisitor();
            var walker = new OpenApiWalker(locator);
            walker.Walk(doc);

            locator.Locations.ShouldBeEquivalentTo(new List<string> {
                "#/servers",
                "#/tags",
                "#/paths",
                "#/paths/~1test",
                "#/paths/~1test/get",
                "#/paths/~1test/get/tags",
                "#/paths/~1test/get/responses",
                "#/paths/~1test/get/responses/200",
                "#/paths/~1test/get/responses/200/content",
                "#/paths/~1test/get/responses/200/content/application~1json",
                "#/paths/~1test/get/responses/200/content/application~1json/schema",

            });

            locator.Keys.ShouldBeEquivalentTo(new List<string> { "/test","Get","200", "application/json" });
        }

        [Fact]
        public void WalkDOMWithCycles()
        {
            var loopySchema = new OpenApiSchema()
            {
                Type = "object",
                Properties = new Dictionary<string,OpenApiSchema>()
                {
                    ["name"] = new OpenApiSchema() { Type = "string" }
                }
            };

            loopySchema.Properties.Add("parent", loopySchema);

            var doc = new OpenApiDocument()
            {
                Paths = new OpenApiPaths(),
                Components = new OpenApiComponents()
                {
                    Schemas = new Dictionary<string, OpenApiSchema>
                    {
                        ["loopy"] = loopySchema
                    }
                }
            };

            var locator = new LocatorVisitor();
            var walker = new OpenApiWalker(locator);
            walker.Walk(doc);

            locator.Locations.ShouldBeEquivalentTo(new List<string> {
                "#/servers",
                "#/paths",
                "#/components",
                "#/components/schemas/loopy",
                "#/components/schemas/loopy/properties/name",
                "#/tags"
            });
        }
    }

    internal class LocatorVisitor : OpenApiVisitorBase
    {
        public List<string> Locations = new List<string>();
        public List<string> Keys = new List<string>();

        public override void Visit(OpenApiInfo info)
        {
            Locations.Add(this.PathString);
        }

        public override void Visit(OpenApiComponents components)
        {
            Locations.Add(this.PathString);
        }

        public override void Visit(OpenApiExternalDocs externalDocs)
        {
            Locations.Add(this.PathString);
        }

        public override void Visit(OpenApiPaths paths)
        {
            Locations.Add(this.PathString);
        }

        public override void Visit(OpenApiPathItem pathItem)
        {
            Keys.Add(CurrentKeys.Path);
            Locations.Add(this.PathString);
        }

        public override void Visit(OpenApiResponses responses)
        {
            Locations.Add(this.PathString);
        }

        public override void Visit(OpenApiOperation operation)
        {
            Keys.Add(CurrentKeys.Operation.ToString());
            Locations.Add(this.PathString);
        }
        public override void Visit(OpenApiResponse response)
        {
            Keys.Add(CurrentKeys.Response);
            Locations.Add(this.PathString);
        }

        public override void Visit(IDictionary<string,OpenApiMediaType> content)
        {
            Locations.Add(this.PathString);
        }

        public override void Visit(OpenApiMediaType mediaType)
        {
            Keys.Add(CurrentKeys.Content);
            Locations.Add(this.PathString);
        }

        public override void Visit(OpenApiSchema schema)
        {
            Locations.Add(this.PathString);
        }

        public override void Visit(IList<OpenApiTag> openApiTags)
        {
            Locations.Add(this.PathString);
        }

        public override void Visit(IList<OpenApiServer> servers)
        {
            Locations.Add(this.PathString);
        }

        public override void Visit(OpenApiServer server)
        {
            Locations.Add(this.PathString);
        }
    }
}
