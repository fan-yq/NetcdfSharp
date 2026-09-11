// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

/*
 * Copyright (c) 1998-2019 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
/*
 * This software is provided by the National Aeronatics and Space
 * Administration Goddard Institute for Space Studies (NASA GISS) for full,
 * free and open release. It is understood by the recipient/user that NASA
 * assumes no liability for any errors contained in the code. Although this
 * software is released without conditions or restrictions in its use, it is
 * expected that appropriate credit be given to its author and to NASA GISS
 * should the software be included by the recipient in acknowledgments or
 * credits in other product development.
 */
namespace NetcdfSharp.ucar.unidata.geoloc.projection
{
	using CF = NetcdfSharp.ucar.nc2.constants.CF;
	using NetcdfSharp.ucar.unidata.geoloc;

	/// <summary>
	/// Rotated-pole longitude-latitude grid.
	/// 
	/// This is probably the same as rotated lat lon, using matrix to do rotation.
	/// Follows CF convention with "north_pole lat/lon", whereas RotatedLatLon uses south pole.
	/// 
	/// @author Robert Schmunk
	/// @author jcaron
	/// </summary>
	[Serializable]
	public class RotatedPole : ProjectionImpl
	{

	  private static readonly NetcdfSharp.org.slf4j.Logger log = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(RotatedPole));

	  private static readonly double RAD_PER_DEG = Math.PI / 180.0;
	  private static readonly double DEG_PER_RAD = 1.0 / RAD_PER_DEG;

	  /* Coordinates of north pole for rotated pole. */
	  private readonly ProjectionPoint northPole;

	  /* Y-axis rotation matrix. */
	  private readonly double[][] rotY = new double[][] { new double[3], new double[3], new double[3] };

	  /* Z-axis rotation matrix. */
	  private readonly double[][] rotZ = new double[][] { new double[3], new double[3], new double[3] };

	  /// <summary>
	  /// Default Constructor, needed for beans.
	  /// </summary>
	  public RotatedPole() : this(0.0, 0.0)
	  {
	  }

	  /// <summary>
	  /// Constructor.
	  /// </summary>
	  public RotatedPole(double northPoleLat, double northPoleLon) : base("RotatedPole", false)
	  {

		northPole = ProjectionPoint.create(northPoleLon, northPoleLat);
		buildRotationMatrices();

		addParameter(CF.GRID_MAPPING_NAME, CF.ROTATED_LATITUDE_LONGITUDE);
		addParameter(CF.GRID_NORTH_POLE_LATITUDE, northPoleLat);
		addParameter(CF.GRID_NORTH_POLE_LONGITUDE, northPoleLon);
	  }

	  private void buildRotationMatrices()
	  {

		double betaRad;
		double gammaRad;

		if (northPole.Y == 90.0)
		{
		  betaRad = 0.0;
		  gammaRad = northPole.X * RAD_PER_DEG;
		}
		else
		{
		  betaRad = -(90.0 - northPole.Y) * RAD_PER_DEG;
		  gammaRad = (northPole.X + 180.0) * RAD_PER_DEG;
		}

		double cosBeta = Math.Cos(betaRad);
		double sinBeta = Math.Sin(betaRad);

		double cosGamma = Math.Cos(gammaRad);
		double sinGamma = Math.Sin(gammaRad);

		rotY[0][0] = cosBeta;
		rotY[0][1] = 0.0;
		rotY[0][2] = -sinBeta;
		rotY[1][0] = 0.0;
		rotY[1][1] = 1.0;
		rotY[1][2] = 0.0;
		rotY[2][0] = sinBeta;
		rotY[2][1] = 0.0;
		rotY[2][2] = cosBeta;

		rotZ[0][0] = cosGamma;
		rotZ[0][1] = sinGamma;
		rotZ[0][2] = 0.0;
		rotZ[1][0] = -sinGamma;
		rotZ[1][1] = cosGamma;
		rotZ[1][2] = 0.0;
		rotZ[2][0] = 0.0;
		rotZ[2][1] = 0.0;
		rotZ[2][2] = 1.0;
	  }

	  public virtual ProjectionPoint NorthPole
	  {
		  get
		  {
			return northPole;
		  }
	  }

	  public override ProjectionImpl constructCopy()
	  {
		ProjectionImpl result = new RotatedPole(northPole.Y, northPole.X);
		result.DefaultMapArea = defaultMapArea;
		result.Name = name;
		return result;
	  }

	  public override string paramsToString()
	  {
		return " northPole= " + northPole;
	  }

	  /// <summary>
	  /// Transform a "real" longitude and latitude to the rotated longitude (X) and
	  /// rotated latitude (Y).
	  /// </summary>
	  public override ProjectionPoint latLonToProj(LatLonPoint latlon, ProjectionPointImpl destPoint)
	  {
		double lon = latlon.Longitude;
		double lat = latlon.Latitude;

		double lonRad = ((lon) * (Math.PI / 180.0));
		double latRad = ((lat) * (Math.PI / 180.0));

		// Lon-lat pair to xyz coordinates on sphere with radius 1
		double[] p0 = new double[] {Math.Cos(latRad) * Math.Cos(lonRad), Math.Cos(latRad) * Math.Sin(lonRad), Math.Sin(latRad)};

		// Rotate around Z-axis
		// double[] p1 = new double[] {
		// rotZ[0][0] * p0[0] + rotZ[0][1] * p0[1] + rotZ[0][2] * p0[2],
		// rotZ[1][0] * p0[0] + rotZ[1][1] * p0[1] + rotZ[1][2] * p0[2],
		// rotZ[2][0] * p0[0] + rotZ[2][1] * p0[1] + rotZ[2][2] * p0[2]};
		double[] p1 = new double[] {rotZ[0][0] * p0[0] + rotZ[0][1] * p0[1], rotZ[1][0] * p0[0] + rotZ[1][1] * p0[1], p0[2]};

		// Rotate around Y-axis
		// double[] p2 = new double[] {
		// rotY[0][0] * p1[0] + rotY[0][1] * p1[1] + rotY[0][2] * p1[2],
		// rotY[1][0] * p1[0] + rotY[1][1] * p1[1] + rotY[1][2] * p1[2],
		// rotY[2][0] * p1[0] + rotY[2][1] * p1[1] + rotY[2][2] * p1[2]};
		double[] p2 = new double[] {rotY[0][0] * p1[0] + rotY[0][2] * p1[2], p1[1], rotY[2][0] * p1[0] + rotY[2][2] * p1[2]};

		double lonR = LatLonPoints.range180(Math.Atan2(p2[1], p2[0]) * DEG_PER_RAD);
		double latR = Math.Asin(p2[2]) * DEG_PER_RAD;

		if (destPoint == null)
		{
		  return ProjectionPoint.create(lonR, latR);
		}
		else
		{
		  destPoint.setLocation(lonR, latR);
		  return destPoint;
		}

	  }

	  /// <summary>
	  /// Transform a rotated longitude (X) and rotated latitude (Y) to a "real"
	  /// longitude-latitude pair.
	  /// </summary>
	  public override LatLonPoint projToLatLon(ProjectionPoint ppt, LatLonPointImpl destPoint)
	  {
		// "x" and "y" input for rotated pole coords are actually a lon-lat pair
		double lonR = LatLonPoints.range180(ppt.X); // LOOK guessing -- shouldn't matter
		double latR = ppt.Y;

		if (Math.Abs(latR) > 90.0)
		{
		  throw new ArgumentException("ProjectionPoint y must be in range [-90,90].");
		}

		double lonRRad = ((lonR) * (Math.PI / 180.0));
		double latRRad = ((latR) * (Math.PI / 180.0));

		// Lon-lat pair to xyz coordinates on sphere with radius 1
		double[] p0 = new double[] {Math.Cos(latRRad) * Math.Cos(lonRRad), Math.Cos(latRRad) * Math.Sin(lonRRad), Math.Sin(latRRad)};

		// Inverse rotate around Y-axis (using transpose of Y matrix)
		// final double[] p1 = new double[] {
		// rotY[0][0] * p0[0] + rotY[1][0] * p0[1] + rotY[2][0] * p0[2],
		// rotY[0][1] * p0[0] + rotY[1][1] * p0[1] + rotY[2][1] * p0[2],
		// rotY[0][2] * p0[0] + rotY[1][2] * p0[1] + rotY[2][2] * p0[2]};
		double[] p1 = new double[] {rotY[0][0] * p0[0] + rotY[2][0] * p0[2], p0[1], rotY[0][2] * p0[0] + rotY[2][2] * p0[2]};

		// Inverse rotate around Z-axis (using transpose of Z matrix)
		// final double[] p2 = new double[] {
		// rotZ[0][0] * p1[0] + rotZ[1][0] * p1[1] + rotZ[2][0] * p1[2],
		// rotZ[0][1] * p1[0] + rotZ[1][1] * p1[1] + rotZ[2][1] * p1[2],
		// rotZ[0][2] * p1[0] + rotZ[1][2] * p1[1] + rotZ[2][2] * p1[2]};
		double[] p2 = new double[] {rotZ[0][0] * p1[0] + rotZ[1][0] * p1[1], rotZ[0][1] * p1[0] + rotZ[1][1] * p1[1], p1[2]};

		double lon = Math.Atan2(p2[1], p2[0]) * DEG_PER_RAD;
		double lat = Math.Asin(p2[2]) * DEG_PER_RAD;

		if (destPoint == null)
		{
		  return LatLonPoint.create(lat, lon);
		}
		else
		{
		  destPoint.set(lat, lon);
		  return destPoint;
		}
	  }

	  /// 
	  public override bool crossSeam(ProjectionPoint pt1, ProjectionPoint pt2)
	  {
		return Math.Abs(pt1.X - pt2.X) > 270.0;
	  }

	  /// 
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

		RotatedPole that = (RotatedPole) o;

		return northPole.Equals(that.northPole);
	  }

	  /// 
	  public override int GetHashCode()
	  {
		return northPole.GetHashCode();
	  }
	}

}
