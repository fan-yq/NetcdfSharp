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
	/// Provides support for attempting to redefine a base quantity in a database.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	[Serializable]
	public class QuantityExistsException : UnitException
	{
	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// Constructs from the base quantity being redefined.
	  /// </summary>
	  public QuantityExistsException(in BaseQuantity quantity) : this("Base quantity \"" + quantity + " already exists")
	  {
	  }

	  /// <summary>
	  /// Constructs from an error message.
	  /// </summary>
	  /// <param name="msg">
	  ///        The error message. </param>
	  private QuantityExistsException(in string msg) : base(msg)
	  {
	  }
	}

}
