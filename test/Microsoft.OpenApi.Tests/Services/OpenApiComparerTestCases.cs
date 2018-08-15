// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Services;

namespace Microsoft.OpenApi.Tests.Services
{
    internal static class OpenApiComparerTestCases
    {
        public static IEnumerable<object[]> GetTestCasesForOpenApiComparerShouldSucceed()
        {
            // New and removed paths
            yield return new object[]
            {
                "New And Removed Paths",
                new OpenApiDocument
                {
                    Paths = new OpenApiPaths
                    {
                        {
                            "/test", new OpenApiPathItem
                            {
                                Summary = "test",
                                Description = "test",
                                Operations = new Dictionary<OperationType, OpenApiOperation>
                                {
                                    {
                                        OperationType.Get, new OpenApiOperation()
                                    }
                                }
                            }
                        }
                    }
                },
                new OpenApiDocument
                {
                    Paths = new OpenApiPaths
                    {
                        {
                            "/newPath", new OpenApiPathItem
                            {
                                Summary = "test",
                                Description = "test",
                                Operations = new Dictionary<OperationType, OpenApiOperation>
                                {
                                    {
                                        OperationType.Get, new OpenApiOperation()
                                    }
                                }
                            }
                        }
                    }
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference()
                    {
                        Pointer = "#/paths/~1newPath",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        OpenApiComparedElementType = typeof(OpenApiPathItem)
                    },
                    new OpenApiDifference()
                    {
                        Pointer = "#/paths/~1test",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                        OpenApiComparedElementType = typeof(OpenApiPathItem)
                    }
                }
            };

            // New and removed operations
            yield return new object[]
            {
                "New And Removed Operations",
                new OpenApiDocument
                {
                    Paths = new OpenApiPaths
                    {
                        {
                            "/test", new OpenApiPathItem
                            {
                                Summary = "test",
                                Description = "test",
                                Operations = new Dictionary<OperationType, OpenApiOperation>
                                {
                                    {
                                        OperationType.Get, new OpenApiOperation()
                                    },
                                    {
                                        OperationType.Post, new OpenApiOperation()
                                    }
                                }
                            }
                        }
                    }
                },
                new OpenApiDocument
                {
                    Paths = new OpenApiPaths
                    {
                        {
                            "/test", new OpenApiPathItem
                            {
                                Summary = "test",
                                Description = "test",
                                Operations = new Dictionary<OperationType, OpenApiOperation>
                                {
                                    {
                                        OperationType.Get, new OpenApiOperation()
                                    },
                                    {
                                        OperationType.Patch, new OpenApiOperation()
                                    }
                                }
                            }
                        }
                    }
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference()
                    {
                        Pointer = "#/paths/~1test/patch",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        OpenApiComparedElementType = typeof(KeyValuePair<OperationType,OpenApiOperation>)
                    },
                    new OpenApiDifference()
                    {
                        Pointer = "#/paths/~1test/post",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                        OpenApiComparedElementType = typeof(KeyValuePair<OperationType,OpenApiOperation>)
                    }
                }
            };

            // Empty target document paths
            yield return new object[]
            {
                "Empty target document",
                new OpenApiDocument
                {
                    Paths = new OpenApiPaths
                    {
                        {
                            "/test", new OpenApiPathItem
                            {
                                Summary = "test",
                                Description = "test",
                                Operations = new Dictionary<OperationType, OpenApiOperation>
                                {
                                    {
                                        OperationType.Get, new OpenApiOperation()
                                    }
                                }
                            }
                        }
                    }
                },
                new OpenApiDocument(),
                new List<OpenApiDifference>
                {
                    new OpenApiDifference()
                    {
                        Pointer = "#/paths/~1test",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                        OpenApiComparedElementType = typeof(OpenApiPathItem)
                    }
                }
            };

            // Empty source document
            yield return new object[]
            {
                "Empty source document",
                new OpenApiDocument(),
                new OpenApiDocument
                {
                    Paths = new OpenApiPaths
                    {
                        {
                            "/newPath", new OpenApiPathItem
                            {
                                Summary = "test",
                                Description = "test",
                                Operations = new Dictionary<OperationType, OpenApiOperation>
                                {
                                    {
                                        OperationType.Get, new OpenApiOperation()
                                    }
                                }
                            }
                        }
                    }
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference()
                    {
                        Pointer = "#/paths/~1newPath",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        OpenApiComparedElementType = typeof(OpenApiPathItem)
                    }
                }
            };

            // Empty target operations
            yield return new object[]
            {
                "Empty target operations",
                new OpenApiDocument
                {
                    Paths = new OpenApiPaths
                    {
                        {
                            "/test", new OpenApiPathItem
                            {
                                Summary = "test",
                                Description = "test",
                                Operations = new Dictionary<OperationType, OpenApiOperation>
                                {
                                    {
                                        OperationType.Get, new OpenApiOperation()
                                    },
                                    {
                                        OperationType.Post, new OpenApiOperation()
                                    }
                                }
                            }
                        }
                    }
                },
                new OpenApiDocument
                {
                    Paths = new OpenApiPaths
                    {
                        {
                            "/test", new OpenApiPathItem
                            {
                                Summary = "test",
                                Description = "test",
                                Operations = new Dictionary<OperationType, OpenApiOperation>()
                            }
                        }
                    }
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference()
                    {
                        Pointer = "#/paths/~1test/get",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                        OpenApiComparedElementType = typeof(KeyValuePair<OperationType,OpenApiOperation>)
                    },
                    new OpenApiDifference()
                    {
                        Pointer = "#/paths/~1test/post",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                        OpenApiComparedElementType = typeof(KeyValuePair<OperationType,OpenApiOperation>)
                    }
                }
            };

            // Empty source operations
            yield return new object[]
            {
                "Empty source operations",
                new OpenApiDocument
                {
                    Paths = new OpenApiPaths
                    {
                        {
                            "/test", new OpenApiPathItem
                            {
                                Summary = "test",
                                Description = "test",
                                Operations = new Dictionary<OperationType, OpenApiOperation>()
                            }
                        }
                    }
                },
                new OpenApiDocument
                {
                    Paths = new OpenApiPaths
                    {
                        {
                            "/test", new OpenApiPathItem
                            {
                                Summary = "test",
                                Description = "test",
                                Operations = new Dictionary<OperationType, OpenApiOperation>
                                {
                                    {
                                        OperationType.Get, new OpenApiOperation()
                                    },
                                    {
                                        OperationType.Patch, new OpenApiOperation()
                                    }
                                }
                            }
                        }
                    }
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference()
                    {
                        Pointer = "#/paths/~1test/get",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        OpenApiComparedElementType = typeof(KeyValuePair<OperationType,OpenApiOperation>)
                    },
                    new OpenApiDifference()
                    {
                        Pointer = "#/paths/~1test/patch",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        OpenApiComparedElementType = typeof(KeyValuePair<OperationType,OpenApiOperation>)
                    }
                }
            };

            // Identical source and target
            yield return new object[]
            {
                "Identical source and target documents",
                new OpenApiDocument
                {
                    Paths = new OpenApiPaths
                    {
                        {
                            "/test", new OpenApiPathItem
                            {
                                Summary = "test",
                                Description = "test",
                                Operations = new Dictionary<OperationType, OpenApiOperation>
                                {
                                    {
                                        OperationType.Get, new OpenApiOperation()
                                    },
                                    {
                                        OperationType.Post, new OpenApiOperation()
                                    }
                                }
                            }
                        }
                    }
                },
                new OpenApiDocument
                {
                    Paths = new OpenApiPaths
                    {
                        {
                            "/test", new OpenApiPathItem
                            {
                                Summary = "test",
                                Description = "test",
                                Operations = new Dictionary<OperationType, OpenApiOperation>
                                {
                                    {
                                        OperationType.Get, new OpenApiOperation()
                                    },
                                    {
                                        OperationType.Post, new OpenApiOperation()
                                    }
                                }
                            }
                        }
                    }
                },
                new List<OpenApiDifference>()
            };

            // Differences in summary and description
            yield return new object[]
            {
                "Differences in summary and description",
                new OpenApiDocument
                {
                    Paths = new OpenApiPaths
                    {
                        {
                            "/test", new OpenApiPathItem
                            {
                                Summary = "test",
                                Description = "test",
                                Operations = new Dictionary<OperationType, OpenApiOperation>
                                {
                                    {
                                        OperationType.Get, new OpenApiOperation()
                                    },
                                    {
                                        OperationType.Post, new OpenApiOperation()
                                    }
                                }
                            }
                        }
                    }
                },
                new OpenApiDocument
                {
                    Paths = new OpenApiPaths
                    {
                        {
                            "/test", new OpenApiPathItem
                            {
                                Summary = "updated",
                                Description = "updated",
                                Operations = new Dictionary<OperationType, OpenApiOperation>
                                {
                                    {
                                        OperationType.Get, new OpenApiOperation()
                                    },
                                    {
                                        OperationType.Post, new OpenApiOperation()
                                    }
                                }
                            }
                        }
                    }
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference()
                    {
                        Pointer = "#/paths/~1test/summary",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(string)
                    },
                    new OpenApiDifference()
                    {
                        Pointer = "#/paths/~1test/description",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(string)
                    }
                }
            };
        }
    }
}