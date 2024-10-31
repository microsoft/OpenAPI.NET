// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Tests.UtilityFiles
{
    /// <summary>
    /// Mock class that creates a sample OpenAPI document.
    /// </summary>
    public static class OpenApiDocumentMock
    {
        /// <summary>
        /// Creates an OpenAPI document.
        /// </summary>
        /// <returns>Instance of an OpenApi document</returns>
        public static OpenApiDocument CreateOpenApiDocument()
        {
            var applicationJsonMediaType = "application/json";

            var document = new OpenApiDocument
            {
                Info = new()
                {
                    Title = "People",
                    Version = "v1.0"
                },
                Servers = new List<OpenApiServer>
                {
                    new()
                    {
                        Url = "https://graph.microsoft.com/v1.0"
                    }
                },
                Paths = new()
                {
                    ["/"] = new() // root path
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>
                        {
                            {
                                OperationType.Get, new OpenApiOperation
                                {
                                    OperationId = "graphService.GetGraphService",
                                    Responses = new()
                                    {
                                        {
                                            "200",new()
                                            {
                                                Description = "OK"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    ["/reports/microsoft.graph.getTeamsUserActivityCounts(period={period})"] = new()
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>
                        {
                            {
                                OperationType.Get, new OpenApiOperation
                                {
                                    Tags = new List<OpenApiTag>
                                    {
                                        {
                                            new()
                                            {
                                                Name = "reports.Functions"
                                            }
                                        }
                                    },
                                    OperationId = "reports.getTeamsUserActivityCounts",
                                    Summary = "Invoke function getTeamsUserActivityUserCounts",
                                    Parameters = new List<OpenApiParameter>
                                    {
                                        {
                                            new()
                                            {
                                                Name = "period",
                                                In = ParameterLocation.Path,
                                                Required = true,
                                                Schema = new()
                                                {
                                                    Type = JsonSchemaType.String
                                                }
                                            }
                                        }
                                    },
                                    Responses = new()
                                    {
                                        {
                                            "200", new()
                                            {
                                                Description = "Success",
                                                Content = new Dictionary<string, OpenApiMediaType>
                                                {
                                                    {
                                                        applicationJsonMediaType,
                                                        new OpenApiMediaType
                                                        {
                                                            Schema = new()
                                                            {
                                                                Type = JsonSchemaType.Array
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        Parameters = new List<OpenApiParameter>
                        {
                            {
                                new()
                                {
                                    Name = "period",
                                    In = ParameterLocation.Path,
                                    Required = true,
                                    Schema = new()
                                    {
                                        Type = JsonSchemaType.String
                                    }
                                }
                            }
                        }
                    },
                    ["/reports/microsoft.graph.getTeamsUserActivityUserDetail(date={date})"] = new()
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>
                        {
                            {
                                OperationType.Get, new OpenApiOperation
                                {
                                    Tags = new List<OpenApiTag>
                                    {
                                        {
                                            new()
                                            {
                                                Name = "reports.Functions"
                                            }
                                        }
                                    },
                                    OperationId = "reports.getTeamsUserActivityUserDetail-a3f1",
                                    Summary = "Invoke function getTeamsUserActivityUserDetail",
                                    Parameters = new List<OpenApiParameter>
                                    {
                                        {
                                            new()
                                            {
                                                Name = "period",
                                                In = ParameterLocation.Path,
                                                Required = true,
                                                Schema = new()
                                                {
                                                    Type = JsonSchemaType.String
                                                }
                                            }
                                        }
                                    },
                                    Responses = new()
                                    {
                                        {
                                            "200", new()
                                            {
                                                Description = "Success",
                                                Content = new Dictionary<string, OpenApiMediaType>
                                                {
                                                    {
                                                        applicationJsonMediaType,
                                                        new OpenApiMediaType
                                                        {
                                                            Schema = new()
                                                            {
                                                                Type = JsonSchemaType.Array
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        Parameters = new List<OpenApiParameter>
                        {
                            new()
                            {
                                Name = "period",
                                In = ParameterLocation.Path,
                                Required = true,
                                Schema = new()
                                {
                                    Type = JsonSchemaType.String
                                }
                            }
                        }
                    },
                    ["/users"] = new()
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>
                        {
                            {
                                OperationType.Get, new OpenApiOperation
                                {
                                    Tags = new List<OpenApiTag>
                                    {
                                        {
                                            new()
                                            {
                                                Name = "users.user"
                                            }
                                        }
                                    },
                                    OperationId = "users.user.ListUser",
                                    Summary = "Get entities from users",
                                    Responses = new()
                                    {
                                        {
                                            "200", new()
                                            {
                                                Description = "Retrieved entities",
                                                Content = new Dictionary<string, OpenApiMediaType>
                                                {
                                                    {
                                                        applicationJsonMediaType,
                                                        new OpenApiMediaType
                                                        {
                                                            Schema = new()
                                                            {
                                                                Title = "Collection of user",
                                                                Type = JsonSchemaType.Object,
                                                                Properties = new Dictionary<string, OpenApiSchema>
                                                                {
                                                                    {
                                                                        "value",
                                                                        new OpenApiSchema
                                                                        {
                                                                            Type = JsonSchemaType.Array,
                                                                            Items = new()
                                                                            {
                                                                                Reference = new()
                                                                                {
                                                                                    Type = ReferenceType.Schema,
                                                                                    Id = "microsoft.graph.user"
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    ["/users/{user-id}"] = new()
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>
                        {
                            {
                                OperationType.Get, new OpenApiOperation
                                {
                                    Tags = new List<OpenApiTag>
                                    {
                                        {
                                            new()
                                            {
                                                Name = "users.user"
                                            }
                                        }
                                    },
                                    OperationId = "users.user.GetUser",
                                    Summary = "Get entity from users by key",
                                    Responses = new()
                                    {
                                        {
                                            "200", new()
                                            {
                                                Description = "Retrieved entity",
                                                Content = new Dictionary<string, OpenApiMediaType>
                                                {
                                                    {
                                                        applicationJsonMediaType,
                                                        new OpenApiMediaType
                                                        {
                                                            Schema = new()
                                                            {
                                                                Reference = new()
                                                                {
                                                                    Type = ReferenceType.Schema,
                                                                    Id = "microsoft.graph.user"
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            {
                                OperationType.Patch, new OpenApiOperation
                                {
                                    Tags = new List<OpenApiTag>
                                    {
                                        {
                                            new()
                                            {
                                                Name = "users.user"
                                            }
                                        }
                                    },
                                    OperationId = "users.user.UpdateUser",
                                    Summary = "Update entity in users",
                                    Responses = new()
                                    {
                                        {
                                            "204", new()
                                            {
                                                Description = "Success"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    ["/users/{user-id}/messages/{message-id}"] = new()
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>
                        {
                            {
                                OperationType.Get, new OpenApiOperation
                                {
                                    Tags = new List<OpenApiTag>
                                    {
                                        {
                                            new()
                                            {
                                                Name = "users.message"
                                            }
                                        }
                                    },
                                    OperationId = "users.GetMessages",
                                    Summary = "Get messages from users",
                                    Description = "The messages in a mailbox or folder. Read-only. Nullable.",
                                    Parameters = new List<OpenApiParameter>
                                    {
                                        new()
                                        {
                                            Name = "$select",
                                            In = ParameterLocation.Query,
                                            Required = true,
                                            Description = "Select properties to be returned",
                                            Schema = new()
                                            {
                                                Type = JsonSchemaType.Array
                                            }
                                            // missing explode parameter
                                        }
                                    },
                                    Responses = new()
                                    {
                                        {
                                            "200", new()
                                            {
                                                Description = "Retrieved navigation property",
                                                Content = new Dictionary<string, OpenApiMediaType>
                                                {
                                                    {
                                                        applicationJsonMediaType,
                                                        new OpenApiMediaType
                                                        {
                                                            Schema = new()
                                                            {
                                                                Reference = new()
                                                                {
                                                                    Type = ReferenceType.Schema,
                                                                    Id = "microsoft.graph.message"
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    ["/administrativeUnits/{administrativeUnit-id}/microsoft.graph.restore"] = new()
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>
                        {
                            {
                                OperationType.Post, new OpenApiOperation
                                {
                                    Tags = new List<OpenApiTag>
                                    {
                                        {
                                            new()
                                            {
                                                Name = "administrativeUnits.Actions"
                                            }
                                        }
                                    },
                                    OperationId = "administrativeUnits.restore",
                                    Summary = "Invoke action restore",
                                    Parameters = new List<OpenApiParameter>
                                    {
                                        {
                                            new()
                                            {
                                                Name = "administrativeUnit-id",
                                                In = ParameterLocation.Path,
                                                Required = true,
                                                Description = "key: id of administrativeUnit",
                                                Schema = new()
                                                {
                                                    Type = JsonSchemaType.String
                                                }
                                            }
                                        }
                                    },
                                    Responses = new()
                                    {
                                        {
                                            "200", new()
                                            {
                                                Description = "Success",
                                                Content = new Dictionary<string, OpenApiMediaType>
                                                {
                                                    {
                                                        applicationJsonMediaType,
                                                        new OpenApiMediaType
                                                        {
                                                            Schema = new()
                                                            {
                                                                AnyOf = new List<OpenApiSchema>
                                                                {
                                                                    new()
                                                                    {
                                                                        Type = JsonSchemaType.String
                                                                    }
                                                                },
                                                                Nullable = true
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    ["/applications/{application-id}/logo"] = new()
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>
                        {
                            {
                                OperationType.Put, new OpenApiOperation
                                {
                                    Tags = new List<OpenApiTag>
                                    {
                                        {
                                            new()
                                            {
                                                Name = "applications.application"
                                            }
                                        }
                                    },
                                    OperationId = "applications.application.UpdateLogo",
                                    Summary = "Update media content for application in applications",
                                    Responses = new()
                                    {
                                        {
                                            "204", new()
                                            {
                                                Description = "Success"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    ["/security/hostSecurityProfiles"] = new()
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>
                        {
                            {
                                OperationType.Get, new OpenApiOperation
                                {
                                    Tags = new List<OpenApiTag>
                                    {
                                        {
                                            new()
                                            {
                                                Name = "security.hostSecurityProfile"
                                            }
                                        }
                                    },
                                    OperationId = "security.ListHostSecurityProfiles",
                                    Summary = "Get hostSecurityProfiles from security",
                                    Responses = new()
                                    {
                                        {
                                            "200", new()
                                            {
                                                Description = "Retrieved navigation property",
                                                Content = new Dictionary<string, OpenApiMediaType>
                                                {
                                                    {
                                                        applicationJsonMediaType,
                                                        new OpenApiMediaType
                                                        {
                                                            Schema = new()
                                                            {
                                                                Title = "Collection of hostSecurityProfile",
                                                                Type = JsonSchemaType.Object,
                                                                Properties = new Dictionary<string, OpenApiSchema>
                                                                {
                                                                    {
                                                                        "value",
                                                                        new OpenApiSchema
                                                                        {
                                                                            Type = JsonSchemaType.Array,
                                                                            Items = new()
                                                                            {
                                                                                Reference = new()
                                                                                {
                                                                                    Type = ReferenceType.Schema,
                                                                                    Id = "microsoft.graph.networkInterface"
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    ["/communications/calls/{call-id}/microsoft.graph.keepAlive"] = new()
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>
                        {
                            {
                                OperationType.Post, new OpenApiOperation
                                {
                                    Tags = new List<OpenApiTag>
                                    {
                                        {
                                            new()
                                            {
                                                Name = "communications.Actions"
                                            }
                                        }
                                    },
                                    OperationId = "communications.calls.call.keepAlive",
                                    Summary = "Invoke action keepAlive",
                                    Parameters = new List<OpenApiParameter>
                                    {
                                        new()
                                        {
                                            Name = "call-id",
                                            In = ParameterLocation.Path,
                                            Description = "key: id of call",
                                            Required = true,
                                            Schema = new()
                                            {
                                                Type = JsonSchemaType.String
                                            },
                                            Extensions = new Dictionary<string, IOpenApiExtension>
                                            {
                                                {
                                                    "x-ms-docs-key-type", new OpenApiAny("call")
                                                }
                                            }
                                        }
                                    },
                                    Responses = new()
                                    {
                                        {
                                            "204", new()
                                            {
                                                Description = "Success"
                                            }
                                        }
                                    },
                                    Extensions = new Dictionary<string, IOpenApiExtension>
                                    {
                                        {
                                            "x-ms-docs-operation-type", new OpenApiAny("action")
                                        }
                                    }
                                }
                            }
                        }
                    },
                    ["/groups/{group-id}/events/{event-id}/calendar/events/microsoft.graph.delta"] = new()
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>
                        {
                            {
                                OperationType.Get, new OpenApiOperation
                                {
                                    Tags = new List<OpenApiTag>
                                    {
                                        new()
                                        {
                                            Name = "groups.Functions"
                                        }
                                    },
                                    OperationId = "groups.group.events.event.calendar.events.delta",
                                    Summary = "Invoke function delta",
                                    Parameters = new List<OpenApiParameter>
                                    {
                                        new()
                                        {
                                            Name = "group-id",
                                            In = ParameterLocation.Path,
                                            Description = "key: id of group",
                                            Required = true,
                                            Schema = new()
                                            {
                                                Type = JsonSchemaType.String
                                            },
                                            Extensions = new Dictionary<string, IOpenApiExtension>
                                            {
                                                {
                                                    "x-ms-docs-key-type", new OpenApiAny("group")
                                                }
                                            }
                                        },
                                        new()
                                        {
                                            Name = "event-id",
                                            In = ParameterLocation.Path,
                                            Description = "key: id of event",
                                            Required = true,
                                            Schema = new()
                                            {
                                                Type = JsonSchemaType.String
                                            },
                                            Extensions = new Dictionary<string, IOpenApiExtension>
                                            {
                                                {
                                                    "x-ms-docs-key-type", new OpenApiAny("event")
                                                }
                                            }
                                        }
                                    },
                                    Responses = new()
                                    {
                                        {
                                            "200", new()
                                            {
                                                Description = "Success",
                                                Content = new Dictionary<string, OpenApiMediaType>
                                                {
                                                    {
                                                        applicationJsonMediaType,
                                                        new OpenApiMediaType
                                                        {
                                                            Schema = new()
                                                            {
                                                                Type = JsonSchemaType.Array,
                                                                Reference = new()
                                                                {
                                                                    Type = ReferenceType.Schema,
                                                                    Id = "microsoft.graph.event"
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    },
                                    Extensions = new Dictionary<string, IOpenApiExtension>
                                    {
                                        {
                                            "x-ms-docs-operation-type", new OpenApiAny("function")
                                        }
                                    }
                                }
                            }
                        }
                    },
                    ["/applications/{application-id}/createdOnBehalfOf/$ref"] = new()
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>
                        {
                            {
                                OperationType.Get, new OpenApiOperation
                                {
                                    Tags = new List<OpenApiTag>
                                    {
                                        new()
                                        {
                                            Name = "applications.directoryObject"
                                        }
                                    },
                                    OperationId = "applications.GetRefCreatedOnBehalfOf",
                                    Summary = "Get ref of createdOnBehalfOf from applications"
                                }
                            }
                        }
                    }
                },
                Components = new()
                {
                    Schemas = new Dictionary<string, OpenApiSchema>
                    {
                        {
                            "microsoft.graph.networkInterface", new OpenApiSchema
                            {
                                Title = "networkInterface",
                                Type = JsonSchemaType.Object,
                                Properties = new Dictionary<string, OpenApiSchema>
                                {
                                    {
                                        "description", new OpenApiSchema
                                        {
                                            Type = JsonSchemaType.String,
                                            Description = "Description of the NIC (e.g. Ethernet adapter, Wireless LAN adapter Local Area Connection <#>, etc.).",
                                            Nullable = true
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
            return document;
        }
    }
}
