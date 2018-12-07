
using System;
using System.Collections.Generic;

namespace TygaSoft.Model
{
    public class NetRequest
    {
        public NetRequest()
        {
            Method = HttpMethodOptions.Get;
            Parameters = new List<NetParameter>();
        }

        public NetRequest(HttpMethodOptions method) : this()
        {
            Method = method;
        }

        public NetRequest(string resource) : this(resource, HttpMethodOptions.Get)
        {
        }

        public NetRequest(string resource, HttpMethodOptions method) : this()
        {
            Resource = resource;
            Method = method;
        }

        public NetRequest(Uri resource) : this(resource, HttpMethodOptions.Get)
        {
        }

        public NetRequest(Uri resource, HttpMethodOptions method): this(resource.IsAbsoluteUri? resource.AbsolutePath + resource.Query: resource.OriginalString, method)
        {
        }

        public HttpMethodOptions Method { get; set; }

        public string Resource { get; set; }

        public List<NetParameter> Parameters { get; }

        public NetRequest AddParameter(NetParameter p)
        {
            Parameters.Add(p);

            return this;
        }

        public NetRequest AddParameter(string contentType)
        {
            return AddParameter(new NetParameter
            {
                ContentType = contentType,
                ParamsOptions = ParameterOptions.HttpContentHeader
            });
        }

        public NetRequest AddParameter(string name, string value)
        {
            return AddParameter(new NetParameter
            {
                Name = name,
                Value = value,
                ParamsOptions = ParameterOptions.GetOrPost
            });
        }

        public NetRequest AddParameter(string name, string value, ParameterOptions type)
        {
            return AddParameter(new NetParameter
            {
                Name = name,
                Value = value,
                ParamsOptions = type
            });
        }

        public NetRequest AddParameter(string name, string value, string contentType, ParameterOptions type)
        {
            return AddParameter(new NetParameter
            {
                Name = name,
                Value = value,
                ContentType = contentType,
                ParamsOptions = type
            });
        }
    }
}