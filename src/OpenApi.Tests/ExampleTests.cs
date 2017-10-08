using Microsoft.OpenApi;
using Microsoft.OpenApi.Writers;
using SharpYaml.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace OpenApi.Tests
{
    public class ExampleTests
    {

        [Fact]
        public void WriteResponseExample()
        {
            var doc = new OpenApiDocument();
            var op = new Operation();

            var response = new Response
            {
                Content = new Dictionary<string, MediaType>() {
                    { "application/json", new MediaType()
                            {
                                Example = "{ \"foo\": \"bar\" }"
                            }
                    }
                }
            };

            op.Responses.Add("200", response);

            var pathItem = new PathItem();
            pathItem.AddOperation(OperationType.Get, op);

            doc.Paths.Add("/test", pathItem);

            var stream = new MemoryStream();
            doc.Save(stream);
            stream.Position = 0;

            var yamlStream = new YamlStream();
            yamlStream.Load(new StreamReader(stream));

            var yamlDocument = yamlStream.Documents.First();

        }

        

    }
}
