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
	/// Provides support for unit names.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	[Serializable]
	public sealed class UnitName : UnitID, IComparable<object>
	{
	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// The name of the unit.
	  /// 
	  /// @serial
	  /// </summary>
	  private readonly string name;

	  /// <summary>
	  /// The plural form of the unit name.
	  /// 
	  /// @serial
	  /// </summary>
	  private readonly string plural;

	  /// <summary>
	  /// The symbol for the unit.
	  /// 
	  /// @serial
	  /// </summary>
	  private readonly string symbol;

	  /// <summary>
	  /// Constructs from a name and a symbol. Regular rules are use to construct
	  /// the plural form of the unit name.
	  /// </summary>
	  /// <param name="name">
	  ///        The name of the unit. Shall not be <code>
	  ///        null</code>. </param>
	  /// <param name="symbol">
	  ///        The symbol for the unit. May be <code>null
	  ///        </code>. </param>
	  /// <exception cref="NameException">
	  ///         <code>name == null</code>. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: protected UnitName(final String name, final String symbol) throws NameException
	  protected internal UnitName(in string name, in string symbol) : this(name, null, symbol)
	  {
	  }

	  /// <summary>
	  /// Constructs from a name, a plural form of the unit name, and a symbol.
	  /// </summary>
	  /// <param name="name">
	  ///        The name of the unit. Shall not be <code>
	  ///        null</code>. </param>
	  /// <param name="plural">
	  ///        The plural form of the name. May be <code>
	  ///        null</code>, in which
	  ///        case regular plural- forming rules are used to construct the
	  ///        plural form from the name. </param>
	  /// <param name="symbol">
	  ///        The symbol for the unit. May be <code>null
	  ///        </code>. </param>
	  /// <exception cref="NameException">
	  ///         <code>name == null</code>. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: protected UnitName(final String name, final String plural, final String symbol) throws NameException
	  protected internal UnitName(in string name, in string plural, in string symbol)
	  {
		if (string.ReferenceEquals(name, null))
		{
		  throw new NameException("Unit name can't be null");
		}
		this.name = name;
		this.plural = string.ReferenceEquals(plural, null) ? makePlural(name) : plural;
		this.symbol = symbol;
	  }

	  /// <summary>
	  /// Factory method for constructing a UnitName from a name.
	  /// </summary>
	  /// <param name="name">
	  ///        The name of the unit. Shall not be <code>
	  ///        null</code>. </param>
	  /// <exception cref="NameException">
	  ///         <code>name == null</code>. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static UnitName newUnitName(final String name) throws NameException
	  public static UnitName newUnitName(in string name)
	  {
		return newUnitName(name, null);
	  }

	  /// <summary>
	  /// Factory method for constructing a UnitName from a name and a plural form
	  /// of the name.
	  /// </summary>
	  /// <param name="name">
	  ///        The name of the unit. Shall not be <code>
	  ///        null</code>. </param>
	  /// <param name="plural">
	  ///        The plural form of the name. May be <code>
	  ///        null</code>, in which
	  ///        case regular plural- forming rules are used to construct the
	  ///        plural form from the name. </param>
	  /// <exception cref="NameException">
	  ///         <code>name == null</code>. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static UnitName newUnitName(final String name, final String plural) throws NameException
	  public static UnitName newUnitName(in string name, in string plural)
	  {
		return newUnitName(name, plural, null);
	  }

	  /// <summary>
	  /// Factory method for constructing a UnitName from a name, a plural form of
	  /// the name, and a symbol.
	  /// </summary>
	  /// <param name="name">
	  ///        The name of the unit. Shall not be <code>
	  ///        null</code>. </param>
	  /// <param name="plural">
	  ///        The plural form of the name. May be <code>
	  ///        null</code>, in which
	  ///        case regular plural- forming rules are used to construct the
	  ///        plural form from the name. </param>
	  /// <param name="symbol">
	  ///        The symbol for the unit. May be <code>null
	  ///        </code>. </param>
	  /// <exception cref="NameException">
	  ///         <code>name == null</code>. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static UnitName newUnitName(final String name, final String plural, final String symbol) throws NameException
	  public static UnitName newUnitName(in string name, in string plural, in string symbol)
	  {
		return new UnitName(name, plural, symbol);
	  }

	  /// <summary>
	  /// Returns the name.
	  /// </summary>
	  /// <returns> The name. Won't be <code>null</code>. </returns>
	  public override string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  /// <summary>
	  /// Returns the plural form of the unit name.
	  /// </summary>
	  /// <returns> The plural form of the unit name. </returns>
	  public override string Plural
	  {
		  get
		  {
			return plural;
		  }
	  }

	  /// <summary>
	  /// Returns the symbol.
	  /// </summary>
	  /// <returns> The symbol. Might be <code>null</code>. </returns>
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
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final String string = getSymbol();
		string @string = Symbol;
		return string.ReferenceEquals(@string, null) ? Name : @string;
	  }

	  /// <summary>
	  /// Compares this UnitName with another UnitName.
	  /// </summary>
	  public int CompareTo(object @object)
	  {
		return string.Compare(Name, ((UnitName) @object).Name, StringComparison.OrdinalIgnoreCase);
	  }

	  /// <summary>
	  /// Indicates if this UnitName is semantically identical to an object.
	  /// </summary>
	  public override bool Equals(object @object)
	  {
		return @object is UnitName && CompareTo(@object) == 0;
	  }

	  /// <summary>
	  /// Returns the hash code of this instance.
	  /// </summary>
	  /// <returns> The hash code of this instance. </returns>
	  public override int GetHashCode()
	  {
		return Name.ToLower().GetHashCode();
	  }

	  /// <summary>
	  /// Returns the plural form of a name. Regular rules are used to generate the
	  /// plural form.
	  /// </summary>
	  /// <param name="name">
	  ///        The name. </param>
	  /// <returns> The plural form of the name. </returns>
	  protected internal string makePlural(in string name)
	  {
		string plural;
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final int length = name.length();
		int length = name.Length;
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final char lastChar = name.charAt(length - 1);
		char lastChar = name[length - 1];
		if (lastChar != 'y')
		{
		  plural = name + (lastChar == 's' || lastChar == 'x' || lastChar == 'z' || name.EndsWith("ch", StringComparison.Ordinal) ? "es" : "s");
		}
		else
		{
		  if (length == 1)
		  {
			plural = name + "s";
		  }
		  else
		  {
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final char penultimateChar = name.charAt(length - 2);
			char penultimateChar = name[length - 2];
			plural = (penultimateChar == 'a' || penultimateChar == 'e' || penultimateChar == 'i' || penultimateChar == 'o' || penultimateChar == 'u') ? name + "s" : name.Substring(0, length - 1) + "ies";
		  }
		}
		return plural;
	  }
	}

}
