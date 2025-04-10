// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Models.References;

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
            const string getTeamsActivityByPeriodPath = "/reports/microsoft.graph.getTeamsUserActivityCounts(period={period})";
            const string getTeamsActivityByDatePath = "/reports/microsoft.graph.getTeamsUserActivityUserDetail(date={date})";
            const string usersPath = "/users";
            const string usersByIdPath = "/users/{user-id}";
            const string messagesByIdPath = "/users/{user-id}/messages/{message-id}";
            const string administrativeUnitRestorePath = "/administrativeUnits/{administrativeUnit-id}/microsoft.graph.restore";
            const string logoPath = "/applications/{application-id}/logo";
            const string securityProfilesPath = "/security/hostSecurityProfiles";
            const string communicationsCallsKeepAlivePath = "/communications/calls/{call-id}/microsoft.graph.keepAlive";
            const string eventsDeltaPath = "/groups/{group-id}/events/{event-id}/calendar/events/microsoft.graph.delta";
            const string refPath = "/applications/{application-id}/createdOnBehalfOf/$ref";

            var document = new OpenApiDocument
            {
                Info = new()
                {
                    Title = "People",
                    Version = "v1.0"
                },
                Servers =
                [
                    new()
                    {
                        Url = "https://graph.microsoft.com/v1.0"
                    }
                ],
                Paths = new()
                {
                    ["/"] = new OpenApiPathItem() // root path
                    {
                        Operations = new()
                        {
                            {
                                HttpMethod.Get, new OpenApiOperation
                                {
                                    OperationId = "graphService.GetGraphService",
                                    Responses = new()
                                    {
                                        {
                                            "200",new OpenApiResponse()
                                            {
                                                Description = "OK"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    [getTeamsActivityByPeriodPath] = new OpenApiPathItem()
                    {
                        Operations = new()
                        {
                            {
                                HttpMethod.Get, new OpenApiOperation
                                {
                                    OperationId = "reports.getTeamsUserActivityCounts",
                                    Summary = "Invoke function getTeamsUserActivityUserCounts",
                                    Parameters =
                                    [
                                        new OpenApiParameter()
                                        {
                                            Name = "period",
                                            In = ParameterLocation.Path,
                                            Required = true,
                                            Schema = new OpenApiSchema()
                                            {
                                                Type = JsonSchemaType.String
                                            }
                                        }
                                    ],
                                    Responses = new()
                                    {
                                        {
                                            "200", new OpenApiResponse()
                                            {
                                                Description = "Success",
                                                Content = new()
                                                {
                                                    {
                                                        applicationJsonMediaType,
                                                        new OpenApiMediaType
                                                        {
                                                            Schema = new OpenApiSchema()
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
                        Parameters = 
                        [
                            new OpenApiParameter()
                            {
                                Name = "period",
                                In = ParameterLocation.Path,
                                Required = true,
                                Schema = new OpenApiSchema()
                                {
                                    Type = JsonSchemaType.String
                                }
                            }
                        ]
                    },
                    [getTeamsActivityByDatePath] = new OpenApiPathItem()
                    {
                        Operations = new()
                        {
                            {
                                HttpMethod.Get, new OpenApiOperation
                                {
                                    OperationId = "reports.getTeamsUserActivityUserDetail-a3f1",
                                    Summary = "Invoke function getTeamsUserActivityUserDetail",
                                    Parameters =
                                    [
                                        new OpenApiParameter()
                                        {
                                            Name = "period",
                                            In = ParameterLocation.Path,
                                            Required = true,
                                            Schema = new OpenApiSchema()
                                            {
                                                Type = JsonSchemaType.String
                                            }
                                        }
                                    ],
                                    Responses = new()
                                    {
                                        {
                                            "200", new OpenApiResponse()
                                            {
                                                Description = "Success",
                                                Content = new()
                                                {
                                                    {
                                                        applicationJsonMediaType,
                                                        new OpenApiMediaType
                                                        {
                                                            Schema = new OpenApiSchema()
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
                        Parameters =
                        [
                            new OpenApiParameter()
                            {
                                Name = "period",
                                In = ParameterLocation.Path,
                                Required = true,
                                Schema = new OpenApiSchema()
                                {
                                    Type = JsonSchemaType.String
                                }
                            }
                        ]
                    },
                    [usersPath] = new OpenApiPathItem()
                    {
                        Operations = new()
                        {
                            {
                                HttpMethod.Get, new OpenApiOperation
                                {
                                    OperationId = "users.user.ListUser",
                                    Summary = "Get entities from users",
                                    Responses = new()
                                    {
                                        {
                                            "200", new OpenApiResponse()
                                            {
                                                Description = "Retrieved entities",
                                                Content = new()
                                                {
                                                    {
                                                        applicationJsonMediaType,
                                                        new OpenApiMediaType
                                                        {
                                                            Schema = new OpenApiSchema()
                                                            {
                                                                Title = "Collection of user",
                                                                Type = JsonSchemaType.Object,
                                                                Properties = new()
                                                                {
                                                                    {
                                                                        "value",
                                                                        new OpenApiSchema
                                                                        {
                                                                            Type = JsonSchemaType.Array,
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
                    [usersByIdPath] = new OpenApiPathItem()
                    {
                        Operations = new()
                        {
                            {
                                HttpMethod.Get, new OpenApiOperation
                                {
                                    OperationId = "users.user.GetUser",
                                    Summary = "Get entity from users by key",
                                    Responses = new()
                                    {
                                        {
                                            "200", new OpenApiResponse()
                                            {
                                                Description = "Retrieved entity",
                                                Content = new()
                                                {
                                                    {
                                                        applicationJsonMediaType,
                                                        new OpenApiMediaType
                                                        {
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            {
                                HttpMethod.Patch, new OpenApiOperation
                                {
                                    OperationId = "users.user.UpdateUser",
                                    Summary = "Update entity in users",
                                    Responses = new()
                                    {
                                        {
                                            "204", new OpenApiResponse()
                                            {
                                                Description = "Success"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    [messagesByIdPath] = new OpenApiPathItem()
                    {
                        Operations = new()
                        {
                            {
                                HttpMethod.Get, new OpenApiOperation
                                {
                                    OperationId = "users.GetMessages",
                                    Summary = "Get messages from users",
                                    Description = "The messages in a mailbox or folder. Read-only. Nullable.",
                                    Parameters =
                                    [
                                        new OpenApiParameter()
                                        {
                                            Name = "$select",
                                            In = ParameterLocation.Query,
                                            Required = true,
                                            Description = "Select properties to be returned",
                                            Schema = new OpenApiSchema()
                                            {
                                                Type = JsonSchemaType.Array
                                            }
                                            // missing explode parameter
                                        }
                                    ],
                                    Responses = new()
                                    {
                                        {
                                            "200", new OpenApiResponse()
                                            {
                                                Description = "Retrieved navigation property",
                                                Content = new()
                                                {
                                                    {
                                                        applicationJsonMediaType,
                                                        new OpenApiMediaType
                                                        {
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
                    [administrativeUnitRestorePath] = new OpenApiPathItem()
                    {
                        Operations = new()
                        {
                            {
                                HttpMethod.Post, new OpenApiOperation
                                {
                                    OperationId = "administrativeUnits.restore",
                                    Summary = "Invoke action restore",
                                    Parameters = new List<IOpenApiParameter>
                                    {
                                        {
                                            new OpenApiParameter()
                                            {
                                                Name = "administrativeUnit-id",
                                                In = ParameterLocation.Path,
                                                Required = true,
                                                Description = "key: id of administrativeUnit",
                                                Schema = new OpenApiSchema()
                                                {
                                                    Type = JsonSchemaType.String
                                                }
                                            }
                                        }
                                    },
                                    Responses = new()
                                    {
                                        {
                                            "200", new OpenApiResponse()
                                            {
                                                Description = "Success",
                                                Content = new()
                                                {
                                                    {
                                                        applicationJsonMediaType,
                                                        new OpenApiMediaType
                                                        {
                                                            Schema = new OpenApiSchema()
                                                            {
                                                                Type = JsonSchemaType.String | JsonSchemaType.Null
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
                    [logoPath] = new OpenApiPathItem()
                    {
                        Operations = new()
                        {
                            {
                                HttpMethod.Put, new OpenApiOperation
                                {
                                    OperationId = "applications.application.UpdateLogo",
                                    Summary = "Update media content for application in applications",
                                    Responses = new()
                                    {
                                        {
                                            "204", new OpenApiResponse()
                                            {
                                                Description = "Success"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    [securityProfilesPath] = new OpenApiPathItem()
                    {
                        Operations = new()
                        {
                            {
                                HttpMethod.Get, new OpenApiOperation
                                {
                                    OperationId = "security.ListHostSecurityProfiles",
                                    Summary = "Get hostSecurityProfiles from security",
                                    Responses = new()
                                    {
                                        {
                                            "200", new OpenApiResponse()
                                            {
                                                Description = "Retrieved navigation property",
                                                Content = new()
                                                {
                                                    {
                                                        applicationJsonMediaType,
                                                        new OpenApiMediaType
                                                        {
                                                            Schema = new OpenApiSchema()
                                                            {
                                                                Title = "Collection of hostSecurityProfile",
                                                                Type = JsonSchemaType.Object,
                                                                Properties = new()
                                                                {
                                                                    {
                                                                        "value",
                                                                        new OpenApiSchema
                                                                        {
                                                                            Type = JsonSchemaType.Array,
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
                    [communicationsCallsKeepAlivePath] = new OpenApiPathItem()
                    {
                        Operations = new()
                        {
                            {
                                HttpMethod.Post, new OpenApiOperation
                                {
                                    OperationId = "communications.calls.call.keepAlive",
                                    Summary = "Invoke action keepAlive",
                                    Parameters =
                                    [
                                        new OpenApiParameter()
                                        {
                                            Name = "call-id",
                                            In = ParameterLocation.Path,
                                            Description = "key: id of call",
                                            Required = true,
                                            Schema = new OpenApiSchema()
                                            {
                                                Type = JsonSchemaType.String
                                            },
                                            Extensions = new()
                                            {
                                                {
                                                    "x-ms-docs-key-type", new OpenApiAny("call")
                                                }
                                            }
                                        }
                                    ],
                                    Responses = new()
                                    {
                                        {
                                            "204", new OpenApiResponse()
                                            {
                                                Description = "Success"
                                            }
                                        }
                                    },
                                    Extensions = new()
                                    {
                                        {
                                            "x-ms-docs-operation-type", new OpenApiAny("action")
                                        }
                                    }
                                }
                            }
                        }
                    },
                    [eventsDeltaPath] = new OpenApiPathItem()
                    {
                        Operations = new()
                        {
                            {
                                HttpMethod.Get, new OpenApiOperation
                                {
                                    OperationId = "groups.group.events.event.calendar.events.delta",
                                    Summary = "Invoke function delta",
                                    Parameters =
                                    [
                                        new OpenApiParameter()
                                        {
                                            Name = "group-id",
                                            In = ParameterLocation.Path,
                                            Description = "key: id of group",
                                            Required = true,
                                            Schema = new OpenApiSchema()
                                            {
                                                Type = JsonSchemaType.String
                                            },
                                            Extensions = new()
                                            {
                                                {
                                                    "x-ms-docs-key-type", new OpenApiAny("group")
                                                }
                                            }
                                        },
                                        new OpenApiParameter()
                                        {
                                            Name = "event-id",
                                            In = ParameterLocation.Path,
                                            Description = "key: id of event",
                                            Required = true,
                                            Schema = new OpenApiSchema()
                                            {
                                                Type = JsonSchemaType.String
                                            },
                                            Extensions = new()
                                            {
                                                {
                                                    "x-ms-docs-key-type", new OpenApiAny("event")
                                                }
                                            }
                                        }
                                    ],
                                    Responses = new()
                                    {
                                        {
                                            "200", new OpenApiResponse()
                                            {
                                                Description = "Success",
                                                Content = new()
                                                {
                                                    {
                                                        applicationJsonMediaType,
                                                        new OpenApiMediaType
                                                        {
                                                            Schema = new OpenApiSchema()
                                                            {
                                                                Properties = new()
                                                                {
                                                                    {
                                                                        "value",
                                                                        new OpenApiSchema
                                                                        {
                                                                            Type = JsonSchemaType.Array,
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
                                    Extensions = new()
                                    {
                                        {
                                            "x-ms-docs-operation-type", new OpenApiAny("function")
                                        }
                                    }
                                }
                            }
                        }
                    },
                    [refPath] = new OpenApiPathItem()
                    {
                        Operations = new()
                        {
                            {
                                HttpMethod.Get, new OpenApiOperation
                                {
                                    OperationId = "applications.GetRefCreatedOnBehalfOf",
                                    Summary = "Get ref of createdOnBehalfOf from applications"
                                }
                            }
                        }
                    }
                },
                Components = new()
                {
                    Schemas = new()
                    {
                        {
                            "microsoft.graph.networkInterface", new OpenApiSchema
                            {
                                Title = "networkInterface",
                                Type = JsonSchemaType.Object,
                                Properties = new()
                                {
                                    {
                                        "description", new OpenApiSchema
                                        {
                                            Type = JsonSchemaType.String | JsonSchemaType.Null,
                                            Description = "Description of the NIC (e.g. Ethernet adapter, Wireless LAN adapter Local Area Connection <#>, etc.).",
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                Tags = new HashSet<OpenApiTag>
                {
                    new()
                    {
                        Name = "reports.Functions",
                        Description = "The reports.Functions operations"
                    },
                    new()
                    {
                        Name = "users.user",
                        Description = "The users.user operations"
                    },
                    new()
                    {
                        Name = "users.message",
                        Description = "The users.message operations"
                    },
                    new()
                    {
                        Name = "administrativeUnits.Actions",
                        Description = "The administrativeUnits.Actions operations"
                    },
                    new()
                    {
                        Name = "applications.application",
                        Description = "The applications.application operations"
                    },
                    new()
                    {
                        Name = "security.hostSecurityProfile",
                        Description = "The security.hostSecurityProfile operations"
                    },
                    new()
                    {
                        Name = "communications.Actions",
                        Description = "The communications.Actions operations"
                    },
                    new()
                    {
                        Name = "groups.Functions",
                        Description = "The groups.Functions operations"
                    },
                    new()
                    {
                        Name = "applications.directoryObject",
                        Description = "The applications.directoryObject operations"
                    }
                }
            };
            document.Paths[getTeamsActivityByPeriodPath].Operations![HttpMethod.Get].Tags = new HashSet<OpenApiTagReference> {new OpenApiTagReference("reports.Functions", document)};
            document.Paths[getTeamsActivityByDatePath].Operations![HttpMethod.Get].Tags = new HashSet<OpenApiTagReference> {new OpenApiTagReference("reports.Functions", document)};
            document.Paths[usersPath].Operations![HttpMethod.Get].Tags = new HashSet<OpenApiTagReference> {new OpenApiTagReference("users.user", document)};
            document.Paths[usersByIdPath].Operations![HttpMethod.Get].Tags = new HashSet<OpenApiTagReference> {new OpenApiTagReference("users.user", document)};
            document.Paths[usersByIdPath].Operations![HttpMethod.Patch].Tags = new HashSet<OpenApiTagReference> {new OpenApiTagReference("users.user", document)};
            document.Paths[messagesByIdPath].Operations![HttpMethod.Get].Tags = new HashSet<OpenApiTagReference> {new OpenApiTagReference("users.message", document)};
            document.Paths[administrativeUnitRestorePath].Operations![HttpMethod.Post].Tags = new HashSet<OpenApiTagReference> {new OpenApiTagReference("administrativeUnits.Actions", document)};
            document.Paths[logoPath].Operations![HttpMethod.Put].Tags = new HashSet<OpenApiTagReference> {new OpenApiTagReference("applications.application", document)};
            document.Paths[securityProfilesPath].Operations![HttpMethod.Get].Tags = new HashSet<OpenApiTagReference> {new OpenApiTagReference("security.hostSecurityProfile", document)};
            document.Paths[communicationsCallsKeepAlivePath].Operations![HttpMethod.Post].Tags = new HashSet<OpenApiTagReference> {new OpenApiTagReference("communications.Actions", document)};
            document.Paths[eventsDeltaPath].Operations![HttpMethod.Get].Tags = new HashSet<OpenApiTagReference> {new OpenApiTagReference("groups.Functions", document)};
            document.Paths[refPath].Operations![HttpMethod.Get].Tags = new HashSet<OpenApiTagReference> {new OpenApiTagReference("applications.directoryObject", document)};
            ((OpenApiSchema)document.Paths[usersPath].Operations![HttpMethod.Get].Responses!["200"].Content![applicationJsonMediaType].Schema!.Properties!["value"]).Items = new OpenApiSchemaReference("microsoft.graph.user", document);
            document.Paths[usersByIdPath].Operations![HttpMethod.Get].Responses!["200"].Content![applicationJsonMediaType].Schema = new OpenApiSchemaReference("microsoft.graph.user", document);
            document.Paths[messagesByIdPath].Operations![HttpMethod.Get].Responses!["200"].Content![applicationJsonMediaType].Schema = new OpenApiSchemaReference("microsoft.graph.message", document);
            ((OpenApiSchema)document.Paths[securityProfilesPath].Operations![HttpMethod.Get].Responses!["200"].Content![applicationJsonMediaType].Schema!.Properties!["value"]).Items = new OpenApiSchemaReference("microsoft.graph.networkInterface", document);
            ((OpenApiSchema)document.Paths[eventsDeltaPath].Operations![HttpMethod.Get].Responses!["200"].Content![applicationJsonMediaType].Schema!.Properties!["value"]).Items = new OpenApiSchemaReference("microsoft.graph.event", document);
            return document;
        }
    }
}
