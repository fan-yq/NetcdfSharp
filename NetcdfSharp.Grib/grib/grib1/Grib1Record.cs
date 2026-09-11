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
	using GribData = NetcdfSharp.ucar.nc2.grib.GribData;
	using GribNumbers = NetcdfSharp.ucar.nc2.grib.GribNumbers;
	using QuasiRegular = NetcdfSharp.ucar.nc2.grib.QuasiRegular;
	using Grib1Customizer = NetcdfSharp.ucar.nc2.grib.grib1.tables.Grib1Customizer;
	using CalendarDate = NetcdfSharp.ucar.nc2.time.CalendarDate;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;

	/// <summary>
	/// A Grib1 message.
	/// 
	/// @author John
	/// @since 9/3/11
	/// </summary>
	public class Grib1Record
	{

	  private readonly Grib1SectionIndicator @is;
	  private readonly Grib1SectionGridDefinition gdss;
	  private readonly Grib1SectionProductDefinition pdss;
	  private readonly Grib1SectionBitMap bitmap;
	  private readonly Grib1SectionBinaryData dataSection;

	  private readonly byte[] header; // anything in between the records - eg idd header
	  private int file; // for multiple files in same dataset

	  /// <summary>
	  /// Construction for Grib1Record.
	  /// </summary>
	  /// <param name="header"> Grib header </param>
	  /// <param name="is"> Grib1IndicatorSection </param>
	  /// <param name="gdss"> Grib1GridDefinitionSection </param>
	  /// <param name="pdss"> Grib1ProductDefinitionSection </param>
	  /// <param name="bitmap"> Grib1SectionBitMap </param>
	  /// <param name="dataSection"> Grib1SectionData </param>
	  public Grib1Record(byte[] header, Grib1SectionIndicator @is, Grib1SectionGridDefinition gdss, Grib1SectionProductDefinition pdss, Grib1SectionBitMap bitmap, Grib1SectionBinaryData dataSection)
	  {

		this.header = header;
		this.@is = @is;
		this.gdss = gdss;
		this.pdss = pdss;
		this.bitmap = bitmap;
		this.dataSection = dataSection;
	  }

	  /// <summary>
	  /// Construct record by reading raf, no checking
	  /// </summary>
	  /// <param name="raf"> positioned at start of message: the 'G' in "GRIB" </param>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public Grib1Record(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public Grib1Record(RandomAccessFile raf)
	  {
		this.header = null;
		@is = new Grib1SectionIndicator(raf);
		pdss = new Grib1SectionProductDefinition(raf);
		gdss = pdss.gdsExists() ? new Grib1SectionGridDefinition(raf) : new Grib1SectionGridDefinition(pdss);
		bitmap = pdss.bmsExists() ? new Grib1SectionBitMap(raf) : null;
		dataSection = new Grib1SectionBinaryData(raf);
	  }

	  // copy constructor
	  internal Grib1Record(Grib1Record from)
	  {
		this.header = from.header;
		this.@is = from.@is;
		this.gdss = from.gdss;
		this.pdss = from.pdss;
		this.bitmap = from.bitmap;
		this.dataSection = from.dataSection;
	  }

	  public virtual byte[] Header
	  {
		  get
		  {
			return header;
		  }
	  }

	  public virtual Grib1SectionIndicator Is
	  {
		  get
		  {
			return @is;
		  }
	  }

	  public virtual Grib1SectionGridDefinition GDSsection
	  {
		  get
		  {
			return gdss;
		  }
	  }

	  public virtual Grib1Gds GDS
	  {
		  get
		  {
			return gdss.GDS;
		  }
	  }

	  public virtual Grib1SectionProductDefinition PDSsection
	  {
		  get
		  {
			return pdss;
		  }
	  }

	  public virtual Grib1SectionBitMap BitMapSection
	  {
		  get
		  {
			return bitmap;
		  }
	  }

	  public virtual Grib1SectionBinaryData DataSection
	  {
		  get
		  {
			return dataSection;
		  }
	  }

	  public virtual CalendarDate ReferenceDate
	  {
		  get
		  {
			return pdss.ReferenceDate;
		  }
	  }

	  public virtual int File
	  {
		  get
		  {
			return file;
		  }
		  set
		  {
			this.file = value;
		  }
	  }


	  private Grib1ParamTime ptime;

	  public virtual Grib1ParamTime getParamTime(Grib1Customizer cust)
	  {
		if (ptime == null)
		{
		  ptime = cust.getParamTime(pdss);
		}
		return ptime;
	  }

	  /////////////// reading data

	  // isolate dependencies here - in case we have a "minimal I/O" mode where not all fields are available
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public float[] readData(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public virtual float[] readData(RandomAccessFile raf)
	  {
		return readData(raf, GribData.getInterpolationMethod());
	  }

	  // dont convertQuasiGrid
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public float[] readDataRaw(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public virtual float[] readDataRaw(RandomAccessFile raf)
	  {
		return readData(raf, GribData.InterpolationMethod.none);
	  }

	  // isolate dependencies here - in case we have a "minimal I/O" mode where not all fields are available
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public float[] readData(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, NetcdfSharp.ucar.nc2.grib.GribData.InterpolationMethod method) throws NetcdfSharp.java.io.IOException
	  public virtual float[] readData(RandomAccessFile raf, GribData.InterpolationMethod method)
	  {
		Grib1Gds gds = GDS;
		Grib1DataReader reader = new Grib1DataReader(pdss.DecimalScale, gds.ScanMode, gds.NxRaw, gds.NyRaw, gds.Npts, dataSection.StartingPosition);
		byte[] bm = (bitmap == null) ? null : bitmap.getBitmap(raf);
		float[] data = reader.getData(raf, bm);

		if (gdss.Thin)
		{
		  data = QuasiRegular.convertQuasiGrid(data, gds.NptsInLine, gds.NxRaw, gds.NyRaw, method);
		}

		lastRecordRead = this;
		return data;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public void showDataInfo(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, NetcdfSharp.java.util.Formatter f) throws NetcdfSharp.java.io.IOException
	  public virtual void showDataInfo(RandomAccessFile raf, Formatter f)
	  {
		Grib1Gds gds = GDS;
		f.format(" decimal scale = %d%n", pdss.DecimalScale);
		f.format("     scan mode = %d%n", gds.ScanMode);
		f.format("            nx = %d%n", gds.Nx);
		f.format("            ny = %d%n", gds.Ny);
		f.format("         nxRaw = %d%n", gds.NxRaw);
		f.format("         nyRaw = %d%n", gds.NyRaw);
		f.format("          Npts = %d%n", gds.Npts);
		f.format("        isThin = %s%n", gdss.Thin);

		GribData.Info info = getBinaryDataInfo(raf);
		f.format("   dataLength = %d%n", info.dataLength);

		// octet 4, 1st half (packing flag)
		f.format("    ----flag = %s%n", Convert.ToString(info.flag, 2));
		f.format("        data = %s%n", info.GridPointS);
		f.format("     packing = %s%n", info.PackingS);
		f.format("        type = %s%n", info.DataTypeS);
		f.format(" moreOctet14 = %s%n", info.hasOctet14());
		f.format("    binscale = %d%n", info.binaryScaleFactor);
		f.format("    decscale = %d%n", info.decimalScaleFactor);
		f.format("reference value = %f%n", info.referenceValue);
		f.format("      nbits = %d%n", info.numberOfBits);

		Grib1DataReader.showComplexPackingInfo(f, raf, dataSection.StartingPosition);
	  }

	  /// <summary>
	  /// Read data array by first reading in GribRecord.
	  /// All sections are read in, so scanMode is from the datafile, not the index.
	  /// </summary>
	  /// <param name="raf"> from this RandomAccessFile </param>
	  /// <param name="startPos"> message starts here </param>
	  /// <returns> data as float[] array </returns>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static float[] readData(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, long startPos) throws NetcdfSharp.java.io.IOException
	  public static float[] readData(RandomAccessFile raf, long startPos)
	  {
		raf.seek(startPos);
		Grib1Record gr = new Grib1Record(raf);
		return gr.readData(raf);
	  }

	  // debugging, do not use
	  public static Grib1Record lastRecordRead;

	  /*
	   * Read data array: use when you want to be independent of the GribRecord
	   *
	   * @param raf from this RandomAccessFile
	   * 
	   * @param bmPos bitmap.start
	   * 
	   * @param decimalScale pds.decimalScale
	   * 
	   * @param scanMode gds.scanMode
	   * 
	   * @param nx gds.nx
	   * 
	   * @param ny gds.ny
	   * 
	   * @return data as float[] array
	   * 
	   * @throws IOException on read error
	   *
	   * public static float[] readData(RandomAccessFile raf, long bmPos, int decimalScale, int scanMode, int nx, int ny)
	   * throws IOException {
	   * raf.seek(bmPos);
	   * Grib1SectionBitMap bms = new Grib1SectionBitMap(raf);
	   * Grib1DataReader reader = new Grib1DataReader(decimalScale, scanMode, nx, ny, raf.getFilePointer());
	   * 
	   * boolean[] bitmap = bms.getBitmap(raf);
	   * 
	   * return reader.getData(raf, bitmap);
	   * }
	   */

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public NetcdfSharp.ucar.nc2.grib.GribData.Info getBinaryDataInfo(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public virtual GribData.Info getBinaryDataInfo(RandomAccessFile raf)
	  {
		GribData.Info info = dataSection.getBinaryDataInfo(raf);
		info.decimalScaleFactor = pdss.DecimalScale;
		info.bitmapLength = (bitmap == null) ? 0 : bitmap.getLength(raf);
		info.nPoints = GDS.Npts;
		info.msgLength = @is.MessageLength;

		if (bitmap == null)
		{
		  info.ndataPoints = info.nPoints;
		}
		else
		{
		  byte[] bm = bitmap.getBitmap(raf);
		  if (bm == null)
		  {
			info.ndataPoints = info.nPoints;
		  }
		  else
		  { // have to count the bits to see how many data values are stored
			info.ndataPoints = GribNumbers.countBits(bm);
		  }
		}

		return info;
	  }

	  // debugging - do not use
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public int[] readRawData(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public virtual int[] readRawData(RandomAccessFile raf)
	  {
		Grib1Gds gds = GDS;
		Grib1DataReader reader = new Grib1DataReader(pdss.DecimalScale, gds.ScanMode, gds.NxRaw, gds.NyRaw, gds.Npts, dataSection.StartingPosition);
		byte[] bm = (bitmap == null) ? null : bitmap.getBitmap(raf);
		return reader.getDataRaw(raf, bm);
	  }
	}

}
