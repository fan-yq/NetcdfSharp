// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.ma2
{
	using Structure = NetcdfSharp.ucar.nc2.Structure;
	using Variable = NetcdfSharp.ucar.nc2.Variable;
	using Sequence = NetcdfSharp.ucar.nc2.Sequence;

	/// <summary>
	/// Concrete implementation of ArrayStructure, data storage is in member arrays, which are converted to
	/// StructureData member data on the fly.
	/// This defers object creation for efficiency. Use getJavaArrayXXX<type>() and getScalarXXX<type>() data accessors if
	/// possible.
	/// 
	/// How to create:
	/// 
	/// <pre>
	/// ArrayStructureMA asma = new ArrayStructureMA(smembers, getShape());
	/// for (int i = 0; i < orgVariables.size(); i++) {
	///   Variable v = (Variable) orgVariables.get(i);
	///   Array data = v.read();
	///   asma.setMemberArray(v.getName(), data);
	/// }
	/// </pre>
	/// 
	/// How to do Nested Structures:
	/// 
	/// <pre>
	///  Structure {
	///    float f1;
	///    short f2(3);
	/// 
	///    Structure {
	///      int g1;
	///      double(2) g2;
	///      double(3,4) g3;
	/// 
	///      Structure {
	///        int h1;
	///        double(2) h2;
	///      } nested2(7);
	/// 
	///    } nested1(12);
	///  } s(4);
	/// </pre>
	/// <ul>
	/// <li>For f1, you need an ArrayFloat of shape {4}
	/// <li>For f2, you need an ArrayShort of shape {4, 3} .
	/// <li>For nested1, you need an ArrayStructure of shape {4, 12}.
	/// Use an ArrayStructureMA that has 3 members:
	/// <ul>
	/// <li>For g1, you need an ArrayInt of shape (4, 12}
	/// <li>For g2, you need an ArrayDouble of shape {4, 12, 2}.
	/// <li>For g3, you need an ArrayDouble of shape {4, 12, 3, 4}.
	/// </ul>
	/// <li>For nested2, you need an ArrayStructure of shape {4, 12, 7}.
	/// Use an ArrayStructureMA that has 2 members:
	/// <ul>
	/// <li>For h1, you need an ArrayInt of shape (4, 12, 7}
	/// <li>For h2, you need an ArrayDouble of shape {4, 12, 7, 2}.
	/// </ul>
	/// </ul>
	/// Example code:
	/// 
	/// <pre>
	/// public void testMA() throws IOException, InvalidRangeException {
	///   StructureMembers members = new StructureMembers("s");
	/// 
	///   StructureMembers.Member m = members.addMember("f1", "desc", CDM.UNITS, DataType.FLOAT, new int[] {1});
	///   Array data = Array.factory(DataType.FLOAT, new int[] {4});
	///   m.setDataArray(data);
	///   fill(data);
	/// 
	///   m = members.addMember("f2", "desc", CDM.UNITS, DataType.SHORT, new int[] {3});
	///   data = Array.factory(DataType.SHORT, new int[] {4, 3});
	///   m.setDataArray(data);
	///   fill(data);
	/// 
	///   m = members.addMember("nested1", "desc", CDM.UNITS, DataType.STRUCTURE, new int[] {9});
	///   data = makeNested1(m);
	///   m.setDataArray(data);
	/// 
	///   ArrayStructureMA as = new ArrayStructureMA(members, new int[] {4});
	///   new TestStructureArray().testArrayStructure(as);
	/// 
	///   // get f2 out of the 3nd "s"
	///   StructureMembers.Member f2 = as.getStructureMembers().findMember("f2");
	///   short[] f2data = as.getJavaArrayShort(2, f2);
	///   assert f2data[0] == 20;
	///   assert f2data[1] == 21;
	///   assert f2data[2] == 22;
	/// 
	///   // get nested1 out of the 3nd "s"
	///   StructureMembers.Member nested1 = as.getStructureMembers().findMember("nested1");
	///   ArrayStructure nested1Data = as.getArrayStructure(2, nested1);
	/// 
	///   // get g1 out of the 7th "nested1"
	///   StructureMembers.Member g1 = nested1Data.getStructureMembers().findMember("g1");
	///   int g1data = nested1Data.getScalarInt(6, g1);
	///   assert g1data == 26;
	/// 
	///   // get nested2 out of the 7th "nested1"
	///   StructureMembers.Member nested2 = nested1Data.getStructureMembers().findMember("nested2");
	///   ArrayStructure nested2Data = nested1Data.getArrayStructure(6, nested2);
	/// 
	///   // get h1 out of the 4th "nested2"
	///   StructureMembers.Member h1 = nested2Data.getStructureMembers().findMember("h1");
	///   int val = nested2Data.getScalarInt(4, h1);
	///   assert (val == 264);
	/// }
	/// 
	/// 
	/// public ArrayStructure makeNested1(StructureMembers.Member parent) throws IOException, InvalidRangeException {
	///   StructureMembers members = new StructureMembers(parent.getName());
	///   parent.setStructureMembers(members);
	/// 
	///   StructureMembers.Member m = members.addMember("g1", "desc", CDM.UNITS, DataType.INT, new int[] {1});
	///   Array data = Array.factory(DataType.INT, new int[] {4, 9});
	///   m.setDataArray(data);
	///   fill(data);
	/// 
	///   m = members.addMember("g2", "desc", CDM.UNITS, DataType.DOUBLE, new int[] {2});
	///   data = Array.factory(DataType.DOUBLE, new int[] {4, 9, 2});
	///   m.setDataArray(data);
	///   fill(data);
	/// 
	///   m = members.addMember("g3", "desc", CDM.UNITS, DataType.DOUBLE, new int[] {3, 4});
	///   data = Array.factory(DataType.DOUBLE, new int[] {4, 9, 3, 4});
	///   m.setDataArray(data);
	///   fill(data);
	/// 
	///   m = members.addMember("nested2", "desc", CDM.UNITS, DataType.STRUCTURE, new int[] {7});
	///   data = makeNested2(m);
	///   m.setDataArray(data);
	/// 
	///   return new ArrayStructureMA(members, new int[] {4, 9});
	/// }
	/// 
	/// public ArrayStructure makeNested2(StructureMembers.Member parent) throws IOException, InvalidRangeException {
	///   StructureMembers members = new StructureMembers(parent.getName());
	///   parent.setStructureMembers(members);
	/// 
	///   StructureMembers.Member m = members.addMember("h1", "desc", CDM.UNITS, DataType.INT, new int[] {1});
	///   Array data = Array.factory(DataType.INT, new int[] {4, 9, 7});
	///   m.setDataArray(data);
	///   fill(data);
	/// 
	///   m = members.addMember("h2", "desc", CDM.UNITS, DataType.DOUBLE, new int[] {2});
	///   data = Array.factory(DataType.DOUBLE, new int[] {4, 9, 7, 2});
	///   m.setDataArray(data);
	///   fill(data);
	/// 
	///   return new ArrayStructureMA(members, new int[] {4, 9, 7});
	/// }
	/// </pre>
	/// 
	/// @author caron </summary>
	/// <seealso cref="Array"/>
	public class ArrayStructureMA : ArrayStructure
	{
	  /*
	   * Implementation notes
	   * Most of the methods are now the default methods in the superclass, so that other subclasses can call them.
	   * This happens when the data is "enhanced", member arrays are set and must override the other possible storage
	   * methods.
	   */

	  /// <summary>
	  /// Create a new Array of type StructureData and the given members and shape.
	  /// <para>
	  /// You must set the data Arrays on each of the Members, using setDataObject(). These data Arrays contain the data
	  /// for that member Variable, for all the StructureData. Therefore it has rank one greater that the Members. The extra
	  /// dimension must be the outermost (slowest varying) dimension. ie, if some member has shape [3,10], the array would
	  /// have
	  /// shape [nrows, 3, 10].
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="members"> a description of the structure members </param>
	  /// <param name="shape"> the shape of the Array. </param>
	  public ArrayStructureMA(StructureMembers members, int[] shape) : base(members, shape)
	  {
	  }

	  public ArrayStructureMA(StructureMembers members, int[] shape, StructureData[] sdata) : base(members, shape)
	  {
		if (nelems != sdata.Length)
		{
		  throw new ArgumentException("StructureData length= " + sdata.Length + "!= shape.length=" + nelems);
		}
		this.sdata = sdata;
	  }

	  /// <summary>
	  /// Turn any ArrayStructure into a ArrayStructureMA
	  /// </summary>
	  /// <param name="from"> copy from here. If from is a ArrayStructureMA, return it. </param>
	  /// <returns> equivalent ArrayStructureMA </returns>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on error reading a sequence </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static ArrayStructureMA factoryMA(ArrayStructure from) throws NetcdfSharp.java.io.IOException
	  public static ArrayStructureMA factoryMA(ArrayStructure from)
	  {
		if (from is ArrayStructureMA)
		{
		  return (ArrayStructureMA) from;
		}

		// To create an ArrayStructureMA that we can iterate over later, we need to know the shape of "from".
		if (from.Size > 0)
		{
		  ArrayStructureMA resultMA = new ArrayStructureMA(from.StructureMembers.toBuilder(false).build(), from.Shape);
		  foreach (StructureMembers.Member m in from.Members)
		  {
			resultMA.setMemberArray(m.Name, from.extractMemberArray(m));
		  }
		  return resultMA;
		}

		// from.getSize() <= 0. This usually means that "from" is an ArraySequence, and that we won't know its size until
		// we iterate over it. extractMemberArray() will do that iteration for us, and then we can use the size of the
		// array it returns to determine the shape of "from".

		int numRecords = -1;
		IDictionary<string, NcArray> memberArrayMap = new LinkedHashMap<string, NcArray>();

		foreach (StructureMembers.Member m in from.Members)
		{
		  NcArray array = from.extractMemberArray(m);
		  Debug.Assert(array.Size > 0, "array's size should have been computed in extractMemberArray().");
		  int firstDimLen = array.Shape[0];

		  if (numRecords == -1)
		  {
			numRecords = firstDimLen;
		  }
		  else
		  {
			Debug.Assert(numRecords == firstDimLen, string.Format("Expected all structure members to have the same first" + "dimension length, but {0:D} != {1:D}.", numRecords, firstDimLen));
		  }

		  memberArrayMap[m.Name] = array;
		}

		int[] shape;
		if (numRecords == -1)
		{
		  shape = new int[] {0}; // "from" really was empty.
		}
		else
		{
		  shape = new int[] {numRecords};
		}

		ArrayStructureMA to = new ArrayStructureMA(from.StructureMembers.toBuilder(false).build(), shape);

		foreach (KeyValuePair<string, NcArray> entry in memberArrayMap.SetOfKeyValuePairs())
		{
		  to.setMemberArray(entry.Key, entry.Value);
		}

		return to;
	  }

	  protected internal override StructureData makeStructureData(ArrayStructure @as, int index)
	  {
		return new StructureDataA(@as, index);
	  }

	  /// <summary>
	  /// Set the data array for this member.
	  /// </summary>
	  /// <param name="memberName"> name of member </param>
	  /// <param name="data"> Array for this member. </param>
	  public virtual void setMemberArray(string memberName, NcArray data)
	  {
		StructureMembers.Member m = members.findMember(memberName);
		m.DataArray = data;
	  }

	  public override NcArray getArray(int recno, StructureMembers.Member m)
	  {
		if (m.VariableLength)
		{
		  NcArray data = m.DataArray;
		  if (data is ArrayObject)
		  {
			ArrayObject ao = (ArrayObject) data;
			object dao = ao.getObject(recno);
			Debug.Assert(dao is NcArray);
			return (NcArray) dao;
		  }
		}
		return base.getArray(recno, m);
	  }

	  /// <summary>
	  /// Create an ArrayStructure for a Structure. Allow nested Structures.
	  /// Create the data arrays, and an iterator.
	  /// </summary>
	  /// <param name="from"> copy from here. If from is a ArrayStructureMA, return it. </param>
	  /// <param name="shape"> the shape of the ArrayStructure </param>
	  /// <returns> equivilent ArrayStructureMA </returns>
	  public static ArrayStructureMA factoryMA(Structure from, int[] shape)
	  {
		StructureMembers sm = from.makeStructureMembers();
		foreach (Variable v in from.Variables)
		{
		  NcArray data;
		  if (v is Sequence)
		  {
			data = NcArray.factory(DataType.SEQUENCE, shape); // an array sequence - one for each parent element
			// Structure s = (Structure) v;
			// StructureMembers smn = s.makeStructureMembers();
			// data = new ArraySequenceNested(smn, (int) Index.computeSize(v.getShapeAll())); // ??

		  }
		  else if (v is Structure)
		  {
			data = ArrayStructureMA.factoryMA((Structure) v, combine(shape, v.Shape));
		  }

		  else
		  {
			data = NcArray.factory(v.DataType, combine(shape, v.Shape));
		  }

		  StructureMembers.Member m = sm.findMember(v.ShortName);
		  m.DataArray = data;
		}

		return new ArrayStructureMA(sm, shape);
	  }

	  private static int[] combine(int[] shape1, int[] shape2)
	  {
		int[] result = new int[shape1.Length + shape2.Length];
		System.Array.Copy(shape1, 0, result, 0, shape1.Length);
		System.Array.Copy(shape2, 0, result, shape1.Length, shape2.Length);
		return result;
	  }


	}

}
