﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace Microsoft.OpenApi.Tests.Services
{
    [Collection("DefaultSettings")]
    public class OpenApiValidatorTests
    {
        [Fact]
        public void ResponseMustHaveADescription()
        {
            var openApiDocument = new OpenApiDocument
            {
                Info = new()
                {
                    Title = "foo",
                    Version = "1.2.2"
                },
                Paths = new()
                {
                    {
                        "/test",
                        new OpenApiPathItem()
                        {
                            Operations = new Dictionary<HttpMethod, OpenApiOperation>
                            {
                                [HttpMethod.Get] = new()
                                {
                                    Responses =
                                    {
                                        ["200"] = new OpenApiResponse()
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var validator = new OpenApiValidator(ValidationRuleSet.GetDefaultRuleSet());
            var walker = new OpenApiWalker(validator);
            walker.Walk(openApiDocument);

            Assert.Equivalent(
                    new List<OpenApiError>
                    {
                        new OpenApiValidatorError(nameof(OpenApiResponseRules.ResponseRequiredFields),"#/paths/~1test/get/responses/200/description",
                            string.Format(SRResource.Validation_FieldIsRequired, "description", "response"))
                    }, validator.Errors);
        }

        [Fact]
        public void ServersShouldBeReferencedByIndex()
        {
            var openApiDocument = new OpenApiDocument
            {
                Info = new()
                {
                    Title = "foo",
                    Version = "1.2.2"
                },
                Servers = [
                new()
                {
                    Url = "http://example.org"
                },
                new()
                {
                },
            ],
                Paths = new()
            };

            var validator = new OpenApiValidator(ValidationRuleSet.GetDefaultRuleSet());
            var walker = new OpenApiWalker(validator);
            walker.Walk(openApiDocument);

            Assert.Equivalent(
                    new List<OpenApiError>
                    {
                        new OpenApiValidatorError(nameof(OpenApiServerRules.ServerRequiredFields), "#/servers/1/url",
                            string.Format(SRResource.Validation_FieldIsRequired, "url", "server"))
                    }, validator.Errors);
        }

        [Fact]
        public void ValidateCustomExtension()
        {
            var ruleset = ValidationRuleSet.GetDefaultRuleSet();

            ruleset.Add(typeof(JsonNodeExtension), 
             new ValidationRule<JsonNodeExtension>("FooExtensionRule",
                 (context, item) =>
                 {
                     if (item.Node["Bar"].ToString() == "hey")
                     {
                         context.AddError(new("FooExtensionRule", context.PathString, "Don't say hey"));
                     }
                 }));

            var openApiDocument = new OpenApiDocument
            {
                Info = new()
                {
                    Title = "foo",
                    Version = "1.2.2"
                },
                Paths = new()
            };

            var fooExtension = new FooExtension
            {
                Bar = "hey",
                Baz = "baz"
            };

            var extensionNode = JsonSerializer.Serialize(fooExtension);
            var jsonNode = JsonNode.Parse(extensionNode);
            openApiDocument.Info.Extensions = new Dictionary<string, IOpenApiExtension>
            {
                { "x-foo", new JsonNodeExtension(jsonNode) }
            };

            var validator = new OpenApiValidator(ruleset);
            var walker = new OpenApiWalker(validator);
            walker.Walk(openApiDocument);

            Assert.Equivalent(
                   new List<OpenApiError>
                   {
                       new OpenApiValidatorError("FooExtensionRule", "#/info/x-foo", "Don't say hey")
                   }, validator.Errors);
        }

        [Fact]
        public void RemoveRuleByName_Invalid()
        {
            Assert.Throws<ArgumentNullException>(() => new ValidationRule<JsonNodeExtension>(null, (vc, oaa) => { }));
            Assert.Throws<ArgumentNullException>(() => new ValidationRule<JsonNodeExtension>(string.Empty, (vc, oaa) => { }));
        }

        [Fact]
        public void RemoveRuleByName()
        {
            var ruleset = ValidationRuleSet.GetDefaultRuleSet();
            int expected = ruleset.Rules.Count - 1;
            ruleset.Remove("KeyMustBeRegularExpression");

            Assert.Equal(expected, ruleset.Rules.Count);
            
            ruleset.Remove("KeyMustBeRegularExpression");
            ruleset.Remove("UnknownName");

            Assert.Equal(expected, ruleset.Rules.Count);
        }

        [Fact]
        public void RemoveRuleByType()
        {
            var ruleset = ValidationRuleSet.GetDefaultRuleSet();
            int expected = ruleset.Rules.Count - 1;
            
            ruleset.Remove(typeof(OpenApiComponents));

            Assert.Equal(expected, ruleset.Rules.Count);

            ruleset.Remove(typeof(OpenApiComponents));
            ruleset.Remove(typeof(int));

            Assert.Equal(expected, ruleset.Rules.Count);
        }
    }

    internal class FooExtension : IOpenApiExtension, IOpenApiElement
    {
        public string Baz { get; set; }

        public string Bar { get; set; }

        public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            writer.WriteStartObject();
            writer.WriteProperty("baz", Baz);
            writer.WriteProperty("bar", Bar);
            writer.WriteEndObject();
        }
    }
}
