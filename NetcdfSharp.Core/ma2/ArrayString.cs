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
	/// Concrete implementation of Array specialized for Strings.
	/// Data storage is with 1D java array of Strings.
	/// 
	/// @author Heimbigner </summary>
	/// <seealso cref="Array"/>
	public class ArrayString : NcArray
	{

		/// <summary>
		/// package private. use Array.factory()
		/// </summary>
		internal static ArrayString factory(NcIndex index)
		{
			return ArrayString.factory(index, null);
		}

		/*
		 * create new ArrayString with given indexImpl and backing store.
		 * Should be private.
		 * 
		 * @param index use this Index
		 * 
		 * @param stor. use this storage. if null, allocate.
		 * 
		 * @return. new ArrayString.D<rank> or ArrayString object.
		 */
		internal static ArrayString factory(NcIndex index, string[] storage)
		{
			if (index is Index0D)
			{
				return new ArrayString.D0(index, storage);
			}
			else if (index is Index1D)
			{
				return new ArrayString.D1(index, storage);
			}
			else if (index is Index2D)
			{
				return new ArrayString.D2(index, storage);
			}
			else if (index is Index3D)
			{
				return new ArrayString.D3(index, storage);
			}
			else if (index is Index4D)
			{
				return new ArrayString.D4(index, storage);
			}
			else if (index is Index5D)
			{
				return new ArrayString.D5(index, storage);
			}
			else if (index is Index6D)
			{
				return new ArrayString.D6(index, storage);
			}
			else if (index is Index7D)
			{
				return new ArrayString.D7(index, storage);
			}
			else
			{
				return new ArrayString(index, storage);
			}
		}

		//////////////////////////////////////////////////////
		protected internal string[] storage;

		/// <summary>
		/// Create a new Array of type long and the given shape.
		/// dimensions.length determines the rank of the new Array.
		/// </summary>
		/// <param name="dimensions"> the shape of the Array. </param>
		public ArrayString(int[] dimensions) : base(DataType.STRING, dimensions)
		{
			storage = new string[(int)indexCalc.Size];
		}

		/// <summary>
		/// Create a new Array using the given IndexArray and backing store.
		/// used for sections. Trusted package private.
		/// </summary>
		/// <param name="ima"> use this IndexArray as the index </param>
		/// <param name="data"> use this as the backing store </param>
		internal ArrayString(NcIndex ima, string[] data) : base(DataType.STRING, ima)
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
				storage = new string[(int)ima.Size];
			}
		}

		/// <summary>
		/// create new Array with given indexImpl and same backing store
		/// </summary>
		protected internal override NcArray createView(NcIndex index)
		{
			return ArrayString.factory(index, storage);
		}

		/* Get underlying primitive array storage. CAUTION! You may invalidate your warranty! */
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
			string[] ja = (string[])javaArray;
			foreach (string aJa in ja)
			{
				iter.ObjectNext = aJa;
			}
		}

		// copy to javaArray from storage using the iterator: used by copyToNDJavaArray;
		protected internal override void copyTo1DJavaArray(IndexIterator iter, object javaArray)
		{
			string[] ja = (string[])javaArray;
			for (int i = 0; i < ja.Length; i++)
			{
				ja[i] = (string)iter.ObjectNext;
			}
		}

		public override ByteBuffer DataAsByteBuffer
		{
			get
			{
				// Store strings as null terminated character sequences
				int totalsize = 0;
				foreach (string aStorage in storage)
				{
					totalsize += (aStorage.Length + 1); // 1 for null terminator
				}
				ByteBuffer bb = ByteBuffer.allocate(2 * totalsize);
				CharBuffer cb = bb.asCharBuffer();
				// Concatenate
				foreach (string s in storage)
				{
					cb.append(s);
					cb.append('\0');
				}
				return bb;
			}
		}

		/// <summary>
		/// Return the element class type
		/// </summary>
		public override Type ElementType
		{
			get
			{
				return typeof(string);
			}
		}

		/// <summary>
		/// get the value at the specified index.
		/// </summary>
		public virtual string get(NcIndex i)
		{
			return storage[i.currentElement()];
		}

		/// <summary>
		/// set the value at the sepcified index.
		/// </summary>
		public virtual void set(NcIndex i, string value)
		{
			storage[i.currentElement()] = value;
		}

		/// <summary>
		/// not legal, throw ForbiddenConversionException
		/// </summary>
		public override double getDouble(NcIndex i)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// not legal, throw ForbiddenConversionException
		/// </summary>
		public override void setDouble(NcIndex i, double value)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// not legal, throw ForbiddenConversionException
		/// </summary>
		public override float getFloat(NcIndex i)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// not legal, throw ForbiddenConversionException
		/// </summary>
		public override void setFloat(NcIndex i, float value)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// not legal, throw ForbiddenConversionException
		/// </summary>
		public override long getLong(NcIndex i)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// not legal, throw ForbiddenConversionException
		/// </summary>
		public override void setLong(NcIndex i, long value)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// not legal, throw ForbiddenConversionException
		/// </summary>
		public override int getInt(NcIndex i)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// not legal, throw ForbiddenConversionException
		/// </summary>
		public override void setInt(NcIndex i, int value)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// not legal, throw ForbiddenConversionException
		/// </summary>
		public override short getShort(NcIndex i)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// not legal, throw ForbiddenConversionException
		/// </summary>
		public override void setShort(NcIndex i, short value)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// not legal, throw ForbiddenConversionException
		/// </summary>
		public override byte getByte(NcIndex i)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// not legal, throw ForbiddenConversionException
		/// </summary>
		public override void setByte(NcIndex i, byte value)
		{
			throw new ForbiddenConversionException();
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

		/// <summary>
		/// not legal, throw ForbiddenConversionException
		/// </summary>
		public override char getChar(NcIndex i)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// not legal, throw ForbiddenConversionException
		/// </summary>
		public override void setChar(NcIndex i, char value)
		{
			throw new ForbiddenConversionException();
		}

		public override object getObject(NcIndex i)
		{
			return storage[i.currentElement()];
		}

		public override void setObject(NcIndex i, object value)
		{
			storage[i.currentElement()] = ((string)value);
		}

		// package private : mostly for iterators
		public override double getDouble(int index)
		{
			throw new ForbiddenConversionException();
		}

		public override void setDouble(int index, double value)
		{
			throw new ForbiddenConversionException();
		}

		public override float getFloat(int index)
		{
			throw new ForbiddenConversionException();
		}

		public override void setFloat(int index, float value)
		{
			throw new ForbiddenConversionException();
		}

		public override long getLong(int index)
		{
			throw new ForbiddenConversionException();
		}

		public override void setLong(int index, long value)
		{
			throw new ForbiddenConversionException();
		}

		public override int getInt(int index)
		{
			throw new ForbiddenConversionException();
		}

		public override void setInt(int index, int value)
		{
			throw new ForbiddenConversionException();
		}

		public override short getShort(int index)
		{
			throw new ForbiddenConversionException();
		}

		public override void setShort(int index, short value)
		{
			throw new ForbiddenConversionException();
		}

		public override byte getByte(int index)
		{
			throw new ForbiddenConversionException();
		}

		public override void setByte(int index, byte value)
		{
			throw new ForbiddenConversionException();
		}

		public override char getChar(int index)
		{
			throw new ForbiddenConversionException();
		}

		public override void setChar(int index, char value)
		{
			throw new ForbiddenConversionException();
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
			return storage[index];
		}

		public override void setObject(int index, object value)
		{
			storage[index] = (string)value;
		}

		/// <summary>
		/// Concrete implementation of Array specialized for String, rank 0.
		/// </summary>
		public class D0 : ArrayString
		{
			internal Index0D ix;

			/// <summary>
			/// Constructor.
			/// </summary>
			public D0() : base(new int[] { })
			{
				ix = (Index0D)indexCalc;
			}

			internal D0(NcIndex i, string[] store) : base(i, store)
			{
				ix = (Index0D)indexCalc;
			}

			/// <summary>
			/// get the value.
			/// </summary>
			public virtual string get()
			{
				return storage[ix.currentElement()];
			}

			/// <summary>
			/// set the value.
			/// </summary>
			public virtual void set(string value)
			{
				storage[ix.currentElement()] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for Strings, rank 1.
		/// </summary>
		public class D1 : ArrayString
		{
			internal Index1D ix;

			/// <summary>
			/// Constructor for array of shape {len0}.
			/// </summary>
			public D1(int len0) : base(new int[] { len0 })
			{
				ix = (Index1D)indexCalc;
			}

			internal D1(NcIndex i, string[] store) : base(i, store)
			{
				ix = (Index1D)indexCalc;
			}

			/// <summary>
			/// get the value.
			/// </summary>
			public virtual string get(int i)
			{
				return storage[ix.setDirect(i)];
			}

			/// <summary>
			/// set the value.
			/// </summary>
			public virtual void set(int i, string value)
			{
				storage[ix.setDirect(i)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for Strings, rank 2.
		/// </summary>
		public class D2 : ArrayString
		{
			internal Index2D ix;

			/// <summary>
			/// Constructor for array of shape {len0,len1}.
			/// </summary>
			public D2(int len0, int len1) : base(new int[] { len0, len1 })
			{
				ix = (Index2D)indexCalc;
			}

			internal D2(NcIndex i, string[] store) : base(i, store)
			{
				ix = (Index2D)indexCalc;
			}

			/// <summary>
			/// get the value.
			/// </summary>
			public virtual string get(int i, int j)
			{
				return storage[ix.setDirect(i, j)];
			}

			/// <summary>
			/// set the value.
			/// </summary>
			public virtual void set(int i, int j, string value)
			{
				storage[ix.setDirect(i, j)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for Strings, rank 3.
		/// </summary>
		public class D3 : ArrayString
		{
			internal Index3D ix;

			/// <summary>
			/// Constructor for array of shape {len0,len1,len2}.
			/// </summary>
			public D3(int len0, int len1, int len2) : base(new int[] { len0, len1, len2 })
			{
				ix = (Index3D)indexCalc;
			}

			internal D3(NcIndex i, string[] store) : base(i, store)
			{
				ix = (Index3D)indexCalc;
			}

			/// <summary>
			/// get the value.
			/// </summary>
			public virtual string get(int i, int j, int k)
			{
				return storage[ix.setDirect(i, j, k)];
			}

			/// <summary>
			/// set the value.
			/// </summary>
			public virtual void set(int i, int j, int k, string value)
			{
				storage[ix.setDirect(i, j, k)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for Strings, rank 4.
		/// </summary>
		public class D4 : ArrayString
		{
			internal Index4D ix;

			/// <summary>
			/// Constructor for array of shape {len0,len1,len2,len3}.
			/// </summary>
			public D4(int len0, int len1, int len2, int len3) : base(new int[] { len0, len1, len2, len3 })
			{
				ix = (Index4D)indexCalc;
			}

			internal D4(NcIndex i, string[] store) : base(i, store)
			{
				ix = (Index4D)indexCalc;
			}

			/// <summary>
			/// get the value.
			/// </summary>
			public virtual string get(int i, int j, int k, int l)
			{
				return storage[ix.setDirect(i, j, k, l)];
			}

			/// <summary>
			/// set the value.
			/// </summary>
			public virtual void set(int i, int j, int k, int l, string value)
			{
				storage[ix.setDirect(i, j, k, l)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for Strings, rank 5.
		/// </summary>
		public class D5 : ArrayString
		{
			internal Index5D ix;

			/// <summary>
			/// Constructor for array of shape {len0,len1,len2,len3,len4}.
			/// </summary>
			public D5(int len0, int len1, int len2, int len3, int len4) : base(new int[] { len0, len1, len2, len3, len4 })
			{
				ix = (Index5D)indexCalc;
			}

			internal D5(NcIndex i, string[] store) : base(i, store)
			{
				ix = (Index5D)indexCalc;
			}

			/// <summary>
			/// get the value.
			/// </summary>
			public virtual string get(int i, int j, int k, int l, int m)
			{
				return storage[ix.setDirect(i, j, k, l, m)];
			}

			/// <summary>
			/// set the value.
			/// </summary>
			public virtual void set(int i, int j, int k, int l, int m, string value)
			{
				storage[ix.setDirect(i, j, k, l, m)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for Strings, rank 6.
		/// </summary>
		public class D6 : ArrayString
		{
			internal Index6D ix;

			/// <summary>
			/// Constructor for array of shape {len0,len1,len2,len3,len4,len5,}.
			/// </summary>
			public D6(int len0, int len1, int len2, int len3, int len4, int len5) : base(new int[] { len0, len1, len2, len3, len4, len5 })
			{
				ix = (Index6D)indexCalc;
			}

			internal D6(NcIndex i, string[] store) : base(i, store)
			{
				ix = (Index6D)indexCalc;
			}

			/// <summary>
			/// get the value.
			/// </summary>
			public virtual string get(int i, int j, int k, int l, int m, int n)
			{
				return storage[ix.setDirect(i, j, k, l, m, n)];
			}

			/// <summary>
			/// set the value.
			/// </summary>
			public virtual void set(int i, int j, int k, int l, int m, int n, string value)
			{
				storage[ix.setDirect(i, j, k, l, m, n)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for Strings, rank 7.
		/// </summary>
		public class D7 : ArrayString
		{
			internal Index7D ix;

			/// <summary>
			/// Constructor for array of shape {len0,len1,len2,len3,len4,len5,len6}.
			/// </summary>
			public D7(int len0, int len1, int len2, int len3, int len4, int len5, int len6) : base(new int[] { len0, len1, len2, len3, len4, len5, len6 })
			{
				ix = (Index7D)indexCalc;
			}

			internal D7(NcIndex i, string[] store) : base(i, store)
			{
				ix = (Index7D)indexCalc;
			}

			/// <summary>
			/// get the value.
			/// </summary>
			public virtual string get(int i, int j, int k, int l, int m, int n, int o)
			{
				return storage[ix.setDirect(i, j, k, l, m, n, o)];
			}

			/// <summary>
			/// set the value.
			/// </summary>
			public virtual void set(int i, int j, int k, int l, int m, int n, int o, string value)
			{
				storage[ix.setDirect(i, j, k, l, m, n, o)] = value;
			}
		}

	}

}
