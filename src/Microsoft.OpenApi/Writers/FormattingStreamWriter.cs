using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OpenApi.Writers
{
    public class FormattingStreamWriter : StreamWriter
    {
        public FormattingStreamWriter(Stream stream, IFormatProvider formatProvider)
            : base(stream)
        {
            this.FormatProvider = formatProvider;
        }

        public override IFormatProvider FormatProvider { get; }
    }
}
