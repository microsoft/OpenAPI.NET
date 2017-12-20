//// Copyright (c) Microsoft Corporation. All rights reserved.
//// Licensed under the MIT license.

//using System;
//using Microsoft.OpenApi.Exceptions;
//using Microsoft.OpenApi.Models;
//using Xunit;

//namespace Microsoft.OpenApi.Validations.Visitors.Tests
//{
//    public class OpenApiVisitorSetTests
//    {
//        [Fact]
//        public void VisitorsPropertyReturnsTheCorrectVisitorList()
//        {
//            for (int i = 0; i < 5; i++) // 5 is just a magic number
//            {
//                // Arrange & Act
//                var visitors = OpenApiVisitorSet.Visitors;

//                // Assert
//                Assert.NotNull(visitors);
//                Assert.NotEmpty(visitors);
//                Assert.Equal(29, visitors.Count); // Now, we have 29 DOM classes.
//            }
//        }

//        [Fact]
//        public void GetVisitorThrowsUnknowElementType()
//        {
//            // Arrange & Act
//            Action test = () => OpenApiVisitorSet.GetVisitor(typeof(OpenApiVisitorSetTests));

//            // Assert
//            var exception = Assert.Throws<OpenApiException>(test);
//            Assert.Equal("Can not find visitor type registered for type 'Microsoft.OpenApi.Validations.Visitors.Tests.OpenApiVisitorSetTests'.",
//                exception.Message);
//        }

//        [Theory]
//        [InlineData(typeof(OpenApiCallback), typeof(CallbackVisitor))]
//        [InlineData(typeof(OpenApiComponents), typeof(ComponentsVisitor))]
//        [InlineData(typeof(OpenApiContact), typeof(ContactVisitor))]
//        [InlineData(typeof(OpenApiDiscriminator), typeof(DiscriminatorVisitor))]
//        [InlineData(typeof(OpenApiDocument), typeof(DocumentVisitor))]
//        [InlineData(typeof(OpenApiEncoding), typeof(EncodingVisitor))]
//        [InlineData(typeof(OpenApiExample), typeof(ExampleVisitor))]
//        [InlineData(typeof(OpenApiExternalDocs), typeof(ExternalDocsVisitor))]
//        [InlineData(typeof(OpenApiHeader), typeof(HeaderVisitor))]
//        [InlineData(typeof(OpenApiInfo), typeof(InfoVisitor))]
//        [InlineData(typeof(OpenApiLicense), typeof(LicenseVisitor))]
//        [InlineData(typeof(OpenApiLink), typeof(LinkVisitor))]
//        [InlineData(typeof(OpenApiMediaType), typeof(MediaTypeVisitor))]
//        [InlineData(typeof(OpenApiOAuthFlows), typeof(OAuthFlowsVisitor))]
//        [InlineData(typeof(OpenApiOAuthFlow), typeof(OAuthFlowVisitor))]
//        [InlineData(typeof(OpenApiOperation), typeof(OperationVisitor))]
//        [InlineData(typeof(OpenApiParameter), typeof(ParameterVisitor))]
//        [InlineData(typeof(OpenApiPathItem), typeof(PathItemVisitor))]
//        [InlineData(typeof(OpenApiPaths), typeof(PathsVisitor))]
//        [InlineData(typeof(OpenApiRequestBody), typeof(RequestBodyVisitor))]
//        [InlineData(typeof(OpenApiResponses), typeof(ResponsesVisitor))]
//        [InlineData(typeof(OpenApiResponse), typeof(ResponseVisitor))]
//        [InlineData(typeof(OpenApiSchema), typeof(SchemaVisitor))]
//        [InlineData(typeof(OpenApiSecurityRequirement), typeof(SecurityRequirementVisitor))]
//        [InlineData(typeof(OpenApiSecurityScheme), typeof(SecuritySchemeVisitor))]
//        [InlineData(typeof(OpenApiServerVariable), typeof(ServerVariableVisitor))]
//        [InlineData(typeof(OpenApiServer), typeof(ServerVisitor))]
//        [InlineData(typeof(OpenApiTag), typeof(TagVisitor))]
//        [InlineData(typeof(OpenApiXml), typeof(XmlVisitor))]
//        public void GetVisitorReturnsTheCorrectVisitor(Type elementType, Type visitorType)
//        {
//            // Arrange & Act
//            var visitor = OpenApiVisitorSet.GetVisitor(elementType);

//            // Assert
//            Assert.NotNull(visitor);
//            Assert.Same(visitorType, visitor.GetType());
//        }
//    }
//}
