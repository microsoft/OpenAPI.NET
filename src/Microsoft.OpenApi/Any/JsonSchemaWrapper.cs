using System;
using System.Collections.Generic;
using Json.Schema;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Any
{
    /// <summary>
    /// 
    /// </summary>
    public class JsonSchemaWrapper : IOpenApiElement, IOpenApiReferenceable, IOpenApiSerializable, IOpenApiExtensible
    {
        private readonly JsonSchema _jsonSchema;
        private IList<JsonSchemaWrapper> _allOf;
        private IList<JsonSchemaWrapper> _oneOf;
        private IList<JsonSchemaWrapper> _anyOf;
        private Dictionary<string, JsonSchemaWrapper> _properties;

        /// <summary>
        /// Initializes the <see cref="OpenApiAny"/> class.
        /// </summary>
        /// <param name="jsonSchema"></param>
        public JsonSchemaWrapper(JsonSchema jsonSchema)
        {
            _jsonSchema = jsonSchema;
        }

        public JsonSchemaWrapper()
        {
            _jsonSchema = new JsonSchemaBuilder();
        }

        /// <summary>
        /// Gets the underlying JsonNode.
        /// </summary>
        public JsonSchema JsonSchema => _jsonSchema;

        public IList<JsonSchemaWrapper> AllOf 
        {
            get
            {
                if (_allOf == null)
                {
                    _allOf = new List<JsonSchemaWrapper>();
                    var allOf = _jsonSchema.GetAllOf();                    
                    if (allOf != null)
                    {
                        foreach (var item in allOf)
                        {
                            _allOf.Add(new JsonSchemaWrapper(item));
                        }
                    }
                }
                return _allOf;
            }
        }

        public IList<JsonSchemaWrapper> OneOf 
        {
            get
            {
                if (_oneOf == null)
                {
                    _oneOf = new List<JsonSchemaWrapper>();
                    var oneOf = _jsonSchema.GetOneOf();
                    if (oneOf != null)
                    {
                        foreach (var item in oneOf)
                        {
                            _oneOf.Add(new JsonSchemaWrapper(item));
                        }
                    }
                }
                return _oneOf;
            }
        }

        public IList<JsonSchemaWrapper> AnyOf
        {
            get
            {
                if (_anyOf == null)
                {
                    _anyOf = new List<JsonSchemaWrapper>();
                    var oneOf = _jsonSchema.GetOneOf();
                    if (oneOf != null)
                    {
                        foreach (var item in oneOf)
                        {
                            _anyOf.Add(new JsonSchemaWrapper(item));
                        }
                    }
                }
                return _anyOf;
            }
        }

        public JsonSchemaWrapper Items => new JsonSchemaWrapper(_jsonSchema.GetItems());

        public IDictionary<string, JsonSchemaWrapper> Properties
        {
            get
            {
                if (_properties == null)
                {
                    _properties = new Dictionary<string, JsonSchemaWrapper>();
                    var properties = _jsonSchema.GetProperties();
                    if (properties != null)
                    {
                        foreach(var item in properties)
                        {
                            _properties.Add(item.Key, new JsonSchemaWrapper(item.Value));
                        }
                    }
                }
                return _properties;
            }
        }

        public JsonSchemaWrapper AdditionalProperties => new JsonSchemaWrapper(_jsonSchema.GetAdditionalProperties());

        /// <inheritdoc/>
        public bool UnresolvedReference { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public OpenApiReference Reference { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IDictionary<string, IOpenApiExtension> Extensions { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void SerializeAsV2WithoutReference(IOpenApiWriter writer)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void SerializeAsV31(IOpenApiWriter writer)
        {
            throw new NotImplementedException();
        }

        public void SerializeAsV31WithoutReference(IOpenApiWriter writer)
        {
            throw new NotImplementedException();
        }

        public void SerializeAsV3WithoutReference(IOpenApiWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
