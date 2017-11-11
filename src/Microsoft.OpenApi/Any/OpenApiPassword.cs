// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.OpenApi.Any
{
    /// <summary>
    /// Open API password.
    /// </summary>
    public class OpenApiPassword : OpenApiPrimitive<string>
    {
        /// <summary>
        /// The primitive type this object represents.
        /// </summary>
        public override PrimitiveType PrimitiveType { get; } = PrimitiveType.Password;

        /// <summary>
        /// Initializes the <see cref="OpenApiPassword"/> class.
        /// </summary>
        public OpenApiPassword(string value)
            : base(value)
        { }
    }
}
