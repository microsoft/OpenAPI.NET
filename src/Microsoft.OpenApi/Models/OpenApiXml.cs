// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// XML Object.
    /// </summary>
    public class OpenApiXml : IOpenApiSerializable, IOpenApiExtensible
    {
        /// <summary>
        /// Replaces the name of the element/attribute used for the described schema property.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The URI of the namespace definition. Value MUST be in the form of an absolute URI.
        /// </summary>
        public Uri? Namespace { get; set; }

        /// <summary>
        /// The prefix to be used for the name
        /// </summary>
        public string? Prefix { get; set; }

        /// <summary>
        /// Declares whether the property definition translates to an attribute instead of an element.
        /// Default value is false.
        /// </summary>
        [Obsolete("Use NodeType property instead. This property will be removed in a future version.")]
        internal bool Attribute
        {
            get
            {
                return NodeType == OpenApiXmlNodeType.Attribute;
            }
            set
            {
                NodeType = value ? OpenApiXmlNodeType.Attribute : OpenApiXmlNodeType.None;
            }
        }

        /// <summary>
        /// Signifies whether the array is wrapped.
        /// Default value is false.
        /// </summary>
        [Obsolete("Use NodeType property instead. This property will be removed in a future version.")]
        internal bool Wrapped
        {
            get
            {
                return NodeType == OpenApiXmlNodeType.Element;
            }
            set
            {
                NodeType = value ? OpenApiXmlNodeType.Element : OpenApiXmlNodeType.None;
            }
        }

        /// <summary>
        /// The node type of the XML representation.
        /// </summary>
        public OpenApiXmlNodeType? NodeType { get; set; }

        /// <summary>
        /// Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public OpenApiXml() { }

        /// <summary>
        /// Initializes a copy of an <see cref="OpenApiXml"/> object
        /// </summary>
        public OpenApiXml(OpenApiXml xml)
        {
            Name = xml?.Name ?? Name;
            Namespace = xml?.Namespace ?? Namespace;
            Prefix = xml?.Prefix ?? Prefix;
            NodeType = xml?.NodeType ?? NodeType;
            Extensions = xml?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(xml.Extensions) : null;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiXml"/> to Open Api v3.2
        /// </summary>
        public virtual void SerializeAsV32(IOpenApiWriter writer)
        {
            Write(writer, OpenApiSpecVersion.OpenApi3_2);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiXml"/> to Open Api v3.1
        /// </summary>
        public virtual void SerializeAsV31(IOpenApiWriter writer)
        {
            Write(writer, OpenApiSpecVersion.OpenApi3_1);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiXml"/> to Open Api v3.0
        /// </summary>
        public virtual void SerializeAsV3(IOpenApiWriter writer)
        {
            Write(writer, OpenApiSpecVersion.OpenApi3_0);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiXml"/> to Open Api v2.0
        /// </summary>
        public virtual void SerializeAsV2(IOpenApiWriter writer)
        {
            Write(writer, OpenApiSpecVersion.OpenApi2_0);
        }

        private void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            Utils.CheckArgumentNull(writer);

            writer.WriteStartObject();

            // name
            writer.WriteProperty(OpenApiConstants.Name, Name);

            // namespace
            writer.WriteProperty(OpenApiConstants.Namespace, Namespace?.AbsoluteUri);

            // prefix
            writer.WriteProperty(OpenApiConstants.Prefix, Prefix);

            // For OpenAPI 3.2.0 and above, serialize nodeType
            if (specVersion >= OpenApiSpecVersion.OpenApi3_2)
            {
                if (NodeType.HasValue)
                {
                    writer.WriteProperty(OpenApiConstants.NodeType, NodeType.Value.GetDisplayName());
                }
            }
            else
            {
                // For OpenAPI 3.1.0 and below, serialize attribute and wrapped
                // Use backing fields if they were set via obsolete properties,
                // otherwise derive from NodeType if set
                var attribute = NodeType.HasValue && NodeType == OpenApiXmlNodeType.Attribute;
                var wrapped = NodeType.HasValue && NodeType == OpenApiXmlNodeType.Element;
                
                writer.WriteProperty(OpenApiConstants.Attribute, attribute, false);
                writer.WriteProperty(OpenApiConstants.Wrapped, wrapped, false);
            }

            // extensions
            writer.WriteExtensions(Extensions, specVersion);

            writer.WriteEndObject();
        }
    }
}
