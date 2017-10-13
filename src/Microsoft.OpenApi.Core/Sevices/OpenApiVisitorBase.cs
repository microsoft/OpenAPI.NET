using System.Collections.Generic;

namespace Microsoft.OpenApi
{
    public abstract class OpenApiVisitorBase
    {
        public virtual void Visit(OpenApiDocument doc) { }
        public virtual void Visit(Info info) { }
        public virtual void Visit(List<Server> servers) { }
        public virtual void Visit(Server server) { }
        public virtual void Visit(Paths paths) { }
        public virtual void Visit(PathItem pathItem) { }
        public virtual void Visit(ServerVariable serverVariable) { }
        public virtual void Visit(IReadOnlyDictionary<string,Operation> operations) { }
        public virtual void Visit(Operation operation) { }
        public virtual void Visit(List<Parameter> parameters) { }
        public virtual void Visit(Parameter parameter) { }
        public virtual void Visit(RequestBody requestBody) { }
        public virtual void Visit(IReadOnlyDictionary<string,Response> responses) { }
        public virtual void Visit(Response response) { }
        public virtual void Visit(Dictionary<string,MediaType> content) { }
        public virtual void Visit(MediaType mediaType) { }
        public virtual void Visit(Dictionary<string,Example> example) { }
        public virtual void Visit(Schema example) { }
        public virtual void Visit(Dictionary<string,Link> links) { }
        public virtual void Visit(Link link) { }
    }
}
