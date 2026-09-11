// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.units
{
	/// <summary>
	/// Interface for a system of units.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	public interface UnitSystem
	{
	  /// <summary>
	  /// Returns the database of base units.
	  /// </summary>
	  /// <returns> The database of base units. </returns>
	  UnitDB BaseUnitDB {get;}

	  /// <summary>
	  /// Returns the complete database of units (base units and
	  /// derived units acceptable for use in the system of units.
	  /// </summary>
	  /// <returns> The complete database of units. </returns>
	  UnitDB UnitDB {get;}

	  /// <summary>
	  /// Returns the base unit corresponding to a base quantity.
	  /// </summary>
	  /// <param name="quantity"> A base quantity. </param>
	  /// <returns> The base unit corresponding to the base
	  ///         quantity or <code>null</code> if no such
	  ///         unit exists. </returns>
	  BaseUnit getBaseUnit(BaseQuantity quantity);
	}

}
