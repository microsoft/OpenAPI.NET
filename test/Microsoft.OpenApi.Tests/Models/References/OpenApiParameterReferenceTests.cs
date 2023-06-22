using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Writers;
using VerifyXunit;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models.References
{
    public class OpenApiParameterReferenceTests
    {
        private const string OpenApi = @"
openapi: 3.0.0
info:
  title: Sample API
  version: 1.0.0
paths:
  /users:
    get:
      summary: Get users
      parameters:
        - $ref: '#/components/parameters/limitParam'
      responses:
        200:
          description: Successful operation
components:
  parameters:
    limitParam:
      name: limit
      in: query
      description: Number of results to return
      schema:
        type: integer
        minimum: 1
        maximum: 100
";
        readonly OpenApiParameterReference _parameterReference;

        public OpenApiParameterReferenceTests()
        {
            var reader = new OpenApiStringReader();
            OpenApiDocument openApiDoc = reader.Read(OpenApi, out _);
            _parameterReference = new("limitParam", openApiDoc)
            {
                Description = "Results to return"
            };
        }

        [Fact]
        public void ReferenceResolutionWorks()
        {
            // Assert
            Assert.Equal("limit", _parameterReference.Name);
            Assert.Equal("Results to return", _parameterReference.Description);
            _parameterReference.Description = "Number of limit results";
            
        }
    }
}
