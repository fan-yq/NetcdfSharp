// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

/*
 * Copyright (c) 1998-2025 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.unidata.geoloc
{

	/// <summary>
	/// Static utilities for LatLonPoint.
	/// 
	/// @author caron
	/// @since 3/15/2020.
	/// </summary>
	public class LatLonPoints
	{

	  /// <summary>
	  /// Test if point lies between two longitudes, deal with wrapping.
	  /// </summary>
	  /// <param name="lon"> point to test </param>
	  /// <param name="lonBeg"> beginning longitude </param>
	  /// <param name="lonEnd"> ending longitude </param>
	  /// <returns> true if lon is between lonBeg and lonEnd. </returns>
	  public static bool betweenLon(double lon, double lonBeg, double lonEnd)
	  {
		lonBeg = lonNormal(lonBeg, lon);
		lonEnd = lonNormal(lonEnd, lon);
		return (lon >= lonBeg) && (lon <= lonEnd);
	  }

	  /// <summary>
	  /// put longitude into the range [-180, 180] deg
	  /// </summary>
	  /// <param name="lon"> lon to normalize </param>
	  /// <returns> longitude in range [-180, 180] deg </returns>
	  public static double range180(double lon)
	  {
		return lonNormal(lon);
	  }

	  /// <summary>
	  /// put longitude into the range [0, 360] deg
	  /// </summary>
	  /// <param name="lon"> lon to normalize </param>
	  /// <returns> longitude into the range [0, 360] deg </returns>
	  public static double lonNormal360(double lon)
	  {
		return lonNormal(lon, 180.0);
	  }

	  /// <summary>
	  /// put longitude into the range [center +/- 180] deg
	  /// </summary>
	  /// <param name="lon"> lon to normalize </param>
	  /// <param name="center"> center point </param>
	  /// <returns> longitude into the range [center +/- 180] deg </returns>
	  public static double lonNormal(double lon, double center)
	  {
		return center + Math.IEEERemainder(lon - center, 360.0);
	  }

	  /// <summary>
	  /// put longitude into the range [start, start+360] deg
	  /// </summary>
	  /// <param name="lon"> lon to normalize </param>
	  /// <param name="start"> starting point </param>
	  /// <returns> longitude into the [start, start+360] deg </returns>
	  public static double lonNormalFrom(double lon, double start)
	  {
		while (lon < start)
		{
		  lon += 360;
		}
		while (lon > start + 360)
		{
		  lon -= 360;
		}
		return lon;
	  }

	  /// <summary>
	  /// Normalize the longitude to lie between +/-180
	  /// </summary>
	  /// <param name="lon"> east longitude in degrees </param>
	  /// <returns> normalized lon </returns>
	  public static double lonNormal(double lon)
	  {
		if ((lon < -180.0) || (lon > 180.0))
		{
		  return Math.IEEERemainder(lon, 360.0);
		}
		else
		{
		  return lon;
		}
	  }

	  /// <summary>
	  /// Find difference (lon1 - lon2) normalized so that maximum value is += 180.
	  /// </summary>
	  /// <param name="lon1"> start </param>
	  /// <param name="lon2"> end </param>
	  /// <returns> normalized difference </returns>
	  public static double lonDiff(double lon1, double lon2)
	  {
		return Math.IEEERemainder(lon1 - lon2, 360.0);
	  }

	  /// <summary>
	  /// Normalize the latitude to lie between +/-90
	  /// </summary>
	  /// <param name="lat"> north latitude in degrees </param>
	  /// <returns> normalized lat </returns>
	  public static double latNormal(double lat)
	  {
		if (lat < -90.0)
		{
		  return -90.0;
		}
		else if (lat > 90.0)
		{
		  return 90.0;
		}
		else
		{
		  return lat;
		}
	  }

	  /// <summary>
	  /// Make a nicely formatted representation of a latitude, eg 40.34N or 12.9S.
	  /// </summary>
	  /// <param name="lat"> the latitude. </param>
	  /// <param name="ndec"> number of digits to right of decimal point </param>
	  /// <returns> String representation. </returns>
	  public static string latToString(double lat, int ndec)
	  {
		bool is_north = (lat >= 0.0);
		if (!is_north)
		{
		  lat = -lat;
		}

		string f = "%." + ndec + "f";

		Formatter latBuff = new Formatter();
		latBuff.format(f, lat);
		latBuff.format("%s", is_north ? "N" : "S");

		return latBuff.ToString();
	  }

	  /// <summary>
	  /// Make a nicely formatted representation of a longitude, eg 120.3W or 99.99E.
	  /// </summary>
	  /// <param name="lon"> the longitude. </param>
	  /// <param name="ndec"> number of digits to right of decimal point </param>
	  /// <returns> String representation. </returns>
	  public static string lonToString(double lon, int ndec)
	  {
		double wlon = lonNormal(lon);
		bool is_east = (wlon >= 0.0);
		if (!is_east)
		{
		  wlon = -wlon;
		}

		string f = "%." + ndec + "f";

		Formatter latBuff = new Formatter();
		latBuff.format(f, wlon);
		latBuff.format("%s", is_east ? "E" : "W");

		return latBuff.ToString();
	  }

	  /// <summary>
	  /// See if either coordinate in <code>pt</code> is +/- infinite.
	  /// This happens sometimes in projective geometry.
	  /// </summary>
	  /// <param name="pt"> point to check </param>
	  /// <returns> true if either coordinate is +/- infinite. </returns>
	  public static bool isInfinite(ProjectionPoint pt)
	  {
		return (pt.X == double.PositiveInfinity) || (pt.X == double.NegativeInfinity) || (pt.Y == double.PositiveInfinity) || (pt.Y == double.NegativeInfinity);
	  }

	  /// <summary>
	  /// String representation in the form, eg 40.23N 105.1W
	  /// </summary>
	  /// <param name="pt"> the LatLonPoint </param>
	  /// <param name="sigDigits"> significant digits </param>
	  /// <returns> String representation </returns>
	  public static string toString(LatLonPoint pt, int sigDigits)
	  {
		Formatter sbuff = new Formatter();
		sbuff.format("%s %s", LatLonPoints.latToString(pt.Latitude, sigDigits), LatLonPoints.lonToString(pt.Longitude, sigDigits));
		return sbuff.ToString();
	  }

	}

}
