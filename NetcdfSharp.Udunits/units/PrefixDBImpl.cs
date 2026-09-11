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
	/// Provides a concrete implementation of a database of unit prefixes.
	/// 
	/// Instances of this class are modifiable.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	[Serializable]
	public class PrefixDBImpl : PrefixDB
	{
	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// The set of prefix names.
	  /// 
	  /// @serial
	  /// </summary>
	  private readonly SortedSet<Prefix> nameSet;

	  /// <summary>
	  /// The set of prefix symbols.
	  /// 
	  /// @serial
	  /// </summary>
	  private readonly SortedSet<Prefix> symbolSet;

	  /// <summary>
	  /// The mapping between prefix values and prefixes.
	  /// 
	  /// @serial
	  /// </summary>
	  private readonly IDictionary<double, Prefix> valueMap;

	  /// <summary>
	  /// Constructs from nothing.
	  /// </summary>
	  public PrefixDBImpl()
	  {
		nameSet = new SortedSet<Prefix>();
		symbolSet = new SortedSet<Prefix>();
		valueMap = new SortedDictionary<double, Prefix>();
	  }

	  /// <summary>
	  /// Adds a prefix to the database by name.
	  /// </summary>
	  /// <param name="name">
	  ///        The name of the prefix to be added. </param>
	  /// <param name="value">
	  ///        The value of the prefix. </param>
	  /// <exception cref="PrefixExistsException">
	  ///         Another prefix with the same name or value already exists in
	  ///         the database. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public void addName(final String name, final double value) throws PrefixExistsException
	  public virtual void addName(string name, double value)
	  {
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final Prefix prefix = new PrefixName(name, value);
		Prefix prefix = new PrefixName(name, value);
		nameSet.Add(prefix);
	  }

	  /// <summary>
	  /// Adds a prefix symbol to the database.
	  /// </summary>
	  /// <param name="symbol">
	  ///        The symbol of the prefix to be added. </param>
	  /// <param name="value">
	  ///        The value of the prefix. </param>
	  /// <exception cref="PrefixExistsException">
	  ///         Another prefix with the same symbol or value already exists
	  ///         in the database. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public void addSymbol(final String symbol, final double value) throws PrefixExistsException
	  public virtual void addSymbol(string symbol, double value)
	  {
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final Prefix prefix = new PrefixSymbol(symbol, value);
		Prefix prefix = new PrefixSymbol(symbol, value);
		symbolSet.Add(prefix);
		valueMap[value] = prefix;
	  }

	  /// <summary>
	  /// Gets a prefix by name.
	  /// </summary>
	  /// <param name="string">
	  ///        The name to be matched. </param>
	  /// <returns> The prefix whose name matches or null. </returns>
	  public virtual Prefix getPrefixByName(string @string)
	  {
		return getPrefix(@string, nameSet);
	  }

	  /// <summary>
	  /// Gets a prefix by symbol.
	  /// </summary>
	  /// <param name="string">
	  ///        The symbol to be matched. </param>
	  /// <returns> The prefix whose symbol matches or null. </returns>
	  public virtual Prefix getPrefixBySymbol(string @string)
	  {
		return getPrefix(@string, symbolSet);
	  }

	  /// <summary>
	  /// Gets a prefix by value.
	  /// </summary>
	  /// <param name="value">
	  ///        The value to be matched. </param>
	  /// <returns> The prefix whose value matches or null. </returns>
	  public virtual Prefix getPrefixByValue(double value)
	  {
		return valueMap[value];
	  }

	  /// <summary>
	  /// Returns the prefix from the given set with the given identifier.
	  /// </summary>
	  /// <param name="string">
	  ///        The prefix identifier. </param>
	  /// <param name="set">
	  ///        The set to search. </param>
	  private static Prefix getPrefix(in string @string, in ISet<Prefix> set)
	  {
		foreach (Prefix prefix in set)
		{
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final int comp = prefix.compareTo(string);
		  int comp = prefix.CompareTo(@string);
		  if (comp == 0)
		  {
			return prefix;
		  }
		  if (comp > 0)
		  {
			break;
		  }
		}
		return null;
	  }

	  /// <summary>
	  /// Returns a string representation of this database.
	  /// </summary>
	  /// <returns> A string representation of this database. </returns>
	  public override string ToString()
	  {
		return "nameSet=" + nameSet + "symbolSet=" + symbolSet + "valueMap=" + valueMap;
	  }

	  /// <summary>
	  /// Gets an iterator over the prefixes in the database.
	  /// </summary>
	  /// <returns> An iterator over the entries in the database. The objects
	  ///         returned by the iterator will be of type <code>Prefix</code>. </returns>
	  public virtual IEnumerator<Prefix> iterator()
	  {
		return nameSet.GetEnumerator();
	  }

	}

}
