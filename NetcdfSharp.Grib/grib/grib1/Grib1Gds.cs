// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Text;

/*
 * Copyright (c) 1998-2026 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.grib1
{
	using GdsHorizCoordSys = NetcdfSharp.ucar.nc2.grib.GdsHorizCoordSys;
	using GribNumbers = NetcdfSharp.ucar.nc2.grib.GribNumbers;
	using QuasiRegular = NetcdfSharp.ucar.nc2.grib.QuasiRegular;
	using Misc = NetcdfSharp.ucar.nc2.util.Misc;
	using NetcdfSharp.ucar.unidata.geoloc;
	using LatLonProjection = NetcdfSharp.ucar.unidata.geoloc.projection.LatLonProjection;
	using Stereographic = NetcdfSharp.ucar.unidata.geoloc.projection.Stereographic;

	/// <summary>
	/// GRIB1 GDS - subclass for each type
	/// <p/>
	/// <ul>
	/// <li>0 Latitude/longitude grid – equidistant cylindrical or Plate Carrée projection
	/// <li>1 Mercator projection
	/// <li>2 Gnomonic projection
	/// <li>3 Lambert conformal, secant or tangent, conic or bi-polar, projection
	/// <li>4 Gaussian latitude/longitude grid
	/// <li>5 Polar stereographic projection
	/// <li>6 Universal Transverse Mercator (UTM) projection
	/// <li>7 Simple polyconic projection
	/// <li>8 Albers equal-area, secant or tangent, conic or bi-polar, projection
	/// <li>9 Miller’s cylindrical projection
	/// <li>10 Rotated latitude/longitude grid
	/// <li>13 Oblique Lambert conformal, secant or tangent, conic or bi-polar, projection
	/// <li>14 Rotated Gaussian latitude/longitude grid
	/// <li>20 Stretched latitude/longitude grid
	/// <li>24 Stretched Gaussian latitude/longitude grid
	/// <li>30 Stretched and rotated latitude/longitude grids
	/// <li>34 Stretched and rotated Gaussian latitude/longitude grids
	/// <li>50 Spherical harmonic coefficients
	/// <li>60 Rotated spherical harmonic coefficients
	/// <li>70 Stretched spherical harmonics
	/// <li>80 Stretched and rotated spherical harmonic coefficients
	/// <li>90 Space view, perspective or orthographic
	/// </ul>
	/// 
	/// @author John
	/// @since 9/3/11
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public abstract class Grib1Gds
	public abstract class Grib1Gds
	{
	  private static readonly NetcdfSharp.org.slf4j.Logger log = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(Grib1Gds));
	  internal const double maxRelativeErrorPos = .01; // relative error in position - GRIB numbers sometime miscoded

	  /*
	   * Code table 6 – Data representation type
	   * Code figure Meaning
	   * 0 Latitude/longitude grid – equidistant cylindrical or Plate Carrée projection
	   * 1 Mercator projection
	   * 2 Gnomonic projection
	   * 3 Lambert conformal, secant or tangent, conic or bi-polar, projection
	   * 4 Gaussian latitude/longitude grid
	   * 5 Polar stereographic projection
	   * 6 Universal Transverse Mercator (UTM) projection
	   * 7 Simple polyconic projection
	   * 8 Albers equal-area, secant or tangent, conic or bi-polar, projection
	   * 9 Miller’s cylindrical projection
	   * 10 Rotated latitude/longitude grid
	   * 11–12 Reserved
	   * 13 Oblique Lambert conformal, secant or tangent, conic or bi-polar, projection
	   * 14 Rotated Gaussian latitude/longitude grid
	   * 15–19 Reserved
	   * 20 Stretched latitude/longitude grid
	   * 21–23 Reserved
	   * 24 Stretched Gaussian latitude/longitude grid
	   * 25–29 Reserved
	   * 30 Stretched and rotated latitude/longitude grids
	   * 31–33 Reserved
	   * 34 Stretched and rotated Gaussian latitude/longitude grids
	   * 35–49 Reserved
	   * ^ 50 Spherical harmonic coefficients
	   * 51–59 Reserved
	   * 60 Rotated spherical harmonic coefficients
	   * 61–69 Reserved
	   * 70 Stretched spherical harmonics
	   * 71–79 Reserved
	   * 80 Stretched and rotated spherical harmonic coefficients
	   * 81–89 Reserved
	   * 90 Space view, perspective or orthographic
	   * 91–191 Reserved
	   * 192–254 Reserved for local use
	   */
	  public static Grib1Gds factory(int template, byte[] data)
	  {
		switch (template)
		{
		  case 0:
			return new LatLon(data, 0);
		  case 1:
			return new Mercator(data, 1);
		  case 3:
			return new LambertConformal(data, 3);
		  case 4:
			return new GaussianLatLon(data, 4);
		  case 5:
			return new PolarStereographic(data, 5);
		  case 10:
			return new RotatedLatLon(data, 10);
		  case 50:
			return new SphericalHarmonicCoefficients(data, 50);
		  default:
			return new UnknownGds(data, template);
		}
	  }

	  ///////////////////////////////////////////////////
	  private static readonly float scale3 = (float) 1.0e-3;
	  // private static final float scale6 = (float) 1.0e-6;

	  protected internal readonly byte[] data;
	  protected internal int[] nptsInLine; // thin grids, else null

	  public readonly int template;
	  protected internal int nx, ny;
	  public int scanMode, resolution;
	  protected internal int lastOctet;

	  protected internal Grib1Gds(int template)
	  {
		this.template = template;
		this.data = null;
	  }

	  public Grib1Gds(byte[] data, int template)
	  {
		this.data = data;
		this.template = template;

		nx = getOctet2(7);
		ny = getOctet2(9);
	  }

	  public virtual byte[] RawBytes
	  {
		  get
		  {
			return data;
		  }
	  }

	  public virtual int Npts
	  {
		  get
		  {

			if (nptsInLine != null)
			{
			  int npts = 0;
			  foreach (int pts in nptsInLine)
			  {
				npts += pts;
			  }
			  return npts;
			}
			else
			{
			  return nx * ny;
			}

		  }
	  }

	  public virtual int[] NptsInLine
	  {
		  get
		  {
			return nptsInLine;
		  }
		  set
		  {
			this.nptsInLine = value;
		  }
	  }


	  protected internal virtual int getOctet(int index)
	  {
		if (index > data.Length)
		{
		  return GribNumbers.UNDEFINED;
		}
		return data[index - 1] & 0xff;
	  }

	  // signed
	  protected internal virtual int getOctet2(int start)
	  {
		return GribNumbers.int2(getOctet(start), getOctet(start + 1));
	  }

	  // signed
	  protected internal virtual int getOctet3(int start)
	  {
		return GribNumbers.int3(getOctet(start), getOctet(start + 1), getOctet(start + 2));
	  }

	  // signed
	  protected internal virtual int getOctet4(int start)
	  {
		return GribNumbers.int4(getOctet(start), getOctet(start + 1), getOctet(start + 2), getOctet(start + 3));
	  }

	  /*
	   * Code table 7 – Resolution and component flags
	   * Bit Value Meaning
	   * 1 0 Direction increments not given
	   * 1 Direction increments given
	   * 2 0 Earth assumed spherical with radius 6367.47 km
	   * 1 Earth assumed oblate spheroidal with size as determined by IAU in 1965 (6378.160 km, 6356.775 km, f = 1/297.0)
	   * 3–4 Reserved
	   * 5 0 Resolved u- and v-components of vector quantities relative to easterly and northerly directions
	   * 1 Resolved u- and v-components of vector quantities relative to the defined grid in the direction of increasing x
	   * and y (or i and j) coordinates respectively
	   * 6–8 0 Reserved – set to zero
	   */

	  private static bool getDirectionIncrementsGiven(int resolution)
	  {
		return GribNumbers.testGribBitIsSet(resolution, 1);
	  }

	  private static bool getEarthShapeIsSpherical(int resolution)
	  {
		return !GribNumbers.testGribBitIsSet(resolution, 2);
	  }

	  private static bool getUVisReletiveToEastNorth(int resolution)
	  {
		return !GribNumbers.testGribBitIsSet(resolution, 5);
	  }

	  protected internal virtual Earth Earth
	  {
		  get
		  {
			if (getEarthShapeIsSpherical(resolution))
			{
			  return new Earth(6367470.0);
			}
			else
			{
			  return EarthEllipsoid.IAU;
			}
		  }
	  }

	  public virtual int EarthShape
	  {
		  get
		  {
			return getEarthShapeIsSpherical(resolution) ? 0 : 1;
		  }
	  }

	  public virtual bool UVisReletiveToEastNorth
	  {
		  get
		  {
			return getUVisReletiveToEastNorth(resolution);
		  }
	  }

	  public virtual int Resolution
	  {
		  get
		  {
			return resolution;
		  }
	  }

	  public virtual bool IsLatLon
	  {
		  get
		  {
			return false;
		  }
	  }

	  /*
	   * Flag/Code table 8 – Scanning mode
	   * Bit Value Meaning
	   * 1 0 Points scan in +i direction
	   * 1 Points scan in –i direction
	   * 2 0 Points scan in –j direction
	   * 1 Points scan in +j direction
	   * 3 0 Adjacent points in i direction are consecutive
	   * 1 Adjacent points in j direction are consecutive
	   */
	  public virtual int ScanMode
	  {
		  get
		  {
			return scanMode;
		  }
	  }

	  public virtual int NxRaw
	  {
		  get
		  {
			return nx;
		  }
	  }

	  public virtual int NyRaw
	  {
		  get
		  {
			return ny;
		  }
	  }

	  ///////// thin grid crapola
	  /*
	   * For data on a quasi-regular grid, in which all the rows or columns do not necessarily have the same number of grid
	   * points, either Ni (octets 7–8) or Nj (octets 9–10) and the corresponding Di (octets 24–25) or Dj (octets 26–27)
	   * shall be
	   * coded with all bits set to 1 (missing); the actual number of points along each parallel or meridian shall be coded.
	   */

	  // number of points along nx, adjusted for thin grid
	  public virtual int Nx
	  {
		  get
		  {
			if (nptsInLine == null || nx > 0)
			{
			  return nx;
			}
			return QuasiRegular.getMax(nptsInLine);
		  }
	  }

	  // number of points along ny, adjusted for thin grid
	  public virtual int Ny
	  {
		  get
		  {
			if (nptsInLine == null || ny > 0)
			{
			  return ny;
			}
			return QuasiRegular.getMax(nptsInLine);
		  }
	  }

	  // delta in x direction
	  public virtual float Dx
	  {
		  get
		  {
			return DxRaw;
		  }
	  }

	  // delta in y direction
	  public virtual float Dy
	  {
		  get
		  {
			return DyRaw;
		  }
	  }

	  public abstract float DxRaw {get;}

	  public abstract float DyRaw {get;}

	  public abstract GdsHorizCoordSys makeHorizCoordSys();

	  public abstract void testHorizCoordSys(Formatter f);

	  public virtual string NameShort
	  {
		  get
		  {
	// JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			string className = this.GetType().FullName;
			int pos = className.LastIndexOf("$", StringComparison.Ordinal);
			return className.Substring(pos + 1);
		  }
	  }

	  public override string ToString()
	  {
		StringBuilder sb = new StringBuilder("Grib1Gds{");
		sb.Append(" template=").Append(template);
		sb.Append(", nx=").Append(nx);
		sb.Append(", ny=").Append(ny);
		sb.Append(", scanMode=").Append(scanMode);
		sb.Append(", resolution=").Append(resolution);
		sb.Append(", lastOctet=").Append(lastOctet);
		if (nptsInLine == null)
		{
		  sb.Append(", nptsInLine=null");
		}
		else
		{
		  sb.Append(", nptsInLine (").Append(nptsInLine.Length);
		  sb.Append(")=").Append("[" + string.Join(", ", nptsInLine) + "]");
		}
		sb.Append('}');
		return sb.ToString();
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

		Grib1Gds Grib1Gds = (Grib1Gds) o;
		if (nx != Grib1Gds.nx)
		{
		  return false;
		}
		if (ny != Grib1Gds.ny)
		{
		  return false;
		}
		return template == Grib1Gds.template;

	  }

	  public override int GetHashCode()
	  {
		int result = template;
		result = 31 * result + nx;
		result = 31 * result + ny;
		return result;
	  }

// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
	  protected internal int hashCode_Conflict;

	  /*
	   * Grid definition – latitude/longitude grid (or equidistant cylindrical, or Plate Carrée)
	   * Octet No. Contents
	   * 7–8 Ni – number of points along a parallel
	   * 9–10 Nj – number of points along a meridian
	   * 11–13 La1 – latitude of first grid point
	   * 14–16 Lo1 – longitude of first grid point
	   * 17 Resolution and component flags (see Code table 7)
	   * 18–20 La2 – latitude of last grid point
	   * 21–23 Lo2 – longitude of last grid point
	   * 24–25 Di – i direction increment
	   * 26–27 Dj – j direction increment
	   * 28 Scanning mode (flags – see Flag/Code table 8)
	   * 29–32 Set to zero (reserved)
	   * 33–35 Latitude of the southern pole in millidegrees (integer) / Latitude of pole of stretching in millidegrees
	   * (integer)
	   * 36–38 Longitude of the southern pole in millidegrees (integer) / Longitude of pole of stretching in millidegrees
	   * (integer)
	   * 39–42 Angle of rotation (represented in the same way as the reference value) / Stretching factor (representation as
	   * for the reference value)
	   * 43–45 Latitude of pole of stretching in millidegrees (integer)
	   * 46–48 Longitude of pole of stretching in millidegrees (integer)
	   * 49–52 Stretching factor (representation as for the reference value)
	   */
	  public class LatLon : Grib1Gds
	  {

		protected internal float la1, lo1, la2, lo2, deltaLon, deltaLat;

		protected internal LatLon(int template) : base(template)
		{
		}

		internal LatLon(byte[] data, int template) : base(data, template)
		{

		  la1 = getOctet3(11) * scale3;
		  lo1 = getOctet3(14) * scale3;
		  resolution = getOctet(17); // Resolution and component flags (see Code table 7)
		  la2 = getOctet3(18) * scale3;
		  lo2 = getOctet3(21) * scale3;

		  if (lo2 < lo1)
		  {
			lo2 += 360.0F;
		  }

		  deltaLon = getOctet2(24);
		  float calcDelta = (lo2 - lo1) / (nx - 1); // more accurate - deltaLon may have roundoff
		  if (deltaLon != GribNumbers.UNDEFINED)
		  {
			deltaLon *= scale3; // undefined for thin grids
		  }
		  else
		  {
			deltaLon = calcDelta;
		  }
		  // If calcDelta is a finite number (isn't the case when we only have one lat/lon
		  // point in the GDS, which can lead to NaN or +/-Inf), can compare deltaLon
		  // against a calculated delta, which appears to be more accurate in some cases.
		  if (float.IsFinite(calcDelta) && !Misc.nearlyEquals(deltaLon, calcDelta))
		  {
			log.debug("deltaLon != calcDeltaLon");
			deltaLon = calcDelta;
		  }

		  deltaLat = getOctet2(26);
		  calcDelta = (la2 - la1) / (ny - 1); // more accurate - deltaLon may have roundoff
		  if (deltaLat != GribNumbers.UNDEFINED)
		  {
			deltaLat *= scale3; // undefined for thin grids
			if (la2 < la1)
			{
			  deltaLat *= (float) - 1.0;
			}
		  }
		  else
		  {
			deltaLat = calcDelta;
		  }

		  /*
		   * thanks to johann.sorel@geomatys.com 11/1/2012. withdrawn for now 11/2/2012
		   * if (deltaLat != GribNumbers.UNDEFINED) {
		   * deltaLat *= scale3; // undefined for thin grids
		   * if (la2 < la1) {
		   * //flip declaration order
		   * float latemp = la1;
		   * la1 = la2;
		   * la2 = latemp;
		   * calcDelta *= -1.0;
		   * 
		   * //we must also consider the cell corner, since we flipped the order
		   * //we should specify that the true value is at the BOTTOM-LEFT corner
		   * //but we can't show this information so we make a one cell displacement
		   * //to move the value on a TOP-LEFT corner.
		   * la1 -= calcDelta;
		   * la2 -= calcDelta;
		   * }
		   * } else {
		   * deltaLat = calcDelta;
		   * }
		   */
		  // If calcDelta is a finite number (isn't the case when we only have one lat/lon
		  // point in the GDS, which can lead to NaN or +/-Inf), can compare deltaLon
		  // against a calculated delta, which appears to be more accurate in some cases.
		  if (float.IsFinite(calcDelta) && !Misc.nearlyEquals(deltaLat, calcDelta))
		  {
			log.debug("deltaLat != calcDeltaLat");
			deltaLat = calcDelta;
		  }

		  scanMode = (byte) getOctet(28);

		  lastOctet = 28;
		}

		public override int[] NptsInLine
		{
			set
			{
			  base.NptsInLine = value;
			  // now that we know this, we may have to recalc some stuff
			  int n = QuasiRegular.getMax(value);
			  if (nx < 0)
			  {
				deltaLon = (lo2 - lo1) / (n - 1);
			  }
			  if (ny < 0)
			  {
				deltaLat = (la2 - la1) / (n - 1);
			  }
			}
		}

		public override bool IsLatLon
		{
			get
			{
			  return true;
			}
		}

		public override float Dx
		{
			get
			{
			  if (nptsInLine == null || deltaLon != GribNumbers.UNDEFINED)
			  {
				return deltaLon;
			  }
			  return (lo2 - lo1) / (Nx - 1);
			}
		}

		public override float Dy
		{
			get
			{
			  if (nptsInLine == null || deltaLat != GribNumbers.UNDEFINED)
			  {
				return deltaLat;
			  }
			  return (la2 - la1) / (Ny - 1);
			}
		}

		public override float DxRaw
		{
			get
			{
			  return deltaLon;
			}
		}

		public override float DyRaw
		{
			get
			{
			  return deltaLat;
			}
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
		  if (!base.Equals(o))
		  {
			return false;
		  }

		  LatLon other = (LatLon) o;
		  if (!Misc.nearlyEqualsAbs(la1, other.la1, maxRelativeErrorPos * deltaLat))
		  {
			return false; // allow some slop, relative to grid size
		  }
		  if (!Misc.nearlyEqualsAbs(lo1, other.lo1, maxRelativeErrorPos * deltaLon))
		  {
			return false;
		  }
		  if (!Misc.nearlyEquals(deltaLat, other.deltaLat))
		  {
			return false;
		  }
		  return Misc.nearlyEquals(deltaLon, other.deltaLon);
		}

		public override int GetHashCode()
		{
		  if (hashCode_Conflict == 0)
		  {
			int useLat = (int) (la1 / (maxRelativeErrorPos * deltaLat)); // Two equal objects must have the same hashCode() value
			int useLon = (int) (lo1 / (maxRelativeErrorPos * deltaLon));
			int useDeltaLon = (int) (deltaLon / Misc.defaultMaxRelativeDiffFloat);
			int useDeltaLat = (int) (deltaLat / Misc.defaultMaxRelativeDiffFloat);

			int result = base.GetHashCode();
			result = 31 * result + useLat;
			result = 31 * result + useLon;
			result = 31 * result + useDeltaLon;
			result = 31 * result + useDeltaLat;
			hashCode_Conflict = result;
		  }
		  return hashCode_Conflict;
		}

		public override string ToString()
		{
		  return "LatLon{" + "la1=" + la1 + ", lo1=" + lo1 + ", la2=" + la2 + ", lo2=" + lo2 + ", deltaLon=" + deltaLon + ", deltaLat=" + deltaLat + "} " + base.ToString();
		}

		public override GdsHorizCoordSys makeHorizCoordSys()
		{
		  LatLonProjection proj = new LatLonProjection(Earth);
		  double centerLon = ((int) ((lo2 - lo1 + deltaLon) / 2 / scale3)) * scale3;
		  proj.setCenterLon(centerLon, false);
		  double startx = lo1; // startP.getX();
		  double starty = la1; // startP.getY();
		  return new GdsHorizCoordSys(NameShort, template, 0, scanMode, proj, startx, Dx, starty, Dy, Nx, Ny, null);
		}

		public virtual float getLa1()
		{
		  return la1;
		}

		public virtual float getLo1()
		{
		  return lo1;
		}

		public override void testHorizCoordSys(Formatter f)
		{
		  GdsHorizCoordSys cs = makeHorizCoordSys();
		  double Lo2 = lo2;
		  if (Lo2 < lo1)
		  {
			Lo2 += 360;
		  }
		  LatLonPoint startLL = LatLonPoint.create(la1, lo1);
		  LatLonPoint endLL = LatLonPoint.create(la2, Lo2);

		  f.format("%s testProjection%n", this.GetType().FullName);
		  f.format("  start at latlon= %s%n", startLL);
		  f.format("    end at latlon= %s%n", endLL);

		  ProjectionPoint endPP = cs.proj.latLonToProj(endLL);
		  f.format("   start at proj coord= %s%n", ProjectionPoint.create(cs.startx, cs.starty));
		  f.format("     end at proj coord= %s%n", endPP);

		  double endx = cs.startx + (Nx - 1) * cs.dx;
		  double endy = cs.starty + (Ny - 1) * cs.dy;
		  f.format("   should end at x= (%f,%f)%n", endx, endy);
		}
	  }

	  public class GaussianLatLon : LatLon
	  {

		internal readonly int nparellels;
		public float latSouthPole, lonSouthPole, rotAngle, latPole, lonPole, stretchFactor;

		internal GaussianLatLon(byte[] data, int template) : base(data, template)
		{
		  nparellels = getOctet2(26);

		  if (data.Length > 32)
		  {
			latSouthPole = getOctet3(33) * scale3;
			lonSouthPole = getOctet3(36) * scale3;
			rotAngle = getOctet4(39) * scale3;
			latPole = getOctet3(43) * scale3;
			lonPole = getOctet3(46) * scale3;
			stretchFactor = getOctet4(49) * scale3;
		  }

		  lastOctet = 52;
		}

		public override GdsHorizCoordSys makeHorizCoordSys()
		{
		  GdsHorizCoordSys hc = base.makeHorizCoordSys();
		  hc.setGaussianLats(nparellels, la1, la2);
		  return hc;
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
		  if (!base.Equals(o))
		  {
			return false;
		  }

		  GaussianLatLon that = (GaussianLatLon) o;
		  return nparellels == that.nparellels;
		}

		public override int GetHashCode()
		{
		  int result = base.GetHashCode();
		  result = 31 * result + nparellels;
		  return result;
		}

		public override string ToString()
		{
		  return "GaussianLatLon{" + "nparellels=" + nparellels + ", latSouthPole=" + latSouthPole + ", lonSouthPole=" + lonSouthPole + ", rotAngle=" + rotAngle + ", latPole=" + latPole + ", lonPole=" + lonPole + ", stretchFactor=" + stretchFactor + "} " + base.ToString();
		}
	  }

	  public class PolarStereographic : Grib1Gds
	  {

		protected internal float la1, lo1, lov, dX, dY;
		protected internal int projCenterFlag;
		private readonly float lad = (float) 60.0; // LOOK

		protected internal PolarStereographic(int template) : base(template)
		{
		}

		internal PolarStereographic(byte[] data, int template) : base(data, template)
		{

		  la1 = getOctet3(11) * scale3;
		  lo1 = getOctet3(14) * scale3;
		  resolution = getOctet(17); // Resolution and component flags (see Code table 7)
		  lov = getOctet3(18) * scale3;
		  dX = getOctet3(21) * scale3;
		  dY = getOctet3(24) * scale3;

		  projCenterFlag = getOctet(27);
		  scanMode = getOctet(28);

		  lastOctet = 28;
		}

		public override string ToString()
		{
		  return "PolarStereographic{" + "la1=" + la1 + ", lo1=" + lo1 + ", lov=" + lov + ", dX=" + dX + ", dY=" + dY + ", projCenterFlag=" + projCenterFlag + ", lad=" + lad + "} " + base.ToString();
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
		  if (!base.Equals(o))
		  {
			return false;
		  }

		  PolarStereographic that = (PolarStereographic) o;

		  if (!Misc.nearlyEqualsAbs(la1, that.la1, maxRelativeErrorPos * dY))
		  {
			return false; // allow some slop, relative to grid size
		  }
		  if (!Misc.nearlyEqualsAbs(lo1, that.lo1, maxRelativeErrorPos * dX))
		  {
			return false;
		  }
		  if (!Misc.nearlyEquals(lov, that.lov))
		  {
			return false;
		  }
		  if (!Misc.nearlyEquals(dY, that.dY))
		  {
			return false;
		  }
		  if (!Misc.nearlyEquals(dX, that.dX))
		  {
			return false;
		  }

		  return projCenterFlag == that.projCenterFlag;

		}

		public override int GetHashCode()
		{
		  if (hashCode_Conflict == 0)
		  {
			int useLat = (int) (la1 / (maxRelativeErrorPos * dY)); // Two equal objects must have the same hashCode() value
			int useLon = (int) (lo1 / (maxRelativeErrorPos * dX));
			int useLov = (int) (lov / Misc.defaultMaxRelativeDiffFloat);
			int useDeltaLon = (int) (dX / Misc.defaultMaxRelativeDiffFloat);
			int useDeltaLat = (int) (dY / Misc.defaultMaxRelativeDiffFloat);

			int result = base.GetHashCode();
			result = 31 * result + useLat;
			result = 31 * result + useLon;
			result = 31 * result + useLov;
			result = 31 * result + useDeltaLon;
			result = 31 * result + useDeltaLat;
			result = 31 * result + projCenterFlag;
			hashCode_Conflict = result;
		  }
		  return hashCode_Conflict;
		}

		public override float DxRaw
		{
			get
			{
			  return dX;
			}
		}

		public override float DyRaw
		{
			get
			{
			  return dY;
			}
		}

		public virtual float getLa1()
		{
		  return la1;
		}

		public virtual float getLo1()
		{
		  return lo1;
		}

		public virtual float getLov()
		{
		  return lov;
		}

		public virtual int getProjCenterFlag()
		{
		  return projCenterFlag;
		}

		public override GdsHorizCoordSys makeHorizCoordSys()
		{
		  bool northPole = (projCenterFlag & 128) == 0;
		  double latOrigin = northPole ? 90.0 : -90.0;

		  // Why the scale factor?. according to GRIB docs:
		  // "Grid lengths are in units of meters, at the 60 degree latitude circle nearest to the pole"
		  // since the scale factor at 60 degrees = k = 2*k0/(1+sin(60)) [Snyder,Working Manual p157]
		  // then to make scale = 1 at 60 degrees, k0 = (1+sin(60))/2 = .933
		  double scale;
		  if (float.IsNaN(lad))
		  { // LOOK ??
			scale = 0.9330127018922193;
		  }
		  else
		  {
			scale = (1.0 + Math.Sin(((Math.Abs(lad)) * (Math.PI / 180.0)))) / 2;
		  }

		  ProjectionImpl proj;

		  Earth earth = Earth;
		  if (earth.Spherical)
		  {
			proj = new Stereographic(latOrigin, lov, scale);
		  }
		  else
		  {
			throw new System.NotSupportedException("Ellipsoidal polar stereographic (proj4 StereographicAzimuthalProjection) is not ported");
		  }

		  ProjectionPointImpl start = (ProjectionPointImpl) proj.latLonToProj(LatLonPoint.create(la1, lo1));
		  return new GdsHorizCoordSys(NameShort, template, 0, scanMode, proj, start.X, Dx, start.Y, Dy, Nx, Ny, null);
		}

		public override void testHorizCoordSys(Formatter f)
		{
		  GdsHorizCoordSys cs = makeHorizCoordSys();
		  f.format("%s testProjection %s%n", this.GetType().FullName, cs.proj.GetType().FullName);

		  double endx = cs.startx + (Nx - 1) * cs.dx;
		  double endy = cs.starty + (Ny - 1) * cs.dy;
		  ProjectionPoint endPP = ProjectionPoint.create(endx, endy);
		  f.format("   start at proj coord= %s%n", ProjectionPoint.create(cs.startx, cs.starty));
		  f.format("     end at proj coord= %s%n", endPP);

		  LatLonPoint startLL = LatLonPoint.create(la1, lo1);
		  LatLonPoint endLL = cs.proj.projToLatLon(endPP);

		  f.format("  start at latlon= %s%n", startLL);
		  f.format("    end at latlon= %s%n", endLL);
		}

	  }

	  public class LambertConformal : Grib1Gds
	  {

		protected internal readonly float la1;
		protected internal readonly float lo1;
		protected internal readonly float lov;
		protected internal float lad;
		protected internal readonly float dX;
		protected internal readonly float dY;
		protected internal readonly float latin1;
		protected internal readonly float latin2;
		protected internal readonly float latSouthPole;
		protected internal readonly float lonSouthPole;
		protected internal readonly int projCenterFlag;

		internal LambertConformal(byte[] data, int template) : base(data, template)
		{

		  la1 = getOctet3(11) * scale3;
		  lo1 = getOctet3(14) * scale3;
		  resolution = getOctet(17); // Resolution and component flags (see Code table 7)
		  lov = getOctet3(18) * scale3;

		  dX = getOctet3(21) * scale3;
		  dY = getOctet3(24) * scale3;

		  projCenterFlag = getOctet(27);
		  scanMode = getOctet(28);

		  latin1 = getOctet3(29) * scale3;
		  latin2 = getOctet3(32) * scale3;
		  latSouthPole = getOctet3(35) * scale3;
		  lonSouthPole = getOctet3(38) * scale3;

		  lastOctet = 42;
		}

		public override float DxRaw
		{
			get
			{
			  return dX;
			}
		}

		public override float DyRaw
		{
			get
			{
			  return dY;
			}
		}

		public virtual float getLa1()
		{
		  return la1;
		}

		public virtual float getLo1()
		{
		  return lo1;
		}

		public virtual float getLov()
		{
		  return lov;
		}

		public virtual float getLatin1()
		{
		  return latin1;
		}

		public virtual float getLatin2()
		{
		  return latin2;
		}

		public override string ToString()
		{
		  return "LambertConformal{" + "la1=" + la1 + ", lo1=" + lo1 + ", lov=" + lov + ", lad=" + lad + ", dX=" + dX + ", dY=" + dY + ", latin1=" + latin1 + ", latin2=" + latin2 + ", latSouthPole=" + latSouthPole + ", lonSouthPole=" + lonSouthPole + ", projCenterFlag=" + projCenterFlag + "} " + base.ToString();
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
		  if (!base.Equals(o))
		  {
			return false;
		  }

		  LambertConformal that = (LambertConformal) o;

		  if (!Misc.nearlyEqualsAbs(la1, that.la1, maxRelativeErrorPos * dY))
		  {
			return false; // allow some slop, relative to grid size
		  }
		  if (!Misc.nearlyEqualsAbs(lo1, that.lo1, maxRelativeErrorPos * dX))
		  {
			return false;
		  }
		  if (!Misc.nearlyEquals(lad, that.lad))
		  {
			return false;
		  }
		  if (!Misc.nearlyEquals(lov, that.lov))
		  {
			return false;
		  }
		  if (!Misc.nearlyEquals(dY, that.dY))
		  {
			return false;
		  }
		  if (!Misc.nearlyEquals(dX, that.dX))
		  {
			return false;
		  }
		  if (!Misc.nearlyEquals(latin1, that.latin1))
		  {
			return false;
		  }
		  return Misc.nearlyEquals(latin2, that.latin2);

		}

		public override int GetHashCode()
		{
		  if (hashCode_Conflict == 0)
		  {
			int useLat = (int) (la1 / (maxRelativeErrorPos * dY)); // Two equal objects must have the same hashCode() value
			int useLon = (int) (lo1 / (maxRelativeErrorPos * dX));
			int useLad = (int) (lad / Misc.defaultMaxRelativeDiffFloat);
			int useLov = (int) (lov / Misc.defaultMaxRelativeDiffFloat);
			int useDeltaLon = (int) (dX / Misc.defaultMaxRelativeDiffFloat);
			int useDeltaLat = (int) (dY / Misc.defaultMaxRelativeDiffFloat);
			int useLatin1 = (int) (latin1 / Misc.defaultMaxRelativeDiffFloat);
			int useLatin2 = (int) (latin2 / Misc.defaultMaxRelativeDiffFloat);

			int result = base.GetHashCode();
			result = 31 * result + useLat;
			result = 31 * result + useLon;
			result = 31 * result + useLad;
			result = 31 * result + useLov;
			result = 31 * result + useDeltaLon;
			result = 31 * result + useDeltaLat;
			result = 31 * result + useLatin1;
			result = 31 * result + useLatin2;
			result = 31 * result + projCenterFlag;

			hashCode_Conflict = result;
		  }
		  return hashCode_Conflict;
		}

		public override GdsHorizCoordSys makeHorizCoordSys()
		{
		  ProjectionImpl proj;

		  Earth earth = Earth;
		  if (earth.Spherical)
		  {
			proj = new NetcdfSharp.ucar.unidata.geoloc.projection.LambertConformal(latin1, lov, latin1, latin2, 0.0, 0.0, earth.EquatorRadius * .001);
		  }
		  else
		  {
			throw new System.NotSupportedException("Ellipsoidal Lambert conformal (proj4 LambertConformalConicEllipse) is not ported");
		  }

		  LatLonPoint startLL = LatLonPoint.create(la1, lo1);
		  ProjectionPointImpl start = (ProjectionPointImpl) proj.latLonToProj(startLL);
		  return new GdsHorizCoordSys(NameShort, template, 0, scanMode, proj, start.X, Dx, start.Y, Dy, Nx, Ny, null);
		}

		public override void testHorizCoordSys(Formatter f)
		{
		  GdsHorizCoordSys cs = makeHorizCoordSys();
		  f.format("%s testProjection %s%n", this.GetType().FullName, cs.proj.GetType().FullName);

		  double endx = cs.startx + (Nx - 1) * cs.dx;
		  double endy = cs.starty + (Ny - 1) * cs.dy;
		  ProjectionPoint endPP = ProjectionPoint.create(endx, endy);
		  f.format("   start at proj coord= %s%n", ProjectionPoint.create(cs.startx, cs.starty));
		  f.format("     end at proj coord= %s%n", endPP);

		  LatLonPoint startLL = LatLonPoint.create(la1, lo1);
		  LatLonPoint endLL = cs.proj.projToLatLon(endPP);

		  f.format("  start at latlon= %s%n", startLL);
		  f.format("    end at latlon= %s%n", endLL);
		}

	  }

	  public class Mercator : Grib1Gds
	  {

		protected internal readonly float la1;
		protected internal readonly float lo1;
		protected internal readonly float la2;
		protected internal readonly float lo2;
		protected internal readonly float latin;
		protected internal readonly float dX;
		protected internal readonly float dY;

		internal Mercator(byte[] data, int template) : base(data, template)
		{

		  la1 = getOctet3(11) * scale3;
		  lo1 = getOctet3(14) * scale3;
		  resolution = getOctet(17); // Resolution and component flags (see Code table 7)
		  la2 = getOctet3(18) * scale3;
		  lo2 = getOctet3(21) * scale3;
		  latin = getOctet3(24) * scale3;

		  scanMode = getOctet(28);

		  dX = getOctet3(29) * scale3; // km
		  dY = getOctet3(32) * scale3; // km

		  lastOctet = 42;
		}

		public override float DxRaw
		{
			get
			{
			  return dX;
			}
		}

		public override float DyRaw
		{
			get
			{
			  return dY;
			}
		}

		public virtual float getLa1()
		{
		  return la1;
		}

		public virtual float getLo1()
		{
		  return lo1;
		}

		public virtual float getLa2()
		{
		  return la2;
		}

		public virtual float getLo2()
		{
		  return lo2;
		}

		public virtual float getLatin()
		{
		  return latin;
		}

		public override string ToString()
		{
		  return "Mercator{" + "la1=" + la1 + ", lo1=" + lo1 + ", la2=" + la2 + ", lo2=" + lo2 + ", latin=" + latin + ", dX=" + dX + ", dY=" + dY + "} " + base.ToString();
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
		  if (!base.Equals(o))
		  {
			return false;
		  }

		  Mercator that = (Mercator) o;

		  if (!Misc.nearlyEqualsAbs(la1, that.la1, maxRelativeErrorPos * dY))
		  {
			return false; // allow some slop, relative to grid size
		  }
		  if (!Misc.nearlyEqualsAbs(lo1, that.lo1, maxRelativeErrorPos * dX))
		  {
			return false;
		  }
		  if (!Misc.nearlyEquals(latin, that.latin))
		  {
			return false;
		  }
		  if (!Misc.nearlyEquals(dY, that.dY))
		  {
			return false;
		  }
		  return Misc.nearlyEquals(dX, that.dX);

		}

		public override int GetHashCode()
		{
		  if (hashCode_Conflict == 0)
		  {
			int useLat = (int) (la1 / (maxRelativeErrorPos * dY)); // Two equal objects must have the same hashCode() value
			int useLon = (int) (lo1 / (maxRelativeErrorPos * dX));
			int useLad = (int) (latin / Misc.defaultMaxRelativeDiffFloat);
			int useDeltaLon = (int) (dX / Misc.defaultMaxRelativeDiffFloat);
			int useDeltaLat = (int) (dY / Misc.defaultMaxRelativeDiffFloat);

			int result = base.GetHashCode();
			result = 31 * result + useLat;
			result = 31 * result + useLon;
			result = 31 * result + useLad;
			result = 31 * result + useDeltaLon;
			result = 31 * result + useDeltaLat;
			hashCode_Conflict = result;
		  }
		  return hashCode_Conflict;
		}

		public override GdsHorizCoordSys makeHorizCoordSys()
		{
		  // put longitude origin at first point - doesnt actually matter
		  // param par standard parallel (degrees). cylinder cuts earth at this latitude.
		  // LOOK dont have an elipsoidal Mercator projection
		  Earth earth = Earth;
		  NetcdfSharp.ucar.unidata.geoloc.projection.Mercator proj = new NetcdfSharp.ucar.unidata.geoloc.projection.Mercator(lo1, latin, 0, 0, earth.EquatorRadius * .001);

		  // find out where things start
		  ProjectionPoint startP = proj.latLonToProj(LatLonPoint.create(la1, lo1));
		  double startx = startP.X;
		  double starty = startP.Y;

		  return new GdsHorizCoordSys(NameShort, template, 0, scanMode, proj, startx, Dx, starty, Dy, Nx, Ny, null);
		}

		public override void testHorizCoordSys(Formatter f)
		{
		  GdsHorizCoordSys cs = makeHorizCoordSys();
		  double Lo2 = lo2;
		  if (Lo2 < lo1)
		  {
			Lo2 += 360;
		  }
		  LatLonPoint startLL = LatLonPoint.create(la1, lo1);
		  LatLonPoint endLL = LatLonPoint.create(la2, Lo2);

		  f.format("%s testProjection%n", this.GetType().FullName);
		  f.format("  start at latlon= %s%n", startLL);
		  f.format("    end at latlon= %s%n", endLL);

		  ProjectionPoint endPP = cs.proj.latLonToProj(endLL);
		  f.format("   start at proj coord= %s%n", ProjectionPoint.create(cs.startx, cs.starty));
		  f.format("     end at proj coord= %s%n", endPP);

		  double endx = cs.startx + (Nx - 1) * cs.dx;
		  double endy = cs.starty + (Ny - 1) * cs.dy;
		  f.format("   should end at x= (%f,%f)%n", endx, endy);
		}

	  }

	  public class RotatedLatLon : LatLon
	  {

		protected internal readonly float angleRotation; // Angle of rotation (represented in the same way as the reference value)
		protected internal readonly float latSouthPole; // Latitude of pole of stretching in millidegrees (integer)
		protected internal readonly float lonSouthPole; // Longitude of pole of stretching in millidegrees (integer)

		internal RotatedLatLon(byte[] data, int template) : base(data, template)
		{

		  latSouthPole = getOctet3(33) * scale3;
		  lonSouthPole = getOctet3(36) * scale3;
		  angleRotation = getOctet4(39) * scale3;

		  lastOctet = 43;
		}

		public virtual string getName()
		{
		  return "Rotated latitude/longitude";
		}

		public override string ToString()
		{
		  return "RotatedLatLon{" + "angleRotation=" + angleRotation + ", latSouthPole=" + latSouthPole + ", lonSouthPole=" + lonSouthPole + "} " + base.ToString();
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
		  if (!base.Equals(o))
		  {
			return false;
		  }

		  RotatedLatLon other = (RotatedLatLon) o;
		  return Misc.nearlyEquals(angleRotation, other.angleRotation);
		}

		public override int GetHashCode()
		{
		  if (hashCode_Conflict == 0)
		  {
			int result = base.GetHashCode();
			result = 31 * result + (angleRotation != 0.0f ? BitConverter.SingleToInt32Bits(angleRotation) : 0);
			hashCode_Conflict = result;
		  }
		  return hashCode_Conflict;
		}

		public override GdsHorizCoordSys makeHorizCoordSys()
		{
		  NetcdfSharp.ucar.unidata.geoloc.projection.RotatedLatLon proj = new NetcdfSharp.ucar.unidata.geoloc.projection.RotatedLatLon(latSouthPole, lonSouthPole, angleRotation);
		  // LOOK dont transform - works for grib1 Q:/cdmUnitTest/transforms/HIRLAMhybrid.grib
		  // LatLonPoint startLL = proj.projToLatLon(ProjectionPoint.create(lo1, la1));
		  // double startx = startLL.getLongitude();
		  // double starty = startLL.getLatitude();
		  return new GdsHorizCoordSys(NameShort, template, 0, scanMode, proj, lo1, deltaLon, la1, deltaLat, nx, ny, null);
		}

		public override void testHorizCoordSys(Formatter f)
		{
		  GdsHorizCoordSys cs = makeHorizCoordSys();
		  LatLonPoint startLL = cs.proj.projToLatLon(ProjectionPoint.create(lo1, la1));
		  LatLonPoint endLL = cs.proj.projToLatLon(ProjectionPoint.create(lo2, la2));

		  f.format("%s testProjection%n", this.GetType().FullName);
		  f.format("  start at latlon= %s%n", startLL);
		  f.format("    end at latlon= %s%n", endLL);

		  ProjectionPoint endPP = cs.proj.latLonToProj(endLL);
		  f.format("   start at proj coord= %s%n", ProjectionPoint.create(cs.startx, cs.starty));
		  f.format("     end at proj coord= %s%n", endPP);

		  double endx = cs.startx + (nx - 1) * cs.dx;
		  double endy = cs.starty + (ny - 1) * cs.dy;
		  f.format("   should end at x= (%f,%f)%n", endx, endy);
		}

	  }

	  public class SphericalHarmonicCoefficients : Grib1Gds
	  {

		internal readonly int j;
		internal readonly int k;
		internal readonly int m;
		internal readonly int type;
		internal readonly int mode;

		internal SphericalHarmonicCoefficients(byte[] data, int template) : base(data, template)
		{

		  j = getOctet2(7);
		  k = getOctet2(9);
		  m = getOctet2(11);
		  type = getOctet(13); // code table 9
		  mode = getOctet(14); // code table 10
		}

		public override float DxRaw
		{
			get
			{
			  return 0;
			}
		}

		public override float DyRaw
		{
			get
			{
			  return 0;
			}
		}

		public override GdsHorizCoordSys makeHorizCoordSys()
		{
		  return null; // LOOK not done yet
		}

		public override void testHorizCoordSys(Formatter f)
		{
		}

		public override string ToString()
		{
		  return "SphericalHarmonicCoefficients{" + "j=" + j + ", k=" + k + ", m=" + m + ", type=" + type + ", mode=" + mode + "} " + base.ToString();
		}
	  }

	  public class UnknownGds : Grib1Gds
	  {

		internal UnknownGds(byte[] data, int template) : base(data, template)
		{
		}

		public override float DxRaw
		{
			get
			{
			  return 0;
			}
		}

		public override float DyRaw
		{
			get
			{
			  return 0;
			}
		}

		public override GdsHorizCoordSys makeHorizCoordSys()
		{
		  return null; // LOOK
		}

		public override void testHorizCoordSys(Formatter f)
		{

		}

		public override string ToString()
		{
		  return "UnknownGds{} " + base.ToString();
		}
	  }


	}

}
