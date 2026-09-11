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
	/// Provides support for errors in unit string specifications.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	[Serializable]
	public class SpecificationException : UnitException
	{
	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// Constructs from an error message.
	  /// </summary>
	  /// <param name="reason">
	  ///        The error message. </param>
	  public SpecificationException(in string reason) : base("Specification error: " + reason)
	  {
	  }

	  /// <summary>
	  /// Constructs from the string to be parsed and an error message.
	  /// </summary>
	  /// <param name="spec">
	  ///        The string to be parsed. </param>
	  /// <param name="reason">
	  ///        The error message. </param>
	  public SpecificationException(in string spec, in string reason) : base("Specification error in \"" + spec + "\": " + reason)
	  {
	  }

	  public SpecificationException(in string message, in Exception e) : base(message, e)
	  {
	  }
	}

}
