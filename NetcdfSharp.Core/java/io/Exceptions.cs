// java.io exception shims for netcdf-java translation.
// IOException/EOFException are now aliased directly to System.IO.IOException/EndOfStreamException.
using System;

namespace NetcdfSharp.java.io
{
    /// <summary>Signals that a UTF-8 encoded string is malformed.</summary>
    public class UTFDataFormatException : System.IO.IOException
    {
        public UTFDataFormatException() { }
        public UTFDataFormatException(string message) : base(message) { }
        public UTFDataFormatException(string message, Exception inner) : base(message, inner) { }
    }
}
