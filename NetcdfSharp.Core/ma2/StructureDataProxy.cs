// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.ma2
{
	/// <summary>
	/// Proxy for another StructureData.
	/// Does nothing, is intended to be overridden.
	/// 
	/// @author caron
	/// @since 8/20/13
	/// </summary>
	public class StructureDataProxy : StructureData
	{
	  protected internal StructureData org;

	  protected internal StructureDataProxy(StructureData org) : base(org.StructureMembers)
	  {
		this.org = org;
	  }

	  public StructureDataProxy(StructureMembers members, StructureData org) : base(members)
	  {
		this.org = org;
	  }

	  public virtual StructureData OriginalStructureData
	  {
		  get
		  {
			return org;
		  }
	  }

	  public override NcArray getArray(StructureMembers.Member m)
	  {
		return org.getArray(m.Name);
	  }

	  public override float convertScalarFloat(StructureMembers.Member m)
	  {
		return org.convertScalarFloat(m.Name);
	  }

	  public override double convertScalarDouble(StructureMembers.Member m)
	  {
		return org.convertScalarDouble(m.Name);
	  }

	  public override int convertScalarInt(StructureMembers.Member m)
	  {
		return org.convertScalarInt(m.Name);
	  }

	  public override long convertScalarLong(StructureMembers.Member m)
	  {
		return org.convertScalarLong(m.Name);
	  }

	  public override double getScalarDouble(StructureMembers.Member m)
	  {
		return org.getScalarDouble(m.Name);
	  }

	  public override double[] getJavaArrayDouble(StructureMembers.Member m)
	  {
		return org.getJavaArrayDouble(m.Name);
	  }

	  public override float getScalarFloat(StructureMembers.Member m)
	  {
		return org.getScalarFloat(m.Name);
	  }

	  public override float[] getJavaArrayFloat(StructureMembers.Member m)
	  {
		return org.getJavaArrayFloat(m.Name);
	  }

	  public override byte getScalarByte(StructureMembers.Member m)
	  {
		return org.getScalarByte(m.Name);
	  }

	  public override byte[] getJavaArrayByte(StructureMembers.Member m)
	  {
		return org.getJavaArrayByte(m.Name);
	  }

	  public override int getScalarInt(StructureMembers.Member m)
	  {
		return org.getScalarInt(m.Name);
	  }

	  public override int[] getJavaArrayInt(StructureMembers.Member m)
	  {
		return org.getJavaArrayInt(m.Name);
	  }

	  public override short getScalarShort(StructureMembers.Member m)
	  {
		return org.getScalarShort(m.Name);
	  }

	  public override short[] getJavaArrayShort(StructureMembers.Member m)
	  {
		return org.getJavaArrayShort(m.Name);
	  }

	  public override long getScalarLong(StructureMembers.Member m)
	  {
		return org.getScalarLong(m.Name);
	  }

	  public override long[] getJavaArrayLong(StructureMembers.Member m)
	  {
		return org.getJavaArrayLong(m.Name);
	  }

	  public override char getScalarChar(StructureMembers.Member m)
	  {
		return org.getScalarChar(m.Name);
	  }

	  public override char[] getJavaArrayChar(StructureMembers.Member m)
	  {
		return org.getJavaArrayChar(m.Name);
	  }

	  public override string getScalarString(StructureMembers.Member m)
	  {
		return org.getScalarString(m.Name);
	  }

	  public override string[] getJavaArrayString(StructureMembers.Member m)
	  {
		return org.getJavaArrayString(m.Name);
	  }

	  public override StructureData getScalarStructure(StructureMembers.Member m)
	  {
		return org.getScalarStructure(m.Name);
	  }

	  public override ArrayStructure getArrayStructure(StructureMembers.Member m)
	  {
		return org.getArrayStructure(m.Name);
	  }

	  public override ArraySequence getArraySequence(StructureMembers.Member m)
	  {
		return org.getArraySequence(m.Name);
	  }

	  public override object getScalarObject(StructureMembers.Member m)
	  {
		return org.getScalarObject(m.Name);
	  }
	}

}
