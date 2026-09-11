// NetcdfSharp.java.util.zip.Deflater shim for netcdf-java translation.
// Minimal subset used by the translated code: setInput / finish / deflate / end.
using System;
using System.IO;
using System.IO.Compression;

namespace NetcdfSharp.java.util.zip
{
    /// <summary>
    /// Simplified replacement for NetcdfSharp.java.util.zip.Deflater.
    /// Compresses a single input buffer using raw DEFLATE.
    /// </summary>
    public class Deflater
    {
        private byte[] _input;
        private int _inputOffset;
        private int _inputLength;
        private bool _finished;
        private bool _ended;

        public Deflater()
        {
        }

        public Deflater(int level)
        {
        }

        public virtual void setInput(byte[] input)
        {
            setInput(input, 0, input == null ? 0 : input.Length);
        }

        public virtual void setInput(byte[] input, int offset, int len)
        {
            _input = input;
            _inputOffset = offset;
            _inputLength = len;
        }

        public virtual void setLevel(int level)
        {
        }

        public virtual void finish()
        {
            _finished = true;
        }

        public virtual bool finished()
        {
            return _finished;
        }

        /// <summary>
        /// Compress the input into <paramref name="output"/>. Returns the number of bytes written.
        /// </summary>
        public virtual int deflate(byte[] output)
        {
            if (_ended)
            {
                throw new ObjectDisposedException(nameof(Deflater));
            }
            if (_input == null || _inputLength == 0)
            {
                return 0;
            }
            using (var ms = new MemoryStream())
            {
                using (var ds = new DeflateStream(ms, CompressionLevel.Fastest, leaveOpen: true))
                {
                    ds.Write(_input, _inputOffset, _inputLength);
                }
                byte[] compressed = ms.ToArray();
                int n = Math.Min(compressed.Length, output.Length);
                System.Array.Copy(compressed, 0, output, 0, n);
                _input = null;
                _inputLength = 0;
                _finished = true;
                return n;
            }
        }

        public virtual void end()
        {
            _ended = true;
            _input = null;
        }
    }
}
