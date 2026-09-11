// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

/*
 * Copyright (c) 1998-2026 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.unidata.geoloc.projection
{
	using CF = NetcdfSharp.ucar.nc2.constants.CF;
	using NetcdfSharp.ucar.unidata.geoloc;
	using Format = NetcdfSharp.ucar.unidata.util.Format;

	/// <summary>
	/// This is the "fake" identity projection where world coord = latlon coord.
	/// Topologically its the same as a cylinder tangent to the earth at the equator.
	/// The cylinder is cut at the "seam" = centerLon +- 180.
	/// Longitude values are always kept in the range [centerLon +-180]
	/// 
	/// @author John Caron </summary>
	/// <seealso cref="ProjectionImpl"/>


	[Serializable]
	public class LatLonProjection : ProjectionImpl
	{

	  /// <summary>
	  /// center longitude
	  /// </summary>
	  private double centerLon;
	  private Earth earth = EarthEllipsoid.DEFAULT;

	  public override ProjectionImpl constructCopy()
	  {
		LatLonProjection result = new LatLonProjection(Name, DefaultMapArea);
		result.DefaultMapArea = defaultMapArea;
		result.Name = name;
		result.earth = this.earth;
		return result;
	  }

	  /// <summary>
	  /// Default constructor
	  /// </summary>
	  public LatLonProjection() : this("LatLonProjection")
	  {
		addParameters();
	  }

	  public LatLonProjection(Earth earth) : this("LatLonProjection")
	  {
		this.earth = earth;
		addParameters();
	  }

	  /// <summary>
	  /// Create a new LatLonProjection
	  /// </summary>
	  /// <param name="name"> name of projection </param>
	  public LatLonProjection(string name) : this(name, new ProjectionRect(-180, -90, 180, 90))
	  {
		addParameters();
	  }

	  /// <summary>
	  /// Create a new LatLonProjection
	  /// </summary>
	  /// <param name="name"> name of projection </param>
	  /// <param name="defaultMapArea"> bounding box </param>
	  public LatLonProjection(string name, ProjectionRect defaultMapArea) : base(name, true)
	  {
		this.defaultMapArea = defaultMapArea;
		addParameters();
	  }

	  private void addParameters()
	  {
		addParameter(CF.GRID_MAPPING_NAME, CF.LATITUDE_LONGITUDE);
		if (earth.Spherical)
		{
		  addParameter(CF.EARTH_RADIUS, earth.EquatorRadius);
		}
		else
		{
		  addParameter(CF.SEMI_MAJOR_AXIS, earth.EquatorRadius);
		  addParameter(CF.SEMI_MINOR_AXIS, earth.PoleRadius);
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
			return "Lat/Lon";
		  }
	  }

	  /// <summary>
	  /// Get a String of the parameters
	  /// </summary>
	  /// <returns> a String of the parameters </returns>
	  public override string paramsToString()
	  {
		return "Center lon:" + Format.d(centerLon, 3);
	  }


	  /// <summary>
	  /// See if this projection equals the object in question
	  /// </summary>
	  /// <param name="o"> object in question </param>
	  /// <returns> true if it is a LatLonProjection and covers the same area </returns>

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

		LatLonProjection that = (LatLonProjection) o;
		if ((defaultMapArea == null) != (that.defaultMapArea == null))
		{
		  return false; // common case is that these are null
		}
		if (defaultMapArea != null && !that.defaultMapArea.Equals(defaultMapArea))
		{
		  return false;
		}

		return that.centerLon.CompareTo(centerLon) == 0;
	  }

	  public override int GetHashCode()
	  {
		int result;
		long temp;
		temp = System.BitConverter.DoubleToInt64Bits(centerLon);
		result = (int)(temp ^ (temp >>> 32));
		if (defaultMapArea != null)
		{
		  result = 31 * result + defaultMapArea.GetHashCode();
		}
		return result;
	  }

	  public override ProjectionPoint latLonToProj(LatLonPoint latlon, ProjectionPointImpl result)
	  {
		result.setLocation(LatLonPoints.lonNormal(latlon.Longitude, centerLon), latlon.Latitude);
		return result;
	  }

	  public override LatLonPoint projToLatLon(ProjectionPoint world, LatLonPointImpl result)
	  {
		result.Longitude = world.X;
		result.Latitude = world.Y;
		return result;
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

		float[] fromX = from[INDEX_X];
		float[] fromY = from[INDEX_Y];
		to[INDEX_LAT] = fromY;
		to[INDEX_LON] = fromX;
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
	  public override float[][] latLonToProj(float[][] from, float[][] to, int latIndex, int lonIndex)
	  {
		int cnt = from[0].Length;
		float[] toX = to[INDEX_X];
		float[] toY = to[INDEX_Y];
		float[] fromLat = from[latIndex];
		float[] fromLon = from[lonIndex];
		float lat, lon;
		for (int i = 0; i < cnt; i++)
		{
		  lat = fromLat[i];
		  lon = (float)(centerLon + Math.IEEERemainder(fromLon[i] - centerLon, 360.0));
		  toX[i] = lon;
		  toY[i] = lat;
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

		double[] fromX = from[INDEX_X];
		double[] fromY = from[INDEX_Y];
		to[INDEX_LAT] = fromY;
		to[INDEX_LON] = fromX;
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
		double[] toX = to[INDEX_X];
		double[] toY = to[INDEX_Y];
		double[] fromLat = from[latIndex];
		double[] fromLon = from[lonIndex];
		double lat, lon;
		for (int i = 0; i < cnt; i++)
		{
		  lat = fromLat[i];
		  lon = centerLon + Math.IEEERemainder(fromLon[i] - centerLon, 360.0);
		  toX[i] = lon;
		  toY[i] = lat;
		}
		return to;
	  }


	  /// <summary>
	  /// Set the center longitude of this projection, normalizing it into the range [-180, 180].
	  /// 
	  /// <para>
	  /// The cylinder is cut at the "seam" = centerLon +- 180.
	  /// Use this to keep the Longitude values kept in the range [centerLon +-180], which
	  /// makes seam handling easier.
	  /// <seealso cref="latLonToProj"/> returns longitudes in the range
	  /// [{@code centerLon} - 180, {@code centerLon} + 180], so the center longitude controls where the
	  /// seam falls and therefore which longitude values are produced.
	  /// 
	  /// </para>
	  /// <para>
	  /// This method always normalizes the supplied value to [-180, 180], which discards information
	  /// about grids whose longitude coordinates legitimately lie outside that range. To preserve
	  /// such information, use
	  /// <seealso cref="setCenterLon(double, bool)"/> with {@code normalize = false}.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="centerLon"> the center of the longitude range, in degrees east. </param>
	  /// <returns> centerLon normalized to [-180, 180]. </returns>
	  /// @deprecated use <seealso cref="setCenterLon(double, bool)"/> with {@code normalize = true} 
	  [Obsolete("use <seealso cref=\"setCenterLon(double, bool)\"/> with {@code normalize = true}")]
	  public virtual double setCenterLon(double centerLon)
	  {
		return setCenterLon(centerLon, true);
	  }

	  /// <summary>
	  /// Set the center longitude of this projection, optionally normalizing it into the range [-180, 180].
	  /// 
	  /// <para>
	  /// The center longitude controls the location of the projection "seam" (at {@code centerLon} +/- 180).
	  /// This also means it controls the range of longitudes returned by the projection. When {@code normalize}
	  /// is {@code false}, the supplied value is stored as given. This allows the projection to
	  /// reproduce grid longitudes that lie outside [-180, 180].
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="centerLon"> the center of the longitude range, in degrees east. </param>
	  /// <param name="normalize"> if {@code true}, normalize {@code centerLon} into [-180, 180]; if {@code false},
	  ///        store it unchanged. </param>
	  /// <returns> the center longitude that was stored. </returns>
	  public virtual double setCenterLon(double centerLon, bool normalize)
	  {
		this.centerLon = normalize ? LatLonPoints.lonNormal(centerLon) : centerLon;
		return this.centerLon;
	  }

	  /// <summary>
	  /// Get the center longitude of this projection, in degrees east.
	  /// 
	  /// <para>
	  /// Note that this value is normalized to [-180, 180] only if it was set via
	  /// <seealso cref="setCenterLon(double)"/> (or <seealso cref=".setCenterLon(double, bool)"/> with
	  /// {@code normalize = true}).
	  /// 
	  /// </para>
	  /// </summary>
	  /// <returns> the center longitude. </returns>
	  public virtual double CenterLon
	  {
		  get
		  {
			return centerLon;
		  }
	  }

	  /// <summary>
	  /// Does the line between these two points cross the projection "seam".
	  /// </summary>
	  /// <param name="pt1"> the line goes between these two points </param>
	  /// <param name="pt2"> the line goes between these two points </param>
	  /// <returns> false if there is no seam </returns>
	  public override bool crossSeam(ProjectionPoint pt1, ProjectionPoint pt2)
	  {
		return Math.Abs(pt1.X - pt2.X) > 270.0; // ?? LOOK: do I believe this
	  }


	  /// <summary>
	  /// Set a reasonable bounding box for this projection.
	  /// </summary>
	  /// <param name="bb"> a reasonable bounding box </param>
	  public override ProjectionRect DefaultMapArea
	  {
		  set
		  {
			base.DefaultMapArea = value;
			this.centerLon = value.CenterX;
			// setCenterLon( value.getCenterX());
		  }
	  }

	  /// <summary>
	  /// Split a latlon rectangle to the equivalent ProjectionRect
	  /// using this LatLonProjection to split it at the seam if needed.
	  /// </summary>
	  /// <param name="latlonR"> the latlon rectangle to transform </param>
	  /// <returns> 1 or 2 ProjectionRect. If it doesnt cross the seam,
	  ///         the second rectangle is null. </returns>
	  public virtual ProjectionRect[] latLonToProjRect(LatLonRect latlonR)
	  {

		double lat0 = latlonR.LowerLeftPoint.Latitude;
		double height = Math.Abs(latlonR.UpperRightPoint.Latitude - lat0);
		double width = latlonR.Width;
		double lon0 = LatLonPoints.lonNormal(latlonR.LowerLeftPoint.Longitude, centerLon);
		double lon1 = LatLonPoints.lonNormal(latlonR.UpperRightPoint.Longitude, centerLon);

		ProjectionRect[] rects = new ProjectionRect[]
		{
			new ProjectionRect(),
			new ProjectionRect()
		};
		if (lon0 < lon1)
		{
		  rects[0].setRect(lon0, lat0, width, height);
		  rects[1] = null;

		}
		else
		{
		  double y = centerLon + 180 - lon0;
		  rects[0].setRect(lon0, lat0, y, height);
		  rects[1].setRect(lon1 - width + y, lat0, width - y, height);
		}

		return rects;
	  }

	  public override LatLonRect projToLatLonBB(ProjectionRect world)
	  {
		double startLat = world.MinY;
		double startLon = world.MinX;

		double deltaLat = world.Height;
		double deltaLon = world.Width;

		LatLonPoint llpt = LatLonPoint.create(startLat, startLon);
		return new LatLonRect(llpt, deltaLat, deltaLon);
	  }

	  /// <summary>
	  /// Create a latlon rectangle and split it into the equivalent
	  /// ProjectionRect using this LatLonProjection. The latlon rect is
	  /// constructed from 2 lat/lon points. The lon values are considered
	  /// coords in the latlonProjection, and so do not have to be +/- 180.
	  /// </summary>
	  /// <param name="lat0"> lat of point 1 </param>
	  /// <param name="lon0"> lon of point 1 </param>
	  /// <param name="lat1"> lat of point 1 </param>
	  /// <param name="lon1"> lon of point 1 </param>
	  /// <returns> 1 or 2 ProjectionRect. If it doesnt cross the seam,
	  ///         the second rectangle is null. </returns>
	  public virtual ProjectionRect[] latLonToProjRect(double lat0, double lon0, double lat1, double lon1)
	  {

		double height = Math.Abs(lat1 - lat0);
		lat0 = Math.Min(lat1, lat0);
		double width = lon1 - lon0;
		if (width < 1.0e-8)
		{
		  width = 360.0; // assume its the whole thing
		}
		lon0 = LatLonPoints.lonNormal(lon0, centerLon);
		lon1 = LatLonPoints.lonNormal(lon1, centerLon);

		ProjectionRect[] rects = new ProjectionRect[]
		{
			new ProjectionRect(),
			new ProjectionRect()
		};
		if (width >= 360.0)
		{
		  rects[0].setRect(centerLon - 180.0, lat0, 360.0, height);
		  rects[1] = null;
		}
		else if (lon0 < lon1)
		{
		  rects[0].setRect(lon0, lat0, width, height);
		  rects[1] = null;
		}
		else
		{
		  double y = centerLon + 180 - lon0;
		  rects[0].setRect(lon0, lat0, y, height);
		  rects[1].setRect(lon1 - width + y, lat0, width - y, height);
		}
		return rects;
	  }

	}

}
