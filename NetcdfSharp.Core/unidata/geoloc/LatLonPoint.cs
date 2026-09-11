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
	using Misc = NetcdfSharp.ucar.nc2.util.Misc;

	/// <summary>
	/// Points on the Earth's surface, represented as (longitude,latitude),
	/// in units of degrees.
	/// Longitude is always between -180 and +180 deg.
	/// Latitude is always between -90 and +90 deg.
	/// 
	/// TODO will be an Immutable class in ver6
	/// </summary>
	public interface LatLonPoint
	{

	  /// <summary>
	  /// Returns the longitude, between +/-180 degrees
	  /// </summary>
	  /// <returns> longitude (degrees) </returns>
	  double Longitude {get;}

	  /// <summary>
	  /// Returns the latitude, between +/- 90 degrees.
	  /// </summary>
	  /// <returns> latitude (degrees) </returns>
	  double Latitude {get;}

	  /// <summary>
	  /// Returns the result of <seealso cref="nearlyEquals(LatLonPoint, double)"/>, with <seealso cref="Misc.defaultMaxRelativeDiffDouble"/>.
	  /// </summary>
	  bool nearlyEquals(LatLonPoint that)
	  {
		return nearlyEquals(that, Misc.defaultMaxRelativeDiffDouble);
	  }

	  /// <summary>
	  /// Returns {@code true} if this point is nearly equal to {@code that}. The "near equality" of points is determined
	  /// using <seealso cref="Misc.nearlyEquals(double, double, double)"/>, with the specified maxRelDiff.
	  /// </summary>
	  /// <param name="that"> the other point to check. </param>
	  /// <param name="maxRelDiff"> the maximum <seealso cref="Misc.relativeDifference relative difference"/> the two points may have. </param>
	  /// <returns> {@code true} if this point is nearly equal to {@code that}. </returns>
	  bool nearlyEquals(LatLonPoint that, double maxRelDiff);

	  /// <summary>
	  /// Standard way to create a LatLonPoint. </summary>
	  static LatLonPoint create(double lat, double lon)
	  {
		return new LatLonPointImpl(lat, lon);
	  }

	  /// <summary>
	  /// Standard way to create a "default" LatLonPoint. </summary>
	  static LatLonPoint create()
	  {
		return new LatLonPointImpl();
	  }
	}

}
