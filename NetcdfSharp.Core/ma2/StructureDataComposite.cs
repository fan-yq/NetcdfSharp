// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// A composite of other StructureData.
	/// If multiple members of same name exist, the first one added is used
	/// TODO make Immutable
	/// </summary>
	public class StructureDataComposite : StructureData
	{
	  protected internal IDictionary<string, StructureData> proxy = new Dictionary<string, StructureData>(32);

	  public static StructureDataComposite create(IEnumerable<StructureData> sdatas)
	  {
		IDictionary<string, StructureData> proxy = new Dictionary<string, StructureData>(32);
		StructureMembers.Builder builder = StructureMembers.builder();
		foreach (StructureData sdata in sdatas)
		{
		  if (sdata != null)
		  {
			foreach (StructureMembers.Member m in sdata.Members)
			{
			  if (!builder.hasMember(m.Name))
			  {
				builder.addMember(m.toBuilder(true));
				proxy[m.Name] = sdata;
			  }
			}
		  }
		}

		return new StructureDataComposite(builder.build(), proxy);
	  }

	  private StructureDataComposite(StructureMembers smembers, IDictionary<string, StructureData> proxy) : base(smembers)
	  {
		this.proxy = proxy;
	  }

	  /// @deprecated use StructureDataComposite.make(StructureData... sdatas) 
	  [Obsolete("use StructureDataComposite.make(StructureData... sdatas)")]
	  public StructureDataComposite() : base(new StructureMembers(""))
	  {
	  }

	  /// @deprecated use StructureDataComposite.make(StructureData... sdatas) 
	  [Obsolete("use StructureDataComposite.make(StructureData... sdatas)")]
	  public virtual void add(StructureData sdata)
	  {
		foreach (StructureMembers.Member m in sdata.Members)
		{
		  if (this.members.findMember(m.Name) == null)
		  {
			this.members.addMember(m);
			proxy[m.Name] = sdata;
		  }
		}
	  }

	  /// @deprecated use StructureDataComposite.make(StructureData... sdatas) 
	  [Obsolete("use StructureDataComposite.make(StructureData... sdatas)")]
	  public virtual void add(int pos, StructureData sdata)
	  {
		foreach (StructureMembers.Member m in sdata.Members)
		{
		  if (this.members.findMember(m.Name) == null)
		  {
			this.members.addMember(pos++, m);
			proxy[m.Name] = sdata;
		  }
		}
	  }

	  public override NcArray getArray(StructureMembers.Member m)
	  {
		StructureData sdata = proxy[m.Name];
		return sdata.getArray(m.Name);
	  }

	  public override float convertScalarFloat(StructureMembers.Member m)
	  {
		return proxy[m.Name].convertScalarFloat(m.Name);
	  }

	  public override double convertScalarDouble(StructureMembers.Member m)
	  {
		return proxy[m.Name].convertScalarDouble(m.Name);
	  }

	  public override int convertScalarInt(StructureMembers.Member m)
	  {
		return proxy[m.Name].convertScalarInt(m.Name);
	  }

	  public override long convertScalarLong(StructureMembers.Member m)
	  {
		return proxy[m.Name].convertScalarLong(m.Name);
	  }

	  public override double getScalarDouble(StructureMembers.Member m)
	  {
		return proxy[m.Name].getScalarDouble(m.Name);
	  }

	  public override double[] getJavaArrayDouble(StructureMembers.Member m)
	  {
		return proxy[m.Name].getJavaArrayDouble(m.Name);
	  }

	  public override float getScalarFloat(StructureMembers.Member m)
	  {
		return proxy[m.Name].getScalarFloat(m.Name);
	  }

	  public override float[] getJavaArrayFloat(StructureMembers.Member m)
	  {
		return proxy[m.Name].getJavaArrayFloat(m.Name);
	  }

	  public override byte getScalarByte(StructureMembers.Member m)
	  {
		return proxy[m.Name].getScalarByte(m.Name);
	  }

	  public override byte[] getJavaArrayByte(StructureMembers.Member m)
	  {
		return proxy[m.Name].getJavaArrayByte(m.Name);
	  }

	  public override int getScalarInt(StructureMembers.Member m)
	  {
		return proxy[m.Name].getScalarInt(m.Name);
	  }

	  public override int[] getJavaArrayInt(StructureMembers.Member m)
	  {
		return proxy[m.Name].getJavaArrayInt(m.Name);
	  }

	  public override short getScalarShort(StructureMembers.Member m)
	  {
		return proxy[m.Name].getScalarShort(m.Name);
	  }

	  public override short[] getJavaArrayShort(StructureMembers.Member m)
	  {
		return proxy[m.Name].getJavaArrayShort(m.Name);
	  }

	  public override long getScalarLong(StructureMembers.Member m)
	  {
		return proxy[m.Name].getScalarLong(m.Name);
	  }

	  public override long[] getJavaArrayLong(StructureMembers.Member m)
	  {
		return proxy[m.Name].getJavaArrayLong(m.Name);
	  }

	  public override char getScalarChar(StructureMembers.Member m)
	  {
		return proxy[m.Name].getScalarChar(m.Name);
	  }

	  public override char[] getJavaArrayChar(StructureMembers.Member m)
	  {
		return proxy[m.Name].getJavaArrayChar(m.Name);
	  }

	  public override string getScalarString(StructureMembers.Member m)
	  {
		return proxy[m.Name].getScalarString(m.Name);
	  }

	  public override string[] getJavaArrayString(StructureMembers.Member m)
	  {
		return proxy[m.Name].getJavaArrayString(m.Name);
	  }

	  public override StructureData getScalarStructure(StructureMembers.Member m)
	  {
		return proxy[m.Name].getScalarStructure(m.Name);
	  }

	  public override ArrayStructure getArrayStructure(StructureMembers.Member m)
	  {
		return proxy[m.Name].getArrayStructure(m.Name);
	  }

	  public override ArraySequence getArraySequence(StructureMembers.Member m)
	  {
		return proxy[m.Name].getArraySequence(m.Name);
	  }

	  public override object getScalarObject(StructureMembers.Member m)
	  {
		return proxy[m.Name].getScalarObject(m.Name);
	  }

	}

}
