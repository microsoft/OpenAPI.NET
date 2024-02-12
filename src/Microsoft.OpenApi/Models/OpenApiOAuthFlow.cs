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
    /// OAuth Flow Object.
    /// </summary>
    public class OpenApiOAuthFlow : IOpenApiSerializable, IOpenApiExtensible
    {
        /// <summary>
        /// REQUIRED. The authorization URL to be used for this flow.
        /// Applies to implicit and authorizationCode OAuthFlow.
        /// </summary>
        public Uri AuthorizationUrl { get; set; }

        /// <summary>
        /// REQUIRED. The token URL to be used for this flow.
        /// Applies to password, clientCredentials, and authorizationCode OAuthFlow.
        /// </summary>
        public Uri TokenUrl { get; set; }

        /// <summary>
        /// The URL to be used for obtaining refresh tokens.
        /// </summary>
        public Uri RefreshUrl { get; set; }

        /// <summary>
        /// REQUIRED. A map between the scope name and a short description for it.
        /// </summary>
        public IDictionary<string, string> Scopes { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public OpenApiOAuthFlow() { }

        /// <summary>
        /// Initializes a copy of an <see cref="OpenApiOAuthFlow"/> object
        /// </summary>
        public OpenApiOAuthFlow(OpenApiOAuthFlow oAuthFlow)
        {
            AuthorizationUrl = oAuthFlow?.AuthorizationUrl != null ? new Uri(oAuthFlow.AuthorizationUrl.OriginalString, UriKind.RelativeOrAbsolute) : null;
            TokenUrl = oAuthFlow?.TokenUrl != null ? new Uri(oAuthFlow.TokenUrl.OriginalString, UriKind.RelativeOrAbsolute) : null;
            RefreshUrl = oAuthFlow?.RefreshUrl != null ? new Uri(oAuthFlow.RefreshUrl.OriginalString, UriKind.RelativeOrAbsolute) : null;
            Scopes = oAuthFlow?.Scopes != null ? new Dictionary<string, string>(oAuthFlow.Scopes) : null;
            Extensions = oAuthFlow?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(oAuthFlow.Extensions) : null;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiOAuthFlow"/> to Open Api v3.1
        /// </summary>
        public void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiOAuthFlow"/> to Open Api v3.0
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiOAuthFlow"/> to Open Api v3.0
        /// </summary>
        private void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version)
        {
            Utils.CheckArgumentNull(writer);;

            writer.WriteStartObject();

            // authorizationUrl
            writer.WriteProperty(OpenApiConstants.AuthorizationUrl, AuthorizationUrl?.ToString());

            // tokenUrl
            writer.WriteProperty(OpenApiConstants.TokenUrl, TokenUrl?.ToString());

            // refreshUrl
            writer.WriteProperty(OpenApiConstants.RefreshUrl, RefreshUrl?.ToString());

            // scopes
            writer.WriteRequiredMap(OpenApiConstants.Scopes, Scopes, (w, s) => w.WriteValue(s));

            // extensions
            writer.WriteExtensions(Extensions, version);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiOAuthFlow"/> to Open Api v2.0
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            // OAuthFlow object does not exist in V2.
        }

        /// <summary>
        /// Parses a local file path or Url into an OpenApiOAuthFlow object.
        /// </summary>
        /// <param name="url"> The path to the OpenAPI file.</param>
        /// <param name="version">The OpenAPI specification version.</param>
        /// <param name="diagnostic"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static OpenApiOAuthFlow Load(string url, OpenApiSpecVersion version, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings settings = null)
        {
            return OpenApiModelFactory.Load<OpenApiOAuthFlow>(url, version, out diagnostic, settings);
        }

        /// <summary>
        /// Reads the stream input and parses it into an OpenApiOAuthFlow object.
        /// </summary>
        /// <param name="stream">Stream containing OpenAPI description to parse.</param>
        /// <param name="format">The OpenAPI format to use during parsing.</param>
        /// <param name="version"></param>
        /// <param name="diagnostic"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static OpenApiOAuthFlow Load(Stream stream,
                                           string format,
                                           OpenApiSpecVersion version,
                                           out OpenApiDiagnostic diagnostic,
                                           OpenApiReaderSettings settings = null)
        {
            return OpenApiModelFactory.Load<OpenApiOAuthFlow>(stream, version, out diagnostic, format, settings);
        }

        /// <summary>
        /// Reads the text reader content and parses it into an OpenApiOAuthFlow object.
        /// </summary>
        /// <param name="input">TextReader containing OpenAPI description to parse.</param>
        /// <param name="format"> The OpenAPI format to use during parsing.</param>
        /// <param name="version"></param>
        /// <param name="diagnostic"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static OpenApiOAuthFlow Load(TextReader input,
                                           string format,
                                           OpenApiSpecVersion version,
                                           out OpenApiDiagnostic diagnostic,
                                           OpenApiReaderSettings settings = null)
        {
            return OpenApiModelFactory.Load<OpenApiOAuthFlow>(input, version, out diagnostic, format, settings);
        }


        /// <summary>
        /// Parses a string into a <see cref="OpenApiOAuthFlow"/> object.
        /// </summary>
        /// <param name="input"> The string input.</param>
        /// <param name="version"></param>
        /// <param name="diagnostic"></param>
        /// <param name="format"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static OpenApiOAuthFlow Parse(string input,
                                            OpenApiSpecVersion version,
                                            out OpenApiDiagnostic diagnostic,
                                            string format = null,
                                            OpenApiReaderSettings settings = null)
        {
            return OpenApiModelFactory.Parse<OpenApiOAuthFlow>(input, version, out diagnostic, format, settings);
        }
    }
}
