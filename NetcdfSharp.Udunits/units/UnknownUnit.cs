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
	/// Provides support for unknown base units. This can be used, for example, to
	/// accomodate an unknown unit (e.g. "foo"). Values in such a unit will only be
	/// convertible with units derived from "foo" (e.g. "20 foo").
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public final class UnknownUnit extends BaseUnit
	[Serializable]
	public sealed class UnknownUnit : BaseUnit
	{
	  private const long serialVersionUID = 1L;
	  /// <summary>
	  /// The name-to-unit map.
	  /// 
	  /// @serial
	  /// </summary>
	  private static readonly SortedDictionary<string, UnknownUnit> map = new SortedDictionary<string, UnknownUnit>();

	  /// <summary>
	  /// Constructs from a name.
	  /// </summary>
	  /// <param name="name"> The name of the unit. </param>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private UnknownUnit(final String name) throws NameException
	  private UnknownUnit(in string name) : base(UnitName.newUnitName(name, null, name), BaseQuantity.UNKNOWN)
	  {
	  }

	  /// <summary>
	  /// Factory method for constructing an unknown unit from a name.
	  /// </summary>
	  /// <param name="name"> The name of the unit. </param>
	  /// <returns> The unknown unit. </returns>
	  /// <exception cref="NameException"> <code>name == null</code>. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static UnknownUnit create(String name) throws NameException
	  public static UnknownUnit create(string name)
	  {
		UnknownUnit unit;
		name = name.ToLower();
		lock (map)
		{
		  unit = map[name];
		  if (unit == null)
		  {
			unit = new UnknownUnit(name);
			map[unit.Name] = unit;
			map[unit.Plural] = unit;
		  }
		}
		return unit;
	  }

	  /*
	   * From Unit:
	   */

	  /// <summary>
	  /// Indicates if this unit is semantically identical to an object.
	  /// </summary>
	  /// <param name="object"> The object. </param>
	  /// <returns> <code>true</code> if and only if this instance is semantically
	  ///         identical to the object. </returns>
	  public override bool Equals(object @object)
	  {
		if (this == @object)
		{
		  return true;
		}
		if (!(@object is UnknownUnit))
		{
		  return false;
		}
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final UnknownUnit that = (UnknownUnit) object;
		UnknownUnit that = (UnknownUnit) @object;
		return Name.Equals(that.Name, StringComparison.OrdinalIgnoreCase);
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
	  /// Indicates if this unit is dimensionless. An unknown unit is never
	  /// dimensionless.
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
