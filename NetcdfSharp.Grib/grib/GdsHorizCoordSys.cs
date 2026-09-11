// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Text;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib
{
	using NcArray = NetcdfSharp.ucar.ma2.NcArray;
	using DataType = NetcdfSharp.ucar.ma2.DataType;
	using NetcdfSharp.ucar.unidata.geoloc;
	using LatLonProjection = NetcdfSharp.ucar.unidata.geoloc.projection.LatLonProjection;
	using GaussianLatitudes = NetcdfSharp.ucar.unidata.util.GaussianLatitudes;
	using StringUtil2 = NetcdfSharp.ucar.unidata.util.StringUtil2;

	/// <summary>
	/// A Horizontal coordinate system generated from a GRIB-1 or GRIB-2 GDS.
	/// 
	/// @author John
	/// @since 9/5/11
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class GdsHorizCoordSys
	public class GdsHorizCoordSys
	{
	  private static readonly NetcdfSharp.org.slf4j.Logger log = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(GdsHorizCoordSys));

	  private readonly string name;
	  public readonly int template, gdsNumberPoints, scanMode;
	  public readonly NetcdfSharp.ucar.unidata.geoloc.ProjectionImpl proj;
	  public readonly double startx, dx; // km
	  public readonly double starty, dy; // km
	  public readonly int nx, ny; // regridded
	  public readonly int nxRaw, nyRaw; // raw
	  public readonly int[] nptsInLine; // non-null id thin grid

	  // hmmmm
	  private NcArray gaussLats;
	  private NcArray gaussw;

	  public GdsHorizCoordSys(string name, int template, int gdsNumberPoints, int scanMode, ProjectionImpl proj, double startx, double dx, double starty, double dy, int nxRaw, int nyRaw, int[] nptsInLine)
	  {

		this.name = name;
		this.template = template;
		this.gdsNumberPoints = gdsNumberPoints; // only used by GRIB2
		this.scanMode = scanMode;
		this.proj = proj;
		this.startx = startx;
		this.dx = dx;
		this.starty = starty;
		this.dy = dy;
		this.nxRaw = nxRaw;
		this.nyRaw = nyRaw;
		this.nptsInLine = nptsInLine;

		// thin grids
		if (nptsInLine != null)
		{
		  if (nxRaw > 0)
		  {
			nx = nxRaw;
			ny = QuasiRegular.getMax(nptsInLine);
		  }
		  else if (nyRaw > 0)
		  {
			ny = nyRaw;
			nx = QuasiRegular.getMax(nptsInLine);
		  }
		  else
		  {
			throw new ArgumentException("Quasi Grids nx,ny=" + nxRaw + "," + nyRaw);
		  }
		}
		else
		{
		  nx = nxRaw;
		  ny = nyRaw;
		}
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public virtual double StartX
	  {
		  get
		  {
			return startx;
		  }
	  }

	  public virtual double StartY
	  {
		  get
		  {
			if (gaussLats != null)
			{
			  return gaussLats.getDouble(0);
			}
			return starty;
		  }
	  }

	  public virtual double EndX
	  {
		  get
		  {
			return startx + dx * (nx - 1);
		  }
	  }

	  public virtual double EndY
	  {
		  get
		  {
			if (gaussLats != null)
			{
			  return gaussLats.getDouble((int) gaussLats.Size - 1);
			}
			return starty + dy * (ny - 1);
		  }
	  }

	  public virtual int ScanMode
	  {
		  get
		  {
			return scanMode;
		  }
	  }

	  public virtual bool LatLon
	  {
		  get
		  {
			return proj is LatLonProjection;
		  }
	  }

	  public virtual LatLonPoint CenterLatLon
	  {
		  get
		  {
			return proj.projToLatLon(startx + dx * nx / 2, starty + dy * ny / 2);
		  }
	  }

	  public virtual string makeDescription()
	  {
		return name + "_" + ny + "X" + nx + " (Center " + CenterLatLon + ")";
	  }

	  public virtual string makeId()
	  {
		LatLonPoint center = CenterLatLon;
		StringBuilder result = new StringBuilder(name + "_" + ny + "X" + nx + "-" + LatLonPoints.toString(center, 2));
		StringUtil2.replace(result, ". ", "p-");
		return result.ToString();
	  }

	  public virtual ProjectionRect ProjectionBB
	  {
		  get
		  {
			return new ProjectionRect(ProjectionPoint.create(StartX, StartY), EndX - StartX, EndY - StartY);
		  }
	  }

	  public virtual LatLonRect LatLonBB
	  {
		  get
		  {
			if (LatLon)
			{
			  return new LatLonRect(LatLonPoint.create(StartY, StartX), dy * (ny - 1), dx * (nx - 1));
			}
			else
			{
			  return proj.projToLatLonBB(ProjectionBB);
			}
		  }
	  }

	  ////////////////////////////////////////////////

	  // set gaussian weights based on nparallels
	  // some weird adjustment for la1 and la2.
	  public virtual void setGaussianLats(int nparallels, float la1, float la2)
	  {
		log.debug("la1 {}, la2 {}", la1, la2);
		if (this.gaussLats != null)
		{
		  throw new Exception("Cant modify GdsHorizCoordSys");
		}

		int nlats = (2 * nparallels);
		GaussianLatitudes gaussLats = GaussianLatitudes.factory(nlats);

		int bestStartIndex = 0, bestEndIndex = 0;
		double bestStartDiff = double.MaxValue;
		double bestEndDiff = double.MaxValue;
		for (int i = 0; i < nlats; i++)
		{
		  double diff = Math.Abs(gaussLats.latd[i] - la1);
		  if (diff < bestStartDiff)
		  {
			bestStartDiff = diff;
			bestStartIndex = i;
		  }
		  diff = Math.Abs(gaussLats.latd[i] - la2);
		  if (diff < bestEndDiff)
		  {
			bestEndDiff = diff;
			bestEndIndex = i;
		  }
		}

		log.debug("first pass: bestStartIndex {}, bestEndIndex {}", bestStartIndex, bestEndIndex);

		if (Math.Abs(bestEndIndex - bestStartIndex) + 1 != nyRaw)
		{
		  log.warn("GRIB gaussian lats: NP != NY, use NY"); // see email from Toussaint@dkrz.de datafil:
		  nlats = nyRaw;
		  gaussLats = GaussianLatitudes.factory(nlats);
		  bestStartIndex = 0;
		  bestEndIndex = nyRaw - 1;
		}
		bool goesUp = bestEndIndex > bestStartIndex;

		log.debug("bestStartIndex {}, bestEndIndex {}, goesUp {}", bestStartIndex, bestEndIndex, goesUp);

		// create the data
		int useIndex = bestStartIndex;
		float[] data = new float[nyRaw];
		float[] gaussw = new float[nyRaw];
		for (int i = 0; i < nyRaw; i++)
		{
		  data[i] = (float) gaussLats.latd[useIndex];
		  gaussw[i] = (float) gaussLats.gaussw[useIndex];

		  log.trace("i {}, useIndex {}, data {}, gaussw {}", i, useIndex, data[i], gaussw[i]);
		  if (goesUp)
		  {
			useIndex++;
		  }
		  else
		  {
			useIndex--;
		  }
		}

		this.gaussLats = NcArray.factory(DataType.FLOAT, new int[] {nyRaw}, data);
		this.gaussw = NcArray.factory(DataType.FLOAT, new int[] {nyRaw}, gaussw);
	  }

	  public virtual NcArray GaussianLats
	  {
		  get
		  {
			return gaussLats;
		  }
	  }

	  public virtual NcArray GaussianWeights
	  {
		  get
		  {
			return gaussw;
		  }
	  }

	  public override string ToString()
	  {
		return MoreObjects.toStringHelper(this).add("name", name).add("template", template).add("gdsNumberPoints", gdsNumberPoints).add("scanMode", scanMode).add("proj", proj).add("startx", startx).add("dx", dx).add("starty", starty).add("dy", dy).add("nx", nx).add("ny", ny).add("nxRaw", nxRaw).add("nyRaw", nyRaw).add("nptsInLine", nptsInLine).add("gaussLats", gaussLats).add("gaussw", gaussw).ToString();
	  }
	}

}
