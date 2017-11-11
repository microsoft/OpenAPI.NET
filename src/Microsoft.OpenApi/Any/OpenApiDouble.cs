// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.OpenApi.Any
{
    /// <summary>
    /// Open API Double
    /// </summary>
    public class OpenApiDouble : OpenApiPrimitive<double>
    {
        /// <summary>
        /// Initializes the <see cref="OpenApiDouble"/> class.
        /// </summary>
        public OpenApiDouble(float value)
            : base(value)
        {
        }

        /// <summary>
        /// Primitive type this object represents.
        /// </summary>
        public override PrimitiveType PrimitiveType { get; } = PrimitiveType.Double;
    }
}