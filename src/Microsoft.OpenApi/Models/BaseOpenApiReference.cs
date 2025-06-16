// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Reader;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// A simple object to allow referencing other components in the specification, internally and externally.
    /// </summary>
    public class BaseOpenApiReference : IOpenApiSerializable
    {
        /// <summary>
        /// External resource in the reference.
        /// It maybe:
        /// 1. a absolute/relative file path, for example:  ../commons/pet.json
        /// 2. a Url, for example: http://localhost/pet.json
        /// </summary>
        public string? ExternalResource { get; init; }
        
        /// <summary>
        /// The element type referenced.
        /// </summary>
        /// <remarks>This must be present if <see cref="ExternalResource"/> is not present.</remarks>
        public ReferenceType Type { get; init; }

        /// <summary>
        /// The identifier of the reusable component of one particular ReferenceType.
        /// If ExternalResource is present, this is the path to the component after the '#/'.
        /// For example, if the reference is 'example.json#/path/to/component', the Id is 'path/to/component'.
        /// If ExternalResource is not present, this is the name of the component without the reference type name.
        /// For example, if the reference is '#/components/schemas/componentName', the Id is 'componentName'.
        /// </summary>
        public string? Id { get; init; }

        /// <summary>
        /// Gets a flag indicating whether this reference is an external reference.
        /// </summary>
        public bool IsExternal => ExternalResource != null;

        /// <summary>
        /// Gets a flag indicating whether this reference is a local reference.
        /// </summary>
        public bool IsLocal => ExternalResource == null;

        /// <summary>
        /// Gets a flag indicating whether a file is a valid OpenAPI document or a fragment
        /// </summary>
        public bool IsFragment { get; init; }

        private OpenApiDocument? hostDocument;        
        /// <summary>
        /// The OpenApiDocument that is hosting the OpenApiReference instance. This is used to enable dereferencing the reference.
        /// </summary>
        public OpenApiDocument? HostDocument { get => hostDocument; init => hostDocument = value; }

        private string? _referenceV3;
        /// <summary>
        /// Gets the full reference string for v3.0.
        /// </summary>
        public string? ReferenceV3
        {
            get
            {
                if (!string.IsNullOrEmpty(_referenceV3))
                {
                    return _referenceV3;
                }

                if (IsExternal)
                {
                    return GetExternalReferenceV3();
                }

                if (Type == ReferenceType.Tag)
                {
                    return Id;
                }

                if (Type == ReferenceType.SecurityScheme)
                {
                    return Id;
                }
                if (!string.IsNullOrEmpty(Id) && Id is not null &&
                    (Id.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                     Id.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
#if NETSTANDARD2_1 || NETCOREAPP2_1_OR_GREATER || NET5_0_OR_GREATER
                     Id.Contains("#/components", StringComparison.OrdinalIgnoreCase)))
#else
                     Id.Contains("#/components")))
#endif
                {
                    return Id;
                }

                return $"#/components/{Type.GetDisplayName()}/{Id}";
            }
            private set 
            { 
                if (value is not null)
                {
                    _referenceV3 = value;
                }               
            }
        }

        /// <summary>
        /// Gets the full reference string for V2.0
        /// </summary>
        public string? ReferenceV2
        {
            get
            {
                if (IsExternal)
                {
                    return GetExternalReferenceV2();
                }

                if (Type == ReferenceType.Tag)
                {
                    return Id;
                }

                if (Type == ReferenceType.SecurityScheme)
                {
                    return Id;
                }

                return $"#/{GetReferenceTypeNameAsV2(Type)}/{Id}";
            }
        }

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public BaseOpenApiReference() { }

        /// <summary>
        /// Initializes a copy instance of the <see cref="BaseOpenApiReference"/> object
        /// </summary>
        public BaseOpenApiReference(BaseOpenApiReference reference)
        {
            Utils.CheckArgumentNull(reference);
            ExternalResource = reference.ExternalResource;
            Type = reference.Type;
            Id = reference.Id;
            HostDocument = reference.HostDocument;
        }

        /// <inheritdoc/>
        public virtual void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, SerializeAdditionalV31Properties);
        }

        /// <summary>
        /// Serialize additional properties for Open Api v3.1.
        /// </summary>
        /// <param name="writer"></param>
        protected virtual void SerializeAdditionalV31Properties(IOpenApiWriter writer)
        {
            // noop for the base type
        }

        /// <inheritdoc/>
        public virtual void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer);
        }

        /// <summary>
        /// Serialize <see cref="BaseOpenApiReference"/>
        /// </summary>
        private void SerializeInternal(IOpenApiWriter writer, Action<IOpenApiWriter>? callback = null)
        {
            Utils.CheckArgumentNull(writer);

            if (Type == ReferenceType.Tag && !string.IsNullOrEmpty(ReferenceV3) && ReferenceV3 is not null)
            {
                // Write the string value only
                writer.WriteValue(ReferenceV3);
                return;
            }

            writer.WriteStartObject();
            if (callback is not null)
            {
                callback(writer);
            }

            // $ref
            writer.WriteProperty(OpenApiConstants.DollarRef, ReferenceV3);

            writer.WriteEndObject();
        }

        /// <inheritdoc/>
        public virtual void SerializeAsV2(IOpenApiWriter writer)
        {
            Utils.CheckArgumentNull(writer);

            if (Type == ReferenceType.Tag && !string.IsNullOrEmpty(ReferenceV2) && ReferenceV2 is not null)
            {
                // Write the string value only
                writer.WriteValue(ReferenceV2);
                return;
            }

            if (Type == ReferenceType.SecurityScheme && !string.IsNullOrEmpty(ReferenceV2) && ReferenceV2 is not null)
            {
                // Write the string as property name
                writer.WritePropertyName(ReferenceV2);
                return;
            }

            writer.WriteStartObject();

            // $ref
            writer.WriteProperty(OpenApiConstants.DollarRef, ReferenceV2);

            writer.WriteEndObject();
        }

        private string? GetExternalReferenceV3()
        {
            if (Id != null)
            {
                if (IsFragment)
                {
                    return ExternalResource + "#" + Id;
                }

                if (Id.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                    Id.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    return Id;
                }

                return ExternalResource + "#/components/" + Type.GetDisplayName() + "/"+ Id; 
            }

            return ExternalResource;
        }

        private string? GetExternalReferenceV2()
        {
            if (Id is not null)
            {
                return ExternalResource + "#/" + GetReferenceTypeNameAsV2(Type) + "/" + Id;
            }

            return ExternalResource;
        }

        private static string? GetReferenceTypeNameAsV2(ReferenceType type)
        {
            return type switch
            {
                ReferenceType.Schema => OpenApiConstants.Definitions,
                ReferenceType.Parameter or ReferenceType.RequestBody => OpenApiConstants.Parameters,
                ReferenceType.Response => OpenApiConstants.Responses,
                ReferenceType.Header => OpenApiConstants.Headers,
                ReferenceType.Tag => OpenApiConstants.Tags,
                ReferenceType.SecurityScheme => OpenApiConstants.SecurityDefinitions,
                _ => null,// If the reference type is not supported in V2, simply return null
                          // to indicate that the reference is not pointing to any object.
            };
        }

        /// <summary>
        /// Sets the host document after deserialization or before serialization.
        /// This method is internal on purpose to avoid consumers mutating the host document.
        /// </summary>
        /// <param name="currentDocument">Host document to set if none is present</param>
        internal void EnsureHostDocumentIsSet(OpenApiDocument currentDocument)
        {
            Utils.CheckArgumentNull(currentDocument);
            hostDocument ??= currentDocument;
        }
        /// <summary>
        /// Gets the property value from a JsonObject node.
        /// </summary>
        /// <param name="jsonObject">The object to get the value from</param>
        /// <param name="key">The key of the property</param>
        /// <returns>The property value</returns>
        protected internal static string? GetPropertyValueFromNode(JsonObject jsonObject, string key) =>
        jsonObject.TryGetPropertyValue(key, out var valueNode) && valueNode is JsonValue valueCast && valueCast.TryGetValue<string>(out var strValue) ? strValue : null;
        internal virtual void SetMetadataFromMapNode(MapNode mapNode)
        {
            if (mapNode.JsonNode is not JsonObject jsonObject) return;
            SetAdditional31MetadataFromMapNode(jsonObject);
        }

        /// <summary>
        /// Sets additional metadata from the map node.
        /// </summary>
        /// <param name="jsonObject">The object to get the data from</param>
        protected virtual void SetAdditional31MetadataFromMapNode(JsonObject jsonObject)
        {
            // noop for the base type
        }

        internal void SetJsonPointerPath(string pointer, string nodeLocation)
        {
            // Relative reference to internal JSON schema node/resource (e.g. "#/properties/b")
#if NETSTANDARD2_1 || NETCOREAPP2_1_OR_GREATER || NET5_0_OR_GREATER
            if (pointer.StartsWith("#/", StringComparison.OrdinalIgnoreCase) && !pointer.Contains("/components/schemas", StringComparison.OrdinalIgnoreCase))
#else
            if (pointer.StartsWith("#/", StringComparison.OrdinalIgnoreCase) && !pointer.ToLowerInvariant().Contains("/components/schemas"))
#endif
            {
                ReferenceV3 = ResolveRelativePointer(nodeLocation, pointer);
            }

            // Absolute reference or anchor (e.g. "#/components/schemas/..." or full URL)
            else if ((pointer.Contains('#') || pointer.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                && !string.Equals(ReferenceV3, pointer, StringComparison.OrdinalIgnoreCase))
            {
                ReferenceV3 = pointer;
            }
        }

        private static string ResolveRelativePointer(string nodeLocation, string relativeRef)
        {
            // Convert nodeLocation to path segments
            var nodeLocationSegments = nodeLocation.TrimStart('#').Split(['/'], StringSplitOptions.RemoveEmptyEntries).ToList();

            // Convert relativeRef to dynamic segments
            var relativeSegments = relativeRef.TrimStart('#').Split(['/'], StringSplitOptions.RemoveEmptyEntries);

            // Locate the first occurrence of relativeRef segments in the full path
            for (int i = 0; i <= nodeLocationSegments.Count - relativeSegments.Length; i++)
            {
                if (relativeSegments.SequenceEqual(nodeLocationSegments.Skip(i).Take(relativeSegments.Length), StringComparer.Ordinal) &&
                    nodeLocationSegments.Take(i + relativeSegments.Length).ToArray() is {Length: > 0} matchingSegments)
                {
                    // Trim to include just the matching segment chain
                    return $"#/{string.Join("/", matchingSegments)}";
                }
            }

            // Fallback on building a full path
            if (nodeLocation.StartsWith("#/components/schemas/", StringComparison.OrdinalIgnoreCase))
            { // If the nodeLocation is a schema, we only want to keep the first three segments which are components/schemas/{schemaName}
                return $"#/{string.Join("/", nodeLocationSegments.Take(3).Concat(relativeSegments))}";
            }
#if NETSTANDARD2_1 || NETCOREAPP2_1_OR_GREATER || NET5_0_OR_GREATER
            return $"#/{string.Join("/", nodeLocationSegments.SkipLast(relativeSegments.Length).Concat(relativeSegments))}";
#else
            return $"#/{string.Join("/", nodeLocationSegments.Take(nodeLocationSegments.Count - relativeSegments.Length).Concat(relativeSegments))}";
#endif
        }
    }
}
