// NetcdfSharp.java.io.BufferedInputStream / BufferedOutputStream shims for netcdf-java translation.
using System;
using System.IO;

namespace NetcdfSharp.java.io
{
    /// <summary>
    /// Simplified replacement for NetcdfSharp.java.io.BufferedInputStream.
    /// Wraps a Stream and exposes it as a Stream (buffering is delegated to .NET).
    /// </summary>
    public class BufferedInputStream : Stream
    {
        private readonly Stream _inner;

        public BufferedInputStream(Stream inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public BufferedInputStream(Stream inner, int bufferSize) : this(inner) { }

        public BufferedInputStream(InputStream inner)
        {
            _inner = new InputStreamAdapter(inner);
        }

        public override bool CanRead => _inner.CanRead;
        public override bool CanSeek => _inner.CanSeek;
        public override bool CanWrite => _inner.CanWrite;
        public override long Length => _inner.Length;

        public override long Position
        {
            get => _inner.Position;
            set => _inner.Position = value;
        }

        public override void Flush() => _inner.Flush();
        public override int Read(byte[] buffer, int offset, int count) => _inner.Read(buffer, offset, count);
        public override int ReadByte() => _inner.ReadByte();
        public override long Seek(long offset, SeekOrigin origin) => _inner.Seek(offset, origin);
        public override void SetLength(long value) => _inner.SetLength(value);
        public override void Write(byte[] buffer, int offset, int count) => _inner.Write(buffer, offset, count);
        public override void WriteByte(byte value) => _inner.WriteByte(value);

        public override void Close()
        {
            base.Close();
            _inner.Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _inner.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// Simplified replacement for NetcdfSharp.java.io.BufferedOutputStream.
    /// Wraps a Stream and exposes it as a Stream.
    /// </summary>
    public class BufferedOutputStream : Stream
    {
        private readonly Stream _inner;

        public BufferedOutputStream(Stream inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public override bool CanRead => _inner.CanRead;
        public override bool CanSeek => _inner.CanSeek;
        public override bool CanWrite => _inner.CanWrite;
        public override long Length => _inner.Length;

        public override long Position
        {
            get => _inner.Position;
            set => _inner.Position = value;
        }

        public override void Flush() => _inner.Flush();
        public override int Read(byte[] buffer, int offset, int count) => _inner.Read(buffer, offset, count);
        public override long Seek(long offset, SeekOrigin origin) => _inner.Seek(offset, origin);
        public override void SetLength(long value) => _inner.SetLength(value);
        public override void Write(byte[] buffer, int offset, int count) => _inner.Write(buffer, offset, count);
        public override void WriteByte(byte value) => _inner.WriteByte(value);

        public override void Close()
        {
            base.Close();
            _inner.Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _inner.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    /// <summary>Adapts a NetcdfSharp.java.io.InputStream to a System.IO.Stream.</summary>
    internal class InputStreamAdapter : Stream
    {
        private readonly InputStream _inner;

        public InputStreamAdapter(InputStream inner)
        {
            _inner = inner;
        }

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
        public override int Read(byte[] buffer, int offset, int count) => _inner.read(buffer, offset, count);
        public override int ReadByte() => _inner.read();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}
