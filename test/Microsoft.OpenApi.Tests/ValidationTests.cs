// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Services;
using Xunit;

namespace Microsoft.OpenApi.Tests
{
    public class ValidationTests
    {
        [Fact]
        public void ResponseMustHaveADescription()
        {
            var doc = new OpenApiDocument();
            doc.AddPathItem("/test",
                p => p.AddOperation(OperationType.Get,
                    o => o.AddResponse("200",
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
