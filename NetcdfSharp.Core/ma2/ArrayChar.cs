// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// Concrete implementation of Array specialized for chars.
	/// Data storage is with 1D java array of chars.
	/// <p/>
	/// issues: what should we do if a conversion loses accuracy? nothing ? Exception ?
	/// 
	/// @author caron </summary>
	/// <seealso cref="Array"/>
	public class ArrayChar : NcArray, IEnumerable<string>
	{

		/// <summary>
		/// package private. use Array.factory()
		/// </summary>
		internal static ArrayChar factory(NcIndex index)
		{
			return ArrayChar.factory(index, null);
		}

		/*
		 * create new ArrayChar with given indexImpl and backing store.
		 * Should be private.
		 * 
		 * @param index use this Index
		 * 
		 * @param stor. use this storage. if null, allocate.
		 * 
		 * @return. new ArrayDouble.D<rank> or ArrayDouble object.
		 */
		internal static ArrayChar factory(NcIndex index, char[] storage)
		{
			if (index is Index0D)
			{
				return new ArrayChar.D0(index, storage);
			}
			else if (index is Index1D)
			{
				return new ArrayChar.D1(index, storage);
			}
			else if (index is Index2D)
			{
				return new ArrayChar.D2(index, storage);
			}
			else if (index is Index3D)
			{
				return new ArrayChar.D3(index, storage);
			}
			else if (index is Index4D)
			{
				return new ArrayChar.D4(index, storage);
			}
			else if (index is Index5D)
			{
				return new ArrayChar.D5(index, storage);
			}
			else if (index is Index6D)
			{
				return new ArrayChar.D6(index, storage);
			}
			else if (index is Index7D)
			{
				return new ArrayChar.D7(index, storage);
			}
			else
			{
				return new ArrayChar(index, storage);
			}
		}

		///////////////////////////////////////////////////////////////////////////////
		protected internal char[] storage;

		/// <summary>
		/// Create a new Array of type char and the given shape.
		/// dimensions.length determines the rank of the new Array.
		/// </summary>
		/// <param name="dimensions"> the shape of the Array. </param>
		public ArrayChar(int[] dimensions) : base(DataType.CHAR, dimensions)
		{
			storage = new char[(int)indexCalc.Size];
		}

		/// <summary>
		/// Create a new Array using the given IndexArray and backing store.
		/// used for sections. Trusted package private.
		/// </summary>
		/// <param name="ima"> use this IndexArray as the index </param>
		/// <param name="data"> use this as the backing store </param>
		internal ArrayChar(NcIndex ima, char[] data) : base(DataType.CHAR, ima)
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
				storage = new char[(int)ima.Size];
			}
		}

		public ArrayChar(string s) : base(DataType.CHAR, new int[] { s.Length })
		{
			storage = new char[s.Length];
			for (int i = 0; i < s.Length; i++)
			{
				storage[i] = s[i];
			}
		}

		/// <summary>
		/// create new Array with given indexImpl and same backing store
		/// </summary>
		protected internal override NcArray createView(NcIndex index)
		{
			return ArrayChar.factory(index, storage);
		}

		// used only by copyTo1DJavaArray
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
			char[] ja = (char[])javaArray;
			foreach (char aJa in ja)
			{
				iter.CharNext = aJa;
			}
		}

		// copy to javaArray from storage using the iterator: used by copyToNDJavaArray;
		protected internal override void copyTo1DJavaArray(IndexIterator iter, object javaArray)
		{
			char[] ja = (char[])javaArray;
			for (int i = 0; i < ja.Length; i++)
			{
				ja[i] = iter.CharNext;
			}
		}

		/// <summary>
		/// Trasfer data to a ByteBuffer.
		/// Note we cast char to byte, discarding top byte, if any.
		/// This is because CDM char is really a byte, not a java char.
		/// </summary>
		/// <returns> data in a ByteBuffer </returns>
		public override ByteBuffer DataAsByteBuffer
		{
			get
			{
				ByteBuffer bb = ByteBuffer.allocate((int)Size);
				resetLocalIterator();
				while (hasNext())
				{
					bb.put(nextByte());
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
				return typeof(char);
			}
		}

		/// <summary>
		/// get the value at the specified index.
		/// </summary>
		public virtual char get(NcIndex i)
		{
			return storage[i.currentElement()];
		}

		/// <summary>
		/// set the value at the sepcified index.
		/// </summary>
		public virtual void set(NcIndex i, char value)
		{
			storage[i.currentElement()] = value;
		}

		public override double getDouble(NcIndex i)
		{
			return (double)storage[i.currentElement()];
		}

		public override void setDouble(NcIndex i, double value)
		{
			storage[i.currentElement()] = (char)value;
		}

		public override float getFloat(NcIndex i)
		{
			return (float)storage[i.currentElement()];
		}

		public override void setFloat(NcIndex i, float value)
		{
			storage[i.currentElement()] = (char)value;
		}

		public override long getLong(NcIndex i)
		{
			return (long)storage[i.currentElement()];
		}

		public override void setLong(NcIndex i, long value)
		{
			storage[i.currentElement()] = (char)value;
		}

		public override int getInt(NcIndex i)
		{
			return (int)storage[i.currentElement()];
		}

		public override void setInt(NcIndex i, int value)
		{
			storage[i.currentElement()] = (char)value;
		}

		public override short getShort(NcIndex i)
		{
			return (short)storage[i.currentElement()];
		}

		public override void setShort(NcIndex i, short value)
		{
			storage[i.currentElement()] = (char)value;
		}

		public override byte getByte(NcIndex i)
		{
			return (byte)storage[i.currentElement()];
		}

		public override void setByte(NcIndex i, byte value)
		{
			storage[i.currentElement()] = (char)value;
		}

		public override char getChar(NcIndex i)
		{
			return storage[i.currentElement()];
		}

		public override void setChar(NcIndex i, char value)
		{
			storage[i.currentElement()] = value;
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
			storage[i.currentElement()] = ((char?)value).Value;
		}

		/// <summary>
		/// Create a String out of this rank one ArrayChar object.
		/// If there is a null (0) value in the ArrayChar array, the String will end there.
		/// The null is not returned as part of the String.
		/// </summary>
		/// <returns> String value of CharArray </returns>
		/// <exception cref="IllegalArgumentException"> if rank != 1 </exception>
		public virtual string String
		{
			get
			{
				int rank = Rank;
				if (rank == 0)
				{
					return new string(storage);
				}
				if (rank != 1)
				{
					throw new ArgumentException("ArayChar.getString rank must be 1");
				}
				int strLen = indexCalc.getShape(0);

				int count = 0;
				for (int k = 0; k < strLen; k++)
				{
					if ((char)0 == storage[k])
					{
						break;
					}
					count++;
				}
				return new string(storage, 0, count);
			}
		}

		public virtual string getString() => String;

		/// <summary>
		/// Create a String out of this rank two ArrayChar object.
		/// This treats the ArrayChar as a 1D array of Strings.
		/// If there is a null (0) value in the ArrayChar array, the String will end there.
		/// The null is not returned as part of the String.
		/// </summary>
		/// <param name="index"> index into 1D String array, must be < getShape(0). </param>
		/// <returns> String value </returns>
		/// <exception cref="IllegalArgumentException"> if rank != 2 </exception>
		public virtual string getString(int index)
		{
			NcIndex ima = Index;
			return getString(ima.set(index));
		}


		/// <summary>
		/// Create a String out of this ArrayChar object. The rank must be 1 or greater.
		/// If there is a null (0) value in the ArrayChar array, the String will end there.
		/// The null is not returned as part of the String.
		/// <p/>
		/// If rank=1, then this will make a string out of the entire CharArray, ignoring ima.
		/// If rank is greater than 1, then make a String out of the characters of the last dimension,
		/// indexed by ima. This method treats the CharArray like an array of Strings, and
		/// allows you to iterate over them, eg for a 2D ArrayChar:
		/// <para>
		/// <code>
		/// ArrayChar ca;
		/// Index ima = ca.getIndex();
		/// for (int i=0; i<ca.getShape()[0]; i++)
		/// String s = ca.getString(ima.set0(i));
		/// </code>
		/// 
		/// </para>
		/// </summary>
		/// <param name="ima"> the Index of where in the Array to start. </param>
		/// <returns> String value of CharArray </returns>
		/// <exception cref="IllegalArgumentException"> if rank != 1 </exception>
		public virtual string getString(NcIndex ima)
		{
			int rank = Rank;
			if (rank == 0)
			{
				throw new ArgumentException("ArayChar.getString rank must not be 0");
			}
			if (rank == 1)
			{
				return String;
			}

			int strLen = indexCalc.getShape(rank - 1);

			char[] carray = new char[strLen];
			int count = 0;
			for (int k = 0; k < strLen; k++)
			{
				ima.setDim(rank - 1, k);
				carray[k] = getChar(ima);
				if ((char)0 == carray[k])
				{
					break;
				}
				count++;
			}
			return new string(carray, 0, count);
		}


		/// <summary>
		/// Set the ArrayChar values from the characters in the String. Rank must be 1.
		/// If String longer than ArrayChar, ignore extra chars; if shorter, fill with 0.
		/// </summary>
		/// <param name="val"> set characters from this String </param>
		/// <exception cref="IllegalArgumentException"> if rank != 2 </exception>
		public virtual void setString(string val)
		{
			int rank = Rank;
			if (rank != 1)
			{
				throw new ArgumentException("ArayChar.setString rank must be 1");
			}
			int arrayLen = indexCalc.getShape(0);
			int strLen = Math.Min(val.Length, arrayLen);

			for (int k = 0; k < strLen; k++)
			{
				storage[k] = val[k];
			}

			char c = (char)0;
			for (int k = strLen; k < arrayLen; k++)
			{
				storage[k] = c;
			}
		}

		/// <summary>
		/// Set the ArrayChar values from the characters in the String. Rank must be 2.
		/// This treats the ArrayChar as a 1D array of Strings.
		/// If String val longer than ArrayChar, ignore extra chars; if shorter, fill with 0.
		/// <p/>
		/// <para>
		/// <code>
		/// String[] val = new String[n];
		/// ArrayChar ca;
		/// Index ima = ca.getIndex();
		/// for (int i=0; i<n; i++)
		/// ca.setString(i, val[i]);
		/// </code>
		/// 
		/// </para>
		/// </summary>
		/// <param name="index"> index into 1D String array, must be < getShape(0). </param>
		/// <param name="val"> set chars from this sString </param>
		public virtual void setString(int index, string val)
		{
			int rank = Rank;
			if (rank != 2)
			{
				throw new ArgumentException("ArrayChar.setString rank must be 2");
			}

			NcIndex ima = Index;
			setString(ima.set(index), val);
		}


		/// <summary>
		/// Set the ArrayChar values from the characters in the String. Rank must be 1 or greater.
		/// If String longer than ArrayChar, ignore extra chars; if shorter, fill with 0.
		/// If rank 1, set entire ArrayChar, ignoring ima.
		/// If rank > 1, treat the ArrayChar like an array of Strings of rank-1, and set the
		/// row indexed by ima. For example, rank 3:
		/// <para>
		/// <code>
		/// String[][] val;
		/// ArrayChar ca;
		/// Index ima = ca.getIndex();
		/// int rank0 = ca.getShape()[0];
		/// int rank1 = ca.getShape()[1];
		/// <p/>
		/// for (int i=0; i<rank0; i++)
		/// for (int j=0; j<rank1; j++) {
		/// ima.set(i,j);
		/// ca.setString(ima, val[i][j]);
		/// }
		/// </code>
		/// 
		/// </para>
		/// </summary>
		/// <param name="ima"> the Index of where in the Array to start. </param>
		/// <param name="val"> set to this value </param>
		public virtual void setString(NcIndex ima, string val)
		{
			int rank = Rank;
			if (rank == 0)
			{
				throw new ArgumentException("ArrayChar.setString rank must not be 0");
			}
			int arrayLen = indexCalc.getShape(rank - 1);
			int strLen = Math.Min(val.Length, arrayLen);

			int count = 0;
			for (int k = 0; k < strLen; k++)
			{
				ima.setDim(rank - 1, k);
				setChar(ima, val[k]);
				count++;
			}
			char c = (char)0;
			for (int k = count; k < arrayLen; k++)
			{
				ima.setDim(rank - 1, k);
				setChar(ima, c);
			}
		}


		// package private : mostly for iterators
		public override double getDouble(int index)
		{
			return (double)storage[index];
		}

		public override void setDouble(int index, double value)
		{
			storage[index] = (char)value;
		}

		public override float getFloat(int index)
		{
			return storage[index];
		}

		public override void setFloat(int index, float value)
		{
			storage[index] = (char)value;
		}

		public override long getLong(int index)
		{
			return (long)storage[index];
		}

		public override void setLong(int index, long value)
		{
			storage[index] = (char)value;
		}

		public override int getInt(int index)
		{
			return (int)storage[index];
		}

		public override void setInt(int index, int value)
		{
			storage[index] = (char)value;
		}

		public override short getShort(int index)
		{
			return (short)storage[index];
		}

		public override void setShort(int index, short value)
		{
			storage[index] = (char)value;
		}

		public override byte getByte(int index)
		{
			return (byte)storage[index];
		}

		public override void setByte(int index, byte value)
		{
			storage[index] = (char)(value & 0xFF);
		}

		public override char getChar(int index)
		{
			return storage[index];
		}

		public override void setChar(int index, char value)
		{
			storage[index] = value;
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
			return getChar(index);
		}

		public override void setObject(int index, object value)
		{
			storage[index] = ((char?)value).Value;
		}

		/// <summary>
		/// Concrete implementation of Array specialized for char, rank 0.
		/// </summary>
		public class D0 : ArrayChar
		{
			internal Index0D ix;

			/// <summary>
			/// Constructor.
			/// </summary>
			public D0() : base(new int[] { })
			{
				ix = (Index0D)indexCalc;
			}

			internal D0(NcIndex i, char[] store) : base(i, store)
			{
				ix = (Index0D)indexCalc;
			}

			/// <summary>
			/// get the value.
			/// </summary>
			public virtual char get()
			{
				return storage[ix.currentElement()];
			}

			/// <summary>
			/// set the value.
			/// </summary>
			public virtual void set(char value)
			{
				storage[ix.currentElement()] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for char, rank 1.
		/// </summary>
		public class D1 : ArrayChar
		{
			internal Index1D ix;

			/// <summary>
			/// Constructor for array of shape {len0}.
			/// </summary>
			public D1(int len0) : base(new int[] { len0 })
			{
				ix = (Index1D)indexCalc;
			}

			internal D1(NcIndex i, char[] store) : base(i, store)
			{
				ix = (Index1D)indexCalc;
			}

			/// <summary>
			/// get the value.
			/// </summary>
			public virtual char get(int i)
			{
				return storage[ix.setDirect(i)];
			}

			/// <summary>
			/// set the value.
			/// </summary>
			public virtual void set(int i, char value)
			{
				storage[ix.setDirect(i)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for char, rank 2.
		/// </summary>
		public class D2 : ArrayChar
		{
			internal Index2D ix;

			/// <summary>
			/// Constructor for array of shape {len0,len1}.
			/// </summary>
			public D2(int len0, int len1) : base(new int[] { len0, len1 })
			{
				ix = (Index2D)indexCalc;
			}

			internal D2(NcIndex i, char[] store) : base(i, store)
			{
				ix = (Index2D)indexCalc;
			}

			/// <summary>
			/// get the value.
			/// </summary>
			public virtual char get(int i, int j)
			{
				return storage[ix.setDirect(i, j)];
			}

			/// <summary>
			/// set the value.
			/// </summary>
			public virtual void set(int i, int j, char value)
			{
				storage[ix.setDirect(i, j)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for char, rank 3.
		/// </summary>
		public class D3 : ArrayChar
		{
			internal Index3D ix;

			/// <summary>
			/// Constructor for array of shape {len0,len1,len2}.
			/// </summary>
			public D3(int len0, int len1, int len2) : base(new int[] { len0, len1, len2 })
			{
				ix = (Index3D)indexCalc;
			}

			internal D3(NcIndex i, char[] store) : base(i, store)
			{
				ix = (Index3D)indexCalc;
			}

			/// <summary>
			/// get the value.
			/// </summary>
			public virtual char get(int i, int j, int k)
			{
				return storage[ix.setDirect(i, j, k)];
			}

			/// <summary>
			/// set the value.
			/// </summary>
			public virtual void set(int i, int j, int k, char value)
			{
				storage[ix.setDirect(i, j, k)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for char, rank 4.
		/// </summary>
		public class D4 : ArrayChar
		{
			internal Index4D ix;

			/// <summary>
			/// Constructor for array of shape {len0,len1,len2,len3}.
			/// </summary>
			public D4(int len0, int len1, int len2, int len3) : base(new int[] { len0, len1, len2, len3 })
			{
				ix = (Index4D)indexCalc;
			}


			internal D4(NcIndex i, char[] store) : base(i, store)
			{
				ix = (Index4D)indexCalc;
			}

			/// <summary>
			/// get the value.
			/// </summary>
			public virtual char get(int i, int j, int k, int l)
			{
				return storage[ix.setDirect(i, j, k, l)];
			}

			/// <summary>
			/// set the value.
			/// </summary>
			public virtual void set(int i, int j, int k, int l, char value)
			{
				storage[ix.setDirect(i, j, k, l)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for char, rank 5.
		/// </summary>
		public class D5 : ArrayChar
		{
			internal Index5D ix;

			/// <summary>
			/// Constructor for array of shape {len0,len1,len2,len3,len4}.
			/// </summary>
			public D5(int len0, int len1, int len2, int len3, int len4) : base(new int[] { len0, len1, len2, len3, len4 })
			{
				ix = (Index5D)indexCalc;
			}

			internal D5(NcIndex i, char[] store) : base(i, store)
			{
				ix = (Index5D)indexCalc;
			}

			/// <summary>
			/// get the value.
			/// </summary>
			public virtual char get(int i, int j, int k, int l, int m)
			{
				return storage[ix.setDirect(i, j, k, l, m)];
			}

			/// <summary>
			/// set the value.
			/// </summary>
			public virtual void set(int i, int j, int k, int l, int m, char value)
			{
				storage[ix.setDirect(i, j, k, l, m)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for char, rank 6.
		/// </summary>
		public class D6 : ArrayChar
		{
			internal Index6D ix;

			/// <summary>
			/// Constructor for array of shape {len0,len1,len2,len3,len4,len5}.
			/// </summary>
			public D6(int len0, int len1, int len2, int len3, int len4, int len5) : base(new int[] { len0, len1, len2, len3, len4, len5 })
			{
				ix = (Index6D)indexCalc;
			}

			internal D6(NcIndex i, char[] store) : base(i, store)
			{
				ix = (Index6D)indexCalc;
			}

			/// <summary>
			/// get the value.
			/// </summary>
			public virtual char get(int i, int j, int k, int l, int m, int n)
			{
				return storage[ix.setDirect(i, j, k, l, m, n)];
			}

			/// <summary>
			/// set the value.
			/// </summary>
			public virtual void set(int i, int j, int k, int l, int m, int n, char value)
			{
				storage[ix.setDirect(i, j, k, l, m, n)] = value;
			}
		}

		/// <summary>
		/// Concrete implementation of Array specialized for char, rank 7.
		/// </summary>
		public class D7 : ArrayChar
		{
			internal Index7D ix;

			/// <summary>
			/// Constructor for array of shape {len0,len1,len2,len3,len4,len5,len6}.
			/// </summary>
			public D7(int len0, int len1, int len2, int len3, int len4, int len5, int len6) : base(new int[] { len0, len1, len2, len3, len4, len5, len6 })
			{
				ix = (Index7D)indexCalc;
			}

			internal D7(NcIndex i, char[] store) : base(i, store)
			{
				ix = (Index7D)indexCalc;
			}

			/// <summary>
			/// get the value.
			/// </summary>
			public virtual char get(int i, int j, int k, int l, int m, int n, int o)
			{
				return storage[ix.setDirect(i, j, k, l, m, n, o)];
			}

			/// <summary>
			/// set the value.
			/// </summary>
			public virtual void set(int i, int j, int k, int l, int m, int n, int o, char value)
			{
				storage[ix.setDirect(i, j, k, l, m, n, o)] = value;
			}
		}

		public override string ToString()
		{
			StringBuilder sbuff = new StringBuilder();
			StringIterator ii = getStringIterator();
			int count = 0;
			while (ii.hasNext())
			{
				if (count > 0)
					sbuff.Append(",");
				string data = ii.next();
				sbuff.Append(data);
				count++;
			}
			return sbuff.ToString();
		}

		///////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Treat this Variable as an array of Strings, and iterate over all the strings in the array.
		/// If rank == 0, will make single String of length 1.
		/// </summary>
		/// <returns> Iterator over Strings. </returns>
		public virtual StringIterator getStringIterator()
		{
			return new StringIterator(this);
		}

		public virtual IEnumerator<string> GetEnumerator()
		{
			return new StringIterator(this);
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// rank must be > 0
		/// </summary>
		public class StringIterator : IEnumerator<string>
		{
			private readonly ArrayChar outerInstance;

			internal IndexIterator ii;
			internal int strLen;
			internal char[] carray;

			internal StringIterator(ArrayChar outerInstance)
			{
				this.outerInstance = outerInstance;
				this.ii = outerInstance.getIndexIterator();
				if (outerInstance.rank == 0)
					strLen = 1;
				else
					strLen = outerInstance.indexCalc.getShape(outerInstance.rank - 1);
				carray = new char[strLen];
			}

			public virtual int getNumElems()
			{
				return (int)outerInstance.Size / strLen;
			}

			public virtual bool hasNext()
			{
				return ii.hasNext();
			}

			public virtual string next()
			{
				int stop = strLen;
				for (int k = 0; k < strLen; k++)
				{
					carray[k] = ii.CharNext;
					if (0 == carray[k] && stop == strLen) // catch zero termination
						stop = k;
				}
				return new string(carray, 0, stop);
			}

			public virtual string Current
			{
				get
				{
					return next();
				}
			}

			public virtual bool MoveNext()
			{
				return hasNext();
			}

			public virtual void Reset()
			{
				throw new System.NotSupportedException();
			}

			public virtual void Dispose()
			{
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return Current;
				}
			}
		}

		/// <summary>
		/// Make this into the equivilent 1D ArrayObject of Strings.
		/// </summary>
		/// <returns> 1D ArrayObject of Strings </returns>
		public virtual ArrayObject make1DStringArray()
		{
			int nelems = (Rank == 0) ? 1 : (int)Size / indexCalc.getShape(Rank - 1);
			NcArray sarr = NcArray.factory(DataType.STRING, new int[] { nelems });
			IndexIterator newsiter = sarr.IndexIterator;

			ArrayChar.StringIterator siter = getStringIterator();
			while (siter.hasNext())
			{
				newsiter.ObjectNext = siter.next();
			}
			return (ArrayObject)sarr;
		}

		/// <summary>
		/// Create an ArrayChar from a String
		/// </summary>
		/// <param name="s"> String </param>
		/// <param name="max"> maximum length </param>
		/// <returns> equivilent ArrayChar </returns>
		public static ArrayChar makeFromString(string s, int max)
		{
			ArrayChar result = new ArrayChar.D1(max);
			for (int i = 0; i < max && i < s.Length; i++)
				result.setChar(i, s[i]);

			return result;
		}

		/// <summary>
		/// Create an ArrayChar from an ArrayObject of Strings.
		/// </summary>
		/// <param name="values"> ArrayObject of String </param>
		/// <returns> equivilent ArrayChar, using maximum length of String. Unused are zero filled. </returns>
		public static ArrayChar makeFromStringArray(ArrayObject values)
		{
			// find longest string
			IndexIterator ii = values.IndexIterator;
			int strlen = 0;
			while (ii.hasNext())
			{
				string s = (string)ii.next();
				strlen = Math.Max(s.Length, strlen);
			}

			return makeFromStringArray(values, strlen);
		}

		/// <summary>
		/// Create an ArrayChar from an ArrayObject of Strings.
		/// Inverse of make1DStringArray. Copies the data.
		/// </summary>
		/// <param name="values"> ArrayObject of String </param>
		/// <param name="strlen"> string length dimension size </param>
		/// <returns> equivilent ArrayChar. Unused are zero filled. </returns>
		public static ArrayChar makeFromStringArray(ArrayObject values, int strlen)
		{
			// create shape for equivilent charArray
			Section section = Section.builder().appendRanges(values.Shape).appendRange(strlen).build();

			int[] shape = section.Shape;
			long size = section.computeSize();

			// populate char array
			char[] cdata = new char[(int)size];
			int start = 0;
			IndexIterator ii = values.IndexIterator;
			while (ii.hasNext())
			{
				string s = (string)ii.next();
				for (int k = 0; k < s.Length && k < strlen; k++)
					cdata[start + k] = s[k];
				start += strlen;
			}

			// ready to create the char Array
			NcArray carr = NcArray.factory(DataType.CHAR, shape, cdata);
			return (ArrayChar)carr;
		}
	}

}
