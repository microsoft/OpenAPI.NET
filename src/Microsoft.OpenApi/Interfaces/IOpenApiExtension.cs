using Microsoft.OpenApi.Writers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OpenApi.Interfaces
{
    /// <summary>
    /// Interface requuired for implementing any custom extension
    /// </summary>
    public interface IOpenApiExtension
    {
        /// <summary>
        /// Write out contents of custom extension
        /// </summary>
        /// <param name="writer"></param>
        void Write(IOpenApiWriter writer);
    }
}
