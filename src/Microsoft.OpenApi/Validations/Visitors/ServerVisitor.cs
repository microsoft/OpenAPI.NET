// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Diagnostics;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations.Visitors
{
    /// <summary>
    /// Visit <see cref="OpenApiServer"/>.
    /// </summary>
    internal class ServerVisitor : VisitorBase<OpenApiServer>
    {
        /// <summary>
        /// Visit the children of the <see cref="OpenApiServer"/>.
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="server">The <see cref="OpenApiServer"/>.</param>
        protected override void Next(ValidationContext context, OpenApiServer server)
        {
            Debug.Assert(context != null);
            Debug.Assert(server != null);

            context.ValidateMap(server.Variables);

            // add more.
            base.Next(context, server);
        }
    }
}
