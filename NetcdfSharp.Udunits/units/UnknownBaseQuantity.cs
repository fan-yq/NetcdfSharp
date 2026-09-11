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
	/// Provides support for an unknown base quantity.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	[Serializable]
	public sealed class UnknownBaseQuantity : BaseQuantity
	{
	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// Constructs from nothing.
	  /// </summary>
	  protected internal UnknownBaseQuantity() : base("Unknown", "x", true)
	  {
	  }

	  /// <summary>
	  /// Indicates if this quantity is semantically the same as an object. Unknown
	  /// quantities are never equal by definition -- not even to itself.
	  /// </summary>
	  /// <param name="object">
	  ///        The object. </param>
	  /// <returns> <code>false</code> always. </returns>
	  public override bool Equals(object @object)
	  {
		return false;
	  }

	  /// <summary>
	  /// Returns the hash code of this instance.
	  /// </summary>
	  /// <returns> The hash code of this instance. </returns>
	  public override int GetHashCode()
	  {
		return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(this);
	  }

	  /// <summary>
	  /// Indicates if this quantity is dimensionless. Unknown quantities are never
	  /// dimensionless by definition.
	  /// </summary>
	  /// <returns> <code>false</code> always. </returns>
	  public override bool Dimensionless
	  {
		  get
		  {
			return false;
		  }
	  }
	}

}
