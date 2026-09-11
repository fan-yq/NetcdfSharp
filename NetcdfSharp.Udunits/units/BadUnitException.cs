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
	/// Provides support for failures due to attempts to redefine an existing unit in
	/// a unit database.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	[Serializable]
	public sealed class BadUnitException : UnitDBException
	{
	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// Constructs from an error message.
	  /// </summary>
	  /// <param name="msg">
	  ///        The error message. </param>
	  public BadUnitException(in string msg) : base(msg)
	  {
	  }
	}

}
