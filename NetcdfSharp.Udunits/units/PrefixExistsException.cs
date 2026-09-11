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
	/// Provides support for prefix database failures.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	[Serializable]
	public class PrefixExistsException : PrefixDBException
	{
	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// Constructs from an old prefix and a new prefix.
	  /// </summary>
	  /// <param name="oldPrefix">
	  ///        The previously-existing prefix. </param>
	  /// <param name="newPrefix">
	  ///        The replacement prefix. </param>
	  public PrefixExistsException(in Prefix oldPrefix, in Prefix newPrefix) : base("Attempt to replace \"" + oldPrefix + "\" with \"" + newPrefix + "\" in prefix database")
	  {
	  }
	}

}
