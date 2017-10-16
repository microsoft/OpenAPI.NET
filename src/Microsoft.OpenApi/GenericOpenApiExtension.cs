//---------------------------------------------------------------------
// <copyright file="GenericOpenApiExtension.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OpenApi
{
    public class GenericOpenApiExtension : IOpenApiExtension
    {
        string node;
        public GenericOpenApiExtension(string n)
        {
            this.node = n;
        }
    }
}
