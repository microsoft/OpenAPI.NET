﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OpenApi.Interfaces;
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

            locator.Locations.Should().BeEquivalentTo(new List<string> {
                "#/servers",
                "#/tags"
            });
        }

        [Fact]
        public void LocateTopLevelArrayItems()
        {
            var doc = new OpenApiDocument
            {
                Servers = new List<OpenApiServer>
                {
                    new(),
                    new()
                },
                Paths = new(),
                Tags = new List<OpenApiTag>
                {
                    new()
                }
            };

            var locator = new LocatorVisitor();
            var walker = new OpenApiWalker(locator);
            walker.Walk(doc);

            locator.Locations.Should().BeEquivalentTo(new List<string> {
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
            var doc = new OpenApiDocument
            {
                Paths = new()
            };
            doc.Paths.Add("/test", new()
            {
                Operations = new Dictionary<OperationType, OpenApiOperation>
                {
                    [OperationType.Get] = new()
                    {
                        Responses = new()
                        {
                            ["200"] = new()
                            {
                                Content = new Dictionary<string, OpenApiMediaType>
                                {
                                    ["application/json"] = new()
                                    {
                                        Schema = new()
                                        {
                                            Type = "string"
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

            locator.Locations.Should().BeEquivalentTo(new List<string> {
                "#/servers",
                "#/paths",
                "#/paths/~1test",
                "#/paths/~1test/get",
                "#/paths/~1test/get/responses",
                "#/paths/~1test/get/responses/200",
                "#/paths/~1test/get/responses/200/content",
                "#/paths/~1test/get/responses/200/content/application~1json",
                "#/paths/~1test/get/responses/200/content/application~1json/schema",
                "#/paths/~1test/get/tags",
                "#/tags",

            });

            locator.Keys.Should().BeEquivalentTo(new List<string> { "/test", "Get", "200", "application/json" });
        }

        [Fact]
        public void WalkDOMWithCycles()
        {
            var loopySchema = new OpenApiSchema
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    ["name"] = new() { Type = "string" }
                }
            };

            loopySchema.Properties.Add("parent", loopySchema);

            var doc = new OpenApiDocument
            {
                Paths = new(),
                Components = new()
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

            locator.Locations.Should().BeEquivalentTo(new List<string> {
                "#/servers",
                "#/paths",
                "#/components",
                "#/components/schemas/loopy",
                "#/components/schemas/loopy/properties/name",
                "#/tags"
            });
        }

        /// <summary>
        /// Walk document and discover all references to components, including those inside components
        /// </summary>
        [Fact]
        public void LocateReferences()
        {
            var baseSchema = new OpenApiSchema
            {
                Reference = new()
                {
                    Id = "base",
                    Type = ReferenceType.Schema
                },
                UnresolvedReference = false
            };

            var derivedSchema = new OpenApiSchema
            {
                AnyOf = new List<OpenApiSchema> { baseSchema },
                Reference = new()
                {
                    Id = "derived",
                    Type = ReferenceType.Schema
                },
                UnresolvedReference = false
            };

            var testHeader = new OpenApiHeader
            {
                Schema = derivedSchema,
                Reference = new()
                {
                    Id = "test-header",
                    Type = ReferenceType.Header
                },
                UnresolvedReference = false
            };

            var doc = new OpenApiDocument
            {
                Paths = new()
                {
                    ["/"] = new()
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>
                        {
                            [OperationType.Get] = new()
                            {
                                Responses = new()
                                {
                                    ["200"] = new()
                                    {
                                        Content = new Dictionary<string, OpenApiMediaType>
                                        {
                                            ["application/json"] = new()
                                            {
                                                Schema = derivedSchema
                                            }
                                        },
                                        Headers = new Dictionary<string, OpenApiHeader>
                                        {
                                            ["test-header"] = testHeader
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
                        ["derived"] = derivedSchema,
                        ["base"] = baseSchema,
                    },
                    Headers = new Dictionary<string, OpenApiHeader>
                    {
                        ["test-header"] = testHeader
                    }
                }
            };

            var locator = new LocatorVisitor();
            var walker = new OpenApiWalker(locator);
            walker.Walk(doc);

            locator.Locations.Where(l => l.StartsWith("referenceAt:")).Should().BeEquivalentTo(new List<string> {
                "referenceAt: #/paths/~1/get/responses/200/content/application~1json/schema",
                "referenceAt: #/paths/~1/get/responses/200/headers/test-header",
                "referenceAt: #/components/schemas/derived/anyOf/0",
                "referenceAt: #/components/headers/test-header/schema"
            });
        }
    }

    internal class LocatorVisitor : OpenApiVisitorBase
    {
        public List<string> Locations = new();
        public List<string> Keys = new();

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

        public override void Visit(IOpenApiReferenceable referenceable)
        {
            Locations.Add("referenceAt: " + this.PathString);
        }
        public override void Visit(IDictionary<string, OpenApiMediaType> content)
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
