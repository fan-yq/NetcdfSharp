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
	/// Provides support for a database of standard unit prefixes.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	[Serializable]
	public sealed class StandardPrefixDB : PrefixDBImpl
	{
	  private const long serialVersionUID = 1L;
	  /// <summary>
	  /// The singleton instance of this class.
	  /// 
	  /// @serial
	  /// </summary>
// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
	  private static StandardPrefixDB instance_Conflict = null;

	  /// <summary>
	  /// Constructs from nothing.
	  /// </summary>
	  /// <exception cref="PrefixExistsException"> Attempt to redefine a prefix. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private StandardPrefixDB() throws PrefixExistsException
	  private StandardPrefixDB()
	  {
		/*
		 * SI prefixes:
		 */
		add("yotta", "Y", 1e24);
		add("zetta", "Z", 1e21);
		add("exa", "E", 1e18);
		add("peta", "P", 1e15);
		add("tera", "T", 1e12);
		add("giga", "G", 1e9); // 1st syllable pronounced "jig"
		// according to "ASTM Designation: E
		// 380 - 85: Standard for METRIC
		// PRACTICE".
		add("mega", "M", 1e6);
		add("kilo", "k", 1e3);
		add("hecto", "h", 1e2);
		add("deca", "da", 1e1); // Spelling according to "ISO 2955:
		// Information processing --
		// Representation of SI and other units
		// in systems with limited character
		// sets"
		addName("deka", 1e1); // Designation: E 380 - 85: Standard
		// for METRIC PRACTICE", "ANSI/IEEE Std
		// 260-1978 (Reaffirmed 1985): IEEE
		// Standard Letter Symbols for Units of
		// Measurement", and NIST Special
		// Publication 811, 1995 Edition:
		// "Guide for the Use of the
		// International System of Units (SI)".
		add("deci", "d", 1e-1);
		add("centi", "c", 1e-2);
		add("milli", "m", 1e-3);
		add("micro", "u", 1e-6);
		add("nano", "n", 1e-9);
		add("pico", "p", 1e-12);
		add("femto", "f", 1e-15);
		add("atto", "a", 1e-18);
		add("zepto", "z", 1e-21);
		add("yocto", "y", 1e-24);
	  }

	  /// <summary>
	  /// Gets an instance of this database.
	  /// </summary>
	  /// <returns> An instance of this database. </returns>
	  /// <exception cref="PrefixDBException"> The instance couldn't be created. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static synchronized StandardPrefixDB instance() throws PrefixDBException
	  public static StandardPrefixDB instance()
	  {
		  lock (typeof(StandardPrefixDB))
		  {
			if (instance_Conflict == null)
			{
			  try
			  {
				instance_Conflict = new StandardPrefixDB();
			  }
// JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
// ORIGINAL LINE: catch (final Exception e)
			  catch (Exception e)
			  {
				throw new PrefixDBException("Couldn't create standard prefix-database", e);
			  }
			}
			return instance_Conflict;
		  }
	  }

	  /// <summary>
	  /// Adds a prefix to the database.
	  /// </summary>
	  /// <param name="name"> The name of the prefix. </param>
	  /// <param name="symbol"> The symbol for the prefix. </param>
	  /// <param name="definition"> The numeric value of the prefix. </param>
	  /// <exception cref="PrefixExistsException"> Attempt to redefine an existing prefix. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private void add(final String name, final String symbol, final double definition) throws PrefixExistsException
	  private void add(in string name, in string symbol, in double definition)
	  {
		addName(name, definition);
		addSymbol(symbol, definition);
	  }
	}

}
