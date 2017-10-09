using Microsoft.OpenApi;
using Microsoft.OpenApi.Sevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenApi.Tests
{
    public class ValidationTests
    {
        [Fact]
        public void ResponseMustHaveADescription()
        {
            var doc = new OpenApiDocument();
            doc.CreatePath("/test", 
                p => p.CreateOperation(OperationType.Get, 
                    o => o.CreateResponse("200", 
                        r => { }
                   )
               )
             );
            var validator = new OpenApiValidator();
            var walker = new OpenApiWalker(validator);
            walker.Walk(doc);

            Assert.Single(validator.Exceptions);
        }
    }
}
