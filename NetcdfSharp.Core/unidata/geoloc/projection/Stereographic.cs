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

	/// <summary>
	/// Stereographic projection, spherical earth.
	/// Projection plane is a plane tangent to the earth at latt, lont.
	/// see John Snyder, Map Projections used by the USGS, Bulletin 1532, 2nd edition (1983), p 153
	/// 
	/// @author John Caron
	/// </summary>

	[Serializable]
	public class Stereographic : ProjectionImpl
	{

	  /// <summary>
	  /// Construct a Stereographic Projection using latitude of true scale and calculating scale factor.
	  /// <para>
	  /// Since the scale factor at lat = k = 2*k0/(1+sin(lat)) [Snyder,Working Manual p157]
	  /// then to make scale = 1 at lat, set k0 = (1+sin(lat))/2
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="latt"> tangent point of projection, also origin of projection coord system </param>
	  /// <param name="lont"> tangent point of projection, also origin of projection coord system </param>
	  /// <param name="latTrue"> latitude of true scale in degrees north; latitude where scale factor = 1.0 </param>
	  /// <returns> Stereographic projection </returns>
	  public static Stereographic factory(double latt, double lont, double latTrue)
	  {
		double scale = (1.0 + Math.Sin(((latTrue) * (Math.PI / 180.0)))) / 2.0;
		return new Stereographic(latt, lont, scale);
	  }

	  ////////////////////////////////////////////////////////////////////////////////////////////////

	  private double falseEasting, falseNorthing;
	  private double scale, earthRadius;
	  private double latt, lont;
	  private double sinlatt, coslatt, latts;
	  private bool isNorth;
	  private bool isPolar;

	  // values passed in through the constructor
	  // need for constructCopy
	  private double _latts;
	  private double _latt;
	  private double _lont;
	  private double _scale;

	  public override ProjectionImpl constructCopy()
	  {
		ProjectionImpl result = new Stereographic(TangentLat, TangentLon, Scale, falseEasting, falseNorthing, EarthRadius);
		result.DefaultMapArea = defaultMapArea;
		result.Name = name;
		return result;
	  }

	  /// <summary>
	  /// Constructor with default parameters = North Polar
	  /// </summary>
	  public Stereographic() : this(90.0, -105.0, 1.0)
	  {
	  }

	  /// <summary>
	  /// Construct a Stereographic Projection.
	  /// </summary>
	  /// <param name="latt"> tangent point of projection, also origin of projection coord system </param>
	  /// <param name="lont"> tangent point of projection, also origin of projection coord system </param>
	  /// <param name="scale"> scale factor at tangent point, "normally 1.0 but may be reduced" </param>
	  public Stereographic(double latt, double lont, double scale) : this(latt, lont, scale, 0, 0, EARTH_RADIUS)
	  {
	  }

	  /// <summary>
	  /// Construct a Stereographic Projection.
	  /// </summary>
	  /// <param name="latt"> tangent point of projection, also origin of projection coord system </param>
	  /// <param name="lont"> tangent point of projection, also origin of projection coord system </param>
	  /// <param name="scale"> scale factor at tangent point, "normally 1.0 but may be reduced" </param>
	  public Stereographic(double latt, double lont, double scale, double false_easting, double false_northing) : this(latt, lont, scale, false_easting, false_northing, EARTH_RADIUS)
	  {
	  }

	  /// <summary>
	  /// Construct a Stereographic Projection.
	  /// </summary>
	  /// <param name="latt"> tangent point of projection, also origin of projection coord system </param>
	  /// <param name="lont"> tangent point of projection, also origin of projection coord system </param>
	  /// <param name="scale"> scale factor at tangent point, "normally 1.0 but may be reduced" </param>
	  /// <param name="false_easting"> false easting in units of x coords </param>
	  /// <param name="false_northing"> false northing in units of y coords </param>
	  /// <param name="radius"> earth radius in km </param>
	  public Stereographic(double latt, double lont, double scale, double false_easting, double false_northing, double radius) : base("Stereographic", false)
	  {

		this._latts = 0.0;
		this._latt = latt;
		this._lont = lont;
		this._scale = scale;

		this.latt = ((latt) * (Math.PI / 180.0));
		this.lont = ((lont) * (Math.PI / 180.0));
		this.earthRadius = radius;
		this.scale = scale * earthRadius;
		this.falseEasting = false_easting;
		this.falseNorthing = false_northing;
		precalculate();

		addParameter(CF.GRID_MAPPING_NAME, CF.STEREOGRAPHIC);
		addParameter(CF.LONGITUDE_OF_PROJECTION_ORIGIN, lont);
		addParameter(CF.LATITUDE_OF_PROJECTION_ORIGIN, latt);
		addParameter(CF.SCALE_FACTOR_AT_PROJECTION_ORIGIN, scale);
		addParameter(CF.EARTH_RADIUS, earthRadius * 1000);

		if ((false_easting != 0.0) || (false_northing != 0.0))
		{
		  addParameter(CF.FALSE_EASTING, false_easting);
		  addParameter(CF.FALSE_NORTHING, false_northing);
		  addParameter(CDM.UNITS, "km");
		}
	  }

	  /// <summary>
	  /// Construct a polar Stereographic Projection, from the "natural origin" and the tangent point,
	  /// calculating the scale factor.
	  /// </summary>
	  /// <param name="lat_ts_deg"> Latitude at natural origin (degrees_north) </param>
	  /// <param name="latt_deg"> tangent point of projection (degrees_north) </param>
	  /// <param name="lont_deg"> tangent point of projection, also origin of projection coord system (degrees_east) </param>
	  /// <param name="north"> true if north pole, false is south pole </param>
	  public Stereographic(double lat_ts_deg, double latt_deg, double lont_deg, bool north) : base("PolarStereographic", false)
	  {

		this._latts = lat_ts_deg;
		this._latt = latt_deg;
		this._lont = lont_deg;

		this.latts = ((lat_ts_deg) * (Math.PI / 180.0));
		this.latt = ((latt_deg) * (Math.PI / 180.0));
		this.lont = ((lont_deg) * (Math.PI / 180.0));
		this.isPolar = true;
		this.isNorth = north;
		this.earthRadius = EARTH_RADIUS;
		this.falseEasting = 0;
		this.falseNorthing = 0;

		precalculate();

		double scaleFactor = (lat_ts_deg == 90 || lat_ts_deg == -90) ? 1.0 : getScaleFactor(latts, north);
		this.scale = scaleFactor * earthRadius;

		addParameter(CF.GRID_MAPPING_NAME, "polar_stereographic");
		addParameter("longitude_of_projection_origin", lont_deg);
		addParameter("latitude_of_projection_origin", latt_deg);
		addParameter("scale_factor_at_projection_origin", scaleFactor);
	  }

	  /// <summary>
	  /// Calculate polar stereographic scale factor based on the natural latitude and
	  /// longitude of the original
	  /// Ref: OGP Surveying and Positioning Guidance Note number 7, part 2 April 2009
	  /// http://www.epsg.org
	  /// added by Qun He <qunhe@unc.edu>
	  /// </summary>
	  /// <param name="lat_ts"> Latitude at natural origin </param>
	  /// <param name="north"> Is it north polar? </param>
	  /// <returns> scale factor </returns>
	  private double getScaleFactor(double lat_ts, bool north)
	  {
		double e = 0.081819191;
		double tf, mf, k0;
		double root = (1 + e * Math.Sin(lat_ts)) / (1 - e * Math.Sin(lat_ts));
		double power = e / 2;

		if (north)
		{
		  tf = Math.Tan(Math.PI / 4 - lat_ts / 2) * (Math.Pow(root, power));
		}
		else
		{
		  tf = Math.Tan(Math.PI / 4 + lat_ts / 2) / (Math.Pow(root, power));
		}

		mf = Math.Cos(lat_ts) / Math.Sqrt(1 - e * e * Math.Pow(Math.Sin(lat_ts), 2));
		k0 = mf * Math.Sqrt(Math.Pow(1 + e, 1 + e) * Math.Pow(1 - e, 1 - e)) / (2 * tf);

		return double.IsNaN(k0) ? 1.0 : k0;
	  }

	  /// <summary>
	  /// precalculate some stuff
	  /// </summary>
	  private void precalculate()
	  {
		sinlatt = Math.Sin(latt);
		coslatt = Math.Cos(latt);
	  }

	  // bean properties

	  /// <summary>
	  /// Get the scale
	  /// </summary>
	  /// <returns> the scale </returns>
	  public virtual double Scale
	  {
		  get
		  {
			return _scale;
		  }
		  set
		  {
			_scale = value;
			this.scale = earthRadius * value;
		  }
	  }

	  /// <summary>
	  /// Get the latitude at natural origin in degrees
	  /// </summary>
	  /// <returns> latitude at natural origin </returns>
	  public virtual double NaturalOriginLat
	  {
		  get
		  {
			return _latts;
		  }
	  }

	  /// <summary>
	  /// Get the tangent longitude in degrees
	  /// </summary>
	  /// <returns> the origin longitude in degrees. </returns>
	  public virtual double TangentLon
	  {
		  get
		  {
			return _lont;
		  }
		  set
		  {
			_lont = value;
			this.lont = ((value) * (Math.PI / 180.0));
			precalculate();
		  }
	  }

	  /// <summary>
	  /// Get the tangent latitude in degrees
	  /// </summary>
	  /// <returns> the origin latitude in degrees. </returns>
	  public virtual double TangentLat
	  {
		  get
		  {
			return _latt;
		  }
		  set
		  {
			_latt = value;
			this.latt = ((value) * (Math.PI / 180.0));
			precalculate();
		  }
	  }

	  public virtual double EarthRadius
	  {
		  get
		  {
			return earthRadius;
		  }
	  }

	  public virtual bool North
	  {
		  get
		  {
			return isNorth;
		  }
	  }

	  public virtual bool Polar
	  {
		  get
		  {
			return isPolar;
		  }
	  }

	  //////////////////////////////////////////////
	  // setters for IDV serialization - do not use except for object creating




	  // cruft from IDV bundle

	  /// <summary>
	  /// @deprecated
	  /// </summary>
	  [Obsolete]
	  public virtual double CentralMeridian
	  {
		  set
		  {
			TangentLon = value;
		  }
	  }

	  /// <summary>
	  /// Get the parameters as a String
	  /// </summary>
	  /// <returns> the parameters as a String </returns>
	  public override string paramsToString()
	  {
		return ToString();
	  }

	  //////////////////////////////////////////////

	  public override string ToString()
	  {
		return "Stereographic{" + "falseEasting=" + falseEasting + ", falseNorthing=" + falseNorthing + ", scale=" + scale + ", earthRadius=" + earthRadius + ", latt=" + _latt + ", lont=" + _lont + '}';
	  }

	  /// <summary>
	  /// Does the line between these two points cross the projection "seam".
	  /// </summary>
	  /// <param name="pt1"> the line goes between these two points </param>
	  /// <param name="pt2"> the line goes between these two points </param>
	  /// <returns> false if there is no seam </returns>
	  public override bool crossSeam(ProjectionPoint pt1, ProjectionPoint pt2)
	  {
		return false; // LatLonPoints.isInfinite(pt1) || LatLonPoints.isInfinite(pt2);
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

		Stereographic that = (Stereographic) o;

		if (that.earthRadius.CompareTo(earthRadius) != 0)
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
		if (that.latt.CompareTo(latt) != 0)
		{
		  return false;
		}
		if (that.lont.CompareTo(lont) != 0)
		{
		  return false;
		}
		if (that.scale.CompareTo(scale) != 0)
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
		temp = falseEasting != 0.0d ? System.BitConverter.DoubleToInt64Bits(falseEasting) : 0L;
		result = (int)(temp ^ (temp >>> 32));
		temp = falseNorthing != 0.0d ? System.BitConverter.DoubleToInt64Bits(falseNorthing) : 0L;
		result = 31 * result + (int)(temp ^ (temp >>> 32));
		temp = scale != 0.0d ? System.BitConverter.DoubleToInt64Bits(scale) : 0L;
		result = 31 * result + (int)(temp ^ (temp >>> 32));
		temp = earthRadius != 0.0d ? System.BitConverter.DoubleToInt64Bits(earthRadius) : 0L;
		result = 31 * result + (int)(temp ^ (temp >>> 32));
		temp = latt != 0.0d ? System.BitConverter.DoubleToInt64Bits(latt) : 0L;
		result = 31 * result + (int)(temp ^ (temp >>> 32));
		temp = lont != 0.0d ? System.BitConverter.DoubleToInt64Bits(lont) : 0L;
		result = 31 * result + (int)(temp ^ (temp >>> 32));
		return result;
	  }




	  /*
	   * MACROBODY
	   * projToLatLon {double phi, lam;} {
	   * double rho = Math.sqrt( fromX*fromX + fromY*fromY);
	   * double c = 2.0 * Math.atan2( rho, 2.0*scale);
	   * double sinc = Math.sin(c);
	   * double cosc = Math.cos(c);
	   *
	   * if (Math.abs(rho) < TOLERANCE)
	   * phi = latt;
	   * else
	   * phi = Math.asin( cosc * sinlatt + fromY * sinc * coslatt / rho);
	   *
	   * toLat = ((phi) * (180.0 / Math.PI));
	   *
	   * if ((Math.abs(fromX) < TOLERANCE) && (Math.abs(fromY) < TOLERANCE))
	   * lam = lont;
	   * else if (Math.abs(coslatt) < TOLERANCE)
	   * lam = lont + Math.atan2( fromX, ((latt > 0) ? -fromY : fromY) );
	   * else
	   * lam = lont + Math.atan2( fromX*sinc, rho*coslatt*cosc - fromY*sinc*sinlatt);
	   *
	   * toLon = ((lam) * (180.0 / Math.PI));
	   * }
	   *
	   * latLonToProj {} {
	   * double lat = Math.toRadians (fromLat);
	   * double lon = ((fromLon) * (Math.PI / 180.0));
	   * // keep away from the singular point
	   * if ((Math.abs(lat + latt) <= TOLERANCE)) {
	   * lat = -latt * (1.0 - TOLERANCE);
	   * }
	   *
	   * double sdlon = Math.sin(lon - lont);
	   * double cdlon = Math.cos(lon - lont);
	   * double sinlat = Math.sin(lat);
	   * double coslat = Math.cos(lat);
	   *
	   * double k = 2.0 * scale / (1.0 + sinlatt * sinlat + coslatt * coslat * cdlon);
	   * toX = k * coslat * sdlon;
	   * toY = k * ( coslatt * sinlat - sinlatt * coslat * cdlon);
	   * }
	   * MACROBODY
	   */


	  /* BEGINGENERATED */

	  /*
	   * Note this section has been generated using the convert.tcl script.
	   * This script, run as:
	   * tcl convert.tcl Stereographic.java
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


		double lat = ((fromLat) * (Math.PI / 180.0));
		double lon = ((fromLon) * (Math.PI / 180.0));
		// keep away from the singular point
		if ((Math.Abs(lat + latt) <= TOLERANCE))
		{
		  lat = -latt * (1.0 - TOLERANCE);
		}

		double sdlon = Math.Sin(lon - lont);
		double cdlon = Math.Cos(lon - lont);
		double sinlat = Math.Sin(lat);
		double coslat = Math.Cos(lat);

		double k = 2.0 * scale / (1.0 + sinlatt * sinlat + coslatt * coslat * cdlon);
		toX = k * coslat * sdlon;
		toY = k * (coslatt * sinlat - sinlatt * coslat * cdlon);

		result.setLocation(toX + falseEasting, toY + falseNorthing);
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
		double phi, lam;

		double rho = Math.Sqrt(fromX * fromX + fromY * fromY);
		double c = 2.0 * Math.Atan2(rho, 2.0 * scale);
		double sinc = Math.Sin(c);
		double cosc = Math.Cos(c);

		if (Math.Abs(rho) < TOLERANCE)
		{
		  phi = latt;
		}
		else
		{
		  phi = Math.Asin(cosc * sinlatt + fromY * sinc * coslatt / rho);
		}

		toLat = ((phi) * (180.0 / Math.PI));

		if ((Math.Abs(fromX) < TOLERANCE) && (Math.Abs(fromY) < TOLERANCE))
		{
		  lam = lont;
		}
		else if (Math.Abs(coslatt) < TOLERANCE)
		{
		  lam = lont + Math.Atan2(fromX, ((latt > 0) ? -fromY : fromY));
		}
		else
		{
		  lam = lont + Math.Atan2(fromX * sinc, rho * coslatt * cosc - fromY * sinc * sinlatt);
		}

		toLon = ((lam) * (180.0 / Math.PI));

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

		  double lat = ((fromLat) * (Math.PI / 180.0));
		  double lon = ((fromLon) * (Math.PI / 180.0));
		  // keep away from the singular point
		  if ((Math.Abs(lat + latt) <= TOLERANCE))
		  {
			lat = -latt * (1.0 - TOLERANCE);
		  }

		  double sdlon = Math.Sin(lon - lont);
		  double cdlon = Math.Cos(lon - lont);
		  double sinlat = Math.Sin(lat);
		  double coslat = Math.Cos(lat);

		  double k = 2.0 * scale / (1.0 + sinlatt * sinlat + coslatt * coslat * cdlon);
		  toX = k * coslat * sdlon;
		  toY = k * (coslatt * sinlat - sinlatt * coslat * cdlon);

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
		double phi, lam;
		double toLat, toLon;
		for (int i = 0; i < cnt; i++)
		{
		  double fromX = fromXA[i] - falseEasting;
		  double fromY = fromYA[i] - falseNorthing;

		  double rho = Math.Sqrt(fromX * fromX + fromY * fromY);
		  double c = 2.0 * Math.Atan2(rho, 2.0 * scale);
		  double sinc = Math.Sin(c);
		  double cosc = Math.Cos(c);

		  if (Math.Abs(rho) < TOLERANCE)
		  {
			phi = latt;
		  }
		  else
		  {
			phi = Math.Asin(cosc * sinlatt + fromY * sinc * coslatt / rho);
		  }

		  toLat = ((phi) * (180.0 / Math.PI));

		  if ((Math.Abs(fromX) < TOLERANCE) && (Math.Abs(fromY) < TOLERANCE))
		  {
			lam = lont;
		  }
		  else if (Math.Abs(coslatt) < TOLERANCE)
		  {
			lam = lont + Math.Atan2(fromX, ((latt > 0) ? -fromY : fromY));
		  }
		  else
		  {
			lam = lont + Math.Atan2(fromX * sinc, rho * coslatt * cosc - fromY * sinc * sinlatt);
		  }

		  toLon = ((lam) * (180.0 / Math.PI));

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

		  double lat = ((fromLat) * (Math.PI / 180.0));
		  double lon = ((fromLon) * (Math.PI / 180.0));
		  // keep away from the singular point
		  if ((Math.Abs(lat + latt) <= TOLERANCE))
		  {
			lat = -latt * (1.0 - TOLERANCE);
		  }

		  double sdlon = Math.Sin(lon - lont);
		  double cdlon = Math.Cos(lon - lont);
		  double sinlat = Math.Sin(lat);
		  double coslat = Math.Cos(lat);

		  double k = 2.0 * scale / (1.0 + sinlatt * sinlat + coslatt * coslat * cdlon);
		  toX = k * coslat * sdlon;
		  toY = k * (coslatt * sinlat - sinlatt * coslat * cdlon);

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
		double phi, lam;
		double toLat, toLon;
		for (int i = 0; i < cnt; i++)
		{
		  double fromX = fromXA[i] - falseEasting;
		  double fromY = fromYA[i] - falseNorthing;

		  double rho = Math.Sqrt(fromX * fromX + fromY * fromY);
		  double c = 2.0 * Math.Atan2(rho, 2.0 * scale);
		  double sinc = Math.Sin(c);
		  double cosc = Math.Cos(c);

		  if (Math.Abs(rho) < TOLERANCE)
		  {
			phi = latt;
		  }
		  else
		  {
			phi = Math.Asin(cosc * sinlatt + fromY * sinc * coslatt / rho);
		  }

		  toLat = ((phi) * (180.0 / Math.PI));

		  if ((Math.Abs(fromX) < TOLERANCE) && (Math.Abs(fromY) < TOLERANCE))
		  {
			lam = lont;
		  }
		  else if (Math.Abs(coslatt) < TOLERANCE)
		  {
			lam = lont + Math.Atan2(fromX, ((latt > 0) ? -fromY : fromY));
		  }
		  else
		  {
			lam = lont + Math.Atan2(fromX * sinc, rho * coslatt * cosc - fromY * sinc * sinlatt);
		  }

		  toLon = ((lam) * (180.0 / Math.PI));

		  toLatA[i] = toLat;
		  toLonA[i] = toLon;
		}
		return to;
	  }

	}

}
