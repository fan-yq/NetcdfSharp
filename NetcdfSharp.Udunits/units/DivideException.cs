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
	/// Provides support for unit division failures.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	[Serializable]
	public sealed class DivideException : OperationException
	{
	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// Constructs from a unit that can't be divided.
	  /// </summary>
	  /// <param name="unit">
	  ///        The unit that can't be divided. </param>
	  public DivideException(in Unit unit) : base("Can't divide unit \"" + unit + "\"")
	  {
	  }

	  /// <summary>
	  /// Constructs from dividend and divisor units.
	  /// </summary>
	  /// <param name="numerator">
	  ///        The unit attempting to be divided. </param>
	  /// <param name="denominator">
	  ///        The unit attempting to divide. </param>
	  public DivideException(in Unit numerator, in Unit denominator) : base("Can't divide unit \"" + numerator + "\" by unit \"" + denominator + "\"")
	  {
	  }
	}

}
