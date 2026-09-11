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
	/// Provides support for symbols for units.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	[Serializable]
	public sealed class UnitSymbol : UnitID
	{
	  private const long serialVersionUID = 1L;
	  /// <summary>
	  /// The symbol for the unit.
	  /// 
	  /// @serial
	  /// </summary>
	  private readonly string symbol;

	  /// <summary>
	  /// Constructs from a symbol.
	  /// </summary>
	  /// <param name="symbol">
	  ///        The symbol for the unit. Shall not be <code>
	  ///        null</code>. </param>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public UnitSymbol(final String symbol) throws NameException
	  public UnitSymbol(in string symbol)
	  {
		if (string.ReferenceEquals(symbol, null))
		{
		  throw new NameException("Symbol can't be null");
		}
		this.symbol = symbol;
	  }

	  /// <summary>
	  /// Returns the name of the unit. Always returns <code>null</code>.
	  /// </summary>
	  /// <returns> <code>null</code>. </returns>
	  public override string Name
	  {
		  get
		  {
			return null;
		  }
	  }

	  /// <summary>
	  /// Returns the plural form of the name of the unit. Always returns
	  /// <code>null</code>.
	  /// </summary>
	  /// <returns> <code>null</code>. </returns>
	  public override string Plural
	  {
		  get
		  {
			return null;
		  }
	  }

	  /// <summary>
	  /// Returns the symbol for the unit.
	  /// </summary>
	  /// <returns> The symbol for the unit. Never <code>null
	  ///        </code>. </returns>
	  public override string Symbol
	  {
		  get
		  {
			return symbol;
		  }
	  }

	  /// <summary>
	  /// Returns the string representation of this identifier.
	  /// </summary>
	  /// <returns> The string representation of this identifier. </returns>
	  public override string ToString()
	  {
		return Symbol;
	  }
	}

}
