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
	/// @author Steven R. Emmerson
	/// </summary>
	[Serializable]
	public class UnitSystemImpl : UnitSystem
	{
	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// The quantity-to-base-unit map.
	  /// 
	  /// @serial
	  /// </summary>
	  private readonly Dictionary<BaseQuantity, BaseUnit> quantityMap;

	  /// <summary>
	  /// The base unit database;
	  /// 
	  /// @serial
	  /// </summary>
	  private readonly UnitDB baseUnitDB;

	  /// <summary>
	  /// The complete database;
	  /// 
	  /// @serial
	  /// </summary>
	  private readonly UnitDBImpl acceptableUnitDB;

	  /// <summary>
	  /// Constructs from a base unit database and a derived unit database.
	  /// </summary>
	  /// <param name="baseUnitDB">
	  ///        The base unit database. Shall only contain base units. </param>
	  /// <param name="derivedUnitDB">
	  ///        The derived unit database. Shall not contain any base units. </param>
	  /// <exception cref="UnitExistsException">
	  ///         A unit with the same identifier exists in both databases. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: protected UnitSystemImpl(final UnitDBImpl baseUnitDB, final UnitDBImpl derivedUnitDB) throws UnitExistsException
	  protected internal UnitSystemImpl(in UnitDBImpl baseUnitDB, in UnitDBImpl derivedUnitDB)
	  {
		quantityMap = new Dictionary<BaseQuantity, BaseUnit>(baseUnitDB.nameCount());
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: for (final NetcdfSharp.java.util.Iterator<?> iter = baseUnitDB.getIterator(); iter.hasNext();)
		for (var iter = baseUnitDB.Iterator; iter.MoveNext();)
		{
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final Unit unit = (Unit) iter.next();
		  Unit unit = (Unit) iter.Current;
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final BaseUnit baseUnit = (BaseUnit) unit;
		  BaseUnit baseUnit = (BaseUnit) unit;
		  quantityMap[baseUnit.BaseQuantity] = baseUnit;
		}
		this.baseUnitDB = baseUnitDB;
		acceptableUnitDB = new UnitDBImpl(baseUnitDB.nameCount() + derivedUnitDB.nameCount(), baseUnitDB.symbolCount() + derivedUnitDB.symbolCount());
		acceptableUnitDB.add(baseUnitDB);
		acceptableUnitDB.add(derivedUnitDB);
	  }

	  /// <summary>
	  /// Returns the base unit database.
	  /// </summary>
	  /// <returns> The base unit database. </returns>
	  public UnitDB BaseUnitDB
	  {
		  get
		  {
			return baseUnitDB;
		  }
	  }

	  /// <summary>
	  /// Returns the complete unit database.
	  /// </summary>
	  /// <returns> The complete unit database (both base units and derived units). </returns>
	  public UnitDB UnitDB
	  {
		  get
		  {
			return acceptableUnitDB;
		  }
	  }

	  /// <summary>
	  /// Returns the base unit corresponding to a base quantity.
	  /// </summary>
	  /// <param name="quantity">
	  ///        The base quantity. </param>
	  /// <returns> The base unit corresponding to the base quantity in this system
	  ///         of units or <code>
	  ///        null</code> if no such unit exists. </returns>
	  public BaseUnit getBaseUnit(BaseQuantity quantity)
	  {
		return quantityMap[quantity];
	  }
	}

}
