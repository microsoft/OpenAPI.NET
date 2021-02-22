using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Readers
{
    /// <summary>
    /// 
    /// </summary>
    public class ReadResult
    {
        public OpenApiDocument OpenApiDocument { set; get; }
        public OpenApiDiagnostic OpenApiDiagnostic { set; get; }
    }
}
