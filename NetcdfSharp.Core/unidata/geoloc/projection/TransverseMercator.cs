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
	using SpecialMathFunction = NetcdfSharp.ucar.unidata.util.SpecialMathFunction;

	/// <summary>
	/// Transverse Mercator projection, spherical earth.
	/// Projection plane is a cylinder tangent to the earth at tangentLon.
	/// See John Snyder, Map Projections used by the USGS, Bulletin 1532, 2nd edition (1983), p 53
	/// 
	/// @author John Caron
	/// </summary>

	[Serializable]
	public class TransverseMercator : ProjectionImpl
	{

	  private double lat0, lon0, scale, earthRadius;
	  private double falseEasting, falseNorthing;

	  // values passed in through the constructor
	  // need for constructCopy
	  private double _lat0, _lon0, _scale;

	  public override ProjectionImpl constructCopy()
	  {
		ProjectionImpl result = new TransverseMercator(OriginLat, TangentLon, Scale, FalseEasting, FalseNorthing, EarthRadius);
		result.DefaultMapArea = defaultMapArea;
		result.Name = name;
		return result;
	  }

	  /// <summary>
	  /// Constructor with default parameteres
	  /// </summary>
	  public TransverseMercator() : this(40.0, -105.0, .9996)
	  {
	  }

	  /// <summary>
	  /// Construct a TransverseMercator Projection.
	  /// </summary>
	  /// <param name="lat0"> origin of projection coord system is at (lat0, tangentLon) </param>
	  /// <param name="tangentLon"> longitude that the cylinder is tangent at ("central meridian") </param>
	  /// <param name="scale"> scale factor along the central meridian </param>
	  public TransverseMercator(double lat0, double tangentLon, double scale) : this(lat0, tangentLon, scale, 0.0, 0.0, EARTH_RADIUS)
	  {
	  }

	  /// <summary>
	  /// Construct a TransverseMercator Projection.
	  /// </summary>
	  /// <param name="lat0"> origin of projection coord system is at (lat0, tangentLon) </param>
	  /// <param name="tangentLon"> longitude that the cylinder is tangent at ("central meridian") </param>
	  /// <param name="scale"> scale factor along the central meridian </param>
	  /// <param name="east"> false easting in km </param>
	  /// <param name="north"> false northing in km </param>
	  public TransverseMercator(double lat0, double tangentLon, double scale, double east, double north) : this(lat0, tangentLon, scale, east, north, EARTH_RADIUS)
	  {
	  }

	  /// <summary>
	  /// Construct a TransverseMercator Projection.
	  /// </summary>
	  /// <param name="lat0"> origin of projection coord system is at (lat0, tangentLon) </param>
	  /// <param name="tangentLon"> longitude that the cylinder is tangent at ("central meridian") </param>
	  /// <param name="scale"> scale factor along the central meridian </param>
	  /// <param name="east"> false easting in units of km </param>
	  /// <param name="north"> false northing in units of km </param>
	  /// <param name="radius"> earth radius in km </param>
	  public TransverseMercator(double lat0, double tangentLon, double scale, double east, double north, double radius) : base("TransverseMercator", false)
	  {

		this._lon0 = tangentLon;
		this._lat0 = lat0;
		this._scale = scale;

		this.lat0 = ((lat0) * (Math.PI / 180.0));
		this.lon0 = ((tangentLon) * (Math.PI / 180.0));
		this.earthRadius = radius;
		this.scale = scale * earthRadius;
		this.falseEasting = (!double.IsNaN(east)) ? east : 0.0;
		this.falseNorthing = (!double.IsNaN(north)) ? north : 0.0;

		addParameter(CF.GRID_MAPPING_NAME, CF.TRANSVERSE_MERCATOR);
		addParameter(CF.LONGITUDE_OF_CENTRAL_MERIDIAN, tangentLon);
		addParameter(CF.LATITUDE_OF_PROJECTION_ORIGIN, lat0);
		addParameter(CF.SCALE_FACTOR_AT_CENTRAL_MERIDIAN, scale);
		addParameter(CF.EARTH_RADIUS, earthRadius * 1000);

		if ((falseEasting != 0.0) || (falseNorthing != 0.0))
		{
		  addParameter(CF.FALSE_EASTING, falseEasting);
		  addParameter(CF.FALSE_NORTHING, falseNorthing);
		  addParameter(CDM.UNITS, "km");
		}
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
	  /// Get the tangent longitude in degrees
	  /// </summary>
	  /// <returns> the origin longitude in degrees. </returns>
	  public virtual double TangentLon
	  {
		  get
		  {
			return _lon0;
		  }
		  set
		  {
			_lon0 = value;
			lon0 = ((value) * (Math.PI / 180.0));
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
			lat0 = ((value) * (Math.PI / 180.0));
		  }
	  }

	  /// <summary>
	  /// Get the false easting, in units of km.
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
	  /// Get the false northing, in units of km
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

	  public virtual double EarthRadius
	  {
		  get
		  {
			return earthRadius;
		  }
	  }

	  //////////////////////////////////////////////
	  // setters for IDV serialization - do not use except for object creating






	  /////////////////////////////////////////////

	  /// <summary>
	  /// Get the label to be used in the gui for this type of projection
	  /// </summary>
	  /// <returns> Type label </returns>
	  public override string ProjectionTypeLabel
	  {
		  get
		  {
			return "Transverse mercator";
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

	  public override string ToString()
	  {
		return "TransverseMercator{" + "lat0=" + _lat0 + ", lon0=" + _lon0 + ", scale=" + _scale + ", earthRadius=" + earthRadius + ", falseEasting=" + falseEasting + ", falseNorthing=" + falseNorthing + '}';
	  }

	  /// <summary>
	  /// Does the line between these two points cross the projection "seam".
	  /// </summary>
	  /// <param name="pt1"> the line goes between these two points </param>
	  /// <param name="pt2"> the line goes between these two points </param>
	  /// <returns> false if there is no seam </returns>
	  public override bool crossSeam(ProjectionPoint pt1, ProjectionPoint pt2)
	  {
		// either point is infinite
		if (LatLonPoints.isInfinite(pt1) || LatLonPoints.isInfinite(pt2))
		{
		  return true;
		}

		double y1 = pt1.Y - falseNorthing;
		double y2 = pt2.Y - falseNorthing;

		// opposite signed long lines
		return (y1 * y2 < 0) && (Math.Abs(y1 - y2) > 2 * earthRadius);
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

		TransverseMercator that = (TransverseMercator) o;
		double tolerance = 1e-6;

		if (DoubleMath.fuzzyCompare(that.earthRadius, earthRadius, tolerance) != 0)
		{
		  return false;
		}
		if (DoubleMath.fuzzyCompare(that.falseEasting, falseEasting, tolerance) != 0)
		{
		  return false;
		}
		if (DoubleMath.fuzzyCompare(that.falseNorthing, falseNorthing, tolerance) != 0)
		{
		  return false;
		}
		if (DoubleMath.fuzzyCompare(that.lat0, lat0, tolerance) != 0)
		{
		  return false;
		}
		if (DoubleMath.fuzzyCompare(that.lon0, lon0, tolerance) != 0)
		{
		  return false;
		}
		if (DoubleMath.fuzzyCompare(that.scale, scale, tolerance) != 0)
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
		temp = scale != 0.0d ? System.BitConverter.DoubleToInt64Bits(scale) : 0L;
		result = 31 * result + (int)(temp ^ (temp >>> 32));
		temp = earthRadius != 0.0d ? System.BitConverter.DoubleToInt64Bits(earthRadius) : 0L;
		result = 31 * result + (int)(temp ^ (temp >>> 32));
		temp = falseEasting != 0.0d ? System.BitConverter.DoubleToInt64Bits(falseEasting) : 0L;
		result = 31 * result + (int)(temp ^ (temp >>> 32));
		temp = falseNorthing != 0.0d ? System.BitConverter.DoubleToInt64Bits(falseNorthing) : 0L;
		result = 31 * result + (int)(temp ^ (temp >>> 32));
		return result;
	  }

	  /*
	   * MACROBODY
	   * projToLatLon {} {
	   * double x = (fromX-falseEasting)/scale;
	   * double d = (fromY-falseNorthing)/scale + lat0;
	   * toLon = ((lon0 + Math.atan2(SpecialMathFunction.sinh(x), Math.cos(d))) * (180.0 / Math.PI));
	   * toLat = ((Math.asin( Math.sin(d)/ SpecialMathFunction.cosh(x))) * (180.0 / Math.PI));
	   * }
	   * 
	   * latLonToProj {} {
	   * double lon = ((fromLon) * (Math.PI / 180.0));
	   * double lat = ((fromLat) * (Math.PI / 180.0));
	   * double dlon = lon-lon0;
	   * double b = Math.cos( lat) * Math.sin(dlon);
	   * // infinite projection
	   * if ((Math.abs(Math.abs(b) - 1.0)) < TOLERANCE) {
	   * toX = 0.0; toY = 0.0;
	   * } else {
	   * toX = scale * SpecialMathFunction.atanh(b) + falseEasting;
	   * toY = scale * (Math.atan2(Math.tan(lat),Math.cos(dlon)) - lat0) + falseNorthing;
	   * }
	   * }
	   * 
	   * 
	   * 
	   * MACROBODY
	   */

	  /* BEGINGENERATED */

	  /*
	   * Note this section has been generated using the convert.tcl script.
	   * This script, run as:
	   * tcl convert.tcl TransverseMercator.java
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

		double lon = ((fromLon) * (Math.PI / 180.0));
		double lat = ((fromLat) * (Math.PI / 180.0));
		double dlon = lon - lon0;
		double b = Math.Cos(lat) * Math.Sin(dlon);

		if ((Math.Abs(Math.Abs(b) - 1.0)) < TOLERANCE)
		{ // infinite projection
		  toX = double.PositiveInfinity;
		  toY = double.PositiveInfinity;
		}
		else
		{
		  toX = scale * SpecialMathFunction.atanh(b);
		  toY = scale * (Math.Atan2(Math.Tan(lat), Math.Cos(dlon)) - lat0);
		}

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
		double fromX = world.X;
		double fromY = world.Y;

		double x = (fromX - falseEasting) / scale;
		double d = (fromY - falseNorthing) / scale + lat0;
		toLon = ((lon0 + Math.Atan2(Math.Sinh(x), Math.Cos(d))) * (180.0 / Math.PI));
		toLat = ((Math.Asin(Math.Sin(d) / Math.Cosh(x))) * (180.0 / Math.PI));

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

		  double lon = ((fromLon) * (Math.PI / 180.0));
		  double lat = ((fromLat) * (Math.PI / 180.0));
		  double dlon = lon - lon0;
		  double b = Math.Cos(lat) * Math.Sin(dlon);
		  // infinite projection
		  if ((Math.Abs(Math.Abs(b) - 1.0)) < TOLERANCE)
		  {
			toX = 0.0;
			toY = 0.0;
		  }
		  else
		  {
			toX = scale * SpecialMathFunction.atanh(b) + falseEasting;
			toY = scale * (Math.Atan2(Math.Tan(lat), Math.Cos(dlon)) - lat0) + falseNorthing;
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

		  double x = (fromX - falseEasting) / scale;
		  double d = (fromY - falseNorthing) / scale + lat0;
		  toLon = ((lon0 + Math.Atan2(Math.Sinh(x), Math.Cos(d))) * (180.0 / Math.PI));
		  toLat = ((Math.Asin(Math.Sin(d) / Math.Cosh(x))) * (180.0 / Math.PI));

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

		  double lon = ((fromLon) * (Math.PI / 180.0));
		  double lat = ((fromLat) * (Math.PI / 180.0));
		  double dlon = lon - lon0;
		  double b = Math.Cos(lat) * Math.Sin(dlon);
		  // infinite projection
		  if ((Math.Abs(Math.Abs(b) - 1.0)) < TOLERANCE)
		  {
			toX = 0.0;
			toY = 0.0;
		  }
		  else
		  {
			toX = scale * SpecialMathFunction.atanh(b) + falseEasting;
			toY = scale * (Math.Atan2(Math.Tan(lat), Math.Cos(dlon)) - lat0) + falseNorthing;
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

		  double x = (fromX - falseEasting) / scale;
		  double d = (fromY - falseNorthing) / scale + lat0;
		  toLon = ((lon0 + Math.Atan2(Math.Sinh(x), Math.Cos(d))) * (180.0 / Math.PI));
		  toLat = ((Math.Asin(Math.Sin(d) / Math.Cosh(x))) * (180.0 / Math.PI));

		  toLatA[i] = toLat;
		  toLonA[i] = toLon;
		}
		return to;
	  }

	  /* ENDGENERATED */


	}



}
