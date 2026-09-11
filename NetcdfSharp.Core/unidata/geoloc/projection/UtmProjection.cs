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
	/// Universal Transverse Mercator.
	/// Ellipsoidal earth.
	/// <p/>
	/// Origin of coordinate system is relative to the point where the
	/// central meridian and the equator cross.
	/// This point has x,y value = (500, 0) km for north hemisphere.
	/// and (500, 10,0000) km for south hemisphere.
	/// Increasing values always go north and east.
	/// <p/>
	/// The central meridian = (zone * 6 - 183) degrees, where zone in [1,60].
	/// 
	/// </summary>
	[Serializable]
	public class UtmProjection : ProjectionImpl
	{
	  public const string GRID_MAPPING_NAME = "universal_transverse_mercator";
	  public const string UTM_ZONE1 = "utm_zone_number";
	  public const string UTM_ZONE2 = "UTM_zone";

	  private readonly Utm_To_Gdc_Converter convert2latlon;
	  private readonly Gdc_To_Utm_Converter convert2xy;

	  // TODO will noty implement Serializable in ver 6.
	  [Serializable]
	  private class SaveParams
	  {
		internal readonly double a;
		internal readonly double f;
		internal readonly int zone;
		internal readonly bool isNorth;

		internal SaveParams(double a, double f, int zone, bool isNorth)
		{
		  this.a = a;
		  this.f = f;
		  this.zone = zone;
		  this.isNorth = isNorth;
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
		  SaveParams that = (SaveParams) o;
		  if (that.a.CompareTo(a) != 0)
		  {
			return false;
		  }
		  if (that.f.CompareTo(f) != 0)
		  {
			return false;
		  }
		  if (isNorth != that.isNorth)
		  {
			return false;
		  }
		  return zone == that.zone;
		}

		public override int GetHashCode()
		{
		  int result;
		  long temp;
		  temp = System.BitConverter.DoubleToInt64Bits(a);
		  result = (int)(temp ^ (temp >>> 32));
		  temp = System.BitConverter.DoubleToInt64Bits(f);
		  result = 31 * result + (int)(temp ^ (temp >>> 32));
		  result = 31 * result + zone;
		  result = 31 * result + (isNorth ? 1 : 0);
		  return result;
		}
	  }

	  private readonly SaveParams saveParams; // needed for constructCopy

	  public override ProjectionImpl constructCopy()
	  {
		ProjectionImpl result = (saveParams == null) ? new UtmProjection(Zone, North) : new UtmProjection(saveParams.a, saveParams.f, Zone, North);
		result.DefaultMapArea = defaultMapArea;
		result.Name = name;
		return result;
	  }

	  /// <summary>
	  /// Constructor with default parameters
	  /// </summary>
	  public UtmProjection() : this(5, true)
	  {
	  }

	  /// <summary>
	  /// Constructor with default WGS 84 ellipsoid.
	  /// </summary>
	  /// <param name="zone"> the UTM zone number (1-60) </param>
	  /// <param name="isNorth"> true if the UTM coordinate is in the northern hemisphere </param>
	  public UtmProjection(int zone, bool isNorth) : base("UtmProjection", false)
	  {
		convert2latlon = new Utm_To_Gdc_Converter(zone, isNorth);
		convert2xy = new Gdc_To_Utm_Converter(zone, isNorth);
		saveParams = new SaveParams(EarthEllipsoid.WGS84.Major, 1 / EarthEllipsoid.WGS84.Flattening, zone, isNorth);

		addParameter(CF.GRID_MAPPING_NAME, GRID_MAPPING_NAME);
		addParameter(CF.SEMI_MAJOR_AXIS, EarthEllipsoid.WGS84.Major);
		addParameter(CF.INVERSE_FLATTENING, EarthEllipsoid.WGS84.Flattening);
		addParameter(UTM_ZONE1, zone);
	  }

	  /// <summary>
	  /// Construct a Universal Transverse Mercator Projection.
	  /// </summary>
	  /// <param name="a"> the semi-major axis (meters) for the ellipsoid </param>
	  /// <param name="f"> the inverse flattening for the ellipsoid </param>
	  /// <param name="zone"> the UTM zone number (1-60) </param>
	  /// <param name="isNorth"> true if the UTM coordinate is in the northern hemisphere </param>
	  public UtmProjection(double a, double f, int zone, bool isNorth) : base("UtmProjection", false)
	  {
		saveParams = new SaveParams(a, f, zone, isNorth);

		convert2latlon = new Utm_To_Gdc_Converter(a, f, zone, isNorth);
		convert2xy = new Gdc_To_Utm_Converter(a, f, zone, isNorth);

		addParameter(CF.GRID_MAPPING_NAME, GRID_MAPPING_NAME);
		addParameter(CF.SEMI_MAJOR_AXIS, a);
		addParameter(CF.INVERSE_FLATTENING, f);
		addParameter(UTM_ZONE1, zone);
	  }

	  /// <summary>
	  /// Get the zone number = [1,60]
	  /// </summary>
	  /// <returns> zone number </returns>
	  public virtual int Zone
	  {
		  get
		  {
			return convert2latlon.Zone;
		  }
	  }

	  /// <summary>
	  /// Get whether in North or South Hemisphere.
	  /// </summary>
	  /// <returns> true if north </returns>
	  public virtual bool North
	  {
		  get
		  {
			return convert2latlon.North;
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
			return "Universal transverse mercator";
		  }
	  }

	  /*
	   * Getting the central meridian in degrees. depends on the zone
	   * 
	   * @return the central meridian in degrees.
	   */
	  public virtual double CentralMeridian
	  {
		  get
		  {
			return convert2xy.CentralMeridian;
		  }
	  }

	  /// <summary>
	  /// Get the parameters as a String
	  /// </summary>
	  /// <returns> the parameters as a String </returns>
	  public override string paramsToString()
	  {
		return Zone + " " + North;
	  }

	  /// <summary>
	  /// Does the line between these two points cross the projection "seam".
	  /// </summary>
	  /// <param name="pt1"> the line goes between these two points </param>
	  /// <param name="pt2"> the line goes between these two points </param>
	  /// <returns> false if there is no seam </returns>
	  public override bool crossSeam(ProjectionPoint pt1, ProjectionPoint pt2)
	  {
		return false;
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
		UtmProjection that = (UtmProjection) o;
		return saveParams.Equals(that.saveParams);
	  }

	  public override int GetHashCode()
	  {
		return saveParams.GetHashCode();
	  }

	  /*
	   * Returns true if this represents the same Projection as proj.
	   *
	   * @param proj projection in question
	   * 
	   * @return true if this represents the same Projection as proj.
	   *
	   * public boolean equals(Object proj) {
	   * if (!(proj instanceof UtmProjection)) {
	   * return false;
	   * }
	   * 
	   * UtmProjection op = (UtmProjection) proj;
	   * if ((this.getDefaultMapArea() == null) != (op.defaultMapArea == null)) return false; // common case is that these
	   * are null
	   * if (this.getDefaultMapArea() != null && !this.defaultMapArea.equals(op.defaultMapArea)) return false;
	   * 
	   * return op.getZone() == getZone();
	   * }
	   */



	  /// <summary>
	  /// Convert a LatLonPoint to projection coordinates
	  /// </summary>
	  /// <param name="latLon"> convert from these lat, lon coordinates </param>
	  /// <param name="result"> the object to write to </param>
	  /// <returns> the given result </returns>
	  public override ProjectionPoint latLonToProj(LatLonPoint latLon, ProjectionPointImpl result)
	  {
		double fromLat = latLon.Latitude;
		double fromLon = latLon.Longitude;

		return convert2xy.latLonToProj(fromLat, fromLon, result);
	  }

	  public override double[][] latLonToProj(double[][] from, double[][] to, int latIndex, int lonIndex)
	  {
		if ((from == null) || (from.Length != 2))
		{
		  throw new ArgumentException("ProjectionImpl.latLonToProj:null array argument or wrong dimension (from)");
		}
		if ((to == null) || (to.Length != 2))
		{
		  throw new ArgumentException("ProjectionImpl.latLonToProj:null array argument or wrong dimension (to)");
		}
		if (from[0].Length != to[0].Length)
		{
		  throw new ArgumentException("ProjectionImpl.latLonToProj:from array not same length as to array");
		}

		return convert2xy.latLonToProj(from, to, latIndex, lonIndex);
	  }

	  public override float[][] latLonToProj(float[][] from, float[][] to, int latIndex, int lonIndex)
	  {
		if ((from == null) || (from.Length != 2))
		{
		  throw new ArgumentException("ProjectionImpl.latLonToProj:null array argument or wrong dimension (from)");
		}
		if ((to == null) || (to.Length != 2))
		{
		  throw new ArgumentException("ProjectionImpl.latLonToProj:null array argument or wrong dimension (to)");
		}
		if (from[0].Length != to[0].Length)
		{
		  throw new ArgumentException("ProjectionImpl.latLonToProj:from array not same length as to array");
		}

		return convert2xy.latLonToProj(from, to, latIndex, lonIndex);
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
		return convert2latlon.projToLatLon(world.X, world.Y, result);
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
		if ((from == null) || (from.Length != 2))
		{
		  throw new ArgumentException("ProjectionImpl.projToLatLon:null array argument or wrong dimension (from)");
		}
		if ((to == null) || (to.Length != 2))
		{
		  throw new ArgumentException("ProjectionImpl.projToLatLon:null array argument or wrong dimension (to)");
		}

		if (from[0].Length != to[0].Length)
		{
		  throw new ArgumentException("ProjectionImpl.projToLatLon:from array not same length as to array");
		}

		return convert2latlon.projToLatLon(from, to);
	  }

	  public override double[][] projToLatLon(double[][] from, double[][] to)
	  {
		if ((from == null) || (from.Length != 2))
		{
		  throw new ArgumentException("ProjectionImpl.projToLatLon:null array argument or wrong dimension (from)");
		}
		if ((to == null) || (to.Length != 2))
		{
		  throw new ArgumentException("ProjectionImpl.projToLatLon:null array argument or wrong dimension (to)");
		}

		if (from[0].Length != to[0].Length)
		{
		  throw new ArgumentException("ProjectionImpl.projToLatLon:from array not same length as to array");
		}

		return convert2latlon.projToLatLon(from, to);
	  }

	}



}
