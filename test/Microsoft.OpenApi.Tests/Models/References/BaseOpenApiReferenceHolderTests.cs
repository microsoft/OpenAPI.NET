// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models.References
{
    public class BaseOpenApiReferenceHolderTests
    {
        [Fact]
        public void DelegatedAccessDetectsCyclesForEveryComponentReferenceType()
        {
            AssertCircularAccessThrows(document =>
            {
                document.AddComponent("A", new OpenApiCallbackReference("B", document));
                document.AddComponent("B", new OpenApiCallbackReference("A", document));
                return document.Components!.Callbacks!["A"].PathItems;
            });
            AssertCircularAccessThrows(document =>
            {
                document.AddComponent("A", new OpenApiExampleReference("B", document));
                document.AddComponent("B", new OpenApiExampleReference("A", document));
                return document.Components!.Examples!["A"].Description;
            });
            AssertCircularAccessThrows(document =>
            {
                document.AddComponent("A", new OpenApiHeaderReference("B", document));
                document.AddComponent("B", new OpenApiHeaderReference("A", document));
                return document.Components!.Headers!["A"].Description;
            });
            AssertCircularAccessThrows(document =>
            {
                document.AddComponent("A", new OpenApiLinkReference("B", document));
                document.AddComponent("B", new OpenApiLinkReference("A", document));
                return document.Components!.Links!["A"].Description;
            });
            AssertCircularAccessThrows(document =>
            {
                document.Components = new OpenApiComponents
                {
                    MediaTypes = new Dictionary<string, IOpenApiMediaType>
                    {
                        ["A"] = new OpenApiMediaTypeReference("B", document),
                        ["B"] = new OpenApiMediaTypeReference("A", document)
                    }
                };
                document.RegisterComponents();
                return document.Components.MediaTypes["A"].Schema;
            });
            AssertCircularAccessThrows(document =>
            {
                document.AddComponent("A", new OpenApiParameterReference("B", document));
                document.AddComponent("B", new OpenApiParameterReference("A", document));
                return document.Components!.Parameters!["A"].Description;
            });
            AssertCircularAccessThrows(document =>
            {
                document.AddComponent("A", new OpenApiPathItemReference("B", document));
                document.AddComponent("B", new OpenApiPathItemReference("A", document));
                return document.Components!.PathItems!["A"].Description;
            });
            AssertCircularAccessThrows(document =>
            {
                document.AddComponent("A", new OpenApiRequestBodyReference("B", document));
                document.AddComponent("B", new OpenApiRequestBodyReference("A", document));
                return document.Components!.RequestBodies!["A"].Description;
            });
            AssertCircularAccessThrows(document =>
            {
                document.AddComponent("A", new OpenApiResponseReference("B", document));
                document.AddComponent("B", new OpenApiResponseReference("A", document));
                return document.Components!.Responses!["A"].Description;
            });
            AssertCircularAccessThrows(document =>
            {
                document.AddComponent("A", new OpenApiSchemaReference("B", document));
                document.AddComponent("B", new OpenApiSchemaReference("A", document));
                return document.Components!.Schemas!["A"].Description;
            });
            AssertCircularAccessThrows(document =>
            {
                document.AddComponent("A", new OpenApiSecuritySchemeReference("B", document));
                document.AddComponent("B", new OpenApiSecuritySchemeReference("A", document));
                return document.Components!.SecuritySchemes!["A"].Description;
            });
        }

        [Fact]
        public void InliningACircularNonSchemaReferenceThrows()
        {
            // Arrange
            var document = new OpenApiDocument();
            document.AddComponent("A", new OpenApiResponseReference("B", document));
            document.AddComponent("B", new OpenApiResponseReference("A", document));
            var response = Assert.IsType<OpenApiResponseReference>(document.Components!.Responses!["A"]);
            var writer = new OpenApiJsonWriter(
                new StringWriter(),
                new OpenApiWriterSettings { InlineLocalReferences = true });

            // Act
            var exception = Assert.Throws<InvalidOperationException>(() => response.SerializeAsV31(writer));

            // Assert
            Assert.Contains("Circular reference detected while resolving reference:", exception.Message, StringComparison.Ordinal);
        }

        private static void AssertCircularAccessThrows(Func<OpenApiDocument, object> access)
        {
            var exception = Assert.Throws<InvalidOperationException>(() => access(new OpenApiDocument()));
            Assert.Contains("Circular reference detected while resolving reference:", exception.Message, StringComparison.Ordinal);
        }
    }
}
