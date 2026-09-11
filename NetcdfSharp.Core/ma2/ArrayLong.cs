// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// Concrete implementation of Array specialized for longs.
	/// Data storage is with 1D java array of longs.
	/// 
	/// issues: what should we do if a conversion loses accuracy? nothing ? Exception ?
	/// </summary>
	/// <seealso cref="Array"
	/// @author caron/>
	public class ArrayLong : NcArray
	{

		/// <summary>
		/// package private. use Array.factory() </summary>
		internal static ArrayLong factory(NcIndex index, bool isUnsigned)
		{
			return ArrayLong.factory(index, isUnsigned, null);
		}

		/*
		 * create new ArrayLong with given indexImpl and backing store.
		 * 
		 * @param index use this Index
		 * 
		 * @param stor. use this storage. if null, allocate.
		 * 
		 * @return. new ArrayLong.D<rank> or ArrayLong object.
		 */
		internal static ArrayLong factory(NcIndex index, bool isUnsigned, long[] storage)
		{
			if (index is Index0D)
			{
				return new ArrayLong.D0(index, isUnsigned, storage);
			}
			else if (index is Index1D)
			{
				return new ArrayLong.D1(index, isUnsigned, storage);
			}
			else if (index is Index2D)
			{
				return new ArrayLong.D2(index, isUnsigned, storage);
			}
			else if (index is Index3D)
			{
				return new ArrayLong.D3(index, isUnsigned, storage);
			}
			else if (index is Index4D)
			{
				return new ArrayLong.D4(index, isUnsigned, storage);
			}
			else if (index is Index5D)
			{
				return new ArrayLong.D5(index, isUnsigned, storage);
			}
			else if (index is Index6D)
			{
				return new ArrayLong.D6(index, isUnsigned, storage);
			}
			else if (index is Index7D)
			{
				return new ArrayLong.D7(index, isUnsigned, storage);
			}
			else
			{
				return new ArrayLong(index, isUnsigned, storage);
			}
		}

		//////////////////////////////////////////////////////
		protected internal long[] storage;

		/// <summary>
		/// Create a new Array of type long and the given shape.
		/// dimensions.length determines the rank of the new Array.
		/// </summary>
		/// <param name="dimensions"> the shape of the Array. </param>
		public ArrayLong(int[] dimensions, bool isUnsigned) : base(isUnsigned ? DataType.ULONG : DataType.LONG, dimensions)
		{
			storage = new long[(int)indexCalc.Size];
		}

		/// <summary>
		/// Create a new Array using the given IndexArray and backing store.
		/// used for sections. Trusted package private.
		/// </summary>
		/// <param name="ima"> use this IndexArray as the index </param>
		/// <param name="data"> use this as the backing store </param>
		internal ArrayLong(NcIndex ima, bool isUnsigned, long[] data) : base(isUnsigned ? DataType.ULONG : DataType.LONG, ima)
		{
			/*
			 * replace by something better
			 * if (ima.getSize() != data.length)
			 * throw new IllegalArgumentException("bad data length");
			 */
			if (data != null)
			{
				storage = data;
			}
			else
			{
				storage = new long[(int)ima.Size];
			}
		}

		/// <summary>
		/// create new Array with given indexImpl and same backing store </summary>
		protected internal override NcArray createView(NcIndex index)
		{
			return ArrayLong.factory(index, isUnsigned(), storage);
		}

		/* Get underlying primitive array storage. CAUTION! You may invalidate your warrentee! */
		public override object Storage
		{
			get
			{
				return storage;
			}
		}

		// copy from javaArray to storage using the iterator: used by factory( Object);
		protected internal override void copyFrom1DJavaArray(IndexIterator iter, object javaArray)
		{
			long[] ja = (long[])javaArray;
			foreach (long aJa in ja)
			{
				iter.LongNext = aJa;
			}
		}

		// copy to javaArray from storage using the iterator: used by copyToNDJavaArray;
		protected internal override void copyTo1DJavaArray(IndexIterator iter, object javaArray)
		{
			long[] ja = (long[])javaArray;
			for (int i = 0; i < ja.Length; i++)
			{
				ja[i] = iter.LongNext;
			}
		}

		public override ByteBuffer DataAsByteBuffer
		{
			get
			{
				return getDataAsByteBuffer(null);
			}
		}

		public override ByteBuffer getDataAsByteBuffer(ByteOrder order)
		{
			ByteBuffer bb = base.getDataAsByteBuffer((int)(8 * Size), order);
			LongBuffer ib = bb.asLongBuffer();
			ib.put((long[])get1DJavaArray(DataType)); // make sure its in canonical order
			return bb;
		}

		/// <summary>
		/// Return the element class type </summary>
		public override Type ElementType
		{
			get
			{
				return typeof(long);
			}
		}

		/// <summary>
		/// get the value at the specified index. </summary>
		public virtual long get(NcIndex i)
		{
			return storage[i.currentElement()];
		}

		/// <summary>
		/// set the value at the sepcified index. </summary>
		public virtual void set(NcIndex i, long value)
		{
			storage[i.currentElement()] = value;
		}

		public override double getDouble(NcIndex i)
		{
			return (double)storage[i.currentElement()];
		}

		public override void setDouble(NcIndex i, double value)
		{
			storage[i.currentElement()] = (long)value;
		}

		public override float getFloat(NcIndex i)
		{
			return (float)storage[i.currentElement()];
		}

		public override void setFloat(NcIndex i, float value)
		{
			storage[i.currentElement()] = (long)value;
		}

		public override long getLong(NcIndex i)
		{
			return storage[i.currentElement()];
		}

		public override void setLong(NcIndex i, long value)
		{
			storage[i.currentElement()] = value;
		}

		public override int getInt(NcIndex i)
		{
			return (int)storage[i.currentElement()];
		}

		public override void setInt(NcIndex i, int value)
		{
			storage[i.currentElement()] = (long)value;
		}

		public override short getShort(NcIndex i)
		{
			return (short)storage[i.currentElement()];
		}

		public override void setShort(NcIndex i, short value)
		{
			storage[i.currentElement()] = (long)value;
		}

		public override byte getByte(NcIndex i)
		{
			return (byte)storage[i.currentElement()];
		}

		public override void setByte(NcIndex i, byte value)
		{
			storage[i.currentElement()] = (long)value;
		}

		public override char getChar(NcIndex i)
		{
			return (char)storage[i.currentElement()];
		}

		public override void setChar(NcIndex i, char value)
		{
			storage[i.currentElement()] = (long)value;
		}

		/// <summary>
		/// not legal, throw ForbiddenConversionException </summary>
		public override bool getBoolean(NcIndex i)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// not legal, throw ForbiddenConversionException </summary>
		public override void setBoolean(NcIndex i, bool value)
		{
			throw new ForbiddenConversionException();
		}

		public override object getObject(NcIndex i)
		{
			return storage[i.currentElement()];
		}

		public override void setObject(NcIndex i, object value)
		{
			storage[i.currentElement()] = Convert.ToInt64(value);
		}

		// package private : mostly for iterators
		public override double getDouble(int index)
		{
			return (double)storage[index];
		}

		public override void setDouble(int index, double value)
		{
			storage[index] = (long)value;
		}

		public override float getFloat(int index)
		{
			return storage[index];
		}

		public override void setFloat(int index, float value)
		{
			storage[index] = (long)value;
		}

		public override long getLong(int index)
		{
			return storage[index];
		}

		public override void setLong(int index, long value)
		{
			storage[index] = value;
		}

		public override int getInt(int index)
		{
			return (int)storage[index];
		}

		public override void setInt(int index, int value)
		{
			storage[index] = (long)value;
		}

		public override short getShort(int index)
		{
			return (short)storage[index];
		}

		public override void setShort(int index, short value)
		{
			storage[index] = (long)value;
		}

		public override byte getByte(int index)
		{
			return (byte)storage[index];
		}

		public override void setByte(int index, byte value)
		{
			storage[index] = (long)value;
		}

		public override char getChar(int index)
		{
			return (char)storage[index];
		}

		public override void setChar(int index, char value)
		{
			storage[index] = (long)value;
		}

		public override bool getBoolean(int index)
		{
			throw new ForbiddenConversionException();
		}

		public override void setBoolean(int index, bool value)
		{
			throw new ForbiddenConversionException();
		}

		public override object getObject(int index)
		{
			return getLong(index);
		}

		public override void setObject(int index, object value)
		{
			storage[index] = Convert.ToInt64(value);
		}

		/// <summary>
		/// Concrete implementation of Array specialized for longs, rank 0. </summary>
		public class D0 : ArrayLong
		{
			internal Index0D ix;

			/// <summary>
			/// Constructor. </summary>
			public D0(bool isUnsigned) : base(new int[] { }, isUnsigned)
			{
				ix = (Index0D)indexCalc;
			}

			internal D0(NcIndex i, bool isUnsigned, long[] store) : base(i, isUnsigned, store)
			{
				ix = (Index0D)indexCalc;
			}

			/// <summary>
			/// get the value. </summary>
			public virtual long get()
			{
				return storage[ix.currentElement()];
			}

			/// <summary>
			/// set the value. </summary>
			public virtual void set(long value)
			{
				storage[ix.currentElement()] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for longs, rank 1. </summary>
		public class D1 : ArrayLong
		{
			internal Index1D ix;

			/// <summary>
			/// Constructor for array of shape {len0}. </summary>
			public D1(int len0, bool isUnsigned) : base(new int[] { len0 }, isUnsigned)
			{
				ix = (Index1D)indexCalc;
			}

			internal D1(NcIndex i, bool isUnsigned, long[] store) : base(i, isUnsigned, store)
			{
				ix = (Index1D)indexCalc;
			}

			/// <summary>
			/// get the value. </summary>
			public virtual long get(int i)
			{
				return storage[ix.setDirect(i)];
			}

			/// <summary>
			/// set the value. </summary>
			public virtual void set(int i, long value)
			{
				storage[ix.setDirect(i)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for longs, rank 2. </summary>
		public class D2 : ArrayLong
		{
			internal Index2D ix;

			/// <summary>
			/// Constructor for array of shape {len0,len1}. </summary>
			public D2(int len0, int len1, bool isUnsigned) : base(new int[] { len0, len1 }, isUnsigned)
			{
				ix = (Index2D)indexCalc;
			}

			internal D2(NcIndex i, bool isUnsigned, long[] store) : base(i, isUnsigned, store)
			{
				ix = (Index2D)indexCalc;
			}

			/// <summary>
			/// get the value. </summary>
			public virtual long get(int i, int j)
			{
				return storage[ix.setDirect(i, j)];
			}

			/// <summary>
			/// set the value. </summary>
			public virtual void set(int i, int j, long value)
			{
				storage[ix.setDirect(i, j)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for longs, rank 3. </summary>
		public class D3 : ArrayLong
		{
			internal Index3D ix;

			/// <summary>
			/// Constructor for array of shape {len0,len1,len2}. </summary>
			public D3(int len0, int len1, int len2, bool isUnsigned) : base(new int[] { len0, len1, len2 }, isUnsigned)
			{
				ix = (Index3D)indexCalc;
			}

			internal D3(NcIndex i, bool isUnsigned, long[] store) : base(i, isUnsigned, store)
			{
				ix = (Index3D)indexCalc;
			}

			/// <summary>
			/// get the value. </summary>
			public virtual long get(int i, int j, int k)
			{
				return storage[ix.setDirect(i, j, k)];
			}

			/// <summary>
			/// set the value. </summary>
			public virtual void set(int i, int j, int k, long value)
			{
				storage[ix.setDirect(i, j, k)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for longs, rank 4. </summary>
		public class D4 : ArrayLong
		{
			internal Index4D ix;

			/// <summary>
			/// Constructor for array of shape {len0,len1,len2,len3}. </summary>
			public D4(int len0, int len1, int len2, int len3, bool isUnsigned) : base(new int[] { len0, len1, len2, len3 }, isUnsigned)
			{
				ix = (Index4D)indexCalc;
			}

			internal D4(NcIndex i, bool isUnsigned, long[] store) : base(i, isUnsigned, store)
			{
				ix = (Index4D)indexCalc;
			}

			/// <summary>
			/// get the value. </summary>
			public virtual long get(int i, int j, int k, int l)
			{
				return storage[ix.setDirect(i, j, k, l)];
			}

			/// <summary>
			/// set the value. </summary>
			public virtual void set(int i, int j, int k, int l, long value)
			{
				storage[ix.setDirect(i, j, k, l)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for longs, rank 5. </summary>
		public class D5 : ArrayLong
		{
			internal Index5D ix;

			/// <summary>
			/// Constructor for array of shape {len0,len1,len2,len3,len4}. </summary>
			public D5(int len0, int len1, int len2, int len3, int len4, bool isUnsigned) : base(new int[] { len0, len1, len2, len3, len4 }, isUnsigned)
			{
				ix = (Index5D)indexCalc;
			}

			internal D5(NcIndex i, bool isUnsigned, long[] store) : base(i, isUnsigned, store)
			{
				ix = (Index5D)indexCalc;
			}

			/// <summary>
			/// get the value. </summary>
			public virtual long get(int i, int j, int k, int l, int m)
			{
				return storage[ix.setDirect(i, j, k, l, m)];
			}

			/// <summary>
			/// set the value. </summary>
			public virtual void set(int i, int j, int k, int l, int m, long value)
			{
				storage[ix.setDirect(i, j, k, l, m)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for longs, rank 6. </summary>
		public class D6 : ArrayLong
		{
			internal Index6D ix;

			/// <summary>
			/// Constructor for array of shape {len0,len1,len2,len3,len4,len5,}. </summary>
			public D6(int len0, int len1, int len2, int len3, int len4, int len5, bool isUnsigned) : base(new int[] { len0, len1, len2, len3, len4, len5 }, isUnsigned)
			{
				ix = (Index6D)indexCalc;
			}

			internal D6(NcIndex i, bool isUnsigned, long[] store) : base(i, isUnsigned, store)
			{
				ix = (Index6D)indexCalc;
			}

			/// <summary>
			/// get the value. </summary>
			public virtual long get(int i, int j, int k, int l, int m, int n)
			{
				return storage[ix.setDirect(i, j, k, l, m, n)];
			}

			/// <summary>
			/// set the value. </summary>
			public virtual void set(int i, int j, int k, int l, int m, int n, long value)
			{
				storage[ix.setDirect(i, j, k, l, m, n)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for longs, rank 7. </summary>
		public class D7 : ArrayLong
		{
			internal Index7D ix;

			/// <summary>
			/// Constructor for array of shape {len0,len1,len2,len3,len4,len5,len6}. </summary>
			public D7(int len0, int len1, int len2, int len3, int len4, int len5, int len6, bool isUnsigned) : base(new int[] { len0, len1, len2, len3, len4, len5, len6 }, isUnsigned)
			{
				ix = (Index7D)indexCalc;
			}

			internal D7(NcIndex i, bool isUnsigned, long[] store) : base(i, isUnsigned, store)
			{
				ix = (Index7D)indexCalc;
			}

			/// <summary>
			/// get the value. </summary>
			public virtual long get(int i, int j, int k, int l, int m, int n, int o)
			{
				return storage[ix.setDirect(i, j, k, l, m, n, o)];
			}

			/// <summary>
			/// set the value. </summary>
			public virtual void set(int i, int j, int k, int l, int m, int n, int o, long value)
			{
				storage[ix.setDirect(i, j, k, l, m, n, o)] = value;
			}
		}

	}

}
