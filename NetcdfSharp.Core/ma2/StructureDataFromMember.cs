// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2020 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.ma2
{
	/// <summary>
	/// A StructureData implementation that has its data self-contained.
	/// Created from the existing Data Array in each member.
	/// 
	/// @author caron
	/// </summary>
	public class StructureDataFromMember : StructureDataW
	{

	  public StructureDataFromMember(StructureMembers sm) : base(sm)
	  {
		foreach (StructureMembers.Member member in sm.Members)
		{
		  setMemberData(member, member.DataArray);
		}
	  }

	}


}
