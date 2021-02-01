using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Diff.BusinessObjects
{
    public class DiffContextBO
    {
        public string URL { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public OperationType Method { get; private set; }
        public bool IsResponse { get; private set; }
        public bool IsRequest { get; private set; }

        public bool IsRequired { get; private set; }

        public DiffContextBO()
        {
            Parameters = new Dictionary<string, string>();
            IsResponse = false;
            IsRequest = true;
        }

        public string GetDiffContextElementType() => IsResponse ? "Response" : "Request";


        public DiffContextBO CopyWithMethod(OperationType method)
        {
            var result = Copy();
            result.Method = method;
            return result;
        }

        public DiffContextBO CopyWithRequired(bool required)
        {
            var result = Copy();
            result.IsRequired = required;
            return result;
        }

        public DiffContextBO CopyAsRequest()
        {
            var result = Copy();
            result.IsRequest = true;
            result.IsResponse = false;
            return result;
        }

        public DiffContextBO CopyAsResponse()
        {
            var result = Copy();
            result.IsResponse = true;
            result.IsRequest = false;
            return result;
        }

        private DiffContextBO Copy()
        {
            var context = new DiffContextBO
            {
                URL = URL,
                Parameters = Parameters,
                Method = Method,
                IsResponse = IsResponse,
                IsRequest = IsRequest,
                IsRequired = IsRequired
            };
            return context;
        }
        public override bool Equals(object o)
        {
            if (this == o) return true;

            if (o == null || GetType() != o.GetType()) return false;

            var that = (DiffContextBO)o;

            return IsResponse.Equals(that.IsResponse)
                   && IsRequest.Equals(that.IsRequest)
                   && URL.Equals(that.URL)
                   && Parameters.Equals(that.Parameters)
                   && Method.Equals(that.Method)
                   && IsRequired.Equals(that.IsRequired);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(URL, Parameters, Method, IsResponse, IsRequest, IsRequired);
        }
    }
}
