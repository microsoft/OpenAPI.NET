using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OpenApi.Readers.Interface
{
    public interface IStreamLoader
    {
        Task<Stream> LoadAsync(Uri uri);
    }
}
