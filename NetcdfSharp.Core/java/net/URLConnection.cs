// java.net URLConnection / HttpURLConnection minimal shims.
// HTTP access is not supported in NetcdfSharp; these types exist so the translated
// remote-read methods in IO.cs compile. Calling them throws NotSupportedException.
using System;
using System.Collections.Generic;
using System.IO;

namespace NetcdfSharp.java.net
{
    /// <summary>Abstract URL connection. All operations throw NotSupportedException (no HTTP in NetcdfSharp).</summary>
    public abstract class URLConnection
    {
        protected URL _url;
        private readonly Dictionary<string, List<string>> _properties = new Dictionary<string, List<string>>();

        protected URLConnection(URL url) { _url = url; }

        public virtual URL getURL() => _url;
        public virtual void addRequestProperty(string key, string value)
        {
            if (!_properties.TryGetValue(key, out var list)) { list = new List<string>(); _properties[key] = list; }
            list.Add(value);
        }
        public virtual void setRequestProperty(string key, string value)
        {
            _properties[key] = new List<string> { value };
        }
        public virtual IDictionary<string, List<string>> getRequestProperties() => _properties;
        public virtual string getHeaderField(int n) => null;
        public virtual string getHeaderFieldKey(int n) => null;
        public virtual string getContentEncoding() => null;
        public virtual Stream getInputStream() => throw new NotSupportedException("HTTP access is not supported in NetcdfSharp.");
        public virtual Stream getOutputStream() => throw new NotSupportedException("HTTP access is not supported in NetcdfSharp.");
        public virtual void connect() => throw new NotSupportedException("HTTP access is not supported in NetcdfSharp.");
    }

    /// <summary>HTTP-specific URL connection. Not supported in NetcdfSharp.</summary>
    public class HttpURLConnection : URLConnection
    {
        public HttpURLConnection(URL url) : base(url) { }
        public virtual void setRequestMethod(string method) { }
        public virtual void setDoOutput(bool doOutput) { }
        public virtual void setDoInput(bool doInput) { }
        public virtual int getResponseCode() => throw new NotSupportedException("HTTP access is not supported in NetcdfSharp.");
        public virtual string getResponseMessage() => throw new NotSupportedException("HTTP access is not supported in NetcdfSharp.");
    }
}
