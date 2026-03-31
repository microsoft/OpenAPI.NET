// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Security Requirement Object.
    /// Each name MUST correspond to a security scheme which is declared in
    /// the Security Schemes under the Components Object.
    /// If the security scheme is of type "oauth2" or "openIdConnect",
    /// then the value is a list of scope names required for the execution.
    /// For other security scheme types, the array MUST be empty.
    /// </summary>
    public class OpenApiSecurityRequirement : Dictionary<OpenApiSecuritySchemeReference, List<string>>,
        IOpenApiSerializable
    {
        /// <summary>
        /// Initializes the <see cref="OpenApiSecurityRequirement"/> class.
        /// This constructor ensures that only Reference.Id is considered when two dictionary keys
        /// of type <see cref="OpenApiSecurityScheme"/> are compared.
        /// </summary>
        public OpenApiSecurityRequirement()
            : base(new OpenApiSecuritySchemeReferenceEqualityComparer())
        {
        }

        /// <summary>
        /// Serialize <see cref="OpenApiSecurityRequirement"/> to Open Api v3.1
        /// </summary>
        public virtual void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, (w, s) =>
            {
                if(!string.IsNullOrEmpty(s.Reference.ReferenceV3) && s.Reference.ReferenceV3 is not null)
                {
                    w.WritePropertyName(s.Reference.ReferenceV3);
                }
            });
        }

        /// <summary>
        /// Serialize <see cref="OpenApiSecurityRequirement"/> to Open Api v3.0
        /// </summary>
        public virtual void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, (w, s) =>
            {
                if (!string.IsNullOrEmpty(s.Reference.ReferenceV3) && s.Reference.ReferenceV3 is not null)
                {
                    w.WritePropertyName(s.Reference.ReferenceV3);
                }
            });
        }

        /// <summary>
        /// Serialize <see cref="OpenApiSecurityRequirement"/> 
        /// </summary>
        private void SerializeInternal(IOpenApiWriter writer, Action<IOpenApiWriter, OpenApiSecuritySchemeReference> callback)
        {
            Utils.CheckArgumentNull(writer);

            writer.WriteStartObject();

            foreach (var securitySchemeAndScopesValuePair in this.Where(static p => CanSerializeSecurityScheme(p.Key)))
            {
                var securityScheme = securitySchemeAndScopesValuePair.Key;
                var scopes = securitySchemeAndScopesValuePair.Value;

                callback(writer, securityScheme);

                writer.WriteStartArray();

                foreach (var scope in scopes)
                {
                    writer.WriteValue(scope);
                }

                writer.WriteEndArray();
            }

            writer.WriteEndObject();
        }

        private static bool CanSerializeSecurityScheme(OpenApiSecuritySchemeReference? securityScheme)
        {
            if (securityScheme is null)
            {
                return false;
            }

            if (securityScheme.Target is not null)
            {
                return true;
            }

            var schemeId = securityScheme.Reference?.Id;
            var securitySchemes = securityScheme.Reference?.HostDocument?.Components?.SecuritySchemes;
            return !string.IsNullOrEmpty(schemeId)
                && securitySchemes is not null
                && securitySchemes.ContainsKey(schemeId!);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiSecurityRequirement"/> to Open Api v2.0
        /// </summary>
        public virtual void SerializeAsV2(IOpenApiWriter writer)
        {
            SerializeInternal(writer, (w, s) => s.SerializeAsV2(w));
        }

        /// <summary>
        /// Comparer for OpenApiSecurityScheme that only considers the Id in the Reference
        /// (i.e. the string that will actually be displayed in the written document)
        /// </summary>
        private sealed class OpenApiSecuritySchemeReferenceEqualityComparer : IEqualityComparer<OpenApiSecuritySchemeReference>
        {
            /// <summary>
            /// Determines whether the specified objects are equal.
            /// </summary>
            public bool Equals(OpenApiSecuritySchemeReference? x, OpenApiSecuritySchemeReference? y)
            {
                if (x == null && y == null)
                {
                    return true;
                }

                if (x == null || y == null)
                {
                    return false;
                }

                return GetHashCode(x) == GetHashCode(y);
            }

            /// <summary>
            /// Returns a hash code for the specified object.
            /// </summary>
            public int GetHashCode(OpenApiSecuritySchemeReference obj)
            {
                if (obj is null)
                {
                    return 0;
                }
                var id = obj.Reference?.Id;
                return string.IsNullOrEmpty(id) ? 0 : id!.GetHashCode();
            }
        }
    }
}
