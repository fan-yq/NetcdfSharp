// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Text;

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
	/// Lambert Conformal Projection, one or two standard parallels, spherical earth.
	/// Projection plane is a cone whose vertex lies on the line of the earth's axis,
	/// and intersects the earth at two parellels (par1, par2), or is tangent to the earth at
	/// one parellel par1 = par2. The cone is flattened by splitting along the longitude = lon0+180.
	/// <p/>
	/// See John Snyder, Map Projections used by the USGS, Bulletin 1532, 2nd edition (1983), p 104
	/// 
	/// @author John Caron </summary>
	/// <seealso cref="Projection"/>
	/// <seealso cref="ProjectionImpl"/>

	[Serializable]
	public class LambertConformal : ProjectionImpl
	{
	  private const string NAME = "LambertConformal";
	  private const string DEFAULT_UNITS = "km";

	  private readonly double earth_radius;
	  private double lat0, lon0; // lat/lon in radians
	  private double par1, par2; // standard parallel 1 and 2 degrees
	  private double falseEasting, falseNorthing;

	  // values passed in through the constructor
	  // need for constructCopy
	  private double _lat0, _lon0;

	  private double n, F, rho; // constants from Snyder's equations
	  private double earthRadiusTimesF; // Earth's radius time F
	  private double lon0Degrees; // lon naught ??

	  public override ProjectionImpl constructCopy()
	  {
		ProjectionImpl result = new LambertConformal(OriginLat, OriginLon, ParallelOne, ParallelTwo, FalseEasting, FalseNorthing, earth_radius);
		result.DefaultMapArea = defaultMapArea;
		result.Name = name;
		return result;
	  }

	  /// <summary>
	  /// Constructor with default parameters
	  /// </summary>
	  public LambertConformal() : this(40.0, -105.0, 20.0, 60.0, 0.0, 0.0, EARTH_RADIUS)
	  {
	  }

	  /// <summary>
	  /// Construct a LambertConformal Projection, two standard parellels.
	  /// For the one standard parellel case, set them both to the same value.
	  /// </summary>
	  /// <param name="lat0"> lat origin of the coord. system on the projection plane </param>
	  /// <param name="lon0"> lon origin of the coord. system on the projection plane </param>
	  /// <param name="par1"> standard parallel 1 </param>
	  /// <param name="par2"> standard parallel 2 </param>
	  /// <exception cref="IllegalArgumentException"> if lat0 > +/-90 or if par1, par2 >= +/-90 deg </exception>
	  public LambertConformal(double lat0, double lon0, double par1, double par2) : this(lat0, lon0, par1, par2, 0.0, 0.0, EARTH_RADIUS)
	  {
	  }

	  /// <summary>
	  /// Construct a LambertConformal Projection, two standard parellels.
	  /// For the one standard parellel case, set them both to the same value.
	  /// </summary>
	  /// <param name="lat0"> lat origin of the coord. system on the projection plane </param>
	  /// <param name="lon0"> lon origin of the coord. system on the projection plane </param>
	  /// <param name="par1"> standard parallel 1 </param>
	  /// <param name="par2"> standard parallel 2 </param>
	  /// <param name="false_easting"> natural_x_coordinate + false_easting = x coordinate in km </param>
	  /// <param name="false_northing"> natural_y_coordinate + false_northing = y coordinate in km </param>
	  /// <exception cref="IllegalArgumentException"> if lat0 > +/-90 or if par1, par2 >= +/-90 deg </exception>
	  public LambertConformal(double lat0, double lon0, double par1, double par2, double false_easting, double false_northing) : this(lat0, lon0, par1, par2, false_easting, false_northing, EARTH_RADIUS)
	  {

	  }

	  /// <summary>
	  /// Construct a LambertConformal Projection, two standard parellels.
	  /// For the one standard parellel case, set them both to the same value.
	  /// </summary>
	  /// <param name="lat0"> lat origin of the coord. system on the projection plane </param>
	  /// <param name="lon0"> lon origin of the coord. system on the projection plane </param>
	  /// <param name="par1"> standard parallel 1 </param>
	  /// <param name="par2"> standard parallel 2 </param>
	  /// <param name="false_easting"> natural_x_coordinate + false_easting = x coordinate in km </param>
	  /// <param name="false_northing"> natural_y_coordinate + false_northing = y coordinate in km </param>
	  /// <param name="earth_radius"> radius of the earth in km </param>
	  /// <exception cref="IllegalArgumentException"> if lat0 > +/-90 or if par1, par2 >= +/-90 deg </exception>
	  public LambertConformal(double lat0, double lon0, double par1, double par2, double false_easting, double false_northing, double earth_radius) : base(NAME, DEFAULT_UNITS, false)
	  {


		this._lat0 = lat0;
		this._lon0 = lon0;

		this.lat0 = ((lat0) * (Math.PI / 180.0));
		this.lon0 = ((lon0) * (Math.PI / 180.0));

		this.par1 = par1;
		this.par2 = par2;

		this.falseEasting = false_easting;
		this.falseNorthing = false_northing;
		this.earth_radius = earth_radius;

		precalculate();

		addParameter(CF.GRID_MAPPING_NAME, CF.LAMBERT_CONFORMAL_CONIC);
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
		if ((false_easting != 0.0) || (false_northing != 0.0))
		{
		  addParameter(CF.FALSE_EASTING, false_easting);
		  addParameter(CF.FALSE_NORTHING, false_northing);
		  addParameter(CDM.UNITS, "km");
		}
		addParameter(CF.EARTH_RADIUS, earth_radius * 1000);
	  }

	  /// <summary>
	  /// Precalculate some stuff
	  /// </summary>
	  private void precalculate()
	  {
		if (Math.Abs(lat0) > Math.PI / 2.0)
		{
		  throw new ArgumentException("LambertConformal lat0 outside range (-90,90)");
		}
		if (Math.Abs(par1) >= 90.0)
		{
		  throw new ArgumentException("LambertConformal abs(par1) >= 90");
		}
		if (Math.Abs(par2) >= 90.0)
		{
		  throw new ArgumentException("LambertConformal abs(par2) >= 90");
		}

		if (Math.Abs(par1 - 90.0) < TOLERANCE)
		{
		  throw new ArgumentException("LambertConformal par1 = 90");
		}
		if (Math.Abs(par1 + 90.0) < TOLERANCE)
		{
		  throw new ArgumentException("LambertConformal par1 = -90");
		}
		if (Math.Abs(par2 - 90.0) < TOLERANCE)
		{
		  throw new ArgumentException("LambertConformal par2 = 90");
		}
		if (Math.Abs(par2 + 90.0) < TOLERANCE)
		{
		  throw new ArgumentException("LambertConformal par2 = -90");
		}

		double par1r = ((this.par1) * (Math.PI / 180.0));
		double par2r = ((this.par2) * (Math.PI / 180.0));

		double t1 = Math.Tan(Math.PI / 4 + par1r / 2);
		double t2 = Math.Tan(Math.PI / 4 + par2r / 2);

		if (Math.Abs(par2 - par1) < TOLERANCE)
		{ // single parallel
		  n = Math.Sin(par1r);
		}
		else
		{
		  n = Math.Log(Math.Cos(par1r) / Math.Cos(par2r)) / Math.Log(t2 / t1);
		}

		double t1n = Math.Pow(t1, n);
		F = Math.Cos(par1r) * t1n / n;
		earthRadiusTimesF = earth_radius * F;

		double t0n = Math.Pow(Math.Tan(Math.PI / 4 + lat0 / 2), n);
		rho = earthRadiusTimesF / t0n;

		lon0Degrees = ((lon0) * (180.0 / Math.PI));
		// need to know the pole value for crossSeam
		// Point2D pt = latLonToProj( 90.0, 0.0);
		// maxY = pt.getY();
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

		LambertConformal that = (LambertConformal) o;

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
		temp = earth_radius != 0.0d ? System.BitConverter.DoubleToInt64Bits(earth_radius) : 0L;
		result = (int)(temp ^ (temp >>> 32));
		temp = lat0 != 0.0d ? System.BitConverter.DoubleToInt64Bits(lat0) : 0L;
		result = 31 * result + (int)(temp ^ (temp >>> 32));
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
			this.par2 = value;
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
			this.par1 = value;
			precalculate();
		  }
	  }

	  /// <summary>
	  /// Get the origin longitude in degrees
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
			this.lon0 = ((value) * (Math.PI / 180.0));
			precalculate();
		  }
	  }

	  /// <summary>
	  /// Get the origin latitude in degrees
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
			this.lat0 = ((value) * (Math.PI / 180.0));
			precalculate();
		  }
	  }

	  /// <summary>
	  /// Get the false easting, in km.
	  /// </summary>
	  /// <returns> the false easting. </returns>
	  public virtual double FalseEasting
	  {
		  get
		  {
			return falseEasting;
		  }
		  set
		  {
			this.falseEasting = value;
		  }
	  }

	  /// <summary>
	  /// Get the false northing, in km.
	  /// </summary>
	  /// <returns> the false northing. </returns>
	  public virtual double FalseNorthing
	  {
		  get
		  {
			return falseNorthing;
		  }
		  set
		  {
			this.falseNorthing = value;
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
			return "Lambert conformal conic";
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
		return "LambertConformal{" + "earth_radius=" + earth_radius + ", lat0=" + _lat0 + ", lon0=" + _lon0 + ", par1=" + par1 + ", par2=" + par2 + ", falseEasting=" + falseEasting + ", falseNorthing=" + falseNorthing + '}';
	  }

	  /// <summary>
	  /// Create a WKS string
	  /// </summary>
	  /// <returns> WKS string </returns>
	  public virtual string toWKS()
	  {
		StringBuilder sbuff = new StringBuilder();
		sbuff.Append("PROJCS[\"").Append(Name).Append("\",");
		// Java keeps both WKT formats behind `if (true)` for easy switching; the else is intentionally unreachable.
#pragma warning disable CS0162
		if (true)
		{
		  sbuff.Append("GEOGCS[\"Normal Sphere (r=6371007)\",");
		  sbuff.Append("DATUM[\"unknown\",");
		  sbuff.Append("SPHEROID[\"sphere\",6371007,0]],");
		}
		else
		{
		  sbuff.Append("GEOGCS[\"WGS 84\",");
		  sbuff.Append("DATUM[\"WGS_1984\",");
		  sbuff.Append("SPHEROID[\"WGS 84\",6378137,298.257223563],");
		  sbuff.Append("TOWGS84[0,0,0,0,0,0,0]],");
		}
#pragma warning restore CS0162
		sbuff.Append("PRIMEM[\"Greenwich\",0],");
		sbuff.Append("UNIT[\"degree\",0.0174532925199433]],");
		sbuff.Append("PROJECTION[\"Lambert_Conformal_Conic_1SP\"],");
		sbuff.Append("PARAMETER[\"latitude_of_origin\",").Append(OriginLat).Append("],"); // LOOK assumes getOriginLat
																							   // = getParellel
		sbuff.Append("PARAMETER[\"central_meridian\",").Append(OriginLon).Append("],");
		sbuff.Append("PARAMETER[\"scale_factor\",1],");
		sbuff.Append("PARAMETER[\"false_easting\",").Append(falseEasting).Append("],");
		sbuff.Append("PARAMETER[\"false_northing\",").Append(falseNorthing).Append("],");

		return sbuff.ToString();
	  }

	  /// <summary>
	  /// Get the scale at the given lat.
	  /// </summary>
	  /// <param name="lat"> lat to use </param>
	  /// <returns> scale factor at this latitude </returns>
	  public virtual double getScale(double lat)
	  {
		lat = ((lat) * (Math.PI / 180.0));
		double t = Math.Tan(Math.PI / 4 + lat / 2);
		double tn = Math.Pow(t, n);
		double r1 = n * F;
		double r2 = Math.Cos(lat) * tn;
		return r1 / r2;
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
		// opposite signed X values, larger then 20,000 km
		return (pt1.X * pt2.X < 0) && (Math.Abs(pt1.X - pt2.X) > 20000.0);
	  }

	  /*
	   * MACROBODY
	   * latLonToProj {} {
	   * fromLat = ((fromLat) * (Math.PI / 180.0));
	   * double dlon = LatLonPoints.lonNormal(fromLon - lon0Degrees);
	   * double theta = n * ((dlon) * (Math.PI / 180.0));
	   * double tn = Math.pow( Math.tan(PI_OVER_4 + fromLat/2), n);
	   * double r = earthRadiusTimesF / tn;
	   * toX = r * Math.sin(theta);
	   * toY = rho - r * Math.cos(theta);
	   * }
	   * projToLatLon {} {
	   * double rhop = rho;
	   * if (n < 0) {
	   * rhop *= -1.0;
	   * fromX *= -1.0;
	   * fromY *= -1.0;
	   * }
	   *
	   * double yd = (rhop - fromY);
	   * double theta = Math.atan2( fromX, yd);
	   * double r = Math.sqrt( fromX*fromX + yd*yd);
	   * if (n < 0.0)
	   * r *= -1.0;
	   *
	   * toLon = (((theta/n + lon0) * (180.0 / Math.PI)));
	   *
	   * if (Math.abs(r) < TOLERANCE) {
	   * toLat = ((n < 0.0) ? -90.0 : 90.0);
	   * } else {
	   * double rn = Math.pow( EARTH_RADIUS * F / r, 1/n);
	   * toLat = ((2.0 * Math.atan( rn) - Math.PI/2) * (180.0 / Math.PI));
	   * }
	   * }
	   *
	   * MACROBODY
	   */

	  /* BEGINGENERATED */

	  /*
	   * Note this section has been generated using the convert.tcl script.
	   * This script, run as:
	   * tcl convert.tcl LambertConformal.java
	   * takes the actual projection conversion code defined in the MACROBODY
	   * section above and generates the following 6 methods
	   */


	  /// <summary>
	  /// Convert a LatLonPoint to projection coordinates in km
	  /// </summary>
	  /// <param name="latLon"> convert from these lat, lon coordinates </param>
	  /// <param name="result"> the object to write to </param>
	  /// <returns> the given result in km </returns>
	  public override ProjectionPoint latLonToProj(LatLonPoint latLon, ProjectionPointImpl result)
	  {
		double toX, toY;
		double fromLat = latLon.Latitude;
		double fromLon = latLon.Longitude;


		fromLat = ((fromLat) * (Math.PI / 180.0));
		double dlon = LatLonPoints.lonNormal(fromLon - lon0Degrees);
		double theta = n * ((dlon) * (Math.PI / 180.0));
		double tn = Math.Pow(Math.Tan(Math.PI / 4.0 + fromLat / 2), n);
		double r = earthRadiusTimesF / tn;
		toX = r * Math.Sin(theta);
		toY = rho - r * Math.Cos(theta);

		result.setLocation(toX + falseEasting, toY + falseNorthing);
		return result;
	  }

	  /// <summary>
	  /// Convert projection coordinates to latitude and longitude
	  /// </summary>
	  /// <param name="world"> projection coordinates in km </param>
	  /// <param name="result"> the object to write to </param>
	  /// <returns> the resulting latitude and longitude </returns>
	  public override LatLonPoint projToLatLon(ProjectionPoint world, LatLonPointImpl result)
	  {
		double toLat, toLon;
		double fromX = world.X - falseEasting;
		double fromY = world.Y - falseNorthing;
		double rhop = rho;

		if (n < 0)
		{
		  rhop *= -1.0;
		  fromX *= -1.0;
		  fromY *= -1.0;
		}

		double yd = (rhop - fromY);
		double theta = Math.Atan2(fromX, yd);
		double r = Math.Sqrt(fromX * fromX + yd * yd);
		if (n < 0.0)
		{
		  r *= -1.0;
		}

		toLon = (((theta / n + lon0) * (180.0 / Math.PI)));

		if (Math.Abs(r) < TOLERANCE)
		{
		  toLat = ((n < 0.0) ? -90.0 : 90.0);
		}
		else
		{
		  double rn = Math.Pow(earth_radius * F / r, 1 / n);
		  toLat = ((2.0 * Math.Atan(rn) - Math.PI / 2) * (180.0 / Math.PI));
		}

		result.Latitude = toLat;
		result.Longitude = toLon;
		return result;
	  }

	  /// <summary>
	  /// Convert lat/lon coordinates to projection coordinates.
	  /// </summary>
	  /// <param name="from"> array of lat/lon coordinates: from[2][n], where
	  ///        (from[latIndex][i], from[lonIndex][i]) is the (lat,lon)
	  ///        coordinate of the ith point </param>
	  /// <param name="to"> resulting array of projection coordinates: to[2][n]
	  ///        where (to[0][i], to[1][i]) is the (x,y) coordinate of
	  ///        the ith point </param>
	  /// <param name="latIndex"> index of lat coordinate; must be 0 or 1 </param>
	  /// <param name="lonIndex"> index of lon coordinate; must be 0 or 1 </param>
	  /// <returns> the "to" array </returns>
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
		  double dlon = LatLonPoints.lonNormal(fromLon - lon0Degrees);
		  double theta = n * ((dlon) * (Math.PI / 180.0));
		  double tn = Math.Pow(Math.Tan(Math.PI / 4.0 + fromLat / 2), n);
		  double r = earthRadiusTimesF / tn;
		  toX = r * Math.Sin(theta);
		  toY = rho - r * Math.Cos(theta);

		  resultXA[i] = (float)(toX + falseEasting);
		  resultYA[i] = (float)(toY + falseNorthing);
		}
		return to;
	  }

	  /// <summary>
	  /// Convert projection coordinates to lat/lon coordinate.
	  /// </summary>
	  /// <param name="from"> array of projection coordinates: from[2][n], where
	  ///        (from[0][i], from[1][i]) is the (x, y) coordinate
	  ///        of the ith point </param>
	  /// <param name="to"> resulting array of lat/lon coordinates: to[2][n] where
	  ///        (to[0][i], to[1][i]) is the (lat, lon) coordinate of
	  ///        the ith point </param>
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
		  double fromX = fromXA[i] - falseEasting;
		  double fromY = fromYA[i] - falseNorthing;
		  double rhop = rho;

		  if (n < 0)
		  {
			rhop *= -1.0;
			fromX *= -1.0;
			fromY *= -1.0;
		  }

		  double yd = (rhop - fromY);
		  double theta = Math.Atan2(fromX, yd);
		  double r = Math.Sqrt(fromX * fromX + yd * yd);
		  if (n < 0.0)
		  {
			r *= -1.0;
		  }

		  toLon = (((theta / n + lon0) * (180.0 / Math.PI)));

		  if (Math.Abs(r) < TOLERANCE)
		  {
			toLat = ((n < 0.0) ? -90.0 : 90.0);
		  }
		  else
		  {
			double rn = Math.Pow(earth_radius * F / r, 1 / n);
			toLat = ((2.0 * Math.Atan(rn) - Math.PI / 2) * (180.0 / Math.PI));
		  }

		  toLatA[i] = (float) toLat;
		  toLonA[i] = (float) toLon;
		}
		return to;
	  }

	  /// <summary>
	  /// Convert lat/lon coordinates to projection coordinates.
	  /// </summary>
	  /// <param name="from"> array of lat/lon coordinates: from[2][n], where
	  ///        (from[latIndex][i], from[lonIndex][i]) is the (lat,lon)
	  ///        coordinate of the ith point </param>
	  /// <param name="to"> resulting array of projection coordinates: to[2][n]
	  ///        where (to[0][i], to[1][i]) is the (x,y) coordinate of
	  ///        the ith point </param>
	  /// <param name="latIndex"> index of lat coordinate; must be 0 or 1 </param>
	  /// <param name="lonIndex"> index of lon coordinate; must be 0 or 1 </param>
	  /// <returns> the "to" array </returns>
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
		  double dlon = LatLonPoints.lonNormal(fromLon - lon0Degrees);
		  double theta = n * ((dlon) * (Math.PI / 180.0));
		  double tn = Math.Pow(Math.Tan(Math.PI / 4.0 + fromLat / 2), n);
		  double r = earthRadiusTimesF / tn;
		  toX = r * Math.Sin(theta);
		  toY = rho - r * Math.Cos(theta);

		  resultXA[i] = toX + falseEasting;
		  resultYA[i] = toY + falseNorthing;
		}
		return to;
	  }

	  /// <summary>
	  /// Convert projection coordinates to lat/lon coordinate.
	  /// </summary>
	  /// <param name="from"> array of projection coordinates: from[2][n], where
	  ///        (from[0][i], from[1][i]) is the (x, y) coordinate
	  ///        of the ith point </param>
	  /// <param name="to"> resulting array of lat/lon coordinates: to[2][n] where
	  ///        (to[0][i], to[1][i]) is the (lat, lon) coordinate of
	  ///        the ith point </param>
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
		  double fromX = fromXA[i] - falseEasting;
		  double fromY = fromYA[i] - falseNorthing;
		  double rhop = rho;

		  if (n < 0)
		  {
			rhop *= -1.0;
			fromX *= -1.0;
			fromY *= -1.0;
		  }

		  double yd = (rhop - fromY);
		  double theta = Math.Atan2(fromX, yd);
		  double r = Math.Sqrt(fromX * fromX + yd * yd);
		  if (n < 0.0)
		  {
			r *= -1.0;
		  }

		  toLon = (((theta / n + lon0) * (180.0 / Math.PI)));

		  if (Math.Abs(r) < TOLERANCE)
		  {
			toLat = ((n < 0.0) ? -90.0 : 90.0);
		  }
		  else
		  {
			double rn = Math.Pow(earth_radius * F / r, 1 / n);
			toLat = ((2.0 * Math.Atan(rn) - Math.PI / 2) * (180.0 / Math.PI));
		  }

		  toLatA[i] = toLat;
		  toLonA[i] = toLon;
		}
		return to;
	  }

	}

}
