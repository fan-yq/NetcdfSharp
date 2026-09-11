// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.grib1
{

	/// <summary>
	/// Helper class for pre-defined grid definition section (GDS) .
	/// These are NCEP.
	/// </summary>
	/// <seealso cref=""http://www.nco.ncep.noaa.gov/pmb/docs/on388/tableb.html""/>

	public class Grib1GdsPredefined
	{

	  /// <summary>
	  /// Constructs a Grib1Gds object from a pds and predefined tables.
	  /// </summary>
	  /// <param name="center"> center id </param>
	  /// <param name="gridNumber"> from pds (octet 7) </param>
	  /// <returns> predefined GDS </returns>
	  public static Grib1Gds factory(int center, int gridNumber)
	  {
		if (center == 7)
		{
		  return factoryNCEP(gridNumber);
		}
		else
		{
		  throw new ArgumentException("Dont have predefined GDS " + gridNumber + " from " + center);
		}
	  }

	  // 21-26, 61-64: International Exchange and Family of Services (FOS) grids. So may be more general than NCEP

	  private static Grib1Gds factoryNCEP(int gridNumber)
	  {
		switch (gridNumber)
		{
		  case 21:
			return new NcepLatLon(gridNumber, 37, 36, 0.0F, 0.0F, 90.0F, 180.0F, 5.0F, 2.5F, unchecked((byte) 0x88), (byte) 64);
		  case 22:
			return new NcepLatLon(gridNumber, 37, 36, 0.0F, -180.0F, 90.0F, 0.0F, 5.0F, 2.5F, unchecked((byte) 0x88), (byte) 64);
		  case 23:
			return new NcepLatLon(gridNumber, 37, 36, -90.0F, 0.0F, 180.0F, 0.0F, 5.0F, 2.5F, unchecked((byte) 0x88), (byte) 64);
		  case 24:
			return new NcepLatLon(gridNumber, 37, 36, -90.0F, -180.0F, 0.0F, 0.0F, 5.0F, 2.5F, unchecked((byte) 0x88), (byte) 64);
		  case 25:
			return new NcepLatLon(gridNumber, 72, 18, 0.0F, 0.0F, 90.0F, 355.0F, 5.0F, 5.0F, unchecked((byte) 0x88), (byte) 64);
		  case 26:
			return new NcepLatLon(gridNumber, 72, 18, -90.0F, 0.0F, 0.0F, 355.0F, 5.0F, 5.0F, unchecked((byte) 0x88), (byte) 64);
		  case 61:
			return new NcepLatLon(gridNumber, 91, 45, 0.0F, 0.0F, 90.0F, 180.0F, 2.0F, 2.0F, unchecked((byte) 0x88), (byte) 64);
		  case 62:
		  case 63:
			return new NcepLatLon(gridNumber, 91, 45, -90.0F, 0.0F, 0.0F, 180.0F, 2.0F, 2.0F, unchecked((byte) 0x88), (byte) 64);
		  case 64:
			return new NcepLatLon(gridNumber, 91, 45, -90.0F, -180.0F, 0.0F, 0.0F, 2.0F, 2.0F, unchecked((byte) 0x88), (byte) 64);
		  case 87:
			return new NcepPS(gridNumber, 81, 62, 22.8756F, 239.5089F, 255.0F, 68153.0F, 68153.0F, (byte) 0x08, (byte) 64);
		}
		throw new ArgumentException("Dont have predefined GDS " + gridNumber + " from NCEP (center 7)");
	  }

	  private class NcepLatLon : Grib1Gds.LatLon
	  {
		internal readonly int gridNumber;

		internal NcepLatLon(int gridNumber, int nx, int ny, float la1, float lo1, float la2, float lo2, float deltaLon, float deltaLat, byte resolution, byte scan) : base(1000 * gridNumber)
		{
		  this.gridNumber = gridNumber;
		  this.nx = nx;
		  this.ny = ny;
		  this.la1 = la1;
		  this.lo1 = lo1;
		  this.la2 = la2;
		  this.lo2 = lo2;
		  this.deltaLon = deltaLon;
		  this.deltaLat = deltaLat;
		  this.resolution = resolution;
		  this.scanMode = scan;
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

		  NcepLatLon that = (NcepLatLon) o;
		  return gridNumber == that.gridNumber;
		}

		public override int GetHashCode()
		{
		  return gridNumber;
		}
	  }

	  private class NcepPS : Grib1Gds.PolarStereographic
	  {
		internal readonly int gridNumber;

		internal NcepPS(int gridNumber, int nx, int ny, float la1, float lo1, float lov, float dX, float dY, byte resolution, byte scan) : base(1000 * gridNumber)
		{
		  this.gridNumber = gridNumber;
		  this.nx = nx;
		  this.ny = ny;
		  this.la1 = la1;
		  this.lo1 = lo1;
		  this.lov = lov;
		  this.dX = dX;
		  this.dY = dY;
		  this.resolution = resolution;
		  this.scanMode = scan;
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

		  NcepPS that = (NcepPS) o;
		  return gridNumber == that.gridNumber;
		}

		public override int GetHashCode()
		{
		  return gridNumber;
		}
	  }

	}



}
