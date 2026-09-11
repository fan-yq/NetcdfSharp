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
//
// Filename: Gdc_To_Utm_Converter.java
//
// Author: Dan Toms, SRI International
//
// Package: GeoTransform <http://www.ai.sri.com/geotransformtest/>
//
// Acknowledgements:
// The algorithms used in the package were created by Ralph Toms and
// first appeared as part of the SEDRIS Coordinate Transformation API.
// These were subsequently modified for this package. This package is
// not part of the SEDRIS project, and the Java code written for this
// package has not been certified or tested for correctness by NIMA.
//
// License:
// The contents of this file are subject to GeoTransform License Agreement
// (the "License"); you may not use this file except in compliance with
// the License. You may obtain a copy of the License at
// http://www.ai.sri.com/geotransformtest/license.html
//
// Software distributed under the License is distributed on an "AS IS"
// basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See
// the License for the specific language governing rights and limitations
// under the License.
//
// Portions are Copyright (c) SRI International, 1998.
//

namespace NetcdfSharp.ucar.unidata.geoloc.projection
{

	using EarthEllipsoid = NetcdfSharp.ucar.unidata.geoloc.EarthEllipsoid;
	using LatLonPoints = NetcdfSharp.ucar.unidata.geoloc.LatLonPoints;
	using ProjectionPoint = NetcdfSharp.ucar.unidata.geoloc.ProjectionPoint;
	using ProjectionPointImpl = NetcdfSharp.ucar.unidata.geoloc.ProjectionPointImpl;

	/// <summary>
	/// Converts GDC coordinate(s) to UTM.
	/// <p/>
	/// This class provides the capability to convert from
	/// geodetic (GDC), i.e. lat/long coordinates to
	/// Universal Transverse Mercator (UTM).
	/// This is a direct conversion.
	/// 
	/// @author Dan Toms, SRI International
	///         <p/>
	///         modified JCaron 01/2005
	///         <ol>
	///         <li>turn static methods into object methods, to make thread-safe
	///         <li>rename methods to follow upper/lower case conventions
	///         <li>add convenience methods for NetcdfSharp.ucar.unidata.geoloc.Projection
	///         <li>longitude must be in range +=180.
	///         </ol>
	///         <p/>
	///         random testing shows:
	///         avg error x= 0.4 y=0.06 meters
	///         but sometimes x error can be as high as 15 meters
	///         where err = abs(x - inverse(f(x)))
	///         <p/>
	///         timing: inverse(f(x)) takes 2 - 3 microseconds.
	/// </summary>

	internal class Gdc_To_Utm_Converter
	{
	  internal const double RADIANS_PER_DEGREE = 0.0174532925199432957692;

	  private double A;
	  private double F; // flattening
	  private double Eps2, Eps25, Epps2, CScale = .9996, poly1b, poly2b, poly3b, poly4b, poly5b;
	  private double axlon0, axlon0_deg;

	  /// <summary>
	  /// Constructor.
	  /// </summary>
	  /// <param name="a"> the semi-major axis (meters) for the ellipsoid </param>
	  /// <param name="f"> the inverse flattening for the ellipsoid </param>
	  /// <param name="zone"> the UTM zone number (1..60) </param>
	  /// <param name="hemisphere_north"> true if the UTM coordinate is in the northern hemisphere </param>
	  public Gdc_To_Utm_Converter(double a, double f, int zone, bool hemisphere_north)
	  {
		init(a, f, zone, hemisphere_north);
	  }

	  /// <summary>
	  /// Default contructor uses WGS 84 ellipsoid
	  /// </summary>
	  /// <param name="zone"> the UTM zone number (1..60) </param>
	  /// <param name="hemisphere_north"> true if the UTM coordinate is in the northern hemisphere </param>
	  public Gdc_To_Utm_Converter(int zone, bool hemisphere_north) : this(EarthEllipsoid.WGS84, zone, hemisphere_north) // default to wgs 84
	  {
	  }

	  /// <summary>
	  /// Constructor with ellipsoid.
	  /// </summary>
	  /// <param name="ellipse"> an EarthEllipsoid, e.g. WE_Ellipsoid </param>
	  /// <param name="zone"> the UTM zone number (1..60) </param>
	  /// <param name="isNorth"> true if the UTM coordinate is in the northern hemisphere </param>
	  public Gdc_To_Utm_Converter(EarthEllipsoid ellipse, int zone, bool isNorth)
	  {
		init(ellipse.Major, 1.0 / ellipse.Flattening, zone, isNorth);
	  }


	  /// <summary>
	  /// _more_
	  /// </summary>
	  /// <param name="a"> major axis </param>
	  /// <param name="f"> inverse flattening </param>
	  /// <param name="zone"> _more_ </param>
	  /// <param name="isNorth"> _more_ </param>
	  protected internal virtual void init(double a, double f, int zone, bool isNorth)
	  {
		A = a;
		F = 1.0 / f; // F is flattening
		this.axlon0_deg = (zone * 6 - 183);
		this.axlon0 = axlon0_deg * RADIANS_PER_DEGREE;

		double polx2b, polx3b, polx4b, polx5b;

		// Create the ERM constants.
		Eps2 = (F) * (2.0 - F);
		Eps25 = .25 * (Eps2);
		Epps2 = (Eps2) / (1.0 - Eps2);
		polx2b = Eps2 + 1.0 / 4.0 * Math.Pow(Eps2, 2) + 15.0 / 128.0 * Math.Pow(Eps2, 3) - 455.0 / 4096.0 * Math.Pow(Eps2, 4);

		polx2b = 3.0 / 8.0 * polx2b;
		polx3b = Math.Pow(Eps2, 2) + 3.0 / 4.0 * Math.Pow(Eps2, 3) - 77.0 / 128.0 * Math.Pow(Eps2, 4);
		polx3b = 15.0 / 256.0 * polx3b;
		polx4b = Math.Pow(Eps2, 3) - 41.0 / 32.0 * Math.Pow(Eps2, 4);
		polx4b = polx4b * 35.0 / 3072.0;
		polx5b = -315.0 / 131072.0 * Math.Pow(Eps2, 4);

		poly1b = 1.0 - (1.0 / 4.0 * Eps2) - (3.0 / 64.0 * Math.Pow(Eps2, 2)) - (5.0 / 256.0 * Math.Pow(Eps2, 3)) - (175.0 / 16384.0 * Math.Pow(Eps2, 4));

		poly2b = polx2b * -2.0 + polx3b * 4.0 - polx4b * 6.0 + polx5b * 8.0;

		poly3b = polx3b * -8.0 + polx4b * 32.0 - polx5b * 80.0;

		poly4b = polx4b * -32.0 + polx5b * 192.0;

		poly5b = polx5b * -128.0;
	  }

	  /// <summary>
	  /// get central meridian in degrees (depends on zone)
	  /// </summary>
	  /// <returns> central meridian in degrees </returns>
	  public virtual double CentralMeridian
	  {
		  get
		  {
			return this.axlon0_deg;
		  }
	  }

	  public virtual ProjectionPoint latLonToProj(double latitude, double longitude, ProjectionPointImpl result)
	  {
		double source_lat, source_lon, s1, c1, tx, s12, rn, al, al2, sm, tn2, cee, poly1, poly2;

		longitude = LatLonPoints.lonNormal(longitude, axlon0_deg); // normalize to the central meridian

		source_lat = latitude * RADIANS_PER_DEGREE;
		source_lon = longitude * RADIANS_PER_DEGREE;

		s1 = Math.Sin(source_lat);
		c1 = Math.Cos(source_lat);
		tx = s1 / c1;
		s12 = s1 * s1;

		rn = A / ((.25 - Eps25 * s12 + .9999944354799 / 4) + (.25 - Eps25 * s12) / (.25 - Eps25 * s12 + .9999944354799 / 4));

		al = (source_lon - axlon0) * c1;
		sm = s1 * c1 * (poly2b + s12 * (poly3b + s12 * (poly4b + s12 * poly5b)));
		sm = A * (poly1b * source_lat + sm);

		tn2 = tx * tx;
		cee = Epps2 * c1 * c1;
		al2 = al * al;
		poly1 = 1.0 - tn2 + cee;
		poly2 = 5.0 + tn2 * (tn2 - 18.0) + cee * (14.0 - tn2 * 58.0);

		double x = CScale * rn * al * (1.0 + al2 * (.166666666666667 * poly1 + .00833333333333333 * al2 * poly2));
		x += 5.0E5;

		poly1 = 5.0 - tn2 + cee * (cee * 4.0 + 9.0);
		poly2 = 61.0 + tn2 * (tn2 - 58.0) + cee * (270.0 - tn2 * 330.0);
		double y = CScale * (sm + rn * tx * al2 * (0.5 + al2 * (.0416666666666667 * poly1 + .00138888888888888 * al2 * poly2)));

		if (source_lat < 0.0)
		{
		  y += 1.0E7;
		}

		result.setLocation(x * .001, y * .001); // wants km
		return result;
	  }


	  public virtual double[][] latLonToProj(double[][] from, double[][] to, int latIndex, int lonIndex)
	  {
		double source_lat, source_lon, s1, c1, tx, s12, rn, al, al2, sm, tn2, cee, poly1, poly2;

		for (int i = 0; i < from[0].Length; i++)
		{
		  double longitude = LatLonPoints.lonNormal(from[lonIndex][i], axlon0_deg); // normalize to the central meridian
		  source_lat = from[latIndex][i] * RADIANS_PER_DEGREE;
		  source_lon = longitude * RADIANS_PER_DEGREE;

		  s1 = Math.Sin(source_lat);
		  c1 = Math.Cos(source_lat);
		  tx = s1 / c1;
		  s12 = s1 * s1;

		  rn = A / ((.25 - Eps25 * s12 + .9999944354799 / 4) + (.25 - Eps25 * s12) / (.25 - Eps25 * s12 + .9999944354799 / 4));

		  al = (source_lon - axlon0) * c1;
		  sm = s1 * c1 * (poly2b + s12 * (poly3b + s12 * (poly4b + s12 * poly5b)));
		  sm = A * (poly1b * source_lat + sm);

		  tn2 = tx * tx;
		  cee = Epps2 * c1 * c1;
		  al2 = al * al;
		  poly1 = 1.0 - tn2 + cee;
		  poly2 = 5.0 + tn2 * (tn2 - 18.0) + cee * (14.0 - tn2 * 58.0);

		  double x = CScale * rn * al * (1.0 + al2 * (.166666666666667 * poly1 + .00833333333333333 * al2 * poly2));
		  x += 5.0E5;

		  poly1 = 5.0 - tn2 + cee * (cee * 4.0 + 9.0);
		  poly2 = 61.0 + tn2 * (tn2 - 58.0) + cee * (270.0 - tn2 * 330.0);
		  double y = CScale * (sm + rn * tx * al2 * (0.5 + al2 * (.0416666666666667 * poly1 + .00138888888888888 * al2 * poly2)));

		  if (source_lat < 0.0)
		  {
			y += 1.0E7;
		  }

		  to[0][i] = x * .001;
		  to[1][i] = y * .001;
		}

		return to;
	  }

	  public virtual float[][] latLonToProj(float[][] from, float[][] to, int latIndex, int lonIndex)
	  {
		double source_lat, source_lon, s1, c1, tx, s12, rn, al, al2, sm, tn2, cee, poly1, poly2;

		for (int i = 0; i < from[0].Length; i++)
		{
		  double longitude = LatLonPoints.lonNormal(from[lonIndex][i], axlon0_deg); // normalize to the central meridian
		  source_lat = from[latIndex][i] * RADIANS_PER_DEGREE;
		  source_lon = longitude * RADIANS_PER_DEGREE;

		  s1 = Math.Sin(source_lat);
		  c1 = Math.Cos(source_lat);
		  tx = s1 / c1;
		  s12 = s1 * s1;

		  rn = A / ((.25 - Eps25 * s12 + .9999944354799 / 4) + (.25 - Eps25 * s12) / (.25 - Eps25 * s12 + .9999944354799 / 4));

		  al = (source_lon - axlon0) * c1;
		  sm = s1 * c1 * (poly2b + s12 * (poly3b + s12 * (poly4b + s12 * poly5b)));
		  sm = A * (poly1b * source_lat + sm);

		  tn2 = tx * tx;
		  cee = Epps2 * c1 * c1;
		  al2 = al * al;
		  poly1 = 1.0 - tn2 + cee;
		  poly2 = 5.0 + tn2 * (tn2 - 18.0) + cee * (14.0 - tn2 * 58.0);

		  double x = CScale * rn * al * (1.0 + al2 * (.166666666666667 * poly1 + .00833333333333333 * al2 * poly2));
		  x += 5.0E5;

		  poly1 = 5.0 - tn2 + cee * (cee * 4.0 + 9.0);
		  poly2 = 61.0 + tn2 * (tn2 - 58.0) + cee * (270.0 - tn2 * 330.0);
		  double y = CScale * (sm + rn * tx * al2 * (0.5 + al2 * (.0416666666666667 * poly1 + .00138888888888888 * al2 * poly2)));

		  if (source_lat < 0.0)
		  {
			y += 1.0E7;
		  }

		  to[0][i] = (float)(x * .001);
		  to[1][i] = (float)(y * .001);
		}

		return to;
	  }


	}


}
