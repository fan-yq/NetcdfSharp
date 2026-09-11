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
	using Misc = NetcdfSharp.ucar.nc2.util.Misc;

	/// <summary>
	/// Standard implementation of LatLonPoint.
	/// Longitude is always between -180 and +180 deg.
	/// Latitude is always between -90 and +90 deg.
	/// 
	/// @author Russ Rew
	/// @author John Caron </summary>
	/// <seealso cref="LatLonPoint"/>
	/// @deprecated only use LatLonPoint in 6. 
	[Obsolete("only use LatLonPoint in 6."), Serializable]
	public class LatLonPointImpl : LatLonPoint
	{
		[Obsolete("use LatLonPoints.betweenLon")]
		public static bool betweenLon(double lon, double lonBeg, double lonEnd)
		{
		lonBeg = LatLonPoints.lonNormal(lonBeg, lon);
		lonEnd = LatLonPoints.lonNormal(lonEnd, lon);
		return (lon >= lonBeg) && (lon <= lonEnd);
		}


	  /// <summary>
	  /// put longitude into the range [-180, 180] deg
	  /// </summary>
	  /// <param name="lon"> lon to normalize </param>
	  /// <returns> longitude in range [-180, 180] deg </returns>
	  /// @deprecated use LatLonPoints.range180 
	  [Obsolete("use LatLonPoints.range180")]
	  public static double range180(double lon)
	  {
		return LatLonPoints.lonNormal(lon);
	  }

	  /// <summary>
	  /// put longitude into the range [0, 360] deg
	  /// </summary>
	  /// <param name="lon"> lon to normalize </param>
	  /// <returns> longitude into the range [0, 360] deg </returns>
	  /// @deprecated use LatLonPoints.lonNormal360 
	  [Obsolete("use LatLonPoints.lonNormal360")]
	  public static double lonNormal360(double lon)
	  {
		return LatLonPoints.lonNormal(lon, 180.0);
	  }

	  /// <summary>
	  /// put longitude into the range [center +/- 180] deg
	  /// </summary>
	  /// <param name="lon"> lon to normalize </param>
	  /// <param name="center"> center point </param>
	  /// <returns> longitude into the range [center +/- 180] deg </returns>
	  /// @deprecated use LatLonPoints.lonNormal 
	  [Obsolete("use LatLonPoints.lonNormal")]
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
	  /// @deprecated use LatLonPoints.lonNormalFrom 
	  [Obsolete("use LatLonPoints.lonNormalFrom")]
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
	  /// <param name="lon"> east latitude in degrees </param>
	  /// <returns> normalized lon </returns>
	  /// @deprecated use LatLonPoints.lonNormal 
	  [Obsolete("use LatLonPoints.lonNormal")]
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
	  /// @deprecated use LatLonPoints.lonDiff 
	  [Obsolete("use LatLonPoints.lonDiff")]
	  public static double lonDiff(double lon1, double lon2)
	  {
		return Math.IEEERemainder(lon1 - lon2, 360.0);
	  }

	  /// <summary>
	  /// Normalize the latitude to lie between +/-90
	  /// </summary>
	  /// <param name="lat"> north latitude in degrees </param>
	  /// <returns> normalized lat </returns>
	  /// @deprecated use LatLonPoints.latNormal 
	  [Obsolete("use LatLonPoints.latNormal")]
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
	  /// @deprecated use LatLonPoints.latToString 
	  [Obsolete("use LatLonPoints.latToString")]
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
	  /// @deprecated use LatLonPoints.lonToString 
	  [Obsolete("use LatLonPoints.lonToString")]
	  public static string lonToString(double lon, int ndec)
	  {
		double wlon = LatLonPoints.lonNormal(lon);
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

	  ///////////////////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// East latitude in degrees, always +/- 90
	  /// </summary>
	  protected internal double lat;

	  /// <summary>
	  /// North longitude in degrees, always +/- 180
	  /// </summary>
	  protected internal double lon;

	  /// @deprecated use LatLonPoint.create 
	  [Obsolete("use LatLonPoint.create")]
	  public LatLonPointImpl()
	  {
		// Don't initialize by calling setLatitude() and setLongitude(), as those methods throw
		// UnsupportedOperationException in LatLonPointImmutable, and this constructor is (implicitly) invoked in the
		// LatLonPointImmutable constructors.
		this.lat = 0;
		this.lon = 0;
	  }

	  /// <summary>
	  /// Copy Constructor.
	  /// </summary>
	  /// @deprecated use LatLonPoint.create 
	  [Obsolete("use LatLonPoint.create")]
	  public LatLonPointImpl(LatLonPoint pt) : this(pt.Latitude, pt.Longitude)
	  {
	  }

	  /// <summary>
	  /// Creates a LatLonPoint from component latitude and longitude values.
	  /// The longitude is adjusted to be in the range [-180.,180.].
	  /// </summary>
	  /// <param name="lat"> north latitude in degrees </param>
	  /// <param name="lon"> east longitude in degrees </param>
	  /// @deprecated use LatLonPoint.create 
	  [Obsolete("use LatLonPoint.create")]
	  public LatLonPointImpl(double lat, double lon)
	  {
		Latitude = lat;
		Longitude = lon;
	  }

	  /// <summary>
	  /// Returns the longitude, in degrees.
	  /// </summary>
	  /// <returns> the longitude, in degrees </returns>
	  public virtual double Longitude
	  {
		  get
		  {
			return lon;
		  }
		  set
		  {
			this.lon = LatLonPoints.lonNormal(value);
		  }
	  }

	  /// <summary>
	  /// Returns the latitude, in degrees.
	  /// </summary>
	  /// <returns> the latitude, in degrees </returns>
	  public virtual double Latitude
	  {
		  get
		  {
			return lat;
		  }
		  set
		  {
			this.lat = LatLonPoints.latNormal(value);
		  }
	  }

	  /// <summary>
	  /// set lat, lon using values of pt
	  /// </summary>
	  /// <param name="pt"> point to use </param>
	  public virtual void set(LatLonPoint pt)
	  {
		Longitude = pt.Longitude;
		Latitude = pt.Latitude;
	  }

	  /// <summary>
	  /// set lat, lon using double values
	  /// </summary>
	  /// <param name="lat"> lat value </param>
	  /// <param name="lon"> lon value </param>
	  public virtual void set(double lat, double lon)
	  {
		Longitude = lon;
		Latitude = lat;
	  }

	  /// <summary>
	  /// set lat, lon using float values
	  /// </summary>
	  /// <param name="lat"> lat value </param>
	  /// <param name="lon"> lon value </param>
	  public virtual void set(float lat, float lon)
	  {
		Longitude = (double) lon;
		Latitude = (double) lat;
	  }



	  public virtual bool nearlyEquals(LatLonPoint that, double maxRelDiff)
	  {
		bool lonOk = Misc.nearlyEquals(that.Longitude, this.lon, maxRelDiff);
		if (!lonOk)
		{
		  // We may be in a situation where "this.lon ≈ -180" and "that.lon ≈ +180", or vice versa.
		  // Those longitudes are equivalent, but not "nearlyEquals()". So, we normalize them both to lie in
		  // [0, 360] and compare them again.
		  lonOk = Misc.nearlyEquals(LatLonPoints.lonNormal360(that.Longitude), LatLonPoints.lonNormal360(this.lon), maxRelDiff);
		}
		return lonOk && Misc.nearlyEquals(that.Latitude, this.lat, maxRelDiff);
	  }

	  // Exact comparison is needed in order to be consistent with hashCode().
	  public override bool Equals(object other)
	  {
		if (this == other)
		{
		  return true;
		}
		if (!(other is LatLonPoint))
		{
		  return false;
		}
		LatLonPoint that = (LatLonPoint) other;
		if (that.Latitude.CompareTo(this.Latitude) != 0)
		{
		  return false;
		}
		return that.Longitude.CompareTo(this.Longitude) == 0;
	  }

	  public override int GetHashCode()
	  {
		int result;
		long temp;
		temp = System.BitConverter.DoubleToInt64Bits(lat);
		result = (int)(temp ^ (temp >>> 32));
		temp = System.BitConverter.DoubleToInt64Bits(lon);
		result = 31 * result + (int)(temp ^ (temp >>> 32));
		return result;
	  }

	  /// <summary>
	  /// Default string representation
	  /// </summary>
	  /// <returns> string representing this point </returns>
	  public override string ToString()
	  {
		return toString(4);
	  }

	  /// <summary>
	  /// String representation in the form, eg 40.23N 105.1W
	  /// </summary>
	  /// <param name="sigDigits"> significant digits </param>
	  /// @deprecated use LatLonPoints.toString 
	  [Obsolete("use LatLonPoints.toString")]
	  public virtual string toString(int sigDigits)
	  {
		Formatter sbuff = new Formatter();
		sbuff.format("%s %s", LatLonPoints.latToString(lat, sigDigits), LatLonPoints.lonToString(lon, sigDigits));
		return sbuff.ToString();
	  }
	}

}
