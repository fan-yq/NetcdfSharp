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
	/// Provides support for general failures with unit format classes.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	[Serializable]
	public class UnitFormatException : UnitException
	{
	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// Constructs from nothing.
	  /// </summary>
	  public UnitFormatException()
	  {
	  }

	  /// <summary>
	  /// Constructs from an error message.
	  /// </summary>
	  /// <param name="message">
	  ///        The error message. </param>
	  public UnitFormatException(in string message) : base(message)
	  {
	  }
	}

}
