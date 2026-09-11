// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

/*
 * Copyright (c) 1998-2020 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.unidata.geoloc
{
	using LatLonProjection = NetcdfSharp.ucar.unidata.geoloc.projection.LatLonProjection;

	/// <summary>
	/// Static utilities for Projection.
	/// </summary>
	public class Projections
	{
	  private const int INDEX_LAT = 0;
	  private const int INDEX_LON = 1;

	  ///////////////////////////////////////////////////////////////////////////////////
	  // optimizations for doing double and float arrays

	  /// <summary>
	  /// Convert projection coordinates to lat/lon coordinates.
	  /// </summary>
	  /// <param name="from"> array of projection coordinates: from[2][n],
	  ///        where from[0][i], from[1][i] is the x, y coordinate
	  ///        of the ith point </param>
	  /// <returns> resulting array of lat/lon coordinates, where to[0][i], to[1][i]
	  ///         is the lat,lon coordinate of the ith point </returns>
	  public static double[][] projToLatLon(Projection proj, double[][] from)
	  {
		return projToLatLon(proj, from, new double[][] { new double[from[0].Length], new double[from[0].Length] });
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
	  public static double[][] projToLatLon(Projection proj, double[][] from, double[][] to)
	  {
		if ((from == null) || (from.Length != 2))
		{
		  throw new ArgumentException("ProjectionImpl.projToLatLon:" + "null array argument or wrong dimension (from)");
		}
		if ((to == null) || (to.Length != 2))
		{
		  throw new ArgumentException("ProjectionImpl.projToLatLon:" + "null array argument or wrong dimension (to)");
		}

		if (from[0].Length != to[0].Length)
		{
		  throw new ArgumentException("ProjectionImpl.projToLatLon:" + "from array not same length as to array");
		}

		for (int i = 0; i < from[0].Length; i++)
		{
		  LatLonPoint endL = proj.projToLatLon(from[0][i], from[1][i]);
		  to[0][i] = endL.Latitude;
		  to[1][i] = endL.Longitude;
		}

		return to;
	  }

	  /// <summary>
	  /// Convert projection coordinates to lat/lon coordinates.
	  /// </summary>
	  /// <param name="from"> array of projection coordinates: from[2][n],
	  ///        where from[0][i], from[1][i] is the x, y coordinate
	  ///        of the ith point </param>
	  /// <returns> resulting array of lat/lon coordinates, where to[0][i], to[1][i]
	  ///         is the lat,lon coordinate of the ith point </returns>
	  public static float[][] projToLatLon(Projection proj, float[][] from)
	  {
		return projToLatLon(proj, from, new float[][] { new float[from[0].Length], new float[from[0].Length] });
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
	  public static float[][] projToLatLon(Projection proj, float[][] from, float[][] to)
	  {
		if ((from == null) || (from.Length != 2))
		{
		  throw new ArgumentException("ProjectionImpl.projToLatLon:" + "null array argument or wrong dimension (from)");
		}
		if ((to == null) || (to.Length != 2))
		{
		  throw new ArgumentException("ProjectionImpl.projToLatLon:" + "null array argument or wrong dimension (to)");
		}

		if (from[0].Length != to[0].Length)
		{
		  throw new ArgumentException("ProjectionImpl.projToLatLon:" + "from array not same length as to array");
		}

		for (int i = 0; i < from[0].Length; i++)
		{
		  ProjectionPoint ppi = ProjectionPoint.create(from[0][i], from[1][i]);
		  LatLonPoint llpi = proj.projToLatLon(ppi);
		  to[0][i] = (float) llpi.Latitude;
		  to[1][i] = (float) llpi.Longitude;
		}

		return to;
	  }

	  /// <summary>
	  /// Convert lat/lon coordinates to projection coordinates.
	  /// </summary>
	  /// <param name="from"> array of lat/lon coordinates: from[2][n],
	  ///        where from[0][i], from[1][i] is the (lat,lon)
	  ///        coordinate of the ith point </param>
	  /// <returns> resulting array of projection coordinates, where to[0][i],
	  ///         to[1][i] is the (x,y) coordinate of the ith point </returns>
	  public static double[][] latLonToProj(Projection proj, double[][] from)
	  {
		return latLonToProj(proj, from, new double[][] { new double[from[0].Length], new double[from[0].Length] });
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
	  public static double[][] latLonToProj(Projection proj, double[][] from, double[][] to)
	  {
		return latLonToProj(proj, from, to, INDEX_LAT, INDEX_LON);
	  }

	  /// <summary>
	  /// Convert lat/lon coordinates to projection coordinates.
	  /// </summary>
	  /// <param name="from"> array of lat/lon coordinates: from[2][n], where
	  ///        (from[latIndex][i], from[lonIndex][i]) is the (lat,lon)
	  ///        coordinate of the ith point </param>
	  /// <param name="latIndex"> index of lat coordinate; must be 0 or 1 </param>
	  /// <param name="lonIndex"> index of lon coordinate; must be 0 or 1 </param>
	  /// <returns> resulting array of projection coordinates: to[2][n] where
	  ///         (to[0][i], to[1][i]) is the (x,y) coordinate of the ith point </returns>
	  public static double[][] latLonToProj(Projection proj, double[][] from, int latIndex, int lonIndex)
	  {
		return latLonToProj(proj, from, new double[][] { new double[from[0].Length], new double[from[0].Length] }, latIndex, lonIndex);
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
	  public static double[][] latLonToProj(Projection proj, double[][] from, double[][] to, int latIndex, int lonIndex)
	  {
		if ((from == null) || (from.Length != 2))
		{
		  throw new ArgumentException("ProjectionImpl.latLonToProj:" + "null array argument or wrong dimension (from)");
		}
		if ((to == null) || (to.Length != 2))
		{
		  throw new ArgumentException("ProjectionImpl.latLonToProj:" + "null array argument or wrong dimension (to)");
		}

		if (from[0].Length != to[0].Length)
		{
		  throw new ArgumentException("ProjectionImpl.latLonToProj:" + "from array not same length as to array");
		}

		for (int i = 0; i < from[0].Length; i++)
		{
		  LatLonPoint llpi = LatLonPoint.create(from[latIndex][i], from[lonIndex][i]);
		  ProjectionPoint ppi = proj.latLonToProj(llpi);
		  to[0][i] = ppi.X;
		  to[1][i] = ppi.Y;
		}
		return to;
	  }

	  /// <summary>
	  /// Convert lat/lon coordinates to projection coordinates.
	  /// </summary>
	  /// <param name="from"> array of lat/lon coordinates: from[2][n],
	  ///        where from[0][i], from[1][i] is the (lat,lon)
	  ///        coordinate of the ith point </param>
	  /// <returns> resulting array of projection coordinates, where to[0][i],
	  ///         to[1][i] is the (x,y) coordinate of the ith point </returns>
	  public static float[][] latLonToProj(Projection proj, float[][] from)
	  {
		return latLonToProj(proj, from, new float[][] { new float[from[0].Length], new float[from[0].Length] });
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
	  public static float[][] latLonToProj(Projection proj, float[][] from, float[][] to)
	  {
		return latLonToProj(proj, from, to, INDEX_LAT, INDEX_LON);
	  }

	  /// <summary>
	  /// Convert lat/lon coordinates to projection coordinates.
	  /// </summary>
	  /// <param name="from"> array of lat/lon coordinates: from[2][n], where
	  ///        (from[latIndex][i], from[lonIndex][i]) is the (lat,lon)
	  ///        coordinate of the ith point </param>
	  /// <param name="latIndex"> index of lat coordinate; must be 0 or 1 </param>
	  /// <param name="lonIndex"> index of lon coordinate; must be 0 or 1 </param>
	  /// <returns> resulting array of projection coordinates: to[2][n] where
	  ///         (to[0][i], to[1][i]) is the (x,y) coordinate of the ith point </returns>
	  public static float[][] latLonToProj(Projection proj, float[][] from, int latIndex, int lonIndex)
	  {
		return latLonToProj(proj, from, new float[][] { new float[from[0].Length], new float[from[0].Length] }, latIndex, lonIndex);
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
	  public static float[][] latLonToProj(Projection proj, float[][] from, float[][] to, int latIndex, int lonIndex)
	  {
		if ((from == null) || (from.Length != 2))
		{
		  throw new ArgumentException("ProjectionImpl.latLonToProj:" + "null array argument or wrong dimension (from)");
		}
		if ((to == null) || (to.Length != 2))
		{
		  throw new ArgumentException("ProjectionImpl.latLonToProj:" + "null array argument or wrong dimension (to)");
		}

		if (from[0].Length != to[0].Length)
		{
		  throw new ArgumentException("ProjectionImpl.latLonToProj:" + "from array not same length as to array");
		}

		for (int i = 0; i < from[0].Length; i++)
		{
		  LatLonPoint llpi = LatLonPoint.create(from[latIndex][i], from[lonIndex][i]);
		  ProjectionPoint ppi = proj.latLonToProj(llpi);
		  to[0][i] = (float) ppi.X;
		  to[1][i] = (float) ppi.Y;
		}

		return to;
	  }

	}

}
