// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Reader;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// A simple object to allow referencing other components in the specification, internally and externally.
    /// </summary>
    public class OpenApiReference : IOpenApiSerializable, IOpenApiDescribedElement, IOpenApiSummarizedElement
    {
        /// <summary>
        /// A short summary which by default SHOULD override that of the referenced component.
        /// If the referenced object-type does not allow a summary field, then this field has no effect.
        /// </summary>
        public string? Summary { get; set; }

        /// <summary>
        /// A description which by default SHOULD override that of the referenced component.
        /// CommonMark syntax MAY be used for rich text representation.
        /// If the referenced object-type does not allow a description field, then this field has no effect.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// A default value which by default SHOULD override that of the referenced component.
        /// If the referenced object-type does not allow a default field, then this field has no effect.
        /// </summary>
        public JsonNode? Default { get; set; }

        /// <summary>
        /// A title which by default SHOULD override that of the referenced component.
        /// If the referenced object-type does not allow a title field, then this field has no effect.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Indicates whether the referenced component is deprecated.
        /// If the referenced object-type does not allow a deprecated field, then this field has no effect.
        /// </summary>
        public bool? Deprecated { get; set; }

        /// <summary>
        /// Indicates whether the referenced component is read-only.
        /// If the referenced object-type does not allow a readOnly field, then this field has no effect.
        /// </summary>
        public bool? ReadOnly { get; set; }

        /// <summary>
        /// Indicates whether the referenced component is write-only.
        /// If the referenced object-type does not allow a writeOnly field, then this field has no effect.
        /// </summary>
        public bool? WriteOnly { get; set; }

        /// <summary>
        /// Example values which by default SHOULD override those of the referenced component.
        /// If the referenced object-type does not allow examples, then this field has no effect.
        /// </summary>
        public IList<JsonNode>? Examples { get; set; }

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
                if (!string.IsNullOrEmpty(Id) && Id is not null && Id.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                    !string.IsNullOrEmpty(Id) && Id is not null && Id.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
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
        public OpenApiReference() { }

        /// <summary>
        /// Initializes a copy instance of the <see cref="OpenApiReference"/> object
        /// </summary>
        public OpenApiReference(OpenApiReference reference)
        {
            Utils.CheckArgumentNull(reference);
            Summary = reference.Summary;
            Description = reference.Description;
            Default = reference.Default;
            Title = reference.Title;
            Deprecated = reference.Deprecated;
            ReadOnly = reference.ReadOnly;
            WriteOnly = reference.WriteOnly;
            Examples = reference.Examples;
            ExternalResource = reference.ExternalResource;
            Type = reference.Type;
            Id = reference.Id;
            HostDocument = reference.HostDocument;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiReference"/> to Open Api v3.1.
        /// </summary>
        public void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, w =>
            {
                // summary and description are in 3.1 but not in 3.0
                w.WriteProperty(OpenApiConstants.Summary, Summary);
                w.WriteProperty(OpenApiConstants.Description, Description);
                
                // Additional schema metadata annotations in 3.1
                w.WriteOptionalObject(OpenApiConstants.Default, Default, (w, d) => w.WriteAny(d));
                w.WriteProperty(OpenApiConstants.Title, Title);
                w.WriteProperty(OpenApiConstants.Deprecated, Deprecated, false);
                w.WriteProperty(OpenApiConstants.ReadOnly, ReadOnly, false);
                w.WriteProperty(OpenApiConstants.WriteOnly, WriteOnly, false);
                if (Examples != null && Examples.Any())
                {
                    w.WriteOptionalCollection(OpenApiConstants.Examples, Examples, (w, e) => w.WriteAny(e));
                }
            });
        }

        /// <summary>
        /// Serialize <see cref="OpenApiReference"/> to Open Api v3.0.
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiReference"/>
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

        /// <summary>
        /// Serialize <see cref="OpenApiReference"/> to Open Api v2.0.
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
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
        private static string? GetPropertyValueFromNode(JsonObject jsonObject, string key) =>
        jsonObject.TryGetPropertyValue(key, out var valueNode) && valueNode is JsonValue valueCast && valueCast.TryGetValue<string>(out var strValue) ? strValue : null;
        internal void SetMetadataFromMapNode(MapNode mapNode)
        {
            if (mapNode.JsonNode is not JsonObject jsonObject) return;

            // Summary and Description
            var description = GetPropertyValueFromNode(jsonObject, OpenApiConstants.Description);
            var summary = GetPropertyValueFromNode(jsonObject, OpenApiConstants.Summary);
            var title = GetPropertyValueFromNode(jsonObject, OpenApiConstants.Title);

            if (!string.IsNullOrEmpty(description))
            {
                Description = description;
            }
            if (!string.IsNullOrEmpty(summary))
            {
                Summary = summary;
            }
            if (!string.IsNullOrEmpty(title))
            {
                Title = title;
            }

            // Boolean properties
            if (jsonObject.TryGetPropertyValue(OpenApiConstants.Deprecated, out var deprecatedNode) && deprecatedNode is JsonValue deprecatedValue)
            {
                if (deprecatedValue.TryGetValue<bool>(out var deprecated))
                {
                    Deprecated = deprecated;
                }
            }

            if (jsonObject.TryGetPropertyValue(OpenApiConstants.ReadOnly, out var readOnlyNode) && readOnlyNode is JsonValue readOnlyValue)
            {
                if (readOnlyValue.TryGetValue<bool>(out var readOnly))
                {
                    ReadOnly = readOnly;
                }
            }

            if (jsonObject.TryGetPropertyValue(OpenApiConstants.WriteOnly, out var writeOnlyNode) && writeOnlyNode is JsonValue writeOnlyValue)
            {
                if (writeOnlyValue.TryGetValue<bool>(out var writeOnly))
                {
                    WriteOnly = writeOnly;
                }
            }

            // Default value
            if (jsonObject.TryGetPropertyValue(OpenApiConstants.Default, out var defaultNode))
            {
                Default = defaultNode;
            }

            // Examples
            if (jsonObject.TryGetPropertyValue(OpenApiConstants.Examples, out var examplesNode) && examplesNode is JsonArray examplesArray)
            {
                Examples = new List<JsonNode>();
                foreach (var example in examplesArray)
                {
                    if (example != null)
                    {
                        Examples.Add(example);
                    }
                }
            }
        }

        internal void SetJsonPointerPath(string pointer, string nodeLocation)
        {
            // Relative reference to internal JSON schema node/resource (e.g. "#/properties/b")
            if (pointer.StartsWith("#/", StringComparison.OrdinalIgnoreCase) && !pointer.Contains("/components/schemas"))
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
            var segments = nodeLocation.TrimStart('#').Split(new char[] {'/'}, StringSplitOptions.RemoveEmptyEntries).ToList();

            // Convert relativeRef to dynamic segments
            var relativeSegments = relativeRef.TrimStart('#').Split(new char[] {'/'}, StringSplitOptions.RemoveEmptyEntries);

            // Locate the first occurrence of relativeRef segments in the full path
            for (int i = 0; i <= segments.Count - relativeSegments.Length; i++)
            {
                if (relativeSegments.SequenceEqual(segments.Skip(i).Take(relativeSegments.Length)))
                {
                    // Trim to include just the matching segment chain
                    segments = [.. segments.Take(i + relativeSegments.Length)];
                    break;
                }
            }

            return $"#/{string.Join("/", segments)}";
        }
    }
}
