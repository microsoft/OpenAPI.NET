using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OpenApi.Any
{
    /// <summary>
    /// Open API TimeSpan
    /// </summary>
    public class OpenApiTimeSpan : OpenApiPrimitive<TimeSpan>
    {
        /// <summary>
        /// Initializes the <see cref="OpenApiTimeSpan"/> class.
        /// </summary>
        public OpenApiTimeSpan(TimeSpan value)
            : base(value)
        {
        }

        /// <summary>
        /// Primitive type this object represents.
        /// </summary>
        public override PrimitiveType PrimitiveType { get; } = PrimitiveType.TimeSpan;
    }
}
