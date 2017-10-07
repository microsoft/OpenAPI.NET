
namespace Microsoft.OpenApi
{

    using System.Collections.Generic;
    using System.Linq;

    public class PathItem 
    {

        public string Summary { get; set; }
        public string Description { get; set; }

        public IReadOnlyDictionary<string, Operation> Operations { get { return operations; } }
        private Dictionary<string, Operation> operations = new Dictionary<string, Operation>();

        public List<Server> Servers { get; set; } = new List<Server>();
        public List<Parameter> Parameters { get; set; } = new List<Parameter>();
        public Dictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        public void AddOperation(OperationType operationType, Operation operation)
        {
            var successResponse = operation.Responses.Keys.Where(k => k.StartsWith("2")).Any();
            if (!successResponse)
            {
             throw new DomainParseException("An operation requires a successful response");
            }


            this.operations.Add(operationType.GetOperationTypeName(), operation);
        }


    }
}