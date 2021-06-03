﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Services;

namespace Microsoft.OpenApi.Tool
{
    internal class StatsVisitor : OpenApiVisitorBase
    {
        public int ParameterCount { get; set; } = 0;

        public override void Visit(OpenApiParameter parameter)
        {
            ParameterCount++;
        }

        public int SchemaCount { get; set; } = 0;

        public override void Visit(OpenApiSchema schema)
        {
            SchemaCount++;
        }

        public int HeaderCount { get; set; } = 0;

        public override void Visit(IDictionary<string, OpenApiHeader> headers)
        {
            HeaderCount++;
        }

        public int PathItemCount { get; set; } = 0;

        public override void Visit(OpenApiPathItem pathItem)
        {
            PathItemCount++;
        }

        public int RequestBodyCount { get; set; } = 0;

        public override void Visit(OpenApiRequestBody requestBody)
        {
            RequestBodyCount++;
        }

        public int ResponseCount { get; set; } = 0;

        public override void Visit(OpenApiResponses response)
        {
            ResponseCount++;
        }

        public int OperationCount { get; set; } = 0;

        public override void Visit(OpenApiOperation operation)
        {
            OperationCount++;
        }

        public int LinkCount { get; set; } = 0;

        public override void Visit(OpenApiLink operation)
        {
            LinkCount++;
        }

        public int CallbackCount { get; set; } = 0;

        public override void Visit(OpenApiCallback callback)
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
