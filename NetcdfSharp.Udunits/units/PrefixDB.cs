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
	/// Interface for a database of unit prefixes.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	public interface PrefixDB
	{
	  /// <summary>
	  /// Adds a prefix to the database by name.
	  /// </summary>
	  /// <param name="name">
	  ///        The name of the prefix. </param>
	  /// <param name="value">
	  ///        The value of the prefix. </param>
	  /// <exception cref="PrefixExistsException">
	  ///         A prefix with the same name already exists in the database. </exception>
	  /// <exception cref="PrefixDBAccessException">
	  ///         Prefix database access failure. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: void addName(String name, double value) throws PrefixExistsException, PrefixDBAccessException;
	  void addName(string name, double value);

	  /// <summary>
	  /// Adds a prefix to the database by symbol.
	  /// </summary>
	  /// <param name="symbol">
	  ///        The symbol for the prefix. </param>
	  /// <param name="value">
	  ///        The value of the prefix. </param>
	  /// <exception cref="PrefixExistsException">
	  ///         A prefix with the same symbol already exists in the database. </exception>
	  /// <exception cref="PrefixDBAccessException">
	  ///         Prefix database access failure. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: void addSymbol(String symbol, double value) throws PrefixExistsException, PrefixDBAccessException;
	  void addSymbol(string symbol, double value);

	  /// <summary>
	  /// Gets a prefix from the database by name.
	  /// </summary>
	  /// <param name="name">
	  ///        The name of the prefix. </param>
	  /// <returns> prefix The prefix or null. </returns>
	  /// <exception cref="PrefixDBAccessException">
	  ///         Prefix database access failure. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: Prefix getPrefixByName(String name) throws PrefixDBAccessException;
	  Prefix getPrefixByName(string name);

	  /// <summary>
	  /// Gets a prefix from the database by symbol.
	  /// </summary>
	  /// <param name="symbol">
	  ///        The symbol for the prefix. </param>
	  /// <returns> prefix The prefix or null. </returns>
	  /// <exception cref="PrefixDBAccessException">
	  ///         Prefix database access failure. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: Prefix getPrefixBySymbol(String symbol) throws PrefixDBAccessException;
	  Prefix getPrefixBySymbol(string symbol);

	  /// <summary>
	  /// Gets a prefix from the database by value.
	  /// </summary>
	  /// <param name="value">
	  ///        The value for the prefix. </param>
	  /// <returns> prefix The prefix or null. </returns>
	  /// <exception cref="PrefixDBAccessException">
	  ///         Prefix database access failure. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: Prefix getPrefixByValue(double value) throws PrefixDBAccessException;
	  Prefix getPrefixByValue(double value);

	  /// <summary>
	  /// Gets a string representation of this database.
	  /// </summary>
	  /// <returns> A string representation of this database. </returns>
	  string ToString();

	  /// <summary>
	  /// Gets an iterator over the entries in the database.
	  /// </summary>
	  /// <returns> An iterator over the database. </returns>
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: NetcdfSharp.java.util.Iterator<?> iterator();
	  IEnumerator<Prefix> iterator();
	}

}
