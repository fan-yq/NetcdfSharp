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
	/// Provides support for a Unit that is an instance of an unknown class.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	[Serializable]
	public sealed class UnitClassException : UnitFormatException
	{
	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// Constructs from an error message.
	  /// </summary>
	  /// <param name="msg">
	  ///        The error message. </param>
	  private UnitClassException(in string msg) : base(msg)
	  {
	  }

	  /// <summary>
	  /// Constructs from the unit that's an instance of an unknown class.
	  /// </summary>
	  /// <param name="unit">
	  ///        The unknown unit. </param>
	  public UnitClassException(in Unit unit) : this("\"" + unit.GetType().FullName + "\" is an unknown unit class")
	  {
// JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  }
	}

}
