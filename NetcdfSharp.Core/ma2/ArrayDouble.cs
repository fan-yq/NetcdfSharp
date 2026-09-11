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
	/// Concrete implementation of Array specialized for doubles.
	/// Data storage is with 1D java array of doubles.
	/// </summary>
	/// <seealso cref="Array"
	/// @author caron/>
	public class ArrayDouble : NcArray
	{

		/// <summary>
		/// package private. use Array.factory() </summary>
		internal static ArrayDouble factory(NcIndex index)
		{
			return ArrayDouble.factory(index, null);
		}

		/*
		 * Create new ArrayDouble with given indexImpl and backing store.
		 * Should be private.
		 * 
		 * @param index use this Index
		 * 
		 * @param stor. use this storage. if null, allocate.
		 * 
		 * @return. new ArrayDouble.D<rank> or ArrayDouble object.
		 */
		internal static ArrayDouble factory(NcIndex index, double[] storage)
		{
			if (index is Index0D)
			{
				return new ArrayDouble.D0(index, storage);
			}
			else if (index is Index1D)
			{
				return new ArrayDouble.D1(index, storage);
			}
			else if (index is Index2D)
			{
				return new ArrayDouble.D2(index, storage);
			}
			else if (index is Index3D)
			{
				return new ArrayDouble.D3(index, storage);
			}
			else if (index is Index4D)
			{
				return new ArrayDouble.D4(index, storage);
			}
			else if (index is Index5D)
			{
				return new ArrayDouble.D5(index, storage);
			}
			else if (index is Index6D)
			{
				return new ArrayDouble.D6(index, storage);
			}
			else if (index is Index7D)
			{
				return new ArrayDouble.D7(index, storage);
			}
			else
			{
				return new ArrayDouble(index, storage);
			}
		}

		///////////////////////////////////////////////////////////////////////////////
		protected internal double[] storageD;

		/// <summary>
		/// Create a new Array of type double and the given shape.
		/// dimensions.length determines the rank of the new Array.
		/// </summary>
		/// <param name="shape"> the shape of the Array. </param>
		public ArrayDouble(int[] shape) : base(DataType.DOUBLE, shape)
		{
			storageD = new double[(int)indexCalc.Size];
		}


		/// <summary>
		/// create new Array with given indexImpl and the same backing store </summary>
		protected internal override NcArray createView(NcIndex index)
		{
			return ArrayDouble.factory(index, storageD);
		}

		/// <summary>
		/// Create a new Array using the given IndexArray and backing store.
		/// used for sections, and factory. Trusted package private.
		/// </summary>
		/// <param name="ima"> use this IndexArray as the index </param>
		/// <param name="data"> use this as the backing store. if null, allocate </param>
		internal ArrayDouble(NcIndex ima, double[] data) : base(DataType.DOUBLE, ima)
		{

			if (data != null)
			{
				storageD = data;
			}
			else
			{
				storageD = new double[(int)indexCalc.Size];
			}
		}

		/* Get underlying primitive array storage. CAUTION! You may invalidate your warrentee! */
		public override object Storage
		{
			get
			{
				return storageD;
			}
		}

		// copy from javaArray to storage using the iterator: used by factory( Object);
		protected internal override void copyFrom1DJavaArray(IndexIterator iter, object javaArray)
		{
			double[] ja = (double[])javaArray;
			foreach (double aJa in ja)
			{
				iter.DoubleNext = aJa;
			}
		}

		// copy to javaArray from storage using the iterator: used by copyToNDJavaArray;
		protected internal override void copyTo1DJavaArray(IndexIterator iter, object javaArray)
		{
			double[] ja = (double[])javaArray;
			for (int i = 0; i < ja.Length; i++)
			{
				ja[i] = iter.DoubleNext;
			}
		}

		public override ByteBuffer DataAsByteBuffer
		{
			get
			{
				ByteBuffer bb = ByteBuffer.allocate((int)(8 * Size));
				DoubleBuffer ib = bb.asDoubleBuffer();
				ib.put((double[])get1DJavaArray(DataType.DOUBLE)); // make sure its in canonical order
				return bb;
			}
		}

		/// <summary>
		/// Return the element class type </summary>
		public override Type ElementType
		{
			get
			{
				return typeof(double);
			}
		}

		/// <summary>
		/// get the value at the specified index. </summary>
		public virtual double get(NcIndex i)
		{
			return storageD[i.currentElement()];
		}

		/// <summary>
		/// set the value at the specified index. </summary>
		public virtual void set(NcIndex i, double value)
		{
			storageD[i.currentElement()] = value;
		}

		public override double getDouble(NcIndex i)
		{
			return storageD[i.currentElement()];
		}

		public override void setDouble(NcIndex i, double value)
		{
			storageD[i.currentElement()] = value;
		}

		public override float getFloat(NcIndex i)
		{
			return (float)storageD[i.currentElement()];
		}

		public override void setFloat(NcIndex i, float value)
		{
			storageD[i.currentElement()] = (double)value;
		}

		public override long getLong(NcIndex i)
		{
			return (long)storageD[i.currentElement()];
		}

		public override void setLong(NcIndex i, long value)
		{
			storageD[i.currentElement()] = (double)value;
		}

		public override int getInt(NcIndex i)
		{
			return (int)storageD[i.currentElement()];
		}

		public override void setInt(NcIndex i, int value)
		{
			storageD[i.currentElement()] = (double)value;
		}

		public override short getShort(NcIndex i)
		{
			return (short)storageD[i.currentElement()];
		}

		public override void setShort(NcIndex i, short value)
		{
			storageD[i.currentElement()] = (double)value;
		}

		public override byte getByte(NcIndex i)
		{
			return (byte)storageD[i.currentElement()];
		}

		public override void setByte(NcIndex i, byte value)
		{
			storageD[i.currentElement()] = (double)value;
		}

		public override char getChar(NcIndex i)
		{
			return (char)storageD[i.currentElement()];
		}

		public override void setChar(NcIndex i, char value)
		{
			storageD[i.currentElement()] = (double)value;
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
			return storageD[i.currentElement()];
		}

		public override void setObject(NcIndex i, object value)
		{
			storageD[i.currentElement()] = Convert.ToDouble(value);
		}

		// trusted, assumes that individual dimension lengths have been checked
		public override double getDouble(int index)
		{
			return storageD[index];
		}

		public override void setDouble(int index, double value)
		{
			storageD[index] = value;
		}

		public override float getFloat(int index)
		{
			return (float)storageD[index];
		}

		public override void setFloat(int index, float value)
		{
			storageD[index] = (double)value;
		}

		public override long getLong(int index)
		{
			return (long)storageD[index];
		}

		public override void setLong(int index, long value)
		{
			storageD[index] = (double)value;
		}

		public override int getInt(int index)
		{
			return (int)storageD[index];
		}

		public override void setInt(int index, int value)
		{
			storageD[index] = (double)value;
		}

		public override short getShort(int index)
		{
			return (short)storageD[index];
		}

		public override void setShort(int index, short value)
		{
			storageD[index] = (double)value;
		}

		public override byte getByte(int index)
		{
			return (byte)storageD[index];
		}

		public override void setByte(int index, byte value)
		{
			storageD[index] = (double)value;
		}

		public override char getChar(int index)
		{
			return (char)storageD[index];
		}

		public override void setChar(int index, char value)
		{
			storageD[index] = (double)value;
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
			return getDouble(index);
		}

		public override void setObject(int index, object value)
		{
			storageD[index] = Convert.ToDouble(value);
		}

		/// <summary>
		/// Concrete implementation of Array specialized for doubles, rank 0. </summary>
		public class D0 : ArrayDouble
		{
			internal Index0D ix;

			/// <summary>
			/// Constructor. </summary>
			public D0() : base(new int[] { })
			{
				ix = (Index0D)indexCalc;
			}

			internal D0(NcIndex i, double[] store) : base(i, store)
			{
				ix = (Index0D)indexCalc;
			}

			/// <summary>
			/// get the value. </summary>
			public virtual double get()
			{
				return storageD[ix.currentElement()];
			}

			/// <summary>
			/// set the value. </summary>
			public virtual void set(double value)
			{
				storageD[ix.currentElement()] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for doubles, rank 1. </summary>
		public class D1 : ArrayDouble
		{
			internal Index1D ix;

			/// <summary>
			/// Constructor for array of shape {len0}. </summary>
			public D1(int len0) : base(new int[] { len0 })
			{
				ix = (Index1D)indexCalc;
			}

			internal D1(NcIndex i, double[] store) : base(i, store)
			{
				ix = (Index1D)indexCalc;
			}

			/// <summary>
			/// get the value. </summary>
			public virtual double get(int i)
			{
				return storageD[ix.setDirect(i)];
			}

			/// <summary>
			/// set the value. </summary>
			public virtual void set(int i, double value)
			{
				storageD[ix.setDirect(i)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for doubles, rank 2. </summary>
		public class D2 : ArrayDouble
		{
			internal Index2D ix;

			/// <summary>
			/// Constructor for array of shape {len0,len1}. </summary>
			public D2(int len0, int len1) : base(new int[] { len0, len1 })
			{
				ix = (Index2D)indexCalc;
			}

			internal D2(NcIndex i, double[] store) : base(i, store)
			{
				ix = (Index2D)indexCalc;
			}

			/// <summary>
			/// get the value. </summary>
			public virtual double get(int i, int j)
			{
				return storageD[ix.setDirect(i, j)];
			}

			/// <summary>
			/// set the value. </summary>
			public virtual void set(int i, int j, double value)
			{
				storageD[ix.setDirect(i, j)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for doubles, rank 3. </summary>
		public class D3 : ArrayDouble
		{
			internal Index3D ix;

			/// <summary>
			/// Constructor for array of shape {len0,len1,len2}. </summary>
			public D3(int len0, int len1, int len2) : base(new int[] { len0, len1, len2 })
			{
				ix = (Index3D)indexCalc;
			}

			internal D3(NcIndex i, double[] store) : base(i, store)
			{
				ix = (Index3D)indexCalc;
			}

			/// <summary>
			/// get the value. </summary>
			public virtual double get(int i, int j, int k)
			{
				return storageD[ix.setDirect(i, j, k)];
			}

			/// <summary>
			/// set the value. </summary>
			public virtual void set(int i, int j, int k, double value)
			{
				storageD[ix.setDirect(i, j, k)] = value;
			}

			public virtual IF getIF()
			{
				return new IF(this);
			}

			public class IF
			{
				internal virtual void InitializeInstanceFields()
				{
					size = (int)outerInstance.ix.Size;
				}

				private readonly ArrayDouble.D3 outerInstance;

				public IF(ArrayDouble.D3 outerInstance)
				{
					this.outerInstance = outerInstance;

					InitializeInstanceFields();
				}

				internal int currElement = -1;
				internal int size;

				public virtual bool hasNext(int howMany)
				{
					return currElement < size - howMany;
				}

				public virtual double Next
				{
					get
					{
						return outerInstance.storageD[++currElement];
					}
					set
					{
						outerInstance.storageD[++currElement] = value;
					}
				}

			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for doubles, rank 4. </summary>
		public class D4 : ArrayDouble
		{
			internal Index4D ix;

			/// <summary>
			/// Constructor for array of shape {len0,len1,len2,len3}. </summary>
			public D4(int len0, int len1, int len2, int len3) : base(new int[] { len0, len1, len2, len3 })
			{
				ix = (Index4D)indexCalc;
			}

			internal D4(NcIndex i, double[] store) : base(i, store)
			{
				ix = (Index4D)indexCalc;
			}

			/// <summary>
			/// get the value. </summary>
			public virtual double get(int i, int j, int k, int l)
			{
				return storageD[ix.setDirect(i, j, k, l)];
			}

			/// <summary>
			/// set the value. </summary>
			public virtual void set(int i, int j, int k, int l, double value)
			{
				storageD[ix.setDirect(i, j, k, l)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for doubles, rank 5. </summary>
		public class D5 : ArrayDouble
		{
			internal Index5D ix;

			/// <summary>
			/// Constructor for array of shape {len0,len1,len2,len3,len4}. </summary>
			public D5(int len0, int len1, int len2, int len3, int len4) : base(new int[] { len0, len1, len2, len3, len4 })
			{
				ix = (Index5D)indexCalc;
			}

			internal D5(NcIndex i, double[] store) : base(i, store)
			{
				ix = (Index5D)indexCalc;
			}

			/// <summary>
			/// get the value. </summary>
			public virtual double get(int i, int j, int k, int l, int m)
			{
				return storageD[ix.setDirect(i, j, k, l, m)];
			}

			/// <summary>
			/// set the value. </summary>
			public virtual void set(int i, int j, int k, int l, int m, double value)
			{
				storageD[ix.setDirect(i, j, k, l, m)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for doubles, rank 6. </summary>
		public class D6 : ArrayDouble
		{
			internal Index6D ix;

			/// <summary>
			/// Constructor for array of shape {len0,len1,len2,len3,len4,len5,}. </summary>
			public D6(int len0, int len1, int len2, int len3, int len4, int len5) : base(new int[] { len0, len1, len2, len3, len4, len5 })
			{
				ix = (Index6D)indexCalc;
			}

			internal D6(NcIndex i, double[] store) : base(i, store)
			{
				ix = (Index6D)indexCalc;
			}

			/// <summary>
			/// get the value. </summary>
			public virtual double get(int i, int j, int k, int l, int m, int n)
			{
				return storageD[ix.setDirect(i, j, k, l, m, n)];
			}

			/// <summary>
			/// set the value. </summary>
			public virtual void set(int i, int j, int k, int l, int m, int n, double value)
			{
				storageD[ix.setDirect(i, j, k, l, m, n)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for doubles, rank 7. </summary>
		public class D7 : ArrayDouble
		{
			internal Index7D ix;

			/// <summary>
			/// Constructor for array of shape {len0,len1,len2,len3,len4,len5,len6}. </summary>
			public D7(int len0, int len1, int len2, int len3, int len4, int len5, int len6) : base(new int[] { len0, len1, len2, len3, len4, len5, len6 })
			{
				ix = (Index7D)indexCalc;
			}

			internal D7(NcIndex i, double[] store) : base(i, store)
			{
				ix = (Index7D)indexCalc;
			}

			/// <summary>
			/// get the value. </summary>
			public virtual double get(int i, int j, int k, int l, int m, int n, int o)
			{
				return storageD[ix.setDirect(i, j, k, l, m, n, o)];
			}

			/// <summary>
			/// set the value. </summary>
			public virtual void set(int i, int j, int k, int l, int m, int n, int o, double value)
			{
				storageD[ix.setDirect(i, j, k, l, m, n, o)] = value;
			}
		}

	}

}
