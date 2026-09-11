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
	/// Interface for derived units.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	public interface DerivedUnit : Unit
	{
	  /// <summary>
	  /// Indicates if this derived unit is the reciprocal of another derived
	  /// unit (e.g. "second" and "hertz").
	  /// </summary>
	  /// <param name="that"> The other, derived unit. </param>
	  bool isReciprocalOf(DerivedUnit that);

	  /// <summary>
	  /// Returns the unit dimension of this derived unit. For example, the unit
	  /// "newton" has the unit dimension "kg.m.s-2".
	  /// </summary>
	  /// <returns> The unit dimension of this derived unit. </returns>
	  UnitDimension Dimension {get;}

	  /// <summary>
	  /// Return the quantity dimension of this derived unit. For example, the
	  /// unit "newton" has the quantity dimension "M.L.t-2").
	  /// </summary>
	  /// <returns> The quantity dimension of this derived unit. </returns>
	  QuantityDimension QuantityDimension {get;}
	}

}
