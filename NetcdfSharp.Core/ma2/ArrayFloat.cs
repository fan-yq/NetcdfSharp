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
	/// Concrete implementation of Array specialized for floats.
	/// Data storage is with 1D java array of floats.
	/// 
	/// issues: what should we do if a conversion loses accuracy? nothing ? Exception ?
	/// </summary>
	/// <seealso cref="Array"
	/// @author caron/>
	public class ArrayFloat : NcArray
	{
		/// <summary>
		/// package private. use Array.factory() </summary>
		internal static ArrayFloat factory(NcIndex index)
		{
			return ArrayFloat.factory(index, null);
		}

		/*
		 * create new ArrayFloat with given indexImpl and backing store.
		 * Should be private.
		 * 
		 * @param index use this Index
		 * 
		 * @param stor. use this storage. if null, allocate.
		 * 
		 * @return. new ArrayFloat.D<rank> or ArrayFloat object.
		 */
		internal static ArrayFloat factory(NcIndex index, float[] storage)
		{
			if (index is Index0D)
			{
				return new ArrayFloat.D0(index, storage);
			}
			else if (index is Index1D)
			{
				return new ArrayFloat.D1(index, storage);
			}
			else if (index is Index2D)
			{
				return new ArrayFloat.D2(index, storage);
			}
			else if (index is Index3D)
			{
				return new ArrayFloat.D3(index, storage);
			}
			else if (index is Index4D)
			{
				return new ArrayFloat.D4(index, storage);
			}
			else if (index is Index5D)
			{
				return new ArrayFloat.D5(index, storage);
			}
			else if (index is Index6D)
			{
				return new ArrayFloat.D6(index, storage);
			}
			else if (index is Index7D)
			{
				return new ArrayFloat.D7(index, storage);
			}
			else
			{
				return new ArrayFloat(index, storage);
			}
		}

		//////////////////////////////////////////////////////
		protected internal float[] storage;

		/// <summary>
		/// Create a new Array of type float and the given shape.
		/// dimensions.length determines the rank of the new Array.
		/// </summary>
		/// <param name="dimensions"> the shape of the Array. </param>
		public ArrayFloat(int[] dimensions) : base(DataType.FLOAT, dimensions)
		{
			storage = new float[(int)indexCalc.Size];
		}

		/// <summary>
		/// Create a new Array using the given IndexArray and backing store.
		/// used for sections. Trusted package private.
		/// </summary>
		/// <param name="ima"> use this IndexArray as the index </param>
		/// <param name="data"> use this as the backing store </param>
		internal ArrayFloat(NcIndex ima, float[] data) : base(DataType.FLOAT, ima)
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
				storage = new float[(int)ima.Size];
			}
		}

		/// <summary>
		/// create new Array with given indexImpl and same backing store </summary>
		protected internal override NcArray createView(NcIndex index)
		{
			return ArrayFloat.factory(index, storage);
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
			float[] ja = (float[])javaArray;
			foreach (float aJa in ja)
			{
				iter.FloatNext = aJa;
			}
		}

		// copy to javaArray from storage using the iterator: used by copyToNDJavaArray;
		protected internal override void copyTo1DJavaArray(IndexIterator iter, object javaArray)
		{
			float[] ja = (float[])javaArray;
			for (int i = 0; i < ja.Length; i++)
			{
				ja[i] = iter.FloatNext;
			}
		}

		public override ByteBuffer DataAsByteBuffer
		{
			get
			{
				ByteBuffer bb = ByteBuffer.allocate((int)(4 * Size));
				FloatBuffer ib = bb.asFloatBuffer();
				ib.put((float[])get1DJavaArray(DataType.FLOAT)); // make sure its in canonical order
				return bb;
			}
		}

		/// <summary>
		/// Return the element class type </summary>
		public override Type ElementType
		{
			get
			{
				return typeof(float);
			}
		}

		/// <summary>
		/// get the value at the specified index. </summary>
		public virtual float get(NcIndex i)
		{
			return storage[i.currentElement()];
		}

		/// <summary>
		/// set the value at the sepcified index. </summary>
		public virtual void set(NcIndex i, float value)
		{
			storage[i.currentElement()] = value;
		}

		public override double getDouble(NcIndex i)
		{
			return (double)storage[i.currentElement()];
		}

		public override void setDouble(NcIndex i, double value)
		{
			storage[i.currentElement()] = (float)value;
		}

		public override float getFloat(NcIndex i)
		{
			return storage[i.currentElement()];
		}

		public override void setFloat(NcIndex i, float value)
		{
			storage[i.currentElement()] = value;
		}

		public override long getLong(NcIndex i)
		{
			return (long)storage[i.currentElement()];
		}

		public override void setLong(NcIndex i, long value)
		{
			storage[i.currentElement()] = (float)value;
		}

		public override int getInt(NcIndex i)
		{
			return (int)storage[i.currentElement()];
		}

		public override void setInt(NcIndex i, int value)
		{
			storage[i.currentElement()] = (float)value;
		}

		public override short getShort(NcIndex i)
		{
			return (short)storage[i.currentElement()];
		}

		public override void setShort(NcIndex i, short value)
		{
			storage[i.currentElement()] = (float)value;
		}

		public override byte getByte(NcIndex i)
		{
			return (byte)storage[i.currentElement()];
		}

		public override void setByte(NcIndex i, byte value)
		{
			storage[i.currentElement()] = (float)value;
		}

		public override char getChar(NcIndex i)
		{
			return (char)storage[i.currentElement()];
		}

		public override void setChar(NcIndex i, char value)
		{
			storage[i.currentElement()] = (float)value;
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
			storage[i.currentElement()] = Convert.ToSingle(value);
		}

		// package private : mostly for iterators
		public override double getDouble(int index)
		{
			return (double)storage[index];
		}

		public override void setDouble(int index, double value)
		{
			storage[index] = (float)value;
		}

		public override float getFloat(int index)
		{
			return storage[index];
		}

		public override void setFloat(int index, float value)
		{
			storage[index] = value;
		}

		public override long getLong(int index)
		{
			return (long)storage[index];
		}

		public override void setLong(int index, long value)
		{
			storage[index] = (float)value;
		}

		public override int getInt(int index)
		{
			return (int)storage[index];
		}

		public override void setInt(int index, int value)
		{
			storage[index] = (float)value;
		}

		public override short getShort(int index)
		{
			return (short)storage[index];
		}

		public override void setShort(int index, short value)
		{
			storage[index] = (float)value;
		}

		public override byte getByte(int index)
		{
			return (byte)storage[index];
		}

		public override void setByte(int index, byte value)
		{
			storage[index] = (float)value;
		}

		public override char getChar(int index)
		{
			return (char)storage[index];
		}

		public override void setChar(int index, char value)
		{
			storage[index] = (float)value;
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
			return getFloat(index);
		}

		public override void setObject(int index, object value)
		{
			storage[index] = Convert.ToSingle(value);
		}

		/// <summary>
		/// Concrete implementation of Array specialized for floats, rank 0. </summary>
		public class D0 : ArrayFloat
		{
			internal Index0D ix;

			/// <summary>
			/// Constructor. </summary>
			public D0() : base(new int[] { })
			{
				ix = (Index0D)indexCalc;
			}

			internal D0(NcIndex i, float[] store) : base(i, store)
			{
				ix = (Index0D)indexCalc;
			}

			/// <summary>
			/// get the value. </summary>
			public virtual float get()
			{
				return storage[ix.currentElement()];
			}

			/// <summary>
			/// set the value. </summary>
			public virtual void set(float value)
			{
				storage[ix.currentElement()] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for floats, rank 1. </summary>
		public class D1 : ArrayFloat
		{
			internal Index1D ix;

			/// <summary>
			/// Constructor for array of shape {len0}. </summary>
			public D1(int len0) : base(new int[] { len0 })
			{
				ix = (Index1D)indexCalc;
			}

			internal D1(NcIndex i, float[] store) : base(i, store)
			{
				ix = (Index1D)indexCalc;
			}

			/// <summary>
			/// get the value. </summary>
			public virtual float get(int i)
			{
				return storage[ix.setDirect(i)];
			}

			/// <summary>
			/// set the value. </summary>
			public virtual void set(int i, float value)
			{
				storage[ix.setDirect(i)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for floats, rank 2. </summary>
		public class D2 : ArrayFloat
		{
			internal Index2D ix;

			/// <summary>
			/// Constructor for array of shape {len0,len1}. </summary>
			public D2(int len0, int len1) : base(new int[] { len0, len1 })
			{
				ix = (Index2D)indexCalc;
			}

			internal D2(NcIndex i, float[] store) : base(i, store)
			{
				ix = (Index2D)indexCalc;
			}

			/// <summary>
			/// get the value. </summary>
			public virtual float get(int i, int j)
			{
				return storage[ix.setDirect(i, j)];
			}

			/// <summary>
			/// set the value. </summary>
			public virtual void set(int i, int j, float value)
			{
				storage[ix.setDirect(i, j)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for floats, rank 3. </summary>
		public class D3 : ArrayFloat
		{
			internal Index3D ix;

			/// <summary>
			/// Constructor for array of shape {len0,len1,len2}. </summary>
			public D3(int len0, int len1, int len2) : base(new int[] { len0, len1, len2 })
			{
				ix = (Index3D)indexCalc;
			}

			internal D3(NcIndex i, float[] store) : base(i, store)
			{
				ix = (Index3D)indexCalc;
			}

			/// <summary>
			/// get the value. </summary>
			public virtual float get(int i, int j, int k)
			{
				return storage[ix.setDirect(i, j, k)];
			}

			/// <summary>
			/// set the value. </summary>
			public virtual void set(int i, int j, int k, float value)
			{
				storage[ix.setDirect(i, j, k)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for floats, rank 4. </summary>
		public class D4 : ArrayFloat
		{
			internal Index4D ix;

			/// <summary>
			/// Constructor for array of shape {len0,len1,len2,len3}. </summary>
			public D4(int len0, int len1, int len2, int len3) : base(new int[] { len0, len1, len2, len3 })
			{
				ix = (Index4D)indexCalc;
			}

			internal D4(NcIndex i, float[] store) : base(i, store)
			{
				ix = (Index4D)indexCalc;
			}

			/// <summary>
			/// get the value. </summary>
			public virtual float get(int i, int j, int k, int l)
			{
				return storage[ix.setDirect(i, j, k, l)];
			}

			/// <summary>
			/// set the value. </summary>
			public virtual void set(int i, int j, int k, int l, float value)
			{
				storage[ix.setDirect(i, j, k, l)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for floats, rank 5. </summary>
		public class D5 : ArrayFloat
		{
			internal Index5D ix;

			/// <summary>
			/// Constructor for array of shape {len0,len1,len2,len3,len4}. </summary>
			public D5(int len0, int len1, int len2, int len3, int len4) : base(new int[] { len0, len1, len2, len3, len4 })
			{
				ix = (Index5D)indexCalc;
			}

			internal D5(NcIndex i, float[] store) : base(i, store)
			{
				ix = (Index5D)indexCalc;
			}

			/// <summary>
			/// get the value. </summary>
			public virtual float get(int i, int j, int k, int l, int m)
			{
				return storage[ix.setDirect(i, j, k, l, m)];
			}

			/// <summary>
			/// set the value. </summary>
			public virtual void set(int i, int j, int k, int l, int m, float value)
			{
				storage[ix.setDirect(i, j, k, l, m)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for floats, rank 6. </summary>
		public class D6 : ArrayFloat
		{
			internal Index6D ix;

			/// <summary>
			/// Constructor for array of shape {len0,len1,len2,len3,len4,len5,}. </summary>
			public D6(int len0, int len1, int len2, int len3, int len4, int len5) : base(new int[] { len0, len1, len2, len3, len4, len5 })
			{
				ix = (Index6D)indexCalc;
			}

			internal D6(NcIndex i, float[] store) : base(i, store)
			{
				ix = (Index6D)indexCalc;
			}

			/// <summary>
			/// get the value. </summary>
			public virtual float get(int i, int j, int k, int l, int m, int n)
			{
				return storage[ix.setDirect(i, j, k, l, m, n)];
			}

			/// <summary>
			/// set the value. </summary>
			public virtual void set(int i, int j, int k, int l, int m, int n, float value)
			{
				storage[ix.setDirect(i, j, k, l, m, n)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for floats, rank 7. </summary>
		public class D7 : ArrayFloat
		{
			internal Index7D ix;

			/// <summary>
			/// Constructor for array of shape {len0,len1,len2,len3,len4,len5,len6}. </summary>
			public D7(int len0, int len1, int len2, int len3, int len4, int len5, int len6) : base(new int[] { len0, len1, len2, len3, len4, len5, len6 })
			{
				ix = (Index7D)indexCalc;
			}

			internal D7(NcIndex i, float[] store) : base(i, store)
			{
				ix = (Index7D)indexCalc;
			}

			/// <summary>
			/// get the value. </summary>
			public virtual float get(int i, int j, int k, int l, int m, int n, int o)
			{
				return storage[ix.setDirect(i, j, k, l, m, n, o)];
			}

			/// <summary>
			/// set the value. </summary>
			public virtual void set(int i, int j, int k, int l, int m, int n, int o, float value)
			{
				storage[ix.setDirect(i, j, k, l, m, n, o)] = value;
			}
		}

	}

}
