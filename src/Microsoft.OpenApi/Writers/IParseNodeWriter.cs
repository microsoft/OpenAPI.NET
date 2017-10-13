﻿using System;

namespace Microsoft.OpenApi.Writers
{
    public interface IParseNodeWriter
    {
        void WriteStartDocument();
        void WriteEndDocument();
        void WriteStartList();
        void WriteEndList();
        void WriteListItem<T>(T item, Action<IParseNodeWriter, T> parser);

        void WriteStartMap();
        void WriteEndMap();

        void WritePropertyName(string name);

        void WriteValue(string value);

        void WriteValue(Decimal value);

        void WriteValue(int value);

        void WriteValue(bool value);
        void WriteNull();

        void WriteRaw(string value);


        void Flush();
    }
}
