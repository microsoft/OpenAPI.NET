// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using SharpYaml.Serialization;
using System;
using System.IO;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.Tests
{
    public class ExampleTests
    {
        [Fact( Skip = "RootNode and GetScalarValue below are from Readers project. " +
            "Either move them to the Models and remove their reference from the test here.")] 
        public void WriteResponseExample()
        {
            var doc = new OpenApiDocument
            {
                Info = new OpenApiInfo()
                {
                    Title = "test",
                    Version = "1.0"
                }
            };

            doc.AddPathItem("/test",
                p => p.AddOperation(OperationType.Get,
                  o => o.AddResponse("200", r =>
                 {
                     r.Description = "foo";
                     r.AddMediaType("application/json", c =>
                     {
                         c.Example = new OpenApiString("xyz"); ///"{ \"foo\": \"bar\" }"; This doesn't work because parser treats it as a node
                     });
                })));

            var stream = new MemoryStream();
            doc.SerializeAsJson(stream);
            stream.Position = 0;

            var yamlStream = new YamlStream();
            yamlStream.Load(new StreamReader(stream));
            
            // I am commenting this out since RootNode and GetScalarValue should not be called here
            // It breaks the encapsulation layer. Readers should depend on the model,
            // but not the other way around.
            //var yamlDocument = yamlStream.Documents.First();
            //var rootNode = new RootNode(new ParsingContext(), yamlDocument);

            //var node = rootNode.Find(new JsonPointer("/paths/~1test/get/responses/200/content/application~1json/example"));
            //string example = node.GetScalarValue();

            // Assert.Equal("xyz", example);
        }
    }

    public static class YamlExtensions
    {
        public static YamlMappingNode AsMap(this YamlNode node)
        {
            return node as YamlMappingNode;
        }
    }
}
