// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;
using Microsoft.OpenApi.Services;
using Microsoft.OpenApi.Validations;
using Microsoft.OpenApi.Writers;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Tests.Services
{
    [Collection("DefaultSettings")]
    public class OpenApiValidatorTests
    {
        private readonly ITestOutputHelper _output;

        public OpenApiValidatorTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ResponseMustHaveADescription()
        {
            var openApiDocument = new OpenApiDocument();
            openApiDocument.Info = new OpenApiInfo()
            {
                Title = "foo",
                Version = "1.2.2"
            };
            openApiDocument.Paths.Add(
                "/test",
                new OpenApiPathItem
                {
                    Operations =
                    {
                        [OperationType.Get] = new OpenApiOperation
                        {
                            Responses =
                            {
                                ["200"] = new OpenApiResponse()
                            }
                        }
                    }
                });

            var validator = new OpenApiValidator();
            var walker = new OpenApiWalker(validator);
            walker.Walk(openApiDocument);

            validator.Errors.ShouldBeEquivalentTo(
                    new List<OpenApiError>
                    {
                        new OpenApiError(ErrorReason.Required, "#/paths/~1test/get/responses/200/description",
                            String.Format(SRResource.Validation_FieldIsRequired, "description", "response"))
        });
        }

        [Fact]
        public void ServersShouldBeReferencedByIndex()
        {
            var openApiDocument = new OpenApiDocument();
            openApiDocument.Info = new OpenApiInfo()
            {
                Title = "foo",
                Version = "1.2.2"
            };
            openApiDocument.Servers = new List<OpenApiServer> {
                new OpenApiServer
                {
                    Url = "http://example.org"
                },
                new OpenApiServer
                {

                }
            };
            
            var validator = new OpenApiValidator();
            var walker = new OpenApiWalker(validator);
            walker.Walk(openApiDocument);

            validator.Errors.ShouldBeEquivalentTo(
                    new List<OpenApiError>
                    {
                        new OpenApiError(ErrorReason.Required, "#/servers/1/url",
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
                         context.AddError(new OpenApiError(ErrorReason.Format, context.PathString, "Don't say hey"));
                     }
                 }));

            var openApiDocument = new OpenApiDocument();
            openApiDocument.Info = new OpenApiInfo()
            {
                Title = "foo",
                Version = "1.2.2"
            };

            var fooExtension = new FooExtension()
            {
                Bar = "hey",
                Baz = "baz"
            };

            openApiDocument.Info.Extensions.Add("x-foo",fooExtension);

            var validator = new OpenApiValidator(ruleset);
            var walker = new OpenApiWalker(validator);
            walker.Walk(openApiDocument);

            validator.Errors.ShouldBeEquivalentTo(
                   new List<OpenApiError>
                   {
                       new OpenApiError(ErrorReason.Format, "#/info/x-foo", "Don't say hey")
                   });
        }

    }

    internal class FooExtension : IOpenApiExtension, IOpenApiElement
    {
        public string Baz { get; set; }

        public string Bar { get; set; }

        public void Write(IOpenApiWriter writer)
        {
            writer.WriteStartObject();
            writer.WriteProperty("baz", Baz);
            writer.WriteProperty("bar", Bar);
            writer.WriteEndObject();
        }
    }
}