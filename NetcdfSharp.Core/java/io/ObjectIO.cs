// java.io ObjectInputStream/ObjectOutputStream minimal shims.
// Java serialization subset used by geoloc classes (readDouble/writeDouble etc).
using System;
using System.IO;

namespace NetcdfSharp.java.io
{
    /// <summary>Java ObjectInputStream subset: primitive data reads in big-endian (Java default).</summary>
    public class ObjectInputStream : DataInput
    {
        private readonly Stream _in;

        public ObjectInputStream(Stream input) { _in = input; }

        private byte[] _buf = new byte[8];
        private int ReadBEInt()
        {
            ReadFullyB(_buf, 0, 4);
            return (_buf[0] & 0xFF) << 24 | (_buf[1] & 0xFF) << 16 | (_buf[2] & 0xFF) << 8 | (_buf[3] & 0xFF);
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
        public int skipBytes(int n) => n;
        public bool readBoolean() => readUnsignedByte() != 0;
        public byte readByte() { int b = _in.ReadByte(); if (b < 0) throw new EndOfStreamException(); return (byte)b; }
        public int readUnsignedByte() => readByte() & 0xFF;
        public short readShort() => (short)readUnsignedShort();
        public int readUnsignedShort() { int hi = readUnsignedByte(), lo = readUnsignedByte(); return hi << 8 | lo; }
        public char readChar() => (char)readUnsignedShort();
        public int readInt() => ReadBEInt();
        public long readLong()
        {
            ReadFullyB(_buf, 0, 8);
            long r = 0;
            for (int i = 0; i < 8; i++) r = (r << 8) | (uint)(_buf[i] & 0xFF);
            return r;
        }
        public float readFloat() => BitConverter.ToSingle(BitConverter.GetBytes(readInt()), 0);
        public double readDouble() => BitConverter.Int64BitsToDouble(readLong());
        public string readLine() => null;
        public string readUTF() { int len = readUnsignedShort(); byte[] b = new byte[len]; readFully(b); return new System.Text.UTF8Encoding(false).GetString(b); }
        public void close() => _in.Dispose();
    }

    /// <summary>Java ObjectOutputStream subset: primitive data writes in big-endian.</summary>
    public class ObjectOutputStream : DataOutput
    {
        private readonly Stream _out;
        public ObjectOutputStream(Stream output) { _out = output; }
        public void write(int b) => _out.WriteByte((byte)b);
        public void write(byte[] b) => _out.Write(b, 0, b.Length);
        public void write(byte[] b, int off, int len) => _out.Write(b, off, len);
        public void writeBoolean(bool v) => _out.WriteByte(v ? (byte)1 : (byte)0);
        public void writeByte(int v) => _out.WriteByte((byte)v);
        public void writeShort(int v) { _out.WriteByte((byte)(v >> 8)); _out.WriteByte((byte)v); }
        public void writeChar(int v) => writeShort(v);
        public void writeInt(int v) { _out.WriteByte((byte)(v >> 24)); _out.WriteByte((byte)(v >> 16)); _out.WriteByte((byte)(v >> 8)); _out.WriteByte((byte)v); }
        public void writeLong(long v) { for (int i = 7; i >= 0; i--) _out.WriteByte((byte)(v >> (i * 8))); }
        public void writeFloat(float v) => writeInt(BitConverter.ToInt32(BitConverter.GetBytes(v), 0));
        public void writeDouble(double v) => writeLong(BitConverter.DoubleToInt64Bits(v));
        public void writeBytes(string s) { foreach (char c in s) _out.WriteByte((byte)c); }
        public void writeChars(string s) { foreach (char c in s) writeChar(c); }
        public void writeUTF(string s) { var enc = new System.Text.UTF8Encoding(false); byte[] b = enc.GetBytes(s); writeShort(b.Length); _out.Write(b, 0, b.Length); }
        public void flush() => _out.Flush();
        public void close() => _out.Dispose();
    }
}
