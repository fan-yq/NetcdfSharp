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
	/// Provides support for the general class of prefix database failures.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	[Serializable]
	public class PrefixDBException : UnitException
	{
	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// Constructs from nothing.
	  /// </summary>
	  public PrefixDBException() : base("Prefix database exception")
	  {
	  }

	  /// <summary>
	  /// Constructs from an error message.
	  /// </summary>
	  /// <param name="message">
	  ///        The error message. </param>
	  public PrefixDBException(in string message) : base("Prefix database exception: " + message)
	  {
	  }

	  /// <summary>
	  /// Constructs from the exception that caused this exception to be thrown.
	  /// </summary>
	  /// <param name="e">
	  ///        The exception that caused this exception to be thrown. </param>
	  public PrefixDBException(in Exception e) : this("Prefix database exception", e)
	  {
	  }

	  /// <summary>
	  /// Constructs from an error message and the exception that caused this
	  /// exception to be thrown.
	  /// </summary>
	  /// <param name="message">
	  ///        The error message. </param>
	  /// <param name="e">
	  ///        The exception that caused this exception to be thrown. </param>
	  public PrefixDBException(in string message, in Exception e) : base(message, e)
	  {
	  }
	}

}
