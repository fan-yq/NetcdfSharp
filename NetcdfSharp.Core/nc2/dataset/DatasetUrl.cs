// NetcdfSharp.ucar.nc2.dataset.DatasetUrl shim: URI protocol helper (subset).
using System;
using System.Collections.Generic;

namespace NetcdfSharp.ucar.nc2.dataset
{
    /// <summary>Parses dataset URLs and extracts protocol list. Subset of NetcdfSharp.ucar.nc2.dataset.DatasetUrl.</summary>
    public class DatasetUrl
    {
        public string location;

        public DatasetUrl(string location)
        {
            this.location = location;
        }

        /// <summary>
        /// Return the protocols from the scheme part of the URL.
        /// e.g. "httpserver:file:/foo.nc" -> ["httpserver", "file"]; "file:/foo.nc" -> ["file"]; "/foo.nc" -> []
        /// </summary>
        public static IList<string> getProtocols(string url)
        {
            var result = new List<string>();
            if (string.IsNullOrEmpty(url)) return result;
            int scheme = url.IndexOf(':');
            if (scheme <= 0) return result;
            string proto = url.Substring(0, scheme).ToLowerInvariant();
            result.Add(proto);
            if (url.Length > scheme + 1 && url[scheme + 1] == '/')
            {
                string rest = url.Substring(scheme + 1);
                int nested = rest.IndexOf(':');
                if (nested > 0)
                {
                    string proto2 = rest.Substring(0, nested).ToLowerInvariant();
                    if (proto2.Length > 0 && proto2.IndexOfAny(new[] { '/', '\\', ' ' }) < 0)
                        result.Add(proto2);
                }
            }
            return result;
        }
    }
}
