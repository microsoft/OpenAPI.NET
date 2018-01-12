using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Validations;
using Microsoft.OpenApi.Validations.Rules;
using Microsoft.OpenApi.Writers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests
{
    public class TestCustomExtension
    {
        [Fact]
        public void ParseCustomExtension()
        {
            var description = @"
openapi: 3.0.0
info: 
    title: A doc with an extension
    version: 1.0.0
    x-foo: 
        bar: hey
        baz: hi!
paths: {}
";
            var ruleset = Validations.ValidationRuleSet.DefaultRuleSet;
            ruleset.Add(
             new ValidationRule<FooExtension>(
                 (context, item) =>
                 {
                     if (item.Bar == "hey")
                     {
                         context.AddError(new ValidationError(ErrorReason.Format, context.PathString, "Don't say hey"));
                     }
                 }));

                     var settings = new OpenApiReaderSettings()
            {
                ExtensionParsers = { { "x-foo", (a) => {
                        var fooNode = (OpenApiObject)a;
                        return new FooExtension() {
                              Bar = (fooNode["bar"] as OpenApiString)?.Value,
                              Baz = (fooNode["baz"] as OpenApiString)?.Value
                        };
                } } },
                RuleSet = ruleset
            };

            var reader = new OpenApiStringReader(settings);
        
            var diag = new OpenApiDiagnostic();
            var doc = reader.Read(description, out diag);

            var fooExtension = doc.Info.Extensions["x-foo"] as FooExtension;

            fooExtension.Should().NotBeNull();
            fooExtension.Bar.Should().Be("hey");
            fooExtension.Baz.Should().Be("hi!");
            var error = diag.Errors.First();
            error.Message.Should().Be("Don't say hey");
            error.Pointer.Should().Be("#/info/x-foo");
        }
    }

    public class FooExtension : IOpenApiExtension, IOpenApiElement
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
