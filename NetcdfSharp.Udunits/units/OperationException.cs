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
	/// Provides support for unit operation failures (ex: multiplication).
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	[Serializable]
	public abstract class OperationException : UnitException
	{
	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// Constructs from nothing.
	  /// </summary>
	  public OperationException()
	  {
	  }

	  /// <summary>
	  /// Constructs from an error message.
	  /// </summary>
	  /// <param name="message">
	  ///        The error message. </param>
	  protected internal OperationException(in string message) : base(message)
	  {
	  }

	  /// <summary>
	  /// Constructs from an error message and the exception that caused the
	  /// problem.
	  /// </summary>
	  /// <param name="message">
	  ///        The error message. </param>
	  /// <param name="e">
	  ///        The exception that caused the problem. </param>
	  protected internal OperationException(in string message, in Exception e) : base(message, e)
	  {
	  }
	}

}
