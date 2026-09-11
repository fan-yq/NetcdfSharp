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
namespace NetcdfSharp.ucar.units
{
	/// <summary>
	/// Provides support for failure to raise a unit to a power.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	[Serializable]
	public sealed class RaiseException : OperationException
	{
	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// Constructs from the unit that couldn't be raised to a power.
	  /// </summary>
	  /// <param name="unit">
	  ///        The unit that couldn't be raised to a power. </param>
	  public RaiseException(in Unit unit) : base("Can't exponentiate unit \"" + unit + "\"")
	  {
	  }
	}

}
