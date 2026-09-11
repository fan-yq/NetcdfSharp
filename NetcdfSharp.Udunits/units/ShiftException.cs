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
	public sealed class ShiftException : OperationException
	{
	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// Constructs from the unit and the origin.
	  /// </summary>
	  /// <param name="unit">
	  ///        The unit. </param>
	  /// <param name="origin">
	  ///        The desired origin. </param>
	  public ShiftException(in Unit unit, in double origin) : base("Can't shift origin of unit \"" + unit + "\" to " + origin)
	  {
	  }

	  /// <summary>
	  /// Constructs from the unit and the origin.
	  /// </summary>
	  /// <param name="unit">
	  ///        The unit. </param>
	  /// <param name="origin">
	  ///        The desired origin. </param>
	  public ShiftException(in Unit unit, in DateTime origin) : base("Can't shift origin of unit \"" + unit + "\" to " + origin)
	  {
	  }
	}

}
