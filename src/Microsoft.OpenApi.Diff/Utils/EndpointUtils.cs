using System.Collections.Generic;
using Microsoft.OpenApi.Diff.BusinessObjects;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Diff.Utils
{
    public class EndpointUtils
    {
        public static List<T> ConvertToEndpoints<T>(string pathUrl, Dictionary<OperationType, OpenApiOperation> dict)
            where T : EndpointBO, new()
        {
            var endpoints = new List<T>();
            if (dict == null) return endpoints;
            foreach (var (key, value) in dict)
            {
                var endpoint = ConvertToEndpoint<T>(pathUrl, key, value);
                endpoints.Add(endpoint);
            }
            return endpoints;
        }

        public static T ConvertToEndpoint<T>(string pathUrl, OperationType httpMethod, OpenApiOperation operation)
            where T : EndpointBO, new()
        {
            var endpoint = new T
            {
                PathUrl = pathUrl,
                Method = httpMethod,
                Summary = operation.Summary,
                Operation = operation
            };
            return endpoint;
        }

        public static List<T> ConvertToEndpointList<T>(Dictionary<string, OpenApiPathItem> dict)
            where T : EndpointBO, new()
        {
            var endpoints = new List<T>();
            if (dict == null) return endpoints;

            foreach (var (key, value) in dict)
            {
                var operationMap = value.Operations;
                foreach (var (operationType, openApiOperation) in operationMap)
                {
                    var endpoint = new T
                    {
                        PathUrl = key,
                        Method = operationType,
                        Summary = openApiOperation.Summary,
                        Path = value,
                        Operation = openApiOperation
                    };
                    endpoints.Add(endpoint);
                }
            }
            return endpoints;
        }
    }

}