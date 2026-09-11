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
namespace NetcdfSharp.ucar.unidata.geoloc
{

	/// <summary>
	/// A named location on the earth.
	/// 
	/// @author caron
	/// @since Feb 18, 2008
	/// </summary>
	public interface Station : EarthLocation, IComparable<Station>
	{

	  /// <summary>
	  /// Station name or id. Must be unique within the collection
	  /// </summary>
	  /// <returns> station name or id. May not be null. </returns>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nonnull String getName();
	  string Name {get;}

	  /// <summary>
	  /// Station description
	  /// </summary>
	  /// <returns> station description or null </returns>
	  string Description {get;}

	  /// <summary>
	  /// WMO station id.
	  /// </summary>
	  /// <returns> WMO station id, or null. </returns>
	  string WmoId {get;}

	  /// <summary>
	  /// get Number of obs at this station
	  /// </summary>
	  /// <returns> Number of obs or -1 if unknown </returns>
	  int Nobs {get;}

	}

}
