// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Defines behavior for comparing properties of <see cref="IList{T}"/>
    /// where T is<see cref="OpenApiServer"/>.
    /// </summary>
    public class OpenApiServersComparer : OpenApiComparerBase<IList<OpenApiServer>>
    {
        /// <summary>
        /// Executes comparision against source and target <see cref="IList{T}"/>
        /// where T is<see cref="OpenApiServer"/>.
        /// </summary>
        /// <param name="sourceServers">The source.</param>
        /// <param name="targetServers">The target.</param>
        /// <param name="comparisonContext">Context under which to compare the source and target.</param>
        public override void Compare(
            IList<OpenApiServer> sourceServers,
            IList<OpenApiServer> targetServers,
            ComparisonContext comparisonContext)
        {
            if (sourceServers == null && targetServers == null)
            {
                return;
            }

            if (sourceServers == null || targetServers == null)
            {
                comparisonContext.AddOpenApiDifference(
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        SourceValue = sourceServers,
                        TargetValue = targetServers,
                        OpenApiComparedElementType = typeof(IList<OpenApiParameter>),
                        Pointer = comparisonContext.PathString
                    });

                return;
            }

            var newServersInTarget = targetServers.Where(
                targetServer => sourceServers.All(sourceServer => sourceServer.Url != targetServer.Url)).ToList();

            for (var i = 0; i < newServersInTarget?.Count; i++)
            {
                WalkAndAddOpenApiDifference(
                    comparisonContext,
                    i.ToString(),
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        TargetValue = newServersInTarget[i],
                        OpenApiComparedElementType = typeof(OpenApiServer)
                    });
            }

            var removedServers = sourceServers.Where(
                sourceServer => targetServers.All(targetServer => sourceServer.Url != targetServer.Url)).ToList();

            for (var i = 0; i < removedServers.Count; i++)
            {
                WalkAndAddOpenApiDifference(
                    comparisonContext,
                    i.ToString(),
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                        SourceValue = removedServers[i],
                        OpenApiComparedElementType = typeof(OpenApiServer)
                    });
            }

            for (var i = 0; i < sourceServers.Count; i++)
            {
                var sourceServer = sourceServers[i];
                var targetServer = targetServers
                    .FirstOrDefault(server => server.Url == sourceServer.Url);

                if (targetServer == null)
                {
                    continue;
                }

                WalkAndCompare(
                    comparisonContext,
                    i.ToString(),
                    () => comparisonContext
                        .GetComparer<OpenApiServer>()
                        .Compare(sourceServer, targetServer, comparisonContext));
            }
        }
    }
}