// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using NetcdfSharp.java.util;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.ma2
{
	using Sequence = NetcdfSharp.ucar.nc2.Sequence;
	using Indent = NetcdfSharp.ucar.nc2.util.Indent;

	/// <summary>
	/// ArraySequence is the way to contain the data for a Sequence, using a StructureDataIterator.
	/// A Sequence is a one-dimensional Structure with indeterminate length.
	/// The only data access is through getStructureIterator().
	/// So an ArraySequence is really a wrapper around a StructureDataIterator, adapting it to an Array.
	/// 
	/// @author caron
	/// @since Feb 27, 2008
	/// </summary>
	public class ArraySequence : ArrayStructure
	{
	  protected internal StructureDataIterator iter; // never use private

	  protected internal ArraySequence(StructureMembers sm, int[] shape) : base(sm, shape)
	  {
	  }

	  /// <summary>
	  /// Constructor
	  /// </summary>
	  /// <param name="members"> the members </param>
	  /// <param name="iter"> the iterator </param>
	  /// <param name="nelems"> iterator count, may be missing (<0) </param>
	  public ArraySequence(StructureMembers members, StructureDataIterator iter, int nelems) : base(members, new int[] {0})
	  {
		this.iter = iter;
		this.nelems = nelems;
	  }

	  public override Type ElementType
	  {
		  get
		  {
			return typeof(StructureDataIterator);
		  }
	  }

	  public override StructureDataIterator StructureDataIterator
	  {
		  get
		  {
			iter = iter.reset();
			return iter;
		  }
	  }

	  public virtual int StructureDataCount
	  {
		  get
		  {
			return nelems;
		  }
	  }

	  public override long SizeBytes
	  {
		  get
		  {
			return nelems * members.StructureSize; // LOOK we may not know the count ???
		  }
	  }

	  protected internal override StructureData makeStructureData(ArrayStructure @as, int index)
	  {
		throw new System.NotSupportedException("Cannot subset a Sequence");
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public Array extractMemberArray(StructureMembers.Member proxym) throws NetcdfSharp.java.io.IOException
	  public override NcArray extractMemberArray(StructureMembers.Member proxym)
	  {
		if (proxym.DataArray != null)
		{
		  return proxym.DataArray;
		}

		DataType dataType = proxym.DataType;
		bool isScalar = (proxym.Size == 1) || (dataType == DataType.SEQUENCE);

		// combine the shapes
		int[] mshape = proxym.Shape;
		int rrank = 1 + mshape.Length;
		int[] rshape = new int[rrank];
		rshape[0] = nelems;
		System.Array.Copy(mshape, 0, rshape, 1, mshape.Length);

		if (nelems < 0)
		{
		  return extractMemberArrayFromIteration(proxym, rshape);
		}

		// create an empty array to hold the result
		NcArray result;
		if (dataType == DataType.STRUCTURE)
		{
		  StructureMembers membersw = proxym.StructureMembers.toBuilder(false).build(); // no data arrays get
																							 // propagated
		  result = new ArrayStructureW(membersw, rshape);
		}
		else
		{
		  result = NcArray.factory(dataType, rshape);
		}

		using (StructureDataIterator sdataIter = StructureDataIterator)
		{
		  IndexIterator resultIter = result.IndexIterator;
		  while (sdataIter.hasNext())
		  {
			StructureData sdata = sdataIter.next();
			StructureMembers.Member realm = sdata.StructureMembers.findMember(proxym.Name);

			if (isScalar)
			{
			  if (dataType == DataType.DOUBLE)
			  {
				resultIter.DoubleNext = sdata.getScalarDouble(realm);
			  }

			  else if (dataType == DataType.FLOAT)
			  {
				resultIter.FloatNext = sdata.getScalarFloat(realm);
			  }

			  else if (dataType.getPrimitiveClassType() == typeof(byte))
			  {
				resultIter.ByteNext = sdata.getScalarByte(realm);
			  }

			  else if (dataType.getPrimitiveClassType() == typeof(short))
			  {
				resultIter.ShortNext = sdata.getScalarShort(realm);
			  }

			  else if (dataType.getPrimitiveClassType() == typeof(int))
			  {
				resultIter.IntNext = sdata.getScalarInt(realm);
			  }

			  else if (dataType.getPrimitiveClassType() == typeof(long))
			  {
				resultIter.LongNext = sdata.getScalarLong(realm);
			  }

			  else if (dataType == DataType.CHAR)
			  {
				resultIter.CharNext = sdata.getScalarChar(realm);
			  }

			  else if (dataType == DataType.STRING)
			  {
				resultIter.ObjectNext = sdata.getScalarString(realm);
			  }

			  else if (dataType == DataType.STRUCTURE)
			  {
				resultIter.ObjectNext = sdata.getScalarStructure(realm);
			  }

			  else if (dataType == DataType.SEQUENCE)
			  {
				resultIter.ObjectNext = sdata.getArraySequence(realm);
			  }

			}
			else
			{
			  if (dataType == DataType.DOUBLE)
			  {
				double[] data = sdata.getJavaArrayDouble(realm);
				foreach (double aData in data)
				{
				  resultIter.DoubleNext = aData;
				}

			  }
			  else if (dataType == DataType.FLOAT)
			  {
				float[] data = sdata.getJavaArrayFloat(realm);
				foreach (float aData in data)
				{
				  resultIter.FloatNext = aData;
				}

			  }
			  else if (dataType.getPrimitiveClassType() == typeof(byte))
			  {
				byte[] data = sdata.getJavaArrayByte(realm);
				foreach (byte aData in data)
				{
				  resultIter.ByteNext = aData;
				}

			  }
			  else if (dataType.getPrimitiveClassType() == typeof(short))
			  {
				short[] data = sdata.getJavaArrayShort(realm);
				foreach (short aData in data)
				{
				  resultIter.ShortNext = aData;
				}

			  }
			  else if (dataType.getPrimitiveClassType() == typeof(int))
			  {
				int[] data = sdata.getJavaArrayInt(realm);
				foreach (int aData in data)
				{
				  resultIter.IntNext = aData;
				}

			  }
			  else if (dataType.getPrimitiveClassType() == typeof(long))
			  {
				long[] data = sdata.getJavaArrayLong(realm);
				foreach (long aData in data)
				{
				  resultIter.LongNext = aData;
				}

			  }
			  else if (dataType == DataType.CHAR)
			  {
				char[] data = sdata.getJavaArrayChar(realm);
				foreach (char aData in data)
				{
				  resultIter.CharNext = aData;
				}

			  }
			  else if (dataType == DataType.STRING)
			  {
				string[] data = sdata.getJavaArrayString(realm);
				foreach (string aData in data)
				{
				  resultIter.ObjectNext = aData;
				}

			  }
			  else if (dataType == DataType.STRUCTURE)
			  {
				ArrayStructure @as = sdata.getArrayStructure(realm);
				StructureDataIterator innerIter = @as.StructureDataIterator;
				while (innerIter.hasNext())
				{
				  resultIter.ObjectNext = innerIter.next();
				}
			  }

			  // LOOK SEQUENCE, OPAQUE ??
			}
		  }
		}

		return result;
	  }

	  // when we dont know how many in the iteration

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private Array extractMemberArrayFromIteration(StructureMembers.Member proxym, int[] rshape) throws NetcdfSharp.java.io.IOException
	  private NcArray extractMemberArrayFromIteration(StructureMembers.Member proxym, int[] rshape)
	  {
		DataType dataType = proxym.DataType;
		object dataArray = null;
		int count = 0;
		int initial = 1000;

		using (StructureDataIterator sdataIter = StructureDataIterator)
		{
		  switch (dataType)
		  {
			case NetcdfSharp.ucar.ma2.DataType.DOUBLE:
			{
			  List<double> result = new List<double>(initial);
			  while (sdataIter.hasNext())
			  {
				StructureData sdata = sdataIter.next();
				StructureMembers.Member realm = sdata.StructureMembers.findMember(proxym.Name);
				double[] data = sdata.getJavaArrayDouble(realm);
				foreach (double aData in data)
				{
				  result.Add(aData);
				}
				count++;
			  }
			  double[] da = new double[result.Count];
			  int i = 0;
			  foreach (double? d in result)
			  {
				da[i++] = d.Value;
			  }
			  dataArray = da;
			  break;
			}

			case NetcdfSharp.ucar.ma2.DataType.FLOAT:
			{
			  List<float> result = new List<float>(initial);
			  while (sdataIter.hasNext())
			  {
				StructureData sdata = sdataIter.next();
				StructureMembers.Member realm = sdata.StructureMembers.findMember(proxym.Name);
				float[] data = sdata.getJavaArrayFloat(realm);
				foreach (float aData in data)
				{
				  result.Add(aData);
				}
				count++;
			  }
			  float[] da = new float[result.Count];
			  int i = 0;
			  foreach (float? d in result)
			  {
				da[i++] = d.Value;
			  }
			  dataArray = da;
			  break;
			}

			case NetcdfSharp.ucar.ma2.DataType.UBYTE:
			case NetcdfSharp.ucar.ma2.DataType.BYTE:
			case NetcdfSharp.ucar.ma2.DataType.ENUM1:
			{
			  List<byte> result = new List<byte>(initial);
			  while (sdataIter.hasNext())
			  {
				StructureData sdata = sdataIter.next();
				StructureMembers.Member realm = sdata.StructureMembers.findMember(proxym.Name);
				byte[] data = sdata.getJavaArrayByte(realm);
				foreach (byte aData in data)
				{
				  result.Add(aData);
				}
				count++;
			  }
			  byte[] da = new byte[result.Count];
			  int i = 0;
			  foreach (byte? d in result)
			  {
				da[i++] = d.Value;
			  }
			  dataArray = da;
			  break;
			}

			case NetcdfSharp.ucar.ma2.DataType.USHORT:
			case NetcdfSharp.ucar.ma2.DataType.SHORT:
			case NetcdfSharp.ucar.ma2.DataType.ENUM2:
			{
			  List<short> result = new List<short>(initial);
			  while (sdataIter.hasNext())
			  {
				StructureData sdata = sdataIter.next();
				StructureMembers.Member realm = sdata.StructureMembers.findMember(proxym.Name);
				short[] data = sdata.getJavaArrayShort(realm);
				foreach (short aData in data)
				{
				  result.Add(aData);
				}
				count++;
			  }
			  short[] da = new short[result.Count];
			  int i = 0;
			  foreach (short? d in result)
			  {
				da[i++] = d.Value;
			  }
			  dataArray = da;
			  break;
			}

			case NetcdfSharp.ucar.ma2.DataType.UINT:
			case NetcdfSharp.ucar.ma2.DataType.INT:
			case NetcdfSharp.ucar.ma2.DataType.ENUM4:
			{
			  List<int> result = new List<int>(initial);
			  while (sdataIter.hasNext())
			  {
				StructureData sdata = sdataIter.next();
				StructureMembers.Member realm = sdata.StructureMembers.findMember(proxym.Name);
				int[] data = sdata.getJavaArrayInt(realm);
				foreach (int aData in data)
				{
				  result.Add(aData);
				}
				count++;
			  }
			  int[] da = new int[result.Count];
			  int i = 0;
			  foreach (int? d in result)
			  {
				da[i++] = d.Value;
			  }
			  dataArray = da;
			  break;
			}

			case NetcdfSharp.ucar.ma2.DataType.ULONG:
			case NetcdfSharp.ucar.ma2.DataType.LONG:
			{
			  List<long> result = new List<long>(initial);
			  while (sdataIter.hasNext())
			  {
				StructureData sdata = sdataIter.next();
				StructureMembers.Member realm = sdata.StructureMembers.findMember(proxym.Name);
				long[] data = sdata.getJavaArrayLong(realm);
				foreach (long aData in data)
				{
				  result.Add(aData);
				}
				count++;
			  }
			  long[] da = new long[result.Count];
			  int i = 0;
			  foreach (long? d in result)
			  {
				da[i++] = d.Value;
			  }
			  dataArray = da;
			  break;
			}

			case NetcdfSharp.ucar.ma2.DataType.CHAR:
			{
			  List<char> result = new List<char>(initial);
			  while (sdataIter.hasNext())
			  {
				StructureData sdata = sdataIter.next();
				StructureMembers.Member realm = sdata.StructureMembers.findMember(proxym.Name);
				char[] data = sdata.getJavaArrayChar(realm);
				foreach (char aData in data)
				{
				  result.Add(aData);
				}
				count++;
			  }
			  char[] da = new char[result.Count];
			  int i = 0;
			  foreach (char? d in result)
			  {
				da[i++] = d.Value;
			  }
			  dataArray = da;
			  break;
			}

			case NetcdfSharp.ucar.ma2.DataType.STRING:
			{
			  List<string> result = new List<string>(initial);
			  while (sdataIter.hasNext())
			  {
				StructureData sdata = sdataIter.next();
				StructureMembers.Member realm = sdata.StructureMembers.findMember(proxym.Name);
				string[] data = sdata.getJavaArrayString(realm);
				result.AddRange(data);
				count++;
			  }
			  string[] da = new string[result.Count];
			  int i = 0;
			  foreach (string d in result)
			  {
				da[i++] = d;
			  }
			  dataArray = da;
			  break;
			}

			case NetcdfSharp.ucar.ma2.DataType.STRUCTURE:
			{
			  List<StructureData> result = new List<StructureData>(initial);
			  while (sdataIter.hasNext())
			  {
				StructureData sdata = sdataIter.next();


				StructureMembers.Member realm = sdata.StructureMembers.findMember(proxym.Name);
				ArrayStructure @as = sdata.getArrayStructure(realm);
				StructureDataIterator innerIter = @as.StructureDataIterator;
				while (innerIter.hasNext())
				{
				  result.Add(innerIter.next());
				}
				count++;
			  }

			  rshape[0] = count;
			  StructureMembers membersw = proxym.StructureMembers.toBuilder(false).build();
			  return new ArrayStructureW(membersw, rshape, result.ToArray());
			}

			// add SEQUENCE data type
			case NetcdfSharp.ucar.ma2.DataType.SEQUENCE:
			{
			  List<ArraySequence> result = new List<ArraySequence>(initial);
			  while (sdataIter.hasNext())
			  {
				StructureData sdata = sdataIter.next();
				StructureMembers.Member realm = sdata.StructureMembers.findMember(proxym.Name);
				ArraySequence @as = sdata.getArraySequence(realm);
				result.Add(@as);
				count++;
			  }
			  ArraySequence[] da = new ArraySequence[result.Count];
			  int i = 0;
			  foreach (ArraySequence d in result)
			  {
				da[i++] = d;
			  }
			  dataArray = da;
			  break;
			}
		  }
		}

		// create an array to hold the result
		rshape[0] = count;
		return NcArray.factory(dataType, rshape, dataArray);
	  }

	  public override string ToString()
	  {
		return "seq n=" + nelems;
	  }

	  public override void showInternal(Formatter f, Indent indent)
	  {
		base.showInternal(f, indent);
		f.format("%sStructureDataIterator Class=%s hash=0x%x%n", indent, iter.GetType().FullName, iter.GetHashCode());
		if (iter is ArrayStructure.ArrayStructureIterator)
		{
		  ArrayStructure.ArrayStructureIterator ii = (ArrayStructure.ArrayStructureIterator) iter;
		  ii.ArrayStructure.showInternal(f, indent.incr());
		  indent.decr();
		}
	  }
	}

}
