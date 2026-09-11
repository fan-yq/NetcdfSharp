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
	/// Handles nested sequences: a 1D array of variable length 1D arrays of StructureData.
	/// Uses same technique as ArrayStructureMA for the inner fields; data storage is in member arrays.
	/// Used only by opendap internals.
	/// 
	/// Example use:
	/// 
	/// <pre>
	/// ArraySequence aseq = new ArraySequence(members, outerLength);
	/// for (int seq = 0; seq < outerLength; seq++) {
	///   aseq.setSequenceLength(seq, seqLength);
	/// }
	/// aseq.finish();
	/// </pre>
	/// 
	/// @author caron
	/// </summary>
	public class ArraySequenceNested : ArrayStructure
	{
	  private int[] sequenceLen;
	  private int[] sequenceOffset;
	  private int total;

	  /// <summary>
	  /// This is used for inner sequences, ie variable length structures nested inside of another structure.
	  /// </summary>
	  /// <param name="members"> the members of the STructure </param>
	  /// <param name="nseq"> the number of sequences, ie the length of the outer structure. </param>
	  public ArraySequenceNested(StructureMembers members, int nseq) : base(members, new int[] {nseq})
	  {
		sequenceLen = new int[nseq];
	  }

	  // not sure how this is used
	  protected internal override StructureData makeStructureData(ArrayStructure @as, int index)
	  {
		return new StructureDataA(@as, index);
	  }

	  public override StructureData getStructureData(int index)
	  {
		return new StructureDataA(this, index);
	  }

	  /// <summary>
	  /// Set the length of one of the sequences.
	  /// </summary>
	  /// <param name="outerIndex"> which sequence? </param>
	  /// <param name="len"> what is its length? </param>
	  public virtual void setSequenceLength(int outerIndex, int len)
	  {
		sequenceLen[outerIndex] = len;
	  }

	  /// <summary>
	  /// Get the length of the ith sequence.
	  /// </summary>
	  /// <param name="outerIndex"> which sequence? </param>
	  /// <returns> its length </returns>
	  public virtual int getSequenceLength(int outerIndex)
	  {
		return sequenceLen[outerIndex];
	  }

	  /// <summary>
	  /// Get the the starting index of the ith sequence.
	  /// </summary>
	  /// <param name="outerIndex"> which sequence? </param>
	  /// <returns> its starting index </returns>
	  public virtual int getSequenceOffset(int outerIndex)
	  {
		return sequenceOffset[outerIndex];
	  }

	  /// <summary>
	  /// Call this when you have set all the sequence lengths.
	  /// </summary>
	  public virtual void finish()
	  {
		sequenceOffset = new int[nelems];

		total = 0;
		for (int i = 0; i < nelems; i++)
		{
		  sequenceOffset[i] = total;
		  total += sequenceLen[i];
		}

		sdata = new StructureData[nelems];
		for (int i = 0; i < nelems; i++)
		{
		  sdata[i] = new StructureDataA(this, sequenceOffset[i]);
		}

		// make the member arrays
		foreach (StructureMembers.Member m in members.Members)
		{
		  int[] mShape = m.Shape;
		  int[] shape = new int[mShape.Length + 1];
		  shape[0] = total;
		  System.Array.Copy(mShape, 0, shape, 1, mShape.Length);

		  // LOOK not doing nested structures
		  NcArray data = NcArray.factory(m.DataType, shape);
		  m.DataArray = data;
		}
	  }

	  /// <returns> the total number of Structures over all the nested sequences. </returns>
	  public virtual int TotalNumberOfStructures
	  {
		  get
		  {
			return total;
		  }
	  }

	  /// <summary>
	  /// Flatten the Structures into a 1D array of Structures of length getTotalNumberOfStructures().
	  /// </summary>
	  /// <returns> Array of Structures </returns>
	  public virtual ArrayStructure flatten()
	  {
		ArrayStructureW aw = new ArrayStructureW(StructureMembers, new int[] {total});
		for (int i = 0; i < total; i++)
		{
		  StructureData sdata = new StructureDataA(this, i);
		  aw.setStructureData(sdata, i);
		}
		return aw;
	  }

	  public override double getScalarDouble(int recnum, StructureMembers.Member m)
	  {
		if (m.DataType != DataType.DOUBLE)
		{
		  throw new ArgumentException("Type is " + m.DataType + ", must be double");
		}
		NcArray data = m.DataArray;
		return data.getDouble(recnum * m.Size); // gets first one in the array
	  }

	  public override double[] getJavaArrayDouble(int recnum, StructureMembers.Member m)
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

	  public override float getScalarFloat(int recnum, StructureMembers.Member m)
	  {
		if (m.DataType != DataType.FLOAT)
		{
		  throw new ArgumentException("Type is " + m.DataType + ", must be float");
		}
		NcArray data = m.DataArray;
		return data.getFloat(recnum * m.Size); // gets first one in the array
	  }

	  public override float[] getJavaArrayFloat(int recnum, StructureMembers.Member m)
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

	  public override byte getScalarByte(int recnum, StructureMembers.Member m)
	  {
		if (!(m.DataType.getPrimitiveClassType() == typeof(byte)))
		{
		  throw new ArgumentException("Type is " + m.DataType + ", must be byte");
		}
		NcArray data = m.DataArray;
		return data.getByte(recnum * m.Size); // gets first one in the array
	  }

	  public override byte[] getJavaArrayByte(int recnum, StructureMembers.Member m)
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

	  public override short getScalarShort(int recnum, StructureMembers.Member m)
	  {
		if (!(m.DataType.getPrimitiveClassType() == typeof(short)))
		{
		  throw new ArgumentException("Type is " + m.DataType + ", must be short");
		}
		NcArray data = m.DataArray;
		return data.getShort(recnum * m.Size); // gets first one in the array
	  }

	  public override short[] getJavaArrayShort(int recnum, StructureMembers.Member m)
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

	  public override int getScalarInt(int recnum, StructureMembers.Member m)
	  {
		if (!(m.DataType.getPrimitiveClassType() == typeof(int)))
		{
		  throw new ArgumentException("Type is " + m.DataType + ", must be int");
		}
		NcArray data = m.DataArray;
		return data.getInt(recnum * m.Size); // gets first one in the array
	  }

	  public override int[] getJavaArrayInt(int recnum, StructureMembers.Member m)
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

	  public override long getScalarLong(int recnum, StructureMembers.Member m)
	  {
		if (!(m.DataType.getPrimitiveClassType() == typeof(long)))
		{
		  throw new ArgumentException("Type is " + m.DataType + ", must be long");
		}
		NcArray data = m.DataArray;
		return data.getLong(recnum * m.Size); // gets first one in the array
	  }

	  public override long[] getJavaArrayLong(int recnum, StructureMembers.Member m)
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

	  public override char getScalarChar(int recnum, StructureMembers.Member m)
	  {
		if (m.DataType != DataType.CHAR)
		{
		  throw new ArgumentException("Type is " + m.DataType + ", must be char");
		}
		NcArray data = m.DataArray;
		return data.getChar(recnum * m.Size); // gets first one in the array
	  }

	  public override char[] getJavaArrayChar(int recnum, StructureMembers.Member m)
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

	  public override string getScalarString(int recnum, StructureMembers.Member m)
	  {
		if (m.DataType == DataType.CHAR)
		{
		  ArrayChar data = (ArrayChar) m.DataArray;
		  return data.getString(recnum);
		}

		if (m.DataType == DataType.STRING)
		{
		  ArrayObject data = (ArrayObject) m.DataArray;
		  return (string) data.getObject(recnum);
		}

		throw new ArgumentException("Type is " + m.DataType + ", must be String or char");
	  }

	  public override string[] getJavaArrayString(int recnum, StructureMembers.Member m)
	  {
		int n = m.Size;
		string[] result = new string[n];

		if (m.DataType == DataType.CHAR)
		{

		  ArrayChar data = (ArrayChar) m.DataArray;
		  for (int i = 0; i < n; i++)
		  {
			result[i] = data.getString(recnum * n + i);
		  }
		  return result;

		}
		else if (m.DataType == DataType.STRING)
		{

		  NcArray data = m.DataArray;
		  for (int i = 0; i < n; i++)
		  {
			result[i] = (string) data.getObject(recnum * n + i);
		  }
		  return result;
		}

		throw new ArgumentException("Type is " + m.DataType + ", must be String or char");
	  }

	  public override StructureData getScalarStructure(int recnum, StructureMembers.Member m)
	  {
		if (m.DataType == DataType.STRUCTURE)
		{
		  ArrayStructure data = (ArrayStructure) m.DataArray;
		  return data.getStructureData(recnum * m.Size); // gets first in the array
		}

		throw new ArgumentException("Type is " + m.DataType + ", must be Structure");
	  }

	  public override ArrayStructure getArrayStructure(int recnum, StructureMembers.Member m)
	  {
		if (m.DataType == DataType.STRUCTURE)
		{
		  ArrayStructure data = (ArrayStructure) m.DataArray;
		  // we need to subset this array structure to deal with just the subset for this recno
		  // use "brute force" for now, see if we can finesse later
		  int count = m.Size;
		  StructureData[] sdata = new StructureData[count];
		  for (int i = 0; i < count; i++)
		  {
			sdata[i] = data.getStructureData(recnum * count + i);
		  }

		  return new ArrayStructureW(data.StructureMembers, m.Shape, sdata);
		}

		throw new ArgumentException("Type is " + m.DataType + ", must be Structure");
	  }

	}

}
