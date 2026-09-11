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
	/// Provides support for general failures of this package.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	[Serializable]
	public abstract class UnitException : Exception
	{
	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// Constructs from nothing.
	  /// </summary>
	  public UnitException()
	  {
	  }

	  /// <summary>
	  /// Constructs from an error message.
	  /// </summary>
	  /// <param name="message">
	  ///        The error message. </param>
	  public UnitException(in string message) : base(message)
	  {
	  }

	  /// <summary>
	  /// Constructs from a message and the exception that caused the failure.
	  /// </summary>
	  /// <param name="message">
	  ///        The message. </param>
	  /// <param name="e">
	  ///        The exception that caused the failure. </param>
	  public UnitException(in string message, in Exception e) : base(message, e)
	  {
	  }
	}

}
