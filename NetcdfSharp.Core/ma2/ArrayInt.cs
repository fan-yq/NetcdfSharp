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
	/// Concrete implementation of Array specialized for ints.
	/// Data storage is with 1D java array of ints.
	/// <p/>
	/// issues: what should we do if a conversion loses accuracy? nothing ? Exception ?
	/// 
	/// @author caron </summary>
	/// <seealso cref="Array"/>
	public class ArrayInt : NcArray
	{

		// package private. use Array.factory()
		internal static ArrayInt factory(NcIndex index, bool isUnsigned)
		{
			return ArrayInt.factory(index, isUnsigned, null);
		}

		/*
		 * create new ArrayInt with given indexImpl and backing store.
		 * Should be private.
		 * 
		 * @param index use this Index
		 * 
		 * @param stor. use this storage. if null, allocate.
		 * 
		 * @return. new ArrayInt.D<rank> or ArrayInt object.
		 */
		internal static ArrayInt factory(NcIndex index, bool isUnsigned, int[] storage)
		{
			if (index is Index0D)
			{
				return new ArrayInt.D0(index, isUnsigned, storage);
			}
			else if (index is Index1D)
			{
				return new ArrayInt.D1(index, isUnsigned, storage);
			}
			else if (index is Index2D)
			{
				return new ArrayInt.D2(index, isUnsigned, storage);
			}
			else if (index is Index3D)
			{
				return new ArrayInt.D3(index, isUnsigned, storage);
			}
			else if (index is Index4D)
			{
				return new ArrayInt.D4(index, isUnsigned, storage);
			}
			else if (index is Index5D)
			{
				return new ArrayInt.D5(index, isUnsigned, storage);
			}
			else if (index is Index6D)
			{
				return new ArrayInt.D6(index, isUnsigned, storage);
			}
			else if (index is Index7D)
			{
				return new ArrayInt.D7(index, isUnsigned, storage);
			}
			else
			{
				return new ArrayInt(index, isUnsigned, storage);
			}
		}

		///////////////////////////////////////////////////////////////////////////////
		protected internal int[] storage;

		/// <summary>
		/// Create a new Array of type int and the given shape.
		/// dimensions.length determines the rank of the new Array.
		/// </summary>
		/// <param name="dimensions"> the shape of the Array. </param>
		public ArrayInt(int[] dimensions, bool isUnsigned) : base(isUnsigned ? DataType.UINT : DataType.INT, dimensions)
		{
			storage = new int[(int)indexCalc.Size];
		}

		/// <summary>
		/// Create a new Array using the given IndexArray and backing store.
		/// used for sections. Trusted package private.
		/// </summary>
		/// <param name="ima"> use this IndexArray as the index </param>
		/// <param name="data"> use this as the backing store </param>
		internal ArrayInt(NcIndex ima, bool isUnsigned, int[] data) : base(isUnsigned ? DataType.UINT : DataType.INT, ima)
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
				storage = new int[(int)ima.Size];
			}
		}

		/// <summary>
		/// create new Array with given indexImpl and same backing store
		/// </summary>
		protected internal override NcArray createView(NcIndex index)
		{
			return ArrayInt.factory(index, isUnsigned(), storage);
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
			int[] ja = (int[])javaArray;
			foreach (int aJa in ja)
			{
				iter.IntNext = aJa;
			}
		}

		// copy to javaArray from storage using the iterator: used by copyToNDJavaArray;
		protected internal override void copyTo1DJavaArray(IndexIterator iter, object javaArray)
		{
			int[] ja = (int[])javaArray;
			for (int i = 0; i < ja.Length; i++)
			{
				ja[i] = iter.IntNext;
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
			ByteBuffer bb = base.getDataAsByteBuffer((int)(4 * Size), order);
			IntBuffer ib = bb.asIntBuffer();
			ib.put((int[])get1DJavaArray(typeof(int))); // make sure its in canonical order
			return bb;
		}

		/// <summary>
		/// Return the element class type
		/// </summary>
		public override Type ElementType
		{
			get
			{
				return typeof(int);
			}
		}

		/// <summary>
		/// Get the value at the specified index.
		/// </summary>
		/// <param name="i"> the index </param>
		/// <returns> the value at the specified index. </returns>
		public virtual int get(NcIndex i)
		{
			return storage[i.currentElement()];
		}

		/// <summary>
		/// Set the value at the specified index.
		/// </summary>
		/// <param name="i"> the index </param>
		/// <param name="value"> set to this value </param>
		public virtual void set(NcIndex i, int value)
		{
			storage[i.currentElement()] = value;
		}

		public override double getDouble(NcIndex i)
		{
			int val = storage[i.currentElement()];
			return (double)(isUnsigned() ? DataTypes.unsignedIntToLong(val) : val);
		}

		public override void setDouble(NcIndex i, double value)
		{
			storage[i.currentElement()] = (int)value;
		}

		public override float getFloat(NcIndex i)
		{
			int val = storage[i.currentElement()];
			return (float)(isUnsigned() ? DataTypes.unsignedIntToLong(val) : val);
		}

		public override void setFloat(NcIndex i, float value)
		{
			storage[i.currentElement()] = (int)value;
		}

		public override long getLong(NcIndex i)
		{
			int val = storage[i.currentElement()];
			return (isUnsigned() ? DataTypes.unsignedIntToLong(val) : val);
		}

		public override void setLong(NcIndex i, long value)
		{
			storage[i.currentElement()] = (int)value;
		}

		public override int getInt(NcIndex i)
		{
			return storage[i.currentElement()];
		}

		public override void setInt(NcIndex i, int value)
		{
			storage[i.currentElement()] = value;
		}

		public override short getShort(NcIndex i)
		{
			return (short)storage[i.currentElement()];
		}

		public override void setShort(NcIndex i, short value)
		{
			storage[i.currentElement()] = (int)value;
		}

		public override byte getByte(NcIndex i)
		{
			return (byte)storage[i.currentElement()];
		}

		public override void setByte(NcIndex i, byte value)
		{
			storage[i.currentElement()] = (int)value;
		}

		public override char getChar(NcIndex i)
		{
			return (char)storage[i.currentElement()];
		}

		public override void setChar(NcIndex i, char value)
		{
			storage[i.currentElement()] = (int)value;
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
			return storage[i.currentElement()];
		}

		public override void setObject(NcIndex i, object value)
		{
			storage[i.currentElement()] = Convert.ToInt32(value);
		}

		// package private : mostly for iterators
		public override double getDouble(int index)
		{
			int val = storage[index];
			return (double)(isUnsigned() ? DataTypes.unsignedIntToLong(val) : val);
		}

		public override void setDouble(int index, double value)
		{
			storage[index] = (int)value;
		}

		public override float getFloat(int index)
		{
			int val = storage[index];
			return (float)(isUnsigned() ? DataTypes.unsignedIntToLong(val) : val);
		}

		public override void setFloat(int index, float value)
		{
			storage[index] = (int)value;
		}

		public override long getLong(int index)
		{
			int val = storage[index];
			return (isUnsigned() ? DataTypes.unsignedIntToLong(val) : val);
		}

		public override void setLong(int index, long value)
		{
			storage[index] = (int)value;
		}

		public override int getInt(int index)
		{
			return storage[index];
		}

		public override void setInt(int index, int value)
		{
			storage[index] = value;
		}

		public override short getShort(int index)
		{
			return (short)storage[index];
		}

		public override void setShort(int index, short value)
		{
			storage[index] = (int)value;
		}

		public override byte getByte(int index)
		{
			return (byte)storage[index];
		}

		public override void setByte(int index, byte value)
		{
			storage[index] = (int)value;
		}

		public override char getChar(int index)
		{
			return (char)storage[index];
		}

		public override void setChar(int index, char value)
		{
			storage[index] = (int)value;
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
			return getInt(index);
		}

		public override void setObject(int index, object value)
		{
			storage[index] = Convert.ToInt32(value);
		}

		/// <summary>
		/// Concrete implementation of Array specialized for ints, rank 0.
		/// </summary>
		public class D0 : ArrayInt
		{
			internal Index0D ix;

			public D0(bool isUnsigned) : base(new int[] { }, isUnsigned)
			{
				ix = (Index0D)indexCalc;
			}

			internal D0(NcIndex i, bool isUnsigned, int[] store) : base(i, isUnsigned, store)
			{
				ix = (Index0D)indexCalc;
			}

			public virtual int get()
			{
				return storage[ix.currentElement()];
			}

			public virtual void set(int value)
			{
				storage[ix.currentElement()] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for ints, rank 1.
		/// </summary>
		public class D1 : ArrayInt
		{
			internal Index1D ix;

			public D1(int len0, bool isUnsigned) : base(new int[] { len0 }, isUnsigned)
			{
				ix = (Index1D)indexCalc;
			}

			internal D1(NcIndex i, bool isUnsigned, int[] store) : base(i, isUnsigned, store)
			{
				ix = (Index1D)indexCalc;
			}

			public virtual int get(int i)
			{
				return storage[ix.setDirect(i)];
			}

			public virtual void set(int i, int value)
			{
				storage[ix.setDirect(i)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for ints, rank 2.
		/// </summary>
		public class D2 : ArrayInt
		{
			internal Index2D ix;

			public D2(int len0, int len1, bool isUnsigned) : base(new int[] { len0, len1 }, isUnsigned)
			{
				ix = (Index2D)indexCalc;
			}

			internal D2(NcIndex i, bool isUnsigned, int[] store) : base(i, isUnsigned, store)
			{
				ix = (Index2D)indexCalc;
			}

			public virtual int get(int i, int j)
			{
				return storage[ix.setDirect(i, j)];
			}

			public virtual void set(int i, int j, int value)
			{
				storage[ix.setDirect(i, j)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for ints, rank 3.
		/// </summary>
		public class D3 : ArrayInt
		{
			internal Index3D ix;

			public D3(int len0, int len1, int len2, bool isUnsigned) : base(new int[] { len0, len1, len2 }, isUnsigned)
			{
				ix = (Index3D)indexCalc;
			}

			internal D3(NcIndex i, bool isUnsigned, int[] store) : base(i, isUnsigned, store)
			{
				ix = (Index3D)indexCalc;
			}

			public virtual int get(int i, int j, int k)
			{
				return storage[ix.setDirect(i, j, k)];
			}

			public virtual void set(int i, int j, int k, int value)
			{
				storage[ix.setDirect(i, j, k)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for ints, rank 4.
		/// </summary>
		public class D4 : ArrayInt
		{
			internal Index4D ix;

			public D4(int len0, int len1, int len2, int len3, bool isUnsigned) : base(new int[] { len0, len1, len2, len3 }, isUnsigned)
			{
				ix = (Index4D)indexCalc;
			}

			internal D4(NcIndex i, bool isUnsigned, int[] store) : base(i, isUnsigned, store)
			{
				ix = (Index4D)indexCalc;
			}

			public virtual int get(int i, int j, int k, int l)
			{
				return storage[ix.setDirect(i, j, k, l)];
			}

			public virtual void set(int i, int j, int k, int l, int value)
			{
				storage[ix.setDirect(i, j, k, l)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for ints, rank 5.
		/// </summary>
		public class D5 : ArrayInt
		{
			internal Index5D ix;

			public D5(int len0, int len1, int len2, int len3, int len4, bool isUnsigned) : base(new int[] { len0, len1, len2, len3, len4 }, isUnsigned)
			{
				ix = (Index5D)indexCalc;
			}

			internal D5(NcIndex i, bool isUnsigned, int[] store) : base(i, isUnsigned, store)
			{
				ix = (Index5D)indexCalc;
			}

			public virtual int get(int i, int j, int k, int l, int m)
			{
				return storage[ix.setDirect(i, j, k, l, m)];
			}

			public virtual void set(int i, int j, int k, int l, int m, int value)
			{
				storage[ix.setDirect(i, j, k, l, m)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for ints, rank 6.
		/// </summary>
		public class D6 : ArrayInt
		{
			internal Index6D ix;

			public D6(int len0, int len1, int len2, int len3, int len4, int len5, bool isUnsigned) : base(new int[] { len0, len1, len2, len3, len4, len5 }, isUnsigned)
			{
				ix = (Index6D)indexCalc;
			}

			internal D6(NcIndex i, bool isUnsigned, int[] store) : base(i, isUnsigned, store)
			{
				ix = (Index6D)indexCalc;
			}

			public virtual int get(int i, int j, int k, int l, int m, int n)
			{
				return storage[ix.setDirect(i, j, k, l, m, n)];
			}

			public virtual void set(int i, int j, int k, int l, int m, int n, int value)
			{
				storage[ix.setDirect(i, j, k, l, m, n)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for ints, rank 7.
		/// </summary>
		public class D7 : ArrayInt
		{
			internal Index7D ix;

			public D7(int len0, int len1, int len2, int len3, int len4, int len5, int len6, bool isUnsigned) : base(new int[] { len0, len1, len2, len3, len4, len5, len6 }, isUnsigned)
			{
				ix = (Index7D)indexCalc;
			}

			internal D7(NcIndex i, bool isUnsigned, int[] store) : base(i, isUnsigned, store)
			{
				ix = (Index7D)indexCalc;
			}

			public virtual int get(int i, int j, int k, int l, int m, int n, int o)
			{
				return storage[ix.setDirect(i, j, k, l, m, n, o)];
			}

			public virtual void set(int i, int j, int k, int l, int m, int n, int o, int value)
			{
				storage[ix.setDirect(i, j, k, l, m, n, o)] = value;
			}
		}

	}

}
