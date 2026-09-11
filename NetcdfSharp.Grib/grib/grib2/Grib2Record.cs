// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.grib2
{
	using GribData = NetcdfSharp.ucar.nc2.grib.GribData;
	using QuasiRegular = NetcdfSharp.ucar.nc2.grib.QuasiRegular;
	using CalendarDate = NetcdfSharp.ucar.nc2.time.CalendarDate;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;
	using StringUtil2 = NetcdfSharp.ucar.unidata.util.StringUtil2;

	/// <summary>
	/// Read one Record from a GRIB-2 files
	/// 
	/// @author caron
	/// @since 3/28/11
	/// </summary>
	public class Grib2Record
	{

	  //////////////////////////////////////////////////////////////
	  private readonly Grib2SectionIndicator @is;
	  private readonly Grib2SectionIdentification id;
	  private Grib2SectionLocalUse lus; // local use section
	  private Grib2SectionGridDefinition gdss;
	  private Grib2SectionProductDefinition pdss;
	  private Grib2SectionDataRepresentation drss;
	  private Grib2SectionBitMap bms;
	  private Grib2SectionData dataSection;

	  private Grib2Pds pds2;

	  private readonly byte[] header; // anything in between the records - eg idd header
	  private int file; // for multiple files in same dataset
	  private bool bmsReplaced;
	  private int scanMode;

	  public int repeat; // debug = see Grib2Report.doDrsSummary

	  /// <summary>
	  /// Construction for Grib2Record.
	  /// </summary>
	  /// <param name="header"> Grib header </param>
	  /// <param name="is"> Grib2IndicatorSection </param>
	  /// <param name="id"> Grib2IdentificationSection </param>
	  /// <param name="lus"> raw bytes of local use section </param>
	  /// <param name="gdss"> Grib2GridDefinitionSection </param>
	  /// <param name="pdss"> Grib2ProductDefinitionSection </param>
	  /// <param name="drs"> Grib2SectionDataRepresentation </param>
	  /// <param name="bms"> Grib2SectionBitMap </param>
	  /// <param name="dataSection"> Grib2SectionData </param>
	  /// <param name="bmsReplaced"> Grib2SectionData </param>
	  /// <param name="scanMode"> from GDS of this record </param>
	  public Grib2Record(byte[] header, Grib2SectionIndicator @is, Grib2SectionIdentification id, Grib2SectionLocalUse lus, Grib2SectionGridDefinition gdss, Grib2SectionProductDefinition pdss, Grib2SectionDataRepresentation drs, Grib2SectionBitMap bms, Grib2SectionData dataSection, bool bmsReplaced, int scanMode)
	  {

		this.header = header;
		this.@is = @is;
		this.id = id;
		this.lus = lus;
		this.gdss = gdss;
		this.pdss = pdss;
		this.drss = drs;
		this.bms = bms;
		this.dataSection = dataSection;
		this.bmsReplaced = bmsReplaced;

		// stored in index file after 4.5 2/6/2014, otherwise equals Grib2Index.ScanModeMissing, so get it from the GDS,
		// which may have wrong one
		this.scanMode = scanMode;
		if (scanMode == Grib2Index.ScanModeMissing && gdss != null)
		{
		  this.scanMode = gdss.GDS.ScanMode;
		}
	  }

	  // copy constructor
	  internal Grib2Record(Grib2Record from)
	  {
		this.header = from.header;
		this.@is = from.@is;
		this.id = from.id;
		this.lus = from.lus;
		this.gdss = from.gdss;
		this.pdss = from.pdss;
		this.drss = from.drss;
		this.bms = from.bms;
		this.dataSection = from.dataSection;
		this.repeat = from.repeat;
		this.bmsReplaced = from.bmsReplaced;
		this.scanMode = from.scanMode;
	  }

	  public virtual byte[] Header
	  {
		  get
		  {
			return header;
		  }
	  }

	  public virtual Grib2SectionIndicator Is
	  {
		  get
		  {
			return @is;
		  }
	  }

	  public virtual Grib2SectionIdentification Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  public virtual bool hasLocalUseSection()
	  {
		return lus != null && lus.RawBytes != null;
	  }

	  public virtual Grib2SectionLocalUse LocalUseSection
	  {
		  get
		  {
			return lus;
		  }
	  }

	  public virtual Grib2SectionGridDefinition GDSsection
	  {
		  get
		  {
			return gdss;
		  }
	  }

	  public virtual Grib2SectionProductDefinition PDSsection
	  {
		  get
		  {
			return pdss;
		  }
	  }

	  public virtual Grib2SectionDataRepresentation DataRepresentationSection
	  {
		  get
		  {
			return drss;
		  }
	  }

	  public virtual Grib2SectionBitMap BitmapSection
	  {
		  get
		  {
			return bms;
		  }
	  }

	  public virtual Grib2SectionData DataSection
	  {
		  get
		  {
			return dataSection;
		  }
		  set
		  {
			this.dataSection = value;
		  }
	  }

	  public virtual int Discipline
	  {
		  get
		  {
			return @is.Discipline;
		  }
	  }

	  public virtual CalendarDate ReferenceDate
	  {
		  get
		  {
			return id.ReferenceDate;
		  }
	  }

	  public virtual Grib2Pds PDS
	  {
		  get
		  {
			if (pds2 == null)
			{
			  pds2 = pdss.PDS;
			}
			return pds2;
		  }
	  }

	  public virtual Grib2Gds GDS
	  {
		  get
		  {
			  lock (this)
			  {
				return gdss.GDS;
			  }
		  }
	  }

	  public virtual int ScanMode
	  {
		  get
		  {
			return scanMode;
		  }
	  }

	  public virtual void show(Formatter f)
	  {
		f.format("discipline=%d ", @is.Discipline);
		Grib2Pds pds = PDS;
		pds.show(f);
	  }

	  //////////////////////////////////////////
	  // setters used by repeating records


	  public virtual Grib2SectionLocalUse Lus
	  {
		  set
		  {
			this.lus = value;
		  }
	  }

	  public virtual Grib2SectionGridDefinition Gdss
	  {
		  set
		  {
			this.gdss = value;
		  }
	  }

	  public virtual Grib2SectionProductDefinition Pdss
	  {
		  set
		  {
			this.pdss = value;
		  }
	  }

	  public virtual Grib2SectionDataRepresentation Drs
	  {
		  set
		  {
			this.drss = value;
		  }
	  }

	  public virtual void setBms(Grib2SectionBitMap bms, bool replaced)
	  {
		this.bms = bms;
		this.bmsReplaced = replaced;
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


	  public virtual bool BmsReplaced
	  {
		  get
		  {
			return bmsReplaced;
		  }
	  }

	  public override string ToString()
	  {
		return MoreObjects.toStringHelper(this).add("is", @is).add("id", id).add("lus", lus).add("gdss", gdss).add("pdss", pdss).add("drss", drss).add("bms", bms).add("dataSection", dataSection).add("pds2", pds2).add("header", header).add("file", file).add("bmsReplaced", bmsReplaced).add("scanMode", scanMode).add("repeat", repeat).ToString();
	  }

	  // isolate dependencies here - in case we have a "minimal I/O" mode where not all fields are available
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public float[] readData(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public virtual float[] readData(RandomAccessFile raf)
	  {
		Grib2Gds gds = GDS;

		Grib2DataReader reader = new Grib2DataReader(drss.DataTemplate, gdss.NumberPoints, drss.DataPoints, ScanMode, gds.NxRaw, dataSection.StartingPosition, dataSection.MsgLength);

		Grib2Drs gdrs = drss.getDrs(raf);

		float[] data = reader.getData(raf, bms, gdrs);

		if (gds.Thin)
		{
		  data = QuasiRegular.convertQuasiGrid(data, gds.NptsInLine, gds.NxRaw, gds.NyRaw, GribData.getInterpolationMethod());
		}

		lastRecordRead = this;
		return data;
	  }

	  // debugging - do not use
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Nullable public int[] readRawData(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public virtual int[] readRawData(RandomAccessFile raf)
	  {
		Grib2Gds gds = GDS;

		Grib2DataReader reader = new Grib2DataReader(drss.DataTemplate, gdss.NumberPoints, drss.DataPoints, ScanMode, gds.NxRaw, dataSection.StartingPosition, dataSection.MsgLength);

		Grib2Drs gdrs = drss.getDrs(raf);

		return reader.getRawData(raf, bms, gdrs);
	  }

	  // debugging - do not use
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Nullable public Grib2Drs.Type40 readDataTest(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public virtual Grib2Drs.Type40 readDataTest(RandomAccessFile raf)
	  {
		Grib2Gds gds = GDS;

		Grib2DataReader reader = new Grib2DataReader(drss.DataTemplate, gdss.NumberPoints, drss.DataPoints, ScanMode, gds.NxRaw, dataSection.StartingPosition, dataSection.MsgLength);

		Grib2Drs gdrs = drss.getDrs(raf);
		if (gdrs is Grib2Drs.Type40)
		{
		  reader.getData(raf, bms, gdrs);
		  return (Grib2Drs.Type40) gdrs;
		}
		return null;
	  }

	  /// <summary>
	  /// Read data array
	  /// </summary>
	  /// <param name="raf"> from this RandomAccessFile </param>
	  /// <param name="drsPos"> Grib2SectionDataRepresentation starts here </param>
	  /// <returns> data as float[] array </returns>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public float[] readData(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, long drsPos) throws NetcdfSharp.java.io.IOException
	  public virtual float[] readData(RandomAccessFile raf, long drsPos)
	  {
		raf.seek(drsPos);
		Grib2SectionDataRepresentation drs = new Grib2SectionDataRepresentation(raf);
		Grib2SectionBitMap bms = new Grib2SectionBitMap(raf);
		Grib2SectionData dataSection = new Grib2SectionData(raf);

		Grib2Gds gds = GDS;
		Grib2DataReader reader = new Grib2DataReader(drs.DataTemplate, gdss.NumberPoints, drs.DataPoints, ScanMode, gds.NxRaw, dataSection.StartingPosition, dataSection.MsgLength);

		Grib2Drs gdrs = drs.getDrs(raf);

		float[] data = reader.getData(raf, bms, gdrs);

		if (gds.Thin)
		{
		  data = QuasiRegular.convertQuasiGrid(data, gds.NptsInLine, gds.NxRaw, gds.NyRaw, GribData.getInterpolationMethod());
		}

		lastRecordRead = this;
		return data;
	  }

	  /// <summary>
	  /// Read data array: use when you want to be independent of the GribRecord
	  /// </summary>
	  /// <param name="raf"> from this RandomAccessFile </param>
	  /// <param name="drsPos"> Grib2SectionDataRepresentation starts here </param>
	  /// <param name="bmsPos"> if non-zero, use the bms that starts here </param>
	  /// <param name="gdsNumberPoints"> gdss.getNumberPoints() </param>
	  /// <param name="scanMode"> gds.scanMode </param>
	  /// <param name="nx"> gds.nx </param>
	  /// <returns> data as float[] array </returns>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static float[] readData(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, long drsPos, long bmsPos, int gdsNumberPoints, int scanMode, int nx, int ny, int[] nptsInLine) throws NetcdfSharp.java.io.IOException
	  public static float[] readData(RandomAccessFile raf, long drsPos, long bmsPos, int gdsNumberPoints, int scanMode, int nx, int ny, int[] nptsInLine)
	  {
		raf.seek(drsPos);
		Grib2SectionDataRepresentation drs = new Grib2SectionDataRepresentation(raf);
		Grib2SectionBitMap bms = new Grib2SectionBitMap(raf);
		Grib2SectionData dataSection = new Grib2SectionData(raf);

		if (bmsPos > 0)
		{
		  bms = Grib2SectionBitMap.factory(raf, bmsPos);
		}

		Grib2DataReader reader = new Grib2DataReader(drs.DataTemplate, gdsNumberPoints, drs.DataPoints, scanMode, nx, dataSection.StartingPosition, dataSection.MsgLength);

		Grib2Drs gdrs = drs.getDrs(raf);

		float[] data = reader.getData(raf, bms, gdrs);

		if (nptsInLine != null)
		{
		  data = QuasiRegular.convertQuasiGrid(data, nptsInLine, nx, ny, GribData.getInterpolationMethod());
		}

		if (getlastRecordRead)
		{
		  lastRecordRead = Grib2RecordScanner.findRecordByDrspos(raf, drsPos);
		}
		return data;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public void check(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, NetcdfSharp.java.util.Formatter f) throws NetcdfSharp.java.io.IOException
	  public virtual void check(RandomAccessFile raf, Formatter f)
	  {
		long messLen = @is.MessageLength;
		long startPos = @is.StartPos;
		long endPos = @is.EndPos;

		if (endPos > raf.length())
		{
		  f.format("End of GRIB message (start=%d len=%d) end=%d > file.length=%d for %s%n", startPos, messLen, endPos, raf.length(), raf.Location);
		  return;
		}

		raf.seek(endPos - 4);
		for (int i = 0; i < 4; i++)
		{
		  if (raf.read() != 55)
		  {
			string clean = StringUtil2.cleanup(header);
			if (clean.Length > 40)
			{
			  clean = clean.Substring(0, 40) + "...";
			}
			f.format("Missing End of GRIB message (start=%d len=%d) end=%d header= %s for %s (len=%d)%n", startPos, messLen, endPos, clean, raf.Location, raf.length());
			break;
		  }
		}

		long dataLen = dataSection.MsgLength;
		long dataStart = dataSection.StartingPosition;
		long dataEnd = dataStart + dataLen;

		if (dataEnd > raf.length())
		{
		  f.format("GRIB data section (start=%d len=%d) end=%d > file.length=%d for %s%n", dataStart, dataLen, dataEnd, raf.length(), raf.Location);
		  return;
		}

		if (dataEnd > endPos)
		{
		  f.format("GRIB data section (start=%d len=%d) end=%d > message end=%d for %s%n", dataStart, dataLen, dataEnd, endPos, raf.Location);
		}

	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public NetcdfSharp.ucar.nc2.grib.GribData.Info getBinaryDataInfo(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public virtual GribData.Info getBinaryDataInfo(RandomAccessFile raf)
	  {
		GribData.Info info = this.drss.getDrs(raf).getBinaryDataInfo(raf);
		info.bitmapLength = (bms == null) ? 0 : bms.getLength(raf);
		info.msgLength = @is.MessageLength;
		info.dataLength = dataSection.MsgLength;
		info.ndataPoints = drss.DataPoints;
		Grib2Gds gds = GDS;
		info.nPoints = gds.Nx * gds.Ny;
		return info;
	  }

	  // debugging do not use
	  public static bool getlastRecordRead;
	  public static Grib2Record lastRecordRead;

	}

}
