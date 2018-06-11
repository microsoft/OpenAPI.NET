using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Exceptions;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V2Tests
{
    public class OpenApiDocumentTests
    {
        [Fact]
        public void ShouldThrowWhenReferenceTypeIsInvalid()
        {
            var input = @"
swagger: 2.0
info: 
  title: test
  version: 1.0.0
paths: 
  '/':
    get:
      responses:
        '200':
          description: ok
          schema:
            $ref: '#/definition/does/notexist'
";

            var reader = new OpenApiStringReader(new OpenApiReaderSettings() {
            });

            OpenApiDocument doc = null;

            Assert.Throws<OpenApiReaderException>(() =>
            doc = reader.Read(input, out var diagnostic));

            Assert.Null(doc);
        }


        [Fact]
        public void ShouldThrowWhenReferenceDoesNotExist()
        {
            var input = @"
swagger: 2.0
info: 
  title: test
  version: 1.0.0
paths: 
  '/':
    get:
      produces: ['application/json']
      responses:
        '200':
          description: ok
          schema:
            $ref: '#/definitions/doesnotexist'
";

            var reader = new OpenApiStringReader(new OpenApiReaderSettings()
            {
            });

            OpenApiDocument doc = null;

            doc = reader.Read(input, out var diagnostic);

            diagnostic.Errors.ShouldBeEquivalentTo(new List<OpenApiError> {
                new OpenApiError( new OpenApiException("Invalid Reference identifier 'doesnotexist'.")) });
        }
    }
}