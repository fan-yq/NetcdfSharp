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
	/// Provides support for prefix database access failures. This is not used in the
	/// default implementation but could be used for remote database implementations,
	/// for example.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	[Serializable]
	public class PrefixDBAccessException : PrefixDBException
	{
	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// Constructs from an error message.
	  /// </summary>
	  /// <param name="reason">
	  ///        The reason the database couldn't be accessed. </param>
	  public PrefixDBAccessException(in string reason) : base("Couldn't access unit-prefix database: " + reason)
	  {
	  }
	}

}
