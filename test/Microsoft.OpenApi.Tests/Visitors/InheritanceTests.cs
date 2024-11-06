using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Services;
using Xunit;

namespace Microsoft.OpenApi.Tests.Visitors
{
    public class InheritanceTests
    {
        [Fact]
        public void ExpectedVirtualsInvolved()
        {
            OpenApiVisitorBase visitor = null;

            visitor = new TestVisitor();

            visitor.Enter(default(string));
            visitor.Visit(default(OpenApiDocument));
            visitor.Visit(default(OpenApiInfo));
            visitor.Visit(default(OpenApiContact));
            visitor.Visit(default(OpenApiLicense));
            visitor.Visit(default(IList<OpenApiServer>));
            visitor.Visit(default(OpenApiServer));
            visitor.Visit(default(OpenApiPaths));
            visitor.Visit(default(OpenApiPathItem));
            visitor.Visit(default(OpenApiServerVariable));
            visitor.Visit(default(IDictionary<OperationType, OpenApiOperation>));
            visitor.Visit(default(OpenApiOperation));
            visitor.Visit(default(IList<OpenApiParameter>));
            visitor.Visit(default(OpenApiParameter));
            visitor.Visit(default(OpenApiRequestBody));
            visitor.Visit(default(IDictionary<string, OpenApiHeader>));
            visitor.Visit(default(IDictionary<string, OpenApiCallback>));
            visitor.Visit(default(OpenApiResponse));
            visitor.Visit(default(OpenApiResponses));
            visitor.Visit(default(IDictionary<string, OpenApiMediaType>));
            visitor.Visit(default(OpenApiMediaType));
            visitor.Visit(default(OpenApiEncoding));
            visitor.Visit(default(IDictionary<string, OpenApiExample>));
            visitor.Visit(default(OpenApiComponents));
            visitor.Visit(default(OpenApiExternalDocs));
            visitor.Visit(default(OpenApiSchema));
            visitor.Visit(default(IDictionary<string, OpenApiLink>));
            visitor.Visit(default(OpenApiLink));
            visitor.Visit(default(OpenApiCallback));
            visitor.Visit(default(OpenApiTag));
            visitor.Visit(default(OpenApiHeader));
            visitor.Visit(default(OpenApiOAuthFlow));
            visitor.Visit(default(OpenApiSecurityRequirement));
            visitor.Visit(default(OpenApiSecurityScheme));
            visitor.Visit(default(OpenApiExample));
            visitor.Visit(default(IList<OpenApiTag>));
            visitor.Visit(default(IList<OpenApiSecurityRequirement>));
            visitor.Visit(default(IOpenApiExtensible));
            visitor.Visit(default(IOpenApiExtension));
            visitor.Visit(default(IList<OpenApiExample>));
            visitor.Visit(default(IDictionary<string, OpenApiServerVariable>));
            visitor.Visit(default(IDictionary<string, OpenApiEncoding>));
            visitor.Visit(default(IOpenApiReferenceable));
            visitor.Exit();
            Assert.True(42 < ((TestVisitor)visitor).CallStack.Count());
        }

        internal protected class TestVisitor : OpenApiVisitorBase
        {
            public Stack<string> CallStack { get; } = new Stack<string>();

            private string EncodeCall([CallerMemberName] string name = "", [CallerLineNumber] int lineNumber = 0)
            {
                var encoding = $"{name}:{lineNumber}";
                CallStack.Push(encoding);
                return encoding;
            }

            public override void Enter(string segment)
            {
                EncodeCall();
                base.Enter(segment);
            }

            public override void Exit()
            {
                EncodeCall();
                base.Exit();
            }

            public override void Visit(OpenApiDocument doc)
            {
                EncodeCall();
                base.Visit(doc);
            }

            public override void Visit(OpenApiInfo info)
            {
                EncodeCall();
                base.Visit(info);
            }

            public override void Visit(OpenApiContact contact)
            {
                EncodeCall();
                base.Visit(contact);
            }

            public override void Visit(OpenApiLicense license)
            {
                EncodeCall();
                base.Visit(license);
            }

            public override void Visit(IList<OpenApiServer> servers)
            {
                EncodeCall();
                base.Visit(servers);
            }

            public override void Visit(OpenApiServer server)
            {
                EncodeCall();
                base.Visit(server);
            }

            public override void Visit(OpenApiPaths paths)
            {
                EncodeCall();
                base.Visit(paths);
            }

            public override void Visit(OpenApiPathItem pathItem)
            {
                EncodeCall();
                base.Visit(pathItem);
            }

            public override void Visit(OpenApiServerVariable serverVariable)
            {
                EncodeCall();
                base.Visit(serverVariable);
            }

            public override void Visit(IDictionary<OperationType, OpenApiOperation> operations)
            {
                EncodeCall();
                base.Visit(operations);
            }

            public override void Visit(OpenApiOperation operation)
            {
                EncodeCall();
                base.Visit(operation);
            }

            public override void Visit(IList<OpenApiParameter> parameters)
            {
                EncodeCall();
                base.Visit(parameters);
            }

            public override void Visit(OpenApiParameter parameter)
            {
                EncodeCall();
                base.Visit(parameter);
            }

            public override void Visit(OpenApiRequestBody requestBody)
            {
                EncodeCall();
                base.Visit(requestBody);
            }

            public override void Visit(IDictionary<string, OpenApiHeader> headers)
            {
                EncodeCall();
                base.Visit(headers);
            }

            public override void Visit(IDictionary<string, OpenApiCallback> callbacks)
            {
                EncodeCall();
                base.Visit(callbacks);
            }

            public override void Visit(OpenApiResponse response)
            {
                EncodeCall();
                base.Visit(response);
            }

            public override void Visit(OpenApiResponses response)
            {
                EncodeCall();
                base.Visit(response);
            }

            public override void Visit(IDictionary<string, OpenApiMediaType> content)
            {
                EncodeCall();
                base.Visit(content);
            }

            public override void Visit(OpenApiMediaType mediaType)
            {
                EncodeCall();
                base.Visit(mediaType);
            }

            public override void Visit(OpenApiEncoding encoding)
            {
                EncodeCall();
                base.Visit(encoding);
            }

            public override void Visit(IDictionary<string, OpenApiExample> examples)
            {
                EncodeCall();
                base.Visit(examples);
            }

            public override void Visit(OpenApiComponents components)
            {
                EncodeCall();
                base.Visit(components);
            }

            public override void Visit(OpenApiExternalDocs externalDocs)
            {
                EncodeCall();
                base.Visit(externalDocs);
            }

            public override void Visit(OpenApiSchema schema)
            {
                EncodeCall();
                base.Visit(schema);
            }

            public override void Visit(IDictionary<string, OpenApiLink> links)
            {
                EncodeCall();
                base.Visit(links);
            }

            public override void Visit(OpenApiLink link)
            {
                EncodeCall();
                base.Visit(link);
            }

            public override void Visit(OpenApiCallback callback)
            {
                EncodeCall();
                base.Visit(callback);
            }

            public override void Visit(OpenApiTag tag)
            {
                EncodeCall();
                base.Visit(tag);
            }

            public override void Visit(OpenApiHeader header)
            {
                EncodeCall();
                base.Visit(header);
            }

            public override void Visit(OpenApiOAuthFlow openApiOAuthFlow)
            {
                EncodeCall();
                base.Visit(openApiOAuthFlow);
            }

            public override void Visit(OpenApiSecurityRequirement securityRequirement)
            {
                EncodeCall();
                base.Visit(securityRequirement);
            }

            public override void Visit(OpenApiSecurityScheme securityScheme)
            {
                EncodeCall();
                base.Visit(securityScheme);
            }

            public override void Visit(OpenApiExample example)
            {
                EncodeCall();
                base.Visit(example);
            }

            public override void Visit(IList<OpenApiTag> openApiTags)
            {
                EncodeCall();
                base.Visit(openApiTags);
            }

            public override void Visit(IList<OpenApiSecurityRequirement> openApiSecurityRequirements)
            {
                EncodeCall();
                base.Visit(openApiSecurityRequirements);
            }

            public override void Visit(IOpenApiExtensible openApiExtensible)
            {
                EncodeCall();
                base.Visit(openApiExtensible);
            }

            public override void Visit(IOpenApiExtension openApiExtension)
            {
                EncodeCall();
                base.Visit(openApiExtension);
            }

            public override void Visit(IList<OpenApiExample> example)
            {
                EncodeCall();
                base.Visit(example);
            }

            public override void Visit(IDictionary<string, OpenApiServerVariable> serverVariables)
            {
                EncodeCall();
                base.Visit(serverVariables);
            }

            public override void Visit(IDictionary<string, OpenApiEncoding> encodings)
            {
                EncodeCall();
                base.Visit(encodings);
            }

            public override void Visit(IOpenApiReferenceable referenceable)
            {
                EncodeCall();
                base.Visit(referenceable);
            }
        }
    }
}
