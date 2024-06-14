using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Diff.BusinessObjects;
using Microsoft.OpenApi.Diff.Extensions;
using Microsoft.OpenApi.Diff.Utils;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Diff.Compare
{
    public class OperationDiff
    {
        private readonly OpenApiDiff _openApiDiff;

        public OperationDiff(OpenApiDiff openApiDiff)
        {
            _openApiDiff = openApiDiff;
        }

        public ChangedOperationBO Diff(
            OpenApiOperation oldOperation, OpenApiOperation newOperation, DiffContextBO context)
        {
            var changedOperation =
                new ChangedOperationBO(context.URL, context.Method, oldOperation, newOperation)
                {
                    Summary = _openApiDiff
                        .MetadataDiff
                        .Diff(oldOperation.Summary, newOperation.Summary, context),
                    Description = _openApiDiff
                        .MetadataDiff
                        .Diff(oldOperation.Description, newOperation.Description, context),
                    IsDeprecated = !oldOperation.Deprecated && newOperation.Deprecated
                };

            if (oldOperation.RequestBody != null || newOperation.RequestBody != null)
                changedOperation.RequestBody = _openApiDiff
                    .RequestBodyDiff
                    .Diff(
                        oldOperation.RequestBody, newOperation.RequestBody, context.CopyAsRequest());

            var parametersDiff = _openApiDiff
                .ParametersDiff
                .Diff(oldOperation.Parameters.ToList(), newOperation.Parameters.ToList(), context);

            if (parametersDiff != null)
            {
                RemovePathParameters(context.Parameters, parametersDiff);
                changedOperation.Parameters = parametersDiff;
            }


            if (oldOperation.Responses != null || newOperation.Responses != null)
            {

                var diff = _openApiDiff
                    .APIResponseDiff
                    .Diff(oldOperation.Responses, newOperation.Responses, context.CopyAsResponse());

                if (diff != null)
                    changedOperation.APIResponses = diff;
            }

            if (oldOperation.Security != null || newOperation.Security != null)
            {
                var diff = _openApiDiff
                    .SecurityRequirementsDiff
                    .Diff(oldOperation.Security, newOperation.Security, context);

                if (diff != null)
                    changedOperation.SecurityRequirements = diff;
            }

            changedOperation.Extensions =
                _openApiDiff
                    .ExtensionsDiff
                    .Diff(oldOperation.Extensions, newOperation.Extensions, context);

            return ChangedUtils.IsChanged(changedOperation);
        }

        public void RemovePathParameters(Dictionary<string, string> pathParameters, ChangedParametersBO parameters)
        {
            foreach (var (oldParam, newParam) in pathParameters)
            {
                RemovePathParameter(oldParam, parameters.Missing);
                RemovePathParameter(newParam, parameters.Increased);
            }
        }

        public void RemovePathParameter(string name, List<OpenApiParameter> parameters)
        {
            var openApiParameters = parameters
                .FirstOrDefault(x => x.In == ParameterLocation.Path && x.Name == name);
            if (!parameters.IsNullOrEmpty() && openApiParameters != null)
                parameters.Remove(openApiParameters);
        }
    }
}
