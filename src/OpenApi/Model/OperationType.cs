using System;

namespace Tavis.OpenApi.Model
{
    public static class OperationTypeExtensions
    {
        public static OperationType ParseOperationType(string operationType)
        {
            switch (operationType)
            {
                case "get":
                    return OperationType.Get;
                case "put":
                    return OperationType.Put;
                case "post":
                    return OperationType.Post;
                case "delete":
                    return OperationType.Delete;
                case "options":
                    return OperationType.Options;
                case "head":
                    return OperationType.Head;
                case "patch":
                    return OperationType.Patch;
                case "trace":
                    return OperationType.Trace;
                default:
                    throw new ArgumentException();
            }
        }

        public static string GetOperationTypeName(this OperationType operationType)
        {
            switch (operationType)
            {
                case OperationType.Get:
                    return "get";
                case OperationType.Put:
                    return "put";
                case OperationType.Post:
                    return "post";
                case OperationType.Delete:
                    return "delete";
                case OperationType.Options:
                    return "options";
                case OperationType.Head:
                    return "head";
                case OperationType.Patch:
                    return "patch";
                case OperationType.Trace:
                    return "trace";
                default:
                    throw new ArgumentException();
            }
        }
    }

    public enum OperationType
    {
        Get,
        Put,
        Post,
        Delete,
        Options,
        Head,
        Patch,
        Trace
    }
}
