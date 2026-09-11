// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.grib1
{
	using GribNumbers = NetcdfSharp.ucar.nc2.grib.GribNumbers;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;

	/// <summary>
	/// The Grid Definition Section for GRIB-1 files
	/// Effectively immutable, but caching lazy gds
	/// 
	/// @author caron
	/// </summary>

	// @Immutable gds makes it not immutable
	public class Grib1SectionGridDefinition
	{
	  private readonly byte[] rawData;
	  private readonly long startingPosition;
	  private readonly int gridTemplate; // octet 6

	  private readonly int predefinedGridDefinition; // = -1 if not used
	  private readonly int predefinedGridDefinitionCenter; // = -1 if not used

	  /// <summary>
	  /// Read Grib Definition section from raf.
	  /// </summary>
	  /// <param name="raf"> RandomAccessFile, with pointer at start of section </param>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on I/O error </exception>
	  /// <exception cref="IllegalArgumentException"> if not a GRIB-2 record </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public Grib1SectionGridDefinition(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public Grib1SectionGridDefinition(RandomAccessFile raf)
	  {

		startingPosition = raf.FilePointer;

		// octets 1-3 (Length of GDS)
		int length = GribNumbers.int3(raf);

		// octet 6
		raf.skipBytes(2);
		gridTemplate = GribNumbers.@uint(raf);

		// read in whole GDS as byte[]
		rawData = new byte[length];
		raf.seek(startingPosition);
		raf.readFully(rawData);

		predefinedGridDefinition = -1;
		predefinedGridDefinitionCenter = -1;
	  }

	  /// <summary>
	  /// Set Grib Definition section from byte array.
	  /// </summary>
	  /// <param name="rawData"> the byte array </param>
	  public Grib1SectionGridDefinition(byte[] rawData)
	  {
		this.rawData = rawData;
		this.gridTemplate = getOctet(6);
		this.startingPosition = -1;
		predefinedGridDefinition = -1;
		predefinedGridDefinitionCenter = -1;
	  }

	  public Grib1SectionGridDefinition(Grib1SectionProductDefinition pds)
	  {
		startingPosition = -1;
		gridTemplate = -pds.GridDefinition; // LOOK ??
		rawData = null;
		predefinedGridDefinitionCenter = pds.Center;
		predefinedGridDefinition = pds.GridDefinition;
	  }

	  /// <summary>
	  /// get the raw bytes of the GDS
	  /// </summary>
	  /// <returns> GDS as byte[] </returns>
	  public virtual byte[] RawBytes
	  {
		  get
		  {
			return rawData;
		  }
	  }

	  /// <summary>
	  /// Calculate the CRC of the entire byte array
	  /// </summary>
	  /// <returns> CRC of the entire byte array </returns>
	  public virtual long calcCRC()
	  {
		long crc;
		if (rawData == null)
		{
		  crc = predefinedGridDefinitionCenter << 16 + predefinedGridDefinition;
		}
		else
		{
		  CRC32 crc32 = new CRC32();
		  crc32.update(rawData);
		  crc = crc32.getValue();
		}
		return crc;
	  }

	  public virtual int Length
	  {
		  get
		  {
			return (rawData == null) ? 0 : rawData.Length;
		  }
	  }

	  public virtual long Offset
	  {
		  get
		  {
			return startingPosition;
		  }
	  }

	  /// <summary>
	  /// Get Grid Template number (code table 6)
	  /// </summary>
	  /// <returns> Grid Template number. </returns>
	  public virtual int GridTemplate
	  {
		  get
		  {
			return gridTemplate;
		  }
	  }

	  /// <summary>
	  /// From pds.getGridDefinition
	  /// </summary>
	  /// <returns> pds.getGridDefinition, or -1 if uses a real Grib1SectionGridDefinition </returns>
	  public virtual int PredefinedGridDefinition
	  {
		  get
		  {
			return predefinedGridDefinition;
		  }
	  }

	  private int getOctet(int index)
	  {
		if (rawData == null)
		{
		  return 255; // predefined
		}
		if (index > rawData.Length)
		{
		  return GribNumbers.UNDEFINED;
		}
		return rawData[index - 1] & 0xff;
	  }

	  private Grib1Gds gds;

	  public virtual Grib1Gds GDS
	  {
		  get
		  {
			  lock (this)
			  {
				if (gds != null)
				{
				  return gds;
				}

				if (predefinedGridDefinition != -1)
				{
				  gds = NetcdfSharp.ucar.nc2.grib.grib1.Grib1GdsPredefined.factory(predefinedGridDefinitionCenter, predefinedGridDefinition);
				  return gds;
				}

				gds = Grib1Gds.factory(gridTemplate, rawData);
				if (Thin)
				{
				  gds.NptsInLine = getNptsInLine(gds);
				}
				return gds;
			  }
		  }
	  }


	  //////////////////////////////////
	  // the infinite grib1 baloney

	  /// <summary>
	  /// is a "thin" grid
	  /// </summary>
	  /// <returns> if a thin grid </returns>
	  public bool Thin
	  {
		  get
		  {
			if (rawData == null)
			{
			  return false;
			}
			int octet5 = getOctet(5);
			int nv = getOctet(4);
			return (octet5 != 255) && (nv == 0 || nv == 255);
		  }
	  }

	  /// <summary>
	  /// Gets the number of points in each line for Quasi/Thin grids
	  /// "List of numbers of points in each row (length = NROWS x 2 octets, where NROWS is the
	  /// total number of rows defined within the grid description)"
	  /// </summary>
	  /// <returns> number of points in each line as int[] </returns>
	  private int[] getNptsInLine(Grib1Gds gds)
	  {
		int numPts;
		if ((gds.ScanMode & 32) == 0)
		{ // bit3 = 0 : Adjacent points in i direction are consecutive
		  numPts = gds.Ny;
		}
		else
		{ // bit3 = 1 : Adjacent points in j direction are consecutive
		  numPts = gds.Nx;
		}

		int[] parallels = new int[numPts];
		int offset = getOctet(5);
		for (int i = 0; i < numPts; i++)
		{
		  parallels[i] = GribNumbers.int2(getOctet(offset++), getOctet(offset++));
		}
		return parallels;
	  }

	  /////////////////////////////////////////////////

	  public virtual bool hasVerticalCoordinateParameters()
	  {
		if (rawData == null)
		{
		  return false;
		}
		int octet5 = getOctet(5);
		int nv = getOctet(4);
		return (octet5 != 255) && (nv != 0 && nv != 255);
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable private double[] getVerticalCoordinateParameters()
	  private double[] VerticalCoordinateParameters
	  {
		  get
		  {
			if (!hasVerticalCoordinateParameters())
			{
			  return null;
			}

			int offset = getOctet(5);
			int n = getOctet(4);
			double[] vertCoords = new double[n];
			for (int i = 0; i < n; i++)
			{
			  vertCoords[i] = GribNumbers.float4(getOctet(offset++), getOctet(offset++), getOctet(offset++), getOctet(offset++));
			}
			return vertCoords;
		  }
	  }

	  public override string ToString()
	  {
		using (Formatter sb = new Formatter())
		{
		  sb.format("Grib1SectionGridDefinition");
		  sb.format("  gridTemplate=%d%n", gridTemplate);
		  if (predefinedGridDefinition >= 0)
		  {
			sb.format("  predefinedGridDefinition=%d%n", predefinedGridDefinition);
		  }
		  double[] verts = VerticalCoordinateParameters;
		  if (verts != null)
		  {
			sb.format("  verticalPressureLevels (%d)=", verts.Length);
			foreach (double d in verts)
			{
			  sb.format("%10.4f ", d);
			}
			sb.format("%n");
		  }
		  sb.format("%n");
		  return sb.ToString();
		}
	  }
	}


}
