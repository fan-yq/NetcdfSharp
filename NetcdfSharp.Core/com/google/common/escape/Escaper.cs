// Guava (NetcdfSharp.com.google.common.escape) compatible helpers - minimal subset used by NetcdfSharp.
using System;
using System.Text;

namespace NetcdfSharp.com.google.common.escape
{
    /// <summary>
    /// Simplified replacement for NetcdfSharp.com.google.common.escape.Escaper.
    /// </summary>
    public abstract class Escaper
    {
        public abstract string escape(string s);
    }
}
