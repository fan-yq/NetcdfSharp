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

namespace NetcdfSharp.ucar.nc2.grib.grib2
{
	using GribNumbers = NetcdfSharp.ucar.nc2.grib.GribNumbers;
	using CalendarDate = NetcdfSharp.ucar.nc2.time.CalendarDate;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;

	/// <summary>
	/// The Identification section 1 for GRIB-2 files
	/// 
	/// @author caron
	/// @since 3/28/11
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class Grib2SectionIdentification
	public class Grib2SectionIdentification
	{
	  private readonly int center_id;
	  private readonly int subcenter_id;
	  private readonly int master_table_version;
	  private readonly int local_table_version;
	  private readonly int significanceOfRT;
	  private readonly int year, month, day, hour, minute, second;
	  private readonly int productionStatus;
	  private readonly int processedDataType;

	  /// <summary>
	  /// Read Grib2SectionIndicator from raf.
	  /// </summary>
	  /// <param name="raf"> RandomAccessFile, with pointer at start of indicator section </param>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on I/O error </exception>
	  /// <exception cref="IllegalArgumentException"> if not a GRIB-2 record </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public Grib2SectionIdentification(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public Grib2SectionIdentification(RandomAccessFile raf)
	  {
		long sectionEnd = raf.FilePointer;

		// section 1 octet 1-4 (length of section)
		int length = GribNumbers.int4(raf);
		sectionEnd += length;

		int section = raf.read();
		if (section != 1)
		{
		  throw new ArgumentException("Not a GRIB-2 Identification section");
		}

		// Center octet 6-7
		center_id = GribNumbers.int2(raf);

		// subCenter octet 8-9
		subcenter_id = GribNumbers.int2(raf);

		// master table octet 10 (code table 1.0)
		master_table_version = raf.read();

		// local table octet 11 (code table 1.1)
		local_table_version = raf.read();

		// significanceOfRT octet 12 (code table 1.1)
		significanceOfRT = raf.read();

		// octets 13-19 (reference time of forecast)
		year = GribNumbers.int2(raf);
		month = raf.read();
		day = raf.read();
		hour = raf.read();
		minute = raf.read();
		second = raf.read();
		// refTime = CalendarDate.of(null, year, month, day, hour, minute, second);

		productionStatus = raf.read();
		processedDataType = raf.read();

		raf.seek(sectionEnd);
	  }

	  public Grib2SectionIdentification(int center_id, int subcenter_id, int master_table_version, int local_table_version, int significanceOfRT, int year, int month, int day, int hour, int minute, int second, int productionStatus, int processedDataType)
	  {
		this.center_id = center_id;
		this.subcenter_id = subcenter_id;
		this.master_table_version = master_table_version;
		this.local_table_version = local_table_version;
		this.significanceOfRT = significanceOfRT;
		this.year = year;
		this.month = month;
		this.day = day;
		this.hour = hour;
		this.minute = minute;
		this.second = second;
		this.productionStatus = productionStatus;
		this.processedDataType = processedDataType;
	  }

	  /// <summary>
	  /// Identification of center (Common Code Table C-11)
	  /// </summary>
	  /// <returns> center id </returns>
	  public virtual int Center_id
	  {
		  get
		  {
			return center_id;
		  }
	  }

	  /// <summary>
	  /// Identification of subcenter (allocated by center)
	  /// </summary>
	  /// <returns> subcenter </returns>
	  public virtual int Subcenter_id
	  {
		  get
		  {
			return subcenter_id;
		  }
	  }

	  /// <summary>
	  /// Parameter Table Version number (code table 1.0)
	  /// </summary>
	  /// <returns> master_table_version as int </returns>
	  public virtual int Master_table_version
	  {
		  get
		  {
			return master_table_version;
		  }
	  }

	  /// <summary>
	  /// local table version number (code table 1.1)
	  /// </summary>
	  /// <returns> local_table_version as int </returns>
	  public virtual int Local_table_version
	  {
		  get
		  {
			return local_table_version;
		  }
	  }

	  /// <summary>
	  /// Note 2 : if true, all entries come from the local table (!)
	  /// </summary>
	  /// <returns> useLocalTablesOnly </returns>
	  public virtual bool useLocalTablesOnly()
	  {
		return master_table_version == 255 && local_table_version != 0;
	  }

	  /// <summary>
	  /// Significance of Reference time (code table 1.2)
	  /// </summary>
	  /// <returns> significanceOfRT as int </returns>
	  public virtual int SignificanceOfRT
	  {
		  get
		  {
			return significanceOfRT;
		  }
	  }

	  /// <summary>
	  /// reference reference or base time as Dare.
	  /// </summary>
	  /// <returns> baseTime </returns>
	  public virtual CalendarDate ReferenceDate
	  {
		  get
		  {
			return CalendarDate.of(null, year, month, day, hour, minute, second);
		  }
	  }

	  /// <summary>
	  /// production Status (code table 1.3)
	  /// </summary>
	  /// <returns> production Status </returns>
	  public virtual int ProductionStatus
	  {
		  get
		  {
			return productionStatus;
		  }
	  }

	  /// <summary>
	  /// Get type Of Processed Data (code table 1.4)
	  /// </summary>
	  /// <returns> productType as int </returns>
	  public virtual int TypeOfProcessedData
	  {
		  get
		  {
			return processedDataType;
		  }
	  }

	  public virtual int Year
	  {
		  get
		  {
			return year;
		  }
	  }

	  public virtual int Month
	  {
		  get
		  {
			return month;
		  }
	  }

	  public virtual int Day
	  {
		  get
		  {
			return day;
		  }
	  }

	  public virtual int Hour
	  {
		  get
		  {
			return hour;
		  }
	  }

	  public virtual int Minute
	  {
		  get
		  {
			return minute;
		  }
	  }

	  public virtual int Second
	  {
		  get
		  {
			return second;
		  }
	  }

	  public override string ToString()
	  {
		return "id {" + "center_id=" + center_id + ", subcenter_id=" + subcenter_id + ", master_table_version=" + master_table_version + ", local_table_version=" + local_table_version + '}';
	  }
	}

}
