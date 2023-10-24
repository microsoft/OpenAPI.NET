﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;
using Microsoft.OpenApi.Services;
using Microsoft.OpenApi.Validations;
using Microsoft.OpenApi.Validations.Rules;
using Microsoft.OpenApi.Writers;
using Xunit;

namespace Microsoft.OpenApi.Tests.Services
{
    [Collection("DefaultSettings")]
    public class OpenApiValidatorTests
    {
        [Fact]
        public void ResponseMustHaveADescription()
        {
            var openApiDocument = new OpenApiDocument();
            openApiDocument.Info = new()
            {
                Title = "foo",
                Version = "1.2.2"
            };
            openApiDocument.Paths = new();
            openApiDocument.Paths.Add(
                "/test",
                new()
                {
                    Operations =
                    {
                        [OperationType.Get] = new()
                        {
                            Responses =
                            {
                                ["200"] = new()
                            }
                        }
                    }
                });

            var validator = new OpenApiValidator(ValidationRuleSet.GetDefaultRuleSet());
            var walker = new OpenApiWalker(validator);
            walker.Walk(openApiDocument);

            validator.Errors.Should().BeEquivalentTo(
                    new List<OpenApiError>
                    {
                        new OpenApiValidatorError(nameof(OpenApiResponseRules.ResponseRequiredFields),"#/paths/~1test/get/responses/200/description",
                            String.Format(SRResource.Validation_FieldIsRequired, "description", "response"))
        });
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
                Servers = new List<OpenApiServer> {
                new()
                {
                    Url = "http://example.org"
                },
                new()
                {
                },
            },
                Paths = new()
            };

            var validator = new OpenApiValidator(ValidationRuleSet.GetDefaultRuleSet());
            var walker = new OpenApiWalker(validator);
            walker.Walk(openApiDocument);

            validator.Errors.Should().BeEquivalentTo(
                    new List<OpenApiError>
                    {
                        new OpenApiValidatorError(nameof(OpenApiServerRules.ServerRequiredFields), "#/servers/1/url",
                            String.Format(SRResource.Validation_FieldIsRequired, "url", "server"))
        });
        }

        [Fact]
        public void ValidateCustomExtension()
        {
            var ruleset = ValidationRuleSet.GetDefaultRuleSet();

            ruleset.Add(
             new ValidationRule<FooExtension>(
                 (context, item) =>
                 {
                     if (item.Bar == "hey")
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

            openApiDocument.Info.Extensions.Add("x-foo", fooExtension);

            var validator = new OpenApiValidator(ruleset);
            var walker = new OpenApiWalker(validator);
            walker.Walk(openApiDocument);

            validator.Errors.Should().BeEquivalentTo(
                   new List<OpenApiError>
                   {
                       new OpenApiValidatorError("FooExtensionRule", "#/info/x-foo", "Don't say hey")
                   });
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
