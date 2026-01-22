// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Generic dictionary type for Open API dictionary element.
    /// </summary>
    /// <typeparam name="T">The Open API element, <see cref="IOpenApiElement"/></typeparam>
    public abstract class OpenApiExtensibleDictionary<T> : Dictionary<string, T>,
        IOpenApiSerializable,
        IOpenApiExtensible
        where T : IOpenApiSerializable
    {
        /// <summary>
        /// Parameterless constructor
        /// </summary>
        protected OpenApiExtensibleDictionary() : this([]) { }
        /// <summary>
        /// Initializes a copy of <see cref="OpenApiExtensibleDictionary{T}"/> class.
        /// </summary>
        /// <param name="dictionary">The generic dictionary.</param>
        /// <param name="extensions">The dictionary of <see cref="IOpenApiExtension"/>.</param>
        protected OpenApiExtensibleDictionary(
            Dictionary<string, T> dictionary,
            Dictionary<string, IOpenApiExtension>? extensions = null) : base(dictionary is null ? [] : dictionary)
        {
            Extensions = extensions != null ? new Dictionary<string, IOpenApiExtension>(extensions) : [];
        }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

        /// <summary>
        /// Serialize to Open Api v3.2
        /// </summary>
        /// <param name="writer"></param>
        public void SerializeAsV32(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_2, (writer, element) => element.SerializeAsV32(writer));
        }

        /// <summary>
        /// Serialize to Open Api v3.1
        /// </summary>
        /// <param name="writer"></param>
        public void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, (writer, element) => element.SerializeAsV31(writer));
        }

        /// <summary>
        /// Serialize to Open Api v3.0
        /// </summary>
        /// <param name="writer"></param>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0, (writer, element) => element.SerializeAsV3(writer));
        }

        /// <summary>
        /// Serialize to Open Api v3.0
        /// </summary>
        private void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version,
            Action<IOpenApiWriter, IOpenApiSerializable> callback)
        {
            Utils.CheckArgumentNull(writer);

            ValidatePathTemplateOperators(version);

            writer.WriteStartObject();

            foreach (var item in this)
            {
                writer.WriteRequiredObject(item.Key, item.Value, callback);
            }

            writer.WriteExtensions(Extensions, version);

            writer.WriteEndObject();
        }

        private static readonly char[] Rfc6570Operators = ['+', '#', '.', '/', ';', '?', '&'];

        private void ValidatePathTemplateOperators(OpenApiSpecVersion version)
        {
            if (version >= OpenApiSpecVersion.OpenApi3_2 || this is not OpenApiPaths)
            {
                return;
            }

            foreach (var path in Keys)
            {
                if (ContainsRfc6570Operator(path))
                {
                    throw new OpenApiException($"RFC 6570 URI template operators are only supported in OpenAPI 3.2 and later versions. Path: '{path}'. Current version: {version}");
                }
            }
        }

        /// <summary>
        /// Determines whether the given path contains any RFC 6570 operators.
        /// </summary>
        private static bool ContainsRfc6570Operator(string path)
        {
            if (path.Length == 0)
            {
                return false;
            }

            var template = path;

            var startIndex = 0;
            while (true)
            {
                var open = template.IndexOf('{', startIndex);
                if (open < 0)
                {
                    break;
                }

                var close = template.IndexOf('}', open + 1);
                if (close < 0)
                {
                    break;
                }

                var expression = template.Substring(open + 1, close - open - 1);
                if (!string.IsNullOrEmpty(expression))
                {
                    var first = expression[0];
                    if (Array.IndexOf(Rfc6570Operators, first) >= 0)
                    {
                        return true;
                    }

                    if (expression.IndexOf('*') >= 0)
                    {
                        return true;
                    }
                }

                startIndex = close + 1;
            }

            return false;
        }

        /// <summary>
        /// Serialize to Open Api v2.0
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            Utils.CheckArgumentNull(writer);

            ValidatePathTemplateOperators(OpenApiSpecVersion.OpenApi2_0);

            writer.WriteStartObject();

            foreach (var item in this)
            {
                writer.WriteRequiredObject(item.Key, item.Value, (w, p) => p.SerializeAsV2(w));
            }

            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi2_0);

            writer.WriteEndObject();
        }
    }
}
