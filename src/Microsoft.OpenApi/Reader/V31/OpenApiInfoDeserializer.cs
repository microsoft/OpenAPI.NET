using System;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader.ParseNodes;

namespace Microsoft.OpenApi.Reader.V31
{
    /// <summary>
    /// Class containing logic to deserialize Open API V31 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV31Deserializer
    {
        public static readonly FixedFieldMap<OpenApiInfo> InfoFixedFields = new()
        {
            {
                "title", (o, n) =>
                {
                    o.Title = n.GetScalarValue();
                }
            },
            {
                "version", (o, n) =>
                {
                    o.Version = n.GetScalarValue();
                }
            },
            {
                "summary", (o, n) =>
                {
                    o.Summary = n.GetScalarValue();
                }
            },
            {
                "description", (o, n) =>
                {
                    o.Description = n.GetScalarValue();
                }
            },
            {
                "termsOfService", (o, n) =>
                {
                    o.TermsOfService = new Uri(n.GetScalarValue(), UriKind.RelativeOrAbsolute);
                }
            },
            {
                "contact", (o, n) =>
                {
                    o.Contact = LoadContact(n);
                }
            },
            {
                "license", (o, n) =>
                {
                    o.License = LoadLicense(n);
                }
            }
        };

        public static readonly PatternFieldMap<OpenApiInfo> InfoPatternFields = new()
        {
            {s => s.StartsWith("x-"), (o, k, n) => o.AddExtension(k,LoadExtension(k, n))}
        };

        public static OpenApiInfo LoadInfo(ParseNode node, OpenApiDocument hostDocument = null)
        {
            var mapNode = node.CheckMapNode("Info");
            var info = new OpenApiInfo();
            ParseMap(mapNode, info, InfoFixedFields, InfoPatternFields);

            return info;
        }
    }
}
