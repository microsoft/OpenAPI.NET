//---------------------------------------------------------------------
// <copyright file="OpenApiException.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OpenApi
{
    public class OpenApiException : Exception
    {
        public string Pointer { get; set; }
        public OpenApiException(string message) : base(message)
        {

        }

    }


}
