// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.grib2.table
{
	using TimeUnitConverter = NetcdfSharp.thredds.featurecollection.TimeUnitConverter;
	using GribNumbers = NetcdfSharp.ucar.nc2.grib.GribNumbers;
	using GribStatType = NetcdfSharp.ucar.nc2.grib.GribStatType;
	using GribTables = NetcdfSharp.ucar.nc2.grib.GribTables;
	using VariableIndex = NetcdfSharp.ucar.nc2.grib.collection.GribCollectionImmutable.VariableIndex;
	using TimeCoordIntvDateValue = NetcdfSharp.ucar.nc2.grib.coord.TimeCoordIntvDateValue;
	using TimeCoordIntvValue = NetcdfSharp.ucar.nc2.grib.coord.TimeCoordIntvValue;
	using VertCoordType = NetcdfSharp.ucar.nc2.grib.coord.VertCoordType;
	using Grib2Pds = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Pds;
	using Grib2Record = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Record;
	using Grib2SectionIdentification = NetcdfSharp.ucar.nc2.grib.grib2.Grib2SectionIdentification;
	using Grib2Utils = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Utils;
	using TableType = NetcdfSharp.ucar.nc2.grib.grib2.table.WmoCodeFlagTables.TableType;
	using WmoTable = NetcdfSharp.ucar.nc2.grib.grib2.table.WmoCodeFlagTables.WmoTable;
	using CalendarDate = NetcdfSharp.ucar.nc2.time.CalendarDate;
	using CalendarPeriod = NetcdfSharp.ucar.nc2.time.CalendarPeriod;
	using CommonCodeTable = NetcdfSharp.ucar.nc2.wmo.CommonCodeTable;
	using Parameter = NetcdfSharp.ucar.nc2.grib.GribTables.Parameter;

	/// <summary>
	/// Grib 2 Tables - allows local overrides and augmentation of WMO tables.
	/// This class serves the standard WMO tables, local tables are subclasses that override.
	/// Methods are placed here because they may be overridden by local Tables.
	/// 
	/// Tables include code, flag and parameter tables.
	/// 
	/// @author caron
	/// @since 4/3/11
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class Grib2Tables implements NetcdfSharp.ucar.nc2.grib.GribTables, NetcdfSharp.thredds.featurecollection.TimeUnitConverter
	public class Grib2Tables : GribTables, TimeUnitConverter
	{
	  private static readonly NetcdfSharp.org.slf4j.Logger logger = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(Grib2Tables));
	  private static readonly IDictionary<Grib2TablesId, Grib2Tables> tables = new Dictionary<Grib2TablesId, Grib2Tables>();
	  private static Grib2Tables wmoStandardTable;

	  public static Grib2Tables factory(Grib2Record gr)
	  {
		Grib2SectionIdentification ids = gr.Id;
		Grib2Pds pds = gr.PDS;
		return factory(ids.Center_id, ids.Subcenter_id, ids.Master_table_version, ids.Local_table_version, pds.GenProcessId);
	  }

	  // Lazy instantiation.
	  public static Grib2Tables factory(int center, int subCenter, int masterVersion, int localVersion, int genProcessId)
	  {
		Grib2TablesId id = new Grib2TablesId(center, subCenter, masterVersion, localVersion, genProcessId);
		Grib2Tables cust;
		if (tables.TryGetValue(id, out cust) && cust != null)
		{
		  return cust;
		}

		// note that we match on id, so same Grib2Customizer may be mapped to multiple id's (eg match on -1)
		Grib2TableConfig config = Grib2TableConfig.matchTable(id);
		cust = build(config);

		tables[id] = cust;
		return cust;
	  }

	  private static Grib2Tables build(Grib2TableConfig config)
	  {
		switch (config.Type)
		{
		  case NetcdfSharp.ucar.nc2.grib.grib2.table.Grib2TablesId.Type.cfsr:
			return new CfsrLocalTables(config);
		  case NetcdfSharp.ucar.nc2.grib.grib2.table.Grib2TablesId.Type.eccodes:
			return new EccodesLocalTables(config);
		  case NetcdfSharp.ucar.nc2.grib.grib2.table.Grib2TablesId.Type.gempak:
			return new GempakLocalTables(config); // LOOK: not used
		  case NetcdfSharp.ucar.nc2.grib.grib2.table.Grib2TablesId.Type.gsd:
			return new FslHrrrLocalTables(config);
		  case NetcdfSharp.ucar.nc2.grib.grib2.table.Grib2TablesId.Type.kma:
			return new KmaLocalTables(config);
		  case NetcdfSharp.ucar.nc2.grib.grib2.table.Grib2TablesId.Type.ncep:
			return new NcepLocalTables(config);
		  case NetcdfSharp.ucar.nc2.grib.grib2.table.Grib2TablesId.Type.ndfd:
			return new NdfdLocalTables(config);
		  case NetcdfSharp.ucar.nc2.grib.grib2.table.Grib2TablesId.Type.mrms:
			return new MrmsLocalTables(config);
		  case NetcdfSharp.ucar.nc2.grib.grib2.table.Grib2TablesId.Type.nwsDev:
			return new NwsMetDevTables(config);
		  default:
			if (wmoStandardTable == null)
			{
			  wmoStandardTable = new Grib2Tables(config);
			}
			return wmoStandardTable;
		}
	  }

	  public static int makeParamId(int discipline, int category, int number)
	  {
		return (discipline << 16) + (category << 8) + number;
	  }

	  public static int[] unmakeParamId(int code)
	  {
		int number = code & 255;
		code = code >> 8;
		int category = code & 255;
		int discipline = code >> 8;
		return new int[] {discipline, category, number};
	  }

	  public static string makeParamCode(int discipline, int category, int number)
	  {
		return string.Format("{0:D}-{1:D}-{2:D}", discipline, category, number);
	  }

	  public static string makeParamCode(int code)
	  {
		int number = code & 255;
		code = code >> 8;
		int category = code & 255;
		int discipline = code >> 8;
		return string.Format("{0:D}-{1:D}-{2:D}", discipline, category, number);
	  }

	  public static bool isLocal(Parameter p)
	  {
		return isLocal(p.Discipline, p.Category, p.Number);
	  }

	  public static bool isLocal(int discipline, int category, int number)
	  {
		return ((discipline <= 191) && (category <= 191) && (number <= 191));
	  }

	  public static bool isLocal(int code)
	  {
		int[] uncode = unmakeParamId(code);
		return isLocal(uncode[0], uncode[1], uncode[2]);
	  }

	  public static ImmutableList<Grib2Tables> AllRegisteredTables
	  {
		  get
		  {
			ImmutableList<Grib2Tables>.Builder builder = ImmutableList<Grib2Tables>.builder();
			foreach (Grib2TableConfig config in Grib2TableConfig.Tables)
			{
			  builder.add(build(config));
			}
			return builder.build();
		  }
	  }

	  ///////////////////////////////////////////////////////////////
	  protected internal readonly Grib2TableConfig config;

	  protected internal Grib2Tables(Grib2TableConfig config)
	  {
		this.config = config;
	  }

	  public virtual string Name
	  {
		  get
		  {
			return config.Name;
		  }
	  }

	  public virtual int CenterId
	  {
		  get
		  {
			return config.ConfigId.center;
		  }
	  }

	  public virtual string Path
	  {
		  get
		  {
			return config.Path;
		  }
	  }

	  public virtual Grib2TablesId ConfigId
	  {
		  get
		  {
			return config.ConfigId;
		  }
	  }

	  public virtual Grib2TablesId.Type Type
	  {
		  get
		  {
			return config.Type;
		  }
	  }

	  public virtual string getVariableName(Grib2Record gr)
	  {
		return getVariableName(gr.Discipline, gr.PDS.ParameterCategory, gr.PDS.ParameterNumber);
	  }

	  /// <summary>
	  /// Make a IOSP Variable name, using the Parameter name is available, otherwise a synthesized name.
	  /// </summary>
	  public virtual string getVariableName(int discipline, int category, int parameter)
	  {
		string s = WmoParamTable.getParameterName(discipline, category, parameter);
		if (string.ReferenceEquals(s, null))
		{
		  s = "U" + discipline + "-" + category + "-" + parameter;
		}
		return s;
	  }

	  /////////////////////////////////////////////////////////////////////////////////////
	  // Parameter interface (table 4.2.x)

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public NetcdfSharp.ucar.nc2.grib.GribTables.Parameter getParameter(NetcdfSharp.ucar.nc2.grib.grib2.Grib2Record gr)
	  public virtual GribTables.Parameter getParameter(Grib2Record gr)
	  {
		return getParameter(gr.Discipline, gr.PDS.ParameterCategory, gr.PDS.ParameterNumber);
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public NetcdfSharp.ucar.nc2.grib.GribTables.Parameter getParameter(NetcdfSharp.ucar.nc2.grib.collection.GribCollectionImmutable.VariableIndex vindex)
	  public virtual GribTables.Parameter getParameter(VariableIndex vindex)
	  {
		return getParameter(vindex.Discipline, vindex.Category, vindex.Parameter);
	  }

	  public virtual GribTables.Parameter getParameter(int discipline, Grib2Pds pds)
	  {
		return getParameter(discipline, pds.ParameterCategory, pds.ParameterNumber);
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable GribTables.Parameter getParameter(int discipline, int category, int number)
	  public virtual GribTables.Parameter getParameter(int discipline, int category, int number)
	  {
		return WmoParamTable.getParameter(discipline, category, number);
	  }

	  /////////////////////////////////////////////////////////////////////////////////////
	  // Code interface (tables other than 4.2.x)

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public String getCodeTableValue(String tableName, int code)
	  public virtual string getCodeTableValue(string tableName, int code)
	  {
		WmoCodeTable codeTable = WmoCodeFlagTables.Instance.getCodeTable(tableName);
		Grib2CodeTableInterface.Entry entry = (codeTable == null) ? null : codeTable.getEntry(code);
		return (entry == null) ? null : entry.Name;
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override @Nullable public String getSubCenterName(int center_id, int subcenter_id)
	  public virtual string getSubCenterName(int center_id, int subcenter_id)
	  {
		return CommonCodeTable.getSubCenterName(center_id, subcenter_id);
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public String getGeneratingProcessName(int genProcess)
	  public virtual string getGeneratingProcessName(int genProcess)
	  {
		return null;
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public String getGeneratingProcessTypeName(int genProcess)
	  public virtual string getGeneratingProcessTypeName(int genProcess)
	  {
		return getCodeTableValue("4.3", genProcess);
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public String getCategory(int discipline, int category)
	  public virtual string getCategory(int discipline, int category)
	  {
		WmoCodeTable catTable = WmoCodeFlagTables.Instance.getCodeTable("4.1." + discipline);
		Grib2CodeTableInterface.Entry entry = (catTable == null) ? null : catTable.getEntry(category);
		return (entry == null) ? null : entry.Name;
	  }

	  public virtual string getStatisticName(int id)
	  {
		string result = getCodeTableValue("4.10", id); // WMO
		if (string.ReferenceEquals(result, null))
		{
		  result = getStatisticNameShort(id);
		}
		return result;
	  }

	  public virtual string getStatisticNameShort(int id)
	  {
		GribStatType stat = GribStatType.getStatTypeFromGrib2(id);
		return (stat == null) ? "UnknownStatType-" + id : stat.ToString();
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override @Nullable public NetcdfSharp.ucar.nc2.grib.GribStatType getStatType(int grib2StatCode)
	  public virtual GribStatType getStatType(int grib2StatCode)
	  {
		return GribStatType.getStatTypeFromGrib2(grib2StatCode);
	  }

	  /*
	   * Code Table Code table 4.7 - Derived forecast (4.7)
	   * 0: Unweighted mean of all members
	   * 1: Weighted mean of all members
	   * 2: Standard deviation with respect to cluster mean
	   * 3: Standard deviation with respect to cluster mean, normalized
	   * 4: Spread of all members
	   * 5: Large anomaly index of all members
	   * 6: Unweighted mean of the cluster members
	   * 7: Interquartile range (range between the 25th and 75th quantile)
	   * 8: Minimum of all ensemble members
	   * 9: Maximum of all ensemble members
	   * -1: Reserved
	   * -1: Reserved for local use
	   * 255: Missing
	   */
	  public virtual string getProbabilityNameShort(int id)
	  {
		switch (id)
		{
		  case 0:
			return "unweightedMean";
		  case 1:
			return "weightedMean";
		  case 2:
			return "stdDev";
		  case 3:
			return "stdDevNormalized";
		  case 4:
			return "spread";
		  case 5:
			return "largeAnomalyIndex";
		  case 6:
			return "unweightedMeanCluster";
		  case 7:
			return "interquartileRange";
		  case 8:
			return "minimumEnsemble";
		  case 9:
			return "maximumEnsemble";
		  default:
			return "UnknownProbType" + id;
		}
	  }

	  ///////////////////////////////////////////////////////////////////////////////////////
	  // Vertical Units

	  /// <summary>
	  /// Unit of vertical coordinate.
	  /// from Grib2 code table 4.5.
	  /// Only levels with units get a dimension added
	  /// </summary>
	  /// <param name="code"> code from table 4.5 </param>
	  /// <returns> level unit, default is empty unit string </returns>
	  public virtual VertCoordType getVertUnit(int code)
	  {
		// VertCoordType(int code, String desc, String abbrev, String units, String datum, boolean isPositiveUp, boolean
		// isLayer)
		switch (code)
		{

		  case 11:
		  case 12:
		  case 117:
		  case 237:
		  case 238:
			return new VertCoordType(code, "m", null, true);

		  case 20:
			return new VertCoordType(code, "K", null, false);

		  case 100:
		  case 119:
			return new VertCoordType(code, "Pa", null, false);

		  case 102:
			return new VertCoordType(code, "m", "mean sea level", true);

		  case 103:
			return new VertCoordType(code, "m", "ground", true);

		  case 104:
		  case 105:
			return new VertCoordType(code, "sigma", null, false); // positive?

		  case 106:
			return new VertCoordType(code, "m", "land surface", false);

		  case 107:
			return new VertCoordType(code, "K", null, true); // positive?

		  case 108:
			return new VertCoordType(code, "Pa", "ground", true);

		  case 109:
			return new VertCoordType(code, "K m2 kg-1 s-1", null, true); // positive?

		  case 111:
			return new VertCoordType(code, "eta", null, false);

		  case 114:
			return new VertCoordType(code, "numeric", null, false); // ??

		  case 160:
			return new VertCoordType(code, "m", "sea level", false);

		  case 161:
			return new VertCoordType(code, "m", "water surface", false);

		  // LOOK NCEP specific
		  case 235:
			return new VertCoordType(code, "0.1 C", null, true);

		  default:
			return new VertCoordType(code, null, null, true);
		}
	  }

	  public virtual bool isLevelUsed(int code)
	  {
		VertCoordType vunit = getVertUnit(code);
		return vunit.VerticalCoordinate;
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public String getLevelName(int id)
	  public virtual string getLevelName(int id)
	  {
		return getCodeTableValue("4.5", id);
	  }

	  public virtual bool isLayer(Grib2Pds pds)
	  {
		return !(pds.LevelType2 == 255 || pds.LevelType2 == 0);
	  }

	  // Table 4.5
	  public virtual string getLevelNameShort(int id)
	  {

		switch (id)
		{
		  case 1:
			return "surface";
		  case 2:
			return "cloud_base";
		  case 3:
			return "cloud_tops";
		  case 4:
			return "zeroDegC_isotherm";
		  case 5:
			return "adiabatic_condensation_lifted";
		  case 6:
			return "maximum_wind";
		  case 7:
			return "tropopause";
		  case 8:
			return "atmosphere_top";
		  case 9:
			return "sea_bottom";
		  case 10:
			return "entire_atmosphere";
		  case 11:
			return "cumulonimbus_base";
		  case 12:
			return "cumulonimbus_top";
		  case 20:
			return "isotherm";
		  case 100:
			return "isobaric";
		  case 101:
			return "msl";
		  case 102:
			return "altitude_above_msl";
		  case 103:
			return "height_above_ground";
		  case 104:
			return "sigma";
		  case 105:
			return "hybrid";
		  case 106:
			return "depth_below_surface";
		  case 107:
			return "isentrope";
		  case 108:
			return "pressure_difference";
		  case 109:
			return "potential_vorticity_surface";
		  case 111:
			return "eta";
		  case 113:
			return "log_hybrid";
		  case 117:
			return "mixed_layer_depth";
		  case 118:
			return "hybrid_height";
		  case 119:
			return "hybrid_pressure";
		  case 120:
			return "pressure_thickness";
		  case 160:
			return "depth_below_sea";
		  case GribNumbers.UNDEFINED:
			return "none";
		  default:
			return "UnknownLevelType-" + id;
		}
	  }

	  ////////////////////////////////////////////////////////////////////////////////////////////
	  // Time utilities, generally Grib2 specific.

	  private TimeUnitConverter timeUnitConverter; // LOOK not really immutable

	  public virtual void setTimeUnitConverter(TimeUnitConverter timeUnitConverter)
	  {
		if (this.timeUnitConverter != null)
		{
		  throw new Exception("Cant modify timeUnitConverter once its been set");
		}
		this.timeUnitConverter = timeUnitConverter;
	  }

	  public virtual int convertTimeUnit(int timeUnit)
	  {
		if (timeUnitConverter == null)
		{
		  return timeUnit;
		}
		return timeUnitConverter.convertTimeUnit(timeUnit);
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public NetcdfSharp.ucar.nc2.time.CalendarDate getForecastDate(NetcdfSharp.ucar.nc2.grib.grib2.Grib2Record gr)
	  public virtual CalendarDate getForecastDate(Grib2Record gr)
	  {
		Grib2Pds pds = gr.PDS;
		if (pds.isTimeInterval())
		{
		  TimeCoordIntvDateValue intv = getForecastTimeInterval(gr);
		  return (intv == null) ? null : intv.End;

		}
		else
		{
		  int val = pds.ForecastTime;
		  CalendarPeriod period = Grib2Utils.getCalendarPeriod(pds.TimeUnit);
		  if (period == null)
		  {
			return null;
		  }
		  return gr.ReferenceDate.add(period.multiply(val));
		}
	  }

	  public virtual TimeCoordIntvDateValue getForecastTimeInterval(Grib2Record gr)
	  {
		if (!gr.PDS.isTimeInterval())
		{
		  return null;
		}
		Grib2Pds.PdsInterval pdsIntv = (Grib2Pds.PdsInterval) gr.PDS;

		// the time "range" in units of pdsIntv timeUnits
		TimeIntervalAndUnits intvu = getForecastTimeInterval(pdsIntv);

		// convert time "range" to units of pds timeUnits
		int timeUnitOrg = gr.PDS.TimeUnit;
		CalendarPeriod wantPeriod = Grib2Utils.getCalendarPeriod(timeUnitOrg);
		if (wantPeriod == null)
		{
		  return null;
		}
		CalendarPeriod havePeriod = Grib2Utils.getCalendarPeriod(intvu.timeUnitIntv);
		double fac2 = intvu.timeRange * wantPeriod.getConvertFactor(havePeriod);
		CalendarPeriod period = wantPeriod.multiply((int) fac2); // LOOK needs to be an int

		// note from Arthur Taylor (degrib):
		/*
		 * If there was a range I used:
		 *
		 * End of interval (EI) = (bytes 36-42 show an "end of overall time interval")
		 * C1) End of Interval = EI;
		 * Begin of Interval = EI - range
		 *
		 * and if there was no interval then I used:
		 * C2) End of Interval = Begin of Interval = Ref + ForeT.
		 */
		CalendarDate EI = pdsIntv.IntervalTimeEnd;
		if (EI == CalendarDate.UNKNOWN)
		{ // all values were set to zero LOOK guessing!
		  return new TimeCoordIntvDateValue(gr.ReferenceDate, period);
		}
		else
		{
		  return new TimeCoordIntvDateValue(period, EI);
		}
	  }

	  /// <summary>
	  /// Get interval size in units of hours.
	  /// Only use in GribVariable to decide on variable identity when intvMerge = false.
	  /// </summary>
	  /// <param name="pds"> must be a Grib2Pds.PdsInterval </param>
	  /// <returns> interval size in units of hours
	  ///         LOOK generalize this to return a size and unit? </returns>
	  public virtual double getForecastTimeIntervalSizeInHours(Grib2Pds pds)
	  {
		Grib2Pds.PdsInterval pdsIntv = (Grib2Pds.PdsInterval) pds;

		// calculate total "range"
		TimeIntervalAndUnits intvu = getForecastTimeInterval(pdsIntv);

		// convert that range to units of hours.
		CalendarPeriod timeUnitPeriod = Grib2Utils.getCalendarPeriod(intvu.timeUnitIntv);
		if (timeUnitPeriod == null)
		{
		  return GribNumbers.UNDEFINEDD;
		}
		if (timeUnitPeriod.Equals(CalendarPeriod.Hour))
		{
		  return intvu.timeRange;
		}

		// LOOK leave this until we fix getConvertFactor()
		double fac;
		if (timeUnitPeriod.getField() == CalendarPeriod.Field.Month)
		{
		  fac = 30.0 * 24.0; // nominal hours in a month
		}
		else if (timeUnitPeriod.getField() == CalendarPeriod.Field.Year)
		{
		  fac = 365.0 * 24.0; // nominal hours in a year
		}
		else
		{
		  fac = CalendarPeriod.Hour.getConvertFactor(timeUnitPeriod);
		}
		return fac * intvu.timeRange;
	  }

	  private class TimeIntervalAndUnits
	  {
		internal int timeUnitIntv;
		internal int timeRange;

		internal TimeIntervalAndUnits(int timeUnitIntv, int timeRange)
		{
		  this.timeUnitIntv = timeUnitIntv;
		  this.timeRange = timeRange;
		}
	  }

	  private TimeIntervalAndUnits getForecastTimeInterval(Grib2Pds.PdsInterval pdsIntv)
	  {
		int range = 0;
		int timeUnitIntv = -999;
		foreach (Grib2Pds.TimeInterval ti in pdsIntv.TimeIntervals)
		{
		  if (ti.timeRangeUnit == 255)
		  {
			continue;
		  }
		  if (timeUnitIntv < 0)
		  {
			timeUnitIntv = ti.timeRangeUnit;
		  }
		  else if ((ti.timeRangeUnit != timeUnitIntv) // make sure it doesn't change
			  || (ti.timeIncrementUnit != timeUnitIntv && ti.timeIncrementUnit != 255 && ti.timeIncrement != 0))
		  { // LOOK
			// WTF?
			logger.warn("TimeInterval(2) has different units timeUnit first=" + timeUnitIntv + " TimeInterval=" + ti.timeIncrementUnit);
			throw new Exception("TimeInterval(2) has different units");
		  }

		  range += ti.timeRangeLength;
		  if (ti.timeIncrementUnit != 255)
		  {
			range += ti.timeIncrement;
		  }
		}
		if (timeUnitIntv < 0)
		{
		  range = 0;
		  timeUnitIntv = 1; /* hours */
		}
		return new TimeIntervalAndUnits(timeUnitIntv, range);
	  }

	  /// <summary>
	  /// If this has a time interval coordinate, get time interval
	  /// </summary>
	  /// <param name="gr"> from this record </param>
	  /// <returns> time interval in units of pds.getTimeUnit(), or null if not a time interval </returns>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public int[] getForecastTimeIntervalOffset(Grib2Record gr)
	  public virtual int[] getForecastTimeIntervalOffset(Grib2Record gr)
	  {
		TimeCoordIntvDateValue tinvd = getForecastTimeInterval(gr);
		if (tinvd == null)
		{
		  return null;
		}

		Grib2Pds pds = gr.PDS;
		int unit = pds.TimeUnit;
		TimeCoordIntvValue tinv = tinvd.convertReferenceDate(gr.ReferenceDate, Grib2Utils.getCalendarPeriod(unit));
		if (tinv == null)
		{
		  return null;
		}
		int[] result = new int[2];
		result[0] = tinv.Bounds1;
		result[1] = tinv.Bounds2;
		return result;
	  }

	  /////////////////////////////////////////////////////
	  // debugging

	  /// <summary>
	  /// Get the unprocessed parameter provided by this Grib2Table.
	  /// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public NetcdfSharp.ucar.nc2.grib.GribTables.Parameter getParameterRaw(int discipline, int category, int number)
	  public virtual GribTables.Parameter getParameterRaw(int discipline, int category, int number)
	  {
		return WmoParamTable.getParameter(discipline, category, number);
	  }

	  /// <summary>
	  /// Get the name of the parameter table that is being used for this parameter.
	  /// </summary>
	  public virtual string getParamTablePathUsedFor(int discipline, int category, int number)
	  {
		return WmoCodeFlagTables.standard.ResourceName;
	  }

	  /// <summary>
	  /// Get the list of parameters provided by this Grib2Table.
	  /// </summary>
	  public virtual ImmutableList<GribTables.Parameter> Parameters
	  {
		  get
		  {
			ImmutableList<GribTables.Parameter>.Builder allParams = ImmutableList<GribTables.Parameter>.builder();
			foreach (WmoTable wmoTable in WmoCodeFlagTables.Instance.WmoTables)
			{
			  if (wmoTable.Type == TableType.param)
			  {
				WmoParamTable paramTable = new WmoParamTable(wmoTable);
				allParams.addAll(paramTable.Parameters);
			  }
			}
			return allParams.build();
		  }
	  }

	  public virtual void lookForProblems(Formatter f)
	  {
	  }

	  public virtual void showDetails(Formatter f)
	  {
	  }

	  public virtual void showEntryDetails(Formatter f, IList<GribTables.Parameter> @params)
	  {
	  }

	  public virtual void showSpecialPdsInfo(Grib2Record pds, Formatter f)
	  {
	  }
	}

}
