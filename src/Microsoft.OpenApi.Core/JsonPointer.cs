

namespace Microsoft.OpenApi
{
    using System;
    using System.Linq;

    public class JsonPointer
    {
        private readonly string[] _Tokens;

        public string[] Tokens { get { return _Tokens;  } }
        public JsonPointer(string pointer)
        {
            _Tokens = pointer.Split('/').Skip(1).Select(Decode).ToArray();
        }

        private JsonPointer(string[] tokens)
        {
            _Tokens = tokens;
        }
        private string Decode(string token)
        {
            return Uri.UnescapeDataString(token).Replace("~1", "/").Replace("~0", "~");
        }

        public bool IsNewPointer()
        {
            return _Tokens.Last() == "-";
        }

        public JsonPointer ParentPointer
        {
            get
            {
                if (_Tokens.Length == 0) return null;
                return new JsonPointer(_Tokens.Take(_Tokens.Length - 1).ToArray());
            }
        }

        public override string ToString()
        {
            return "/" + String.Join("/", _Tokens);
        }
    }
}
