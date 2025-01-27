// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Services;

namespace Microsoft.OpenApi.Hidi
{
    internal class StatsVisitor : OpenApiVisitorBase
    {
        public int ParameterCount { get; set; }

        public override void Visit(IOpenApiParameter parameter)
        {
            ParameterCount++;
        }

        public int SchemaCount { get; set; }

        public override void Visit(OpenApiSchema schema)
        {
            SchemaCount++;
        }

        public int HeaderCount { get; set; }

        public override void Visit(IDictionary<string, IOpenApiHeader> headers)
        {
            HeaderCount++;
        }

        public int PathItemCount { get; set; }

        public override void Visit(IOpenApiPathItem pathItem)
        {
            PathItemCount++;
        }

        public int RequestBodyCount { get; set; }

        public override void Visit(OpenApiRequestBody requestBody)
        {
            RequestBodyCount++;
        }

        public int ResponseCount { get; set; }

        public override void Visit(OpenApiResponses response)
        {
            ResponseCount++;
        }

        public int OperationCount { get; set; }

        public override void Visit(OpenApiOperation operation)
        {
            OperationCount++;
        }

        public int LinkCount { get; set; }

        public override void Visit(IOpenApiLink link)
        {
            LinkCount++;
        }

        public int CallbackCount { get; set; }

        public override void Visit(IOpenApiCallback callback)
        {
            CallbackCount++;
        }

        public string GetStatisticsReport()
        {
            return $"Path Items: {PathItemCount}" + Environment.NewLine
                 + $"Operations: {OperationCount}" + Environment.NewLine
                 + $"Parameters: {ParameterCount}" + Environment.NewLine
                 + $"Request Bodies: {RequestBodyCount}" + Environment.NewLine
                 + $"Responses: {ResponseCount}" + Environment.NewLine
                 + $"Links: {LinkCount}" + Environment.NewLine
                 + $"Callbacks: {CallbackCount}" + Environment.NewLine
                 + $"Schemas: {SchemaCount}" + Environment.NewLine;
        }
    }
}
