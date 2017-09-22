using SharpYaml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavis.OpenApi
{
    public class JsonPointer
    {
        private readonly string[] _Tokens;

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

        public YamlNode Find(YamlNode sample)
        {
            if (_Tokens.Length == 0)
            {
                return sample;
            }
            try
            {
                var pointer = sample;
                foreach (var token in _Tokens)
                {
                    var sequence = pointer as YamlSequenceNode;
                    if (sequence != null)
                    {
                        pointer = sequence.Children[Convert.ToInt32(token)];
                    }
                    else
                    {
                        var map = pointer as YamlMappingNode;
                        if (map != null)
                        {
                            if (!map.Children.TryGetValue(new YamlScalarNode(token), out pointer)) {
                                return null;
                            }
                        }
                    }
                }
                return pointer;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Failed to dereference pointer", ex);
            }
        }

        public override string ToString()
        {
            return "/" + String.Join("/", _Tokens);
        }
    }
}
