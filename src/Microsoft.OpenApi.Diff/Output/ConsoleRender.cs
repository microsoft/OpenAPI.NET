using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Diff.BusinessObjects;
using Microsoft.OpenApi.Diff.Enums;
using Microsoft.OpenApi.Diff.Extensions;
using Microsoft.OpenApi.Diff.Utils;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Diff.Output
{
    public class ConsoleRender : IConsoleRender
    {
        private static readonly RefPointer<OpenApiSchema> RefPointer = new RefPointer<OpenApiSchema>(RefTypeEnum.Schemas);
        private static ChangedOpenApiBO _diff;

        private StringBuilder _output;

        public Task<string> Render(ChangedOpenApiBO diff)
        {
            _diff = diff;
            _output = new StringBuilder();
            if (diff.IsUnchanged())
            {
                _output.Append("No differences. Specifications are equivalents");
            }
            else
            {
                _output
                    .Append(Environment.NewLine)
                    .Append(BigTitle("Api Change Log"))
                    .Append(Center(diff.NewSpecOpenApi.Info.Title))
                    .Append(Environment.NewLine);

                var newEndpoints = diff.NewEndpoints;
                var olNewEndpoint = ListEndpoints(newEndpoints, "What's New");

                var missingEndpoints = diff.MissingEndpoints;
                var olMissingEndpoint = ListEndpoints(missingEndpoints, "What's Deleted");

                var deprecatedEndpoints = diff.GetDeprecatedEndpoints();
                var olDeprecatedEndpoint = ListEndpoints(deprecatedEndpoints, "What's Deprecated");

                var changedOperations = diff.ChangedOperations;
                var olChanged = OlChanged(changedOperations);

                _output
                    .Append(renderBody(olNewEndpoint, olMissingEndpoint, olDeprecatedEndpoint, olChanged))
                    .Append(Title("Result"))
                    .Append(
                        Center(
                            diff.IsCompatible()
                                ? "API changes are backward compatible"
                                : "API changes broke backward compatibility"))
                    .Append(Environment.NewLine)
                    .Append(Separator('-'));
            }
            return Task.FromResult(_output.ToString());
        }
        private static string ListEndpoints(IReadOnlyCollection<EndpointBO> endpoints, string title)
        {
            if (null == endpoints || endpoints.Count == 0) return "";
            var sb = new StringBuilder();
            sb.Append(Title(title));
            foreach (var endpoint in endpoints)
            {
                sb.Append(ItemEndpoint(
                    endpoint.Method.ToString(), endpoint.PathUrl, endpoint.Summary));
            }
            return sb.Append(Environment.NewLine).ToString();
        }
        private static string ItemEndpoint(string method, string path, string desc)
        {
            var sb = new StringBuilder();
            sb.Append($"- {method,6} {path}{Environment.NewLine}");
            return sb.ToString();
        }
        private static string renderBody(string olNew, string olMiss, string olDeprecated, string olChanged)
        {
            var sb = new StringBuilder();
            sb.Append(olNew).Append(olMiss).Append(olDeprecated).Append(olChanged);
            return sb.ToString();
        }
        private static string BigTitle(string title)
        {
            const char ch = '=';
            return Title(title.ToUpper(), ch);
        }
        private static string Title(string title, char ch = '-')
        {
            var little = new string(ch, 2);
            var offset = little.Length * 2;

            return $"{Separator(ch)}{little}{Center(title, -offset)}{little.PadLeft(Console.WindowWidth / 2 - title.Length / 2 + little.Length)}{Environment.NewLine}{Separator(ch)}";
        }
        private static StringBuilder Separator(char ch)
        {
            var sb = new StringBuilder();
            return sb.Append(new string(ch, Console.WindowWidth))
                .Append(Environment.NewLine);
        }
        private static string Center(string center, int offset = 0)
        {
            return string.Format("{0," + (Console.WindowWidth / 2 + center.Length / 2 + offset) + "}", center);
        }
        private static string OlChanged(IReadOnlyCollection<ChangedOperationBO> operations)
        {
            if (null == operations || operations.Count == 0) return "";
            var sb = new StringBuilder();
            sb.Append(Title("What's Changed"));
            foreach (var operation in operations)
            {
                var pathUrl = operation.PathUrl;
                var method = operation.HttpMethod.ToString();
                var desc = operation.Summary?.Right ?? "";

                var ul_detail = new StringBuilder();
                if (ChangedBO.Result(operation.Parameters).IsDifferent())
                {
                    ul_detail
                        .Append(new string(' ', 2))
                        .Append("Parameter:")
                        .Append(Environment.NewLine)
                        .Append(UlParam(operation.Parameters));
                }
                if (operation.ResultRequestBody().IsDifferent())
                {
                    ul_detail
                        .Append(new string(' ', 2))
                        .Append("Request:")
                        .Append(Environment.NewLine)
                        .Append(UlContent(operation.RequestBody.Content, true));
                }
                if (operation.ResultApiResponses().IsDifferent())
                {
                    ul_detail
                        .Append(new string(' ', 2))
                        .Append("Return Type:")
                        .Append(Environment.NewLine)
                        .Append(UlResponse(operation.APIResponses));
                }
                sb.Append(ItemEndpoint(method, pathUrl, desc)).Append(ul_detail);
            }

            return sb.ToString();
        }
        private static string UlParam(ChangedParametersBO changedParameters)
        {
            var addParameters = changedParameters.Increased;
            var delParameters = changedParameters.Missing;
            var changed = changedParameters.Changed;
            var sb = new StringBuilder();
            foreach (var param in addParameters)
            {
                sb.Append(ItemParam("Add ", param));
            }
            foreach (var param in changed)
            {
                sb.Append(LiChangedParam(param));
            }
            foreach (var param in delParameters)
            {
                sb.Append(ItemParam("Delete ", param));

            }
            return sb.ToString();
        }
        private static string UlResponse(ChangedAPIResponseBO changedApiResponse)
        {
            var addResponses = changedApiResponse.Increased;
            var delResponses = changedApiResponse.Missing;
            var changedResponses = changedApiResponse.Changed;
            var sb = new StringBuilder();
            foreach (var propName in addResponses.Keys)
            {
                sb.Append(ItemResponse("Add ", propName));
            }
            foreach (var propName in delResponses.Keys)
            {
                sb.Append(ItemResponse("Deleted ", propName));
            }
            foreach (var propName in changedResponses.Keys)
            {
                sb.Append(ItemChangedResponse("Changed ", propName, changedResponses[propName]));
            }
            return sb.ToString();
        }
        private static string ItemResponse(string title, string code)
        {
            var sb = new StringBuilder();
            var status = "";
            if (code != "default" && int.TryParse(code, out var statusCode))
            {
                status = ((HttpStatusCode)statusCode).ToString();
            }
            sb.Append(new string(' ', 4))
                .Append("- ")
                .Append(title)
                .Append(code)
                .Append(' ')
                .Append(status)
                .Append(Environment.NewLine);
            return sb.ToString();
        }
        private static string ItemParam(string title, OpenApiParameter param)
        {
            var sb = new StringBuilder("");
            sb.Append(new string(' ', 4))
                .Append("- ")
                .Append(title)
                .Append(param.Name)
                .Append(" in ")
                .Append(param.In)
                .Append(Environment.NewLine);

            return sb.ToString();
        }
        private static string LiChangedParam(ChangedParameterBO changeParam)
        {
            return ItemParam(changeParam.IsDeprecated ? "Deprecated " : "Changed ", changeParam.NewParameter);
        }
        private static string ItemChangedResponse(string title, string contentType, ChangedResponseBO response)
        {
            var sb = new StringBuilder();
            sb.Append(ItemResponse(title, contentType));
            sb.Append(new string(' ', 6)).Append("Media types:").Append(Environment.NewLine);
            sb.Append(UlContent(response.Content, false));
            return sb.ToString();
        }
        private static string UlContent(ChangedContentBO changedContent, bool isRequest)
        {
            var sb = new StringBuilder();
            if (changedContent == null)
            {
                return sb.ToString();
            }
            foreach (var propName in changedContent.Increased.Keys)
            {
                sb.Append(ItemContent("Added ", propName));
            }
            foreach (var propName in changedContent.Missing.Keys)
            {
                sb.Append(ItemContent("Deleted ", propName));
            }
            foreach (var propName in changedContent.Changed.Keys)
            {
                sb.Append(ItemContent("Changed ", propName, changedContent.Changed[propName], isRequest));
            }
            return sb.ToString();
        }
        private static string ItemContent(string title, string contentType)
        {
            var sb = new StringBuilder();
            sb.Append(new string(' ', 8))
                .Append("- ")
                .Append(title)
                .Append(contentType)
                .Append(Environment.NewLine);
            return sb.ToString();
        }
        private static string ItemContent(string title, string contentType, ChangedMediaTypeBO changedMediaType, bool isRequest)
        {
            var sb = new StringBuilder();
            sb.Append(ItemContent(title, contentType))
                .Append(new string(' ', 10))
                .Append("Schema: ")
                .Append(changedMediaType.IsCompatible() ? "Backward compatible" : "Broken compatibility")
                .Append(Environment.NewLine);
            if (!changedMediaType.IsCompatible())
            {
                sb.Append(Incompatibilities(changedMediaType.Schema));
            }
            return sb.ToString();
        }
        private static string Incompatibilities(ChangedSchemaBO schema)
        {
            return Incompatibilities("", schema);
        }
        private static string Incompatibilities(string propName, ChangedSchemaBO schema)
        {
            var sb = new StringBuilder();
            if (schema.Items != null)
            {
                sb.Append(Items(propName, schema.Items));
            }
            if (schema.IsCoreChanged().DiffResult == DiffResultEnum.Incompatible && schema.IsChangedType)
            {
                var type = schema.OldSchema.GetSchemaType() + " -> " + schema.OldSchema.GetSchemaType();
                sb.Append(Property(propName, "Changed property type", type));
            }
            var prefix = propName.IsNullOrEmpty() ? "" : propName + ".";
            sb.Append(
                Properties(prefix, "Missing property", schema.MissingProperties, schema.Context));
            foreach (var (name, value) in schema.ChangedProperties)
            {
                sb.Append(Incompatibilities(prefix + name, value));
            }
            return sb.ToString();
        }
        private static string Items(string propName, ChangedSchemaBO schema)
        {
            var sb = new StringBuilder();
            sb.Append(Incompatibilities(propName + "[]", schema));
            return sb.ToString();
        }
        private static string Properties(string propPrefix, string title, Dictionary<string, OpenApiSchema> properties, DiffContextBO context)
        {
            var sb = new StringBuilder();
            if (properties != null)
            {
                foreach (var (key, value) in properties)
                {
                    sb.Append(Property(propPrefix + key, title, Resolve(value)));
                }
            }
            return sb.ToString();
        }
        private static OpenApiSchema Resolve(OpenApiSchema schema)
        {
            return RefPointer.ResolveRef(_diff.NewSpecOpenApi.Components, schema, schema.Reference?.ReferenceV3);
        }
        private static string Property(string name, string title, OpenApiSchema schema)
        {
            return Property(name, title, Type(schema));
        }
        private static string Property(string name, string title, string type)
        {
            return $"{new string(' ', 10)}{title}: {name} {type}\n";
        }
        private static string Type(OpenApiSchema schema)
        {
            var result = "object";
            if (schema.GetSchemaType() == SchemaTypeEnum.ArraySchema)
            {
                result = "array";
            }
            else if (schema.Type != null)
            {
                result = schema.Type;
            }
            return result;
        }
    }
}
