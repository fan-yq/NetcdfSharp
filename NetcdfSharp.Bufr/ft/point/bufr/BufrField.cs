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

namespace NetcdfSharp.ucar.nc2.ft.point.bufr
{

	/// <summary>
	/// Abstraction for BUFR field.
	/// Used in writing index, so we can make changes in BufrCdmIndexPanel
	/// 
	/// @author caron
	/// @since 8/20/13
	/// </summary>
	public interface BufrField
	{
	  string Name {get;}

	  string Desc {get;}

	  string Units {get;}

	  short Fxy {get;}

	  string FxyName {get;}

	  BufrCdmIndexProto.FldAction Action {get;}

	  BufrCdmIndexProto.FldType Type {get;}

	  bool Seq {get;}

	  int Min {get;}

	  int Max {get;}

	  int Scale {get;}

	  int Reference {get;}

	  int BitWidth {get;}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: NetcdfSharp.java.util.List<? extends BufrField> getChildren();
	  IList<BufrField> Children {get;}

	}

}
