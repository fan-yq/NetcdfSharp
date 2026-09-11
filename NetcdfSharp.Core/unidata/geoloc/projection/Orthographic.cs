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
namespace NetcdfSharp.ucar.unidata.geoloc.projection
{
	using CF = NetcdfSharp.ucar.nc2.constants.CF;
	using NetcdfSharp.ucar.unidata.geoloc;

	/// <summary>
	/// Orthographic Projection spherical earth.
	/// <p/>
	/// See John Snyder, Map Projections used by the USGS, Bulletin 1532,
	/// 2nd edition (1983), p 145
	/// 
	/// @author Unidata Development Team </summary>
	/// <seealso cref="Projection"/>
	/// <seealso cref="ProjectionImpl"/>

	[Serializable]
	public class Orthographic : ProjectionImpl
	{

	  private double lat0, lon0; // center lat/lon in radians
	  private readonly double R; // radius

	  /// <summary>
	  /// constants from Snyder's equations
	  /// </summary>
	  private double lon0Degrees;
	  private double cosLat0, sinLat0;

	  // values passed in through the constructor
	  // need for constructCopy
	  private double _lat0;

	  public override ProjectionImpl constructCopy()
	  {
		ProjectionImpl result = new Orthographic(OriginLat, OriginLon, R);
		result.DefaultMapArea = defaultMapArea;
		result.Name = name;
		return result;
	  }

	  /// <summary>
	  /// Constructor with default parameters
	  /// </summary>
	  public Orthographic() : this(0.0, 0.0)
	  {
	  }

	  /// <summary>
	  /// Construct a Orthographic Projection.
	  /// </summary>
	  /// <param name="lat0"> lat origin of the coord. system on the projection plane </param>
	  /// <param name="lon0"> lon origin of the coord. system on the projection plane </param>
	  public Orthographic(double lat0, double lon0) : this(lat0, lon0, EARTH_RADIUS)
	  {
	  }

	  /// <summary>
	  /// Construct a Orthographic Projection, specify earth radius
	  /// </summary>
	  /// <param name="lat0"> lat origin of the coord. system on the projection plane </param>
	  /// <param name="lon0"> lon origin of the coord. system on the projection plane </param>
	  /// <param name="earthRadius"> radius of the earth </param>
	  /// <exception cref="IllegalArgumentException"> if lat0, par1, par2 = +/-90 deg </exception>
	  public Orthographic(double lat0, double lon0, double earthRadius) : base("Orthographic", false)
	  {

		this._lat0 = lat0;
		this.lat0 = ((lat0) * (Math.PI / 180.0));
		this.lon0 = ((lon0) * (Math.PI / 180.0));
		R = earthRadius;

		precalculate();

		addParameter(CF.GRID_MAPPING_NAME, CF.ORTHOGRAPHIC);
		addParameter(CF.LATITUDE_OF_PROJECTION_ORIGIN, lat0);
		addParameter(CF.LONGITUDE_OF_PROJECTION_ORIGIN, lon0);
		addParameter(CF.EARTH_RADIUS, earthRadius * 1000);
	  }

	  // move this to ucar.unit or NetcdfSharp.ucar.unidata.util

	  /// <summary>
	  /// Precalculate some stuff
	  /// </summary>
	  private void precalculate()
	  {
		sinLat0 = Math.Sin(lat0);
		cosLat0 = Math.Cos(lat0);
		lon0Degrees = ((lon0) * (180.0 / Math.PI));
	  }

	  public override bool Equals(object o)
	  {
		if (this == o)
		{
		  return true;
		}
		if (o == null || this.GetType() != o.GetType())
		{
		  return false;
		}

		Orthographic that = (Orthographic) o;

		if (that.R.CompareTo(R) != 0)
		{
		  return false;
		}
		if (that.lat0.CompareTo(lat0) != 0)
		{
		  return false;
		}
		if (that.lon0.CompareTo(lon0) != 0)
		{
		  return false;
		}
		if ((defaultMapArea == null) != (that.defaultMapArea == null))
		{
		  return false; // common case is that these are null
		}
		return defaultMapArea == null || that.defaultMapArea.Equals(defaultMapArea);

	  }

	  public override int GetHashCode()
	  {
		int result;
		long temp;
		temp = lat0 != 0.0d ? System.BitConverter.DoubleToInt64Bits(lat0) : 0L;
		result = (int)(temp ^ (temp >>> 32));
		temp = lon0 != 0.0d ? System.BitConverter.DoubleToInt64Bits(lon0) : 0L;
		result = 31 * result + (int)(temp ^ (temp >>> 32));
		temp = R != 0.0d ? System.BitConverter.DoubleToInt64Bits(R) : 0L;
		result = 31 * result + (int)(temp ^ (temp >>> 32));
		return result;
	  }

	  // bean properties

	  /// <summary>
	  /// Get the origin longitude in degrees
	  /// </summary>
	  /// <returns> the origin longitude. </returns>
	  public virtual double OriginLon
	  {
		  get
		  {
			return lon0Degrees;
		  }
		  set
		  {
			lon0Degrees = value;
			lon0 = ((value) * (Math.PI / 180.0));
			precalculate();
		  }
	  }

	  /// <summary>
	  /// Get the origin latitude in degrees
	  /// </summary>
	  /// <returns> the origin latitude. </returns>
	  public virtual double OriginLat
	  {
		  get
		  {
			return _lat0;
		  }
		  set
		  {
			_lat0 = value;
			lat0 = ((value) * (Math.PI / 180.0));
			precalculate();
		  }
	  }

	  //////////////////////////////////////////////
	  // setters for IDV serialization - do not use except for object creating



	  //////////////////////////////////////////////

	  /// <summary>
	  /// Get the label to be used in the gui for this type of projection
	  /// </summary>
	  /// <returns> Type label </returns>
	  public override string ProjectionTypeLabel
	  {
		  get
		  {
			return "Orthographic";
		  }
	  }

	  /// <summary>
	  /// Create a String of the parameters.
	  /// </summary>
	  /// <returns> a String of the parameters </returns>
	  public override string paramsToString()
	  {
		return ToString();
	  }

	  public override string ToString()
	  {
		return "Orthographic{" + "lat0=" + _lat0 + ", lon0=" + lon0Degrees + ", R=" + R + '}';
	  }

	  /// <summary>
	  /// This returns true when the line between pt1 and pt2 crosses the seam.
	  /// When the cone is flattened, the "seam" is lon0 +- 180.
	  /// </summary>
	  /// <param name="pt1"> point 1 </param>
	  /// <param name="pt2"> point 2 </param>
	  /// <returns> true when the line between pt1 and pt2 crosses the seam. </returns>
	  public override bool crossSeam(ProjectionPoint pt1, ProjectionPoint pt2)
	  {
		// either point is infinite
		if (LatLonPoints.isInfinite(pt1) || LatLonPoints.isInfinite(pt2))
		{
		  return true;
		}

		// opposite signed X values, larger then 5000 km
		return (pt1.X * pt2.X < 0) && (Math.Abs(pt1.X - pt2.X) > 5000.0);
	  }

	  public override ProjectionPoint latLonToProj(LatLonPoint latLon, ProjectionPointImpl result)
	  {
		double toX, toY;
		double fromLat = latLon.Latitude;
		double fromLon = latLon.Longitude;


		fromLat = ((fromLat) * (Math.PI / 180.0));
		double lonDiff = ((LatLonPoints.lonNormal(fromLon - lon0Degrees)) * (Math.PI / 180.0));
		double cosc = sinLat0 * Math.Sin(fromLat) + cosLat0 * Math.Cos(fromLat) * Math.Cos(lonDiff);
		if (cosc >= 0)
		{
		  toX = R * Math.Cos(fromLat) * Math.Sin(lonDiff);
		  toY = R * (cosLat0 * Math.Sin(fromLat) - sinLat0 * Math.Cos(fromLat) * Math.Cos(lonDiff));
		}
		else
		{
		  toX = double.PositiveInfinity;
		  toY = double.PositiveInfinity;
		}

		result.setLocation(toX, toY);
		return result;
	  }

	  public override LatLonPoint projToLatLon(ProjectionPoint world, LatLonPointImpl result)
	  {
		double toLat, toLon;
		double fromX = world.X;
		double fromY = world.Y;

		double rho = Math.Sqrt(fromX * fromX + fromY * fromY);
		double c = Math.Asin(rho / R);

		toLon = lon0;
		double temp = 0;
		if (Math.Abs(rho) > TOLERANCE)
		{
		  toLat = Math.Asin(Math.Cos(c) * sinLat0 + (fromY * Math.Sin(c) * cosLat0 / rho));
		  if (Math.Abs(lat0 - Math.PI / 4.0) > TOLERANCE)
		  { // not 90 or -90
			temp = rho * cosLat0 * Math.Cos(c) - fromY * sinLat0 * Math.Sin(c);
			toLon = lon0 + Math.Atan(fromX * Math.Sin(c) / temp);
		  }
		  else if (lat0.CompareTo(Math.PI / 4.0) == 0)
		  {
			toLon = lon0 + Math.Atan(fromX / -fromY);
			temp = -fromY;
		  }
		  else
		  {
			toLon = lon0 + Math.Atan(fromX / fromY);
			temp = fromY;
		  }
		}
		else
		{
		  toLat = lat0;
		}
		toLat = ((toLat) * (180.0 / Math.PI));
		toLon = ((toLon) * (180.0 / Math.PI));
		if (temp < 0)
		{
		  toLon += 180;
		}
		toLon = LatLonPoints.lonNormal(toLon);

		result.Latitude = toLat;
		result.Longitude = toLon;
		return result;
	  }

	  /// <summary>
	  /// Convert lat/lon coordinates to projection coordinates.
	  /// </summary>
	  /// <param name="from"> array of lat/lon coordinates: from[2][n],
	  ///        where from[0][i], from[1][i] is the (lat,lon)
	  ///        coordinate of the ith point </param>
	  /// <param name="to"> resulting array of projection coordinates,
	  ///        where to[0][i], to[1][i] is the (x,y) coordinate
	  ///        of the ith point </param>
	  /// <param name="latIndex"> index of latitude in "from" </param>
	  /// <param name="lonIndex"> index of longitude in "from" </param>
	  /// <returns> the "to" array. </returns>
	  public override float[][] latLonToProj(float[][] from, float[][] to, int latIndex, int lonIndex)
	  {
		int cnt = from[0].Length;
		float[] fromLatA = from[latIndex];
		float[] fromLonA = from[lonIndex];
		float[] resultXA = to[INDEX_X];
		float[] resultYA = to[INDEX_Y];
		double toX, toY;

		for (int i = 0; i < cnt; i++)
		{
		  double fromLat = fromLatA[i];
		  double fromLon = fromLonA[i];

		  fromLat = ((fromLat) * (Math.PI / 180.0));
		  double lonDiff = ((LatLonPointImpl.lonNormal(fromLon - lon0Degrees)) * (Math.PI / 180.0));
		  double cosc = sinLat0 * Math.Sin(fromLat) + cosLat0 * Math.Cos(fromLat) * Math.Cos(lonDiff);
		  if (cosc >= 0)
		  {
			toX = R * Math.Cos(fromLat) * Math.Sin(lonDiff);
			toY = R * (cosLat0 * Math.Sin(fromLat) - sinLat0 * Math.Cos(fromLat) * Math.Cos(lonDiff));
		  }
		  else
		  {
			toX = double.PositiveInfinity;
			toY = double.PositiveInfinity;
		  }

		  resultXA[i] = (float) toX;
		  resultYA[i] = (float) toY;
		}
		return to;
	  }

	  /// <summary>
	  /// Convert lat/lon coordinates to projection coordinates.
	  /// </summary>
	  /// <param name="from"> array of lat/lon coordinates: from[2][n], where
	  ///        (from[0][i], from[1][i]) is the (lat,lon) coordinate
	  ///        of the ith point </param>
	  /// <param name="to"> resulting array of projection coordinates: to[2][n]
	  ///        where (to[0][i], to[1][i]) is the (x,y) coordinate
	  ///        of the ith point </param>
	  /// <returns> the "to" array </returns>
	  public override float[][] projToLatLon(float[][] from, float[][] to)
	  {
		int cnt = from[0].Length;
		float[] fromXA = from[INDEX_X];
		float[] fromYA = from[INDEX_Y];
		float[] toLatA = to[INDEX_LAT];
		float[] toLonA = to[INDEX_LON];

		double toLat, toLon;
		for (int i = 0; i < cnt; i++)
		{
		  double fromX = fromXA[i];
		  double fromY = fromYA[i];

		  double rho = Math.Sqrt(fromX * fromX + fromY * fromY);
		  double c = Math.Asin(rho / R);

		  toLon = lon0;
		  double temp = 0;
		  if (Math.Abs(rho) > TOLERANCE)
		  {
			toLat = Math.Asin(Math.Cos(c) * sinLat0 + (fromY * Math.Sin(c) * cosLat0 / rho));
			if (Math.Abs(lat0 - Math.PI / 4.0) > TOLERANCE)
			{ // not 90 or -90
			  temp = rho * cosLat0 * Math.Cos(c) - fromY * sinLat0 * Math.Sin(c);
			  toLon = lon0 + Math.Atan(fromX * Math.Sin(c) / temp);
			}
			else if (lat0.CompareTo(Math.PI / 4.0) == 0)
			{
			  toLon = lon0 + Math.Atan(fromX / -fromY);
			  temp = -fromY;
			}
			else
			{
			  toLon = lon0 + Math.Atan(fromX / fromY);
			  temp = fromY;
			}
		  }
		  else
		  {
			toLat = lat0;
		  }
		  toLat = ((toLat) * (180.0 / Math.PI));
		  toLon = ((toLon) * (180.0 / Math.PI));
		  if (temp < 0)
		  {
			toLon += 180;
		  }
		  toLon = LatLonPoints.lonNormal(toLon);

		  toLatA[i] = (float) toLat;
		  toLonA[i] = (float) toLon;
		}
		return to;
	  }

	  /// <summary>
	  /// Convert lat/lon coordinates to projection coordinates.
	  /// </summary>
	  /// <param name="from"> array of lat/lon coordinates: from[2][n],
	  ///        where from[0][i], from[1][i] is the (lat,lon)
	  ///        coordinate of the ith point </param>
	  /// <param name="to"> resulting array of projection coordinates,
	  ///        where to[0][i], to[1][i] is the (x,y) coordinate
	  ///        of the ith point </param>
	  /// <param name="latIndex"> index of latitude in "from" </param>
	  /// <param name="lonIndex"> index of longitude in "from" </param>
	  /// <returns> the "to" array. </returns>
	  public override double[][] latLonToProj(double[][] from, double[][] to, int latIndex, int lonIndex)
	  {
		int cnt = from[0].Length;
		double[] fromLatA = from[latIndex];
		double[] fromLonA = from[lonIndex];
		double[] resultXA = to[INDEX_X];
		double[] resultYA = to[INDEX_Y];
		double toX, toY;

		for (int i = 0; i < cnt; i++)
		{
		  double fromLat = fromLatA[i];
		  double fromLon = fromLonA[i];

		  fromLat = ((fromLat) * (Math.PI / 180.0));
		  double lonDiff = ((LatLonPointImpl.lonNormal(fromLon - lon0Degrees)) * (Math.PI / 180.0));
		  double cosc = sinLat0 * Math.Sin(fromLat) + cosLat0 * Math.Cos(fromLat) * Math.Cos(lonDiff);
		  if (cosc >= 0)
		  {
			toX = R * Math.Cos(fromLat) * Math.Sin(lonDiff);
			toY = R * (cosLat0 * Math.Sin(fromLat) - sinLat0 * Math.Cos(fromLat) * Math.Cos(lonDiff));
		  }
		  else
		  {
			toX = double.PositiveInfinity;
			toY = double.PositiveInfinity;
		  }

		  resultXA[i] = toX;
		  resultYA[i] = toY;
		}
		return to;
	  }

	  /// <summary>
	  /// Convert lat/lon coordinates to projection coordinates.
	  /// </summary>
	  /// <param name="from"> array of lat/lon coordinates: from[2][n], where
	  ///        (from[0][i], from[1][i]) is the (lat,lon) coordinate
	  ///        of the ith point </param>
	  /// <param name="to"> resulting array of projection coordinates: to[2][n]
	  ///        where (to[0][i], to[1][i]) is the (x,y) coordinate
	  ///        of the ith point </param>
	  /// <returns> the "to" array </returns>
	  public override double[][] projToLatLon(double[][] from, double[][] to)
	  {
		int cnt = from[0].Length;
		double[] fromXA = from[INDEX_X];
		double[] fromYA = from[INDEX_Y];
		double[] toLatA = to[INDEX_LAT];
		double[] toLonA = to[INDEX_LON];

		double toLat, toLon;
		for (int i = 0; i < cnt; i++)
		{
		  double fromX = fromXA[i];
		  double fromY = fromYA[i];

		  double rho = Math.Sqrt(fromX * fromX + fromY * fromY);
		  double c = Math.Asin(rho / R);

		  toLon = lon0;
		  double temp = 0;
		  if (Math.Abs(rho) > TOLERANCE)
		  {
			toLat = Math.Asin(Math.Cos(c) * sinLat0 + (fromY * Math.Sin(c) * cosLat0 / rho));
			if (Math.Abs(lat0 - Math.PI / 4.0) > TOLERANCE)
			{ // not 90 or -90
			  temp = rho * cosLat0 * Math.Cos(c) - fromY * sinLat0 * Math.Sin(c);
			  toLon = lon0 + Math.Atan(fromX * Math.Sin(c) / temp);
			}
			else if (lat0.CompareTo(Math.PI / 4.0) == 0)
			{
			  toLon = lon0 + Math.Atan(fromX / -fromY);
			  temp = -fromY;
			}
			else
			{
			  toLon = lon0 + Math.Atan(fromX / fromY);
			  temp = fromY;
			}
		  }
		  else
		  {
			toLat = lat0;
		  }
		  toLat = ((toLat) * (180.0 / Math.PI));
		  toLon = ((toLon) * (180.0 / Math.PI));
		  if (temp < 0)
		  {
			toLon += 180;
		  }
		  toLon = LatLonPoints.lonNormal(toLon);

		  toLatA[i] = toLat;
		  toLonA[i] = toLon;
		}
		return to;
	  }

	}


}
