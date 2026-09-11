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
	/// Concrete implementation of Array specialized for shorts.
	/// Data storage is with 1D java array of shorts.
	/// <p/>
	/// issues: what should we do if a conversion loses accuracy? nothing ? Exception ?
	/// 
	/// @author caron </summary>
	/// <seealso cref="Array"/>
	public class ArrayShort : NcArray
	{

		// package private. use Array.factory()
		internal static ArrayShort factory(NcIndex index, bool isUnsigned)
		{
			return ArrayShort.factory(index, isUnsigned, null);
		}

		/*
		 * create new ArrayShort with given indexImpl and backing store.
		 * 
		 * @param index use this Index
		 * 
		 * @param stor. use this storage. if null, allocate.
		 * 
		 * @return. new ArrayShort.D<rank> or ArrayShort object.
		 */
		internal static ArrayShort factory(NcIndex index, bool isUnsigned, short[] storage)
		{
			if (index is Index0D)
			{
				return new ArrayShort.D0(index, isUnsigned, storage);
			}
			else if (index is Index1D)
			{
				return new ArrayShort.D1(index, isUnsigned, storage);
			}
			else if (index is Index2D)
			{
				return new ArrayShort.D2(index, isUnsigned, storage);
			}
			else if (index is Index3D)
			{
				return new ArrayShort.D3(index, isUnsigned, storage);
			}
			else if (index is Index4D)
			{
				return new ArrayShort.D4(index, isUnsigned, storage);
			}
			else if (index is Index5D)
			{
				return new ArrayShort.D5(index, isUnsigned, storage);
			}
			else if (index is Index6D)
			{
				return new ArrayShort.D6(index, isUnsigned, storage);
			}
			else if (index is Index7D)
			{
				return new ArrayShort.D7(index, isUnsigned, storage);
			}
			else
			{
				return new ArrayShort(index, isUnsigned, storage);
			}
		}

		///////////////////////////////////////////////////////////////////////////////
		protected internal short[] storage;

		/// <summary>
		/// Create a new Array of type short and the given shape.
		/// dimensions.length determines the rank of the new Array.
		/// </summary>
		/// <param name="dimensions"> the shape of the Array. </param>
		public ArrayShort(int[] dimensions, bool isUnsigned) : base(isUnsigned ? DataType.USHORT : DataType.SHORT, dimensions)
		{
			storage = new short[(int)indexCalc.Size];
		}

		/// <summary>
		/// Create a new Array using the given IndexArray and backing store.
		/// used for sections. Trusted package private.
		/// </summary>
		/// <param name="ima"> use this IndexArray as the index </param>
		/// <param name="data"> use this as the backing store </param>
		internal ArrayShort(NcIndex ima, bool isUnsigned, short[] data) : base(isUnsigned ? DataType.USHORT : DataType.SHORT, ima)
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
				storage = new short[(int)ima.Size];
			}
		}

		/// <summary>
		/// create new Array with given indexImpl and same backing store
		/// </summary>
		protected internal override NcArray createView(NcIndex index)
		{
			return ArrayShort.factory(index, isUnsigned(), storage);
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
			short[] ja = (short[])javaArray;
			foreach (short aJa in ja)
			{
				iter.ShortNext = aJa;
			}
		}

		// copy to javaArray from storage using the iterator: used by copyToNDJavaArray;
		protected internal override void copyTo1DJavaArray(IndexIterator iter, object javaArray)
		{
			short[] ja = (short[])javaArray;
			for (int i = 0; i < ja.Length; i++)
			{
				ja[i] = iter.ShortNext;
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
			ByteBuffer bb = base.getDataAsByteBuffer((int)(2 * Size), order);
			ShortBuffer ib = bb.asShortBuffer();
			ib.put((short[])get1DJavaArray(DataType)); // make sure its in canonical order
			return bb;
		}

		/// <summary>
		/// Return the element class type
		/// </summary>
		public override Type ElementType
		{
			get
			{
				return typeof(short);
			}
		}

		/// <summary>
		/// Get the value at the specified index.
		/// </summary>
		/// <param name="i"> the index </param>
		/// <returns> the value at the specified index. </returns>
		public virtual short get(NcIndex i)
		{
			return storage[i.currentElement()];
		}

		/// <summary>
		/// Set the value at the specified index.
		/// </summary>
		/// <param name="i"> the index </param>
		/// <param name="value"> set to this value </param>
		public virtual void set(NcIndex i, short value)
		{
			storage[i.currentElement()] = value;
		}

		public override double getDouble(NcIndex i)
		{
			short val = storage[i.currentElement()];
			return (double)(isUnsigned() ? DataTypes.unsignedShortToInt(val) : val);
		}

		public override void setDouble(NcIndex i, double value)
		{
			storage[i.currentElement()] = (short)value;
		}

		public override float getFloat(NcIndex i)
		{
			short val = storage[i.currentElement()];
			return (float)(isUnsigned() ? DataTypes.unsignedShortToInt(val) : val);
		}

		public override void setFloat(NcIndex i, float value)
		{
			storage[i.currentElement()] = (short)value;
		}

		public override long getLong(NcIndex i)
		{
			short val = storage[i.currentElement()];
			return (long)(isUnsigned() ? DataTypes.unsignedShortToInt(val) : val);
		}

		public override void setLong(NcIndex i, long value)
		{
			storage[i.currentElement()] = (short)value;
		}

		public override int getInt(NcIndex i)
		{
			short val = storage[i.currentElement()];
			return (isUnsigned() ? DataTypes.unsignedShortToInt(val) : val);
		}

		public override void setInt(NcIndex i, int value)
		{
			storage[i.currentElement()] = (short)value;
		}

		public override short getShort(NcIndex i)
		{
			return storage[i.currentElement()];
		}

		public override void setShort(NcIndex i, short value)
		{
			storage[i.currentElement()] = value;
		}

		public override byte getByte(NcIndex i)
		{
			return (byte)storage[i.currentElement()];
		}

		public override void setByte(NcIndex i, byte value)
		{
			storage[i.currentElement()] = (short)value;
		}

		public override char getChar(NcIndex i)
		{
			short val = storage[i.currentElement()];
			return (char)(isUnsigned() ? DataTypes.unsignedShortToInt(val) : (char)val);
		}

		public override void setChar(NcIndex i, char value)
		{
			storage[i.currentElement()] = (short)value;
		}

		/// <summary>
		/// not legal, throw ForbiddenConversionException
		/// </summary>
		public override bool getBoolean(NcIndex i)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// not legal, throw ForbiddenConversionException
		/// </summary>
		public override void setBoolean(NcIndex i, bool value)
		{
			throw new ForbiddenConversionException();
		}

		public override object getObject(NcIndex i)
		{
			return storage[i.currentElement()]; // Short
		}

		public override void setObject(NcIndex i, object value)
		{
			storage[i.currentElement()] = Convert.ToInt16(value);
		}

		// package private : mostly for iterators
		public override double getDouble(int index)
		{
			short val = storage[index];
			return (double)(isUnsigned() ? DataTypes.unsignedShortToInt(val) : val);
		}

		public override void setDouble(int index, double value)
		{
			storage[index] = (short)value;
		}

		public override float getFloat(int index)
		{
			short val = storage[index];
			return (float)(isUnsigned() ? DataTypes.unsignedShortToInt(val) : val);
		}

		public override void setFloat(int index, float value)
		{
			storage[index] = (short)value;
		}

		public override long getLong(int index)
		{
			short val = storage[index];
			return (long)(isUnsigned() ? DataTypes.unsignedShortToInt(val) : val);
		}

		public override void setLong(int index, long value)
		{
			storage[index] = (short)value;
		}

		public override int getInt(int index)
		{
			short val = storage[index];
			return (isUnsigned() ? DataTypes.unsignedShortToInt(val) : val);
		}

		public override void setInt(int index, int value)
		{
			storage[index] = (short)value;
		}

		public override short getShort(int index)
		{
			return storage[index];
		}

		public override void setShort(int index, short value)
		{
			storage[index] = value;
		}

		public override byte getByte(int index)
		{
			return (byte)storage[index];
		}

		public override void setByte(int index, byte value)
		{
			storage[index] = (short)value;
		}

		public override char getChar(int index)
		{
			short val = storage[index];
			return (char)(isUnsigned() ? DataTypes.unsignedShortToInt(val) : (char)val);
		}

		public override void setChar(int index, char value)
		{
			storage[index] = (short)value;
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
			return getShort(index);
		}

		public override void setObject(int index, object value)
		{
			storage[index] = Convert.ToInt16(value);
		}

		/// <summary>
		/// Concrete implementation of Array specialized for shorts, rank 0.
		/// </summary>
		public class D0 : ArrayShort
		{
			internal Index0D ix;

			public D0(bool isUnsigned) : base(new int[] { }, isUnsigned)
			{
				ix = (Index0D)indexCalc;
			}

			internal D0(NcIndex i, bool isUnsigned, short[] store) : base(i, isUnsigned, store)
			{
				ix = (Index0D)indexCalc;
			}

			public virtual short get()
			{
				return storage[ix.currentElement()];
			}

			public virtual void set(short value)
			{
				storage[ix.currentElement()] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for shorts, rank 1.
		/// </summary>
		public class D1 : ArrayShort
		{
			internal Index1D ix;

			public D1(int len0, bool isUnsigned) : base(new int[] { len0 }, isUnsigned)
			{
				ix = (Index1D)indexCalc;
			}

			internal D1(NcIndex i, bool isUnsigned, short[] store) : base(i, isUnsigned, store)
			{
				ix = (Index1D)indexCalc;
			}

			public virtual short get(int i)
			{
				return storage[ix.setDirect(i)];
			}

			public virtual void set(int i, short value)
			{
				storage[ix.setDirect(i)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for shorts, rank 2.
		/// </summary>
		public class D2 : ArrayShort
		{
			internal Index2D ix;

			public D2(int len0, int len1, bool isUnsigned) : base(new int[] { len0, len1 }, isUnsigned)
			{
				ix = (Index2D)indexCalc;
			}

			internal D2(NcIndex i, bool isUnsigned, short[] store) : base(i, isUnsigned, store)
			{
				ix = (Index2D)indexCalc;
			}

			public virtual short get(int i, int j)
			{
				return storage[ix.setDirect(i, j)];
			}

			public virtual void set(int i, int j, short value)
			{
				storage[ix.setDirect(i, j)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for shorts, rank 3.
		/// </summary>
		public class D3 : ArrayShort
		{
			internal Index3D ix;

			public D3(int len0, int len1, int len2, bool isUnsigned) : base(new int[] { len0, len1, len2 }, isUnsigned)
			{
				ix = (Index3D)indexCalc;
			}

			internal D3(NcIndex i, bool isUnsigned, short[] store) : base(i, isUnsigned, store)
			{
				ix = (Index3D)indexCalc;
			}

			public virtual short get(int i, int j, int k)
			{
				return storage[ix.setDirect(i, j, k)];
			}

			public virtual void set(int i, int j, int k, short value)
			{
				storage[ix.setDirect(i, j, k)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for shorts, rank 4.
		/// </summary>
		public class D4 : ArrayShort
		{
			internal Index4D ix;

			public D4(int len0, int len1, int len2, int len3, bool isUnsigned) : base(new int[] { len0, len1, len2, len3 }, isUnsigned)
			{
				ix = (Index4D)indexCalc;
			}

			internal D4(NcIndex i, bool isUnsigned, short[] store) : base(i, isUnsigned, store)
			{
				ix = (Index4D)indexCalc;
			}

			public virtual short get(int i, int j, int k, int l)
			{
				return storage[ix.setDirect(i, j, k, l)];
			}

			public virtual void set(int i, int j, int k, int l, short value)
			{
				storage[ix.setDirect(i, j, k, l)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for shorts, rank 5.
		/// </summary>
		public class D5 : ArrayShort
		{
			internal Index5D ix;

			public D5(int len0, int len1, int len2, int len3, int len4, bool isUnsigned) : base(new int[] { len0, len1, len2, len3, len4 }, isUnsigned)
			{
				ix = (Index5D)indexCalc;
			}

			internal D5(NcIndex i, bool isUnsigned, short[] store) : base(i, isUnsigned, store)
			{
				ix = (Index5D)indexCalc;
			}

			public virtual short get(int i, int j, int k, int l, int m)
			{
				return storage[ix.setDirect(i, j, k, l, m)];
			}

			public virtual void set(int i, int j, int k, int l, int m, short value)
			{
				storage[ix.setDirect(i, j, k, l, m)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for shorts, rank 6.
		/// </summary>
		public class D6 : ArrayShort
		{
			internal Index6D ix;

			public D6(int len0, int len1, int len2, int len3, int len4, int len5, bool isUnsigned) : base(new int[] { len0, len1, len2, len3, len4, len5 }, isUnsigned)
			{
				ix = (Index6D)indexCalc;
			}

			internal D6(NcIndex i, bool isUnsigned, short[] store) : base(i, isUnsigned, store)
			{
				ix = (Index6D)indexCalc;
			}

			public virtual short get(int i, int j, int k, int l, int m, int n)
			{
				return storage[ix.setDirect(i, j, k, l, m, n)];
			}

			public virtual void set(int i, int j, int k, int l, int m, int n, short value)
			{
				storage[ix.setDirect(i, j, k, l, m, n)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for shorts, rank 7.
		/// </summary>
		public class D7 : ArrayShort
		{
			internal Index7D ix;

			public D7(int len0, int len1, int len2, int len3, int len4, int len5, int len6, bool isUnsigned) : base(new int[] { len0, len1, len2, len3, len4, len5, len6 }, isUnsigned)
			{
				ix = (Index7D)indexCalc;
			}

			internal D7(NcIndex i, bool isUnsigned, short[] store) : base(i, isUnsigned, store)
			{
				ix = (Index7D)indexCalc;
			}

			public virtual short get(int i, int j, int k, int l, int m, int n, int o)
			{
				return storage[ix.setDirect(i, j, k, l, m, n, o)];
			}

			public virtual void set(int i, int j, int k, int l, int m, int n, int o, short value)
			{
				storage[ix.setDirect(i, j, k, l, m, n, o)] = value;
			}
		}

	}

}
