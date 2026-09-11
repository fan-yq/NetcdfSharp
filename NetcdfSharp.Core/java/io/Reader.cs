// NetcdfSharp.java.io.Reader shim for netcdf-java translation.
using System;

namespace NetcdfSharp.java.io
{
    /// <summary>Abstract class for reading character streams. Minimal subset.</summary>
    public abstract class Reader : IDisposable
    {
        public virtual int read() => -1;
        public virtual int read(char[] cbuf) => read(cbuf, 0, cbuf.Length);
        public abstract int read(char[] cbuf, int off, int len);
        public virtual long skip(long n) => 0;
        public virtual bool ready() => false;
        public virtual void close() => Dispose(true);
        protected virtual void Dispose(bool disposing) { }
        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
    }
}
