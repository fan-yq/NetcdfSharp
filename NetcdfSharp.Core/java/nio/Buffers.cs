// NetcdfSharp.java.nio.Buffer view shims (ShortBuffer, IntBuffer, etc.) for netcdf-java translation.
using System;

namespace NetcdfSharp.java.nio
{
    /// <summary>Base class for typed buffer views over a byte[] or existing typed data.</summary>
    public abstract class JBuffer
    {
        public virtual int capacity() => 0;
        public virtual int position() => 0;
        public virtual JBuffer position(int newPosition) => this;
        public virtual int limit() => 0;
        public virtual JBuffer limit(int newLimit) => this;
        public virtual int remaining() => limit() - position();
        public virtual bool hasRemaining() => remaining() > 0;
        public virtual JBuffer clear() => this;
        public virtual JBuffer flip() => this;
        public virtual JBuffer rewind() => this;
        public virtual bool isReadOnly() => false;
    }

    /// <summary>ByteBuffer-ordered short view, backed by a ByteBuffer.</summary>
    public class ShortBuffer : JBuffer
    {
        internal ByteBuffer _bb;
        private readonly int _offset; // in shorts
        private readonly int _len;
        private int _pos;
        private int _lim;

        internal ShortBuffer(ByteBuffer bb, int offset, int len, int pos, int lim)
        {
            _bb = bb; _offset = offset; _len = len; _pos = pos; _lim = lim;
        }
        internal ShortBuffer(ByteBuffer bb)
        {
            _bb = bb; _offset = 0; _len = bb.remaining() / 2; _pos = 0; _lim = _len;
        }
        public static ShortBuffer allocate(int capacity) => new ShortBuffer(ByteBuffer.allocate(capacity * 2), 0, capacity, 0, capacity);
        public static ShortBuffer wrap(short[] array) => new ShortBuffer(ByteBuffer.wrap(ByteOrderHelper.ToInt16Bytes(array)), 0, array.Length, 0, array.Length);
        public override int capacity() => _len;
        public override int position() => _pos;
        public override JBuffer position(int newPosition) { _pos = newPosition; return this; }
        public override int limit() => _lim;
        public override JBuffer limit(int newLimit) { _lim = newLimit; return this; }
        public override int remaining() => _lim - _pos;
        public override bool hasRemaining() => _pos < _lim;
        public override JBuffer clear() { _pos = 0; _lim = _len; return this; }
        public override JBuffer flip() { _lim = _pos; _pos = 0; return this; }
        public override JBuffer rewind() { _pos = 0; return this; }
        public ShortBuffer duplicate() => new ShortBuffer(_bb, _offset, _len, _pos, _lim);
        public short get()
        {
            if (_pos >= _lim) throw new IndexOutOfRangeException();
            return _bb.getShort((_offset + _pos++) * 2);
        }
        public short get(int index) => _bb.getShort((_offset + index) * 2);
        public ShortBuffer get(short[] dst)
        {
            for (int i = 0; i < dst.Length; i++) dst[i] = get();
            return this;
        }
        public ShortBuffer put(short s) { _bb.putShort((_offset + _pos++) * 2, s); return this; }
        public ShortBuffer put(short[] src) { for (int i = 0; i < src.Length; i++) put(src[i]); return this; }
        public ShortBuffer put(int index, short s) { _bb.putShort((_offset + index) * 2, s); return this; }
        public short[] array() => _bb.ToShortArray();
    }

    /// <summary>Int view over a ByteBuffer.</summary>
    public class IntBuffer : JBuffer
    {
        internal ByteBuffer _bb;
        private readonly int _offset;
        private readonly int _len;
        private int _pos;
        private int _lim;

        internal IntBuffer(ByteBuffer bb, int offset, int len, int pos, int lim)
        {
            _bb = bb; _offset = offset; _len = len; _pos = pos; _lim = lim;
        }
        internal IntBuffer(ByteBuffer bb)
        {
            _bb = bb; _offset = 0; _len = bb.remaining() / 4; _pos = 0; _lim = _len;
        }
        public static IntBuffer allocate(int capacity) => new IntBuffer(ByteBuffer.allocate(capacity * 4), 0, capacity, 0, capacity);
        public static IntBuffer wrap(int[] array) => new IntBuffer(ByteBuffer.wrap(ByteOrderHelper.ToInt32Bytes(array)), 0, array.Length, 0, array.Length);
        public override int capacity() => _len;
        public override int position() => _pos;
        public override JBuffer position(int newPosition) { _pos = newPosition; return this; }
        public override int limit() => _lim;
        public override JBuffer limit(int newLimit) { _lim = newLimit; return this; }
        public override int remaining() => _lim - _pos;
        public override bool hasRemaining() => _pos < _lim;
        public override JBuffer clear() { _pos = 0; _lim = _len; return this; }
        public override JBuffer flip() { _lim = _pos; _pos = 0; return this; }
        public override JBuffer rewind() { _pos = 0; return this; }
        public IntBuffer duplicate() => new IntBuffer(_bb, _offset, _len, _pos, _lim);
        public int get() { if (_pos >= _lim) throw new IndexOutOfRangeException(); return _bb.getInt((_offset + _pos++) * 4); }
        public int get(int index) => _bb.getInt((_offset + index) * 4);
        public IntBuffer get(int[] dst) { for (int i = 0; i < dst.Length; i++) dst[i] = get(); return this; }
        public IntBuffer put(int v) { _bb.putInt((_offset + _pos++) * 4, v); return this; }
        public IntBuffer put(int[] src) { for (int i = 0; i < src.Length; i++) put(src[i]); return this; }
        public IntBuffer put(int index, int v) { _bb.putInt((_offset + index) * 4, v); return this; }
        public int[] array() => _bb.ToIntArray();
    }

    /// <summary>Long view over a ByteBuffer.</summary>
    public class LongBuffer : JBuffer
    {
        internal ByteBuffer _bb;
        private readonly int _offset;
        private readonly int _len;
        private int _pos;
        private int _lim;

        internal LongBuffer(ByteBuffer bb, int offset, int len, int pos, int lim)
        {
            _bb = bb; _offset = offset; _len = len; _pos = pos; _lim = lim;
        }
        internal LongBuffer(ByteBuffer bb)
        {
            _bb = bb; _offset = 0; _len = bb.remaining() / 8; _pos = 0; _lim = _len;
        }
        public static LongBuffer allocate(int capacity) => new LongBuffer(ByteBuffer.allocate(capacity * 8), 0, capacity, 0, capacity);
        public static LongBuffer wrap(long[] array) => new LongBuffer(ByteBuffer.wrap(ByteOrderHelper.ToInt64Bytes(array)), 0, array.Length, 0, array.Length);
        public override int capacity() => _len;
        public override int position() => _pos;
        public override JBuffer position(int newPosition) { _pos = newPosition; return this; }
        public override int limit() => _lim;
        public override JBuffer limit(int newLimit) { _lim = newLimit; return this; }
        public override int remaining() => _lim - _pos;
        public override bool hasRemaining() => _pos < _lim;
        public override JBuffer clear() { _pos = 0; _lim = _len; return this; }
        public override JBuffer flip() { _lim = _pos; _pos = 0; return this; }
        public override JBuffer rewind() { _pos = 0; return this; }
        public LongBuffer duplicate() => new LongBuffer(_bb, _offset, _len, _pos, _lim);
        public long get() { if (_pos >= _lim) throw new IndexOutOfRangeException(); return _bb.getLong((_offset + _pos++) * 8); }
        public long get(int index) => _bb.getLong((_offset + index) * 8);
        public LongBuffer get(long[] dst) { for (int i = 0; i < dst.Length; i++) dst[i] = get(); return this; }
        public LongBuffer put(long v) { _bb.putLong((_offset + _pos++) * 8, v); return this; }
        public LongBuffer put(long[] src) { for (int i = 0; i < src.Length; i++) put(src[i]); return this; }
        public LongBuffer put(int index, long v) { _bb.putLong((_offset + index) * 8, v); return this; }
        public long[] array() => _bb.ToLongArray();
    }

    /// <summary>Float view over a ByteBuffer.</summary>
    public class FloatBuffer : JBuffer
    {
        internal ByteBuffer _bb;
        private readonly int _offset;
        private readonly int _len;
        private int _pos;
        private int _lim;

        internal FloatBuffer(ByteBuffer bb, int offset, int len, int pos, int lim)
        {
            _bb = bb; _offset = offset; _len = len; _pos = pos; _lim = lim;
        }
        internal FloatBuffer(ByteBuffer bb)
        {
            _bb = bb; _offset = 0; _len = bb.remaining() / 4; _pos = 0; _lim = _len;
        }
        public static FloatBuffer allocate(int capacity) => new FloatBuffer(ByteBuffer.allocate(capacity * 4), 0, capacity, 0, capacity);
        public static FloatBuffer wrap(float[] array) => new FloatBuffer(ByteBuffer.wrap(ByteOrderHelper.ToSingleBytes(array)), 0, array.Length, 0, array.Length);
        public override int capacity() => _len;
        public override int position() => _pos;
        public override JBuffer position(int newPosition) { _pos = newPosition; return this; }
        public override int limit() => _lim;
        public override JBuffer limit(int newLimit) { _lim = newLimit; return this; }
        public override int remaining() => _lim - _pos;
        public override bool hasRemaining() => _pos < _lim;
        public override JBuffer clear() { _pos = 0; _lim = _len; return this; }
        public override JBuffer flip() { _lim = _pos; _pos = 0; return this; }
        public override JBuffer rewind() { _pos = 0; return this; }
        public FloatBuffer duplicate() => new FloatBuffer(_bb, _offset, _len, _pos, _lim);
        public float get() { if (_pos >= _lim) throw new IndexOutOfRangeException(); return _bb.getFloat((_offset + _pos++) * 4); }
        public float get(int index) => _bb.getFloat((_offset + index) * 4);
        public FloatBuffer get(float[] dst) { for (int i = 0; i < dst.Length; i++) dst[i] = get(); return this; }
        public FloatBuffer put(float v) { _bb.putFloat((_offset + _pos++) * 4, v); return this; }
        public FloatBuffer put(float[] src) { for (int i = 0; i < src.Length; i++) put(src[i]); return this; }
        public FloatBuffer put(int index, float v) { _bb.putFloat((_offset + index) * 4, v); return this; }
        public float[] array() => _bb.ToSingleArray();
    }

    /// <summary>Double view over a ByteBuffer.</summary>
    public class DoubleBuffer : JBuffer
    {
        internal ByteBuffer _bb;
        private readonly int _offset;
        private readonly int _len;
        private int _pos;
        private int _lim;

        internal DoubleBuffer(ByteBuffer bb, int offset, int len, int pos, int lim)
        {
            _bb = bb; _offset = offset; _len = len; _pos = pos; _lim = lim;
        }
        internal DoubleBuffer(ByteBuffer bb)
        {
            _bb = bb; _offset = 0; _len = bb.remaining() / 8; _pos = 0; _lim = _len;
        }
        public static DoubleBuffer allocate(int capacity) => new DoubleBuffer(ByteBuffer.allocate(capacity * 8), 0, capacity, 0, capacity);
        public static DoubleBuffer wrap(double[] array) => new DoubleBuffer(ByteBuffer.wrap(ByteOrderHelper.ToDoubleBytes(array)), 0, array.Length, 0, array.Length);
        public override int capacity() => _len;
        public override int position() => _pos;
        public override JBuffer position(int newPosition) { _pos = newPosition; return this; }
        public override int limit() => _lim;
        public override JBuffer limit(int newLimit) { _lim = newLimit; return this; }
        public override int remaining() => _lim - _pos;
        public override bool hasRemaining() => _pos < _lim;
        public override JBuffer clear() { _pos = 0; _lim = _len; return this; }
        public override JBuffer flip() { _lim = _pos; _pos = 0; return this; }
        public override JBuffer rewind() { _pos = 0; return this; }
        public DoubleBuffer duplicate() => new DoubleBuffer(_bb, _offset, _len, _pos, _lim);
        public double get() { if (_pos >= _lim) throw new IndexOutOfRangeException(); return _bb.getDouble((_offset + _pos++) * 8); }
        public double get(int index) => _bb.getDouble((_offset + index) * 8);
        public DoubleBuffer get(double[] dst) { for (int i = 0; i < dst.Length; i++) dst[i] = get(); return this; }
        public DoubleBuffer put(double v) { _bb.putDouble((_offset + _pos++) * 8, v); return this; }
        public DoubleBuffer put(double[] src) { for (int i = 0; i < src.Length; i++) put(src[i]); return this; }
        public DoubleBuffer put(int index, double v) { _bb.putDouble((_offset + index) * 8, v); return this; }
        public double[] array() => _bb.ToDoubleArray();
    }

    /// <summary>Helpers to convert primitive arrays to/from bytes with a chosen byte order.</summary>
    internal static class ByteOrderHelper
    {
        public static byte[] ToInt16Bytes(short[] a)
        {
            byte[] b = new byte[a.Length * 2];
            for (int i = 0; i < a.Length; i++)
            {
                b[i * 2] = (byte)(a[i] >> 8);
                b[i * 2 + 1] = (byte)a[i];
            }
            return b;
        }
        public static byte[] ToInt32Bytes(int[] a)
        {
            byte[] b = new byte[a.Length * 4];
            for (int i = 0; i < a.Length; i++)
            {
                b[i * 4] = (byte)(a[i] >> 24);
                b[i * 4 + 1] = (byte)(a[i] >> 16);
                b[i * 4 + 2] = (byte)(a[i] >> 8);
                b[i * 4 + 3] = (byte)a[i];
            }
            return b;
        }
        public static byte[] ToInt64Bytes(long[] a)
        {
            byte[] b = new byte[a.Length * 8];
            for (int i = 0; i < a.Length; i++)
            {
                b[i * 8] = (byte)(a[i] >> 56);
                b[i * 8 + 1] = (byte)(a[i] >> 48);
                b[i * 8 + 2] = (byte)(a[i] >> 40);
                b[i * 8 + 3] = (byte)(a[i] >> 32);
                b[i * 8 + 4] = (byte)(a[i] >> 24);
                b[i * 8 + 5] = (byte)(a[i] >> 16);
                b[i * 8 + 6] = (byte)(a[i] >> 8);
                b[i * 8 + 7] = (byte)a[i];
            }
            return b;
        }
        public static byte[] ToSingleBytes(float[] a)
        {
            byte[] b = new byte[a.Length * 4];
            System.Buffer.BlockCopy(a, 0, b, 0, b.Length);
            return b;
        }
        public static byte[] ToDoubleBytes(double[] a)
        {
            byte[] b = new byte[a.Length * 8];
            System.Buffer.BlockCopy(a, 0, b, 0, b.Length);
            return b;
        }
    }

    /// <summary>Char view over a ByteBuffer.</summary>
    public class CharBuffer : JBuffer
    {
        internal ByteBuffer _bb;
        private readonly int _offset;
        private readonly int _len;
        private int _pos;
        private int _lim;

        internal CharBuffer(ByteBuffer bb, int offset, int len, int pos, int lim)
        {
            _bb = bb; _offset = offset; _len = len; _pos = pos; _lim = lim;
        }
        internal CharBuffer(ByteBuffer bb)
        {
            _bb = bb; _offset = 0; _len = bb.remaining() / 2; _pos = 0; _lim = _len;
        }
        public override int capacity() => _len;
        public override int position() => _pos;
        public override JBuffer position(int newPosition) { _pos = newPosition; return this; }
        public override int limit() => _lim;
        public override JBuffer limit(int newLimit) { _lim = newLimit; return this; }
        public override int remaining() => _lim - _pos;
        public override bool hasRemaining() => _pos < _lim;
        public override JBuffer clear() { _pos = 0; _lim = _len; return this; }
        public override JBuffer flip() { _lim = _pos; _pos = 0; return this; }
        public override JBuffer rewind() { _pos = 0; return this; }
        public CharBuffer duplicate() => new CharBuffer(_bb, _offset, _len, _pos, _lim);
        public char get() { if (_pos >= _lim) throw new IndexOutOfRangeException(); return _bb.getChar((_offset + _pos++) * 2); }
        public char get(int index) => _bb.getChar((_offset + index) * 2);
        public CharBuffer put(char v) { _bb.putChar((_offset + _pos++) * 2, v); return this; }
        public CharBuffer put(int index, char v) { _bb.putChar((_offset + index) * 2, v); return this; }

        // java Appendable.append semantics
        public CharBuffer append(char c) { put(c); return this; }
        public CharBuffer append(string s)
        {
            foreach (char c in s) put(c);
            return this;
        }
    }
}
