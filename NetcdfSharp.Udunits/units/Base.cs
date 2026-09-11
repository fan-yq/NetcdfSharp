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
	/// Interface for "base" entities like base units or base quantities.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	public interface Base
	{
	  /// <summary>
	  /// Indicates if this base entity is dimensionless.
	  /// </summary>
	  /// <returns> <code>true</code> if and only if the base entity is dimensionless
	  ///         (e.g. (BaseQuantity.SOLID_ANGLE</code>). </returns>
	  bool Dimensionless {get;}

	  /// <summary>
	  /// Returns the identifier for the base entity.
	  /// </summary>
	  /// <returns> The base entity's identifier (i.e. symbol or name). </returns>
	  string ID {get;}

	  /// <summary>
	  /// Indicates if this base entity is semantically the same as another object.
	  /// </summary>
	  /// <param name="object">
	  ///        The other object. </param>
	  /// <returns> <code>true</code> if and only if this base entity is semantically
	  ///         the same as <code>object</code>. </returns>
	  bool Equals(object @object);
	}

}
