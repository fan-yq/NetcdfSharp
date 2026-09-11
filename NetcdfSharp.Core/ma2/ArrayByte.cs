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
	/// Concrete implementation of Array specialized for bytes.
	/// Data storage is with 1D java array of bytes.
	/// <p/>
	/// issues: what should we do if a conversion loses accuracy? nothing ? Exception ?
	/// 
	/// @author caron </summary>
	/// <seealso cref="Array"/>
	public class ArrayByte : NcArray
	{

		// package private. use Array.factory() */
		internal static ArrayByte factory(NcIndex index, bool isUnsigned)
		{
			return ArrayByte.factory(index, isUnsigned, null);
		}

		/*
		 * create new ArrayByte with given Index and backing store.
		 * 
		 * @param index use this Index
		 * 
		 * @param storage use this storage. if null, allocate.
		 * 
		 * @return new ArrayByte.D<rank> or ArrayByte object.
		 */
		internal static ArrayByte factory(NcIndex index, bool isUnsigned, byte[] storage)
		{
			if (index is Index0D)
			{
				return new ArrayByte.D0(index, isUnsigned, storage);
			}
			else if (index is Index1D)
			{
				return new ArrayByte.D1(index, isUnsigned, storage);
			}
			else if (index is Index2D)
			{
				return new ArrayByte.D2(index, isUnsigned, storage);
			}
			else if (index is Index3D)
			{
				return new ArrayByte.D3(index, isUnsigned, storage);
			}
			else if (index is Index4D)
			{
				return new ArrayByte.D4(index, isUnsigned, storage);
			}
			else if (index is Index5D)
			{
				return new ArrayByte.D5(index, isUnsigned, storage);
			}
			else if (index is Index6D)
			{
				return new ArrayByte.D6(index, isUnsigned, storage);
			}
			else if (index is Index7D)
			{
				return new ArrayByte.D7(index, isUnsigned, storage);
			}
			else
			{
				return new ArrayByte(index, isUnsigned, storage);
			}
		}

		///////////////////////////////////////////////////////////////////////////////
		protected internal byte[] storage;

		/// <summary>
		/// Create a new Array of type byte and the given shape.
		/// dimensions.length determines the rank of the new Array.
		/// </summary>
		/// <param name="dimensions"> the shape of the Array. </param>
		public ArrayByte(int[] dimensions, bool isUnsigned) : base(isUnsigned ? DataType.UBYTE : DataType.BYTE, dimensions)
		{
			storage = new byte[(int)indexCalc.Size];
		}

		/// <summary>
		/// Create a new Array using the given IndexArray and backing store.
		/// used for sections. Trusted package private.
		/// </summary>
		/// <param name="ima"> use this IndexArray as the index </param>
		/// <param name="data"> use this as the backing store </param>
		internal ArrayByte(NcIndex ima, bool isUnsigned, byte[] data) : base(isUnsigned ? DataType.UBYTE : DataType.BYTE, ima)
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
				storage = new byte[(int)ima.Size];
			}
		}

		protected internal override NcArray createView(NcIndex index)
		{
			return ArrayByte.factory(index, isUnsigned(), storage);
		}

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
			byte[] ja = (byte[])javaArray;
			foreach (byte aJa in ja)
			{
				iter.ByteNext = aJa;
			}
		}

		// copy to javaArray from storage using the iterator: used by copyToNDJavaArray;
		protected internal override void copyTo1DJavaArray(IndexIterator iter, object javaArray)
		{
			byte[] ja = (byte[])javaArray;
			for (int i = 0; i < ja.Length; i++)
			{
				ja[i] = iter.ByteNext;
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
		{ // order irrelevant here
			return ByteBuffer.wrap((byte[])get1DJavaArray(DataType));
		}

		/// <summary>
		/// Return the element class type
		/// </summary>
		public override Type ElementType
		{
			get
			{
				return typeof(byte);
			}
		}

		/// <summary>
		/// get the value at the specified index.
		/// </summary>
		/// <param name="i"> index </param>
		/// <returns> byte value </returns>
		public virtual byte get(NcIndex i)
		{
			return storage[i.currentElement()];
		}

		/// <summary>
		/// set the value at the sepcified index.
		/// </summary>
		/// <param name="i"> index </param>
		/// <param name="value"> set to this value </param>
		public virtual void set(NcIndex i, byte value)
		{
			storage[i.currentElement()] = value;
		}

		public override double getDouble(NcIndex i)
		{
			byte val = storage[i.currentElement()];
			return (double)(isUnsigned() ? DataTypes.unsignedByteToShort(val) : val);
		}

		public override void setDouble(NcIndex i, double value)
		{
			storage[i.currentElement()] = (byte)value;
		}

		public override float getFloat(NcIndex i)
		{
			byte val = storage[i.currentElement()];
			return (float)(isUnsigned() ? DataTypes.unsignedByteToShort(val) : val);
		}

		public override void setFloat(NcIndex i, float value)
		{
			storage[i.currentElement()] = (byte)value;
		}

		public override long getLong(NcIndex i)
		{
			byte val = storage[i.currentElement()];
			return (long)(isUnsigned() ? DataTypes.unsignedByteToShort(val) : val);
		}

		public override void setLong(NcIndex i, long value)
		{
			storage[i.currentElement()] = (byte)value;
		}

		public override int getInt(NcIndex i)
		{
			byte val = storage[i.currentElement()];
			return (int)(isUnsigned() ? DataTypes.unsignedByteToShort(val) : val);
		}

		public override void setInt(NcIndex i, int value)
		{
			storage[i.currentElement()] = (byte)value;
		}

		public override short getShort(NcIndex i)
		{
			byte val = storage[i.currentElement()];
			return (isUnsigned() ? DataTypes.unsignedByteToShort(val) : val);
		}

		public override void setShort(NcIndex i, short value)
		{
			storage[i.currentElement()] = (byte)value;
		}

		public override byte getByte(NcIndex i)
		{
			return storage[i.currentElement()];
		}

		public override void setByte(NcIndex i, byte value)
		{
			storage[i.currentElement()] = value;
		}

		public override char getChar(NcIndex i)
		{
			byte val = storage[i.currentElement()];
			return (char)(isUnsigned() ? (val & 0xFF) : val);
		}

		public override void setChar(NcIndex i, char value)
		{
			storage[i.currentElement()] = (byte)value;
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
			storage[i.currentElement()] = Convert.ToByte(value);
		}

		// package private : mostly for iterators
		public override double getDouble(int index)
		{
			byte val = storage[index];
			return (double)(isUnsigned() ? DataTypes.unsignedByteToShort(val) : val);
		}

		public override void setDouble(int index, double value)
		{
			storage[index] = (byte)value;
		}

		public override float getFloat(int index)
		{
			byte val = storage[index];
			return (float)(isUnsigned() ? DataTypes.unsignedByteToShort(val) : val);
		}

		public override void setFloat(int index, float value)
		{
			storage[index] = (byte)value;
		}

		public override long getLong(int index)
		{
			byte val = storage[index];
			return (long)(isUnsigned() ? DataTypes.unsignedByteToShort(val) : val);
		}

		public override void setLong(int index, long value)
		{
			storage[index] = (byte)value;
		}

		public override int getInt(int index)
		{
			byte val = storage[index];
			return (int)(isUnsigned() ? DataTypes.unsignedByteToShort(val) : val);
		}

		public override void setInt(int index, int value)
		{
			storage[index] = (byte)value;
		}

		public override short getShort(int index)
		{
			byte val = storage[index];
			return (isUnsigned() ? DataTypes.unsignedByteToShort(val) : val);
		}

		public override void setShort(int index, short value)
		{
			storage[index] = (byte)value;
		}

		public override byte getByte(int index)
		{
			return storage[index];
		}

		public override void setByte(int index, byte value)
		{
			storage[index] = value;
		}

		public override char getChar(int index)
		{
			byte val = storage[index];
			return (char)(isUnsigned() ? (val & 0xFF) : val);
		}

		public override void setChar(int index, char value)
		{
			storage[index] = (byte)value;
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
			return getByte(index);
		}

		public override void setObject(int index, object value)
		{
			storage[index] = Convert.ToByte(value);
		}

		/// <summary>
		/// Concrete implementation of Array specialized for byte, rank 0.
		/// </summary>
		public class D0 : ArrayByte
		{
			internal Index0D ix;

			public D0(bool isUnsigned) : base(new int[] { }, isUnsigned)
			{
				ix = (Index0D)indexCalc;
			}

			internal D0(NcIndex i, bool isUnsigned, byte[] store) : base(i, isUnsigned, store)
			{
				ix = (Index0D)indexCalc;
			}

			public virtual byte get()
			{
				return storage[ix.currentElement()];
			}

			public virtual void set(byte value)
			{
				storage[ix.currentElement()] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for byte, rank 1.
		/// </summary>
		public class D1 : ArrayByte
		{
			internal Index1D ix;

			public D1(int len0, bool isUnsigned) : base(new int[] { len0 }, isUnsigned)
			{
				ix = (Index1D)indexCalc;
			}

			internal D1(NcIndex i, bool isUnsigned, byte[] store) : base(i, isUnsigned, store)
			{
				ix = (Index1D)indexCalc;
			}

			public virtual byte get(int i)
			{
				return storage[ix.setDirect(i)];
			}

			public virtual void set(int i, byte value)
			{
				storage[ix.setDirect(i)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for byte, rank 2.
		/// </summary>
		public class D2 : ArrayByte
		{
			internal Index2D ix;

			public D2(int len0, int len1, bool isUnsigned) : base(new int[] { len0, len1 }, isUnsigned)
			{
				ix = (Index2D)indexCalc;
			}

			internal D2(NcIndex i, bool isUnsigned, byte[] store) : base(i, isUnsigned, store)
			{
				ix = (Index2D)indexCalc;
			}

			public virtual byte get(int i, int j)
			{
				return storage[ix.setDirect(i, j)];
			}

			public virtual void set(int i, int j, byte value)
			{
				storage[ix.setDirect(i, j)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for byte, rank 3.
		/// </summary>
		public class D3 : ArrayByte
		{
			internal Index3D ix;

			public D3(int len0, int len1, int len2, bool isUnsigned) : base(new int[] { len0, len1, len2 }, isUnsigned)
			{
				ix = (Index3D)indexCalc;
			}

			internal D3(NcIndex i, bool isUnsigned, byte[] store) : base(i, isUnsigned, store)
			{
				ix = (Index3D)indexCalc;
			}

			public virtual byte get(int i, int j, int k)
			{
				return storage[ix.setDirect(i, j, k)];
			}

			public virtual void set(int i, int j, int k, byte value)
			{
				storage[ix.setDirect(i, j, k)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for byte, rank 4.
		/// </summary>
		public class D4 : ArrayByte
		{
			internal Index4D ix;

			public D4(int len0, int len1, int len2, int len3, bool isUnsigned) : base(new int[] { len0, len1, len2, len3 }, isUnsigned)
			{
				ix = (Index4D)indexCalc;
			}

			internal D4(NcIndex i, bool isUnsigned, byte[] store) : base(i, isUnsigned, store)
			{
				ix = (Index4D)indexCalc;
			}

			public virtual byte get(int i, int j, int k, int l)
			{
				return storage[ix.setDirect(i, j, k, l)];
			}

			public virtual void set(int i, int j, int k, int l, byte value)
			{
				storage[ix.setDirect(i, j, k, l)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for byte, rank 5.
		/// </summary>
		public class D5 : ArrayByte
		{
			internal Index5D ix;

			public D5(int len0, int len1, int len2, int len3, int len4, bool isUnsigned) : base(new int[] { len0, len1, len2, len3, len4 }, isUnsigned)
			{
				ix = (Index5D)indexCalc;
			}

			internal D5(NcIndex i, bool isUnsigned, byte[] store) : base(i, isUnsigned, store)
			{
				ix = (Index5D)indexCalc;
			}

			public virtual byte get(int i, int j, int k, int l, int m)
			{
				return storage[ix.setDirect(i, j, k, l, m)];
			}

			public virtual void set(int i, int j, int k, int l, int m, byte value)
			{
				storage[ix.setDirect(i, j, k, l, m)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for byte, rank 6.
		/// </summary>
		public class D6 : ArrayByte
		{
			internal Index6D ix;

			public D6(int len0, int len1, int len2, int len3, int len4, int len5, bool isUnsigned) : base(new int[] { len0, len1, len2, len3, len4, len5 }, isUnsigned)
			{
				ix = (Index6D)indexCalc;
			}

			internal D6(NcIndex i, bool isUnsigned, byte[] store) : base(i, isUnsigned, store)
			{
				ix = (Index6D)indexCalc;
			}

			public virtual byte get(int i, int j, int k, int l, int m, int n)
			{
				return storage[ix.setDirect(i, j, k, l, m, n)];
			}

			public virtual void set(int i, int j, int k, int l, int m, int n, byte value)
			{
				storage[ix.setDirect(i, j, k, l, m, n)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for byte, rank 7.
		/// </summary>
		public class D7 : ArrayByte
		{
			protected internal Index7D ix;

			public D7(int len0, int len1, int len2, int len3, int len4, int len5, int len6, bool isUnsigned) : base(new int[] { len0, len1, len2, len3, len4, len5, len6 }, isUnsigned)
			{
				ix = (Index7D)indexCalc;
			}

			internal D7(NcIndex i, bool isUnsigned, byte[] store) : base(i, isUnsigned, store)
			{
				ix = (Index7D)indexCalc;
			}

			public virtual byte get(int i, int j, int k, int l, int m, int n, int o)
			{
				return storage[ix.setDirect(i, j, k, l, m, n, o)];
			}

			public virtual void set(int i, int j, int k, int l, int m, int n, int o, byte value)
			{
				storage[ix.setDirect(i, j, k, l, m, n, o)] = value;
			}
		}

	}

}
