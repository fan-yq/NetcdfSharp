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
	/// Concrete implementation of ArrayStructure, with data access deferred to the StructureData objects.
	/// The StructureData objects may be of any subclass.
	/// Using ArrayStructureW and StructureDataW is often the easiest to construct, but not efficient for large arrays
	/// of Structures due to excessive object creation.
	/// 
	/// @author caron </summary>
	/// <seealso cref="Array"/>
	public class ArrayStructureW : ArrayStructure
	{

	  /// <summary>
	  /// Create a new Array of type StructureData and the given members and shape.
	  /// You must completely construct by calling setStructureData()
	  /// </summary>
	  /// <param name="members"> a description of the structure members </param>
	  /// <param name="shape"> the shape of the Array. </param>
	  public ArrayStructureW(StructureMembers members, int[] shape) : base(members, shape)
	  {
		this.sdata = new StructureData[nelems];
	  }

	  // wrap a StructureDatain an ArrrayStructure
	  public ArrayStructureW(StructureData sdata) : base(sdata.StructureMembers, new int[] {1})
	  {
		this.sdata = new StructureData[1];
		this.sdata[0] = sdata;
	  }

	  /// <summary>
	  /// Create a new Array of type StructureData and the given members, shape, and array of StructureData.
	  /// </summary>
	  /// <param name="members"> a description of the structure members </param>
	  /// <param name="shape"> the shape of the Array. </param>
	  /// <param name="sdata"> StructureData array, must be </param>
	  public ArrayStructureW(StructureMembers members, int[] shape, StructureData[] sdata) : base(members, shape)
	  {
		if (nelems != sdata.Length)
		{
		  throw new ArgumentException("StructureData length= " + sdata.Length + "!= shape.length=" + nelems);
		}
		this.sdata = sdata;
	  }

	  /// <summary>
	  /// Set one of the StructureData of this ArrayStructure.
	  /// </summary>
	  /// <param name="sd"> set it to this StructureData. </param>
	  /// <param name="index"> which one to set, as an index into 1D backing store. </param>
	  public virtual void setStructureData(StructureData sd, int index)
	  {
		sdata[index] = sd;
	  }

	  protected internal override StructureData makeStructureData(ArrayStructure @as, int index)
	  {
		return new StructureDataW(@as.StructureMembers);
	  }

	  public override NcArray getArray(int recnum, StructureMembers.Member m)
	  {
		if (m.DataArray != null)
		{
		  return base.getArray(recnum, m);
		}
		StructureData sd = getStructureData(recnum);
		return sd.getArray(m.Name);
	  }

	  public override double getScalarDouble(int recnum, StructureMembers.Member m)
	  {
		if (m.DataArray != null)
		{
		  return base.getScalarDouble(recnum, m);
		}
		StructureData sd = getStructureData(recnum);
		return sd.getScalarDouble(m.Name);
	  }

	  public override double[] getJavaArrayDouble(int recnum, StructureMembers.Member m)
	  {
		if (m.DataArray != null)
		{
		  return base.getJavaArrayDouble(recnum, m);
		}
		StructureData sd = getStructureData(recnum);
		return sd.getJavaArrayDouble(m.Name);
	  }

	  public override float getScalarFloat(int recnum, StructureMembers.Member m)
	  {
		if (m.DataArray != null)
		{
		  return base.getScalarFloat(recnum, m);
		}
		StructureData sd = getStructureData(recnum);
		return sd.getScalarFloat(m.Name);
	  }

	  public override float[] getJavaArrayFloat(int recnum, StructureMembers.Member m)
	  {
		if (m.DataArray != null)
		{
		  return base.getJavaArrayFloat(recnum, m);
		}
		StructureData sd = getStructureData(recnum);
		return sd.getJavaArrayFloat(m.Name);
	  }

	  public override byte getScalarByte(int recnum, StructureMembers.Member m)
	  {
		if (m.DataArray != null)
		{
		  return base.getScalarByte(recnum, m);
		}
		StructureData sd = getStructureData(recnum);
		return sd.getScalarByte(m.Name);
	  }

	  public override byte[] getJavaArrayByte(int recnum, StructureMembers.Member m)
	  {
		if (m.DataArray != null)
		{
		  return base.getJavaArrayByte(recnum, m);
		}
		StructureData sd = getStructureData(recnum);
		return sd.getJavaArrayByte(m.Name);
	  }

	  public override short getScalarShort(int recnum, StructureMembers.Member m)
	  {
		if (m.DataArray != null)
		{
		  return base.getScalarShort(recnum, m);
		}
		StructureData sd = getStructureData(recnum);
		return sd.getScalarShort(m.Name);
	  }

	  public override short[] getJavaArrayShort(int recnum, StructureMembers.Member m)
	  {
		if (m.DataArray != null)
		{
		  return base.getJavaArrayShort(recnum, m);
		}
		StructureData sd = getStructureData(recnum);
		return sd.getJavaArrayShort(m.Name);
	  }

	  public override int getScalarInt(int recnum, StructureMembers.Member m)
	  {
		if (m.DataArray != null)
		{
		  return base.getScalarInt(recnum, m);
		}
		StructureData sd = getStructureData(recnum);
		return sd.getScalarInt(m.Name);
	  }

	  public override int[] getJavaArrayInt(int recnum, StructureMembers.Member m)
	  {
		if (m.DataArray != null)
		{
		  return base.getJavaArrayInt(recnum, m);
		}
		StructureData sd = getStructureData(recnum);
		return sd.getJavaArrayInt(m.Name);
	  }

	  public override long getScalarLong(int recnum, StructureMembers.Member m)
	  {
		if (m.DataArray != null)
		{
		  return base.getScalarLong(recnum, m);
		}
		StructureData sd = getStructureData(recnum);
		return sd.getScalarLong(m.Name);
	  }

	  public override long[] getJavaArrayLong(int recnum, StructureMembers.Member m)
	  {
		if (m.DataArray != null)
		{
		  return base.getJavaArrayLong(recnum, m);
		}
		StructureData sd = getStructureData(recnum);
		return sd.getJavaArrayLong(m.Name);
	  }

	  public override char getScalarChar(int recnum, StructureMembers.Member m)
	  {
		if (m.DataArray != null)
		{
		  return base.getScalarChar(recnum, m);
		}
		StructureData sd = getStructureData(recnum);
		return sd.getScalarChar(m.Name);
	  }

	  public override char[] getJavaArrayChar(int recnum, StructureMembers.Member m)
	  {
		if (m.DataArray != null)
		{
		  return base.getJavaArrayChar(recnum, m);
		}
		StructureData sd = getStructureData(recnum);
		return sd.getJavaArrayChar(m.Name);
	  }

	  public override string getScalarString(int recnum, StructureMembers.Member m)
	  {
		if (m.DataArray != null)
		{
		  return base.getScalarString(recnum, m);
		}
		StructureData sd = getStructureData(recnum);
		return sd.getScalarString(m.Name);
	  }

	  public override string[] getJavaArrayString(int recnum, StructureMembers.Member m)
	  {
		if (m.DataArray != null)
		{
		  return base.getJavaArrayString(recnum, m);
		}
		StructureData sd = getStructureData(recnum);
		return sd.getJavaArrayString(m.Name);
	  }

	  public override StructureData getScalarStructure(int recnum, StructureMembers.Member m)
	  {
		if (m.DataArray != null)
		{
		  return base.getScalarStructure(recnum, m);
		}
		StructureData sd = getStructureData(recnum);
		return sd.getScalarStructure(m.Name);
	  }

	  public override ArrayStructure getArrayStructure(int recnum, StructureMembers.Member m)
	  {
		if (m.DataArray != null)
		{
		  return base.getArrayStructure(recnum, m);
		}
		StructureData sd = getStructureData(recnum);
		return sd.getArrayStructure(m.Name);
	  }

	  public override ArraySequence getArraySequence(int recnum, StructureMembers.Member m)
	  {
		if (m.DataArray != null)
		{
		  return base.getArraySequence(recnum, m);
		}
		StructureData sd = getStructureData(recnum);
		return sd.getArraySequence(m.Name);
	  }
	}

}
