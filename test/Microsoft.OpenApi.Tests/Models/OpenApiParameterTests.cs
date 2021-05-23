// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiParameterTests
    {
        public static OpenApiParameter BasicParameter = new OpenApiParameter
        {
            Name = "name1",
            In = ParameterLocation.Path
        };

        public static OpenApiParameter ReferencedParameter = new OpenApiParameter
        {
            Name = "name1",
            In = ParameterLocation.Path,
            Reference = new OpenApiReference
            {
                Type = ReferenceType.Parameter,
                Id = "example1"
            }
        };

        public static OpenApiParameter AdvancedPathParameterWithSchema = new OpenApiParameter
        {
            Name = "name1",
            In = ParameterLocation.Path,
            Description = "description1",
            Required = true,
            Deprecated = false,

            Style = ParameterStyle.Simple,
            Explode = true,
            Schema = new OpenApiSchema
            {
                Title = "title2",
                Description = "description2"
            },
            Examples = new Dictionary<string, OpenApiExample>
            {
                ["test"] = new OpenApiExample
                {
                    Summary = "summary3",
                    Description = "description3"
                }
            }
        };

        public static OpenApiParameter ParameterWithFormStyleAndExplodeFalse = new OpenApiParameter
        {
            Name = "name1",
            In = ParameterLocation.Query,
            Description = "description1",
            Style = ParameterStyle.Form,
            Explode = false,
            Schema = new OpenApiSchema
            {
                Type = "array",
                Items = new OpenApiSchema
                {
                    Enum = new List<IOpenApiAny>
                    {
                        new OpenApiString("value1"),
                        new OpenApiString("value2")
                    }
                }
            }

        };

        public static OpenApiParameter ParameterWithFormStyleAndExplodeTrue = new OpenApiParameter
        {
            Name = "name1",
            In = ParameterLocation.Query,
            Description = "description1",
            Style = ParameterStyle.Form,
            Explode = true,
            Schema = new OpenApiSchema
            {
                Type = "array",
                Items = new OpenApiSchema
                {
                    Enum = new List<IOpenApiAny>
                    {
                        new OpenApiString("value1"),
                        new OpenApiString("value2")
                    }
                }
            }

        };

        public static OpenApiParameter AdvancedHeaderParameterWithSchemaReference = new OpenApiParameter
        {
            Name = "name1",
            In = ParameterLocation.Header,
            Description = "description1",
            Required = true,
            Deprecated = false,

            Style = ParameterStyle.Simple,
            Explode = true,
            Schema = new OpenApiSchema
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.Schema,
                    Id = "schemaObject1"
                },
                UnresolvedReference = true
            },
            Examples = new Dictionary<string, OpenApiExample>
            {
                ["test"] = new OpenApiExample
                {
                    Summary = "summary3",
                    Description = "description3"
                }
            }
        };

        public static OpenApiParameter AdvancedHeaderParameterWithSchemaTypeObject = new OpenApiParameter
        {
            Name = "name1",
            In = ParameterLocation.Header,
            Description = "description1",
            Required = true,
            Deprecated = false,

            Style = ParameterStyle.Simple,
            Explode = true,
            Schema = new OpenApiSchema
            {
                Type = "object"
            },
            Examples = new Dictionary<string, OpenApiExample>
            {
                ["test"] = new OpenApiExample
                {
                    Summary = "summary3",
                    Description = "description3"
                }
            }
        };

        private readonly ITestOutputHelper _output;

        public OpenApiParameterTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(ParameterStyle.Form, true)]
        [InlineData(ParameterStyle.SpaceDelimited, false)]
        [InlineData(null, false)]
        public void WhenStyleIsFormTheDefaultValueOfExplodeShouldBeTrueOtherwiseFalse(ParameterStyle? style, bool expectedExplode)
        {
            // Arrange
            var parameter = new OpenApiParameter
            {
                Name = "name1",
                In = ParameterLocation.Query,
                Style = style
            };

            // Act & Assert
            parameter.Explode.Should().Be(expectedExplode);
        }

        [Fact]
        public void SerializeBasicParameterAsV3JsonWorks()
        {
            // Arrange
            var expected = @"{
  ""name"": ""name1"",
  ""in"": ""path""
}";

            // Act
            var actual = BasicParameter.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedParameterAsV3JsonWorks()
        {
            // Arrange
            var expected = @"{
  ""name"": ""name1"",
  ""in"": ""path"",
  ""description"": ""description1"",
  ""required"": true,
  ""style"": ""simple"",
  ""explode"": true,
  ""schema"": {
    ""title"": ""title2"",
    ""description"": ""description2""
  },
  ""examples"": {
    ""test"": {
      ""summary"": ""summary3"",
      ""description"": ""description3""
    }
  }
}";

            // Act
            var actual = AdvancedPathParameterWithSchema.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeReferencedParameterAsV3JsonWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
  ""$ref"": ""#/components/parameters/example1""
}";

            // Act
            ReferencedParameter.SerializeAsV3(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeReferencedParameterAsV3JsonWithoutReferenceWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
  ""name"": ""name1"",
  ""in"": ""path""
}";

            // Act
            ReferencedParameter.SerializeAsV3WithoutReference(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeReferencedParameterAsV2JsonWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
  ""$ref"": ""#/parameters/example1""
}";

            // Act
            ReferencedParameter.SerializeAsV2(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeReferencedParameterAsV2JsonWithoutReferenceWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
  ""in"": ""path"",
  ""name"": ""name1""
}";

            // Act
            ReferencedParameter.SerializeAsV2WithoutReference(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeParameterWithSchemaReferenceAsV2JsonWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
  ""in"": ""header"",
  ""name"": ""name1"",
  ""description"": ""description1"",
  ""required"": true,
  ""type"": ""string""
}";

            // Act
            AdvancedHeaderParameterWithSchemaReference.SerializeAsV2(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeParameterWithSchemaTypeObjectAsV2JsonWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
  ""in"": ""header"",
  ""name"": ""name1"",
  ""description"": ""description1"",
  ""required"": true,
  ""type"": ""string""
}";

            // Act
            AdvancedHeaderParameterWithSchemaTypeObject.SerializeAsV2(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeParameterWithFormStyleAndExplodeFalseWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
  ""name"": ""name1"",
  ""in"": ""query"",
  ""description"": ""description1"",
  ""style"": ""form"",
  ""explode"": false,
  ""schema"": {
    ""type"": ""array"",
    ""items"": {
      ""enum"": [
        ""value1"",
        ""value2""
      ]
    }
  }
}";

            // Act
            ParameterWithFormStyleAndExplodeFalse.SerializeAsV3WithoutReference(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeParameterWithFormStyleAndExplodeTrueWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
  ""name"": ""name1"",
  ""in"": ""query"",
  ""description"": ""description1"",
  ""style"": ""form"",
  ""schema"": {
    ""type"": ""array"",
    ""items"": {
      ""enum"": [
        ""value1"",
        ""value2""
      ]
    }
  }
}";

            // Act
            ParameterWithFormStyleAndExplodeTrue.SerializeAsV3WithoutReference(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }
    }
}
