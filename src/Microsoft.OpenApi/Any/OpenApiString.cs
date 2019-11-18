// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

namespace Microsoft.OpenApi.Any
{
    /// <summary>
    /// Open API string type.
    /// </summary>
    public class OpenApiString : OpenApiPrimitive<string>
    {
        private bool isExplicit;

        /// <summary>
        /// Initializes the <see cref="OpenApiString"/> class.
        /// </summary>
        /// <param name="value"></param>
        public OpenApiString(string value, bool isExplicit = false)
            : base(value)
        {
            this.isExplicit = isExplicit;
        }

        /// <summary>
        /// The primitive class this object represents.
        /// </summary>
        public override PrimitiveType PrimitiveType { get; } = PrimitiveType.String;

        /// <summary>
        /// True if string was specified explicitly by the means of double quotes, single quotes, or literal or folded style.
        /// </summary>
        public bool IsExplicit()
        {
            return this.isExplicit;
        }
    }
}