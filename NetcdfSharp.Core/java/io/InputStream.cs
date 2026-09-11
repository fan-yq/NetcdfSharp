// NetcdfSharp.java.io.InputStream / FilterInputStream / ByteArrayInputStream shims.
// Used by HDF4/HDF5 decompression paths and CBZip2InputStream.
using System;
using System.IO;

namespace NetcdfSharp.java.io
{
    /// <summary>Abstract base for byte input streams. Mirrors NetcdfSharp.java.io.InputStream read() semantics.</summary>
    public abstract class InputStream : System.IO.Stream
    {
        public abstract int read();

        public virtual int read(byte[] b) => read(b, 0, b.Length);

        public virtual int read(byte[] b, int off, int len)
        {
            if (b == null) throw new ArgumentNullException(nameof(b));
            if (off < 0 || len < 0 || off + len > b.Length) throw new IndexOutOfRangeException();
            if (len == 0) return 0;
            int c = read();
            if (c < 0) return -1;
            b[off] = (byte)c;
            int count = 1;
            for (; count < len; count++)
            {
                c = read();
                if (c < 0) break;
                b[off + count] = (byte)c;
            }
            return count;
        }

        public virtual long skip(long n)
        {
            long skipped = 0;
            while (skipped < n)
            {
                int c = read();
                if (c < 0) break;
                skipped++;
            }
            return skipped;
        }

        public virtual int available() => 0;
        public virtual void close() { }
        public virtual void mark(int readlimit) { }
        public virtual void reset() { throw new IOException("mark/reset not supported"); }
        public virtual bool markSupported() => false;

        // ---- System.IO.Stream bridge ----
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }
        public override void Flush() { }
        public override int Read(byte[] buffer, int offset, int count)
        {
            int n = read(buffer, offset, count);
            return n < 0 ? 0 : n;
        }
        public override int ReadByte() => read();
        public override long Seek(long offset, System.IO.SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                close();
            }
            base.Dispose(disposing);
        }
    }

    /// <summary>A filter stream that delegates to an underlying InputStream.</summary>
    public class FilterInputStream : InputStream
    {
        protected internal InputStream @in;
        public FilterInputStream(InputStream @in) { this.@in = @in; }
        public override int read() => @in.read();
        public override int read(byte[] b) => @in.read(b);
        public override int read(byte[] b, int off, int len) => @in.read(b, off, len);
        public override long skip(long n) => @in.skip(n);
        public override int available() => @in.available();
        public override void close() => @in.close();
        public override void mark(int readlimit) => @in.mark(readlimit);
        public override void reset() => @in.reset();
        public override bool markSupported() => @in.markSupported();
    }

    /// <summary>An input stream over a byte array.</summary>
    public class ByteArrayInputStream : InputStream
    {
        private readonly byte[] _buf;
        private int _pos;
        public ByteArrayInputStream(byte[] buf) { _buf = buf ?? throw new ArgumentNullException(nameof(buf)); }
        public ByteArrayInputStream(byte[] buf, int offset, int length)
        {
            _buf = new byte[length];
            System.Array.Copy(buf, offset, _buf, 0, length);
        }
        public override int read() => _pos < _buf.Length ? (_buf[_pos++] & 0xFF) : -1;
        public override int read(byte[] b, int off, int len)
        {
            if (_pos >= _buf.Length) return -1;
            int n = Math.Min(len, _buf.Length - _pos);
            System.Array.Copy(_buf, _pos, b, off, n);
            _pos += n;
            return n;
        }
        public override long skip(long n)
        {
            long k = Math.Min(n, _buf.Length - _pos);
            _pos += (int)k;
            return k;
        }
        public override int available() => _buf.Length - _pos;
        public override void reset() => _pos = 0;
    }

    /// <summary>Adapts a .NET Stream to a NetcdfSharp.java.io.InputStream.</summary>
    public class StreamInputStream : InputStream
    {
        private readonly Stream _s;
        public StreamInputStream(Stream s) { _s = s; }
        public virtual Stream BaseStream2 => _s;
        public override int read() => _s.ReadByte();
        public override int read(byte[] b, int off, int len) => _s.Read(b, off, len);
        public override long skip(long n)
        {
            if (_s.CanSeek) { long p = _s.Position; _s.Position = p + n; return n; }
            return 0;
        }
        public override int available() => (int)Math.Max(0, _s.Length - _s.Position);
        public override void close() => _s.Dispose();
    }
}
