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
	/// Mercator projection, spherical earth.
	/// Projection plane is a cylinder tangent to the earth at tangentLon.
	/// See John Snyder, Map Projections used by the USGS, Bulletin 1532, 2nd edition (1983), p 43-47
	/// 
	/// @author John Caron </summary>
	/// <seealso cref="Projection"/>
	/// <seealso cref="ProjectionImpl"/>

	[Serializable]
	public class Mercator : ProjectionImpl
	{

	  /// <summary>
	  /// Convert "scale at standard parellel" to "standard parellel"
	  /// </summary>
	  /// <param name="scale"> scale at standard parallel </param>
	  /// <returns> standard parallel in degrees </returns>
	  public static double convertScaleToStandardParallel(double scale)
	  {
		// k = 1 / cos (par); snyder p 44
		// par = arccos(1/k);
		double par = Math.Acos(1.0 / scale);
		return ((par) * (180.0 / Math.PI));
	  }

	  /////////////////////////////////////////////////////////////////////
	  private readonly double earthRadius;
	  private double lon0; // longitude of the origin in degrees
	  private double par; // standard parallel in degrees
	  private double falseEasting, falseNorthing;

	  private double par_r; // standard parallel in radians
	  private double A;

	  public override ProjectionImpl constructCopy()
	  {
		ProjectionImpl result = new Mercator(OriginLon, Parallel, FalseEasting, FalseNorthing, EarthRadius);
		result.DefaultMapArea = defaultMapArea;
		result.Name = name;
		return result;
	  }

	  /// <summary>
	  /// Constructor with default parameters
	  /// </summary>
	  public Mercator() : this(-105, 20.0, 0.0, 0.0, EARTH_RADIUS)
	  {
	  }

	  /// <summary>
	  /// Construct a Mercator Projection.
	  /// </summary>
	  /// <param name="lon0"> longitude of origin (degrees) </param>
	  /// <param name="par"> standard parallel (degrees). cylinder cuts earth at this latitude. </param>
	  public Mercator(double lon0, double par) : this(lon0, par, 0.0, 0.0, EARTH_RADIUS)
	  {
	  }

	  public Mercator(double lon0, double par, double false_easting, double false_northing) : this(lon0, par, false_easting, false_northing, EARTH_RADIUS)
	  {
	  }

	  /// <summary>
	  /// Construct a Mercator Projection.
	  /// </summary>
	  /// <param name="lon0"> longitude of origin (degrees) </param>
	  /// <param name="par"> standard parallel (degrees). cylinder cuts earth at this latitude. </param>
	  /// <param name="false_easting"> false_easting in km </param>
	  /// <param name="false_northing"> false_northing in km </param>
	  /// <param name="radius"> earth radius in km </param>
	  public Mercator(double lon0, double par, double false_easting, double false_northing, double radius) : base("Mercator", false)
	  {

		this.lon0 = lon0;
		this.par = par;
		this.falseEasting = false_easting;
		this.falseNorthing = false_northing;
		this.earthRadius = radius;

		this.par_r = ((par) * (Math.PI / 180.0));

		precalculate();

		addParameter(CF.GRID_MAPPING_NAME, CF.MERCATOR);
		addParameter(CF.LONGITUDE_OF_PROJECTION_ORIGIN, lon0);
		addParameter(CF.STANDARD_PARALLEL, par);
		addParameter(CF.EARTH_RADIUS, earthRadius * 1000);
		if ((false_easting != 0.0) || (false_northing != 0.0))
		{
		  addParameter(CF.FALSE_EASTING, false_easting);
		  addParameter(CF.FALSE_NORTHING, false_northing);
		  addParameter(CDM.UNITS, "km");
		}

	  }

	  /// <summary>
	  /// Precalculate some params
	  /// </summary>
	  private void precalculate()
	  {
		A = earthRadius * Math.Cos(par_r); // incorporates the scale factor at par
	  }

	  /// <summary>
	  /// Get the first standard parallel
	  /// </summary>
	  /// <returns> the first standard parallel </returns>
	  public virtual double Parallel
	  {
		  get
		  {
			return par;
		  }
		  set
		  {
			this.par = value;
			this.par_r = ((value) * (Math.PI / 180.0));
			precalculate();
		  }
	  }

	  /// <summary>
	  /// Get the origin longitude.
	  /// </summary>
	  /// <returns> the origin longitude. </returns>
	  public virtual double OriginLon
	  {
		  get
		  {
			return lon0;
		  }
		  set
		  {
			lon0 = value;
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
		return "Mercator{" + "earthRadius=" + earthRadius + ", lon0=" + lon0 + ", par=" + par + ", falseEasting=" + falseEasting + ", falseNorthing=" + falseNorthing + '}';
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

		// opposite signed long lines: LOOK ????
		return (pt1.X * pt2.X < 0);
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

		Mercator mercator = (Mercator) o;

		if (mercator.earthRadius.CompareTo(earthRadius) != 0)
		{
		  return false;
		}
		if (mercator.falseEasting.CompareTo(falseEasting) != 0)
		{
		  return false;
		}
		if (mercator.falseNorthing.CompareTo(falseNorthing) != 0)
		{
		  return false;
		}
		if (mercator.lon0.CompareTo(lon0) != 0)
		{
		  return false;
		}
		if (mercator.par.CompareTo(par) != 0)
		{
		  return false;
		}
		if ((defaultMapArea == null) != (mercator.defaultMapArea == null))
		{
		  return false; // common case is that these are null
		}
		return defaultMapArea == null || mercator.defaultMapArea.Equals(defaultMapArea);

	  }

	  public override int GetHashCode()
	  {
		int result;
		long temp;
		temp = earthRadius != 0.0d ? System.BitConverter.DoubleToInt64Bits(earthRadius) : 0L;
		result = (int)(temp ^ (temp >>> 32));
		temp = lon0 != 0.0d ? System.BitConverter.DoubleToInt64Bits(lon0) : 0L;
		result = 31 * result + (int)(temp ^ (temp >>> 32));
		temp = par != 0.0d ? System.BitConverter.DoubleToInt64Bits(par) : 0L;
		result = 31 * result + (int)(temp ^ (temp >>> 32));
		temp = falseEasting != 0.0d ? System.BitConverter.DoubleToInt64Bits(falseEasting) : 0L;
		result = 31 * result + (int)(temp ^ (temp >>> 32));
		temp = falseNorthing != 0.0d ? System.BitConverter.DoubleToInt64Bits(falseNorthing) : 0L;
		result = 31 * result + (int)(temp ^ (temp >>> 32));
		return result;
	  }

	  public override ProjectionPoint latLonToProj(LatLonPoint latLon, ProjectionPointImpl result)
	  {
		double toX, toY;
		double fromLat = latLon.Latitude;
		double fromLon = latLon.Longitude;
		double fromLat_r = ((fromLat) * (Math.PI / 180.0));

		// infinite projection
		if ((Math.Abs(90.0 - Math.Abs(fromLat))) < TOLERANCE)
		{
		  toX = double.PositiveInfinity;
		  toY = double.PositiveInfinity;
		}
		else
		{
		  toX = A * ((LatLonPoints.range180(fromLon - this.lon0)) * (Math.PI / 180.0));
		  toY = A * SpecialMathFunction.atanh(Math.Sin(fromLat_r)); // p 41 Snyder
		}

		result.setLocation(toX + falseEasting, toY + falseNorthing);
		return result;
	  }

	  public override LatLonPoint projToLatLon(ProjectionPoint world, LatLonPointImpl result)
	  {
		double fromX = world.X - falseEasting;
		double fromY = world.Y - falseNorthing;

		double toLon = ((fromX / A) * (180.0 / Math.PI)) + lon0;

		double e = Math.Exp(-fromY / A);
		double toLat = ((Math.PI / 2 - 2 * Math.Atan(e)) * (180.0 / Math.PI)); // Snyder p 44

		result.Latitude = toLat;
		result.Longitude = toLon;
		return result;
	  }

	}


}
