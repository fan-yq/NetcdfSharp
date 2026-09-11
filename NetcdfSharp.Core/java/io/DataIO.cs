// java.io DataInput/DataOutput/DataInputStream shims for netcdf-java translation.
using System;
using System.IO;

namespace NetcdfSharp.java.io
{
    /// <summary>Interface for reading from a binary stream.</summary>
    public interface DataInput
    {
        void readFully(byte[] b);
        void readFully(byte[] b, int off, int len);
        int skipBytes(int n);
        bool readBoolean();
        byte readByte();
        int readUnsignedByte();
        short readShort();
        int readUnsignedShort();
        char readChar();
        int readInt();
        long readLong();
        float readFloat();
        double readDouble();
        string readLine();
        string readUTF();
    }

    /// <summary>Interface for writing to a binary stream.</summary>
    public interface DataOutput
    {
        void write(int b);
        void write(byte[] b);
        void write(byte[] b, int off, int len);
        void writeBoolean(bool v);
        void writeByte(int v);
        void writeShort(int v);
        void writeChar(int v);
        void writeInt(int v);
        void writeLong(long v);
        void writeFloat(float v);
        void writeDouble(double v);
        void writeBytes(string s);
        void writeChars(string s);
        void writeUTF(string s);
    }

    /// <summary>Data input stream backed by a Stream, implementing DataInput.</summary>
    public class DataInputStream : DataInput
    {
        private readonly Stream _in;

        public DataInputStream(Stream input) { _in = input; }
        public DataInputStream(byte[] data) { _in = new MemoryStream(data); }

        private static byte[] _buf = new byte[8];
        private int ReadIntBE()
        {
            byte[] b = _buf;
            ReadFullyB(b, 0, 4);
            return (b[0] & 0xFF) << 24 | (b[1] & 0xFF) << 16 | (b[2] & 0xFF) << 8 | (b[3] & 0xFF);
        }
        private void ReadFullyB(byte[] b, int off, int len)
        {
            int total = 0;
            while (total < len)
            {
                int n = _in.Read(b, off + total, len - total);
                if (n <= 0) throw new EndOfStreamException();
                total += n;
            }
        }
        public void readFully(byte[] b) => readFully(b, 0, b.Length);
        public void readFully(byte[] b, int off, int len) => ReadFullyB(b, off, len);
        public int skipBytes(int n)
        {
            if (_in.CanSeek) { _in.Position += n; return n; }
            long start = _in.Position;
            byte[] tmp = new byte[n];
            ReadFullyB(tmp, 0, n);
            return (int)(_in.Position - start);
        }
        public long skip(long n) => skipBytes((int)n);
        public bool readBoolean() => readUnsignedByte() != 0;
        public byte readByte()
        {
            int b = _in.ReadByte();
            if (b < 0) throw new EndOfStreamException();
            return (byte)b;
        }
        public int readUnsignedByte() => readByte() & 0xFF;
        public short readShort() => (short)readUnsignedShort();
        public int readUnsignedShort()
        {
            int hi = readUnsignedByte(), lo = readUnsignedByte();
            return hi << 8 | lo;
        }
        public char readChar() => (char)readUnsignedShort();
        public int readInt() => ReadIntBE();
        public long readLong()
        {
            byte[] b = _buf;
            ReadFullyB(b, 0, 8);
            long r = 0;
            for (int i = 0; i < 8; i++) r = (r << 8) | (uint)(b[i] & 0xFF);
            return r;
        }
        public float readFloat() { int v = readInt(); return BitConverter.ToSingle(BitConverter.GetBytes(v), 0); }
        public double readDouble() { long v = readLong(); return BitConverter.Int64BitsToDouble(v); }
        public string readLine()
        {
            var sb = new System.Text.StringBuilder();
            int c;
            while ((c = _in.ReadByte()) >= 0)
            {
                if (c == '\n') break;
                if (c == '\r') { c = _in.ReadByte(); if (c >= 0 && c != '\n') _in.Position--; break; }
                sb.Append((char)c);
            }
            return sb.Length == 0 && (_in.Position >= _in.Length) ? null : sb.ToString();
        }
        public string readUTF()
        {
            int len = readUnsignedShort();
            byte[] b = new byte[len];
            ReadFullyB(b, 0, len);
            return new System.Text.UTF8Encoding(false).GetString(b);
        }
        public void close() => _in.Dispose();
        public Stream BaseStream => _in;
    }

    /// <summary>Data output stream backing by a Stream, implementing DataOutput.</summary>
    public class DataOutputStream : DataOutput
    {
        private readonly Stream _out;
        public DataOutputStream(Stream output) { _out = output; }
        public void write(int b) => _out.WriteByte((byte)b);
        public void write(byte[] b) => _out.Write(b, 0, b.Length);
        public void write(byte[] b, int off, int len) => _out.Write(b, off, len);
        public void writeBoolean(bool v) => _out.WriteByte(v ? (byte)1 : (byte)0);
        public void writeByte(int v) => _out.WriteByte((byte)v);
        public void writeShort(int v) { _out.WriteByte((byte)(v >> 8)); _out.WriteByte((byte)v); }
        public void writeChar(int v) => writeShort(v);
        public void writeInt(int v) { _out.WriteByte((byte)(v >> 24)); _out.WriteByte((byte)(v >> 16)); _out.WriteByte((byte)(v >> 8)); _out.WriteByte((byte)v); }
        public void writeLong(long v)
        {
            for (int i = 7; i >= 0; i--) _out.WriteByte((byte)(v >> (i * 8)));
        }
        public void writeFloat(float v) => writeInt(BitConverter.ToInt32(BitConverter.GetBytes(v), 0));
        public void writeDouble(double v) => writeLong(BitConverter.DoubleToInt64Bits(v));
        public void writeBytes(string s)
        {
            foreach (char c in s) _out.WriteByte((byte)c);
        }
        public void writeChars(string s)
        {
            foreach (char c in s) writeChar(c);
        }
        public void writeUTF(string s)
        {
            var enc = new System.Text.UTF8Encoding(false);
            byte[] b = enc.GetBytes(s);
            writeShort(b.Length);
            _out.Write(b, 0, b.Length);
        }
        public void flush() => _out.Flush();
        public void close() => _out.Dispose();
        public Stream BaseStream => _out;
    }
}
