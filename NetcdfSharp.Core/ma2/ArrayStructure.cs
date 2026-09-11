// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using NetcdfSharp.java.util;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.ma2
{
	using Indent = NetcdfSharp.ucar.nc2.util.Indent;

	/// <summary>
	/// Superclass for implementations of Array of StructureData.
	/// <p/>
	/// The general way to access data in an ArrayStructure is to use
	/// 
	/// <pre>
	///  StructureData getStructureData(Index index).
	/// </pre>
	/// <p/>
	/// For 1D arrays (or by calculating your own recnum for nD arrays), you can also use:
	/// 
	/// <pre>
	///  StructureData getStructureData(int recnum).
	/// </pre>
	/// <p/>
	/// Once you have a StructureData object, you can access data in a general way by using:
	/// 
	/// <pre>
	///  Array StructureData.getArray(Member m)
	/// </pre>
	/// <p/>
	/// When dealing with large arrays of Structures, there can be significant overhead in using the generic interfaces.
	/// A number of convenience routines may be able to avoid extra Object creation, and so are recommended for efficiency.
	/// The following may avoid the overhead of creating the StructureData object:
	/// 
	/// <pre>
	///  Array getArray(int recno, StructureMembers.Member m)
	/// </pre>
	/// <p/>
	/// The following can be convenient for accessing all the data in the ArrayStructure for one member, but its efficiency
	/// depends on the implementation:
	/// 
	/// <pre>
	///  Array getMemberArray(StructureMembers.Member m)
	/// </pre>
	/// <p/>
	/// These require that you know the data types of the member data, but they are the most efficent:
	/// 
	/// <pre>
	/// getScalarXXX(int recnum, Member m)
	/// getJavaArrayXXX(int recnum, Member m)
	/// </pre>
	/// 
	/// where XXX is Byte, Char, Double, Float, Int, Long, Short, or String. For members that are themselves Structures,
	/// the equivilent is:
	/// 
	/// <pre>
	/// StructureData getScalarStructure(int recnum, Member m)
	/// ArrayStructure getArrayStructure(int recnum, Member m)
	/// </pre>
	/// 
	/// These will return any compatible type as a double or float, but will have extra overhead when the types dont match:
	/// 
	/// <pre>
	/// convertScalarXXX(int recnum, Member m)
	/// convertJavaArrayXXX(int recnum, Member m)
	/// </pre>
	/// 
	/// where XXX is Double or Float
	/// 
	/// @author caron </summary>
	/// <seealso cref="Array"/>
	/// <seealso cref="StructureData"/>
	public abstract class ArrayStructure : NcArray, IEnumerable<StructureData>
	{

		/*
		 * Implementation notes
		 * ArrayStructure contains the default implementation of storing the data in individual member arrays.
		 * ArrayStructureMA uses all of these.
		 * ArrayStructureW uses some of these.
		 * ArrayStructureBB override all such methods.
		 */

		protected internal StructureMembers members;
		protected internal int nelems;
		protected internal StructureData[] sdata;

		/// <summary>
		/// Create a new Array of type StructureData and the given members and shape.
		/// dimensions.length determines the rank of the new Array.
		/// </summary>
		/// <param name="members"> a description of the structure members </param>
		/// <param name="shape"> the shape of the Array. </param>
		protected internal ArrayStructure(StructureMembers members, int[] shape) : base(DataType.STRUCTURE, shape)
		{
			this.members = members;
			this.nelems = (int)indexCalc.Size;
		}

		// for subclasses to create views
		protected internal ArrayStructure(StructureMembers members, NcIndex ima) : base(DataType.STRUCTURE, ima)
		{
			this.members = members;
			this.nelems = (int)indexCalc.Size;
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

		public override Type ElementType
		{
			get
			{
				return typeof(StructureData);
			}
		}

		/// <summary>
		/// Get the StructureMembers object.
		/// </summary>
		/// <returns> the StructureMembers object. </returns>
		public virtual StructureMembers StructureMembers
		{
			get
			{
				return members;
			}
		}

		public virtual StructureMembers getStructureMembers() => StructureMembers;

		public virtual long getSize() => Size;

		/// <summary>
		/// Get a list of structure members.
		/// </summary>
		/// <returns> the structure members. </returns>
		public virtual IList<StructureMembers.Member> Members
		{
			get
			{
				return members.Members;
			}
		}

		/// <summary>
		/// Get a list structure member names.
		/// </summary>
		/// <returns> the structure members. </returns>
		public virtual IList<string> StructureMemberNames
		{
			get
			{
				return members.MemberNames;
			}
		}

		/// <summary>
		/// Find a member by its name.
		/// </summary>
		/// <param name="memberName"> find member with this name </param>
		/// <returns> StructureMembers.Member matching the name, or null if not found </returns>
		public virtual StructureMembers.Member findMember(string memberName)
		{
			return members.findMember(memberName);
		}

		public override long SizeBytes
		{
			get
			{
				return indexCalc.Size * members.StructureSize;
			}
		}

		/// <summary>
		/// Get the index-th StructureData of this ArrayStructure.
		/// </summary>
		/// <param name="i"> which one to get, specified by an Index. </param>
		/// <returns> object of type StructureData. </returns>
		public override object getObject(NcIndex i)
		{
			return getObject(i.currentElement());
		}

		/// <summary>
		/// Set one of the StructureData of this ArrayStructure.
		/// </summary>
		/// <param name="i"> which one to set, specified by an Index. </param>
		/// <param name="value"> must be type StructureData. </param>
		public override void setObject(NcIndex i, object value)
		{
			setObject(i.currentElement(), value);
		}

		/// <summary>
		/// Get the index-th StructureData of this ArrayStructure.
		/// </summary>
		/// <param name="index"> which one to get, specified by an integer. </param>
		/// <returns> object of type StructureData. </returns>
		public override object getObject(int index)
		{
			return getStructureData(index);
		}

		/// <summary>
		/// Set the index-th StructureData of this ArrayStructure.
		/// </summary>
		/// <param name="index"> which one to set. </param>
		/// <param name="value"> must be type StructureData. </param>
		public override void setObject(int index, object value)
		{
			if (sdata == null)
			{
				sdata = new StructureData[nelems];
			}
			sdata[index] = (StructureData)value;
		}

		/// <summary>
		/// Get the index-th StructureData of this ArrayStructure.
		/// </summary>
		/// <param name="i"> which one to get, specified by an Index. </param>
		/// <returns> object of type StructureData. </returns>
		public virtual StructureData getStructureData(NcIndex i)
		{
			return getStructureData(i.currentElement());
		}

		/// <summary>
		/// Get the index-th StructureData of this ArrayStructure.
		/// </summary>
		/// <param name="index"> which one to get, specified by an integer. </param>
		/// <returns> object of type StructureData. </returns>
		public virtual StructureData getStructureData(int index)
		{
			if (sdata == null)
			{
				sdata = new StructureData[nelems];
			}
			if (index >= sdata.Length)
			{
				throw new ArgumentException(index + " > " + sdata.Length);
			}
			if (sdata[index] == null)
			{
				sdata[index] = makeStructureData(this, index);
			}
			return sdata[index];
		}

		public override object Storage
		{
			get
			{
				// this fills the sdata array
				for (int i = 0; i < nelems; i++)
				{
					getStructureData(i);
				}
				return sdata;
			}
		}

		protected internal abstract StructureData makeStructureData(ArrayStructure @as, int recno);

		/// <summary>
		/// Get the size of each StructureData object in bytes.
		/// </summary>
		/// <returns> the size of each StructureData object in bytes. </returns>
		public virtual int StructureSize
		{
			get
			{
				return members.StructureSize;
			}
		}

		public virtual IEnumerator<StructureData> GetEnumerator()
		{ // throws NetcdfSharp.java.io.IOException {
			return new ArrayStructureIterator(this);
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public virtual StructureDataIterator StructureDataIterator
		{
			get
			{ // throws NetcdfSharp.java.io.IOException {
				return new ArrayStructureIterator(this);
			}
		}

		public class ArrayStructureIterator : StructureDataIterator, IEnumerator<StructureData>
		{
			internal virtual void InitializeInstanceFields()
			{
				size = (int)outerInstance.Size;
			}

			private readonly ArrayStructure outerInstance;

			public ArrayStructureIterator(ArrayStructure outerInstance)
			{
				this.outerInstance = outerInstance;

				InitializeInstanceFields();
			}

			internal int count;
			internal int size;

			public virtual bool hasNext()
			{
				return count < size;
			}

			public virtual StructureData next()
			{
				return outerInstance.getStructureData(count++);
			}

			public virtual StructureDataIterator reset()
			{
				count = 0;
				return this;
			}

			public virtual int CurrentRecno
			{
				get
				{
					return count - 1;
				}
			}

			// debugging
			public virtual ArrayStructure ArrayStructure
			{
				get
				{
					return outerInstance;
				}
			}

			public virtual StructureData Current => next();
			object System.Collections.IEnumerator.Current => Current;
			public virtual bool MoveNext() => hasNext();
			public virtual void Reset() => reset();
			public virtual void Dispose() { }
		}

		///////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Get member data of any type for a specific record as an Array.
		/// This may avoid the overhead of creating the StructureData object, but is equivilent to
		/// getStructure(recno).getArray( Member m).
		/// </summary>
		/// <param name="recno"> get data from the recnum-th StructureData of the ArrayStructure. Must be less than getSize(); </param>
		/// <param name="m"> get data from this StructureMembers.Member. </param>
		/// <returns> Array values. </returns>
		public virtual NcArray getArray(int recno, StructureMembers.Member m)
		{

			DataType dataType = m.DataType;
			switch (dataType)
			{

				case NetcdfSharp.ucar.ma2.DataType.DOUBLE:
					double[] da = getJavaArrayDouble(recno, m);
					return NcArray.factory(dataType, m.Shape, da);

				case NetcdfSharp.ucar.ma2.DataType.FLOAT:
					float[] fa = getJavaArrayFloat(recno, m);
					return NcArray.factory(dataType, m.Shape, fa);

				case NetcdfSharp.ucar.ma2.DataType.BYTE:
				case NetcdfSharp.ucar.ma2.DataType.UBYTE:
				case NetcdfSharp.ucar.ma2.DataType.ENUM1:
					byte[] ba = getJavaArrayByte(recno, m);
					return NcArray.factory(dataType, m.Shape, ba);

				case NetcdfSharp.ucar.ma2.DataType.SHORT:
				case NetcdfSharp.ucar.ma2.DataType.USHORT:
				case NetcdfSharp.ucar.ma2.DataType.ENUM2:
					short[] sa = getJavaArrayShort(recno, m);
					return NcArray.factory(dataType, m.Shape, sa);

				case NetcdfSharp.ucar.ma2.DataType.INT:
				case NetcdfSharp.ucar.ma2.DataType.UINT:
				case NetcdfSharp.ucar.ma2.DataType.ENUM4:
					int[] ia = getJavaArrayInt(recno, m);
					return NcArray.factory(dataType, m.Shape, ia);

				case NetcdfSharp.ucar.ma2.DataType.ULONG:
				case NetcdfSharp.ucar.ma2.DataType.LONG:
					long[] la = getJavaArrayLong(recno, m);
					return NcArray.factory(dataType, m.Shape, la);

				case NetcdfSharp.ucar.ma2.DataType.CHAR:
					char[] ca = getJavaArrayChar(recno, m);
					return NcArray.factory(dataType, m.Shape, ca);

				case NetcdfSharp.ucar.ma2.DataType.STRING:
					string[] str = getJavaArrayString(recno, m);
					return NcArray.factory(dataType, m.Shape, str);

				case NetcdfSharp.ucar.ma2.DataType.STRUCTURE:
					return getArrayStructure(recno, m);

				case NetcdfSharp.ucar.ma2.DataType.SEQUENCE:
					return getArraySequence(recno, m);

				case NetcdfSharp.ucar.ma2.DataType.OPAQUE:
					return getArrayObject(recno, m);
			}

			throw new Exception("Dont have implemenation for " + dataType);


		}

		/// <summary>
		/// Set data for one member, over all structures.
		/// This is used by VariableDS to do scale/offset.
		/// </summary>
		/// <param name="m"> set data for this StructureMembers.Member. </param>
		/// <param name="memberArray"> Array values. </param>
		public virtual void setMemberArray(StructureMembers.Member m, NcArray memberArray)
		{
			m.DataArray = memberArray;
			if (memberArray is ArrayStructure)
			{ // LOOK
				ArrayStructure @as = (ArrayStructure)memberArray;
				m.StructureMembers = @as.StructureMembers;
			}
		}

		/// <summary>
		/// Extract data for one member, over all structures.
		/// </summary>
		/// <param name="m"> get data from this StructureMembers.Member. </param>
		/// <returns> Array values. </returns>
		/// <exception cref="NetcdfSharp.java.io.IOException"> on read error (only happens for Sequences, otherwise data is already read) </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public Array extractMemberArray(StructureMembers.Member m) throws NetcdfSharp.java.io.IOException
		public virtual NcArray extractMemberArray(StructureMembers.Member m)
		{
			if (m.DataArray != null)
			{
				return m.DataArray;
			}
			DataType dataType = m.DataType;

			// combine the shapes
			int[] mshape = m.Shape;
			int rrank = rank + mshape.Length;
			int[] rshape = new int[rrank];
			System.Array.Copy(Shape, 0, rshape, 0, rank);
			System.Array.Copy(mshape, 0, rshape, rank, mshape.Length);

			// create an empty array to hold the result
			NcArray result;
			if (dataType == DataType.STRUCTURE)
			{
				StructureMembers membersw = m.StructureMembers.toBuilder(false).build();
				result = new ArrayStructureW(membersw, rshape);

			}
			else if (dataType == DataType.OPAQUE)
			{
				result = NcArray.factory(DataType.OPAQUE, rshape);

			}
			else
			{
				result = NcArray.factory(dataType, rshape);
			}

			IndexIterator resultIter = result.IndexIterator;
			if (dataType == DataType.DOUBLE)
			{
				for (int recno = 0; recno < Size; recno++)
				{
					copyDoubles(recno, m, resultIter);
				}

			}
			else if (dataType == DataType.FLOAT)
			{
				for (int recno = 0; recno < Size; recno++)
				{
					copyFloats(recno, m, resultIter);
				}

			}
			else if (dataType.getPrimitiveClassType() == typeof(byte))
			{
				for (int recno = 0; recno < Size; recno++)
				{
					copyBytes(recno, m, resultIter);
				}

			}
			else if (dataType.getPrimitiveClassType() == typeof(short))
			{
				for (int recno = 0; recno < Size; recno++)
				{
					copyShorts(recno, m, resultIter);
				}

			}
			else if (dataType.getPrimitiveClassType() == typeof(int))
			{
				for (int recno = 0; recno < Size; recno++)
				{
					copyInts(recno, m, resultIter);
				}

			}
			else if (dataType.getPrimitiveClassType() == typeof(long))
			{
				for (int recno = 0; recno < Size; recno++)
				{
					copyLongs(recno, m, resultIter);
				}

			}
			else if (dataType == DataType.CHAR)
			{
				for (int recno = 0; recno < Size; recno++)
				{
					copyChars(recno, m, resultIter);
				}

			}
			else if ((dataType == DataType.STRING) || (dataType == DataType.OPAQUE))
			{
				for (int recno = 0; recno < Size; recno++)
				{
					copyObjects(recno, m, resultIter);
				}

			}
			else if (dataType == DataType.STRUCTURE)
			{
				for (int recno = 0; recno < Size; recno++)
				{
					copyStructures(recno, m, resultIter);
				}

			}
			else if (dataType == DataType.SEQUENCE)
			{
				for (int recno = 0; recno < Size; recno++)
				{
					copySequences(recno, m, resultIter);
				}

			}

			return result;
		}

		protected internal virtual void copyChars(int recnum, StructureMembers.Member m, IndexIterator result)
		{
			IndexIterator dataIter = getArray(recnum, m).IndexIterator;
			while (dataIter.hasNext())
			{
				result.CharNext = dataIter.CharNext;
			}
		}

		protected internal virtual void copyDoubles(int recnum, StructureMembers.Member m, IndexIterator result)
		{
			IndexIterator dataIter = getArray(recnum, m).IndexIterator;
			while (dataIter.hasNext())
			{
				result.DoubleNext = dataIter.DoubleNext;
			}
		}

		protected internal virtual void copyFloats(int recnum, StructureMembers.Member m, IndexIterator result)
		{
			IndexIterator dataIter = getArray(recnum, m).IndexIterator;
			while (dataIter.hasNext())
			{
				result.FloatNext = dataIter.FloatNext;
			}
		}

		protected internal virtual void copyBytes(int recnum, StructureMembers.Member m, IndexIterator result)
		{
			IndexIterator dataIter = getArray(recnum, m).IndexIterator;
			while (dataIter.hasNext())
			{
				result.ByteNext = dataIter.ByteNext;
			}
		}

		protected internal virtual void copyShorts(int recnum, StructureMembers.Member m, IndexIterator result)
		{
			IndexIterator dataIter = getArray(recnum, m).IndexIterator;
			while (dataIter.hasNext())
			{
				result.ShortNext = dataIter.ShortNext;
			}
		}

		protected internal virtual void copyInts(int recnum, StructureMembers.Member m, IndexIterator result)
		{
			IndexIterator dataIter = getArray(recnum, m).IndexIterator;
			while (dataIter.hasNext())
			{
				result.IntNext = dataIter.IntNext;
			}
		}

		protected internal virtual void copyLongs(int recnum, StructureMembers.Member m, IndexIterator result)
		{
			IndexIterator dataIter = getArray(recnum, m).IndexIterator;
			while (dataIter.hasNext())
			{
				result.LongNext = dataIter.LongNext;
			}
		}

		protected internal virtual void copyObjects(int recnum, StructureMembers.Member m, IndexIterator result)
		{
			IndexIterator dataIter = getArray(recnum, m).IndexIterator;
			while (dataIter.hasNext())
			{
				result.ObjectNext = dataIter.ObjectNext;
			}
		}

		// from the recnum-th structure, copy the member data into result.
		// member data is itself a structure, and may be an array of structures.
		protected internal virtual void copyStructures(int recnum, StructureMembers.Member m, IndexIterator result)
		{
			NcArray data = getArray(recnum, m);
			IndexIterator dataIter = data.IndexIterator;
			while (dataIter.hasNext())
			{
				result.ObjectNext = dataIter.ObjectNext;
			}
		}

		protected internal virtual void copySequences(int recnum, StructureMembers.Member m, IndexIterator result)
		{
			// there can only be one sequence, not an array; copy to an ArrayObject
			NcArray data = getArray(recnum, m);
			result.ObjectNext = data;
		}

		/// <summary>
		/// Get member data array of any type as an Object, eg, Float, Double, String, StructureData etc.
		/// </summary>
		/// <param name="recno"> get data from the recnum-th StructureData of the ArrayStructure. Must be less than getSize(); </param>
		/// <param name="m"> get data from this StructureMembers.Member. </param>
		/// <returns> value as Float, Double, etc.. </returns>
		public virtual object getScalarObject(int recno, StructureMembers.Member m)
		{
			DataType dataType = m.DataType;

			if (dataType == DataType.DOUBLE)
			{
				return getScalarDouble(recno, m);

			}
			else if (dataType == DataType.FLOAT)
			{
				return getScalarFloat(recno, m);

			}
			else if (dataType.getPrimitiveClassType() == typeof(byte))
			{


				return getScalarByte(recno, m);

			}
			else if (dataType.getPrimitiveClassType() == typeof(short))
			{
				return getScalarShort(recno, m);

			}
			else if (dataType.getPrimitiveClassType() == typeof(int))
			{
				return getScalarInt(recno, m);

			}
			else if (dataType.getPrimitiveClassType() == typeof(long))
			{
				return getScalarLong(recno, m);

			}
			else if (dataType == DataType.CHAR)
			{
				return getScalarString(recno, m);

			}
			else if (dataType == DataType.STRING)
			{
				return getScalarString(recno, m);

			}
			else if (dataType == DataType.STRUCTURE)
			{
				return getScalarStructure(recno, m);

			}
			else if (dataType == DataType.OPAQUE)
			{
				ArrayObject data = (ArrayObject)m.DataArray;
				return data.getObject(recno * m.Size); // LOOK ??
			}

			throw new Exception("Dont have implementation for " + dataType);
		}

		/////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Get scalar value as a float, with conversion as needed. Underlying type must be convertible to float.
		/// </summary>
		/// <param name="recnum"> get data from the recnum-th StructureData of the ArrayStructure. Must be less than getSize(); </param>
		/// <param name="m"> member Variable. </param>
		/// <returns> scalar float value </returns>
		/// <exception cref="ForbiddenConversionException"> if not convertible to float. </exception>
		public virtual float convertScalarFloat(int recnum, StructureMembers.Member m)
		{
			if (m.DataType == DataType.FLOAT)
			{
				return getScalarFloat(recnum, m);
			}
			if (m.DataType == DataType.DOUBLE)
			{
				return (float)getScalarDouble(recnum, m);
			}
			object o = getScalarObject(recnum, m);
			if (o is IConvertible)
			{
				return (o as IConvertible).ToSingle(null);
			}
			throw new ForbiddenConversionException("Type is " + m.DataType + ", not convertible to float");
		}

		/// <summary>
		/// Get scalar value as a double, with conversion as needed. Underlying type must be convertible to double.
		/// </summary>
		/// <param name="recnum"> get data from the recnum-th StructureData of the ArrayStructure. Must be less than getSize(); </param>
		/// <param name="m"> member Variable. </param>
		/// <returns> scalar double value </returns>
		/// <exception cref="ForbiddenConversionException"> if not convertible to double. </exception>
		public virtual double convertScalarDouble(int recnum, StructureMembers.Member m)
		{
			if (m.DataType == DataType.DOUBLE)
			{
				return getScalarDouble(recnum, m);
			}
			if (m.DataType == DataType.FLOAT)
			{
				return (double)getScalarFloat(recnum, m);
			}
			object o = getScalarObject(recnum, m);
			if (o is IConvertible)
			{
				return (o as IConvertible).ToDouble(null);
			}
			throw new ForbiddenConversionException("Type is " + m.DataType + ", not convertible to double");
		}

		/// <summary>
		/// Get scalar value as an int, with conversion as needed. Underlying type must be convertible to int.
		/// </summary>
		/// <param name="recnum"> get data from the recnum-th StructureData of the ArrayStructure. Must be less than getSize(); </param>
		/// <param name="m"> member Variable. </param>
		/// <returns> scalar double value </returns>
		/// <exception cref="ForbiddenConversionException"> if not convertible to double. </exception>
		public virtual int convertScalarInt(int recnum, StructureMembers.Member m)
		{
			if (m.DataType == DataType.INT || m.DataType == DataType.UINT)
			{
				return getScalarInt(recnum, m);
			}
			if (m.DataType == DataType.SHORT)
			{
				return (int)getScalarShort(recnum, m);
			}
			if (m.DataType == DataType.USHORT)
			{
				return DataTypes.unsignedShortToInt(getScalarShort(recnum, m));
			}
			if (m.DataType == DataType.BYTE)
			{
				return (int)getScalarByte(recnum, m);
			}
			if (m.DataType == DataType.UBYTE)
			{
				return (int)DataTypes.unsignedByteToShort(getScalarByte(recnum, m));
			}
			if (m.DataType == DataType.LONG || m.DataType == DataType.ULONG)
			{
				return (int)getScalarLong(recnum, m);
			}
			object o = getScalarObject(recnum, m);
			if (o is IConvertible)
			{
				return (o as IConvertible).ToInt32(null);
			}
			throw new ForbiddenConversionException("Type is " + m.DataType + ", not convertible to int");
		}

		public virtual long convertScalarLong(int recnum, StructureMembers.Member m)
		{
			if (m.DataType == DataType.LONG || m.DataType == DataType.ULONG)
			{
				return getScalarLong(recnum, m);
			}
			if (m.DataType == DataType.INT)
			{
				return (long)getScalarInt(recnum, m);
			}
			if (m.DataType == DataType.UINT)
			{
				return DataTypes.unsignedIntToLong(getScalarInt(recnum, m));
			}
			if (m.DataType == DataType.SHORT)
			{
				return (long)getScalarShort(recnum, m);
			}
			if (m.DataType == DataType.USHORT)
			{
				return (long)DataTypes.unsignedShortToInt(getScalarShort(recnum, m));
			}
			if (m.DataType == DataType.BYTE)
			{
				return (long)getScalarByte(recnum, m);
			}
			if (m.DataType == DataType.UBYTE)
			{
				return (long)DataTypes.unsignedByteToShort(getScalarByte(recnum, m));
			}
			object o = getScalarObject(recnum, m);
			if (o is IConvertible)
			{
				return (o as IConvertible).ToInt64(null);
			}
			throw new ForbiddenConversionException("Type is " + m.DataType + ", not convertible to int");
		}

		/////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Get scalar member data of type double.
		/// </summary>
		/// <param name="recnum"> get data from the recnum-th StructureData of the ArrayStructure. Must be less than getSize(); </param>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type double. </param>
		/// <returns> scalar double value </returns>
		public virtual double getScalarDouble(int recnum, StructureMembers.Member m)
		{
			if (m.DataType != DataType.DOUBLE)
			{
				throw new ArgumentException("Type is " + m.DataType + ", must be double");
			}
			NcArray data = m.DataArray;
			return data.getDouble(recnum * m.Size); // gets first one in the array
		}

		/// <summary>
		/// Get member data of type double as a 1D array. The member data may be any rank.
		/// </summary>
		/// <param name="recnum"> get data from the recnum-th StructureData of the ArrayStructure. Must be less than getSize(); </param>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type double. </param>
		/// <returns> double[] </returns>
		public virtual double[] getJavaArrayDouble(int recnum, StructureMembers.Member m)
		{
			if (m.DataType != DataType.DOUBLE)
			{
				throw new ArgumentException("Type is " + m.DataType + ", must be double");
			}
			int count = m.Size;
			NcArray data = m.DataArray;
			double[] pa = new double[count];
			for (int i = 0; i < count; i++)
			{
				pa[i] = data.getDouble(recnum * count + i);
			}
			return pa;
		}

		/// <summary>
		/// Get scalar member data of type float.
		/// </summary>
		/// <param name="recnum"> get data from the recnum-th StructureData of the ArrayStructure. Must be less than getSize(); </param>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type float. </param>
		/// <returns> scalar double value </returns>
		public virtual float getScalarFloat(int recnum, StructureMembers.Member m)
		{
			if (m.DataType != DataType.FLOAT)
			{
				throw new ArgumentException("Type is " + m.DataType + ", must be float");
			}
			NcArray data = m.DataArray;
			return data.getFloat(recnum * m.Size); // gets first one in the array
		}

		/// <summary>
		/// Get member data of type float as a 1D array.
		/// </summary>
		/// <param name="recnum"> get data from the recnum-th StructureData of the ArrayStructure. Must be less than getSize(); </param>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type float. </param>
		/// <returns> float[] </returns>
		public virtual float[] getJavaArrayFloat(int recnum, StructureMembers.Member m)
		{
			if (m.DataType != DataType.FLOAT)
			{
				throw new ArgumentException("Type is " + m.DataType + ", must be float");
			}
			int count = m.Size;
			NcArray data = m.DataArray;
			float[] pa = new float[count];
			for (int i = 0; i < count; i++)
			{
				pa[i] = data.getFloat(recnum * count + i);
			}
			return pa;
		}

		/// <summary>
		/// Get scalar member data of type byte.
		/// </summary>
		/// <param name="recnum"> get data from the recnum-th StructureData of the ArrayStructure. Must be less than getSize(); </param>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type byte. </param>
		/// <returns> scalar double value </returns>
		public virtual byte getScalarByte(int recnum, StructureMembers.Member m)
		{
			if (!(m.DataType.getPrimitiveClassType() == typeof(byte)))
			{
				throw new ArgumentException("Type is " + m.DataType + ", must be byte");
			}
			NcArray data = m.DataArray;
			return data.getByte(recnum * m.Size); // gets first one in the array
		}

		/// <summary>
		/// Get member data of type byte as a 1D array.
		/// </summary>
		/// <param name="recnum"> get data from the recnum-th StructureData of the ArrayStructure. Must be less than getSize(); </param>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type byte. </param>
		/// <returns> byte[] </returns>
		public virtual byte[] getJavaArrayByte(int recnum, StructureMembers.Member m)
		{
			if (!(m.DataType.getPrimitiveClassType() == typeof(byte)))
			{
				throw new ArgumentException("Type is " + m.DataType + ", must be byte");
			}
			int count = m.Size;
			NcArray data = m.DataArray;
			byte[] pa = new byte[count];
			for (int i = 0; i < count; i++)
			{
				pa[i] = data.getByte(recnum * count + i);
			}
			return pa;
		}

		/// <summary>
		/// Get scalar member data of type short.
		/// </summary>
		/// <param name="recnum"> get data from the recnum-th StructureData of the ArrayStructure. Must be less than getSize(); </param>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type short. </param>
		/// <returns> scalar double value </returns>
		public virtual short getScalarShort(int recnum, StructureMembers.Member m)
		{
			if (!(m.DataType.getPrimitiveClassType() == typeof(short)))
			{
				throw new ArgumentException("Type is " + m.DataType + ", must be short");
			}
			NcArray data = m.DataArray;
			return data.getShort(recnum * m.Size); // gets first one in the array
		}

		/// <summary>
		/// Get member data of type short as a 1D array.
		/// </summary>
		/// <param name="recnum"> get data from the recnum-th StructureData of the ArrayStructure. Must be less than getSize(); </param>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type float. </param>
		/// <returns> short[] </returns>
		public virtual short[] getJavaArrayShort(int recnum, StructureMembers.Member m)
		{
			if (!(m.DataType.getPrimitiveClassType() == typeof(short)))
			{
				throw new ArgumentException("Type is " + m.DataType + ", must be short");
			}
			int count = m.Size;
			NcArray data = m.DataArray;
			short[] pa = new short[count];
			for (int i = 0; i < count; i++)
			{
				pa[i] = data.getShort(recnum * count + i);
			}
			return pa;
		}

		/// <summary>
		/// Get scalar member data of type int.
		/// </summary>
		/// <param name="recnum"> get data from the recnum-th StructureData of the ArrayStructure. Must be less than getSize(); </param>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type int. </param>
		/// <returns> scalar double value </returns>
		public virtual int getScalarInt(int recnum, StructureMembers.Member m)
		{
			if (!(m.DataType.getPrimitiveClassType() == typeof(int)))
			{
				throw new ArgumentException("Type is " + m.DataType + ", must be int");
			}
			NcArray data = m.DataArray;
			return data.getInt(recnum * m.Size); // gets first one in the array
		}

		/// <summary>
		/// Get member data of type int as a 1D array.
		/// </summary>
		/// <param name="recnum"> get data from the recnum-th StructureData of the ArrayStructure. Must be less than getSize(); </param>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type int. </param>
		/// <returns> int[] </returns>
		public virtual int[] getJavaArrayInt(int recnum, StructureMembers.Member m)
		{
			if (!(m.DataType.getPrimitiveClassType() == typeof(int)))
			{
				throw new ArgumentException("Type is " + m.DataType + ", must be int");
			}
			int count = m.Size;
			NcArray data = m.DataArray;
			int[] pa = new int[count];
			for (int i = 0; i < count; i++)
			{
				pa[i] = data.getInt(recnum * count + i);
			}
			return pa;
		}

		/// <summary>
		/// Get scalar member data of type long.
		/// </summary>
		/// <param name="recnum"> get data from the recnum-th StructureData of the ArrayStructure. Must be less than getSize(); </param>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type long. </param>
		/// <returns> scalar double value </returns>
		public virtual long getScalarLong(int recnum, StructureMembers.Member m)
		{
			if (!(m.DataType.getPrimitiveClassType() == typeof(long)))
			{
				throw new ArgumentException("Type is " + m.DataType + ", must be long");
			}
			NcArray data = m.DataArray;
			return data.getLong(recnum * m.Size); // gets first one in the array
		}

		/// <summary>
		/// Get member data of type long as a 1D array.
		/// </summary>
		/// <param name="recnum"> get data from the recnum-th StructureData of the ArrayStructure. Must be less than getSize(); </param>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type long. </param>
		/// <returns> long[] </returns>
		public virtual long[] getJavaArrayLong(int recnum, StructureMembers.Member m)
		{
			if (!(m.DataType.getPrimitiveClassType() == typeof(long)))
			{
				throw new ArgumentException("Type is " + m.DataType + ", must be long");
			}
			int count = m.Size;
			NcArray data = m.DataArray;
			long[] pa = new long[count];
			for (int i = 0; i < count; i++)
			{
				pa[i] = data.getLong(recnum * count + i);
			}
			return pa;
		}

		/// <summary>
		/// Get scalar member data of type char.
		/// </summary>
		/// <param name="recnum"> get data from the recnum-th StructureData of the ArrayStructure. Must be less than getSize(); </param>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type char. </param>
		/// <returns> scalar double value </returns>
		public virtual char getScalarChar(int recnum, StructureMembers.Member m)
		{
			if (m.DataType != DataType.CHAR)
			{
				throw new ArgumentException("Type is " + m.DataType + ", must be char");
			}
			NcArray data = m.DataArray;
			return data.getChar(recnum * m.Size); // gets first one in the array
		}

		/// <summary>
		/// Get member data of type char as a 1D array.
		/// </summary>
		/// <param name="recnum"> get data from the recnum-th StructureData of the ArrayStructure. Must be less than getSize(); </param>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type char. </param>
		/// <returns> char[] </returns>
		public virtual char[] getJavaArrayChar(int recnum, StructureMembers.Member m)
		{
			if (m.DataType != DataType.CHAR)
			{
				throw new ArgumentException("Type is " + m.DataType + ", must be char");
			}
			int count = m.Size;
			NcArray data = m.DataArray;
			char[] pa = new char[count];
			for (int i = 0; i < count; i++)
			{
				pa[i] = data.getChar(recnum * count + i);
			}
			return pa;
		}

		/// <summary>
		/// Get member data of type String or char.
		/// </summary>
		/// <param name="recnum"> get data from the recnum-th StructureData of the ArrayStructure. Must be less than getSize(); </param>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type String or char. </param>
		/// <returns> scalar String value </returns>
		public virtual string getScalarString(int recnum, StructureMembers.Member m)
		{
			if (m.DataType == DataType.CHAR)
			{
				ArrayChar data = (ArrayChar)m.DataArray;
				return data.getString(recnum);
			}

			if (m.DataType == DataType.STRING)
			{
				NcArray data = m.DataArray;
				return (string)data.getObject(recnum);
			}

			throw new ArgumentException("Type is " + m.DataType + ", must be String or char");
		}

		/// <summary>
		/// Get member data of type String as a 1D array.
		/// </summary>
		/// <param name="recnum"> get data from the recnum-th StructureData of the ArrayStructure. Must be less than getSize(); </param>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type String. </param>
		/// <returns> String[] </returns>
		public virtual string[] getJavaArrayString(int recnum, StructureMembers.Member m)
		{
			if (m.DataType == DataType.STRING)
			{
				int n = m.Size;
				string[] result = new string[n];
				NcArray data = m.DataArray;
				for (int i = 0; i < n; i++)
				{
					result[i] = (string)data.getObject(recnum * n + i);
				}
				return result;
			}

			if (m.DataType == DataType.CHAR)
			{
				int strlen = indexCalc.getShape(rank - 1);
				int n = m.Size / strlen;
				string[] result = new string[n];
				ArrayChar data = (ArrayChar)m.DataArray;
				for (int i = 0; i < n; i++)
				{
					result[i] = data.getString((recnum * n + i) * strlen);
				}
				return result;
			}

			throw new ArgumentException("Type is " + m.DataType + ", must be String or char");
		}

		/// <summary>
		/// Get member data of type Structure.
		/// </summary>
		/// <param name="recnum"> get data from the recnum-th StructureData of the ArrayStructure. Must be less than getSize(); </param>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type Structure. </param>
		/// <returns> scalar StructureData </returns>
		public virtual StructureData getScalarStructure(int recnum, StructureMembers.Member m)
		{
			if (m.DataType != DataType.STRUCTURE)
			{
				throw new ArgumentException("Type is " + m.DataType + ", must be Structure");
			}

			ArrayStructure data = (ArrayStructure)m.DataArray;
			return data.getStructureData(recnum * m.Size); // gets first in the array
		}

		/// <summary>
		/// Get member data of type array of Structure.
		/// </summary>
		/// <param name="recnum"> get data from the recnum-th StructureData of the ArrayStructure. Must be less than getSize(); </param>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type Structure. </param>
		/// <returns> nested ArrayStructure. </returns>
		public virtual ArrayStructure getArrayStructure(int recnum, StructureMembers.Member m)
		{
			if ((m.DataType != DataType.STRUCTURE) && (m.DataType != DataType.SEQUENCE))
			{
				throw new ArgumentException("Type is " + m.DataType + ", must be Structure or Sequence");
			}

			if (m.DataType == DataType.SEQUENCE)
			{
				return getArraySequence(recnum, m);
			}

			ArrayStructure array = (ArrayStructure)m.DataArray;
			int count = m.Size;
			StructureData[] this_sdata = new StructureData[count];
			for (int i = 0; i < count; i++)
			{
				this_sdata[i] = array.getStructureData(recnum * count + i);
			}

			// make a copy of the members, but remove the data arrays, since the structureData must be used instead
			StructureMembers membersw = array.StructureMembers.toBuilder(false).build();
			return new ArrayStructureW(membersw, m.Shape, this_sdata);
		}

		/// <summary>
		/// Get member data of type ArraySequence
		/// </summary>
		/// <param name="recnum"> get data from the recnum-th StructureData of the ArrayStructure. Must be less than getSize(); </param>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type Structure. </param>
		/// <returns> nested ArrayStructure. </returns>
		public virtual ArraySequence getArraySequence(int recnum, StructureMembers.Member m)
		{
			if (m.DataType != DataType.SEQUENCE)
			{
				throw new ArgumentException("Type is " + m.DataType + ", must be Sequence");
			}

			// should store sequences as ArrayObject of ArraySequence objects
			ArrayObject array = (ArrayObject)m.DataArray;
			return (ArraySequence)array.getObject(recnum);
		}

		/// <summary>
		/// Get member data of type ArrayObject
		/// </summary>
		/// <param name="recnum"> get data from the recnum-th StructureData of the ArrayStructure. Must be less than getSize(); </param>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type Structure. </param>
		/// <returns> ArrayObject. </returns>
		public virtual ArrayObject getArrayObject(int recnum, StructureMembers.Member m)
		{
			if (m.DataType != DataType.OPAQUE)
			{
				throw new ArgumentException("Type is " + m.DataType + ", must be Sequence");
			}

			ArrayObject array = (ArrayObject)m.DataArray;
			return (ArrayObject)array.getObject(recnum); // LOOK ??
		}

		////////////////////////////////////////////////////////////////////////////////////////////////
		public virtual void showInternal(Formatter f, Indent indent)
		{
			f.format("%sArrayStructure %s size=%d class=%s hash=0x%x%n", indent, members.Name, Size, GetType().FullName, GetHashCode());
		}

		public virtual void showInternalMembers(Formatter f, Indent indent)
		{
			f.format("%sArrayStructure %s class=%s hash=0x%x%n", indent, members.Name, GetType().FullName, GetHashCode());
			indent.incr();
			foreach (StructureMembers.Member m in Members)
				m.showInternal(f, indent);
			indent.incr();
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////

		protected internal override NcArray createView(NcIndex index)
		{
			// Section viewSection = index.getSection(); / LOOK if we could do this, we could make this work

			throw new System.NotSupportedException();
		}

		public override NcArray sectionNoReduce(IList<NcRange> ranges)
		{
			Section viewSection = new Section(ranges);
			ArrayStructureW result = new ArrayStructureW(this.members, viewSection.Shape);
			int count = 0;
			Section.Iterator iter = viewSection.getIterator(Shape);
			while (iter.hasNext())
			{
				int recno = iter.next(null);
				StructureData sd = getStructureData(recno);
				result.setStructureData(sd, count++);
			}
			return result;
		}

		/// <summary>
		/// DO NOT USE, throws UnsupportedOperationException
		/// </summary>
		public override NcArray copy()
		{
			throw new System.NotSupportedException();
		}

		/// <summary>
		/// DO NOT USE, throw ForbiddenConversionException
		/// </summary>
		public override double getDouble(NcIndex i)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// DO NOT USE, throw ForbiddenConversionException
		/// </summary>
		public override void setDouble(NcIndex i, double value)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// DO NOT USE, throw ForbiddenConversionException
		/// </summary>
		public override float getFloat(NcIndex i)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// DO NOT USE, throw ForbiddenConversionException
		/// </summary>
		public override void setFloat(NcIndex i, float value)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// DO NOT USE, throw ForbiddenConversionException
		/// </summary>
		public override long getLong(NcIndex i)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// DO NOT USE, throw ForbiddenConversionException
		/// </summary>
		public override void setLong(NcIndex i, long value)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// DO NOT USE, throw ForbiddenConversionException
		/// </summary>
		public override int getInt(NcIndex i)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// DO NOT USE, throw ForbiddenConversionException
		/// </summary>
		public override void setInt(NcIndex i, int value)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// DO NOT USE, throw ForbiddenConversionException
		/// </summary>
		public override short getShort(NcIndex i)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// DO NOT USE, throw ForbiddenConversionException
		/// </summary>
		public override void setShort(NcIndex i, short value)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// DO NOT USE, throw ForbiddenConversionException
		/// </summary>
		public override byte getByte(NcIndex i)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// DO NOT USE, throw ForbiddenConversionException
		/// </summary>
		public override void setByte(NcIndex i, byte value)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// DO NOT USE, throw ForbiddenConversionException
		/// </summary>
		public override bool getBoolean(NcIndex i)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// DO NOT USE, throw ForbiddenConversionException
		/// </summary>
		public override void setBoolean(NcIndex i, bool value)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// DO NOT USE, throw ForbiddenConversionException
		/// </summary>
		public override char getChar(NcIndex i)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// DO NOT USE, throw ForbiddenConversionException
		/// </summary>
		public override void setChar(NcIndex i, char value)
		{
			throw new ForbiddenConversionException();
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
	}

}
