using System;
using System.Collections.Generic;

namespace TygaSoft.Model
{
    public class NetResponse
    {
        private string _content;

        public bool IsSuccessful { get; set; }

        public Uri ResponseUri { get; set; }

        public string ContentType { get; set; }

        public long? ContentLength { get; set; }

        public string ContentEncoding { get; set; }

        public string Content => this._content ?? (this._content = this.RawBytes.AsString().Result);

        public byte[] RawBytes { get; set; }

        public string CookieAppend { get; set; }
    }
}