// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using NetcdfSharp.java.nio;
using NetcdfSharp.java.util;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.ma2
{
	using Indent = NetcdfSharp.ucar.nc2.util.Indent;

	/// <summary>
	/// Concrete implementation of ArrayStructure, data storage is in a ByteBuffer, which is converted to member data on the
	/// fly.
	/// In order to use this, the records must have the same size, and the member offset must be the same for each record.
	/// Use StructureMembers.setStructureSize() to set the record size.
	/// Use StructureMembers.Member.setDataParam() to set the offset of the member from the start of each record.
	/// The member data will then be located in the BB at offset = recnum * getStructureSize() + member.getDataParam().
	/// This defers object creation for efficiency. Use getArray<type>() and getScalar<type>() data accessors if possible.
	/// 
	/// <pre>
	/// Structure pdata = (Structure) ncfile.findVariable(name);
	/// StructureMembers members = pdata.makeStructureMembers();
	/// members.findMember("value").setDataParam(0); // these are the offsets into the record
	/// members.findMember("x_start").setDataParam(2);
	/// members.findMember("y_start").setDataParam(4);
	/// members.findMember("direction").setDataParam(6);
	/// members.findMember("speed").setDataParam(8);
	/// int recsize = pos[1] - pos[0]; // each record must be all the same size
	/// members.setStructureSize(recsize);
	/// ArrayStructureBB asbb = new ArrayStructureBB(members, new int[] {size}, bos, pos[0]);
	/// </pre>
	/// 
	/// For String members, you must store the Strings in the stringHeap. An integer index into the heap is used in the
	/// ByteBuffer.
	/// 
	/// @author caron </summary>
	/// <seealso cref="Array"/>
	public class ArrayStructureBB : ArrayStructure
	{

	  /// <summary>
	  /// Set the offsets, based on m.getSizeBytes().
	  /// Also sets members.setStructureSize().
	  /// </summary>
	  /// <param name="members"> set offsets for these members </param>
	  /// <returns> the total size </returns>
	  public static int setOffsets(StructureMembers members)
	  {
		int offset = 0;
		foreach (StructureMembers.Member m in members.Members)
		{
		  m.DataParam = offset;
		  offset += m.SizeBytes;

		  // set inner offsets (starts again at 0)
		  if (m.StructureMembers != null)
		  {
			setOffsets(m.StructureMembers);
		  }
		}
		members.StructureSize = offset;
		return offset;
	  }

	  public static int showOffsets(StructureMembers members, Indent indent, Formatter f)
	  {
		int offset = 0;
		foreach (StructureMembers.Member m in members.Members)
		{
		  f.format("%s%s offset=%d (%d %s = %d bytes)%n", indent, m.Name, m.DataParam, m.Size, m.DataType, m.SizeBytes);

		  if (m.StructureMembers != null)
		  {
			indent.incr();
			StructureMembers nested = m.StructureMembers;
			f.format("%n%s%s == %d bytes%n", indent, nested.Name, nested.StructureSize);
			showOffsets(nested, indent, f);
			indent.decr();
		  }
		}
		return offset;
	  }

	  /////////////////////////////////////////////////////

	  protected internal ByteBuffer bbuffer;
	  protected internal int bb_offset;

	  /// <summary>
	  /// Create a new Array of type StructureData and the given members and shape.
	  /// Generally, you extract the byte array and fill it:
	  /// 
	  /// <pre>
	  /// byte[] result = (byte[]) structureArray.getStorage();
	  /// </pre>
	  /// </summary>
	  /// <param name="members"> a description of the structure members </param>
	  /// <param name="shape"> the shape of the Array. </param>
	  public ArrayStructureBB(StructureMembers members, int[] shape) : base(members, shape)
	  {
		this.bbuffer = ByteBuffer.allocate(nelems * StructureSize);
		bbuffer.order(ByteOrder.BIG_ENDIAN);
	  }

	  /// <summary>
	  /// Construct an ArrayStructureBB with the given ByteBuffer.
	  /// </summary>
	  /// <param name="members"> the list of structure members. </param>
	  /// <param name="shape"> the shape of the structure array </param>
	  /// <param name="bbuffer"> the data is stored in this ByteBuffer. bbuffer.order must already be set. </param>
	  /// <param name="offset"> offset from the start of the ByteBufffer to the first record. </param>
	  public ArrayStructureBB(StructureMembers members, int[] shape, ByteBuffer bbuffer, int offset) : base(members, shape)
	  {
		this.bbuffer = bbuffer;
		this.bb_offset = offset;
	  }

	  protected internal override StructureData makeStructureData(ArrayStructure @as, int index)
	  {
		return new StructureDataA(@as, index);
	  }

	  /// <summary>
	  /// Return backing storage as a ByteBuffer
	  /// </summary>
	  /// <returns> backing storage as a ByteBuffer </returns>
	  public virtual ByteBuffer ByteBuffer
	  {
		  get
		  {
			return bbuffer;
		  }
	  }

	  public override double getScalarDouble(int recnum, StructureMembers.Member m)
	  {
		if (m.DataType != DataType.DOUBLE)
		{
		  throw new ArgumentException("Type is " + m.DataType + ", must be double");
		}
		if (m.DataArray != null)
		{
		  return base.getScalarDouble(recnum, m);
		}

		int offset = calcOffsetSetOrder(recnum, m);
		return bbuffer.getDouble(offset);
	  }

	  public override double[] getJavaArrayDouble(int recnum, StructureMembers.Member m)
	  {
		if (m.DataType != DataType.DOUBLE)
		{
		  throw new ArgumentException("Type is " + m.DataType + ", must be double");
		}
		if (m.DataArray != null)
		{
		  return base.getJavaArrayDouble(recnum, m);
		}

		int offset = calcOffsetSetOrder(recnum, m);
		int count = m.Size;
		double[] pa = new double[count];
		for (int i = 0; i < count; i++)
		{
		  pa[i] = bbuffer.getDouble(offset + i * 8);
		}
		return pa;
	  }

	  protected internal override void copyDoubles(int recnum, StructureMembers.Member m, IndexIterator result)
	  {
		int offset = calcOffsetSetOrder(recnum, m);
		int count = m.Size;
		for (int i = 0; i < count; i++)
		{
		  result.DoubleNext = bbuffer.getDouble(offset + i * 8);
		}
	  }

	  public override float getScalarFloat(int recnum, StructureMembers.Member m)
	  {
		if (m.DataType != DataType.FLOAT)
		{
		  throw new ArgumentException("Type is " + m.DataType + ", must be float");
		}
		if (m.DataArray != null)
		{
		  return base.getScalarFloat(recnum, m);
		}

		int offset = calcOffsetSetOrder(recnum, m);
		return bbuffer.getFloat(offset);
	  }

	  public override NcArray getArray(int recnum, StructureMembers.Member m)
	  {
		if (m.VariableLength)
		{
		  int offset = calcOffsetSetOrder(recnum, m);
		  int heapIndex = bbuffer.getInt(offset);
		  return (NcArray) heap[heapIndex];
		}

		return base.getArray(recnum, m);
	  }

	  public override float[] getJavaArrayFloat(int recnum, StructureMembers.Member m)
	  {
		if (m.DataType != DataType.FLOAT)
		{
		  throw new ArgumentException("Type is " + m.DataType + ", must be float");
		}
		if (m.DataArray != null)
		{
		  return base.getJavaArrayFloat(recnum, m);
		}

		int offset = calcOffsetSetOrder(recnum, m);
		int count = m.Size;
		float[] pa = new float[count];
		for (int i = 0; i < count; i++)
		{
		  pa[i] = bbuffer.getFloat(offset + i * 4);
		}
		return pa;
	  }

	  protected internal override void copyFloats(int recnum, StructureMembers.Member m, IndexIterator result)
	  {
		int offset = calcOffsetSetOrder(recnum, m);
		int count = m.Size;
		for (int i = 0; i < count; i++)
		{
		  result.FloatNext = bbuffer.getFloat(offset + i * 4);
		}
	  }

	  public override byte getScalarByte(int recnum, StructureMembers.Member m)
	  {
		if (!(m.DataType.getPrimitiveClassType() == typeof(byte)))
		{
		  throw new ArgumentException("Type is " + m.DataType + ", must be byte");
		}
		if (m.DataArray != null)
		{
		  return base.getScalarByte(recnum, m);
		}

		int offset = calcOffsetSetOrder(recnum, m);
		return bbuffer.get(offset);
	  }

	  public override byte[] getJavaArrayByte(int recnum, StructureMembers.Member m)
	  {
		if (!(m.DataType.getPrimitiveClassType() == typeof(byte)))
		{
		  throw new ArgumentException("Type is " + m.DataType + ", must be byte");
		}
		if (m.DataArray != null)
		{
		  return base.getJavaArrayByte(recnum, m);
		}

		int offset = calcOffsetSetOrder(recnum, m);
		int count = m.Size;
		byte[] pa = new byte[count];
		for (int i = 0; i < count; i++)
		{
		  pa[i] = bbuffer.get(offset + i);
		}
		return pa;
	  }

	  protected internal override void copyBytes(int recnum, StructureMembers.Member m, IndexIterator result)
	  {
		int offset = calcOffsetSetOrder(recnum, m);
		int count = m.Size;
		for (int i = 0; i < count; i++)
		{
		  result.ByteNext = bbuffer.get(offset + i);
		}
	  }

	  public override short getScalarShort(int recnum, StructureMembers.Member m)
	  {
		if (!(m.DataType.getPrimitiveClassType() == typeof(short)))
		{
		  throw new ArgumentException("Type is " + m.DataType + ", must be short");
		}
		if (m.DataArray != null)
		{
		  return base.getScalarShort(recnum, m);
		}

		int offset = calcOffsetSetOrder(recnum, m);
		return bbuffer.getShort(offset);
	  }

	  public override short[] getJavaArrayShort(int recnum, StructureMembers.Member m)
	  {
		if (!(m.DataType.getPrimitiveClassType() == typeof(short)))
		{
		  throw new ArgumentException("Type is " + m.DataType + ", must be short");
		}
		if (m.DataArray != null)
		{
		  return base.getJavaArrayShort(recnum, m);
		}

		int offset = calcOffsetSetOrder(recnum, m);
		int count = m.Size;
		short[] pa = new short[count];
		for (int i = 0; i < count; i++)
		{
		  pa[i] = bbuffer.getShort(offset + i * 2);
		}
		return pa;
	  }

	  protected internal override void copyShorts(int recnum, StructureMembers.Member m, IndexIterator result)
	  {
		int offset = calcOffsetSetOrder(recnum, m);
		int count = m.Size;
		for (int i = 0; i < count; i++)
		{
		  result.ShortNext = bbuffer.getShort(offset + i * 2);
		}
	  }

	  public override int getScalarInt(int recnum, StructureMembers.Member m)
	  {
		if (!(m.DataType.getPrimitiveClassType() == typeof(int)))
		{
		  throw new ArgumentException("Type is " + m.DataType + ", must be int");
		}
		if (m.DataArray != null)
		{
		  return base.getScalarInt(recnum, m);
		}

		int offset = calcOffsetSetOrder(recnum, m);
		return bbuffer.getInt(offset);
	  }

	  public override int[] getJavaArrayInt(int recnum, StructureMembers.Member m)
	  {
		if (!(m.DataType.getPrimitiveClassType() == typeof(int)))
		{
		  throw new ArgumentException("Type is " + m.DataType + ", must be int");
		}
		if (m.DataArray != null)
		{
		  return base.getJavaArrayInt(recnum, m);
		}

		int offset = calcOffsetSetOrder(recnum, m);
		int count = m.Size;
		int[] pa = new int[count];
		for (int i = 0; i < count; i++)
		{
		  pa[i] = bbuffer.getInt(offset + i * 4);
		}
		return pa;
	  }

	  protected internal override void copyInts(int recnum, StructureMembers.Member m, IndexIterator result)
	  {
		int offset = calcOffsetSetOrder(recnum, m);
		int count = m.Size;
		for (int i = 0; i < count; i++)
		{
		  result.IntNext = bbuffer.getInt(offset + i * 4);
		}
	  }

	  public override long getScalarLong(int recnum, StructureMembers.Member m)
	  {
		if (!(m.DataType.getPrimitiveClassType() == typeof(long)))
		{
		  throw new ArgumentException("Type is " + m.DataType + ", must be long");
		}
		if (m.DataArray != null)
		{
		  return base.getScalarLong(recnum, m);
		}

		int offset = calcOffsetSetOrder(recnum, m);
		return bbuffer.getLong(offset);
	  }

	  public override long[] getJavaArrayLong(int recnum, StructureMembers.Member m)
	  {
		if (!(m.DataType.getPrimitiveClassType() == typeof(long)))
		{
		  throw new ArgumentException("Type is " + m.DataType + ", must be long");
		}
		if (m.DataArray != null)
		{
		  return base.getJavaArrayLong(recnum, m);
		}

		int offset = calcOffsetSetOrder(recnum, m);
		int count = m.Size;
		long[] pa = new long[count];
		for (int i = 0; i < count; i++)
		{
		  pa[i] = bbuffer.getLong(offset + i * 8);
		}
		return pa;
	  }

	  protected internal override void copyLongs(int recnum, StructureMembers.Member m, IndexIterator result)
	  {
		int offset = calcOffsetSetOrder(recnum, m);
		int count = m.Size;
		for (int i = 0; i < count; i++)
		{
		  result.LongNext = bbuffer.getLong(offset + i * 8);
		}
	  }

	  public override char getScalarChar(int recnum, StructureMembers.Member m)
	  {
		if (m.DataType != DataType.CHAR)
		{
		  throw new ArgumentException("Type is " + m.DataType + ", must be char");
		}
		if (m.DataArray != null)
		{
		  return base.getScalarChar(recnum, m);
		}

		int offset = calcOffsetSetOrder(recnum, m);
		return (char) bbuffer.get(offset);
	  }

	  public override char[] getJavaArrayChar(int recnum, StructureMembers.Member m)
	  {
		if (m.DataType != DataType.CHAR)
		{
		  throw new ArgumentException("Type is " + m.DataType + ", must be char");
		}
		if (m.DataArray != null)
		{
		  return base.getJavaArrayChar(recnum, m);
		}

		int offset = calcOffsetSetOrder(recnum, m);
		int count = m.Size;
		char[] pa = new char[count];
		for (int i = 0; i < count; i++)
		{
		  pa[i] = (char) bbuffer.get(offset + i);
		}
		return pa;
	  }

	  protected internal override void copyChars(int recnum, StructureMembers.Member m, IndexIterator result)
	  {
		int offset = calcOffsetSetOrder(recnum, m);
		int count = m.Size;
		for (int i = 0; i < count; i++)
		{
		  result.ByteNext = bbuffer.get(offset + i);
		}
	  }

	  public override string getScalarString(int recnum, StructureMembers.Member m)
	  {
		if (m.DataArray != null)
		{
		  return base.getScalarString(recnum, m);
		}

		// strings are stored on the "heap", and the index to the heap is kept in the bbuffer
		if (m.DataType == DataType.STRING)
		{
		  int offset = calcOffsetSetOrder(recnum, m);
		  int index = bbuffer.getInt(offset);
		  object data = heap[index];
		  if (data is string)
		  {
			return (string) data;
		  }
		  return ((string[]) data)[0];
		}

		if (m.DataType == DataType.CHAR)
		{
		  int offset = calcOffsetSetOrder(recnum, m);
		  int count = m.Size;
		  byte[] pa = new byte[count];
		  int i;
		  for (i = 0; i < count; i++)
		  {
			pa[i] = bbuffer.get(offset + i);
			if (0 == pa[i])
			{
			  break;
			}
		  }
		  return Encoding.UTF8.GetString(pa, 0, i);
		}

		throw new ArgumentException("Type is " + m.DataType + ", must be String or char");
	  }

	  public override string[] getJavaArrayString(int recnum, StructureMembers.Member m)
	  {
		if (m.DataArray != null)
		{
		  return base.getJavaArrayString(recnum, m);
		}

		// strings are stored on the "heap", and the index to the heap is kept in the bbuffer
		if (m.DataType == DataType.STRING)
		{
		  int offset = calcOffsetSetOrder(recnum, m);
		  int heapIndex = bbuffer.getInt(offset);
		  object ho = heap[heapIndex];
		  if (ho is string[])
		  {
			return (string[]) ho;
		  }
		  else if (ho is string)
		  {
			string[] result = new string[1];
			result[0] = (string) ho;
			return result;
		  }
		  else
		  {
// JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			throw new ArgumentException("Expected a String, but found an object of type  " + ho.GetType().FullName + ", on heap for " + " member " + m.Name);
		  }
		}

		if (m.DataType == DataType.CHAR)
		{
		  int[] shape = m.Shape;
		  int rank = shape.Length;
		  if (rank < 2)
		  {
			string[] result1 = new string[1];
			result1[0] = getScalarString(recnum, m);
			return result1;
		  }

		  int strlen = shape[rank - 1];


		  int n = m.Size / strlen;
		  int offset = calcOffsetSetOrder(recnum, m);
		  string[] result = new string[n];
		  for (int i = 0; i < n; i++)
		  {
			byte[] bytes = new byte[strlen];
			for (int j = 0; j < bytes.Length; j++)
			{
			  bytes[j] = bbuffer.get(offset + i * strlen + j);
			}
			result[i] = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
		  }
		  return result;
		}

		throw new ArgumentException("Type is " + m.DataType + ", must be char");
	  }

	  protected internal override void copyObjects(int recnum, StructureMembers.Member m, IndexIterator result)
	  {
		int offset = calcOffsetSetOrder(recnum, m);
		int count = m.Size;
		int index = bbuffer.getInt(offset);
		string[] data = (string[]) heap[index];

		for (int i = 0; i < count; i++)
		{
		  result.ObjectNext = data[i];
		}
	  }

	  // LOOK not tested ??
	  public override StructureData getScalarStructure(int recnum, StructureMembers.Member m)
	  {
		if (m.DataType != DataType.STRUCTURE)
		{
		  throw new ArgumentException("Type is " + m.DataType + ", must be Structure");
		}
		if (m.DataArray != null)
		{
		  return base.getScalarStructure(recnum, m);
		}

		int offset = calcOffsetSetOrder(recnum, m);
		ArrayStructureBB subset = new ArrayStructureBB(m.StructureMembers, new int[] {1}, this.bbuffer, offset);

		return new StructureDataA(subset, 0);
	  }

	  public override ArrayStructure getArrayStructure(int recnum, StructureMembers.Member m)
	  {
		if (m.DataType != DataType.STRUCTURE)
		{
		  throw new ArgumentException("Type is " + m.DataType + ", must be Structure");
		}
		if (m.DataArray != null)
		{
		  return base.getArrayStructure(recnum, m);
		}

		int offset = calcOffsetSetOrder(recnum, m);
		ArrayStructureBB result = new ArrayStructureBB(m.StructureMembers, m.Shape, this.bbuffer, offset);
		result.heap = this.heap; // share the heap
		return result;
	  }

	  public override ArraySequence getArraySequence(int recnum, StructureMembers.Member m)
	  {
		if (m.DataType != DataType.SEQUENCE)
		{
		  throw new ArgumentException("Type is " + m.DataType + ", must be Sequence");
		}
		// if (m.DataArray != null) return super.getArrayStructure(recnum, m);

		int offset = calcOffsetSetOrder(recnum, m);
		int index = bbuffer.getInt(offset);
		if (heap == null)
		{
		  throw new System.InvalidOperationException("getArraySequence heap is null");
		}
		object ho = heap[index];
		if (ho is ArraySequence)
		{
		  return (ArraySequence) heap[index];
		}
		return null; // LOOK
	  }

	  protected internal virtual int calcOffsetSetOrder(int recnum, StructureMembers.Member m)
	  {
		if (null != m.DataObject)
		{
		  bbuffer.order((ByteOrder) m.DataObject);
		}
		return bb_offset + recnum * StructureSize + m.DataParam;
	  }

	  /*
	   * int index = asbb.addObjectToHeap(s);
	   * bb.order( ByteOrder.nativeOrder()); // the string index is always written in "native order"
	   * bb.putInt(destPos + i * 4, index); // overwrite with the index into the StringHeap
	   */
	  private IList<object> heap;

	  public virtual int addObjectToHeap(object s)
	  {
		if (null == heap)
		{
		  heap = new List<object>();
		}
		heap.Add(s);
		return heap.Count - 1;
	  }

	  public virtual void addObjectToHeap(int recnum, StructureMembers.Member m, object s)
	  {
		if (null == heap)
		{
		  heap = new List<object>();
		}
		heap.Add(s);
		int index = heap.Count - 1;
		// was setInt(calcOffsetSetOrder(recnum, m), index) jc 12/4/2012
		bbuffer.putInt(calcOffsetSetOrder(recnum, m), index);
	  }

	  /// <summary>
	  /// DO NOT MODIFY
	  /// </summary>
	  /// <returns> heap </returns>
	  public virtual IList<object> getHeap()
	  {
		return heap;
	  }

	  public override void showInternal(Formatter f, Indent indent)
	  {
		base.showInternal(f, indent);

		f.format("%sByteBuffer = %s (hash=0x%x)%n", indent, bbuffer, bbuffer.GetHashCode());
		if (null != heap)
		{
		  f.format("%s  Heap Objects%n", indent);
		  for (int i = 0; i < heap.Count; i++)
		  {
			object o = heap[i];
			f.format("%s   %d class=%s hash=0x%x = %s%n", indent, i, o.GetType().FullName, o.GetHashCode(), o);
			if (o is ArrayStructure)
			{
			  ((ArrayStructure) o).showInternal(f, indent.incr());
			  indent.decr();
			}
		  }
		  f.format("%n");
		}
	  }
	}

}
