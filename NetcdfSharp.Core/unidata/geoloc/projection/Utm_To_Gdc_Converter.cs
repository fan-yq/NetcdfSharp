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
// Filename: Utm_To_Gdc_Converter.java
//
// Author: Dan Toms, SRI International
//
// Package: GeoTransform http://www.ai.sri.com/geotransform/
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
	using LatLonPoint = NetcdfSharp.ucar.unidata.geoloc.LatLonPoint;
	using LatLonPointImpl = NetcdfSharp.ucar.unidata.geoloc.LatLonPointImpl;


	/// <summary>
	/// Class: Utm_To_Gdc_Converter
	/// <p/>
	/// Converts UTM coordinate(s) to GDC.
	/// <p/>
	/// This class provides the capability to convert from
	/// Universal Transverse Mercator (UTM) coordinates to
	/// geodetic (GDC), i.e. lat/long.
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

	internal class Utm_To_Gdc_Converter
	{

	  internal const double DEGREES_PER_RADIAN = 57.2957795130823208768;

	  private double A; // major axis
	  private double F; // flattening
	  private double Eps2, Eps25, Con2, EF, Epsp2, Con6, Con24, Con120, Con720, polx2b, polx3b, polx4b, polx5b, conap;

	  private int zone;

	  private bool hemisphere_north;

	  /// <summary>
	  /// Constructor using a, f.
	  /// </summary>
	  /// <param name="a"> the semi-major axis (meters) for the ellipsoid </param>
	  /// <param name="f"> the inverse flattening for the ellipsoid </param>
	  /// <param name="zone"> the UTM zone number (1..60) </param>
	  /// <param name="hemisphere_north"> true if the UTM coordinate is in the northern hemisphere </param>
	  public Utm_To_Gdc_Converter(double a, double f, int zone, bool hemisphere_north)
	  {
		init(a, f, zone, hemisphere_north);
	  }

	  /// <summary>
	  /// Default contructor uses WGS 84 ellipsoid
	  /// </summary>
	  /// <param name="zone"> the UTM zone number (1..60) </param>
	  /// <param name="hemisphere_north"> true if the UTM coordinate is in the northern hemisphere </param>
	  public Utm_To_Gdc_Converter(int zone, bool hemisphere_north) : this(EarthEllipsoid.WGS84, zone, hemisphere_north) // default to wgs 84
	  {
	  }

	  /*
	   * Constructor with ellipsoid.
	   * 
	   * @param E an Ellipsoid instance for the ellipsoid, e.g. WE_Ellipsoid
	   * 
	   * @param zone the UTM zone number (1..60)
	   * 
	   * @param hemisphere_north true if the UTM coordinate is in the northern hemisphere
	   */

	  /// <summary>
	  /// Constructor using given ellipse
	  /// </summary>
	  /// <param name="ellipse"> use this ellipse. </param>
	  /// <param name="zone"> the UTM zone number (1..60) </param>
	  /// <param name="hemisphere_north"> true if the UTM coordinate is in the northern hemisphere </param>
	  public Utm_To_Gdc_Converter(EarthEllipsoid ellipse, int zone, bool hemisphere_north)
	  {
		init(ellipse.Major, 1.0 / ellipse.Flattening, zone, hemisphere_north);
	  }

	  /// <summary>
	  /// initialize
	  /// </summary>
	  /// <param name="a"> major axis </param>
	  /// <param name="f"> inverse flattening </param>
	  /// <param name="zone"> UTM zone </param>
	  /// <param name="hemisphere_north"> is in northern hemisphere </param>
	  private void init(double a, double f, int zone, bool hemisphere_north)
	  {
		A = a;
		F = 1.0 / f; // F is flattening
		this.zone = zone;
		this.hemisphere_north = hemisphere_north;

		// Create the ERM constants.
		Eps2 = (F) * (2.0 - F);
		Eps25 = .25 * (Eps2);
		EF = F / (2.0 - F);
		Con2 = 2 / (1.0 - Eps2);
		Con6 = .166666666666667;
		Con24 = 4 * .0416666666666667 / (1 - Eps2);
		Con120 = .00833333333333333;
		Con720 = 4 * .00138888888888888 / (1 - Eps2);
		double polx1a = 1.0 - Eps2 / 4.0 - 3.0 / 64.0 * Math.Pow(Eps2, 2) - 5.0 / 256.0 * Math.Pow(Eps2, 3) - 175.0 / 16384.0 * Math.Pow(Eps2, 4);

		conap = A * polx1a;

		double polx2a = 3.0 / 2.0 * EF - 27.0 / 32.0 * Math.Pow(EF, 3);

		double polx4a = 21.0 / 16.0 * Math.Pow(EF, 2) - 55.0 / 32.0 * Math.Pow(EF, 4);

		double polx6a = 151.0 / 96.0 * Math.Pow(EF, 3);

		double polx8a = 1097.0 / 512.0 * Math.Pow(EF, 4);

		polx2b = polx2a * 2.0 + polx4a * 4.0 + polx6a * 6.0 + polx8a * 8.0;

		polx3b = polx4a * -8.0 - polx6a * 32.0 - 80.0 * polx8a;
		polx4b = polx6a * 32.0 + 192.0 * polx8a;
		polx5b = -128.0 * polx8a;
	  }

	  public virtual int Zone
	  {
		  get
		  {
			return zone;
		  }
	  }

	  public virtual bool North
	  {
		  get
		  {
			return hemisphere_north;
		  }
	  }

	  /// <param name="x"> the UTM easting coordinate (meters) </param>
	  /// <param name="y"> the UTM northing coordinate (meters) </param>
	  /// <param name="latlon"> put result here </param>
	  /// <returns> LatLonPoint </returns>
	  public virtual LatLonPoint projToLatLon(double x, double y, LatLonPointImpl latlon)
	  {

		double source_x, source_y, u, su, cu, su2, xlon0, temp, phi1;
		double sp, sp2, cp, cp2, tp, tp2, eta2, top, rn, b3, b4, b5, b6, d1, d2;

		source_x = x * 1000.0; // wants meters
		source_x = (source_x - 500000.0) / .9996;

		if (hemisphere_north)
		{
		  source_y = y * 1000.0 / .9996;
		}
		else
		{
		  source_y = (y * 1000.0 - 1.0E7) / .9996;
		}

		u = source_y / conap;

		/* TEST U TO SEE IF AT POLES */
		su = Math.Sin(u);
		cu = Math.Cos(u);
		su2 = su * su;

		xlon0 = (6.0 * ((double) zone) - 183.0) / DEGREES_PER_RADIAN;
		temp = polx2b + su2 * (polx3b + su2 * (polx4b + su2 * polx5b));
		phi1 = u + su * cu * temp;

		sp = Math.Sin(phi1);
		cp = Math.Cos(phi1);
		tp = sp / cp;
		tp2 = tp * tp;
		sp2 = sp * sp;
		cp2 = cp * cp;
		eta2 = Epsp2 * cp2;

		top = .25 - (sp2 * (Eps2 / 4));

		/* inline sq root */
		rn = A / ((.25 - Eps25 * sp2 + .9999944354799 / 4) + (.25 - Eps25 * sp2) / (.25 - Eps25 * sp2 + .9999944354799 / 4));

		b3 = 1.0 + tp2 + tp2 + eta2;
		b4 = 5 + tp2 * (3 - 9 * eta2) + eta2 * (1 - 4 * eta2);
		b5 = 5 + tp2 * (tp2 * 24.0 + 28.0);
		b5 += eta2 * (tp2 * 8.0 + 6.0);
		b6 = 46.0 - 3.0 * eta2 + tp2 * (-252.0 - tp2 * 90.0);
		b6 = eta2 * (b6 + eta2 * tp2 * (tp2 * 225.0 - 66.0));
		b6 += 61.0 + tp2 * (tp2 * 45.0 + 90.0);

		d1 = source_x / rn;
		d2 = d1 * d1;

		double latitude = phi1 - tp * top * (d2 * (Con2 + d2 * ((-Con24) * b4 + d2 * Con720 * b6)));
		double longitude = xlon0 + d1 * (1.0 + d2 * (-Con6 * b3 + d2 * Con120 * b5)) / cp;

		latlon.Latitude = latitude * DEGREES_PER_RADIAN;
		latlon.Longitude = longitude * DEGREES_PER_RADIAN;
		return latlon;
	  }

	  /// <summary>
	  /// _more_
	  /// </summary>
	  /// <param name="from"> _more_ </param>
	  /// <param name="to"> _more_ </param>
	  /// <returns> _more_ </returns>
	  public virtual float[][] projToLatLon(float[][] from, float[][] to)
	  {

		double source_x, source_y, u, su, cu, su2, temp, phi1;
		double sp, sp2, cp, cp2, tp, tp2, eta2, top, rn, b3, b4, b5, b6, d1, d2;

		double xlon0 = (6.0 * ((double) zone) - 183.0) / DEGREES_PER_RADIAN;

		for (int i = 0; i < from[0].Length; i++)
		{
		  source_x = from[0][i] * 1000.0; // wants meters
		  source_x = (source_x - 500000.0) / .9996;

		  if (hemisphere_north)
		  {
			source_y = from[1][i] * 1000.0 / .9996;
		  }
		  else
		  {
			source_y = (from[1][i] * 1000.0 - 1.0E7) / .9996;
		  }

		  u = source_y / conap;
		  su = Math.Sin(u);
		  cu = Math.Cos(u);
		  su2 = su * su;

		  temp = polx2b + su2 * (polx3b + su2 * (polx4b + su2 * polx5b));
		  phi1 = u + su * cu * temp;

		  sp = Math.Sin(phi1);
		  cp = Math.Cos(phi1);
		  tp = sp / cp;
		  tp2 = tp * tp;
		  sp2 = sp * sp;
		  cp2 = cp * cp;
		  eta2 = Epsp2 * cp2;

		  top = .25 - (sp2 * (Eps2 / 4));

		  /* inline sq root */
		  rn = A / ((.25 - Eps25 * sp2 + .9999944354799 / 4) + (.25 - Eps25 * sp2) / (.25 - Eps25 * sp2 + .9999944354799 / 4));

		  b3 = 1.0 + tp2 + tp2 + eta2;
		  b4 = 5 + tp2 * (3 - 9 * eta2) + eta2 * (1 - 4 * eta2);
		  b5 = 5 + tp2 * (tp2 * 24.0 + 28.0);
		  b5 += eta2 * (tp2 * 8.0 + 6.0);
		  b6 = 46.0 - 3.0 * eta2 + tp2 * (-252.0 - tp2 * 90.0);
		  b6 = eta2 * (b6 + eta2 * tp2 * (tp2 * 225.0 - 66.0));
		  b6 += 61.0 + tp2 * (tp2 * 45.0 + 90.0);

		  d1 = source_x / rn;
		  d2 = d1 * d1;

		  double latitude = phi1 - tp * top * (d2 * (Con2 + d2 * ((-Con24) * b4 + d2 * Con720 * b6)));
		  double longitude = xlon0 + d1 * (1.0 + d2 * (-Con6 * b3 + d2 * Con120 * b5)) / cp;

		  to[0][i] = (float)(latitude * DEGREES_PER_RADIAN);
		  to[1][i] = (float)(longitude * DEGREES_PER_RADIAN);
		}

		return to;
	  }

	  /// <summary>
	  /// _more_
	  /// </summary>
	  /// <param name="from"> _more_ </param>
	  /// <param name="to"> _more_ </param>
	  /// <returns> _more_ </returns>
	  public virtual double[][] projToLatLon(double[][] from, double[][] to)
	  {

		double source_x, source_y, u, su, cu, su2, temp, phi1;
		double sp, sp2, cp, cp2, tp, tp2, eta2, top, rn, b3, b4, b5, b6, d1, d2;

		double xlon0 = (6.0 * ((double) zone) - 183.0) / DEGREES_PER_RADIAN;

		for (int i = 0; i < from[0].Length; i++)
		{
		  source_x = from[0][i] * 1000.0; // wants meters
		  source_x = (source_x - 500000.0) / .9996;

		  if (hemisphere_north)
		  {
			source_y = from[1][i] * 1000.0 / .9996;
		  }
		  else
		  {
			source_y = (from[1][i] * 1000.0 - 1.0E7) / .9996;
		  }

		  u = source_y / conap;
		  su = Math.Sin(u);
		  cu = Math.Cos(u);
		  su2 = su * su;

		  temp = polx2b + su2 * (polx3b + su2 * (polx4b + su2 * polx5b));
		  phi1 = u + su * cu * temp;

		  sp = Math.Sin(phi1);
		  cp = Math.Cos(phi1);
		  tp = sp / cp;
		  tp2 = tp * tp;
		  sp2 = sp * sp;
		  cp2 = cp * cp;
		  eta2 = Epsp2 * cp2;

		  top = .25 - (sp2 * (Eps2 / 4));

		  /* inline sq root */
		  rn = A / ((.25 - Eps25 * sp2 + .9999944354799 / 4) + (.25 - Eps25 * sp2) / (.25 - Eps25 * sp2 + .9999944354799 / 4));

		  b3 = 1.0 + tp2 + tp2 + eta2;
		  b4 = 5 + tp2 * (3 - 9 * eta2) + eta2 * (1 - 4 * eta2);
		  b5 = 5 + tp2 * (tp2 * 24.0 + 28.0);
		  b5 += eta2 * (tp2 * 8.0 + 6.0);
		  b6 = 46.0 - 3.0 * eta2 + tp2 * (-252.0 - tp2 * 90.0);
		  b6 = eta2 * (b6 + eta2 * tp2 * (tp2 * 225.0 - 66.0));
		  b6 += 61.0 + tp2 * (tp2 * 45.0 + 90.0);

		  d1 = source_x / rn;
		  d2 = d1 * d1;

		  double latitude = phi1 - tp * top * (d2 * (Con2 + d2 * ((-Con24) * b4 + d2 * Con720 * b6)));
		  double longitude = xlon0 + d1 * (1.0 + d2 * (-Con6 * b3 + d2 * Con120 * b5)) / cp;

		  to[0][i] = (latitude * DEGREES_PER_RADIAN);
		  to[1][i] = (longitude * DEGREES_PER_RADIAN);
		}

		return to;
	  }

	}


}
