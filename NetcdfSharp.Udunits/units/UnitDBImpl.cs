// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.units
{

	/// <summary>
	/// Provides most of a concrete implementation of a database of units.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	[Serializable]
	public class UnitDBImpl : UnitDB
	{
	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// The set of units.
	  /// 
	  /// @serial
	  /// </summary>
	  private readonly ISet<Unit> unitSet;

	  /// <summary>
	  /// The name-to-unit map.
	  /// 
	  /// @serial
	  /// </summary>
	  private readonly IDictionary<string, Unit> nameMap;

	  /// <summary>
	  /// The symbol-to-unit map.
	  /// 
	  /// @serial
	  /// </summary>
	  private readonly IDictionary<string, Unit> symbolMap;

	  /// <summary>
	  /// Constructs from the expected number of names and symbols. The sizes will
	  /// be used to construct the initial database but will not limit its growth.
	  /// </summary>
	  /// <param name="nameCount">
	  ///        The expected number of names (including plurals and aliases). </param>
	  /// <param name="symbolCount">
	  ///        The expected number of symbols. </param>
	  protected internal UnitDBImpl(in int nameCount, in int symbolCount)
	  {
		unitSet = new SortedSet<Unit>(Comparer<Unit>.Create((obj1, obj2) => string.CompareOrdinal(obj1.Name, obj2.Name)));
		nameMap = new Dictionary<string, Unit>(nameCount + 1);
		symbolMap = new Dictionary<string, Unit>(symbolCount + 1);
	  }

	  /// <summary>
	  /// Adds all the entries in another UnitDBImpl to this database.
	  /// </summary>
	  /// <param name="that">
	  ///        The other UnitDBImpl. </param>
	  /// <exception cref="UnitExistsException">
	  ///         Attempt to redefine an existing entry. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public void add(final UnitDBImpl that) throws UnitExistsException
	  public virtual void add(in UnitDBImpl that)
	  {
		unitSet.UnionWith(that.unitSet);
		foreach (var kv in that.nameMap)
		{
		  nameMap[kv.Key] = kv.Value;
		}
		foreach (var kv in that.symbolMap)
		{
		  symbolMap[kv.Key] = kv.Value;
		}
	  }

	  /// <summary>
	  /// Return the number of names in this database
	  /// </summary>
	  /// <returns> The total number of names, plurals, and aliases. </returns>
	  public virtual int nameCount()
	  {
		return nameMap.Count;
	  }

	  /// <summary>
	  /// Return the number of symbols in this database.
	  /// </summary>
	  /// <returns> The number of symbols in this database. </returns>
	  public virtual int symbolCount()
	  {
		return symbolMap.Count;
	  }

	  /// <summary>
	  /// Adds a unit to the database.
	  /// </summary>
	  /// <param name="unit">
	  ///        The unit to be added. </param>
	  /// <exception cref="UnitExistsException">
	  ///         Another unit with the same name or symbol already exists in
	  ///         the database. </exception>
	  /// <exception cref="NameException">
	  ///         Bad unit name. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public void addUnit(final Unit unit) throws UnitExistsException, NameException
	  public virtual void addUnit(Unit unit)
	  {
		if (string.ReferenceEquals(unit.Name, null))
		{
		  throw new NameException("Unit name can't be null");
		}
		addByName(unit.Name, unit);
		addByName(unit.Plural, unit);
		addBySymbol(unit.Symbol, unit);
		unitSet.Add(unit);
	  }

	  /// <summary>
	  /// Adds an alias for a unit already in the database.
	  /// </summary>
	  /// <param name="alias">
	  ///        An alias for the unit. </param>
	  /// <param name="name">
	  ///        The name of the unit already in the database. </param>
	  /// <exception cref="UnitExistsException">
	  ///         Another unit with the same name or symbol already exists in
	  ///         the database. </exception>
	  /// <exception cref="NoSuchUnitException">
	  ///         The unit isn't in the database. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void addAlias(final String alias, final String name) throws NoSuchUnitException, UnitExistsException
	  public void addAlias(string alias, string name)
	  {
		addAlias(alias, name, null);
	  }

	  /// <summary>
	  /// Adds an alias for a unit already in the database.
	  /// </summary>
	  /// <param name="alias">
	  ///        An alias for the unit. </param>
	  /// <param name="name">
	  ///        The name of the unit already in the database. </param>
	  /// <param name="symbol">
	  ///        The symbol for the unit. </param>
	  /// <exception cref="UnitExistsException">
	  ///         Another unit with the same name or symbol already exists in
	  ///         the database. </exception>
	  /// <exception cref="NoSuchUnitException">
	  ///         The unit isn't in the database. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void addAlias(final String alias, final String name, final String symbol) throws NoSuchUnitException, UnitExistsException
	  public void addAlias(string alias, string name, string symbol)
	  {
		addAlias(alias, name, symbol, null);
	  }

	  /// <summary>
	  /// Adds a symbol for a unit already in the database.
	  /// </summary>
	  /// <param name="symbol">
	  ///        The symbol for the unit. </param>
	  /// <param name="name">
	  ///        The name of the unit already in the database. </param>
	  /// <exception cref="UnitExistsException">
	  ///         Another unit with the same name or symbol already exists in
	  ///         the database. </exception>
	  /// <exception cref="NoSuchUnitException">
	  ///         The unit isn't in the database. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void addSymbol(final String symbol, final String name) throws NoSuchUnitException, UnitExistsException
	  public void addSymbol(string symbol, string name)
	  {
		addAlias(null, name, symbol, null);
	  }

	  /// <summary>
	  /// Adds an alias for a unit already in the database.
	  /// </summary>
	  /// <param name="alias">
	  ///        The alias to be added to the database. May be null. </param>
	  /// <param name="name">
	  ///        The name of the unit to have an alias added to the database. </param>
	  /// <param name="symbol">
	  ///        The symbol to be added. May be null. </param>
	  /// <param name="plural">
	  ///        The plural form of the alias. If <code>null
	  ///        </code>, then regular
	  ///        plural-forming rules are followed. </param>
	  /// <exception cref="NoSuchUnitException">
	  ///         The unit is not in the database. </exception>
	  /// <exception cref="UnitExistsException">
	  ///         Another unit with the same alias is already in the database. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void addAlias(final String alias, final String name, final String symbol, final String plural) throws NoSuchUnitException, UnitExistsException
	  public void addAlias(string alias, string name, string symbol, string plural)
	  {
		addAlias(UnitID.newUnitID(alias, plural, symbol), name);
	  }

	  /// <summary>
	  /// Adds an alias for a unit already in the database.
	  /// </summary>
	  /// <param name="alias">
	  ///        The alias to be added to the database. </param>
	  /// <param name="name">
	  ///        The name of the unit to have an alias added to the database. </param>
	  /// <exception cref="NoSuchUnitException">
	  ///         The unit is not in the database. </exception>
	  /// <exception cref="UnitExistsException">
	  ///         Another unit with the same alias is already in the database. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void addAlias(final UnitID alias, final String name) throws NoSuchUnitException, UnitExistsException
	  public void addAlias(UnitID alias, string name)
	  {
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final Unit unit = getByName(name);
		Unit unit = getByName(name);
		if (unit == null)
		{
		  throw new NoSuchUnitException(name);
		}
		addByName(alias.Name, unit);
		addByName(alias.Plural, unit);
		addBySymbol(alias.Symbol, unit);
	  }

	  /// <summary>
	  /// Gets a unit by either name, plural, or symbol. Retrieving the unit by
	  /// symbol is attempted before retrieving the unit by name because symbol
	  /// comparisons are case sensitive and, hence, should be more robust.
	  /// </summary>
	  /// <param name="id">
	  ///        The id to be matched. </param>
	  /// <returns> The unit whose name, plural, or symbol matches or
	  ///         <code>null</code> if no such unit was found. </returns>
	  public virtual Unit get(string id)
	  {
		Unit unit = getBySymbol(id);
		if (unit == null)
		{
		  unit = getByName(id);
		}
		return unit;
	  }

	  /// <summary>
	  /// Gets a unit by name.
	  /// </summary>
	  /// <param name="name">
	  ///        The name to be matched. </param>
	  /// <returns> The unit whose name, plural, or alias matches or
	  ///         <code>null</code> if no such unit was found. </returns>
	  public virtual Unit getByName(string name)
	  {
		return nameMap[canonicalize(name)];
	  }

	  /// <summary>
	  /// Returns the canonical form of a unit name.
	  /// </summary>
	  /// <param name="name">
	  ///        A unit name. </param>
	  /// <returns> The canonical form of the name. </returns>
	  private static string canonicalize(in string name)
	  {
		return name.ToLower().Replace(' ', '_');
	  }

	  /// <summary>
	  /// Gets a unit by symbol.
	  /// </summary>
	  /// <param name="symbol">
	  ///        The symbol to be matched. </param>
	  /// <returns> The unit whose symbol matches or <code>null
	  ///        </code> if no such unit was
	  ///         found. </returns>
	  public virtual Unit getBySymbol(string symbol)
	  {
		return symbolMap[symbol];
	  }

	  /// <summary>
	  /// Returns the string representation of this database.
	  /// </summary>
	  /// <returns> The string representation of this database. </returns>
	  public override string ToString()
	  {
		return unitSet.ToString();
	  }

	  /// <summary>
	  /// Gets an iterator over the units in the database.
	  /// </summary>
	  /// <returns> An iterator over the units in the database. The iterator's
	  ///         <code>next()</code> method returns objects of type
	  ///         <code>Unit</code>. </returns>
	  public IEnumerator<Unit> Iterator
	  {
		  get
		  {
			return unitSet.GetEnumerator();
		  }
	  }

	  /// <summary>
	  /// Adds a unit to the database by name.
	  /// </summary>
	  /// <param name="name">
	  ///        The name of the unit. If <code>null</code> then the unit is
	  ///        not added. </param>
	  /// <param name="newUnit">
	  ///        The unit to be added. </param>
	  /// <exception cref="UnitExistsException">
	  ///         Attempt to redefine an existing unit. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private void addByName(final String name, final Unit newUnit) throws UnitExistsException
	  private void addByName(in string name, in Unit newUnit)
	  {
		if (!string.ReferenceEquals(name, null))
		{
		  addUnique(nameMap, canonicalize(name), newUnit);
		}
	  }

	  /// <summary>
	  /// Adds a unit to the database by symbol.
	  /// </summary>
	  /// <param name="symbol">
	  ///        The symbol for the unit. If <code>null</code> then the unit is
	  ///        not added. </param>
	  /// <param name="newUnit">
	  ///        The unit to be added. </param>
	  /// <exception cref="UnitExistsException">
	  ///         Attempt to redefine an existing unit. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private void addBySymbol(final String symbol, final Unit newUnit) throws UnitExistsException
	  private void addBySymbol(in string symbol, in Unit newUnit)
	  {
		if (!string.ReferenceEquals(symbol, null))
		{
		  addUnique(symbolMap, symbol, newUnit);
		}
	  }

	  /// <summary>
	  /// Adds a unique unit to a map..
	  /// </summary>
	  /// <param name="map">
	  ///        The map to be added to. </param>
	  /// <param name="key">
	  ///        The key for the unit entry. </param>
	  /// <param name="newUnit">
	  ///        The unit to be added. </param>
	  /// <exception cref="UnitExistsException">
	  ///         Attempt to redefine an existing unit. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private static void addUnique(final NetcdfSharp.java.util.Map<String, Unit> map, final String key, final Unit newUnit) throws UnitExistsException
	  private static void addUnique(in IDictionary<string, Unit> map, in string key, in Unit newUnit)
	  {
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final Unit oldUnit = map.put(key, newUnit);
		Unit oldUnit = map[key] = newUnit;
		if (oldUnit != null && !oldUnit.Equals(newUnit))
		{
		  throw new UnitExistsException(oldUnit, newUnit);
		}
	  }
	}

}
