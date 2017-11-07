// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Xunit;
using Microsoft.OpenApi.Properties;
using System.Text;
using System.Collections.Generic;

namespace Microsoft.OpenApi.Tests.Models
{
    public class OpenApiReferenceTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CtorThrowArgumentNullOrWhiteSpaceWithNullInput(string reference)
        {
            // Arrange & Act
            Action action = () => new OpenApiReference(reference);

            // Assert
            action.ShouldThrow<ArgumentException>(String.Format(SRResource.ArgumentNullOrWhiteSpace, "reference"));
        }

        [Theory]
        [InlineData("abc#")]
        [InlineData("#abc")]
        [InlineData("#//")]
        [InlineData("#/components/name")]
        [InlineData("#/any/schema/type")]
        public void CtorThrowInvalidReferenceFormat(string reference)
        {
            // Arrange & Act
            Action action = () => new OpenApiReference(reference);

            // Assert
            action.ShouldThrow<OpenApiException>(String.Format(SRResource.ReferenceHasInvalidFormat, reference));
        }

        [Theory]
        [InlineData("Pet.json", "Pet.json", ReferenceType.Schema, null)]
        [InlineData("Pet.yaml", "Pet.yaml", ReferenceType.Schema, null)]
        [InlineData("abc", "abc", ReferenceType.Schema, null)]
        [InlineData("Pet.json#/Pet", "Pet.json", ReferenceType.Schema, "Pet")]
        [InlineData("Pet.yaml#/Pet", "Pet.yaml", ReferenceType.Schema, "Pet")]
        [InlineData("abc#/Pet", "abc", ReferenceType.Schema, "Pet")]
        [InlineData("#/components/schemas/Pet", null, ReferenceType.Schema, "Pet")]
        [InlineData("#/components/parameters/name", null, ReferenceType.Parameter, "name")]
        [InlineData("#/components/responses/200", null, ReferenceType.Response, "200")]
        public void CtorSetPropertyValue(string input, string file, ReferenceType type, string name)
        {
            // Arrange & Act
            var reference = new OpenApiReference(input);

            // Assert
            reference.ExternalFilePath.Should().Be(file);
            reference.ReferenceType.Should().Be(type);
            reference.TypeName.Should().Be(name);
        }

        public static IEnumerable<object[]> ReferernceStrings()
        {
            var values = new string[]
            {
                "Pet.json",
                 "Pet.yaml",
                 "abc",
                 "Pet.json#/Pet",
                 "Pet.yaml#/Pet",
                "abc#/Pet",
                 "#/components/schemas/Pet",
                 "#/components/parameters/name",
                 "#/components/responses/200",
            };
            foreach (var value in values)
            {
                yield return new object[] { value };
            }
        }

        [Theory]
        [MemberData(nameof(ReferernceStrings))]
        public void SerializeReferenceAsJsonV3Works(string input)
        {
            // Arrange
            var reference = new OpenApiReference(input);
            string expect = @"{
  ""$ref"": ""ABCDE""
}".Replace("ABCDE", input);

            // Act
            string actual = reference.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual.Should().Be(expect);
        }

        [Theory]
        [MemberData(nameof(ReferernceStrings))]
        public void SerializeReferenceAsYamlV3Works(string input)
        {
            // Arrange
            var reference = new OpenApiReference(input);
            string expect = @"$ref: ABCDE";
            if (input.StartsWith("#"))
            {
                expect = expect.Replace("ABCDE", "'" + input + "'");
            }
            else
            {
                expect = expect.Replace("ABCDE", input);
            }

            // Act
            string actual = reference.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual.Should().Be(expect);
        }


        [Theory]
        [MemberData(nameof(ReferernceStrings))]
        public void SerializeReferenceAsJsonV2Works(string input)
        {
            // Arrange
            var reference = new OpenApiReference(input);
            string expect = @"{
  ""$ref"": ""ABCDE""
}".Replace("ABCDE", input).Replace("components", "definitions");

            // Act
            string actual = reference.SerializeAsJson(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual.Should().Be(expect);
        }

        [Theory]
        [MemberData(nameof(ReferernceStrings))]
        public void SerializeReferenceAsYamlV2Works(string input)
        {
            // Arrange
            var reference = new OpenApiReference(input);
            string expect = @"$ref: ABCDE";
            if (input.StartsWith("#"))
            {
                expect = expect.Replace("ABCDE", "'" + input + "'");
            }
            else
            {
                expect = expect.Replace("ABCDE", input);
            }
            expect = expect.Replace("components", "definitions");

            // Act
            string actual = reference.SerializeAsYaml(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual.Should().Be(expect);
        }
    }
}
