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
	/// Provides support for errors in unit specifications.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	[Serializable]
	public class UnitParseException : SpecificationException
	{
	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// Constructs from a reason.
	  /// </summary>
	  /// <param name="reason">
	  ///        The reason for the failure. </param>
	  public UnitParseException(in string reason) : base("Parse error: " + reason)
	  {
	  }

	  /// <summary>
	  /// Constructs from the string to be parsed and a reason.
	  /// </summary>
	  /// <param name="spec">
	  ///        The string to be parsed. </param>
	  /// <param name="reason">
	  ///        The reason for the failure. </param>
	  public UnitParseException(in string spec, in string reason) : base("Couldn't parse \"" + spec + "\": " + reason)
	  {
	  }

	  public UnitParseException(in string message, in Exception e) : base(message, e)
	  {
	  }
	}

}
