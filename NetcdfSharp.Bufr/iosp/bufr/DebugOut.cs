// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.iosp.bufr
{
	using Indent = NetcdfSharp.ucar.nc2.util.Indent;

	/// <summary>
	/// Helper class for debugging BUFR descriptors
	/// 
	/// @author caron
	/// @since Nov 16, 2009
	/// </summary>
	internal class DebugOut
	{
	  internal Formatter f;
// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
	  internal Indent indent_Conflict;
	  internal int fldno; // track fldno to compare with EU output

	  internal DebugOut(Formatter f)
	  {
		this.f = f;
		this.indent_Conflict = new Indent(2);
		this.indent_Conflict.IndentLevel = 0;
		this.fldno = 1;
	  }

	  internal virtual string indent()
	  {
		return indent_Conflict.ToString();
	  }

	}


}
