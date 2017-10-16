//---------------------------------------------------------------------
// <copyright file="OpenApiWalker.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OpenApi
{
    public class OpenApiWalker
    {
        OpenApiVisitorBase visitor;

        public OpenApiWalker(OpenApiVisitorBase visitor)
        {
            this.visitor = visitor;
        }
        public void Walk(OpenApiDocument doc)
        {
            this.visitor.Visit(doc);
            this.visitor.Visit(doc.Info);
            this.visitor.Visit(doc.Servers);
            foreach (var server in doc.Servers)
            {
                this.visitor.Visit(server);
                foreach (var variable in server.Variables.Values)
                {
                    this.visitor.Visit(variable);
                }
            }

            this.visitor.Visit(doc.Paths);
            foreach (var pathItem in doc.Paths.Values)
            {
                this.visitor.Visit(pathItem);
                this.visitor.Visit(pathItem.Operations);
                foreach (var operation in pathItem.Operations.Values)
                {
                    this.visitor.Visit(operation);
                    if (operation.Parameters != null)
                    {
                        this.visitor.Visit(operation.Parameters);
                        foreach (var parameter in operation.Parameters)
                        {
                            this.visitor.Visit(parameter);
                        }
                    }

                    if (operation.RequestBody != null)
                    {
                        this.visitor.Visit(operation.RequestBody);

                        if (operation.RequestBody.Content != null)
                        {
                            WalkContent(operation.RequestBody.Content);
                        }
                    }

                    if (operation.Responses != null)
                    {
                        this.visitor.Visit(operation.Responses);

                        foreach (var response in operation.Responses.Values)
                        {
                            this.visitor.Visit(response);
                            WalkContent(response.Content);

                            if (response.Links != null)
                            {
                                this.visitor.Visit(response.Links);
                                foreach (var link in response.Links.Values)
                                {
                                    this.visitor.Visit(link);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void WalkContent(Dictionary<string, OpenApiMediaType> content)
        {
            if (content == null) return;

            this.visitor.Visit(content);
            foreach (var mediaType in content.Values)
            {
                this.visitor.Visit(mediaType);
                this.visitor.Visit(mediaType.Examples);
                this.visitor.Visit(mediaType.Schema);
            }
        }
    }
}
