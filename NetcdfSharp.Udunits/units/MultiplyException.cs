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
	/// Provides support for unit multiplication failures.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	[Serializable]
	public sealed class MultiplyException : OperationException
	{
	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// Constructs from a unit that can't be multiplied.
	  /// </summary>
	  /// <param name="unit">
	  ///        The unit that can't be multiplied. </param>
	  public MultiplyException(in Unit unit) : base("Can't multiply unit \"" + unit + '"')
	  {
	  }

	  /// <summary>
	  /// Constructs from two units.
	  /// </summary>
	  /// <param name="A">
	  ///        A unit attempting to be multiplied. </param>
	  /// <param name="B">
	  ///        The other unit attempting to be multiplied. </param>
	  public MultiplyException(in Unit A, in Unit B) : base("Can't multiply unit \"" + A + "\" by unit \"" + B + '"')
	  {
	  }

	  /// <summary>
	  /// Constructs from a scale factor and a unit.
	  /// </summary>
	  /// <param name="scale">
	  ///        The scale factor. </param>
	  /// <param name="unit">
	  ///        The unit. </param>
	  public MultiplyException(in double scale, in Unit unit) : base("Can't multiply unit \"" + unit + "\" by " + scale)
	  {
	  }
	}

}
