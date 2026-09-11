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
	/// FlatEarth Projection
	/// This projection surface is tangent at some point (lat0, lon0) and
	/// has a y axis rotated from true North by some angle.
	/// <p/>
	/// We call it "flat" because it should only be used where the spherical
	/// geometry of the earth is not significant. In actuallity, we use the simple
	/// "arclen" routine which computes dy along a meridian, and dx along a
	/// latitude circle. We rotate the coordinate system to/from a true north system.
	/// <p/>
	/// See John Snyder, Map Projections used by the USGS, Bulletin 1532,
	/// 2nd edition (1983), p 145
	/// 
	/// @author Unidata Development Team </summary>
	/// <seealso cref="Projection"/>
	/// <seealso cref="ProjectionImpl"/>

	[Serializable]
	public class FlatEarth : ProjectionImpl
	{
	  public const string ROTATIONANGLE = "rotationAngle";

	  /// <summary>
	  /// constants from Snyder's equations </summary>
	  private readonly double rotAngle, radius;
	  private readonly double lat0, lon0; // center lat/lon in radians

	  // values passed in through the constructor
	  // need for constructCopy
	  private readonly double _lat0, _lon0;

	  private double cosRot, sinRot;

	  public override ProjectionImpl constructCopy()
	  {
		ProjectionImpl result = new FlatEarth(OriginLat, OriginLon, RotationAngle);
		result.DefaultMapArea = defaultMapArea;
		result.Name = name;
		return result;
	  }

	  /// <summary>
	  /// Constructor with default parameters
	  /// </summary>
	  public FlatEarth() : this(0.0, 0.0, 0.0, EARTH_RADIUS)
	  {
	  }

	  public FlatEarth(double lat0, double lon0) : this(lat0, lon0, 0.0, EARTH_RADIUS)
	  {
	  }

	  public FlatEarth(double lat0, double lon0, double rotAngle) : this(lat0, lon0, rotAngle, EARTH_RADIUS)
	  {
	  }

	  /// <summary>
	  /// Construct a FlatEarth Projection, two standard parellels.
	  /// For the one standard parellel case, set them both to the same value.
	  /// </summary>
	  /// <param name="lat0"> lat origin of the coord. system on the projection plane </param>
	  /// <param name="lon0"> lon origin of the coord. system on the projection plane </param>
	  /// <param name="rotAngle"> angle of rotation, in degrees </param>
	  /// <param name="radius"> earth radius in km </param>
	  /// <exception cref="IllegalArgumentException"> if lat0, par1, par2 = +/-90 deg </exception>
	  public FlatEarth(double lat0, double lon0, double rotAngle, double radius) : base("FlatEarth", false)
	  {

		this._lat0 = lat0;
		this._lon0 = lon0;

		this.lat0 = ((lat0) * (Math.PI / 180.0));
		this.lon0 = ((lon0) * (Math.PI / 180.0));
		this.rotAngle = ((rotAngle) * (Math.PI / 180.0));
		this.radius = radius;

		precalculate();

		addParameter(CF.GRID_MAPPING_NAME, "flat_earth");
		addParameter(CF.LATITUDE_OF_PROJECTION_ORIGIN, lat0);
		addParameter(CF.LONGITUDE_OF_PROJECTION_ORIGIN, lon0);
		addParameter(ROTATIONANGLE, rotAngle);
		addParameter(CF.EARTH_RADIUS, radius * 1000);
	  }

	  /// <summary>
	  /// Precalculate some stuff
	  /// </summary>
	  private void precalculate()
	  {
		sinRot = Math.Sin(rotAngle);
		cosRot = Math.Cos(rotAngle);
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

		FlatEarth flatEarth = (FlatEarth) o;

		if (flatEarth.lat0.CompareTo(lat0) != 0)
		{
		  return false;
		}
		if (flatEarth.lon0.CompareTo(lon0) != 0)
		{
		  return false;
		}
		if (flatEarth.radius.CompareTo(radius) != 0)
		{
		  return false;
		}
		if (flatEarth.rotAngle.CompareTo(rotAngle) != 0)
		{
		  return false;
		}
		if ((defaultMapArea == null) != (flatEarth.defaultMapArea == null))
		{
		  return false; // common case is that these are null
		}
		return defaultMapArea == null || flatEarth.defaultMapArea.Equals(defaultMapArea);

	  }

	  public override int GetHashCode()
	  {
		int result;
		long temp;
		temp = rotAngle != 0.0d ? System.BitConverter.DoubleToInt64Bits(rotAngle) : 0L;
		result = (int)(temp ^ (temp >>> 32));
		temp = radius != 0.0d ? System.BitConverter.DoubleToInt64Bits(radius) : 0L;
		result = 31 * result + (int)(temp ^ (temp >>> 32));
		temp = lat0 != 0.0d ? System.BitConverter.DoubleToInt64Bits(lat0) : 0L;
		result = 31 * result + (int)(temp ^ (temp >>> 32));
		temp = lon0 != 0.0d ? System.BitConverter.DoubleToInt64Bits(lon0) : 0L;
		result = 31 * result + (int)(temp ^ (temp >>> 32));
		return result;
	  }

	  // bean properties

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
	  }

	  /// <summary>
	  /// Get the rotation angle.
	  /// </summary>
	  /// <returns> the origin latitude. </returns>
	  public virtual double RotationAngle
	  {
		  get
		  {
			return rotAngle;
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
			return "FlatEarth";
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
		return "FlatEarth{" + "rotAngle=" + rotAngle + ", radius=" + radius + ", lat0=" + _lat0 + ", lon0=" + _lon0 + '}';
	  }

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
		double dx, dy;

		fromLat = ((fromLat) * (Math.PI / 180.0));

		dy = radius * (fromLat - lat0);
		dx = radius * Math.Cos(fromLat) * (((fromLon) * (Math.PI / 180.0)) - lon0);


		toX = cosRot * dx - sinRot * dy;
		toY = sinRot * dx + cosRot * dy;


		result.setLocation(toX, toY);

		return result;
	  }

	  public override LatLonPoint projToLatLon(ProjectionPoint world, LatLonPointImpl result)
	  {
		double toLat, toLon;
		double x = world.X;
		double y = world.Y;
		double cosl;
		double xp, yp;

		xp = cosRot * x + sinRot * y;
		yp = -sinRot * x + cosRot * y;

		toLat = ((lat0) * (180.0 / Math.PI)) + ((yp / radius) * (180.0 / Math.PI));
		// double lat2;
		// lat2 = lat0 + ((yp/radius) * (180.0 / Math.PI));
		cosl = Math.Cos(((toLat) * (Math.PI / 180.0)));
		if (Math.Abs(cosl) < TOLERANCE)
		{
		  toLon = ((lon0) * (180.0 / Math.PI));
		}
		else
		{
		  toLon = ((lon0) * (180.0 / Math.PI)) + ((xp / cosl / radius) * (180.0 / Math.PI));
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
		  double dy = radius * (fromLat - lat0);
		  double dx = radius * Math.Cos(fromLat) * (((fromLon) * (Math.PI / 180.0)) - lon0);


		  toX = cosRot * dx - sinRot * dy;
		  toY = sinRot * dx + cosRot * dy;

		  resultXA[i] = (float) toX;
		  resultYA[i] = (float) toY;
		}
		return to;
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

		return (pt1.X * pt2.X < 0);
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

		  double xp = cosRot * fromX + sinRot * fromY;
		  double yp = -sinRot * fromX + cosRot * fromY;


		  toLat = ((lat0) * (180.0 / Math.PI)) + ((yp / radius) * (180.0 / Math.PI));
		  double cosl = Math.Cos(((toLat) * (Math.PI / 180.0)));

		  if (Math.Abs(cosl) < TOLERANCE)
		  {
			toLon = ((lon0) * (180.0 / Math.PI));
		  }
		  else
		  {
			toLon = ((lon0) * (180.0 / Math.PI)) + ((xp / cosl / radius) * (180.0 / Math.PI));
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
		  double dy = radius * (fromLat - lat0);
		  double dx = radius * Math.Cos(fromLat) * (((fromLon) * (Math.PI / 180.0)) - lon0);

		  toX = cosRot * dx - sinRot * dy;
		  toY = sinRot * dx + cosRot * dy;


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

		  double xp = cosRot * fromX + sinRot * fromY;
		  double yp = -sinRot * fromX + cosRot * fromY;

		  // toLat = lat0 + ((yp) * (180.0 / Math.PI));
		  toLat = ((lat0) * (180.0 / Math.PI)) + ((yp / radius) * (180.0 / Math.PI));
		  double cosl = Math.Cos(((toLat) * (Math.PI / 180.0)));

		  if (Math.Abs(cosl) < TOLERANCE)
		  {
			toLon = ((lon0) * (180.0 / Math.PI));
		  }
		  else
		  {
			toLon = ((lon0) * (180.0 / Math.PI)) + ((xp / cosl / radius) * (180.0 / Math.PI));
		  }

		  toLon = LatLonPoints.lonNormal(toLon);

		  toLatA[i] = toLat;
		  toLonA[i] = toLon;

		}
		return to;
	  }

	}


}
