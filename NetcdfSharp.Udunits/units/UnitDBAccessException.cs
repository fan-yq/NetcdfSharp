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
	/// Provides support for failure to access unit database (e.g. RMI failure).
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	[Serializable]
	public class UnitDBAccessException : UnitDBException
	{
	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// Constructs from a string.
	  /// </summary>
	  /// <param name="reason">
	  ///        The reason for the failure. </param>
	  public UnitDBAccessException(in string reason) : base("Couldn't access unit database: " + reason)
	  {
	  }
	}

}
