// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.V3;
using Microsoft.OpenApi.Validations;
using Microsoft.OpenApi.Validations.Rules;
using SharpYaml.Serialization;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiDiscriminatorTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiDiscriminator/";

        [Fact]
        public void ParseBasicDiscriminatorShouldSucceed()
        {
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "basicDiscriminator.yaml")))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));
                var yamlNode = yamlStream.Documents.First().RootNode;

                var context = new ParsingContext();
                var diagnostic = new OpenApiDiagnostic();

                var node = new MapNode(context, diagnostic, (YamlMappingNode)yamlNode);

                // Act
                var discriminator = OpenApiV3Deserializer.LoadDiscriminator(node);

                // Assert
                discriminator.ShouldBeEquivalentTo(
                    new OpenApiDiscriminator
                    {
                        PropertyName = "pet_type",
                        Mapping =
                        {
                            ["puppy"] = "#/components/schemas/Dog",
                            ["kitten"] = "Cat"
                        }
                    });
            }
        }

        [Fact]
        public void OneOfSchemasWithoutDiscriminatorPropertyShouldYieldError()
        {
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "invalidOneOfDiscriminator.yaml")))
            {
                var diagnostic = new OpenApiDiagnostic();
                var reader = new OpenApiStreamReader();
                var doc = reader.Read(stream, out diagnostic);
                diagnostic.Errors.ShouldAllBeEquivalentTo(new List<OpenApiError>
                {
                    new OpenApiValidatorError(nameof(OpenApiSchemaRules.ValidateOneOfDiscriminator),
                        "#/paths/~1pets~1{id}/get/responses/200/content/application~1json/schema/oneOf",
                        string.Format(SRResource.Validation_SchemaDoesntContainDiscriminatorPropertyInRequiredFieldList, "pet1", "petType")),

                    new OpenApiValidatorError(nameof(OpenApiSchemaRules.ValidateOneOfDiscriminator),
                        "#/paths/~1pets~1{id}/get/responses/200/content/application~1json/schema/oneOf",
                        string.Format(SRResource.Validation_SchemaDoesntContainDiscriminatorProperty, "pet2", "petType")),

                    new OpenApiValidatorError(nameof(OpenApiSchemaRules.ValidateOneOfDiscriminator), 
                        "#/paths/~1pets~1{id}/get/responses/200/content/application~1json/schema/oneOf",
                        string.Format(SRResource.Validation_SchemaDoesntContainDiscriminatorPropertyInRequiredFieldList, "pet2", "petType"))
                });
            }
        }

        [Fact]
        public void AnyOfSchemasWithoutDiscriminatorPropertyShouldYieldError()
        {
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "invalidAnyOfDiscriminator.yaml")))
            {
                var diagnostic = new OpenApiDiagnostic();
                var reader = new OpenApiStreamReader();
                var doc = reader.Read(stream, out diagnostic);
                diagnostic.Errors.ShouldAllBeEquivalentTo(new List<OpenApiError>
                {
                    new OpenApiValidatorError(nameof(OpenApiSchemaRules.ValidateAnyOfDiscriminator),
                        "#/paths/~1pets~1{id}/get/responses/200/content/application~1json/schema/anyOf",
                        string.Format(SRResource.Validation_SchemaDoesntContainDiscriminatorProperty, "pet2", "petType")),

                    new OpenApiValidatorError(nameof(OpenApiSchemaRules.ValidateAnyOfDiscriminator), 
                        "#/paths/~1pets~1{id}/get/responses/200/content/application~1json/schema/anyOf",
                        string.Format(SRResource.Validation_SchemaDoesntContainDiscriminatorPropertyInRequiredFieldList, "pet2", "petType"))
                });
            }
        }
    }
}