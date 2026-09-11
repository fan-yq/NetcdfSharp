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
	/// Provides support for base units.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class BaseUnit extends DerivedUnitImpl implements Base
	[Serializable]
	public class BaseUnit : DerivedUnitImpl, Base
	{
	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// The identifier-to-unit map.
	  /// 
	  /// @serial
	  /// </summary>
	  private static readonly SortedDictionary<UnitName, BaseUnit> nameMap = new SortedDictionary<UnitName, BaseUnit>();

	  /// <summary>
	  /// The quantity-to-unit map.
	  /// 
	  /// @serial
	  /// </summary>
	  private static readonly SortedDictionary<BaseQuantity, BaseUnit> quantityMap = new SortedDictionary<BaseQuantity, BaseUnit>();

	  /// <summary>
	  /// The base quantity associated with this base unit.
	  /// 
	  /// @serial
	  /// </summary>
	  private readonly BaseQuantity baseQuantity;

	  /// <summary>
	  /// Constructs from identifiers and a base quantity.
	  /// </summary>
	  /// <param name="id">
	  ///        The identifiers for the base unit. <code>
	  ///        id.getSymbol()</code>
	  ///        shall not return <code>
	  ///        null</code>. </param>
	  /// <param name="baseQuantity">
	  ///        The base quantity of the base unit. </param>
	  /// <exception cref="NameException">
	  ///         <code>id.getSymbol()</code> returned <code>
	  ///        null</code>. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: protected BaseUnit(final UnitName id, final BaseQuantity baseQuantity) throws NameException
	  protected internal BaseUnit(in UnitName id, in BaseQuantity baseQuantity) : base(id)
	  {
		if (string.ReferenceEquals(id.Symbol, null))
		{
		  throw new NameException("Base unit must have symbol");
		}
		Dimension = new UnitDimension(this);
		this.baseQuantity = baseQuantity;
	  }

	  /// <summary>
	  /// Factory method for creating a new BaseUnit or obtaining a
	  /// previously-created one.
	  /// </summary>
	  /// <param name="id">
	  ///        The identifier for the base unit. <code>
	  ///        id.getSymbol()</code>
	  ///        shall not return <code>
	  ///        null</code>. </param>
	  /// <param name="baseQuantity">
	  ///        The base quantity of the base unit. </param>
	  /// <exception cref="NameException">
	  ///         <code>id.getSymbol()</code> returned <code>
	  ///        null</code>. </exception>
	  /// <exception cref="UnitExistsException">
	  ///         Attempt to incompatibly redefine an existing base unit. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static synchronized BaseUnit getOrCreate(final UnitName id, final BaseQuantity baseQuantity) throws NameException, UnitExistsException
	  public static BaseUnit getOrCreate(in UnitName id, in BaseQuantity baseQuantity)
	  {
		  lock (typeof(BaseUnit))
		  {
			BaseUnit baseUnit;
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final BaseUnit nameUnit = nameMap.get(id);
			BaseUnit nameUnit = nameMap[id];
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final BaseUnit quantityUnit = quantityMap.get(baseQuantity);
			BaseUnit quantityUnit = quantityMap[baseQuantity];
			if (nameUnit != null || quantityUnit != null)
			{
			  baseUnit = nameUnit != null ? nameUnit : quantityUnit;
			  if ((nameUnit != null && !baseQuantity.Equals(nameUnit.BaseQuantity)) || (quantityUnit != null && !id.Equals(quantityUnit.UnitName)))
			  {
				throw new UnitExistsException("Attempt to incompatibly redefine base unit \"" + baseUnit + '"');
			  }
			}
			else
			{
			  baseUnit = new BaseUnit(id, baseQuantity);
			  quantityMap[baseQuantity] = baseUnit;
			  nameMap[id] = baseUnit;
			}
			return baseUnit;
		  }
	  }

	  /// <summary>
	  /// Returns the base quantity associated with this base unit.
	  /// </summary>
	  /// <returns> The base quantity associated with this base unit. </returns>
	  public BaseQuantity BaseQuantity
	  {
		  get
		  {
			return baseQuantity;
		  }
	  }

	  /// <summary>
	  /// Returns the identifier for this base unit. This is identical to
	  /// <code>getSymbol()</code>.
	  /// </summary>
	  /// <returns> The identifier for this base unit. </returns>
	  public string ID
	  {
		  get
		  {
			return Symbol;
		  }
	  }

	  /// <summary>
	  /// Returns the string representation of this base unit. This is identical to
	  /// <code>getID()</code>.
	  /// </summary>
	  /// <returns> The string representation of this base unit. </returns>
	  public sealed override string ToString()
	  {
		return ID;
	  }

	  /// <summary>
	  /// Indicates if this base unit is dimensionless.
	  /// </summary>
	  /// <returns> <code>true</code> if and only if this base unit is dimensionless
	  ///         (e.g. "radian"). </returns>
	  public override bool Dimensionless
	  {
		  get
		  {
			return baseQuantity.Dimensionless;
		  }
	  }

	}

}
