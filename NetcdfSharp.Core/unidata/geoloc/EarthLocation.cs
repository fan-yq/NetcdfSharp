// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.unidata.geoloc
{
	/// <summary>
	/// A location on the earth, specified by lat, lon and optionally altitude.
	/// 
	/// @author caron
	/// @since Feb 18, 2008
	/// </summary>
	public interface EarthLocation
	{
	  /// <summary>
	  /// Standard way to create an EarthLocation. </summary>
	  static EarthLocation create(double lat, double lon, double alt)
	  {
		return new EarthLocationImpl(lat, lon, alt);
	  }

	  /// <summary>
	  /// Returns the latitude in some unit. The unit is very likely decimal degrees north, but we don't enforce that
	  /// anywhere.
	  /// </summary>
	  /// <returns> the latitude in some unit. </returns>
	  // FIXME: Enforce the "decimal degrees north" unit in EarthLocationImpl and other subclasses.
	  // Or, allow a different unit and make it available from EarthLocation.
	  double Latitude {get;}

	  /// <summary>
	  /// Returns the longitude in some unit. The unit is very likely decimal degrees east, but we don't enforce that
	  /// anywhere.
	  /// </summary>
	  /// <returns> the longitude in some unit. </returns>
	  // FIXME: Enforce the "decimal degrees east" unit in EarthLocationImpl and other subclasses.
	  // Or, allow a different unit and make it available from EarthLocation.
	  double Longitude {get;}

	  /// <summary>
	  /// Returns the altitude in some unit.
	  /// </summary>
	  /// <returns> the altitude in some unit. A value of <seealso cref="double.NaN"/> indicates "no altitude". </returns>
	  // FIXME: Make the unit available from EarthLocation.
	  double Altitude {get;}

	  /// <summary>
	  /// Get the lat/lon location
	  /// </summary>
	  /// <returns> lat/lon location </returns>
	  LatLonPoint LatLon {get;}

	  /// <summary>
	  /// Are either lat or lon missing?
	  /// </summary>
	  /// <returns> true if lat or lon is missing </returns>
	  bool Missing {get;}
	}

}
