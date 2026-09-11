// NetcdfSharp.java.io.Writer shim for netcdf-java translation.
using System;
using System.Text;
using System.Threading.Tasks;

namespace NetcdfSharp.java.io
{
    /// <summary>
    /// Abstract class for writing to character streams. Mirrors the subset of
    /// NetcdfSharp.java.io.Writer used by netcdf-java.
    /// </summary>
    public abstract class Writer : IDisposable
    {
        public virtual void write(int c) { }
        public virtual void write(string str)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
            char[] chars = str.ToCharArray();
            write(chars, 0, chars.Length);
        }
        public virtual void write(char[] cbuf) => write(cbuf, 0, cbuf.Length);
        public abstract void write(char[] cbuf, int off, int len);
        public virtual void write(string str, int off, int len) => write(str.Substring(off, len));
        public virtual Writer append(char c) { write(c); return this; }
        public virtual Writer append(string csq)
        {
            if (csq != null) write(csq);
            return this;
        }
        public virtual void flush() { }
        public virtual void close() => Dispose(true);
        protected virtual void Dispose(bool disposing) { }
        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
    }
}
