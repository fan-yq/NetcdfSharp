// java.net URL/URI shims and NetcdfSharp.java.util.TimeZone shim for netcdf-java translation.
using System;
using System.IO;

namespace NetcdfSharp.java.net
{
    /// <summary>A URL for a resource. Minimal subset used by netcdf-java.</summary>
    public class URL
    {
        private readonly Uri _uri;
        public URL(string spec) { _uri = new Uri(spec); }
        public URL(string protocol, string host, int port, string file)
        {
            string s = protocol + "://" + host + (port > 0 ? ":" + port : "") + file;
            _uri = new Uri(s);
        }
        public string getProtocol() => _uri.Scheme;
        public string getHost() => _uri.Host;
        public int getPort() => _uri.Port;
        public int getDefaultPort() => _uri.IsDefaultPort ? _uri.Port : -1;
        public string getPath() => _uri.AbsolutePath;
        public string getFile() => _uri.AbsolutePath + (_uri.Query ?? "");
        public string getQuery() => _uri.Query;
        public string getAuthority() => _uri.Authority;
        public string getRef() => _uri.Fragment;
        public string toExternalForm() => _uri.AbsoluteUri;
        public override string ToString() => _uri.AbsoluteUri;
        public override bool Equals(object obj) => obj is URL u && u._uri == _uri;
        public override int GetHashCode() => _uri?.GetHashCode() ?? 0;
        public URLConnection openConnection() => new NetcdfSharp.java.net.HttpURLConnection(this);
        public Stream openStream() => openConnection().getInputStream();
        public object getContent() => openConnection().getInputStream();
    }

    /// <summary>NetcdfSharp.java.net.URLDecoder - minimal subset.</summary>
    public static class URLDecoder
    {
        public static string decode(string s, string charset)
        {
            return System.Uri.UnescapeDataString(s ?? "");
        }
    }

    /// <summary>A URI for a resource. Minimal subset.</summary>
    public class URI
    {
        private readonly Uri _uri;
        public URI(string str) { _uri = new Uri(str); }
        public static URI create(string str) => new URI(str);
        public static URI create(string scheme, string ssp) => new URI(scheme + ":" + ssp);
        public string getScheme() => _uri.Scheme;
        public string getHost() => _uri.Host;
        public string getPath() => _uri.AbsolutePath;
        public string getQuery() => _uri.Query;
        public string getAuthority() => _uri.Authority;
        public string getRef() => _uri.Fragment;
        public bool isAbsolute() => _uri.IsAbsoluteUri;
        public URI resolve(string relative) => new URI(new Uri(_uri, relative).AbsoluteUri);
        public string toASCIIString() => _uri.AbsoluteUri;
        public URL toURL() => new URL(_uri.AbsoluteUri);
        public override string ToString() => _uri.AbsoluteUri;
        public override bool Equals(object obj) => obj is URI u && u._uri == _uri;
        public override int GetHashCode() => _uri?.GetHashCode() ?? 0;
    }
}

namespace NetcdfSharp.java.util
{
    /// <summary>A time zone. Minimal subset (UTC, fixed offsets).</summary>
    public class JTimeZone
    {
        private readonly TimeSpan _offset;
        private readonly string _id;
        public JTimeZone(TimeSpan offset, string id) { _offset = offset; _id = id; }
        public static readonly JTimeZone UTC = new JTimeZone(TimeSpan.Zero, "UTC");
        public static JTimeZone getDefault() => UTC;
        public static JTimeZone getTimeZone(string id) => id == "GMT" || id == "UTC" ? UTC : new JTimeZone(TimeSpan.Zero, id);
        public static string[] getAvailableIDs() => new[] { "UTC", "GMT" };
        public string getID() => _id;
        public TimeSpan rawOffset() => _offset;
    }
}
