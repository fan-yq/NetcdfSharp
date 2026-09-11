// NetcdfSharp.java.nio.ByteBuffer compatible - minimal subset used by NetcdfSharp.
// An in-memory byte buffer with position semantics, supporting big/little endian
// and typed view buffers (IntBuffer, ShortBuffer, FloatBuffer, DoubleBuffer, LongBuffer, CharBuffer).
using System;
using System.IO;

namespace NetcdfSharp.java.nio
{
    /// <summary>
    /// A byte buffer with position semantics. Simplified replacement for NetcdfSharp.java.nio.ByteBuffer.
    /// </summary>
    public class ByteBuffer
    {
        protected byte[] _data;
        protected int _position;
        protected int _limit;
        protected int _capacity;
        internal ByteOrder _order;

        protected ByteBuffer(byte[] data, int offset, int length)
        {
            _data = data;
            _position = offset;
            _limit = offset + length;
            _capacity = data.Length;
            _order = ByteOrder.BIG_ENDIAN;
        }

        // ---- factories ----

        public static ByteBuffer allocate(int capacity)
        {
            return new ByteBuffer(new byte[capacity], 0, capacity);
        }

        public static ByteBuffer allocateDirect(int capacity)
        {
            return allocate(capacity);
        }

        public static ByteBuffer wrap(byte[] data)
        {
            return new ByteBuffer(data, 0, data.Length);
        }

        public static ByteBuffer wrap(byte[] data, int offset, int length)
        {
            return new ByteBuffer(data, offset, length);
        }

        // ---- position/limit/capacity ----

        public int capacity() => _capacity;

        public int limit() => _limit;

        public ByteBuffer limit(int newLimit)
        {
            _limit = newLimit;
            return this;
        }

        public int position() => _position;

        public ByteBuffer position(int newPosition)
        {
            _position = newPosition;
            return this;
        }

        public int remaining() => _limit - _position;

        public bool hasRemaining() => remaining() > 0;

        public ByteBuffer flip()
        {
            _limit = _position;
            _position = 0;
            return this;
        }

        public ByteBuffer rewind()
        {
            _position = 0;
            return this;
        }

        public ByteBuffer clear()
        {
            _position = 0;
            _limit = _capacity;
            return this;
        }

        public ByteBuffer compact()
        {
            int n = remaining();
            System.Array.Copy(_data, _position, _data, 0, n);
            _position = n;
            _limit = _capacity;
            return this;
        }

        public ByteBuffer order(ByteOrder order)
        {
            _order = order ?? ByteOrder.BIG_ENDIAN;
            return this;
        }

        public ByteOrder order() => _order;

        public byte[] array() => _data;

        public short[] ToShortArray()
        {
            short[] a = new short[_data.Length / 2];
            for (int i = 0; i < a.Length; i++) a[i] = getShort(i * 2);
            return a;
        }

        public int[] ToIntArray()
        {
            int[] a = new int[_data.Length / 4];
            for (int i = 0; i < a.Length; i++) a[i] = getInt(i * 4);
            return a;
        }

        public long[] ToLongArray()
        {
            long[] a = new long[_data.Length / 8];
            for (int i = 0; i < a.Length; i++) a[i] = getLong(i * 8);
            return a;
        }

        public float[] ToSingleArray()
        {
            float[] a = new float[_data.Length / 4];
            for (int i = 0; i < a.Length; i++) a[i] = getFloat(i * 4);
            return a;
        }

        public double[] ToDoubleArray()
        {
            double[] a = new double[_data.Length / 8];
            for (int i = 0; i < a.Length; i++) a[i] = getDouble(i * 8);
            return a;
        }

        public int arrayOffset() => 0;

        public ByteBuffer duplicate()
        {
            var dup = new ByteBuffer(_data, _position, remaining());
            dup._order = _order;
            return dup;
        }

        public ByteBuffer slice()
        {
            return new ByteBuffer(_data, _position, remaining());
        }

        // ---- single byte ops ----

        public byte get()
        {
            if (_position >= _limit) throw new IndexOutOfRangeException("Buffer underflow");
            return _data[_position++];
        }

        public byte get(int index)
        {
            if (index < 0 || index >= _data.Length) throw new IndexOutOfRangeException("Buffer index " + index);
            return _data[index];
        }

        public ByteBuffer get(byte[] dst, int offset, int length)
        {
            System.Array.Copy(_data, _position, dst, offset, length);
            _position += length;
            return this;
        }

        public ByteBuffer put(byte b)
        {
            if (_position >= _limit) throw new IndexOutOfRangeException("Buffer overflow");
            _data[_position++] = b;
            return this;
        }

        public ByteBuffer put(byte[] src)
        {
            return put(src, 0, src.Length);
        }

        public ByteBuffer put(byte[] src, int offset, int length)
        {
            System.Array.Copy(src, offset, _data, _position, length);
            _position += length;
            return this;
        }

        // ---- typed accessors (big/little endian aware) ----

        public short getShort()
        {
            short v = GetShort(_position);
            _position += 2;
            return v;
        }

        public short getShort(int index) => GetShort(index);

        public ByteBuffer putShort(short value)
        {
            PutShort(_position, value);
            _position += 2;
            return this;
        }

        public ByteBuffer putShort(int index, short value)
        {
            PutShort(index, value);
            return this;
        }

        public int getInt()
        {
            int v = GetInt(_position);
            _position += 4;
            return v;
        }

        public int getInt(int index) => GetInt(index);

        public ByteBuffer putInt(int value)
        {
            PutInt(_position, value);
            _position += 4;
            return this;
        }

        public ByteBuffer putInt(int index, int value)
        {
            PutInt(index, value);
            return this;
        }

        public long getLong()
        {
            long v = GetLong(_position);
            _position += 8;
            return v;
        }

        public long getLong(int index) => GetLong(index);

        public ByteBuffer putLong(long value)
        {
            PutLong(_position, value);
            _position += 8;
            return this;
        }

        public ByteBuffer putLong(int index, long value)
        {
            PutLong(index, value);
            return this;
        }

        public float getFloat()
        {
            float v = GetFloat(_position);
            _position += 4;
            return v;
        }

        public float getFloat(int index) => GetFloat(index);

        public ByteBuffer putFloat(float value)
        {
            PutFloat(_position, value);
            _position += 4;
            return this;
        }

        public ByteBuffer putFloat(int index, float value)
        {
            PutFloat(index, value);
            return this;
        }

        public double getDouble()
        {
            double v = GetDouble(_position);
            _position += 8;
            return v;
        }

        public double getDouble(int index) => GetDouble(index);

        public ByteBuffer putDouble(double value)
        {
            PutDouble(_position, value);
            _position += 8;
            return this;
        }

        public ByteBuffer putDouble(int index, double value)
        {
            PutDouble(index, value);
            return this;
        }

        public char getChar()
        {
            char v = GetChar(_position);
            _position += 2;
            return v;
        }

        public char getChar(int index) => GetChar(index);

        public ByteBuffer putChar(char value)
        {
            PutChar(_position, value);
            _position += 2;
            return this;
        }

        public ByteBuffer putChar(int index, char value)
        {
            PutChar(index, value);
            return this;
        }

        // ---- view buffers ----

        public IntBuffer asIntBuffer() => new IntBuffer(this);

        public ShortBuffer asShortBuffer() => new ShortBuffer(this);

        public FloatBuffer asFloatBuffer() => new FloatBuffer(this);

        public DoubleBuffer asDoubleBuffer() => new DoubleBuffer(this);

        public LongBuffer asLongBuffer() => new LongBuffer(this);

        public CharBuffer asCharBuffer() => new CharBuffer(this);

        // ---- endian-aware core readers/writers ----

        internal short GetShort(int pos)
        {
            if (_order.BigEndian)
            {
                return (short)((_data[pos] & 0xFF) << 8 | (_data[pos + 1] & 0xFF));
            }
            else
            {
                return (short)((_data[pos + 1] & 0xFF) << 8 | (_data[pos] & 0xFF));
            }
        }

        internal void PutShort(int pos, short v)
        {
            if (_order.BigEndian)
            {
                _data[pos] = (byte)(v >> 8);
                _data[pos + 1] = (byte)v;
            }
            else
            {
                _data[pos] = (byte)v;
                _data[pos + 1] = (byte)(v >> 8);
            }
        }

        internal int GetInt(int pos)
        {
            if (_order.BigEndian)
            {
                return (_data[pos] & 0xFF) << 24 | (_data[pos + 1] & 0xFF) << 16 | (_data[pos + 2] & 0xFF) << 8 | (_data[pos + 3] & 0xFF);
            }
            else
            {
                return (_data[pos + 3] & 0xFF) << 24 | (_data[pos + 2] & 0xFF) << 16 | (_data[pos + 1] & 0xFF) << 8 | (_data[pos] & 0xFF);
            }
        }

        internal void PutInt(int pos, int v)
        {
            if (_order.BigEndian)
            {
                _data[pos] = (byte)(v >> 24);
                _data[pos + 1] = (byte)(v >> 16);
                _data[pos + 2] = (byte)(v >> 8);
                _data[pos + 3] = (byte)v;
            }
            else
            {
                _data[pos] = (byte)v;
                _data[pos + 1] = (byte)(v >> 8);
                _data[pos + 2] = (byte)(v >> 16);
                _data[pos + 3] = (byte)(v >> 24);
            }
        }

        internal long GetLong(int pos)
        {
            if (_order.BigEndian)
            {
                return ((long)GetInt(pos) << 32) | ((long)GetInt(pos + 4) & 0xFFFFFFFFL);
            }
            else
            {
                return ((long)GetInt(pos + 4) << 32) | ((long)GetInt(pos) & 0xFFFFFFFFL);
            }
        }

        internal void PutLong(int pos, long v)
        {
            if (_order.BigEndian)
            {
                PutInt(pos, (int)(v >> 32));
                PutInt(pos + 4, (int)v);
            }
            else
            {
                PutInt(pos, (int)v);
                PutInt(pos + 4, (int)(v >> 32));
            }
        }

        internal float GetFloat(int pos)
        {
            int bits = GetInt(pos);
            return BitConverter.Int32BitsToSingle(bits);
        }

        internal void PutFloat(int pos, float v)
        {
            PutInt(pos, BitConverter.SingleToInt32Bits(v));
        }

        internal double GetDouble(int pos)
        {
            long bits = GetLong(pos);
            return BitConverter.Int64BitsToDouble(bits);
        }

        internal void PutDouble(int pos, double v)
        {
            PutLong(pos, BitConverter.DoubleToInt64Bits(v));
        }

        internal char GetChar(int pos)
        {
            return (char)GetShort(pos);
        }

        internal void PutChar(int pos, char v)
        {
            PutShort(pos, (short)v);
        }
    }


}
