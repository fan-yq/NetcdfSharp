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
	/// Concrete implementation of Array specialized for Objects.
	/// Data storage is with 1D java array of Objects.
	/// </summary>
	/// <seealso cref="Array"
	/// @author caron/>
	public class ArrayObject : NcArray
	{

		/// <summary>
		/// package private. use Array.factory() </summary>
		internal static ArrayObject factory(DataType dtype, Type elemType, bool isVlen, NcIndex index)
		{
			return ArrayObject.factory(dtype, elemType, isVlen, index, null);
		}

		/*
		 * Create new ArrayObject with given indexImpl and backing store.
		 * Should be private.
		 * 
		 * @param index use this Index
		 * 
		 * @param stor. use this storage. if null, allocate.
		 * 
		 * @return. new ArrayObject.D<rank> or ArrayObject object.
		 */
		internal static ArrayObject factory(DataType dtype, Type elemType, bool isVlen, NcIndex index, object[] storage)
		{
			if (index is Index0D)
			{
				return new ArrayObject.D0(dtype, elemType, isVlen, index, storage);
			}
			else if (index is Index1D)
			{
				return new ArrayObject.D1(dtype, elemType, isVlen, index, storage);
			}
			else if (index is Index2D)
			{
				return new ArrayObject.D2(dtype, elemType, isVlen, index, storage);
			}
			else if (index is Index3D)
			{
				return new ArrayObject.D3(dtype, elemType, isVlen, index, storage);
			}
			else if (index is Index4D)
			{
				return new ArrayObject.D4(dtype, elemType, isVlen, index, storage);
			}
			else if (index is Index5D)
			{
				return new ArrayObject.D5(dtype, elemType, isVlen, index, storage);
			}
			else if (index is Index6D)
			{
				return new ArrayObject.D6(dtype, elemType, isVlen, index, storage);
			}
			else if (index is Index7D)
			{
				return new ArrayObject.D7(dtype, elemType, isVlen, index, storage);
			}
			else
			{
				return new ArrayObject(dtype, elemType, isVlen, index, storage);
			}
		}

		///////////////////////////////////////////////////////////////////////////////
		protected internal readonly bool _isVlen;
		protected internal readonly Type elementType;
		protected internal readonly object[] storage;

		/// <summary>
		/// Create a new Array of type Object and the given shape.
		/// dimensions.length determines the rank of the new Array.
		/// </summary>
		/// <param name="elementType"> the type of element, eg String </param>
		/// <param name="shape"> the shape of the Array. </param>
		public ArrayObject(DataType dtype, Type elementType, bool isVlen, int[] shape) : base(dtype, shape)
		{
			this.elementType = elementType;
			storage = new object[(int)indexCalc.Size];
			this._isVlen = isVlen;
		}


		/// <summary>
		/// Create a new Array of type Object and the given shape and storage.
		/// dimensions.length determines the rank of the new Array.
		/// </summary>
		/// <param name="elementType"> the type of element, eg String </param>
		/// <param name="shape"> the shape of the Array. </param>
		public ArrayObject(DataType dtype, Type elementType, bool isVlen, int[] shape, object[] storage) : base(dtype, shape)
		{
			this.elementType = elementType;
			this.storage = storage;
			this._isVlen = isVlen;
		}

		/// <summary>
		/// Create a new Array using the given IndexArray and backing store.
		/// used for sections, and factory. Trusted package private.
		/// </summary>
		/// <param name="elementType"> the type of element, eg String </param>
		/// <param name="ima"> use this IndexArray as the index </param>
		/// <param name="data"> use this as the backing store. if null, allocate </param>
		internal ArrayObject(DataType dtype, Type elementType, bool isVlen, NcIndex ima, object[] data) : base(dtype, ima)
		{
			this.elementType = elementType;
			this._isVlen = isVlen;

			if (data != null)
			{
				storage = data;
			}
			else
			{
				storage = new object[(int)indexCalc.Size];
			}
		}

		/// <summary>
		/// create new Array with given indexImpl and the same backing store </summary>
		protected internal override NcArray createView(NcIndex index)
		{
			return ArrayObject.factory(dataType, elementType, _isVlen, index, storage);
		}

		public override NcArray copy()
		{
			NcArray newA = factory(DataType, ElementType, Vlen, NcIndex.factory(Shape));
			MAMath.copy(newA, this);
			return newA;
		}

		/// <summary>
		/// Get underlying primitive array storage. CAUTION! You may invalidate your warrentee! </summary>
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
			object[] ja = (object[])javaArray;
			foreach (object aJa in ja)
			{
				iter.ObjectNext = aJa;
			}
		}

		// copy to javaArray from storage using the iterator: used by copyToNDJavaArray;
		protected internal override void copyTo1DJavaArray(IndexIterator iter, object javaArray)
		{
			object[] ja = (object[])javaArray;
			for (int i = 0; i < ja.Length; i++)
			{
				ja[i] = iter.ObjectNext;
			}
		}

		/// <summary>
		/// Return the element class type </summary>
		public override Type ElementType
		{
			get
			{
				return elementType;
			}
		}

		public override bool Vlen
		{
			get
			{
				return _isVlen;
			}
		}

		public override bool isVlen()
		{
			return _isVlen;
		}

		/// <summary>
		/// not legal, throw ForbiddenConversionException </summary>
		public override double getDouble(NcIndex i)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// not legal, throw ForbiddenConversionException </summary>
		public override void setDouble(NcIndex i, double value)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// not legal, throw ForbiddenConversionException </summary>
		public override float getFloat(NcIndex i)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// not legal, throw ForbiddenConversionException </summary>
		public override void setFloat(NcIndex i, float value)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// not legal, throw ForbiddenConversionException </summary>
		public override long getLong(NcIndex i)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// not legal, throw ForbiddenConversionException </summary>
		public override void setLong(NcIndex i, long value)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// not legal, throw ForbiddenConversionException </summary>
		public override int getInt(NcIndex i)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// not legal, throw ForbiddenConversionException </summary>
		public override void setInt(NcIndex i, int value)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// not legal, throw ForbiddenConversionException </summary>
		public override short getShort(NcIndex i)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// not legal, throw ForbiddenConversionException </summary>
		public override void setShort(NcIndex i, short value)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// not legal, throw ForbiddenConversionException </summary>
		public override byte getByte(NcIndex i)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// not legal, throw ForbiddenConversionException </summary>
		public override void setByte(NcIndex i, byte value)
		{
			throw new ForbiddenConversionException();
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

		/// <summary>
		/// not legal, throw ForbiddenConversionException </summary>
		public override char getChar(NcIndex i)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// not legal, throw ForbiddenConversionException </summary>
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
			storage[i.currentElement()] = value;
		}

		// trusted, assumes that individual dimension lengths have been checked
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
			storage[index] = value;
		}

		/// <summary>
		/// Concrete implementation of Array specialized for Objects, rank 0. </summary>
		public class D0 : ArrayObject
		{
			internal Index0D ix;

			/// <summary>
			/// Constructor. </summary>
			public D0(DataType dtype, Type elemType, bool isVlen) : base(dtype, elemType, isVlen, new int[] { })
			{
				ix = (Index0D)indexCalc;
			}

			internal D0(DataType dtype, Type elemType, bool isVlen, NcIndex i, object[] store) : base(dtype, elemType, isVlen, i, store)
			{
				ix = (Index0D)indexCalc;
			}

			/// <summary>
			/// get the value. </summary>
			public virtual object get()
			{
				return storage[ix.currentElement()];
			}

			/// <summary>
			/// set the value. </summary>
			public virtual void set(object value)
			{
				storage[ix.currentElement()] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for Objects, rank 1. </summary>
		public class D1 : ArrayObject
		{
			internal Index1D ix;

			/// <summary>
			/// Constructor for array of shape {len0}. </summary>
			public D1(DataType dtype, Type elemType, bool isVlen, int len0) : base(dtype, elemType, isVlen, new int[] { len0 })
			{
				ix = (Index1D)indexCalc;
			}

			internal D1(DataType dtype, Type elemType, bool isVlen, NcIndex i, object[] store) : base(dtype, elemType, isVlen, i, store)
			{
				ix = (Index1D)indexCalc;
			}

			/// <summary>
			/// get the value. </summary>
			public virtual object get(int i)
			{
				return storage[ix.setDirect(i)];
			}

			/// <summary>
			/// set the value. </summary>
			public virtual void set(int i, object value)
			{
				storage[ix.setDirect(i)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for Objects, rank 2. </summary>
		public class D2 : ArrayObject
		{
			internal Index2D ix;

			/// <summary>
			/// Constructor for array of shape {len0,len1}. </summary>
			public D2(DataType dtype, Type elemType, bool isVlen, int len0, int len1) : base(dtype, elemType, isVlen, new int[] { len0, len1 })
			{
				ix = (Index2D)indexCalc;
			}

			internal D2(DataType dtype, Type elemType, bool isVlen, NcIndex i, object[] store) : base(dtype, elemType, isVlen, i, store)
			{
				ix = (Index2D)indexCalc;
			}

			/// <summary>
			/// get the value. </summary>
			public virtual object get(int i, int j)
			{
				return storage[ix.setDirect(i, j)];
			}

			/// <summary>
			/// set the value. </summary>
			public virtual void set(int i, int j, object value)
			{
				storage[ix.setDirect(i, j)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for Objects, rank 3. </summary>
		public class D3 : ArrayObject
		{
			internal Index3D ix;

			/// <summary>
			/// Constructor for array of shape {len0,len1,len2}. </summary>
			public D3(DataType dtype, Type elemType, bool isVlen, int len0, int len1, int len2) : base(dtype, elemType, isVlen, new int[] { len0, len1, len2 })
			{
				ix = (Index3D)indexCalc;
			}

			internal D3(DataType dtype, Type elemType, bool isVlen, NcIndex i, object[] store) : base(dtype, elemType, isVlen, i, store)
			{
				ix = (Index3D)indexCalc;
			}

			/// <summary>
			/// get the value. </summary>
			public virtual object get(int i, int j, int k)
			{
				return storage[ix.setDirect(i, j, k)];
			}

			/// <summary>
			/// set the value. </summary>
			public virtual void set(int i, int j, int k, object value)
			{
				storage[ix.setDirect(i, j, k)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for Objects, rank 4. </summary>
		public class D4 : ArrayObject
		{
			internal Index4D ix;

			/// <summary>
			/// Constructor for array of shape {len0,len1,len2,len3}. </summary>
			public D4(DataType dtype, Type elemType, bool isVlen, int len0, int len1, int len2, int len3) : base(dtype, elemType, isVlen, new int[] { len0, len1, len2, len3 })
			{
				ix = (Index4D)indexCalc;
			}

			internal D4(DataType dtype, Type elemType, bool isVlen, NcIndex i, object[] store) : base(dtype, elemType, isVlen, i, store)
			{
				ix = (Index4D)indexCalc;
			}

			/// <summary>
			/// get the value. </summary>
			public virtual object get(int i, int j, int k, int l)
			{
				return storage[ix.setDirect(i, j, k, l)];
			}

			/// <summary>
			/// set the value. </summary>
			public virtual void set(int i, int j, int k, int l, object value)
			{
				storage[ix.setDirect(i, j, k, l)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for Objects, rank 5. </summary>
		public class D5 : ArrayObject
		{
			internal Index5D ix;

			/// <summary>
			/// Constructor for array of shape {len0,len1,len2,len3,len4}. </summary>
			public D5(DataType dtype, Type elemType, bool isVlen, int len0, int len1, int len2, int len3, int len4) : base(dtype, elemType, isVlen, new int[] { len0, len1, len2, len3, len4 })
			{
				ix = (Index5D)indexCalc;
			}

			internal D5(DataType dtype, Type elemType, bool isVlen, NcIndex i, object[] store) : base(dtype, elemType, isVlen, i, store)
			{
				ix = (Index5D)indexCalc;
			}

			/// <summary>
			/// get the value. </summary>
			public virtual object get(int i, int j, int k, int l, int m)
			{
				return storage[ix.setDirect(i, j, k, l, m)];
			}

			/// <summary>
			/// set the value. </summary>
			public virtual void set(int i, int j, int k, int l, int m, object value)
			{
				storage[ix.setDirect(i, j, k, l, m)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for Objects, rank 6. </summary>
		public class D6 : ArrayObject
		{
			internal Index6D ix;

			/// <summary>
			/// Constructor for array of shape {len0,len1,len2,len3,len4,len5,}. </summary>
			public D6(DataType dtype, Type elemType, bool isVlen, int len0, int len1, int len2, int len3, int len4, int len5) : base(dtype, elemType, isVlen, new int[] { len0, len1, len2, len3, len4, len5 })
			{
				ix = (Index6D)indexCalc;
			}

			internal D6(DataType dtype, Type elemType, bool isVlen, NcIndex i, object[] store) : base(dtype, elemType, isVlen, i, store)
			{
				ix = (Index6D)indexCalc;
			}

			/// <summary>
			/// get the value. </summary>
			public virtual object get(int i, int j, int k, int l, int m, int n)
			{
				return storage[ix.setDirect(i, j, k, l, m, n)];
			}

			/// <summary>
			/// set the value. </summary>
			public virtual void set(int i, int j, int k, int l, int m, int n, object value)
			{
				storage[ix.setDirect(i, j, k, l, m, n)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for Objects, rank 7. </summary>
		public class D7 : ArrayObject
		{
			internal Index7D ix;

			/// <summary>
			/// Constructor for array of shape {len0,len1,len2,len3,len4,len5,len6}. </summary>
			public D7(DataType dtype, Type elemType, bool isVlen, int len0, int len1, int len2, int len3, int len4, int len5, int len6) : base(dtype, elemType, isVlen, new int[] { len0, len1, len2, len3, len4, len5, len6 })
			{
				ix = (Index7D)indexCalc;
			}

			internal D7(DataType dtype, Type elemType, bool isVlen, NcIndex i, object[] store) : base(dtype, elemType, isVlen, i, store)
			{
				ix = (Index7D)indexCalc;
			}

			/// <summary>
			/// get the value. </summary>
			public virtual object get(int i, int j, int k, int l, int m, int n, int o)
			{
				return storage[ix.setDirect(i, j, k, l, m, n, o)];
			}

			/// <summary>
			/// set the value. </summary>
			public virtual void set(int i, int j, int k, int l, int m, int n, int o, object value)
			{
				storage[ix.setDirect(i, j, k, l, m, n, o)] = value;
			}
		}

	}

}
