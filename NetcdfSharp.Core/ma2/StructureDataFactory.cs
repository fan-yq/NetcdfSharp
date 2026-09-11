// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System.Collections.Generic;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// Convenience routines for constructing one-off StructureData objects
	/// 
	/// @author caron
	/// @since Jan 19, 2009
	/// </summary>
	public class StructureDataFactory
	{

	  /// <summary>
	  /// Make a StructureData with a single member of given name and value. </summary>
	  public static StructureData make(string name, object value)
	  {
		StructureMembers.Builder builder = StructureMembers.builder();
		DataType dtype = DataTypes.getType(value.GetType(), false); // LOOK unsigned
		builder.addMember(name, null, null, dtype, new int[] {1});
		StructureMembers sm = builder.build();
		StructureDataW sw = new StructureDataW(sm);
		NcArray dataArray = NcArray.factory(dtype, new int[] {1});
		dataArray.setObject(dataArray.Index, value);
		sw.setMemberData(sm.findMember(name), dataArray);
		return sw;
	  }

	  public static StructureData make(params StructureData[] sdatas)
	  {
		if (sdatas.Length == 1)
		{
		  return sdatas[0];
		}

		// look for sole
		int count = 0;
		StructureData result = null;
		foreach (StructureData sdata in sdatas)
		{
		  if (sdata != null)
		  {
			count++;
			result = sdata;
		  }
		}
		if (count == 1)
		{
		  return result;
		}

		// combine
		return StructureDataComposite.create(sdatas);
	  }

	  public static StructureData make(IEnumerable<StructureData> sdatas)
	  {
		// look for singleton
		int count = 0;
		StructureData result = null;
		foreach (StructureData sdata in sdatas)
		{
		  if (sdata != null)
		  {
			count++;
			result = sdata;
		  }
		}
		if (count == 1)
		{
		  return result;
		}

		// if multiple, combine into StructureDataComposite
		return StructureDataComposite.create(sdatas);
	  }


	}

}
