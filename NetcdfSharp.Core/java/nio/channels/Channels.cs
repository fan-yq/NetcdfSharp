// NetcdfSharp.java.nio.channels shims for netcdf-java translation.
using System;

namespace NetcdfSharp.java.nio.channels
{
    /// <summary>A byte channel that can write bytes. Minimal NetcdfSharp.java.nio.channels.WritableByteChannel subset.</summary>
    public interface WritableByteChannel
    {
        int write(NetcdfSharp.java.nio.ByteBuffer src);
    }

    /// <summary>A byte channel that can read and write. Minimal NetcdfSharp.java.nio.channels.FileChannel subset.</summary>
    public class FileChannel : WritableByteChannel
    {
        private readonly System.IO.Stream _stream;

        public FileChannel(System.IO.Stream stream)
        {
            _stream = stream;
        }

        public static FileChannel open(string path, string mode)
        {
            System.IO.FileMode fm = mode.StartsWith("w") ? System.IO.FileMode.Create
                : mode.StartsWith("rw") ? System.IO.FileMode.OpenOrCreate
                : System.IO.FileMode.Open;
            System.IO.FileAccess fa = mode.StartsWith("r") && mode.IndexOf('w') < 0
                ? System.IO.FileAccess.Read : System.IO.FileAccess.ReadWrite;
            var fs = new System.IO.FileStream(path, fm, fa);
            return new FileChannel(fs);
        }

        public virtual int write(NetcdfSharp.java.nio.ByteBuffer src)
        {
            byte[] arr = src.array();
            int pos = src.position();
            int remaining = src.remaining();
            _stream.Write(arr, pos, remaining);
            src.position(pos + remaining);
            return remaining;
        }

        public virtual long size() => _stream.CanSeek ? _stream.Length : 0;

        public virtual FileChannel position(long newPos)
        {
            if (_stream.CanSeek) _stream.Position = newPos;
            return this;
        }

        public virtual void truncate(long size)
        {
            _stream.SetLength(size);
        }

        public virtual void force(bool metaData) => _stream.Flush();

        public virtual int read(NetcdfSharp.java.nio.ByteBuffer dst)
        {
            byte[] arr = dst.array();
            int pos = dst.position();
            int remaining = dst.remaining();
            int n = _stream.Read(arr, pos, remaining);
            if (n > 0) dst.position(pos + n);
            return n;
        }

        /// <summary>Transfers up to count bytes from this channel to the target channel (java NIO transferTo).</summary>
        public virtual long transferTo(long position, long count, WritableByteChannel target)
        {
            if (!_stream.CanSeek) throw new NotSupportedException("transferTo requires a seekable channel");
            long saved = _stream.Position;
            try
            {
                _stream.Position = position;
                byte[] buffer = new byte[(int)Math.Min(count, 8192)];
                long total = 0;
                while (total < count)
                {
                    int want = (int)Math.Min(buffer.Length, count - total);
                    int n = _stream.Read(buffer, 0, want);
                    if (n <= 0) break;
                    var bb = NetcdfSharp.java.nio.ByteBuffer.wrap(buffer, 0, n);
                    target.write(bb);
                    total += n;
                }
                return total;
            }
            finally
            {
                _stream.Position = saved;
            }
        }

        public virtual void close() => _stream.Dispose();

        public void Dispose() => close();
    }

    /// <summary>Bridges a System.IO.Stream to a FileChannel (java FileInputStream.getChannel()).</summary>
    public static class ChannelsBridge
    {
        public static FileChannel getChannel(this System.IO.Stream stream)
        {
            return new FileChannel(stream);
        }
    }

    /// <summary>NetcdfSharp.java.nio.channels.Channels static helpers.</summary>
    public static class Channels
    {
        /// <summary>Constructs a stream that writes bytes to the given channel.</summary>
        public static System.IO.Stream newOutputStream(WritableByteChannel channel)
        {
            return new ChannelOutputStream(channel);
        }

        private sealed class ChannelOutputStream : System.IO.Stream
        {
            private readonly WritableByteChannel _channel;
            public ChannelOutputStream(WritableByteChannel channel) { _channel = channel; }
            public override bool CanRead => false;
            public override bool CanSeek => false;
            public override bool CanWrite => true;
            public override long Length => throw new NotSupportedException();
            public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
            public override void Flush() { }
            public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
            public override long Seek(long offset, System.IO.SeekOrigin origin) => throw new NotSupportedException();
            public override void SetLength(long value) => throw new NotSupportedException();
            public override void Write(byte[] buffer, int offset, int count)
            {
                _channel.write(NetcdfSharp.java.nio.ByteBuffer.wrap(buffer, offset, count));
            }
        }
    }
}
