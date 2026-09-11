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
	/// Interface for classes that parse and format unit specifications.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	public interface UnitFormat
	{
	  /// <summary>
	  /// Parses a unit specification.
	  /// </summary>
	  /// <param name="spec"> The unit specification (e.g. "m/s"); </param>
	  /// <returns> The unit corresponding to the specification. </returns>
	  /// <exception cref="NoSuchUnitException"> A unit in the specification couldn't be
	  ///         found (e.g. the "m" in the example). </exception>
	  /// <exception cref="UnitParseException"> The specification is grammatically
	  ///         incorrect. </exception>
	  /// <exception cref="SpecificationException"> The specification is incorrect somehow. </exception>
	  /// <exception cref="UnitDBException"> Problem with the unit database. </exception>
	  /// <exception cref="PrefixDBException"> Problem with the unit-prefix database. </exception>
	  /// <exception cref="UnitSystemException"> Problem with the system of units. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: Unit parse(String spec) throws NoSuchUnitException, UnitParseException, SpecificationException, UnitDBException, PrefixDBException, UnitSystemException;
	  Unit parse(string spec);

	  /// <summary>
	  /// Parses a unit specification.
	  /// </summary>
	  /// <param name="spec"> The unit specification (e.g. "m/s"); </param>
	  /// <param name="unitDB"> The unit database. </param>
	  /// <returns> The unit corresponding to the specification. </returns>
	  /// <exception cref="NoSuchUnitException"> A unit in the specification couldn't be
	  ///         found (e.g. the "m" in the example). </exception>
	  /// <exception cref="UnitParseException"> The specification is grammatically
	  ///         incorrect. </exception>
	  /// <exception cref="SpecificationException"> The specification is incorrect somehow. </exception>
	  /// <exception cref="UnitDBException"> Problem with the unit database. </exception>
	  /// <exception cref="PrefixDBException"> Problem with the unit-prefix database. </exception>
	  /// <exception cref="UnitSystemException"> Problem with the system of units. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: Unit parse(String spec, UnitDB unitDB) throws NoSuchUnitException, UnitParseException, SpecificationException, UnitDBException, PrefixDBException, UnitSystemException;
	  Unit parse(string spec, UnitDB unitDB);

	  /// <summary>
	  /// Formats a Factor (a base unit/exponent pair).
	  /// </summary>
	  /// <param name="factor"> The base unit/exponent pair. </param>
	  /// <returns> The formatted factor. </returns>
	  string format(Factor factor);

	  /// <summary>
	  /// Appends a formatted factor to a string buffer. A factor is a
	  /// base unit/exponent pair).
	  /// </summary>
	  /// <param name="factor"> The base unit/exponent pair. </param>
	  /// <param name="buffer"> The string buffer to be appended to. </param>
	  /// <returns> The string buffer. </returns>
	  StringBuilder format(Factor factor, StringBuilder buffer);

	  /// <summary>
	  /// Formats a unit. If the unit has a symbol or name, then one of them
	  /// will be used; otherwise, a specification of the unit in terms of
	  /// underlying units will be returned.
	  /// </summary>
	  /// <param name="unit"> The unit. </param>
	  /// <returns> The formatted unit. </returns>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: String format(Unit unit) throws UnitClassException;
	  string format(Unit unit);

	  /// <summary>
	  /// Formats a unit using a long form. This always returns a specification
	  /// for the unit in terms of underlying units: it doesn't return the name
	  /// or symbol of the unit unless the unit is a base unit.
	  /// </summary>
	  /// <param name="unit"> The unit. </param>
	  /// <returns> The formatted unit. </returns>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: String longFormat(Unit unit) throws UnitClassException;
	  string longFormat(Unit unit);

	  /// <summary>
	  /// Appends a formatted unit to a string buffer. This is similar to
	  /// <code>format(Unit)</code> but it appends the specification to a
	  /// string buffer.
	  /// </summary>
	  /// <param name="unit"> The unit. </param>
	  /// <param name="buffer"> The string buffer to be appended to. </param>
	  /// <returns> The string buffer. </returns>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: StringBuffer format(Unit unit, StringBuffer buffer) throws UnitClassException;
	  StringBuilder format(Unit unit, StringBuilder buffer);

	  /// <summary>
	  /// Appends a unit formatted according to the long form to a string buffer.
	  /// This is similar to <code>longFormat(Unit)</code> but it appends the
	  /// specification to a string buffer.
	  /// </summary>
	  /// <param name="unit"> The unit. </param>
	  /// <param name="buffer"> The string buffer to be appended to. </param>
	  /// <returns> The string buffer. </returns>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: StringBuffer longFormat(Unit unit, StringBuffer buffer) throws UnitClassException;
	  StringBuilder longFormat(Unit unit, StringBuilder buffer);
	}

}
