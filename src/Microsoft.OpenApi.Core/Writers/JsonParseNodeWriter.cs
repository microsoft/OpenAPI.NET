using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.OpenApi.Writers
{
    public class JsonParseNodeWriter : IParseNodeWriter
    {
        enum ParseState
        {
            Initial,
            InList,
            InMap
        };

        Stack<ParseState> state = new Stack<ParseState>();
        StreamWriter writer;

        public JsonParseNodeWriter(Stream stream)
        {
            this.writer = new StreamWriter(stream)
            {
                NewLine = "\n"
            };
        }

        public void Flush()
        {
            this.writer.Flush();
        }

        string Indent = "";
        bool first = true;
        void IncreaseIndent()
        {
            Indent += "  ";
        }
        void DecreaseIndent()
        {
            Indent = Indent.Substring(0,Indent.Length -2 );
        }
        public void WriteStartDocument() { }
        public void WriteEndDocument() { }
        public void WriteStartList() {
            writer.WriteLine(" [");
            state.Push(ParseState.InList);
            IncreaseIndent();
            first = true;
        }
        public void WriteEndList() {
            writer.WriteLine();
            writer.Write(Indent + "]");
            state.Pop();
            DecreaseIndent();
        }

        public void WriteListItem<T>(T item, Action<IParseNodeWriter, T> parser)
        {
            if (!first)
            {
                writer.Write(",");
                writer.WriteLine();
                writer.Write(Indent);
            }
            else
            {
                writer.Write(Indent);
                first = false;
            }

            if (item != null)
                parser(this, item);
            else
                WriteNull();
        }

        public void WriteStartMap() {
            writer.WriteLine(Indent + "{");
            state.Push(ParseState.InMap);
            IncreaseIndent();
            first = true;
        }
        public void WriteEndMap() {
            writer.WriteLine();
            writer.Write(Indent + "}");
            state.Pop();
            DecreaseIndent();
            first = false;
        }

        public void WritePropertyName(string name) {
            if (!first)
            {
                writer.WriteLine(",");
            } else
            {
                first = false;
            }
            writer.Write(Indent + "\"" + name + "\": " );
        }

        public void WriteValue(string value) {
            value = value.Replace("\n", "\\n");
            writer.Write("\"" + value + "\"");
        }

        public void WriteValue(Decimal value) {
            writer.WriteLine(value.ToString());  //TODO deal with culture issues
        }

        public void WriteValue(int value) {
            writer.Write(value.ToString());  //TODO deal with culture issues
       }

        public void WriteValue(bool value) {
            writer.Write(value.ToString().ToLower());  //TODO deal with culture issues
        }

        public void WriteRaw(string value)
        {
            writer.Write(value);  
        }

        public void WriteNull()
        {
            writer.WriteLine("null");
        }
    }
}
