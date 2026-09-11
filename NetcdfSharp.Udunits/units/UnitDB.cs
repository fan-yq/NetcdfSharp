// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System.Collections.Generic;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.units
{

	/// <summary>
	/// Interface for a unit database.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	public interface UnitDB
	{
	  /// <summary>
	  /// Adds a unit to the database.
	  /// </summary>
	  /// <param name="unit">
	  ///        The unit to be added. Its getName() method must not return
	  ///        null. Its getPlural() and getSymbol() methods may return null. </param>
	  /// <exception cref="NameException">
	  ///         Bad unit name. </exception>
	  /// <exception cref="UnitExistsException">
	  ///         The unit is already in the database. </exception>
	  /// <exception cref="UnitDBAccessException">
	  ///         Problem accessing unit database. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: void addUnit(Unit unit) throws UnitExistsException, UnitDBAccessException, NameException;
	  void addUnit(Unit unit);

	  /// <summary>
	  /// Adds an alias for a unit to the database.
	  /// </summary>
	  /// <param name="alias">
	  ///        The alias for the unit. </param>
	  /// <param name="name">
	  ///        The name of the unit already in the database. </param>
	  /// <exception cref="NameException">
	  ///         Bad unit name. </exception>
	  /// <exception cref="UnitExistsException">
	  ///         The unit is already in the database. </exception>
	  /// <exception cref="UnitDBAccessException">
	  ///         Problem accessing unit database. </exception>
	  /// <exception cref="NoSuchUnitException">
	  ///         The unit doesn't exist in the database. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: void addAlias(String alias, String name) throws NoSuchUnitException, UnitExistsException, UnitDBAccessException, NameException;
	  void addAlias(string alias, string name);

	  /// <summary>
	  /// Adds an alias for a unit to the database.
	  /// </summary>
	  /// <param name="alias">
	  ///        The alias for the unit. </param>
	  /// <param name="name">
	  ///        The name of the unit already in the database. </param>
	  /// <param name="symbol">
	  ///        The symbol for the unit. </param>
	  /// <exception cref="NameException">
	  ///         Bad unit name. </exception>
	  /// <exception cref="UnitExistsException">
	  ///         The unit is already in the database. </exception>
	  /// <exception cref="UnitDBAccessException">
	  ///         Problem accessing unit database. </exception>
	  /// <exception cref="NoSuchUnitException">
	  ///         The unit doesn't exist in the database. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: void addAlias(String alias, String name, String symbol) throws NoSuchUnitException, UnitExistsException, UnitDBAccessException, NameException;
	  void addAlias(string alias, string name, string symbol);

	  /// <summary>
	  /// Adds an alias for a unit to the database.
	  /// </summary>
	  /// <param name="alias">
	  ///        The alias for the unit. </param>
	  /// <param name="name">
	  ///        The name of the unit already in the database. </param>
	  /// <param name="symbol">
	  ///        The symbol of the alias. May be <code>null
	  ///        </code>. </param>
	  /// <param name="plural">
	  ///        The plural form of the alias. May be <code>
	  ///        null</code> in which
	  ///        case regular plural- forming rules are followed. </param>
	  /// <exception cref="NameException">
	  ///         Bad unit name. </exception>
	  /// <exception cref="UnitExistsException">
	  ///         The unit is already in the database. </exception>
	  /// <exception cref="UnitDBAccessException">
	  ///         Problem accessing unit database. </exception>
	  /// <exception cref="NoSuchUnitException">
	  ///         The unit doesn't exist in the database. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: void addAlias(String alias, String name, String symbol, String plural) throws NoSuchUnitException, UnitExistsException, UnitDBAccessException, NameException;
	  void addAlias(string alias, string name, string symbol, string plural);

	  /// <summary>
	  /// Adds an alias for a unit to the database.
	  /// </summary>
	  /// <param name="id">
	  ///        The alias identifier. </param>
	  /// <param name="name">
	  ///        The name of the unit already in the database. </param>
	  /// <exception cref="NoSuchUnitException">
	  ///         The unit doesn't exist in the database. </exception>
	  /// <exception cref="UnitExistsException">
	  ///         The unit is already in the database. </exception>
	  /// <exception cref="UnitDBAccessException">
	  ///         Problem accessing unit database. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: void addAlias(UnitID id, String name) throws NoSuchUnitException, UnitExistsException, UnitDBAccessException;
	  void addAlias(UnitID id, string name);

	  /// <summary>
	  /// Adds a symbol for a unit to the database.
	  /// </summary>
	  /// <param name="symbol">
	  ///        The symbol for the unit. </param>
	  /// <param name="name">
	  ///        The name of the unit already in the database. </param>
	  /// <exception cref="NameException">
	  ///         Bad unit name. </exception>
	  /// <exception cref="UnitExistsException">
	  ///         The unit is already in the database. </exception>
	  /// <exception cref="UnitDBAccessException">
	  ///         Problem accessing unit database. </exception>
	  /// <exception cref="NoSuchUnitException">
	  ///         The unit doesn't exist in the database. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: void addSymbol(String symbol, String name) throws NoSuchUnitException, UnitExistsException, UnitDBAccessException, NameException;
	  void addSymbol(string symbol, string name);

	  /// <summary>
	  /// Gets the unit in the database whose name, plural, or symbol, match an
	  /// identifier. The order of matching is implementation defined.
	  /// </summary>
	  /// <param name="id">
	  ///        The identifier for the unit. </param>
	  /// <returns> The matching unit in the database or <code>
	  ///        null</code> if no
	  ///         matching unit could be found. </returns>
	  /// <exception cref="UnitDBAccessException">
	  ///         Problem accessing unit database. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: Unit get(String id) throws UnitDBAccessException;
	  Unit get(string id);

	  /// <summary>
	  /// Gets a unit in the database by name.
	  /// </summary>
	  /// <param name="name">
	  ///        The name of the unit. </param>
	  /// <returns> The matching unit in the database or <code>
	  ///        null</code> if no
	  ///         matching unit could be found. </returns>
	  /// <exception cref="UnitDBAccessException">
	  ///         Problem accessing unit database. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: Unit getByName(String name) throws UnitDBAccessException;
	  Unit getByName(string name);

	  /// <summary>
	  /// Gets a unit in the database by symbol.
	  /// </summary>
	  /// <param name="symbol">
	  ///        The symbol for the unit. </param>
	  /// <returns> The matching unit in the database or <code>
	  ///        null</code> if no
	  ///         matching unit could be found. </returns>
	  /// <exception cref="UnitDBAccessException">
	  ///         Problem accessing unit database. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: Unit getBySymbol(String symbol) throws UnitDBAccessException;
	  Unit getBySymbol(string symbol);

	  /// <summary>
	  /// Returns the string representation of the database.
	  /// </summary>
	  /// <returns> The string representation of the database. </returns>
	  string ToString();

	  /// <summary>
	  /// Returns an iterator over the units of the database.
	  /// </summary>
	  /// <returns> An iterator over the units of the database. The
	  ///         <code>next()</code> of the iterator returns objects of type
	  ///         <code>Unit</code>. </returns>
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: NetcdfSharp.java.util.Iterator<?> getIterator();
	  IEnumerator<Unit> Iterator {get;}
	}

}
