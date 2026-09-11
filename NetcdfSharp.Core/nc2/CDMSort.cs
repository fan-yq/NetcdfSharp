// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2
{
	/// <summary>
	/// Enumeration of the kinds of nodes in a CDM model.
	/// 
	/// @link CDMNode.java
	/// 
	/// @author Dennis Heimbigner </summary>
	/// @deprecated Will move to Dap4 module in version 6. 
	[Obsolete("Will move to Dap4 module in version 6.")]
	public enum CDMSort
	{
	  ATTRIBUTE,
	  DIMENSION,
	  ENUMERATION,
	  VARIABLE, // Atomic
	  SEQUENCE,
	  STRUCTURE,
	  GROUP
	}

}
