// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// Superclass for implementations of multidimensional arrays. An Array has a <b>classType</b> which gives
	/// the Class of its elements, and a <b>shape</b> which describes the number of elements in each index.
	/// The <b>rank</b> is the number of indices. A <b>scalar</b> Array has rank = 0. An
	/// Array may have arbitrary rank. The Array <b>size</b> is the total number of elements, which must be less than
	/// 2^31 (about 2x10^9).
	/// <p/>
	/// Actual data storage is done with Java 1D arrays and stride index calculations.
	/// This makes our Arrays rectangular, i.e. no "ragged arrays" where different elements
	/// can have different lengths as in Java multidimensional arrays, which are arrays of arrays.
	/// <p/>
	/// Each primitive Java type (boolean, byte, char, short, int, long, float, double) has a corresponding concrete
	/// implementation, e.g. ArrayBoolean, ArrayDouble. Reference types are all implemented using the ArrayObject class,
	/// with the exceptions of the reference types that correspond to the primitive types, eg Double.class is mapped to
	/// typeof(double).
	/// <p/>
	/// For efficiency, each Array type implementation has concrete subclasses for ranks 0-7, eg ArrayDouble.D0 is a double
	/// array of rank 0, ArrayDouble.D1 is a double array of rank 1, etc. These type and rank specific classes are convenient
	/// to work with when you know the type and rank of the Array. Ranks greater than 7 are handled by the type-specific
	/// superclass e.g. ArrayDouble. The Array class itself is used for fully general handling of any type and rank array.
	/// Use the Array.factory() methods to create Arrays in a general way.
	/// <p/>
	/// The stride index calculations allow <b>logical views</b> to be efficiently implemented, eg subset, transpose, slice,
	/// etc.
	/// These views use the same data storage as the original Array they are derived from. The index stride calculations are
	/// equally efficient for any composition of logical views.
	/// <p/>
	/// The type, shape and backing storage of an Array are immutable.
	/// The data itself is read or written using an Index or an IndexIterator, which stores any needed state information
	/// for efficient traversal. This makes use of Arrays thread-safe (as long as you don't share the Index or IndexIterator)
	/// except for the possibility of non-atomic read/write on long/doubles. If this is the case, you should probably
	/// synchronize your calls. Presumably 64-bit CPUs will make those operations atomic also.
	/// 
	/// @author caron </summary>
	/// <seealso cref="Index"/>
	/// <seealso cref="IndexIterator"/>
	public abstract class NcArray
	{

		/// <summary>
		/// Generate new Array with given dataType and shape and zeroed storage.
		/// </summary>
		/// <param name="dataType"> instance of DataType. </param>
		/// <param name="shape"> shape of the array. </param>
		/// <returns> new Array<type> or Array<type>.D<rank> if 0 <= rank <= 7. </returns>
		public static NcArray factory(DataType dataType, int[] shape)
		{
			return factory(dataType, NetcdfSharp.ucar.ma2.NcIndex.factory(shape), null);
		}

		/// <summary>
		/// Generate new Array with given dataType, shape, storage.
		/// </summary>
		/// <param name="dataType"> DataType, eg DataType.DOUBLE. </param>
		/// <param name="shape"> shape of the array. </param>
		/// <param name="storage"> primitive array of correct type </param>
		/// <returns> new Array<type> or Array<type>.D<rank> if 0 <= rank <= 7. </returns>
		/// <exception cref="ClassCastException"> wrong storage type </exception>
		public static NcArray factory(DataType dataType, int[] shape, object storage)
		{
			return factory(dataType, NetcdfSharp.ucar.ma2.NcIndex.factory(shape), storage);
		}

		/* generate new Array with given type, index and storage */
		public static NcArray factory(DataType dtype, NcIndex index, object storage)
		{
			switch (dtype)
			{
				case NetcdfSharp.ucar.ma2.DataType.DOUBLE:
					return ArrayDouble.factory(index, (double[])storage);
				case NetcdfSharp.ucar.ma2.DataType.FLOAT:
					return ArrayFloat.factory(index, (float[])storage);
				case NetcdfSharp.ucar.ma2.DataType.CHAR:
					return ArrayChar.factory(index, (char[])storage);
				case NetcdfSharp.ucar.ma2.DataType.BOOLEAN:
					return ArrayBoolean.factory(index, (bool[])storage);

				case NetcdfSharp.ucar.ma2.DataType.ENUM4:
				case NetcdfSharp.ucar.ma2.DataType.UINT:
				case NetcdfSharp.ucar.ma2.DataType.INT:
					return ArrayInt.factory(index, dtype.isUnsigned(), (int[])storage);
				case NetcdfSharp.ucar.ma2.DataType.ENUM2:
				case NetcdfSharp.ucar.ma2.DataType.USHORT:
				case NetcdfSharp.ucar.ma2.DataType.SHORT:
					return ArrayShort.factory(index, dtype.isUnsigned(), (short[])storage);
				case NetcdfSharp.ucar.ma2.DataType.ENUM1:
				case NetcdfSharp.ucar.ma2.DataType.UBYTE:
				case NetcdfSharp.ucar.ma2.DataType.BYTE:
					return ArrayByte.factory(index, dtype.isUnsigned(), (byte[])storage);
				case NetcdfSharp.ucar.ma2.DataType.ULONG:
				case NetcdfSharp.ucar.ma2.DataType.LONG:
					return ArrayLong.factory(index, dtype.isUnsigned(), (long[])storage);

				case NetcdfSharp.ucar.ma2.DataType.STRING:
					return ArrayObject.factory(dtype, typeof(string), false, index, (object[])storage);
				case NetcdfSharp.ucar.ma2.DataType.STRUCTURE:
					return ArrayObject.factory(dtype, typeof(StructureData), false, index, (object[])storage);
				case NetcdfSharp.ucar.ma2.DataType.SEQUENCE:
					return ArrayObject.factory(dtype, typeof(StructureDataIterator), false, index, (object[])storage);
				case NetcdfSharp.ucar.ma2.DataType.OPAQUE:
					return ArrayObject.factory(dtype, typeof(ByteBuffer), false, index, (object[])storage);
			}

			throw new Exception("Cant use this method for datatype " + dtype);

			// used for VLEN ??
			// default:
			// return ArrayObject.factory(DataType.OBJECT, typeof(object), index, (Object[]) storage); // LOOK dont know the
			// object class
			// }
		}

		/// <summary>
		/// Make a vlen array
		/// </summary>
		/// <param name="shape"> the outer shape, ie excluding the vlen dimension </param>
		/// <param name="storage"> must be an Array type. must not be null </param>
		/// <returns> ArrayObject </returns>
		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: public static Array makeVlenArray(int[] shape, @Nonnull Array[] storage)
		public static NcArray makeVlenArray(int[] shape, NcArray[] storage)
		{
			NcIndex index = NetcdfSharp.ucar.ma2.NcIndex.factory(shape);
			return ArrayObject.factory(storage[0].DataType, storage[0].GetType(), true, index, storage);
		}

		/// <summary>
		/// Generate new Array with given type, shape, storage.
		/// This should be package private, but is exposed for efficiency.
		/// Normally use factory( Class classType, int [] shape) instead.
		/// storage must be 1D array of type classType.
		/// storage.length must equal product of shapes
		/// storage data needs to be in canonical order
		/// </summary>
		/// <param name="classType"> element class type, eg typeof(double). Corresponding Object types like Double.class are
		///        mapped to typeof(double). Any reference types use ArrayObject. </param>
		/// <param name="shape"> array shape </param>
		/// <param name="storage"> 1D java array of type classType, except object types like Double.class are mapped to
		///        their corresponding primitive type, eg typeof(double). </param>
		/// <returns> Array of given type, shape and storage </returns>
		/// <exception cref="IllegalArgumentException"> storage.length != product of shapes </exception>
		/// <exception cref="ClassCastException"> wrong storage type </exception>
		public static NcArray makeObjectArray(DataType dtype, Type classType, int[] shape, object storage)
		{
			NcIndex index = NetcdfSharp.ucar.ma2.NcIndex.factory(shape);
			return ArrayObject.factory(dtype, classType, false, index, (object[])storage);
		}

		/// <summary>
		/// Generate new Array with given type and shape and an Index that always return 0.
		/// </summary>
		/// <param name="dtype"> data type </param>
		/// <param name="shape"> shape of the array. </param>
		/// <param name="storage"> primitive array of correct type of length 1 </param>
		/// <returns> new Array<type> or Array<type>.D<rank> if 0 <= rank <= 7. </returns>
		public static NcArray factoryConstant(DataType dtype, int[] shape, object storage)
		{
			NcIndex index = new IndexConstant(shape);
			// can't go through the factory, must call the general constructor
			switch (dtype)
			{
				case NetcdfSharp.ucar.ma2.DataType.BOOLEAN:
					return new ArrayBoolean(index, (bool[])storage);
				case NetcdfSharp.ucar.ma2.DataType.BYTE:
					return new ArrayByte(index, false, (byte[])storage);
				case NetcdfSharp.ucar.ma2.DataType.CHAR:
					return new ArrayChar(index, (char[])storage);
				case NetcdfSharp.ucar.ma2.DataType.SHORT:
					return new ArrayShort(index, false, (short[])storage);
				case NetcdfSharp.ucar.ma2.DataType.INT:
					return new ArrayInt(index, false, (int[])storage);
				case NetcdfSharp.ucar.ma2.DataType.LONG:
					return new ArrayLong(index, false, (long[])storage);
				case NetcdfSharp.ucar.ma2.DataType.FLOAT:
					return new ArrayFloat(index, (float[])storage);
				case NetcdfSharp.ucar.ma2.DataType.DOUBLE:
					return new ArrayDouble(index, (double[])storage);
				case NetcdfSharp.ucar.ma2.DataType.ENUM1:
				case NetcdfSharp.ucar.ma2.DataType.UBYTE:
					return new ArrayByte(index, true, (byte[])storage);
				case NetcdfSharp.ucar.ma2.DataType.ENUM2:
				case NetcdfSharp.ucar.ma2.DataType.USHORT:
					return new ArrayShort(index, true, (short[])storage);
				case NetcdfSharp.ucar.ma2.DataType.ENUM4:
				case NetcdfSharp.ucar.ma2.DataType.UINT:
					return new ArrayInt(index, true, (int[])storage);
				case NetcdfSharp.ucar.ma2.DataType.ULONG:
					return new ArrayLong(index, true, (long[])storage);

				case NetcdfSharp.ucar.ma2.DataType.STRING:
					return new ArrayObject(dtype, typeof(string), false, index, (object[])storage);
				case NetcdfSharp.ucar.ma2.DataType.STRUCTURE:
					return new ArrayObject(dtype, typeof(StructureData), false, index, (object[])storage);
				case NetcdfSharp.ucar.ma2.DataType.SEQUENCE:
					return new ArrayObject(dtype, typeof(StructureDataIterator), false, index, (object[])storage);
				case NetcdfSharp.ucar.ma2.DataType.OPAQUE:
					return new ArrayObject(dtype, typeof(ByteBuffer), false, index, (object[])storage);

				default:
					return ArrayObject.factory(NetcdfSharp.ucar.ma2.DataType.OBJECT, typeof(object), false, index, (object[])storage); // LOOK dont know
																														   // the object class
			}
		}

		public static NcArray makeFromJavaArray(object javaArray)
		{
			return makeFromJavaArray(javaArray, false);
		}

		/// <summary>
		/// Generate a new Array from a java array of any rank and type.
		/// This makes a COPY of the data values of javaArray of primitive type
		/// LOOK: not sure this works for reference types.
		/// </summary>
		/// <param name="javaArray"> scalar Object or a java array of any rank and type </param>
		/// <returns> Array of the appropriate rank and type, with the data copied from javaArray. </returns>
		public static NcArray makeFromJavaArray(object javaArray, bool isUnsigned)
		{
			// get the rank and type
			int rank_ = 0;
			Type componentType = javaArray.GetType();
			while (componentType.IsArray)
			{
				rank_++;
				componentType = componentType.GetElementType();
			}

			// get the shape
			int count = 0;
			int[] shape = new int[rank_];
			object jArray = javaArray;
			Type cType = jArray.GetType();
			while (cType.IsArray)
			{
				shape[count++] = ((System.Array)jArray).Length;
				jArray = ((System.Array)jArray).GetValue(0);
				cType = jArray.GetType();
			}

			// create the Array
			DataType dtype = DataTypes.getType(componentType, isUnsigned);
			NcArray aa = factory(dtype, shape);

			// copy the original array
			IndexIterator aaIter = aa.IndexIterator;
			reflectArrayCopyIn(javaArray, aa, aaIter);

			return aa;
		}

		private static void reflectArrayCopyIn(object jArray, NcArray aa, IndexIterator aaIter)
		{
			Type cType = jArray.GetType().GetElementType();
			if (cType.IsPrimitive)
			{
				aa.copyFrom1DJavaArray(aaIter, jArray); // subclass does type-specific copy
			}
			else
			{
				for (int i = 0; i < ((System.Array)jArray).Length; i++) // recurse
				{
					reflectArrayCopyIn(((System.Array)jArray).GetValue(i), aa, aaIter);
				}
			}
		}

		private static void reflectArrayCopyOut(object jArray, NcArray aa, IndexIterator aaIter)
		{
			Type cType = jArray.GetType().GetElementType();
			// if (cType.isPrimitive()) { // Rob Weingruber <weingrub@rap.ucar.edu> May 11, 2011
			if (!cType.IsArray)
			{
				aa.copyTo1DJavaArray(aaIter, jArray); // subclass does type-specific copy
			}
			else
			{
				for (int i = 0; i < ((System.Array)jArray).Length; i++) // recurse
				{
					reflectArrayCopyOut(((System.Array)jArray).GetValue(i), aa, aaIter);
				}
			}
		}

		/// <summary>
		/// Cover for System.arraycopy(). Works with the underlying data arrays.
		/// ArraySrc and ArrayDst must be the same primitive type.
		/// Exposed for efficiency; use at your own risk.
		/// </summary>
		/// <param name="arraySrc"> copy from here : if not in canonical order, an extra copy will be done </param>
		/// <param name="srcPos"> starting at </param>
		/// <param name="arrayDst"> copy to here : must be in canonical order </param>
		/// <param name="dstPos"> starting at </param>
		/// <param name="len"> number of elements to copy </param>
		public static void arraycopy(NcArray arraySrc, int srcPos, NcArray arrayDst, int dstPos, int len)
		{
			// deal with special case
			if (arraySrc.Constant)
			{
				double d = arraySrc.getDouble(0);
				for (int i = dstPos; i < dstPos + len; i++)
				{
					arrayDst.setDouble(i, d);
				}
				return;
			}

			object src = arraySrc.get1DJavaArray(arraySrc.DataType); // ensure canonical order
			object dst = arrayDst.Storage;
			System.Array.Copy((System.Array)src, srcPos, (System.Array)dst, dstPos, len);
		}

		/// <summary>
		/// Make a 1D array from a start and incr.
		/// </summary>
		/// <param name="dtype"> data type of result. must be convertible to double. </param>
		/// <param name="npts"> number of points </param>
		/// <param name="start"> starting values </param>
		/// <param name="incr"> increment </param>
		/// <returns> 1D array </returns>
		public static NcArray makeArray(DataType dtype, int npts, double start, double incr)
		{
			NcArray result = NcArray.factory(dtype, new int[] { npts });
			IndexIterator dataI = result.IndexIterator;
			for (int i = 0; i < npts; i++)
			{
				double val = start + i * incr;
				dataI.DoubleNext = val;
			}
			return result;
		}

		/// <summary>
		/// Make an 1D array from a list of strings.
		/// </summary>
		/// <param name="dtype"> data type of the array. </param>
		/// <param name="stringValues"> list of strings. </param>
		/// <returns> resulting 1D array. </returns>
		/// <exception cref="NumberFormatException"> if string values not parseable to specified data type </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public static Array makeArray(DataType dtype, NetcdfSharp.java.util.List<String> stringValues) throws NumberFormatException
		public static NcArray makeArray(DataType dtype, IList<string> stringValues)
		{
			NcArray result = NcArray.factory(dtype, new int[] { stringValues.Count });
			IndexIterator dataI = result.IndexIterator;

			foreach (string s in stringValues)
			{
				if (dtype == NetcdfSharp.ucar.ma2.DataType.STRING)
				{
					dataI.ObjectNext = s;

				}
				else if (dtype == NetcdfSharp.ucar.ma2.DataType.LONG)
				{
					if (dtype.isUnsigned())
					{
						System.Numerics.BigInteger biggy = System.Numerics.BigInteger.Parse(s, System.Globalization.CultureInfo.InvariantCulture);
						dataI.LongNext = (long)(ulong)(biggy & ulong.MaxValue); // > 63 bits will become "negative".

					}
					else
					{
						long val = long.Parse(s);
						dataI.LongNext = val;
					}

				}
				else
				{ // this works for other numerics (LOOK not unsigned)
					double val = double.Parse(s);
					dataI.DoubleNext = val;
				}
			}
			return result;
		}

		/// <summary>
		/// Make an 1D array from an array of strings.
		/// </summary>
		/// <param name="dtype"> data type of the array. Assumed unsigned </param>
		/// <param name="stringValues"> list of strings. </param>
		/// <returns> resulting 1D array. </returns>
		/// <exception cref="NumberFormatException"> if string values not parseable to specified data type </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public static Array makeArray(DataType dtype, String[] stringValues) throws NumberFormatException
		public static NcArray makeArray(DataType dtype, string[] stringValues)
		{
			return makeArray(dtype, stringValues);
		}

		/// <summary>
		/// Add extra outermost dimension with len = 1.
		/// </summary>
		/// <param name="org"> original array </param>
		/// <returns> rank1 array of rank + 1 </returns>
		public static NcArray makeArrayRankPlusOne(NcArray org)
		{
			int[] shape = new int[org.Rank + 1];
			System.Array.Copy(org.Shape, 0, shape, 1, org.Rank);
			shape[0] = 1;
			return factory(org.DataType, shape, org.Storage);
		}

		/// <summary>
		/// Combine list of Arrays by copying the underlying Arrays into a single primitive array
		/// </summary>
		/// <param name="dataType"> the DataType </param>
		/// <param name="shape"> the shape of the combined array </param>
		/// <param name="arrays"> non-empty list of arrays of the same dataType to combine </param>
		/// <returns> a new Array containing data from the arrays </returns>
		/// <exception cref="IllegalArgumentException"> if arrays is empty or if it contains ArrayStructures with different Members </exception>
		public static NcArray factoryCopy(DataType dataType, int[] shape, IList<NcArray> arrays)
		{
			if (arrays.Count == 0)
			{
				throw new ArgumentException("Expected a non-empty list of Arrays to combine");
			}
			if (arrays.Count == 1)
			{
				return factory(dataType, shape, arrays[0].Storage);
			}
			// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
			// ORIGINAL LINE: final long size = Index.computeSize(shape);
			long size = NetcdfSharp.ucar.ma2.NcIndex.computeSize(shape);
			if (size > int.MaxValue)
			{
				throw new System.OutOfMemoryException("Could not combine Arrays");
			}
			if (dataType == NetcdfSharp.ucar.ma2.DataType.STRUCTURE)
			{
				return combineArrayStructures(size, arrays);
			}
			return combinePrimitiveArrays(dataType, shape, arrays);
		}

		private static NcArray combinePrimitiveArrays(DataType dataType, int[] shape, IList<NcArray> arrays)
		{
			// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
			// ORIGINAL LINE: final Array combinedArray = factory(dataType, shape);
			NcArray combinedArray = factory(dataType, shape);

			int start = 0;
			foreach (NcArray array in arrays)
			{
				NcArray.arraycopy(array, 0, combinedArray, start, (int)array.Size);
				start += (int)array.Size;
			}
			return combinedArray;
		}

		private static NcArray combineArrayStructures(long size, IList<NcArray> arrays)
		{
			// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
			// ORIGINAL LINE: final StructureData[] combinedStructureData = new StructureData[(int) size];
			StructureData[] combinedStructureData = new StructureData[(int)size];
			// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
			// ORIGINAL LINE: final StructureMembers members = ((ArrayStructure) arrays.get(0)).getStructureMembers();
			StructureMembers members = ((ArrayStructure)arrays[0]).StructureMembers;

			if (arrays.Any(array => !((ArrayStructure)array).StructureMembers.Equals(members)))
			{
				throw new ArgumentException("Expected ArrayStructures to be combined to have the same members");
			}

			int count = 0;
			foreach (NcArray array in arrays)
			{
				// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
				// ORIGINAL LINE: final ArrayStructure arrayStructure = (ArrayStructure) array;
				ArrayStructure arrayStructure = (ArrayStructure)array;
				foreach (StructureData structureData in arrayStructure)
				{
					combinedStructureData[count++] = structureData;
				}
			}
			return new ArrayStructureW(members, new int[] { count }, combinedStructureData);
		}

		/////////////////////////////////////////////////////
		protected internal readonly DataType dataType;
		protected internal readonly NcIndex indexCalc;
		protected internal readonly int rank;

		// for subclasses only
		protected internal NcArray(DataType dataType, int[] shape)
		{
			this.dataType = dataType;
			this.rank = shape.Length;
			this.indexCalc = NetcdfSharp.ucar.ma2.NcIndex.factory(shape);
		}

		protected internal NcArray(DataType dataType, NcIndex index)
		{
			this.dataType = dataType;
			this.rank = index.Rank;
			this.indexCalc = index;
		}

		/// <summary>
		/// Return the computed datatype for this array
		/// </summary>
		/// <returns> the data type </returns>
		public virtual DataType DataType
		{
			get
			{
				return this.dataType;
			}
		}

		/// <summary>
		/// Get an Index object used for indexed access of this Array.
		/// </summary>
		/// <returns> an Index for this Array </returns>
		/// <seealso cref="Index"/>
		public virtual NcIndex Index
		{
			get
			{
				return (NcIndex)indexCalc.clone();
			}
		}

		/// <summary>
		/// Get an index iterator for traversing the array in canonical order.
		/// </summary>
		/// <returns> an IndexIterator for this Array </returns>
		/// <seealso cref="IndexIterator"/>
		public virtual IndexIterator IndexIterator
		{
			get
			{
				return indexCalc.getIndexIterator(this);
			}
		}

		public virtual IndexIterator getIndexIterator() => IndexIterator;

		/// <summary>
		/// Get the number of dimensions of the array.
		/// </summary>
		/// <returns> number of dimensions of the array </returns>
		public virtual int Rank
		{
			get
			{
				return rank;
			}
		}

		/// <summary>
		/// Get the shape: length of array in each dimension.
		/// </summary>
		/// <returns> array whose length is the rank of this
		///         Array and whose elements represent the length of each of its indices. </returns>
		public virtual int[] Shape
		{
			get
			{
				return indexCalc.Shape;
			}
		}

		/// <summary>
		/// Get the total number of elements in the array.
		/// </summary>
		/// <returns> total number of elements in the array </returns>
		public virtual long Size
		{
			get
			{
				return indexCalc.Size;
			}
		}

		/// <summary>
		/// Get the total number of bytes in the array.
		/// </summary>
		/// <returns> total number of bytes in the array </returns>
		public virtual long SizeBytes
		{
			get
			{
				DataType dtype = DataTypes.getType(this);
				return indexCalc.Size * dtype.getSize();
			}
		}

		/// <summary>
		/// Get an index iterator for traversing a section of the array in canonical order.
		/// This is equivalent to Array.section(ranges).getIterator();
		/// </summary>
		/// <param name="ranges"> list of Ranges that specify the array subset.
		///        Must be same rank as original Array.
		///        A particular Range: 1) may be a subset, or 2) may be null, meaning use entire Range. </param>
		/// <returns> an IndexIterator over the named range. </returns>
		/// <exception cref="InvalidRangeException"> if ranges is invalid </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public IndexIterator getRangeIterator(NetcdfSharp.java.util.List<Range> ranges) throws InvalidRangeException
		public virtual IndexIterator getRangeIterator(IList<NcRange> ranges)
		{
			return section(ranges).IndexIterator;
		}

		/*
		 * Get an index iterator for traversing the array in arbitrary order.
		 * Use this if you dont care what order the elements are returned, eg if you are summing an Array.
		 * To get an iteration in order, use getIndexIterator(), which returns a fast iterator if possible.
		 *
		 * @return an IndexIterator for traversing the array in arbitrary order.
		 * 
		 * @deprecated use getIndexIterator
		 *
		 * public IndexIterator getIndexIteratorFast() {
		 * return indexCalc.getIndexIteratorFast(this);
		 * }
		 */

		/// <summary>
		/// Get the element class type of this Array
		/// </summary>
		/// <returns> the class of the element </returns>
		public abstract Type ElementType { get; }

		/// <summary>
		/// Get underlying primitive array storage.
		/// Exposed for efficiency, use at your own risk.
		/// </summary>
		/// <returns> underlying primitive array storage </returns>
		public abstract object Storage { get; }

		// So it turns out that non-public, non-protected abstract
		// methods cannot be overridden in classes in other packages.
		// If the methods are declared protected, however, they
		// can be overridden; just one of those Java things.
		//
		// So, modify the following methods to be protected:
		// copyFrom1DJavaArray, copyTo1DJavaArray, and createView.
		//
		// This has consequences all over ucar.ma2.

		// used to create Array from java array
		protected internal abstract void copyFrom1DJavaArray(IndexIterator iter, object javaArray);

		protected internal abstract void copyTo1DJavaArray(IndexIterator iter, object javaArray);

		/// <summary>
		/// create new Array with given Index and the same backing store
		/// </summary>
		/// <param name="index"> use this Index </param>
		/// <returns> a view of the Array using the given Index </returns>
		protected internal abstract NcArray createView(NcIndex index);

		/// <summary>
		/// Create a new Array as a subsection of this Array, with rank reduction.
		/// No data is moved, so the new Array references the same backing store as the original.
		/// </summary>
		/// <param name="ranges"> list of Ranges that specify the array subset.
		///        Must be same rank as original Array.
		///        A particular Range: 1) may be a subset, or 2) may be null, meaning use entire Range.
		///        If Range[dim].length == 1, then the rank of the resulting Array is reduced at that dimension. </param>
		/// <returns> the new Array </returns>
		/// <exception cref="InvalidRangeException"> if ranges is invalid </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public Array section(NetcdfSharp.java.util.List<Range> ranges) throws InvalidRangeException
		public virtual NcArray section(IList<NcRange> ranges)
		{
			return createView(indexCalc.section(ranges));
		}

		/// <summary>
		/// Create a new Array as a subsection of this Array, with rank reduction.
		/// No data is moved, so the new Array references the same backing store as the original.
		/// <p/>
		/// </summary>
		/// <param name="origin"> int array specifying the starting index. Must be same rank as original Array. </param>
		/// <param name="shape"> int array specifying the extents in each dimension.
		///        This becomes the shape of the returned Array. Must be same rank as original Array.
		///        If shape[dim] == 1, then the rank of the resulting Array is reduced at that dimension. </param>
		/// <returns> the new Array </returns>
		/// <exception cref="InvalidRangeException"> if ranges is invalid </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public Array section(int[] origin, int[] shape) throws InvalidRangeException
		public virtual NcArray section(int[] origin, int[] shape)
		{
			return section(origin, shape, null);
		}

		/// <summary>
		/// Create a new Array as a subsection of this Array, with rank reduction.
		/// No data is moved, so the new Array references the same backing store as the original.
		/// <p/>
		/// </summary>
		/// <param name="origin"> int array specifying the starting index. Must be same rank as original Array. </param>
		/// <param name="shape"> int array specifying the extents in each dimension.
		///        This becomes the shape of the returned Array. Must be same rank as original Array.
		///        If shape[dim] == 1, then the rank of the resulting Array is reduced at that dimension. </param>
		/// <param name="stride"> int array specifying the strides in each dimension. If null, assume all ones. </param>
		/// <returns> the new Array </returns>
		/// <exception cref="InvalidRangeException"> if ranges is invalid </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public Array section(int[] origin, int[] shape, int[] stride) throws InvalidRangeException
		public virtual NcArray section(int[] origin, int[] shape, int[] stride)
		{
			List<NcRange> ranges = new List<NcRange>(origin.Length);
			if (stride == null)
			{
				stride = new int[origin.Length];
				for (int i = 0; i < stride.Length; i++)
					stride[i] = 1;
			}
			for (int i = 0; i < origin.Length; i++)
				ranges.Add(new NcRange(origin[i], origin[i] + stride[i] * shape[i] - 1, stride[i]));
			return createView(indexCalc.section(ranges));
		}

		/// <summary>
		/// Create a new Array as a subsection of this Array, without rank reduction.
		/// No data is moved, so the new Array references the same backing store as the original.
		/// Vlen is transferred over unchanged.
		/// </summary>
		/// <param name="ranges"> list of Ranges that specify the array subset.
		///        Must be same rank as original Array.
		///        A particular Range: 1) may be a subset, or 2) may be null, meaning use entire Range. </param>
		/// <returns> the new Array </returns>
		public virtual NcArray sectionNoReduce(IList<NcRange> ranges)
		{
			return createView(indexCalc.sectionNoReduce(ranges));
		}

		/// <summary>
		/// Create a new Array as a subsection of this Array, without rank reduction.
		/// No data is moved, so the new Array references the same backing store as the original.
		/// </summary>
		/// <param name="origin"> int array specifying the starting index. Must be same rank as original Array. </param>
		/// <param name="shape"> int array specifying the extents in each dimension.
		///        This becomes the shape of the returned Array. Must be same rank as original Array. </param>
		/// <param name="stride"> int array specifying the strides in each dimension. If null, assume all ones. </param>
		/// <returns> the new Array </returns>
		public virtual NcArray sectionNoReduce(int[] origin, int[] shape, int[] stride)
		{
			List<NcRange> ranges = new List<NcRange>(origin.Length);
			if (stride == null)
			{
				stride = new int[origin.Length];
				for (int i = 0; i < stride.Length; i++)
					stride[i] = 1;
			}
			for (int i = 0; i < origin.Length; i++)
			{
				if (shape[i] < 0) // VLEN
					ranges.Add(NcRange.VLEN);
				else
					ranges.Add(new NcRange(origin[i], origin[i] + stride[i] * shape[i] - 1, stride[i]));
			}
			return createView(indexCalc.sectionNoReduce(ranges));
		}

		/// <summary>
		/// Create a new Array using same backing store as this Array, by
		/// fixing the specified dimension at the specified index value. This reduces rank by 1.
		/// </summary>
		/// <param name="dim"> which dimension to fix </param>
		/// <param name="value"> at what index value </param>
		/// <returns> a new Array </returns>
		public virtual NcArray slice(int dim, int value)
		{
			int[] origin = new int[rank];
			int[] shape = Shape;
			origin[dim] = value;
			shape[dim] = 1;
			try
			{
				return sectionNoReduce(origin, shape, null).reduce(dim); // preserve other dim 1
			}
			catch (InvalidRangeException)
			{
				throw new ArgumentException();
			}
		}

		/// <summary>
		/// Create a copy of this Array, copying the data so that physical order is the same as
		/// logical order
		/// </summary>
		/// <returns> the new Array </returns>
		public virtual NcArray copy()
		{
			NcArray newA = factory(DataType, Shape);
			MAMath.copy(newA, this);
			return newA;
		}

		/// <summary>
		/// This gets the equivalent java array of the wanted type, in correct order.
		/// It avoids copying if possible.
		/// </summary>
		/// <param name="wantType"> returned object will be an array of this type. This must be convertible to it. </param>
		/// <returns> java array of type want </returns>
		public virtual object get1DJavaArray(DataType wantType)
		{
			if (wantType == DataType)
			{
				if (indexCalc.FastIterator)
					return Storage; // already in order
				else
					return copyTo1DJavaArray(); // gotta copy
			}

			// gotta convert to new type
			NcArray newA = factory(wantType, Shape);
			MAMath.copy(newA, this);
			return newA.Storage;
		}

		/// <summary>
		/// @deprecated use get1DJavaArray(DataType wantType)
		/// </summary>
		public virtual object get1DJavaArray(Type wantType)
		{
			DataType want = NetcdfSharp.ucar.ma2.DataTypes.getType(wantType, isUnsigned());
			return get1DJavaArray(want);
		}

		/// <summary>
		/// Type-safe element access: get the value as T, converting as needed.
		/// Only for scalar arrays (rank 0); for higher rank use <see cref="getValue{T}(int[])"/>.
		/// </summary>
		/// <typeparam name="T"> desired value type, e.g. double, int, string </typeparam>
		/// <returns> the single element, converted to T </returns>
		/// <exception cref="InvalidOperationException"> if this array is not scalar </exception>
		public virtual T getValue<T>()
		{
			if (Rank != 0)
			{
				throw new InvalidOperationException("Array is not scalar (rank=" + Rank + "); use getValue<T>(params int[] index) for arrays");
			}
			return ConvertTo<T>(getObject(0));
		}

		/// <summary>
		/// Type-safe element access by linear index: get the value as T, converting as needed.
		/// </summary>
		/// <typeparam name="T"> desired value type, e.g. double, int, string </typeparam>
		/// <param name="elem"> linear index into the array </param>
		/// <returns> the element, converted to T </returns>
		public virtual T getValue<T>(int elem)
		{
			return ConvertTo<T>(getObject(elem));
		}

		/// <summary>
		/// Type-safe element access by multi-dimensional index: get the value as T, converting as needed.
		/// </summary>
		/// <typeparam name="T"> desired value type, e.g. double, int, string </typeparam>
		/// <param name="index"> one index per dimension </param>
		/// <returns> the element, converted to T </returns>
		/// <exception cref="InvalidRangeException"> if index does not match the array shape </exception>
		public virtual T getValue<T>(params int[] index)
		{
			if (index == null || index.Length != Rank)
			{
				throw new InvalidRangeException("index length (" + (index == null ? 0 : index.Length) + ") must equal rank (" + Rank + ")");
			}
			return ConvertTo<T>(getObject(indexCalc.set(index)));
		}

		/// <summary>
		/// Copy this array to a 1D typed array, in canonical order, converting each element to T.
		/// </summary>
		/// <typeparam name="T"> desired element type, e.g. double, int, string </typeparam>
		/// <returns> a new T[] of length <see cref="Size"/> </returns>
		public virtual T[] to1D<T>()
		{
			T[] result = new T[(int)Size];
			IndexIterator iter = IndexIterator;
			int i = 0;
			while (iter.hasNext())
			{
				result[i++] = ConvertTo<T>(iter.ObjectNext);
			}
			return result;
		}

		private static T ConvertTo<T>(object value)
		{
			if (value == null)
			{
				return default(T);
			}
			return (T)Convert.ChangeType(value, typeof(T), System.Globalization.CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// This gets the data as a ByteBuffer, in correct order.
		/// It avoids copying if possible.
		/// Only for numeric types (byte, short, int, long, double, float)
		/// </summary>
		/// <returns> equivalent data in a ByteBuffer </returns>
		public virtual bool Vlen => false;

		public virtual ByteBuffer DataAsByteBuffer
		{
			get
			{
				return getDataAsByteBuffer(null);
			}
		}

		public virtual ByteBuffer getDataAsByteBuffer()
		{
			throw new System.NotSupportedException();
		}

		public virtual ByteBuffer getDataAsByteBuffer(ByteOrder order)
		{
			throw new System.NotSupportedException();
		}

		public virtual ByteBuffer getDataAsByteBuffer(int capacity, ByteOrder order)
		{
			ByteBuffer bb = ByteBuffer.allocate(capacity);
			if (order != null)
				bb.order(order);
			return bb;
		}

		/// <summary>
		/// Create an Array from a ByteBuffer
		/// </summary>
		/// <param name="dtype"> type of data </param>
		/// <param name="shape"> shape of data; if null, then use int[]{bb.limit()} </param>
		/// <param name="bb"> data is in here </param>
		/// <returns> equivilent Array </returns>
		public static NcArray factory(DataType dtype, int[] shape, ByteBuffer bb)
		{
			int size;
			NcArray result;

			switch (dtype)
			{
				case NetcdfSharp.ucar.ma2.DataType.ENUM1:
				case NetcdfSharp.ucar.ma2.DataType.UBYTE:
				case NetcdfSharp.ucar.ma2.DataType.BYTE:
				case NetcdfSharp.ucar.ma2.DataType.CHAR:
					size = (int)bb.limit();
					if (shape == null)
						shape = new int[] { size };
					result = factory(dtype, shape);
					for (int i = 0; i < size; i++)
						result.setByte(i, (byte)bb.get());
					return result;

				case NetcdfSharp.ucar.ma2.DataType.ENUM2:
				case NetcdfSharp.ucar.ma2.DataType.USHORT:
				case NetcdfSharp.ucar.ma2.DataType.SHORT:
					size = (int)bb.limit();
					if (shape == null)
						shape = new int[] { size };
					result = factory(dtype, shape);
					for (int i = 0; i < size; i++)
						result.setShort(i, bb.getShort(i));
					return result;

				case NetcdfSharp.ucar.ma2.DataType.ENUM4:
				case NetcdfSharp.ucar.ma2.DataType.UINT:
				case NetcdfSharp.ucar.ma2.DataType.INT:
					size = (int)bb.limit();
					if (shape == null)
						shape = new int[] { size };
					result = factory(dtype, shape);
					for (int i = 0; i < size; i++)
						result.setInt(i, bb.getInt(i));
					return result;

				case NetcdfSharp.ucar.ma2.DataType.ULONG:
				case NetcdfSharp.ucar.ma2.DataType.LONG:
					size = (int)bb.limit();
					if (shape == null)
						shape = new int[] { size };
					result = factory(dtype, shape);
					for (int i = 0; i < size; i++)
						result.setLong(i, bb.getLong(i));
					return result;

				case NetcdfSharp.ucar.ma2.DataType.FLOAT:
					size = (int)bb.limit();
					if (shape == null)
						shape = new int[] { size };
					result = factory(dtype, shape);
					for (int i = 0; i < size; i++)
						result.setFloat(i, bb.getFloat(i));
					return result;

				case NetcdfSharp.ucar.ma2.DataType.DOUBLE:
					size = (int)bb.limit();
					if (shape == null)
						shape = new int[] { size };
					result = factory(dtype, shape);
					for (int i = 0; i < size; i++)
						result.setDouble(i, bb.getDouble(i));
					return result;
			}

			throw new System.NotSupportedException("" + dtype);
		}

		/// <summary>
		/// Copy this array to a 1D Java primitive array of type getElementType(), with the physical order
		/// of the result the same as logical order.
		/// </summary>
		/// <returns> a Java 1D array of type getElementType(). </returns>
		public virtual object copyTo1DJavaArray()
		{
			NcArray newA = copy();
			return newA.Storage;
		}

		/// <summary>
		/// Copy this array to an n-Dimensional Java primitive array of type getElementType()
		/// and rank getRank(). Makes a copy of the data.
		/// </summary>
		/// <returns> a Java ND array of type getElementType(). </returns>
		public virtual object copyToNDJavaArray()
		{
			// NetcdfSharp.java.lang.reflect.Array.newInstance(getElementType(), getShape()) builds a JAGGED array
			// (nested per-dimension arrays), which reflectArrayCopyOut() recurses over. A rectangular
			// System.Array (CreateInstance with int[] lengths) has element type = base type, so it would
			// not match: build the equivalent jagged array here.
			object javaArray = makeJaggedArray(ElementType, Shape, 0);

			// copy data
			IndexIterator iter = IndexIterator;
			reflectArrayCopyOut(javaArray, this, iter);

			return javaArray;
		}

		/// <summary>
		/// Build a jagged array (array of arrays) matching Java's
		/// <c>Array.newInstance(componentType, dimensions)</c>: outer arrays are typed <c>componentType[][][]...</c>,
		/// the innermost array has element type <c>componentType</c>.
		/// </summary>
		private static object makeJaggedArray(Type componentType, int[] shape, int dim)
		{
			int n = shape[dim];
			// levelType = componentType[][][]... (shape.Length - dim - 1 levels of 1-D arrays).
			// MakeArrayType() without a rank argument builds a single-dimensional (jagged-able) array type;
			// MakeArrayType(rank) would build a rectangular array, which is NOT what Java's Array.newInstance yields.
			Type levelType = componentType;
			for (int i = 0; i < shape.Length - dim - 1; i++)
			{
				levelType = levelType.MakeArrayType();
			}
			System.Array outer = System.Array.CreateInstance(levelType, n);
			if (dim < shape.Length - 1)
			{
				for (int i = 0; i < n; i++)
				{
					outer.SetValue(makeJaggedArray(componentType, shape, dim + 1), i);
				}
			}
			return outer;
		}

		/// <summary>
		/// Copy this array to a C# rectangular (multi-dimensional) array of the same rank and shape,
		/// e.g. <c>double[,]</c> for rank 2, <c>double[,,]</c> for rank 3, and so on.
		/// The element type is <see cref="ElementType"/> and the shape is <see cref="Shape"/>.
		/// Makes a copy of the data in canonical (row-major) order.
		/// </summary>
		/// <returns> a C# rectangular System.Array (double[,], double[,,], ...) with this array's shape. </returns>
		public virtual System.Array copyToCSharpArray()
		{
			System.Array result = System.Array.CreateInstance(ElementType, Shape);

			if (ElementType.IsPrimitive)
			{
				// Fast path: for primitives the data is a 1D primitive array in canonical order, and a
				// .NET rectangular (multidimensional) primitive array is contiguous in row-major order,
				// which is exactly canonical order. So a single Buffer.BlockCopy memcpy fills the whole
				// rectangular array; get1DJavaArray avoids copying when storage is already in canonical order.
				// IndexConstant returns FastIterator=false, so it takes the copyTo1DJavaArray() branch and
				// expands the constant value to full size.
				System.Array src = (System.Array)get1DJavaArray(DataType);
				System.Buffer.BlockCopy(src, 0, result, 0, System.Buffer.ByteLength(src));
				return result;
			}

			// Reference types (string, StructureData, ...): per-element copy in canonical order.
			int[] idx = new int[Rank];
			long n = 0;
			IndexIterator iter = IndexIterator;
			while (iter.hasNext())
			{
				// row-major decomposition of the flat counter
				long rem = n++;
				for (int d = Rank - 1; d >= 0; d--)
				{
					idx[d] = (int)(rem % Shape[d]);
					rem /= Shape[d];
				}
				result.SetValue(iter.ObjectNext, idx);
			}
			return result;
		}

		/// <summary>
		/// Create an NcArray from a C# rectangular (multi-dimensional) array of any rank,
		/// e.g. <c>double[,]</c>, <c>double[,,]</c>, ... The array's rank and per-dimension lengths
		/// become the NcArray shape; its element type becomes the DataType. Makes a copy of the data.
		/// If <paramref name="csharpArray"/> is a jagged array (array of arrays), the call is delegated
		/// to <see cref="makeFromJavaArray(object, bool)"/>, which handles that case.
		/// </summary>
		/// <param name="csharpArray"> a C# rectangular array of any rank, or a jagged array </param>
		/// <returns> NcArray with the same rank, shape and element type as the input </returns>
		public static NcArray makeFromCSharpArray(System.Array csharpArray)
		{
			if (csharpArray == null)
			{
				throw new ArgumentNullException(nameof(csharpArray));
			}
			if (csharpArray.GetType().GetElementType().IsArray)
			{
				// jagged array: delegate to the existing reflection-based path
				return makeFromJavaArray(csharpArray, false);
			}

			int rank = csharpArray.Rank;
			int[] shape = new int[rank];
			for (int d = 0; d < rank; d++)
			{
				shape[d] = csharpArray.GetLength(d);
			}
			DataType dtype = DataTypes.getType(csharpArray.GetType().GetElementType(), false);
			NcArray aa = factory(dtype, shape);

			int[] idx = new int[rank];
			long n = 0;
			IndexIterator iter = aa.IndexIterator;
			while (iter.hasNext())
			{
				long rem = n++;
				for (int d = rank - 1; d >= 0; d--)
				{
					idx[d] = (int)(rem % shape[d]);
					rem /= shape[d];
				}
				iter.ObjectNext = csharpArray.GetValue(idx);
			}
			return aa;
		}

		/// <summary>
		/// The rank of a C# array. For a rectangular (multi-dimensional) array this is its
		/// <see cref="System.Array.Rank"/> (2 for double[,], 3 for double[,,], ...).
		/// For a jagged array (array of arrays) it is the nesting depth (number of nested
		/// array levels before the primitive element type).
		/// </summary>
		public static int getCSharpArrayRank(System.Array csharpArray)
		{
			if (csharpArray == null)
			{
				throw new ArgumentNullException(nameof(csharpArray));
			}
			Type t = csharpArray.GetType().GetElementType();
			if (!t.IsArray)
			{
				return csharpArray.Rank; // rectangular: System.Array.Rank already counts dimensions
			}
			// jagged: nesting depth of array-of-array types
			int rank = 1;
			while (t.IsArray)
			{
				rank++;
				t = t.GetElementType();
			}
			return rank;
		}

		/// <summary>
		/// The shape of a C# rectangular array (per-dimension lengths).
		/// For a jagged array the outer lengths are returned; the inner arrays are assumed regular.
		/// </summary>
		public static int[] getCSharpArrayShape(System.Array csharpArray)
		{
			if (csharpArray == null)
			{
				throw new ArgumentNullException(nameof(csharpArray));
			}
			if (csharpArray.GetType().GetElementType().IsArray)
			{
				// jagged: outer length, then recurse into first row
				int[] inner = getCSharpArrayShape((System.Array)csharpArray.GetValue(0));
				int[] shape = new int[inner.Length + 1];
				shape[0] = csharpArray.Length;
				inner.CopyTo(shape, 1);
				return shape;
			}
			int[] s = new int[csharpArray.Rank];
			for (int d = 0; d < csharpArray.Rank; d++)
			{
				s[d] = csharpArray.GetLength(d);
			}
			return s;
		}

		/// <summary>
		/// Create a new Array using same backing store as this Array, by
		/// flipping the index so that it runs from shape[index]-1 to 0.
		/// </summary>
		/// <param name="dim"> dimension to flip </param>
		/// <returns> the new Array </returns>
		public virtual NcArray flip(int dim)
		{
			return createView(indexCalc.flip(dim));
		}

		/// <summary>
		/// Create a new Array using same backing store as this Array, by
		/// transposing two of the indices.
		/// </summary>
		/// <param name="dim1"> transpose these two indices </param>
		/// <param name="dim2"> transpose these two indices </param>
		/// <returns> the new Array </returns>
		public virtual NcArray transpose(int dim1, int dim2)
		{
			return createView(indexCalc.transpose(dim1, dim2));
		}

		/// <summary>
		/// Create a new Array using same backing store as this Array, by
		/// permuting the indices.
		/// </summary>
		/// <param name="dims"> the old index dims[k] becomes the new kth index. </param>
		/// <returns> the new Array </returns>
		public virtual NcArray permute(int[] dims)
		{
			return createView(indexCalc.permute(dims));
		}

		/// <summary>
		/// Create a new Array by copying this Array to a new one with given shape
		/// </summary>
		/// <param name="shape"> the new shape </param>
		/// <returns> the new Array </returns>
		public virtual NcArray reshape(int[] shape)
		{
			NcArray result = factory(DataType, shape);
			if (result.Size != Size)
				throw new ArgumentException("reshape arrays must have same total size");
			NcArray.arraycopy(this, 0, result, 0, (int)Size);
			return result;
		}

		/// <summary>
		/// Reshape this array without copying data
		/// </summary>
		/// <param name="shape"> the new shape </param>
		/// <returns> the new Array, using same backing object </returns>
		public virtual NcArray reshapeNoCopy(int[] shape)
		{
			NcArray result = factory(DataType, shape, Storage);
			if (result.Size != Size)
				throw new ArgumentException("reshape arrays must have same total size");
			return result;
		}

		/// <summary>
		/// Create a new Array using same backing store as this Array, by
		/// eliminating any dimensions with length one.
		/// </summary>
		/// <returns> the new Array, or the same array if no reduction was done </returns>
		public virtual NcArray reduce()
		{
			NcIndex ri = indexCalc.reduce();
			if (ri == indexCalc)
				return this;
			return createView(ri);
		}

		/// <summary>
		/// Create a new Array using same backing store as this Array, by
		/// eliminating the specified dimension.
		/// </summary>
		/// <param name="dim"> dimension to eliminate: must be of length one, else IllegalArgumentException </param>
		/// <returns> the new Array </returns>
		public virtual NcArray reduce(int dim)
		{
			return createView(indexCalc.reduce(dim));
		}

		//////////////////////////////////////////////////////////////

		/// <summary>
		/// Find whether the underlying data should be interpreted as unsigned.
		/// Only affects byte, short, int, and long.
		/// When true, conversions to wider types are handled correctly.
		/// </summary>
		/// <returns> true if the data is an unsigned integer type. </returns>
		public virtual bool isUnsigned()
		{
			return dataType.isUnsigned();
		}

		/// <summary>
		/// If this is a constant array
		/// </summary>
		/// <returns> If this is a constant array </returns>
		public virtual bool Constant
		{
			get
			{
				return indexCalc is IndexConstant;
			}
		}

		public virtual bool isVlen()
		{
			return false;
		}

		///////////////////////////////////////////////////
		/* these are the type-specific element accessors */
		///////////////////////////////////////////////////

		/// <summary>
		/// Get the array element at the current element of ima, as a double.
		/// </summary>
		public abstract double getDouble(NcIndex ima);

		/// <summary>
		/// Set the array element at the current element of ima.
		/// </summary>
		public abstract void setDouble(NcIndex ima, double value);

		/// <summary>
		/// Get the array element at the current element of ima, as a float.
		/// </summary>
		public abstract float getFloat(NcIndex ima);

		/// <summary>
		/// Set the array element at the current element of ima.
		/// </summary>
		public abstract void setFloat(NcIndex ima, float value);

		/// <summary>
		/// Get the array element at the current element of ima, as a long.
		/// </summary>
		public abstract long getLong(NcIndex ima);

		/// <summary>
		/// Set the array element at the current element of ima.
		/// </summary>
		public abstract void setLong(NcIndex ima, long value);

		/// <summary>
		/// Get the array element at the current element of ima, as a int.
		/// </summary>
		public abstract int getInt(NcIndex ima);

		/// <summary>
		/// Set the array element at the current element of ima.
		/// </summary>
		public abstract void setInt(NcIndex ima, int value);

		/// <summary>
		/// Get the array element at the current element of ima, as a short.
		/// </summary>
		public abstract short getShort(NcIndex ima);

		/// <summary>
		/// Set the array element at the current element of ima.
		/// </summary>
		public abstract void setShort(NcIndex ima, short value);

		/// <summary>
		/// Get the array element at the current element of ima, as a byte.
		/// </summary>
		public abstract byte getByte(NcIndex ima);

		/// <summary>
		/// Set the array element at the current element of ima.
		/// </summary>
		public abstract void setByte(NcIndex ima, byte value);

		/// <summary>
		/// Get the array element at the current element of ima, as a char.
		/// </summary>
		public abstract char getChar(NcIndex ima);

		/// <summary>
		/// Set the array element at the current element of ima.
		/// </summary>
		public abstract void setChar(NcIndex ima, char value);

		/// <summary>
		/// Get the array element at the current element of ima, as a boolean.
		/// </summary>
		public abstract bool getBoolean(NcIndex ima);

		/// <summary>
		/// Set the array element at the current element of ima.
		/// </summary>
		public abstract void setBoolean(NcIndex ima, bool value);

		/// <summary>
		/// Get the array element at index as an Object.
		/// The returned value is wrapped in an object, eg Double for double
		/// </summary>
		public abstract object getObject(NcIndex ima);

		/// <summary>
		/// Set the array element at index to the specified value.
		/// the value must be passed wrapped in the appropriate Object (eg Double for double)
		/// </summary>
		public abstract void setObject(NcIndex ima, object value);

		//// these are for optimized access with no need to check index values
		//// elem is the index into the backing data
		public abstract double getDouble(int elem);

		public abstract void setDouble(int elem, double val);

		public abstract float getFloat(int elem);

		public abstract void setFloat(int elem, float val);

		public abstract long getLong(int elem);

		public abstract void setLong(int elem, long value);

		public abstract int getInt(int elem);

		public abstract void setInt(int elem, int value);

		public abstract short getShort(int elem);

		public abstract void setShort(int elem, short value);

		public abstract byte getByte(int elem);

		public abstract void setByte(int elem, byte value);

		public abstract char getChar(int elem);

		public abstract void setChar(int elem, char value);

		public abstract bool getBoolean(int elem);

		public abstract void setBoolean(int elem, bool value);

		public abstract object getObject(int elem);

		public abstract void setObject(int elem, object value);

		public override string ToString()
		{
			StringBuilder sbuff = new StringBuilder();
			IndexIterator ii = IndexIterator;
			while (ii.hasNext())
			{
				object value = ii.ObjectNext;

				if (isUnsigned())
				{
					Debug.Assert(value is IConvertible, "A data type being unsigned implies that it is numeric.");

					// "value" is an unsigned number, but it will be treated as signed when we print it below, because Java only
					// has signed types. If it's large enough ( >= 2^(BIT_WIDTH-1) ), its most-significant bit will be interpreted
					// as the sign bit, which will result in an invalid (negative) value being printed. To prevent that, we're
					// going to widen the number before printing it, but only if the unsigned number is being seen as negative.
					value = DataTypes.widenNumberIfNegative(value);
				}

				sbuff.Append(value);
				sbuff.Append(" ");
			}
			return sbuff.ToString();
		}

		/// <summary>
		/// Create a string representation of the shape of this Array.
		/// </summary>
		/// <returns> string representation of the shape </returns>
		public virtual string shapeToString()
		{
			int[] shape = Shape;
			if (shape.Length == 0)
				return "";
			StringBuilder sb = new StringBuilder();
			sb.Append('(');
			for (int i = 0; i < shape.Length; i++)
			{
				int s = shape[i];
				if (i > 0)
					sb.Append(",");
				sb.Append(s);
			}
			sb.Append(')');
			return sb.ToString();
		}

		/////////////////////////////////////////////////////////////////////////////////////////
		// can we replace with replace with foreach ??

		private IndexIterator ii; // local iterator

		/// <summary>
		/// Check if more elements in the local iterator.
		/// Uses the local iterator, which is not thread-safe. Use getIndexIterator if you need thread-safety.
		/// You cannot call any of the array.nextXXX() methods without calling hasNext() first.
		/// If you are not sure of the state of the iterator, you must reset it before use.
		/// </summary>
		/// <returns> true if there are more elements in the iteration </returns>
		public virtual bool hasNext()
		{
			if (null == ii)
				ii = IndexIterator;
			return ii.hasNext();
		}

		/// <summary>
		/// Return the next object in the local iterator.
		/// Uses the local iterator, which is not thread-safe. Use getIndexIterator if you need thread-safety.
		/// </summary>
		public virtual object next()
		{
			return ii.ObjectNext;
		}

		/// <summary>
		/// Return the next double in the local iterator.
		/// Uses the local iterator, which is not thread-safe. Use getIndexIterator if you need thread-safety.
		/// </summary>
		public virtual double nextDouble()
		{
			return ii.DoubleNext;
		}

		/// <summary>
		/// Return the next float in the local iterator.
		/// Uses the local iterator, which is not thread-safe. Use getIndexIterator if you need thread-safety.
		/// </summary>
		public virtual float nextFloat()
		{
			return ii.FloatNext;
		}

		/// <summary>
		/// Return the next byte in the local iterator.
		/// Uses the local iterator, which is not thread-safe. Use getIndexIterator if you need thread-safety.
		/// </summary>
		public virtual byte nextByte()
		{
			return ii.ByteNext;
		}

		/// <summary>
		/// Return the next short in the local iterator.
		/// Uses the local iterator, which is not thread-safe. Use getIndexIterator if you need thread-safety.
		/// </summary>
		public virtual short nextShort()
		{
			return ii.ShortNext;
		}

		/// <summary>
		/// Return the next int in the local iterator.
		/// Uses the local iterator, which is not thread-safe. Use getIndexIterator if you need thread-safety.
		/// </summary>
		public virtual int nextInt()
		{
			return ii.IntNext;
		}

		/// <summary>
		/// Return the next long in the local iterator.
		/// Uses the local iterator, which is not thread-safe. Use getIndexIterator if you need thread-safety.
		/// </summary>
		public virtual long nextLong()
		{
			return ii.LongNext;
		}

		/// <summary>
		/// Return the next char in the local iterator.
		/// Uses the local iterator, which is not thread-safe. Use getIndexIterator if you need thread-safety.
		/// </summary>
		public virtual char nextChar()
		{
			return ii.CharNext;
		}

		/// <summary>
		/// Return the next boolean in the local iterator.
		/// Uses the local iterator, which is not thread-safe. Use getIndexIterator if you need thread-safety.
		/// </summary>
		public virtual bool nextBoolean()
		{
			return ii.BooleanNext;
		}

		/// <summary>
		/// Reset the local iterator.
		/// Uses the local iterator, which is not thread-safe. Use getIndexIterator if you need thread-safety.
		/// </summary>
		public virtual void resetLocalIterator()
		{
			ii = null;
		}
	}

}
