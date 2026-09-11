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
	/// Provides support for abstract base quantities (ex: length, time).
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	[Serializable]
	public abstract class BaseQuantity : Base, IComparable<BaseQuantity>
	{
	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// The base quantity of amount of substance.
	  /// </summary>
	  public static readonly RegularBaseQuantity AMOUNT_OF_SUBSTANCE;

	  /// <summary>
	  /// The base quantity of electric current.
	  /// </summary>
	  public static readonly RegularBaseQuantity ELECTRIC_CURRENT;

	  /// <summary>
	  /// The base quantity of length.
	  /// </summary>
	  public static readonly RegularBaseQuantity LENGTH;

	  /// <summary>
	  /// The base quantity of luminous intensity.
	  /// </summary>
	  public static readonly RegularBaseQuantity LUMINOUS_INTENSITY;

	  /// <summary>
	  /// The base quantity of mass.
	  /// </summary>
	  public static readonly RegularBaseQuantity MASS;

	  /// <summary>
	  /// The base quantity of plane angle.
	  /// </summary>
	  public static readonly SupplementaryBaseQuantity PLANE_ANGLE;

	  /// <summary>
	  /// The base quantity of solid angle.
	  /// </summary>
	  public static readonly SupplementaryBaseQuantity SOLID_ANGLE;

	  /// <summary>
	  /// The base quantity of themodynamic temperature.
	  /// </summary>
	  public static readonly RegularBaseQuantity THERMODYNAMIC_TEMPERATURE;

	  /// <summary>
	  /// The base quantity of time.
	  /// </summary>
	  public static readonly RegularBaseQuantity TIME;

	  /// <summary>
	  /// The unknown base quantity.
	  /// </summary>
	  public static readonly UnknownBaseQuantity UNKNOWN;

	  /// <summary>
	  /// @serial
	  /// </summary>
	  private readonly string name; // never
	  // null
	  // or
	  // empty

	  /// <summary>
	  /// @serial
	  /// </summary>
	  private readonly string symbol; // may
	  // be
	  // null
	  // ;
	  // never
	  // empty

	  static BaseQuantity()
	  {
		AMOUNT_OF_SUBSTANCE = new RegularBaseQuantity("Amount of Substance", "N", true);
		ELECTRIC_CURRENT = new RegularBaseQuantity("Electric Current", "I", true);
		LENGTH = new RegularBaseQuantity("Length", "L", true);
		LUMINOUS_INTENSITY = new RegularBaseQuantity("Luminous Intensity", "J", true);
		MASS = new RegularBaseQuantity("Mass", "M", true);
		PLANE_ANGLE = new SupplementaryBaseQuantity("Plane Angle", null, true);
		SOLID_ANGLE = new SupplementaryBaseQuantity("Solid Angle", null, true);
		THERMODYNAMIC_TEMPERATURE = new RegularBaseQuantity("Thermodynamic Temperature", "T", true);
		TIME = new RegularBaseQuantity("Time", "t", true);
		UNKNOWN = new UnknownBaseQuantity();
	  }

	  /// <summary>
	  /// Constructs from a name and a symbol.
	  /// </summary>
	  /// <param name="name">
	  ///        Name of the base quantity. Shall be neither <code>null</code>
	  ///        nor empty. </param>
	  /// <param name="symbol">
	  ///        Symbol for the base quantity. May be <code>null</code> but
	  ///        shall not be empty. </param>
	  /// <exception cref="NameException">
	  ///         <code>name</code> is <code>null</code> or empty. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public BaseQuantity(final String name, final String symbol) throws NameException
	  public BaseQuantity(in string name, in string symbol) : this(name, symbol, true)
	  {
		if (string.ReferenceEquals(name, null) || name.Length == 0 || (!string.ReferenceEquals(symbol, null) && symbol.Length == 0))
		{
		  throw new NameException("Invalid name or symbol");
		}
	  }

	  /// <summary>
	  /// Constructs from a name and a symbol. This is the trusted form of the the
	  /// constructor for use by subclasses only.
	  /// </summary>
	  /// <param name="name">
	  ///        Name of the base quantity. Shall be neither <code>null</code>
	  ///        nor empty. </param>
	  /// <param name="symbol">
	  ///        Symbol for the base quantity. May be <code>null</code> but
	  ///        shall not be empty. </param>
	  protected internal BaseQuantity(in string name, in string symbol, in bool trusted)
	  {
		this.name = name;
		this.symbol = symbol;
	  }

	  /// <summary>
	  /// Returns the name of the base quantity.
	  /// </summary>
	  /// <returns> The name of the base quantity. Shall not be <code>null</code> or
	  ///         empty. </returns>
	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  /// <summary>
	  /// Returns the symbol of the base quantity.
	  /// </summary>
	  /// <returns> The symbol of the base quantity. May be <code>null</code>. If
	  ///         non-<code>null</code>, then shall not be empty. </returns>
	  public virtual string Symbol
	  {
		  get
		  {
			return symbol;
		  }
	  }

	  /// <summary>
	  /// Returns the identifier for the base quantity.
	  /// </summary>
	  /// <returns> The base quantity's identifier (i.e. symbol or name). </returns>
	  public string ID
	  {
		  get
		  {
	// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	// ORIGINAL LINE: final String id = getSymbol();
			string id = Symbol;
			return string.ReferenceEquals(id, null) ? Name : id;
		  }
	  }

	  /// <summary>
	  /// Returns the string representation of the base quantity.
	  /// </summary>
	  /// <returns> The string representation of the base quantity. </returns>
	  public override sealed string ToString()
	  {
		return ID;
	  }

	  /// <summary>
	  /// Indicates if this base quantity is semantically identical to an object.
	  /// </summary>
	  /// <param name="object">
	  ///        The object to examine. </param>
	  /// <returns> <code>true</code> if an only if this base quantity is
	  ///         semantically identical to <code>object</code>. </returns>
	  public override bool Equals(object @object)
	  {
		return this == @object || @object is BaseQuantity && (Name.Equals(((BaseQuantity) @object).Name, StringComparison.OrdinalIgnoreCase) && (string.ReferenceEquals(Symbol, null) || Symbol.Equals(((BaseQuantity) @object).Symbol)));
	  }

	  /// <summary>
	  /// Returns the hash code of this instance.
	  /// </summary>
	  /// <returns> The hash code of this instance. </returns>
	  public override int GetHashCode()
	  {
		return Name.ToLower().GetHashCode() ^ (string.ReferenceEquals(Symbol, null) ? 0 : Symbol.GetHashCode());
	  }

	  /// <summary>
	  /// Compares this base quantity to another base quantity.
	  /// </summary>
	  /// <param name="that">
	  ///        The BaseQuantity to compare against. </param>
	  /// <returns> An integer that is negative, zero, or positive depending on
	  ///         whether this BaseQuantity is less than, equal to, or greater than
	  ///         <code>that</code>. </returns>
	  public virtual int CompareTo(BaseQuantity that)
	  {
		int comp;
		if (this == that)
		{
		  comp = 0;
		}
		else
		{
		  comp = string.Compare(Name, that.Name, StringComparison.OrdinalIgnoreCase);
		  if (comp == 0 && !string.ReferenceEquals(Symbol, null))
		  {
			comp = string.CompareOrdinal(Symbol, that.Symbol);
		  }
		}
		return comp;
	  }

	  /// <summary>
	  /// Indicates if this base quantity is dimensionless.
	  /// </summary>
	  /// <returns> <code>true</code> if an only if this BaseQuantity is
	  ///         dimensionless (e.g. <code>BaseQuantity.SOLID_ANGLE</code>). </returns>
	  public abstract bool Dimensionless {get;}

	}

}
