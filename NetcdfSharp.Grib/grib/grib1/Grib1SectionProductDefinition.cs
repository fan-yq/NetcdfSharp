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
	using GribUtils = NetcdfSharp.ucar.nc2.grib.GribUtils;
	using Grib1Customizer = NetcdfSharp.ucar.nc2.grib.grib1.tables.Grib1Customizer;
	using Grib1ParamTableReader = NetcdfSharp.ucar.nc2.grib.grib1.tables.Grib1ParamTableReader;
	using CalendarDate = NetcdfSharp.ucar.nc2.time.CalendarDate;
	using CalendarDateUnit = NetcdfSharp.ucar.nc2.time.CalendarDateUnit;
	using CalendarPeriod = NetcdfSharp.ucar.nc2.time.CalendarPeriod;
	using CommonCodeTable = NetcdfSharp.ucar.nc2.wmo.CommonCodeTable;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;

	/// <summary>
	/// The Product Definition Section for GRIB-1 files
	/// 
	/// @author caron
	/// </summary>

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public final class Grib1SectionProductDefinition
	public sealed class Grib1SectionProductDefinition
	{

	  private readonly byte[] rawData;

	  /// <summary>
	  /// Read Product Definition section from raf.
	  /// </summary>
	  /// <param name="raf"> RandomAccessFile, with pointer at start of section </param>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on I/O error </exception>
	  /// <exception cref="IllegalArgumentException"> if not a GRIB-2 record </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public Grib1SectionProductDefinition(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public Grib1SectionProductDefinition(RandomAccessFile raf)
	  {
		int length = GribNumbers.uint3(raf);
		rawData = new byte[length];
		raf.skipBytes(-3);
		raf.readFully(rawData);
	  }

	  /// <summary>
	  /// Set PDS section from byte array.
	  /// </summary>
	  /// <param name="rawData"> the byte array </param>
	  public Grib1SectionProductDefinition(byte[] rawData)
	  {
		this.rawData = rawData;
	  }

	  /// <summary>
	  /// get the raw bytes of the PDS.
	  /// </summary>
	  /// <returns> PDS as byte[] </returns>
	  public byte[] RawBytes
	  {
		  get
		  {
			return rawData;
		  }
	  }

	  public int Length
	  {
		  get
		  {
			return GribNumbers.uint3(getOctet(1), getOctet(2), getOctet(3));
		  }
	  }

	  /// <summary>
	  /// gets the Table version (octet 4).
	  /// </summary>
	  /// <returns> table version </returns>
	  public int TableVersion
	  {
		  get
		  {
			return getOctet(4);
		  }
	  }

	  /// <summary>
	  /// Center (octet 5) common code C-1.
	  /// </summary>
	  /// <returns> center id </returns>
	  public int Center
	  {
		  get
		  {
			return getOctet(5);
		  }
	  }

	  /// <summary>
	  /// Generating Process (octet 6).
	  /// </summary>
	  /// <returns> typeGenProcess </returns>
	  public int GenProcess
	  {
		  get
		  {
			return getOctet(6);
		  }
	  }

	  /// <summary>
	  /// Grid Definition (octet 7).
	  /// "Number of grid used – from catalogue defined by originating centre". So this is center dependent.
	  /// "Where octet 7 defines a catalogued grid, that grid should also be defined in Section 2, provided the flag in octet
	  /// 8
	  /// indicates inclusion of Section 2.
	  /// Octet 7 must be set to 255 to indicate a non-catalogued grid, in which case the grid will be defined in Section 2."
	  /// </summary>
	  /// <returns> Grid Definition. </returns>
	  public int GridDefinition
	  {
		  get
		  {
			return getOctet(7);
		  }
	  }

	  /// <summary>
	  /// Flag (octet 8).
	  /// </summary>
	  /// <returns> Flag </returns>
	  public int Flag
	  {
		  get
		  {
			return getOctet(8);
		  }
	  }

	  /// <summary>
	  /// Parameter number (octet 9) - code table 2.
	  /// </summary>
	  /// <returns> index number of parameter in table </returns>
	  public int ParameterNumber
	  {
		  get
		  {
			return getOctet(9);
		  }
	  }

	  /// <summary>
	  /// Level type (octet 10) - code table 3.
	  /// </summary>
	  /// <returns> level type </returns>
	  public int LevelType
	  {
		  get
		  {
			return getOctet(10);
		  }
	  }

	  /// <summary>
	  /// Level value1 (octet 11).
	  /// </summary>
	  /// <returns> level value1 </returns>
	  public int LevelValue1
	  {
		  get
		  {
			return getOctet(11);
		  }
	  }

	  /// <summary>
	  /// Level value2 (octet 12).
	  /// </summary>
	  /// <returns> level value2 </returns>
	  public int LevelValue2
	  {
		  get
		  {
			return getOctet(12);
		  }
	  }

	  /// <summary>
	  /// Reference Date (octet 13-17).
	  /// Reference time of data – date and time of start of averaging or accumulation period.
	  /// </summary>
	  /// <returns> Reference Date as CalendarDate. </returns>
	  public CalendarDate ReferenceDate
	  {
		  get
		  {
			int century = ReferenceCentury - 1;
			if (century == -1)
			{
			  century = 20;
			}

			int year = getOctet(13);
			int month = getOctet(14);
			int day = getOctet(15);
			int hour = getOctet(16);
			int minute = getOctet(17);
			return CalendarDate.of(null, century * 100 + year, month, day, hour, minute, 0);
		  }
	  }

	  /// <summary>
	  /// Time unit (octet 18) - code table 4. Same as GRIB2 Table 4.4
	  /// </summary>
	  /// <returns> time unit </returns>
	  public int TimeUnit
	  {
		  get
		  {
			return getOctet(18);
		  }
	  }

	  /// <summary>
	  /// Time value 1 (octet 19).
	  /// Period of time (number of time units) (0 for analyses or initialized analyses).
	  /// Units of time given by octet 18
	  /// </summary>
	  /// <returns> time value 1 </returns>
	  public int TimeValue1
	  {
		  get
		  {
			return getOctet(19);
		  }
	  }

	  /// <summary>
	  /// Time value 2 (octet 20).
	  /// Period of time (number of time units); or Time interval between successive analyses,
	  /// initialized analyses or forecasts, undergoing averaging or accumulation.
	  /// Units of time given by octet 18
	  /// </summary>
	  /// <returns> time value 2 </returns>
	  public int TimeValue2
	  {
		  get
		  {
			return getOctet(20);
		  }
	  }

	  /// <summary>
	  /// Time range indicator (octet 21) - code table 5.
	  /// </summary>
	  /// <returns> Time range indicator </returns>
	  public int TimeRangeIndicator
	  {
		  get
		  {
			return getOctet(21);
		  }
	  }

	  /// <summary>
	  /// Number included in statistics (octet 22-23).
	  /// Number included in calculation when octet 21 (Code table 5) refers to a statistical
	  /// process, such as average or accumulation; otherwise set to zero
	  /// </summary>
	  /// <returns> Number included in statistics </returns>
	  public int Nincluded
	  {
		  get
		  {
			return GribNumbers.int2(getOctet(22), getOctet(23));
		  }
	  }

	  /// <summary>
	  /// Number missing in statistics (octet 24).
	  /// Number missing from calculation in case of statistical process
	  /// </summary>
	  /// <returns> Number missing in statistics </returns>
	  public int Nmissing
	  {
		  get
		  {
			return getOctet(24);
		  }
	  }

	  /// <summary>
	  /// Century of reference (octet 25).
	  /// </summary>
	  /// <returns> Century of reference </returns>
	  public int ReferenceCentury
	  {
		  get
		  {
			return getOctet(25);
		  }
	  }

	  /// <summary>
	  /// Center (octet 26) common code C-12.
	  /// </summary>
	  /// <returns> subcenter id </returns>
	  public int SubCenter
	  {
		  get
		  {
			return getOctet(26);
		  }
	  }


	  /// <summary>
	  /// Units decimal scale factor (octet 27-28) .
	  /// </summary>
	  /// <returns> Units decimal scale factor </returns>
	  public int DecimalScale
	  {
		  get
		  {
			return GribNumbers.int2(getOctet(27), getOctet(28));
		  }
	  }

	  /// <summary>
	  /// Check if GDS exists from the flag.
	  /// </summary>
	  /// <returns> true, if GDS exists </returns>
	  public bool gdsExists()
	  {
		return (Flag & 128) == 128;
	  }

	  /// <summary>
	  /// Check if BMS exists from the flag
	  /// </summary>
	  /// <returns> true, if BMS exists </returns>
	  public bool bmsExists()
	  {
		return (Flag & 64) == 64;
	  }

	  /// <summary>
	  /// Get the indexth byte in the PDS as an integer.
	  /// THIS IS ONE BASED (not zero) to correspond with the manual
	  /// </summary>
	  /// <param name="index"> 1 based index </param>
	  /// <returns> rawData[index-1] & 0xff </returns>
	  private int getOctet(int index)
	  {
		if (index > rawData.Length)
		{
		  return GribNumbers.UNDEFINED;
		}
		return rawData[index - 1] & 0xff;
	  }

	  /////////////////////////////////////////////////////////////////////

	  private string CalendarPeriodAsString
	  {
		  get
		  {
			try
			{
			  return GribUtils.getCalendarPeriod(TimeUnit).ToString();
			}
			catch (System.NotSupportedException)
			{
			  return "Unknown Time Unit";
			}
		  }
	  }

	  public void showPds(Grib1Customizer cust, Formatter f)
	  {

		f.format("5           Originating Center : (%d) %s%n", Center, CommonCodeTable.getCenterName(Center, 1));
		f.format("26       Originating SubCenter : (%d) %s%n", SubCenter, cust.getSubCenterName(SubCenter));
		f.format("4                Table Version : %d%n", TableVersion);

		Grib1Parameter parameter = cust.getParameter(Center, SubCenter, TableVersion, ParameterNumber);
		if (parameter != null)
		{
		  Grib1ParamTableReader ptable = parameter.Table;
		  f.format("               Parameter Table : (%d-%d-%d) %s%n", Center, SubCenter, TableVersion, (ptable == null) ? "MISSING" : ptable.Path);
		  f.format("                Parameter Name : (%d) %s%n", ParameterNumber, parameter.Name);
		  f.format("                Parameter Desc : %s%n", parameter.Description);
		  f.format("               Parameter Units : %s%n", parameter.Unit);
		}
		else
		{
		  f.format("               Parameter %d not found%n", ParameterNumber);
		}

		f.format("6      Generating Process Type : (%d) %s%n", GenProcess, cust.getGeneratingProcessName(GenProcess));
		f.format("7              Grid Definition : (%d) %n", GridDefinition);
		f.format("8                         Flag : (%d) %n", Flag);

		f.format("13-17           Reference Time : %s%n", ReferenceDate);
		f.format("18                  Time Units : (%d) %s%n", TimeUnit, CalendarPeriodAsString);
		Grib1ParamTime ptime = cust.getParamTime(this);
		f.format("19                 Time 1 (P1) : %d%n", TimeValue1);
		f.format("20                 Time 2 (P2) : %d%n", TimeValue2);
		f.format("21        Time Range Indicator : (%d) %s%n", TimeRangeIndicator, ptime.TimeTypeName);
		f.format("22-23           N in statistic : (%d)%n", Nincluded);
		f.format("24                   N missing : (%d)%n", Nmissing);
		f.format("                   Time  coord : %s == %s%n", ptime.TimeCoord, makeTimeCoord(ptime));
		Grib1ParamLevel plevel = cust.getParamLevel(this);
		f.format("10                  Level Type : (%d) %s%n", LevelType, plevel.NameShort);
		f.format("             Level Description : %s%n", plevel.Description);
		f.format("                 Level Value 1 : %f%n", plevel.Value1);
		f.format("                 Level Value 2 : %f%n", plevel.Value2);
		f.format("27-28     Decimal Scale Factor : %d%n", DecimalScale);

		f.format("                    GDS Exists : %s%n", gdsExists());
		f.format("                    BMS Exists : %s%n", bmsExists());
	  }

	  private string makeTimeCoord(Grib1ParamTime ptime)
	  {
		CalendarPeriod period = GribUtils.getCalendarPeriod(TimeUnit);
		CalendarDateUnit unit = CalendarDateUnit.of(null, period.getField(), ReferenceDate);
		int timeCoord;
		if (ptime.IsInterval())
		{
		  int[] intv = ptime.Interval;
		  CalendarDate cdate1 = unit.makeCalendarDate(period.Value * intv[0]);
		  CalendarDate cdate2 = unit.makeCalendarDate(period.Value * intv[1]);
		  return "(" + cdate1 + "," + cdate2 + ")";
		}
		else
		{
		  timeCoord = ptime.ForecastTime;
		  CalendarDate cdate = unit.makeCalendarDate(period.Value * timeCoord);
		  return cdate.ToString();
		}
	  }

	  ////////////////////////////////////////////////////////
	  // Ensembles

	  /*
	   * http://www.ecmwf.int/publications/manuals/d/gribapi/fm92/grib1/show/local/
	   * 1)
	   * see " local definition # 1"
	   * from Ernst de Vreede via Jitka 01/31/2012
	   * ECMWF operational Ensemble Prediction System (EPS) have extra information in the PDS:
	   * Octet 41 = 1: ECMWF local GRIB extension: 1 = Mars labelling or ensemble forecast data&
	   * Octet 42 = 1: ECMWF GRIB class, 1=operational data
	   * Octet 43 = 10/11: ECMWF GRIB type: 10=Control forecast, 11=Perturbed forecast
	   * Octet 44,45 = 1035, ECMWF GRIB stream 1035 = Ensemble Prediction System
	   * Octet 50 = perturbationNumber (0 for control, 1-50 for perturbed forecasts)
	   * Octet 51 = numberOfForecastsInEnsemble (0 if not ensemble, 1-50 for perturbed forecasts)
	   * 
	   * The really distinguishing octets are: octet 41 =1, octet 42=1, octet 43=10 or 11, octet 51>0 with octet 50 giving
	   * the perturbation number.
	   * The other parameters octets 42 (operational stream) and octet 44/45 might be too specific, might narrow down too
	   * much.
	   * 
	   * 2)
	   * http://www.ecmwf.int/publications/manuals/d/gribapi/fm92/grib1/detail/local/30/
	   */

	  /*
	   * NCEP Appendix C Manual 388
	   * http://www.nco.ncep.noaa.gov/pmb/docs/on388/appendixc.html
	   * states that if the PDS is > 28 bytes and octet 41 == 1
	   * then it's an ensemble an product.
	   */

	  public bool Ensemble
	  {
		  get
		  {
			switch (Center)
			{
			  case 7:
				return ((rawData.Length >= 43) && (getOctet(41) == 1));

			  case 98:
				return ((rawData.Length >= 51) && (getOctet(41) == 1 || getOctet(41) == 30) && (getOctet(43) == 10 || getOctet(43) == 11));
			}
			return false;
		  }
	  }

	  public int PerturbationType
	  {
		  get
		  {
			if (!Ensemble)
			{
			  return GribNumbers.UNDEFINED;
			}
			switch (Center)
			{
			  case 7:
				return getOctet(42);
			  case 98:
				return getOctet(43);
			}
			return GribNumbers.UNDEFINED;
		  }
	  }

	  public int PerturbationNumber
	  {
		  get
		  {
			if (!Ensemble)
			{
			  return GribNumbers.UNDEFINED;
			}

			switch (Center)
			{
			  case 7:
			  {
				int type = getOctet(42);
				int id = getOctet(43);
				if (type == 1)
				{
				  return 0;
				}
				if (type == 2)
				{
				  return id;
				}
				if (type == 3)
				{
				  return 5 + id;
				}
			  }
				  goto case 98;
			  case 98:
				return getOctet(50);
			}
			return GribNumbers.UNDEFINED;
		  }
	  }

	  public long calcCRC()
	  {
		CRC32 crc32 = new CRC32();
		crc32.update(rawData);
		return crc32.getValue();
	  }

	}



}
