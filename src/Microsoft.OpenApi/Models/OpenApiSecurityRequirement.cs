// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Security Requirement Object.
    /// Each name MUST correspond to a security scheme which is declared in
    /// the Security Schemes under the Components Object.
    /// If the security scheme is of type "oauth2" or "openIdConnect",
    /// then the value is a list of scope names required for the execution.
    /// For other security scheme types, the array MUST be empty.
    /// </summary>
    public class OpenApiSecurityRequirement : Dictionary<OpenApiSecurityScheme, IList<string>>,
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
        public void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, (writer, element) => element.SerializeAsV31(writer));
        }

        /// <summary>
        /// Serialize <see cref="OpenApiSecurityRequirement"/> to Open Api v3.0
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, (writer, element) => element.SerializeAsV3(writer));
        }

        /// <summary>
        /// Serialize <see cref="OpenApiSecurityRequirement"/> 
        /// </summary>
        private void SerializeInternal(IOpenApiWriter writer, Action<IOpenApiWriter, IOpenApiSerializable> callback)
        {
            Utils.CheckArgumentNull(writer);;

            writer.WriteStartObject();

            foreach (var securitySchemeAndScopesValuePair in this)
            {
                var securityScheme = securitySchemeAndScopesValuePair.Key;
                var scopes = securitySchemeAndScopesValuePair.Value;

                if (securityScheme.Reference == null)
                {
                    // Reaching this point means the reference to a specific OpenApiSecurityScheme fails.
                    // We are not able to serialize this SecurityScheme/Scopes key value pair since we do not know what
                    // string to output.
                    continue;
                }

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

        /// <summary>
        /// Serialize <see cref="OpenApiSecurityRequirement"/> to Open Api v2.0
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            Utils.CheckArgumentNull(writer);;

            writer.WriteStartObject();

            foreach (var securitySchemeAndScopesValuePair in this)
            {
                var securityScheme = securitySchemeAndScopesValuePair.Key;
                var scopes = securitySchemeAndScopesValuePair.Value;

                if (securityScheme.Reference == null)
                {
                    // Reaching this point means the reference to a specific OpenApiSecurityScheme fails.
                    // We are not able to serialize this SecurityScheme/Scopes key value pair since we do not know what
                    // string to output.
                    continue;
                }

                securityScheme.SerializeAsV2(writer);

                writer.WriteStartArray();

                foreach (var scope in scopes)
                {
                    writer.WriteValue(scope);
                }

                writer.WriteEndArray();
            }

            writer.WriteEndObject();
        }

        /// <summary>
        /// Parses a local file path or Url into an OpenApiSecurityRequirement object.
        /// </summary>
        /// <param name="url"> The path to the OpenAPI file.</param>
        /// <param name="version">The OpenAPI specification version.</param>
        /// <param name="diagnostic"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static OpenApiSecurityRequirement Load(string url, OpenApiSpecVersion version, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings settings = null)
        {
            return OpenApiModelFactory.Load<OpenApiSecurityRequirement>(url, version, out diagnostic, settings);
        }

        /// <summary>
        /// Reads the stream input and parses it into an OpenApiSecurityRequirement object.
        /// </summary>
        /// <param name="stream">Stream containing OpenAPI description to parse.</param>
        /// <param name="format">The OpenAPI format to use during parsing.</param>
        /// <param name="version"></param>
        /// <param name="diagnostic"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static OpenApiSecurityRequirement Load(Stream stream,
                                           string format,
                                           OpenApiSpecVersion version,
                                           out OpenApiDiagnostic diagnostic,
                                           OpenApiReaderSettings settings = null)
        {
            return OpenApiModelFactory.Load<OpenApiSecurityRequirement>(stream, version, out diagnostic, format, settings);
        }

        /// <summary>
        /// Reads the text reader content and parses it into an OpenApiSecurityRequirement object.
        /// </summary>
        /// <param name="input">TextReader containing OpenAPI description to parse.</param>
        /// <param name="format"> The OpenAPI format to use during parsing.</param>
        /// <param name="version"></param>
        /// <param name="diagnostic"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static OpenApiSecurityRequirement Load(TextReader input,
                                           string format,
                                           OpenApiSpecVersion version,
                                           out OpenApiDiagnostic diagnostic,
                                           OpenApiReaderSettings settings = null)
        {
            return OpenApiModelFactory.Load<OpenApiSecurityRequirement>(input, version, out diagnostic, format, settings);
        }


        /// <summary>
        /// Parses a string into a <see cref="OpenApiSecurityRequirement"/> object.
        /// </summary>
        /// <param name="input"> The string input.</param>
        /// <param name="version"></param>
        /// <param name="diagnostic"></param>
        /// <param name="format"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static OpenApiSecurityRequirement Parse(string input,
                                            OpenApiSpecVersion version,
                                            out OpenApiDiagnostic diagnostic,
                                            string format = null,
                                            OpenApiReaderSettings settings = null)
        {
            return OpenApiModelFactory.Parse<OpenApiSecurityRequirement>(input, version, out diagnostic, format, settings);
        }

        /// <summary>
        /// Comparer for OpenApiSecurityScheme that only considers the Id in the Reference
        /// (i.e. the string that will actually be displayed in the written document)
        /// </summary>
        private class OpenApiSecuritySchemeReferenceEqualityComparer : IEqualityComparer<OpenApiSecurityScheme>
        {
            /// <summary>
            /// Determines whether the specified objects are equal.
            /// </summary>
            public bool Equals(OpenApiSecurityScheme x, OpenApiSecurityScheme y)
            {
                if (x == null && y == null)
                {
                    return true;
                }

                if (x == null || y == null)
                {
                    return false;
                }

                if (x.Reference == null || y.Reference == null)
                {
                    return false;
                }

                return x.Reference.Id == y.Reference.Id;
            }

            /// <summary>
            /// Returns a hash code for the specified object.
            /// </summary>
            public int GetHashCode(OpenApiSecurityScheme obj)
            {
                return obj?.Reference?.Id == null ? 0 : obj.Reference.Id.GetHashCode();
            }
        }
    }
}
