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
	/// Provides support for failure to find a unit.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	[Serializable]
	public class NoSuchUnitException : SpecificationException
	{
	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// Constructs from a unit identifier.
	  /// </summary>
	  /// <param name="id">
	  ///        The identifier of the unit that couldn't be found. </param>
	  public NoSuchUnitException(in UnitID id) : this(id.ToString())
	  {
	  }

	  /// <summary>
	  /// Constructs from a unit identifier.
	  /// </summary>
	  /// <param name="id">
	  ///        The identifier of the unit that couldn't be found. </param>
	  public NoSuchUnitException(in string id) : base("Unit \"" + id + "\" not in database")
	  {
	  }
	}

}
