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
	using CDM = NetcdfSharp.ucar.nc2.constants.CDM;
	using CF = NetcdfSharp.ucar.nc2.constants.CF;
	using NetcdfSharp.ucar.unidata.geoloc;
	using Parameter = NetcdfSharp.ucar.unidata.util.Parameter;

	/// <summary>
	/// Albers Equal Area Conic Projection, one or two standard parallels,
	/// spherical earth.
	/// See John Snyder, Map Projections used by the USGS, Bulletin 1532,
	/// 2nd edition (1983), p 98
	/// 
	/// @author Unidata Development Team </summary>
	/// <seealso cref="Projection"/>
	/// <seealso cref="ProjectionImpl"/>

	[Serializable]
	public class AlbersEqualArea : ProjectionImpl
	{

	  private double lat0, lon0; // radians
	  private double par1, par2; // degrees
	  private double falseEasting, falseNorthing;
	  private readonly double earth_radius; // radius in km

	  // values passed in through the constructor
	  // need for constructCopy
	  private double _lat0, _lon0;

	  /// <summary>
	  /// constants from Snyder's equations
	  /// </summary>
	  private double n, C, rho0, lon0Degrees;

	  /// <summary>
	  /// copy constructor - avoid clone !!
	  /// </summary>
	  public override ProjectionImpl constructCopy()
	  {
		ProjectionImpl result = new AlbersEqualArea(OriginLat, OriginLon, ParallelOne, ParallelTwo, falseEasting, falseNorthing, EarthRadius);
		result.DefaultMapArea = defaultMapArea;
		result.Name = name;
		return result;
	  }

	  /// <summary>
	  /// Constructor with default parameters
	  /// </summary>
	  public AlbersEqualArea() : this(23, -96, 29.5, 45.5)
	  {
	  }

	  /// <summary>
	  /// Construct a AlbersEqualArea Projection, two standard parellels.
	  /// For the one standard parellel case, set them both to the same value.
	  /// </summary>
	  /// <param name="lat0"> lat origin of the coord. system on the projection plane </param>
	  /// <param name="lon0"> lon origin of the coord. system on the projection plane </param>
	  /// <param name="par1"> standard parallel 1 </param>
	  /// <param name="par2"> standard parallel 2 </param>
	  /// <exception cref="IllegalArgumentException"> if lat0, par1, par2 = +/-90 deg </exception>
	  public AlbersEqualArea(double lat0, double lon0, double par1, double par2) : this(lat0, lon0, par1, par2, 0, 0, Earth.WGS84_EARTH_RADIUS_KM)
	  {
	  }

	  /// <summary>
	  /// Construct a AlbersEqualArea Projection, two standard parellels.
	  /// For the one standard parellel case, set them both to the same value.
	  /// </summary>
	  /// <param name="lat0"> lat origin of the coord. system on the projection plane </param>
	  /// <param name="lon0"> lon origin of the coord. system on the projection plane </param>
	  /// <param name="par1"> standard parallel 1 </param>
	  /// <param name="par2"> standard parallel 2 </param>
	  /// <param name="falseEasting"> false easting in km </param>
	  /// <param name="falseNorthing"> false easting in km </param>
	  /// <exception cref="IllegalArgumentException"> if lat0, par1, par2 = +/-90 deg </exception>
	  public AlbersEqualArea(double lat0, double lon0, double par1, double par2, double falseEasting, double falseNorthing) : this(lat0, lon0, par1, par2, falseEasting, falseNorthing, Earth.WGS84_EARTH_RADIUS_KM)
	  {
	  }

	  /// <summary>
	  /// Construct a AlbersEqualArea Projection, two standard parellels.
	  /// For the one standard parellel case, set them both to the same value.
	  /// </summary>
	  /// <param name="lat0"> lat origin of the coord. system on the projection plane </param>
	  /// <param name="lon0"> lon origin of the coord. system on the projection plane </param>
	  /// <param name="par1"> standard parallel 1 </param>
	  /// <param name="par2"> standard parallel 2 </param>
	  /// <param name="falseEasting"> false easting in km </param>
	  /// <param name="falseNorthing"> false easting in km </param>
	  /// <param name="earth_radius"> radius of the earth in km </param>
	  /// <exception cref="IllegalArgumentException"> if lat0, par1, par2 = +/-90 deg </exception>
	  public AlbersEqualArea(double lat0, double lon0, double par1, double par2, double falseEasting, double falseNorthing, double earth_radius) : base("AlbersEqualArea", false)
	  {

		this._lat0 = lat0;
		this._lon0 = lon0;

		this.lat0 = ((lat0) * (Math.PI / 180.0));
		lon0Degrees = lon0;
		this.lon0 = ((lon0) * (Math.PI / 180.0));

		this.par1 = par1;
		this.par2 = par2;

		this.falseEasting = falseEasting;
		this.falseNorthing = falseNorthing;
		this.earth_radius = earth_radius;

		precalculate();

		addParameter(CF.GRID_MAPPING_NAME, CF.ALBERS_CONICAL_EQUAL_AREA);
		addParameter(CF.LATITUDE_OF_PROJECTION_ORIGIN, lat0);
		addParameter(CF.LONGITUDE_OF_CENTRAL_MERIDIAN, lon0);

		if (par2 == par1)
		{
		  addParameter(CF.STANDARD_PARALLEL, par1);
		}
		else
		{
		  double[] data = new double[2];
		  data[0] = par1;
		  data[1] = par2;
		  addParameter(new Parameter(CF.STANDARD_PARALLEL, data));
		}

		if ((falseEasting != 0.0) || (falseNorthing != 0.0))
		{
		  addParameter(CF.FALSE_EASTING, falseEasting);
		  addParameter(CF.FALSE_NORTHING, falseNorthing);
		  addParameter(CDM.UNITS, "km");
		}

		addParameter(CF.EARTH_RADIUS, earth_radius * 1000); // must be in meters
	  }

	  /// <summary>
	  /// Precalculate some stuff
	  /// </summary>
	  private void precalculate()
	  {

		double par1r = ((this.par1) * (Math.PI / 180.0));
		double par2r = ((this.par2) * (Math.PI / 180.0));

		if (Math.Abs(par2 - par1) < TOLERANCE)
		{ // single parallel
		  n = Math.Sin(par1r);
		}
		else
		{
		  n = (Math.Sin(par1r) + Math.Sin(par2r)) / 2.0;
		}

		double c2 = Math.Pow(Math.Cos(par1r), 2);
		C = c2 + 2 * n * Math.Sin(par1r);
		rho0 = computeRho(lat0);

	  }

	  /// <summary>
	  /// Compute the RHO parameter
	  /// </summary>
	  /// <param name="lat"> latitude </param>
	  /// <returns> the rho parameter </returns>
	  private double computeRho(double lat)
	  {
		return earth_radius * Math.Sqrt(C - 2 * n * Math.Sin(lat)) / n;
	  }

	  /// <summary>
	  /// Compute theta
	  /// </summary>
	  /// <param name="lon"> longitude </param>
	  /// <returns> theta </returns>
	  private double computeTheta(double lon)
	  {
		double dlon = LatLonPoints.lonNormal(((lon) * (180.0 / Math.PI)) - lon0Degrees);
		return n * ((dlon) * (Math.PI / 180.0));
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

		AlbersEqualArea that = (AlbersEqualArea) o;

		if (that.earth_radius.CompareTo(earth_radius) != 0)
		{
		  return false;
		}
		if (that.falseEasting.CompareTo(falseEasting) != 0)
		{
		  return false;
		}
		if (that.falseNorthing.CompareTo(falseNorthing) != 0)
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
		if (that.par1.CompareTo(par1) != 0)
		{
		  return false;
		}
		if (that.par2.CompareTo(par2) != 0)
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
		temp = par1 != 0.0d ? System.BitConverter.DoubleToInt64Bits(par1) : 0L;
		result = 31 * result + (int)(temp ^ (temp >>> 32));
		temp = par2 != 0.0d ? System.BitConverter.DoubleToInt64Bits(par2) : 0L;
		result = 31 * result + (int)(temp ^ (temp >>> 32));
		temp = falseEasting != 0.0d ? System.BitConverter.DoubleToInt64Bits(falseEasting) : 0L;
		result = 31 * result + (int)(temp ^ (temp >>> 32));
		temp = falseNorthing != 0.0d ? System.BitConverter.DoubleToInt64Bits(falseNorthing) : 0L;
		result = 31 * result + (int)(temp ^ (temp >>> 32));
		temp = earth_radius != 0.0d ? System.BitConverter.DoubleToInt64Bits(earth_radius) : 0L;
		result = 31 * result + (int)(temp ^ (temp >>> 32));
		return result;
	  }

	  // bean properties

	  /// <summary>
	  /// Get the second standard parallel
	  /// </summary>
	  /// <returns> the second standard parallel </returns>
	  public virtual double ParallelTwo
	  {
		  get
		  {
			return par2;
		  }
		  set
		  {
			par2 = value;
			precalculate();
		  }
	  }

	  /// <summary>
	  /// Get the first standard parallel
	  /// </summary>
	  /// <returns> the first standard parallel </returns>
	  public virtual double ParallelOne
	  {
		  get
		  {
			return par1;
		  }
		  set
		  {
			par1 = value;
			precalculate();
		  }
	  }

	  /// <summary>
	  /// Get the origin longitude in degrees.
	  /// </summary>
	  /// <returns> the origin longitude in degrees. </returns>
	  public virtual double OriginLon
	  {
		  get
		  {
			return _lon0;
		  }
		  set
		  {
			_lon0 = value;
			lon0 = ((value) * (Math.PI / 180.0));
			precalculate();
		  }
	  }

	  /// <summary>
	  /// Get the origin latitude in degrees.
	  /// </summary>
	  /// <returns> the origin latitude in degrees. </returns>
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
	  /// Earth radius in km
	  /// </summary>
	  /// <returns> Earth radius in km </returns>
	  public virtual double EarthRadius
	  {
		  get
		  {
			return this.earth_radius;
		  }
	  }

	  /// <summary>
	  /// Get the label to be used in the gui for this type of projection
	  /// </summary>
	  /// <returns> Type label </returns>
	  public override string ProjectionTypeLabel
	  {
		  get
		  {
			return "Albers Equal Area";
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
		return "AlbersEqualArea{" + "lat0=" + _lat0 + ", lon0=" + _lon0 + ", par1=" + par1 + ", par2=" + par2 + ", falseEasting=" + falseEasting + ", falseNorthing=" + falseNorthing + ", earth_radius=" + earth_radius + '}';
	  }

	  /// <summary>
	  /// Get the scale at the given lat.
	  /// </summary>
	  /// <param name="lat"> lat to use </param>
	  /// <returns> scale factor at that latitude </returns>
	  public virtual double getScale(double lat)
	  {
		lat = ((lat) * (Math.PI / 180.0));
		double n = Math.Cos(lat);
		double d = Math.Sqrt(C - 2 * n * Math.Sin(lat));
		return n / d;
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

	  /*
	   * MACROBODY
	   * latLonToProj {} {
	   * fromLat = ((fromLat) * (Math.PI / 180.0));
	   * fromLon = ((fromLon) * (Math.PI / 180.0));
	   * double rho = computeRho(fromLat);
	   * double theta = computeTheta(fromLon);
	   *
	   * toX = rho * Math.sin(theta);
	   * toY = rho0 - rho*Math.cos(theta);
	   * }
	   * projToLatLon {double rrho0 = rho0;} {
	   * if (n < 0) {
	   * rrho0 *= -1.0;
	   * fromX *= -1.0;
	   * fromY *= -1.0;
	   * }
	   *
	   *
	   * double yd = rrho0-fromY;
	   * double rho = Math.sqrt(fromX * fromX + yd*yd);
	   * double theta = Math.atan2( fromX, yd);
	   * if (n < 0) rho *= -1.0;
	   *
	   * toLat = ((Math.asin((C-Math.pow((rho*n/EARTH_RADIUS),2))/(2*n))) * (180.0 / Math.PI));
	   *
	   * toLon = ((theta/n + lon0) * (180.0 / Math.PI));
	   *
	   * }
	   * MACROBODY
	   */

	  /* BEGINGENERATED */

	  /*
	   * Note this section has been generated using the convert.tcl script.
	   * This script, run as:
	   * tcl convert.tcl AlbersEqualArea.java
	   * takes the actual projection conversion code defined in the MACROBODY
	   * section above and generates the following 6 methods
	   */


	  /// <summary>
	  /// Convert a LatLonPoint to projection coordinates
	  /// </summary>
	  /// <param name="latLon"> convert from these lat, lon coordinates </param>
	  /// <param name="result"> the object to write to </param>
	  /// <returns> the given result </returns>
	  public override ProjectionPoint latLonToProj(LatLonPoint latLon, ProjectionPointImpl result)
	  {
		double toX, toY;
		double fromLat = latLon.Latitude;
		double fromLon = latLon.Longitude;

		fromLat = ((fromLat) * (Math.PI / 180.0));
		fromLon = ((fromLon) * (Math.PI / 180.0));
		double rho = computeRho(fromLat);
		double theta = computeTheta(fromLon);

		toX = rho * Math.Sin(theta) + falseEasting;
		toY = rho0 - rho * Math.Cos(theta) + falseNorthing;

		result.setLocation(toX, toY);
		return result;
	  }

	  /// <summary>
	  /// Convert projection coordinates to a LatLonPoint
	  /// Note: a new object is not created on each call for the return value.
	  /// </summary>
	  /// <param name="world"> convert from these projection coordinates </param>
	  /// <param name="result"> the object to write to </param>
	  /// <returns> LatLonPoint convert to these lat/lon coordinates </returns>
	  public override LatLonPoint projToLatLon(ProjectionPoint world, LatLonPointImpl result)
	  {
		double toLat, toLon;
		double fromX = world.X - falseEasting;
		double fromY = world.Y - falseNorthing;
		double rrho0 = rho0;

		if (n < 0)
		{
		  rrho0 *= -1.0;
		  fromX *= -1.0;
		  fromY *= -1.0;
		}

		double yd = rrho0 - fromY;
		double rho = Math.Sqrt(fromX * fromX + yd * yd);
		double theta = Math.Atan2(fromX, yd);
		if (n < 0)
		{
		  rho *= -1.0;
		}
		toLat = ((Math.Asin((C - Math.Pow((rho * n / earth_radius), 2)) / (2 * n))) * (180.0 / Math.PI));

		toLon = ((theta / n + lon0) * (180.0 / Math.PI));

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
		  fromLon = ((fromLon) * (Math.PI / 180.0));
		  double rho = computeRho(fromLat);
		  double theta = computeTheta(fromLon);

		  toX = rho * Math.Sin(theta);
		  toY = rho0 - rho * Math.Cos(theta);

		  resultXA[i] = (float)(toX + falseEasting);
		  resultYA[i] = (float)(toY + falseNorthing);
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
		double rrho0 = rho0;
		double toLat, toLon;
		for (int i = 0; i < cnt; i++)
		{
		  double fromX = fromXA[i] - falseEasting;
		  double fromY = fromYA[i] - falseNorthing;

		  if (n < 0)
		  {
			rrho0 *= -1.0;
			fromX *= -1.0;
			fromY *= -1.0;
		  }


		  double yd = rrho0 - fromY;
		  double rho = Math.Sqrt(fromX * fromX + yd * yd);
		  double theta = Math.Atan2(fromX, yd);
		  if (n < 0)
		  {
			rho *= -1.0;
		  }

		  toLat = ((Math.Asin((C - Math.Pow((rho * n / earth_radius), 2)) / (2 * n))) * (180.0 / Math.PI));

		  toLon = ((theta / n + lon0) * (180.0 / Math.PI));


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
		  fromLon = ((fromLon) * (Math.PI / 180.0));
		  double rho = computeRho(fromLat);
		  double theta = computeTheta(fromLon);

		  toX = rho * Math.Sin(theta);
		  toY = rho0 - rho * Math.Cos(theta);

		  resultXA[i] = toX + falseEasting;
		  resultYA[i] = toY + falseNorthing;
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
		double rrho0 = rho0;
		double toLat, toLon;
		for (int i = 0; i < cnt; i++)
		{
		  double fromX = fromXA[i] - falseEasting;
		  double fromY = fromYA[i] - falseNorthing;

		  if (n < 0)
		  {
			rrho0 *= -1.0;
			fromX *= -1.0;
			fromY *= -1.0;
		  }

		  double yd = rrho0 - fromY;
		  double rho = Math.Sqrt(fromX * fromX + yd * yd);
		  double theta = Math.Atan2(fromX, yd);
		  if (n < 0)
		  {
			rho *= -1.0;
		  }

		  toLat = ((Math.Asin((C - Math.Pow((rho * n / earth_radius), 2)) / (2 * n))) * (180.0 / Math.PI));

		  toLon = ((theta / n + lon0) * (180.0 / Math.PI));

		  toLatA[i] = toLat;
		  toLonA[i] = toLon;
		}
		return to;
	  }

	}

}
