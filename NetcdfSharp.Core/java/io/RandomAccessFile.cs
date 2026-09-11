// NetcdfSharp.java.io.RandomAccessFile shim for netcdf-java translation.
// In Java, NetcdfSharp.ucar.unidata.io.RandomAccessFile extends NetcdfSharp.java.io.RandomAccessFile.
// This shim provides the shared read/write primitives used by the translated code.
using System;
using System.IO;
using NetcdfSharp.java.nio;

namespace NetcdfSharp.java.io
{
    /// <summary>
    /// Instances of this class support both reading and writing to a random
    /// access file. Minimal subset used by the netcdf-java translation.
    /// </summary>
    public class RandomAccessFile
    {
        protected internal System.IO.FileStream _fs;

        public RandomAccessFile(string name, string mode) : this(name, mode, 0) { }

        public RandomAccessFile(string name, string mode, int bufferSize)
        {
            System.IO.FileMode fm = mode.StartsWith("w") || mode.StartsWith("rw") || mode.StartsWith("ws")
                ? System.IO.FileMode.OpenOrCreate : System.IO.FileMode.Open;
            System.IO.FileAccess fa = mode.StartsWith("r") && mode.IndexOf('w') < 0
                ? System.IO.FileAccess.Read
                : mode.Contains("w") ? System.IO.FileAccess.ReadWrite : System.IO.FileAccess.Read;
            _fs = new System.IO.FileStream(name, fm, fa);
        }

        public RandomAccessFile(NetcdfSharp.java.io.File file, string mode) : this(file.GetPath(), mode) { }

        public virtual void seek(long pos) => _fs.Position = pos;
        public virtual long getFilePointer() => _fs.Position;
        public virtual long length() => _fs.Length;
        public virtual void setLength(long newLength) => _fs.SetLength(newLength);
        public virtual void close() => _fs.Dispose();
        public virtual int read() => _fs.ReadByte();
        public virtual int read(byte[] b, int off, int len) => _fs.Read(b, off, len);
        public virtual void readFully(byte[] b) => ReadFully(b, 0, b.Length);
        public virtual void readFully(byte[] b, int off, int len) => ReadFully(b, off, len);

        private void ReadFully(byte[] b, int off, int len)
        {
            int total = 0;
            while (total < len)
            {
                int n = _fs.Read(b, off + total, len - total);
                if (n <= 0) throw new EndOfStreamException();
                total += n;
            }
        }

        public virtual bool readBoolean() => readUnsignedByte() != 0;
        public virtual byte readByte() { int b = _fs.ReadByte(); if (b < 0) throw new EndOfStreamException(); return (byte)b; }
        public virtual int readUnsignedByte() => readByte() & 0xFF;
        public virtual short readShort() => (short)readUnsignedShort();
        public virtual int readUnsignedShort()
        {
            int hi = readUnsignedByte(), lo = readUnsignedByte();
            return hi << 8 | lo;
        }
        public virtual char readChar() => (char)readUnsignedShort();
        public virtual int readInt()
        {
            byte[] b = new byte[4];
            readFully(b);
            return (b[0] & 0xFF) << 24 | (b[1] & 0xFF) << 16 | (b[2] & 0xFF) << 8 | (b[3] & 0xFF);
        }
        public virtual long readLong()
        {
            byte[] b = new byte[8];
            readFully(b);
            long r = 0;
            for (int i = 0; i < 8; i++) r = (r << 8) | (uint)(b[i] & 0xFF);
            return r;
        }
        public virtual float readFloat() => BitConverter.ToSingle(BitConverter.GetBytes(readInt()), 0);
        public virtual double readDouble() => BitConverter.Int64BitsToDouble(readLong());
        public virtual string readUTF()
        {
            int len = readUnsignedShort();
            byte[] b = new byte[len];
            readFully(b);
            return new System.Text.UTF8Encoding(false).GetString(b);
        }

        public virtual void write(int b) => _fs.WriteByte((byte)b);
        public virtual void write(byte[] b) => _fs.Write(b, 0, b.Length);
        public virtual void write(byte[] b, int off, int len) => _fs.Write(b, off, len);
        public virtual void writeBoolean(bool v) => _fs.WriteByte(v ? (byte)1 : (byte)0);
        public virtual void writeByte(int v) => _fs.WriteByte((byte)v);
        public virtual void writeShort(int v) { _fs.WriteByte((byte)(v >> 8)); _fs.WriteByte((byte)v); }
        public virtual void writeChar(int v) => writeShort(v);
        public virtual void writeInt(int v)
        {
            _fs.WriteByte((byte)(v >> 24)); _fs.WriteByte((byte)(v >> 16));
            _fs.WriteByte((byte)(v >> 8)); _fs.WriteByte((byte)v);
        }
        public virtual void writeLong(long v)
        {
            for (int i = 7; i >= 0; i--) _fs.WriteByte((byte)(v >> (i * 8)));
        }
        public virtual void writeFloat(float v) => writeInt(BitConverter.ToInt32(BitConverter.GetBytes(v), 0));
        public virtual void writeDouble(double v) => writeLong(BitConverter.DoubleToInt64Bits(v));
        public virtual void writeUTF(string s)
        {
            var enc = new System.Text.UTF8Encoding(false);
            byte[] b = enc.GetBytes(s);
            writeShort(b.Length);
            _fs.Write(b, 0, b.Length);
        }

        public virtual NetcdfSharp.java.nio.channels.FileChannel getChannel() => new NetcdfSharp.java.nio.channels.FileChannel(_fs);
    }
}
