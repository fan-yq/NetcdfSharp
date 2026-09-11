// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.unidata.geoloc.projection
{
	using CDM = NetcdfSharp.ucar.nc2.constants.CDM;
	using CF = NetcdfSharp.ucar.nc2.constants.CF;
	using Misc = NetcdfSharp.ucar.nc2.util.Misc;
	using NetcdfSharp.ucar.unidata.geoloc;

	/// <summary>
	/// Sinusoidal projection, spherical earth.
	/// See John Snyder, Map Projections used by the USGS, Bulletin 1532, 2nd edition (1983), p 243
	/// 
	/// @author John Caron
	/// @since Feb 24, 2013
	/// </summary>

	[Serializable]
	public class Sinusoidal : ProjectionImpl
	{
	  private readonly double earthRadius;
	  private double centMeridian; // central Meridian in degrees
	  private double falseEasting, falseNorthing;

	  public override ProjectionImpl constructCopy()
	  {
		ProjectionImpl result = new Sinusoidal(CentMeridian, FalseEasting, FalseNorthing, EarthRadius);
		result.DefaultMapArea = defaultMapArea;
		result.Name = name;
		return result;
	  }

	  /// <summary>
	  /// Constructor with default parameters
	  /// </summary>
	  public Sinusoidal() : this(0.0, 0.0, 0.0, EARTH_RADIUS)
	  {
	  }

	  /// <summary>
	  /// Construct a Sinusoidal Projection.
	  /// </summary>
	  /// <param name="centMeridian"> central Meridian (degrees) </param>
	  /// <param name="false_easting"> false_easting in km </param>
	  /// <param name="false_northing"> false_northing in km </param>
	  /// <param name="radius"> earth radius in km </param>
	  public Sinusoidal(double centMeridian, double false_easting, double false_northing, double radius) : base(CF.SINUSOIDAL, false)
	  {

		this.centMeridian = centMeridian;
		this.falseEasting = false_easting;
		this.falseNorthing = false_northing;
		this.earthRadius = radius;

		addParameter(CF.GRID_MAPPING_NAME, CF.SINUSOIDAL);
		addParameter(CF.LONGITUDE_OF_CENTRAL_MERIDIAN, centMeridian);
		addParameter(CF.EARTH_RADIUS, earthRadius * 1000);

		if ((false_easting != 0.0) || (false_northing != 0.0))
		{
		  addParameter(CF.FALSE_EASTING, false_easting);
		  addParameter(CF.FALSE_NORTHING, false_northing);
		  addParameter(CDM.UNITS, "km");
		}

	  }

	  /// <summary>
	  /// Get the central Meridian in degrees
	  /// </summary>
	  /// <returns> the central Meridian </returns>
	  public virtual double CentMeridian
	  {
		  get
		  {
			return centMeridian;
		  }
		  set
		  {
			this.centMeridian = value;
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

	  public virtual double EarthRadius
	  {
		  get
		  {
			return earthRadius;
		  }
	  }

	  //////////////////////////////////////////////
	  // setters for IDV serialization - do not use except for object creating




	  /////////////////////////////////////////////////////


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

		Sinusoidal that = (Sinusoidal) o;

		if (that.centMeridian.CompareTo(centMeridian) != 0)
		{
		  return false;
		}
		if (that.earthRadius.CompareTo(earthRadius) != 0)
		{
		  return false;
		}
		if (that.falseEasting.CompareTo(falseEasting) != 0)
		{
		  return false;
		}
		return that.falseNorthing.CompareTo(falseNorthing) == 0;

	  }

	  public override int GetHashCode()
	  {
		int result;
		long temp;
		temp = earthRadius != 0.0d ? System.BitConverter.DoubleToInt64Bits(earthRadius) : 0L;
		result = (int)(temp ^ (temp >>> 32));
		temp = centMeridian != 0.0d ? System.BitConverter.DoubleToInt64Bits(centMeridian) : 0L;
		result = 31 * result + (int)(temp ^ (temp >>> 32));
		temp = falseEasting != 0.0d ? System.BitConverter.DoubleToInt64Bits(falseEasting) : 0L;
		result = 31 * result + (int)(temp ^ (temp >>> 32));
		temp = falseNorthing != 0.0d ? System.BitConverter.DoubleToInt64Bits(falseNorthing) : 0L;
		result = 31 * result + (int)(temp ^ (temp >>> 32));
		return result;
	  }

	  public override string ToString()
	  {
		string sb = "Sinusoidal" + "{earthRadius=" + earthRadius + ", centMeridian=" + centMeridian + ", falseEasting=" + falseEasting + ", falseNorthing=" + falseNorthing + '}';
		return sb;
	  }

	  /// <summary>
	  /// Get the parameters as a String
	  /// </summary>
	  /// <returns> the parameters as a String </returns>
	  public override string paramsToString()
	  {
		return ToString();
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

		// opposite signed long lines
		double x1 = pt1.X - falseEasting;
		double x2 = pt2.X - falseEasting;
		return (x1 * x2 < 0) && (Math.Abs(x1 - x2) > earthRadius);
	  }

	  /// <summary>
	  /// Convert a LatLonPoint to projection coordinates
	  /// </summary>
	  /// <param name="latLon"> convert from these lat, lon coordinates </param>
	  /// <param name="result"> the object to write to </param>
	  /// <returns> the given result </returns>
	  public override ProjectionPoint latLonToProj(LatLonPoint latLon, ProjectionPointImpl result)
	  {
		double deltaLon_d = LatLonPoints.range180(latLon.Longitude - centMeridian);
		double fromLat_r = ((latLon.Latitude) * (Math.PI / 180.0));

		double toX = earthRadius * ((deltaLon_d) * (Math.PI / 180.0)) * Math.Cos(fromLat_r);
		double toY = earthRadius * fromLat_r; // p 247 Snyder

		result.setLocation(toX + falseEasting, toY + falseNorthing);
		return result;
	  }

	  /// <summary>
	  /// Convert projection coordinates to a LatLonPoint
	  /// </summary>
	  /// <param name="world"> convert from these projection coordinates </param>
	  /// <param name="result"> the object to write to </param>
	  /// <returns> LatLonPoint the lat/lon coordinates </returns>
	  public override LatLonPoint projToLatLon(ProjectionPoint world, LatLonPointImpl result)
	  {
		double fromX = world.X - falseEasting;
		double fromY = world.Y - falseNorthing;

		double toLat_r = fromY / earthRadius;
		double toLon_r;

		if (Misc.nearlyEquals(Math.Abs(toLat_r), Math.PI / 2.0, 1e-10))
		{
		  toLat_r = toLat_r < 0 ? -Math.PI / 2.0 : Math.PI / 2.0;
		  toLon_r = ((centMeridian) * (Math.PI / 180.0)); // if lat == +- pi/2, set lon = centMeridian (Snyder 248)
		}
		else if (Math.Abs(toLat_r) < Math.PI / 2.0)
		{
		  toLon_r = ((centMeridian) * (Math.PI / 180.0)) + fromX / (earthRadius * Math.Cos(toLat_r));
		}
		else
		{
		  return LatLonPointImmutable.INVALID; // Projection point is off the map.
		}

		if (Misc.nearlyEquals(Math.Abs(toLon_r), Math.PI, 1e-10))
		{
		  toLon_r = toLon_r < 0 ? -Math.PI : Math.PI;
		}
		else if (Math.Abs(toLon_r) > Math.PI)
		{
		  return LatLonPointImmutable.INVALID; // Projection point is off the map.
		}

		result.Latitude = ((toLat_r) * (180.0 / Math.PI));
		result.Longitude = ((toLon_r) * (180.0 / Math.PI));
		return result;
	  }

	  public override LatLonRect projToLatLonBB(ProjectionRect projBB)
	  {
		IList<ProjectionPoint> pointsOfInterest = new List<ProjectionPoint>();

		ProjectionPoint northPole = latLonToProj(LatLonPoint.create(90, 0));
		if (projBB.contains(northPole))
		{
		  pointsOfInterest.Add(northPole);
		}

		ProjectionPoint southPole = latLonToProj(LatLonPoint.create(-90, 0));
		if (projBB.contains(southPole))
		{
		  pointsOfInterest.Add(southPole);
		}

		if (pointsOfInterest.Count == 2)
		{ // projBB contains both north and south poles, and thus, the entire map.
		  return new LatLonRect(LatLonPoint.create(-90, -180), LatLonPoint.create(90, 180));
		}

		IList<ProjectionPoint> corners = new List<ProjectionPoint> {projBB.LowerLeftPoint, projBB.LowerRightPoint, projBB.UpperLeftPoint, projBB.UpperRightPoint};

		foreach (ProjectionPoint corner in corners)
		{
		if (projToLatLon(corner) != LatLonPointImmutable.INVALID)
		  {
			pointsOfInterest.Add(corner);
		  }
		}

		((List<ProjectionPoint>)pointsOfInterest).AddRange(getMapEdgeIntercepts(projBB));

		return makeLatLonRect(pointsOfInterest);
	  }

	  /// <summary>
	  /// Returns the points at which {@code projBB} intersects the map edge.
	  /// </summary>
	  /// <param name="projBB"> defines a bounding box that may intersect the map edge, in projection coordinates. </param>
	  /// <returns> the points at which {@code projBB} intersects the map edge. May be empty. </returns>
	  public virtual IList<ProjectionPoint> getMapEdgeIntercepts(ProjectionRect projBB)
	  {
		IList<ProjectionPoint> intercepts = new List<ProjectionPoint>();

		foreach (ProjectionPoint topIntercept in getMapEdgeInterceptsAtY(projBB.UpperRightPoint.Y))
		{
		  if (pointIsBetween(topIntercept, projBB.UpperLeftPoint, projBB.UpperRightPoint))
		  {
			intercepts.Add(topIntercept);
		  }
		}

		foreach (ProjectionPoint rightIntercept in getMapEdgeInterceptsAtX(projBB.UpperRightPoint.X))
		{
		  if (pointIsBetween(rightIntercept, projBB.UpperRightPoint, projBB.LowerRightPoint))
		  {
			intercepts.Add(rightIntercept);
		  }
		}

		foreach (ProjectionPoint bottomIntercept in getMapEdgeInterceptsAtY(projBB.LowerLeftPoint.Y))
		{
		  if (pointIsBetween(bottomIntercept, projBB.LowerLeftPoint, projBB.LowerRightPoint))
		  {
			intercepts.Add(bottomIntercept);
		  }
		}

		foreach (ProjectionPoint leftIntercept in getMapEdgeInterceptsAtX(projBB.LowerLeftPoint.X))
		{
		  if (pointIsBetween(leftIntercept, projBB.LowerLeftPoint, projBB.UpperLeftPoint))
		  {
			intercepts.Add(leftIntercept);
		  }
		}

		return intercepts;
	  }

	  /// <summary>
	  /// Returns the points at which the line {@code x = x0} intersects the map edge.
	  /// </summary>
	  /// <param name="x0"> defines a line that may intersect the map edge, in projection coordinates. </param>
	  /// <returns> the points at which the line {@code x = x0} intersects the map edge. May be empty. </returns>
	  public virtual IList<ProjectionPoint> getMapEdgeInterceptsAtX(double x0)
	  {
		IList<ProjectionPoint> mapEdgeIntercepts = new List<ProjectionPoint>();
		if (projToLatLon(x0, falseNorthing) == LatLonPointImmutable.INVALID)
		{ // The line {@code x = x0} does not intersect the map.
		  return mapEdgeIntercepts; // Empty list.
		}

		double x0natural = x0 - falseEasting;
		double limitLon_r = (x0natural < 0) ? -Math.PI : Math.PI;
		double deltaLon_r = limitLon_r - ((centMeridian) * (Math.PI / 180.0));

		// This formula comes from solving 30-1 for phi, and then plugging it into 30-2. See Snyder, p 247.
		double minY = -earthRadius * Math.Acos(x0natural / (earthRadius * deltaLon_r));
		double maxY = +earthRadius * Math.Acos(x0natural / (earthRadius * deltaLon_r));

		mapEdgeIntercepts.Add(ProjectionPoint.create(x0, minY + falseNorthing));
		mapEdgeIntercepts.Add(ProjectionPoint.create(x0, maxY + falseNorthing));
		return mapEdgeIntercepts;
	  }

	  /// <summary>
	  /// Returns the points at which the line {@code y = y0} intersects the map edge.
	  /// </summary>
	  /// <param name="y0"> defines a line that intersects the map edge, in projection coordinates. </param>
	  /// <returns> the points at which the line {@code y = y0} intersects the map edge. May be empty. </returns>
	  public virtual IList<ProjectionPoint> getMapEdgeInterceptsAtY(double y0)
	  {
		IList<ProjectionPoint> mapEdgeIntercepts = new List<ProjectionPoint>();
		if (projToLatLon(falseEasting, y0) == LatLonPointImmutable.INVALID)
		{ // The line {@code y = y0} does not intersect the map.
		  return mapEdgeIntercepts; // Empty list.
		}

		double minX = getXAt(y0, -Math.PI);
		double maxX = getXAt(y0, Math.PI);

		mapEdgeIntercepts.Add(ProjectionPoint.create(minX, y0));
		mapEdgeIntercepts.Add(ProjectionPoint.create(maxX, y0));
		return mapEdgeIntercepts;
	  }

	  private double getXAt(double y0, double lon_r)
	  {
		double y0natural = y0 - falseNorthing;
		double deltaLon_r = lon_r - ((centMeridian) * (Math.PI / 180.0));

		// This formula comes from plugging 30-6 into 30-1. See Snyder, p 247-248.
		double x = earthRadius * deltaLon_r * Math.Cos(y0natural / earthRadius);

		return x + falseEasting;
	  }

	  private bool pointIsBetween(ProjectionPoint point, ProjectionPoint linePoint1, ProjectionPoint linePoint2)
	  {
		if (linePoint1.X == linePoint2.X)
		{ // No fuzzy comparison necessary.
		  Debug.Assert(point.X == linePoint1.X, "point should have the same X as the line.");

		  double minY = Math.Min(linePoint1.Y, linePoint2.Y);
		  double maxY = Math.Max(linePoint1.Y, linePoint2.Y);

		  // Returns true if point.getY() is in the range [minY, maxY], with fuzzy math.
		  return DoubleMath.fuzzyCompare(minY, point.Y, TOLERANCE) <= 0 && DoubleMath.fuzzyCompare(point.Y, maxY, TOLERANCE) <= 0;
		}
		else if (linePoint1.Y == linePoint2.Y)
		{ // No fuzzy comparison necessary.
		  Debug.Assert(point.Y == linePoint1.Y, "point should have the same Y as the line.");

		  double minX = Math.Min(linePoint1.X, linePoint2.X);
		  double maxX = Math.Max(linePoint1.X, linePoint2.X);

		  // Returns true if point.getX() is in the range [minX, maxX], with fuzzy math.
		  return DoubleMath.fuzzyCompare(minX, point.X, TOLERANCE) <= 0 && DoubleMath.fuzzyCompare(point.X, maxX, TOLERANCE) <= 0;
		}
		else
		{
		  throw new System.InvalidOperationException("CAN'T HAPPEN: linePoint1 and linePoint2 are corners on the same side of a " + "bounding box; they must have *identical* x or y values.");
		}
	  }

	  private LatLonRect makeLatLonRect(IList<ProjectionPoint> projPoints)
	  {
		if (projPoints.Count == 0)
		{
		  return LatLonRect.INVALID;
		}

		double minLat = +double.MaxValue;
		double minLon = +double.MaxValue;
		double maxLat = -double.MaxValue;
		double maxLon = -double.MaxValue;

		foreach (ProjectionPoint projPoint in projPoints)
		{
		  LatLonPoint latLonPoint = projToLatLon(projPoint);
		  Debug.Assert(latLonPoint != LatLonPointImmutable.INVALID, "We should have filtered out bad points and added good ones. WTF?");

		  minLat = Math.Min(minLat, latLonPoint.Latitude);
		  minLon = Math.Min(minLon, latLonPoint.Longitude);
		  maxLat = Math.Max(maxLat, latLonPoint.Latitude);
		  maxLon = Math.Max(maxLon, latLonPoint.Longitude);
		}

		return new LatLonRect(LatLonPoint.create(minLat, minLon), LatLonPoint.create(maxLat, maxLon));
	  }
	}

}
