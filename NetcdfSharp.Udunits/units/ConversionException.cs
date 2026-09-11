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
	/// Provides support for unit conversion exceptions.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	[Serializable]
	public sealed class ConversionException : UnitException
	{
	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// Constructs from nothing.
	  /// </summary>
	  public ConversionException()
	  {
	  }

	  /// <summary>
	  /// Constructs from a message.
	  /// </summary>
	  /// <param name="message">
	  ///        The error message. </param>
	  private ConversionException(in string message) : base(message)
	  {
	  }

	  /// <summary>
	  /// Constructs from a "from" unit and and "to" unit.
	  /// </summary>
	  /// <param name="fromUnit">
	  ///        The unit from which a conversion was attempted. </param>
	  /// <param name="toUnit">
	  ///        The unit to which a conversion was attempted. </param>
	  public ConversionException(in Unit fromUnit, in Unit toUnit) : this("Can't convert from unit \"" + fromUnit + "\" to unit \"" + toUnit + "\"")
	  {
	  }
	}

}
