// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Exceptions;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers.ParseNodes
{
    internal class PropertyNode : ParseNode
    {
        public PropertyNode(ParsingContext context, OpenApiDiagnostic diagnostic, string name, YamlNode node) : base(
            context,
            diagnostic)
        {
            Name = name;
            Value = Create(context, diagnostic, node);
        }

        public string Name { get; set; }

        public ParseNode Value { get; set; }

        public void ParseField<T>(
            T parentInstance,
            IDictionary<string, Action<T, ParseNode>> fixedFields,
            IDictionary<Func<string, bool>, Action<T, string, ParseNode>> patternFields)
        {
            Action<T, ParseNode> fixedFieldMap;
            var found = fixedFields.TryGetValue(Name, out fixedFieldMap);

            if (fixedFieldMap != null)
            {
                try
                {
                    Context.StartObject(Name);
                    fixedFieldMap(parentInstance, Value);
                }
                catch (OpenApiReaderException ex)
                {
                    Diagnostic.Errors.Add(new OpenApiError(ex));
                }
                catch (OpenApiException ex)
                {
                    ex.Pointer = Context.GetLocation();
                    Diagnostic.Errors.Add(new OpenApiError(ex));
                }
                finally
                {
                    Context.EndObject();
                }
            }
            else
            {
                var map = patternFields.Where(p => p.Key(Name)).Select(p => p.Value).FirstOrDefault();
                if (map != null)
                {
                    try
                    {
                        Context.StartObject(Name);
                        map(parentInstance, Name, Value);
                    }
                    catch (OpenApiReaderException ex)
                    {
                        Diagnostic.Errors.Add(new OpenApiError(ex));
                    }
                    catch (OpenApiException ex)
                    {
                        ex.Pointer = Context.GetLocation();
                        Diagnostic.Errors.Add(new OpenApiError(ex));
                    }
                    finally
                    {
                        Context.EndObject();
                    }
                }
                else
                {
                    Diagnostic.Errors.Add(
                        new OpenApiError("", $"{Name} is not a valid property at {Context.GetLocation()}"));
                }
            }
        }

        public override IOpenApiAny CreateAny()
        {
            throw new NotImplementedException();
        }
    }
}