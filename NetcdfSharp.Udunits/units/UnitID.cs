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
	/// Provides support for unit identifiers.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	[Serializable]
	public abstract class UnitID
	{
	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// Factory method for constructing an identifier from a name, plural, and
	  /// symbol.
	  /// </summary>
	  /// <param name="name">
	  ///        The name for the unit. May be <code>null
	  ///        </code>. </param>
	  /// <param name="plural">
	  ///        The plural form of the name. If <code>null
	  ///        </code> and
	  ///        <code>name</code> is non-<code>
	  ///        null</code>, then regular
	  ///        plural-forming rules are used on the name. </param>
	  /// <param name="symbol">
	  ///        The symbol for the unit. May be <code>null
	  ///        </code>. </param>
	  public static UnitID newUnitID(in string name, in string plural, in string symbol)
	  {
		UnitID id;
		try
		{
		  id = string.ReferenceEquals(name, null) ? (NetcdfSharp.ucar.units.UnitID)new UnitSymbol(symbol) : UnitName.newUnitName(name, plural, symbol);
		}
// JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
// ORIGINAL LINE: catch (final NameException e)
		catch (NameException)
		{
		  id = null; // can't happen
		}
		return id;
	  }

	  /// <summary>
	  /// Returns the name of the unit.
	  /// </summary>
	  /// <returns> The name of the unit. May be <code>null</code>. </returns>
	  public abstract string Name {get;}

	  /// <summary>
	  /// Returns the plural form of the name of the unit.
	  /// </summary>
	  /// <returns> The plural form of the name of the unit. May be <code>null</code>
	  ///         . </returns>
	  public abstract string Plural {get;}

	  /// <summary>
	  /// Returns the symbol for the unit.
	  /// </summary>
	  /// <returns> The symbol for the unit. May be <code>null</code>. </returns>
	  public abstract string Symbol {get;}

	  /// <summary>
	  /// Returns the string representation of this identifier.
	  /// </summary>
	  /// <returns> The string representation of this identifier. </returns>
	  public override abstract string ToString();
	}

}
