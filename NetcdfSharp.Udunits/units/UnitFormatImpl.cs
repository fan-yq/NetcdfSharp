// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System.Text;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.units
{

	/// <summary>
	/// Provides support for classes that parse and format unit specifications.
	/// 
	/// Instances are thread-safe.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	public abstract class UnitFormatImpl : UnitFormat
	{
		public abstract StringBuilder longFormat(Unit unit, StringBuilder buffer);
		public abstract StringBuilder format(Unit unit, StringBuilder buffer);
		public abstract StringBuilder format(Factor factor, StringBuilder buffer);
		public abstract Unit parse(string spec, UnitDB unitDB);

	  private const long serialVersionUID = 1L;
	  private static readonly object MUTEX = new object();

	  /// <summary>
	  /// Parses a unit specification. This method is thread-safe.
	  /// </summary>
	  /// <param name="spec">
	  ///        The unit specification (e.g. "m/s"); </param>
	  /// <returns> The unit corresponding to the specification. </returns>
	  /// <exception cref="NoSuchUnitException">
	  ///         A unit in the specification couldn't be found (e.g. the "m"
	  ///         in the example). </exception>
	  /// <exception cref="UnitParseException">
	  ///         The specification is grammatically incorrect. </exception>
	  /// <exception cref="SpecificationException">
	  ///         The specification is incorrect somehow. </exception>
	  /// <exception cref="UnitDBException">
	  ///         Problem with the unit database. </exception>
	  /// <exception cref="PrefixDBException">
	  ///         Problem with the unit-prefix database. </exception>
	  /// <exception cref="UnitSystemException">
	  ///         Problem with the system of units. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final Unit parse(final String spec) throws NoSuchUnitException, UnitParseException, SpecificationException, UnitDBException, PrefixDBException, UnitSystemException
	  public Unit parse(string spec)
	  {
		lock (MUTEX)
		{
		  return parse(spec, UnitDBManager.instance());
		}
	  }

	  /// <summary>
	  /// Formats a Factor (a base unit/exponent pair).
	  /// </summary>
	  /// <param name="factor">
	  ///        The base unit/exponent pair. </param>
	  /// <returns> The formatted factor. </returns>
	  public string format(Factor factor)
	  {
		return format(factor, new StringBuilder(8)).ToString();
	  }

	  /// <summary>
	  /// Formats a unit. If the unit has a symbol or name, then one of them will
	  /// be used; otherwise, a specification of the unit in terms of underlying
	  /// units will be returned.
	  /// </summary>
	  /// <param name="unit">
	  ///        The unit. </param>
	  /// <returns> The formatted unit. </returns>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final String format(final Unit unit) throws UnitClassException
	  public string format(Unit unit)
	  {
		return format(unit, new StringBuilder(80)).ToString();
	  }

	  /// <summary>
	  /// Formats a unit using a long form. This always returns a specification for
	  /// the unit in terms of underlying units: it doesn't return the name or
	  /// symbol of the unit unless the unit is a base unit.
	  /// </summary>
	  /// <param name="unit">
	  ///        The unit. </param>
	  /// <returns> The formatted unit. </returns>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final String longFormat(final Unit unit) throws UnitClassException
	  public string longFormat(Unit unit)
	  {
		return longFormat(unit, new StringBuilder(80)).ToString();
	  }
	}

}
