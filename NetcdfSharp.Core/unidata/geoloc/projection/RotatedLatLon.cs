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
	/// Grib 1 projection 10 and Grib 2 projection 1.
	/// 
	/// The Rotated Latitude Longitude projection algorithms that are coded
	/// here were given by Tor Christian Bekkvik <torc@cmr.no>. The rotated
	/// lat/lon projection coordinates are defined in the grid file that
	/// need to be converted back to unrotated lat/lon projection coordinates
	/// before they can be displayed. The X/Y axis only makes sense in the rotated
	/// projection.
	/// 
	/// @author Tor Christian Bekkvik <torc@cmr.no>
	/// @since Nov 11, 2008
	/// </summary>

	/*
	 * 
	 * Grib-1 doc: (http://rda.ucar.edu/docs/formats/grib/gribdoc/llgrid.html)
	 * 
	 * Three parameters define a general latitude/longitude coordinate system, formed by a general rotation of the sphere.
	 * One choice for these parameters is: <ol>
	 * <l1> The geographic latitude in degrees of the southern pole of the coordinate system, thetap for example;
	 * <li> The geographic longitude in degrees of the southern pole of the coordinate system, lamdap for example;
	 * <li> The angle of rotation in degrees about the new polar axis (measured clockwise when looking from the southern to
	 * the
	 * northern pole) of the coordinate system, assuming the new axis to have been obtained by first rotating the sphere
	 * through lamdap degrees about
	 * the geographic polar axis, and then rotating through (90 + thetap ) degrees so that the southern pole moved along the
	 * (previously
	 * rotated) Greenwhich meridian. </ol>
	 * 
	 * Grib2:
	 * 
	 * Grid Definition Template 3.1: Rotated Latitude/longitude (or equidistant cylindrical, or Plate Carrée)
	 * 
	 * Octet No. Contents
	 * 15-72 Same as Grid Definition Template 3.0 (see Note 1)
	 * 73-76 Latitude of the southern pole of projection
	 * 77-80 Longitude of the southern pole of projection
	 * 81-84 Angle of rotation of projection
	 * 85-nn List of number of points along each meridian or parallel (These octets are only present for quasi-regular grids
	 * as described in Note 3)
	 * 
	 * Notes:
	 * (1) Basic angle of the initial production domain and subdivisions of this basic angle are provided to manage cases
	 * where the
	 * recommended unit of 10-6 degrees is not applicable to describe the extreme longitudes and latitudes, and direction
	 * increments.
	 * For these last six descriptors, unit is equal to the ratio of the basic angle and the subdivisions number. For
	 * ordinary cases,
	 * zero and missing values should be coded, equivalent to respective values of 1 and 106 (10-6 degrees unit).
	 * (2) Three parameters define a general latitude/longitude coordinate system, formed by a general rotation of the
	 * sphere.
	 * One choice for these parameters is:
	 * (a) The geographic latitude in degrees of the southern pole of the coordinate system, θp for example.
	 * (b) The geographic longitude in degrees of the southern pole of the coordinate system, λp for example.
	 * (c) The angle of rotation in degrees about the new polar axis (measured clockwise when looking from the southern to
	 * the northern pole) of the coordinate system, assuming the new axis to have been obtained by first rotating the sphere
	 * through λp degrees about the geographic polar axis, and then rotating through (90 + θp) degrees so that the southern
	 * pole moved along the (previously rotated) Greenwich meridian.
	 * 
	 */

	[Serializable]
	public class RotatedLatLon : ProjectionImpl
	{
	  public const string GRID_MAPPING_NAME = "rotated_latlon_grib";
	  public const string GRID_SOUTH_POLE_LONGITUDE = "grid_south_pole_longitude";
	  public const string GRID_SOUTH_POLE_LATITUDE = "grid_south_pole_latitude";
	  public const string GRID_SOUTH_POLE_ANGLE = "grid_south_pole_angle";

	  private static bool show;

	  private readonly double lonpole; // Longitude of south pole
	  private readonly double latpole; // Latitude of south pole
	  private readonly double polerotate; // Angle of south pole rotation

	  private double cosDlat;
	  private double sinDlat;

	  /// <summary>
	  /// Default Constructor, needed for beans.
	  /// </summary>
	  public RotatedLatLon() : this(0.0, 0.0, 0.0)
	  {
	  }

	  /// <summary>
	  /// Constructor.
	  /// </summary>
	  /// <param name="southPoleLat"> in degrees </param>
	  /// <param name="southPoleLon"> in degrees </param>
	  /// <param name="southPoleAngle"> in degrees </param>
	  public RotatedLatLon(double southPoleLat, double southPoleLon, double southPoleAngle) : base("RotatedLatLon", false)
	  {

		/*
		 * lonsp = aLonsp;
		 * latsp = aLatsp;
		 * rotsp = aRotsp;
		 * double dlat_rad = (latsp - (-90)) * DEG2RAD; //delta latitude
		 * sinDlat = Math.sin(dlat_rad);
		 * cosDlat = Math.cos(dlat_rad);
		 */
		this.latpole = southPoleLat;
		this.lonpole = southPoleLon;
		this.polerotate = southPoleAngle;
		double dlat_rad = ((latpole - (-90)) * (Math.PI / 180.0));
		sinDlat = Math.Sin(dlat_rad);
		cosDlat = Math.Cos(dlat_rad);

		addParameter(CF.GRID_MAPPING_NAME, GRID_MAPPING_NAME);
		addParameter(GRID_SOUTH_POLE_LATITUDE, southPoleLat);
		addParameter(GRID_SOUTH_POLE_LONGITUDE, southPoleLon);
		addParameter(GRID_SOUTH_POLE_ANGLE, southPoleAngle);
	  }

	  public virtual double Lonpole
	  {
		  get
		  {
			return lonpole;
		  }
	  }

	  public virtual double Polerotate
	  {
		  get
		  {
			return polerotate;
		  }
	  }

	  public virtual double SinDlat
	  {
		  get
		  {
			return sinDlat;
		  }
	  }

	  public override ProjectionImpl constructCopy()
	  {
		ProjectionImpl result = new RotatedLatLon(latpole, lonpole, polerotate);
		result.DefaultMapArea = defaultMapArea;
		result.Name = name;
		return result;
	  }

	  /// <summary>
	  /// returns constructor params as a String
	  /// </summary>
	  /// <returns> String </returns>
	  public override string paramsToString()
	  {
		return " southPoleLat =" + latpole + " southPoleLon =" + lonpole + " southPoleAngle =" + polerotate;
	  }

	  /// <summary>
	  /// Transform a "real" longitude and latitude into the rotated longitude (X) and
	  /// rotated latitude (Y).
	  /// </summary>
	  public override ProjectionPoint latLonToProj(LatLonPoint latlon, ProjectionPointImpl destPoint)
	  {
		/*
		 * Tor's algorithm
		 * public double[] fwd(double[] lonlat)
		 * return transform(lonlat, lonpole, polerotate, sinDlat);
		 */
		double[] lonlat = new double[2];
		lonlat[0] = latlon.Longitude;
		lonlat[1] = latlon.Latitude;

		double[] rlonlat = rotate(lonlat, lonpole, polerotate, sinDlat);
		if (destPoint == null)
		{
		  destPoint = new ProjectionPointImpl(rlonlat[0], rlonlat[1]);
		}
		else
		{
		  destPoint.setLocation(rlonlat[0], rlonlat[1]);
		}

		if (show)
		{
		  Console.WriteLine("LatLon= " + latlon + " proj= " + destPoint);
		}

		return destPoint;
	  }

	  /// <summary>
	  /// Transform a rotated longitude (X) and rotated latitude (Y) into a "real"
	  /// longitude-latitude pair.
	  /// </summary>
	  public override LatLonPoint projToLatLon(ProjectionPoint ppt, LatLonPointImpl destPoint)
	  {
		/*
		 * Tor's algorithm
		 * public double[] inv(double[] lonlat)
		 * return rotate(lonlat, -polerotate, -lonpole, -sinDlat);
		 */
		double[] lonlat = new double[2];
		lonlat[0] = ppt.X;
		lonlat[1] = ppt.Y;

		double[] rlonlat = rotate(lonlat, -polerotate, -lonpole, -sinDlat);

		if (destPoint == null)
		{
		  return LatLonPoint.create(rlonlat[1], rlonlat[0]);
		}
		else
		{
		  destPoint.set(rlonlat[1], rlonlat[0]);
		  return destPoint;
		}
	  }

	  // Tor's transform algorithm renamed to rotate for clarity
	  internal virtual double[] rotate(double[] lonlat, double rot1, double rot2, double s)
	  {

		/*
		 * original code
		 * double e = DEG2RAD * (lonlat[0] - rot1); //east
		 * double n = DEG2RAD * lonlat[1]; //north
		 * double cn = Math.cos(n);
		 * double x = cn * Math.cos(e);
		 * double y = cn * Math.sin(e);
		 * double z = Math.sin(n);
		 * double x2 = cosDlat * x + s * z;
		 * double z2 = -s * x + cosDlat * z;
		 * double R = Math.sqrt(x2 * x2 + y * y);
		 * double e2 = Math.atan2(y, x2);
		 * double n2 = Math.atan2(z2, R);
		 * double rlon = RAD2DEG * e2 - rot2;
		 * double rlat = RAD2DEG * n2;
		 * return new double[]{rlon, rlat};
		 */

		double e = ((lonlat[0] - rot1) * (Math.PI / 180.0)); // east
		double n = ((lonlat[1]) * (Math.PI / 180.0)); // north
		double cn = Math.Cos(n);
		double x = cn * Math.Cos(e);
		double y = cn * Math.Sin(e);
		double z = Math.Sin(n);
		double x2 = cosDlat * x + s * z;
		double z2 = -s * x + cosDlat * z;
		double R = Math.Sqrt(x2 * x2 + y * y);
		double e2 = Math.Atan2(y, x2);
		double n2 = Math.Atan2(z2, R);
		double rlon = ((e2) * (180.0 / Math.PI)) - rot2;
		double rlat = ((n2) * (180.0 / Math.PI));
		return new double[] {rlon, rlat};

	  }

	  public override bool crossSeam(ProjectionPoint pt1, ProjectionPoint pt2)
	  {
		return Math.Abs(pt1.X - pt2.X) > 270.0;
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

		RotatedLatLon that = (RotatedLatLon) o;

		if (that.latpole.CompareTo(latpole) != 0)
		{
		  return false;
		}
		if (that.lonpole.CompareTo(lonpole) != 0)
		{
		  return false;
		}
		if (that.polerotate.CompareTo(polerotate) != 0)
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
		temp = lonpole != 0.0d ? System.BitConverter.DoubleToInt64Bits(lonpole) : 0L;
		result = (int)(temp ^ (temp >>> 32));
		temp = latpole != 0.0d ? System.BitConverter.DoubleToInt64Bits(latpole) : 0L;
		result = 31 * result + (int)(temp ^ (temp >>> 32));
		temp = polerotate != 0.0d ? System.BitConverter.DoubleToInt64Bits(polerotate) : 0L;
		result = 31 * result + (int)(temp ^ (temp >>> 32));
		return result;
	  }

	}

}
