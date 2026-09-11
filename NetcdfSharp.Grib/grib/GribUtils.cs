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

namespace NetcdfSharp.ucar.nc2.grib
{
	using MFile = NetcdfSharp.thredds.inventory.MFile;
	using MFiles = NetcdfSharp.thredds.inventory.MFiles;
	using CalendarDate = NetcdfSharp.ucar.nc2.time.CalendarDate;
	using CalendarPeriod = NetcdfSharp.ucar.nc2.time.CalendarPeriod;
	using StringUtil2 = NetcdfSharp.ucar.unidata.util.StringUtil2;

	/// <summary>
	/// General Utilities used by GRIB code.
	/// 
	/// @author caron
	/// @since 11/16/11
	/// </summary>
	public class GribUtils
	{
	  public const string CENTER = "Originating_or_generating_Center";
	  public const string SUBCENTER = "Originating_or_generating_Subcenter";
	  public const string GEN_PROCESS = "Generating_process_or_model";
	  public const string TABLE_VERSION = "GRIB_table_version";

	  /// <summary>
	  /// Convert a time unit to a CalendarPeriod
	  /// GRIB1 and GRIB2 are the same (!)
	  /// </summary>
	  /// <param name="timeUnit"> (GRIB1 table 4) (GRIB2 Code table 4.4 : Indicator of unit of time range) </param>
	  /// <returns> equivalent CalendarPeriod </returns>
	  public static CalendarPeriod getCalendarPeriod(int timeUnit)
	  {
		// LOOK - some way to intern these ? put in hash table ?
		switch (timeUnit)
		{ // code table 4.4
		  case 0:
			return CalendarPeriod.of(1, CalendarPeriod.Field.Minute);
		  case 1:
			return CalendarPeriod.of(1, CalendarPeriod.Field.Hour);
		  case 2:
			return CalendarPeriod.of(1, CalendarPeriod.Field.Day);
		  case 3:
			return CalendarPeriod.of(1, CalendarPeriod.Field.Month);
		  case 4:
			return CalendarPeriod.of(1, CalendarPeriod.Field.Year);
		  case 5:
			return CalendarPeriod.of(10, CalendarPeriod.Field.Year);
		  case 6:
			return CalendarPeriod.of(30, CalendarPeriod.Field.Year);
		  case 7:
			return CalendarPeriod.of(100, CalendarPeriod.Field.Year);
		  case 10:
			return CalendarPeriod.of(3, CalendarPeriod.Field.Hour);
		  case 11:
			return CalendarPeriod.of(6, CalendarPeriod.Field.Hour);
		  case 12:
			return CalendarPeriod.of(12, CalendarPeriod.Field.Hour);
		  case 13:
			return CalendarPeriod.of(15, CalendarPeriod.Field.Minute);
		  case 14:
			return CalendarPeriod.of(30, CalendarPeriod.Field.Minute);
		  default:
			throw new System.NotSupportedException("Unknown time unit = " + timeUnit);
		}
	  }

	  public static CalendarDate getValidTime(CalendarDate refDate, int timeUnit, int offset)
	  {
		CalendarPeriod period = GribUtils.getCalendarPeriod(timeUnit);
		return refDate.add(period.multiply(offset));
	  }

	  public static string cleanupUnits(string unit)
	  {
		if (string.ReferenceEquals(unit, null))
		{
		  return "";
		}
		if (unit.Equals("-", StringComparison.OrdinalIgnoreCase))
		{
		  unit = "";
		}
		else
		{
		  if (unit.StartsWith("/", StringComparison.Ordinal))
		  {
			unit = "1" + unit;
		  }
		  unit = unit.Trim();
		  unit = StringUtil2.remove(unit, "**");
		  StringBuilder sb = new StringBuilder(unit);
		  StringUtil2.removeAll(sb, "^[]");
		  StringUtil2.substitute(sb, " / ", "/");
		  StringUtil2.replace(sb, ' ', ".");
		  StringUtil2.replace(sb, '*', ".");
		  unit = sb.ToString();
		}
		return unit;
	  }

	  public static string cleanupDescription(string desc)
	  {
		if (string.ReferenceEquals(desc, null))
		{
		  return "";
		}
		int pos = desc.IndexOf("(see", StringComparison.Ordinal);
		if (pos < 0)
		{
		  pos = desc.IndexOf("(See", StringComparison.Ordinal);
		}
		if (pos > 0)
		{
		  desc = desc.Substring(0, pos);
		}

		StringBuilder sb = new StringBuilder(desc.Trim());
		StringUtil2.replace(sb, '+', "and");
		StringUtil2.removeAll(sb, ".;,=[]()/");
		return sb.ToString().Trim();
	  }

	  public static string makeNameFromDescription(string desc)
	  {
		if (string.ReferenceEquals(desc, null))
		{
		  return "";
		}
		int pos = desc.IndexOf("(see", StringComparison.Ordinal);
		if (pos < 0)
		{
		  pos = desc.IndexOf("(See", StringComparison.Ordinal);
		}
		if (pos > 0)
		{
		  desc = desc.Substring(0, pos);
		}

		StringBuilder sb = new StringBuilder(desc.Trim());
		StringUtil2.replace(sb, '+', "plus");
		StringUtil2.replace(sb, "/. ", "-p_");
		StringUtil2.removeAll(sb, ";,=[]()");
		return sb.ToString();
	  }

	  /*
	   * scanMode
	   * Grib1
	   * Flag/Code table 8 – Scanning mode
	   * Bit No. Value Meaning
	   * 1 0 Points scan in +i direction
	   * 1 Points scan in –i direction
	   * 2 0 Points scan in –j direction
	   * 1 Points scan in +j direction
	   * 3 0 Adjacent points in i direction are consecutive
	   * 1 Adjacent points in j direction are consecutive
	   * Notes:
	   * (1) i direction: west to east along a parallel, or left to right along an X-axis.
	   * (2) j direction: south to north along a meridian, or bottom to top along a Y-axis.
	   * 
	   * Grin2
	   * Flag table 3.4 – Scanning mode
	   * Bit No. Value Meaning
	   * 128 1 0 Points of first row or column scan in the +i (+x) direction
	   * 1 Points of first row or column scan in the –i (–x) direction
	   * 64 2 0 Points of first row or column scan in the –j (–y) direction
	   * 1 Points of first row or column scan in the +j (+y) direction
	   * 32 3 0 Adjacent points in i (x) direction are consecutive
	   * 1 Adjacent points in j (y) direction is consecutive
	   * 16 4 0 All rows scan in the same direction
	   * 1 Adjacent rows scans in the opposite direction
	   * 5–8 Reserved
	   * Notes:
	   * (1) i direction: west to east along a parallel or left to right along an x-axis.
	   * (2) j direction: south to north along a meridian, or bottom to top along a y-axis.
	   * (3) If bit number 4 is set, the first row scan is as defined by previous flags.
	   */

	  /// <summary>
	  /// X Points scan in +/- direction. Grib 1 or 2.
	  /// Positive means west to east along a parallel, or left to right along an X-axis..
	  /// </summary>
	  /// <param name="scanMode"> scanMode byte </param>
	  /// <returns> true: x points scan in positive direction, false: x points scan in negetive direction </returns>
	  public static bool scanModeXisPositive(int scanMode)
	  {
		return !GribNumbers.testGribBitIsSet(scanMode, 1);
	  }

	  /// <summary>
	  /// Y Points scan in +/- direction. Grib 1 or 2.
	  /// Positive means south to north along a meridian, or bottom to top along a Y-axis.
	  /// </summary>
	  /// <param name="scanMode"> scanMode byte </param>
	  /// <returns> true: y points scan in positive direction, false: y points scan in negetive direction </returns>
	  public static bool scanModeYisPositive(int scanMode)
	  {
		return GribNumbers.testGribBitIsSet(scanMode, 2);
	  }

	  /// <summary>
	  /// Adjacent points in x/y direction are consecutive. Grib 1 or 2.
	  /// </summary>
	  /// <param name="scanMode"> scanMode byte </param>
	  /// <returns> true: x points are consecutive (row) false: y points are consecutive (col) </returns>
	  public static bool scanModeXisConsecutive(int scanMode)
	  {
		return !GribNumbers.testGribBitIsSet(scanMode, 3);
	  }

	  /// <summary>
	  /// All rows scan in the same/opposite direction. Grib 2 only.
	  /// </summary>
	  /// <param name="scanMode"> scanMode byte </param>
	  /// <returns> true: All rows scan in the same direction, false: Adjacent rows scan in the opposite direction, the first
	  ///         row scan is as defined by previous flags </returns>
	  public static bool scanModeSameDirection(int scanMode)
	  {
		return !GribNumbers.testGribBitIsSet(scanMode, 4);
	  }

	  /// <summary>
	  /// Make an index filename with the given suffix from the grib filename.
	  /// 
	  /// Handles special cases where the grib filename is not a file path.
	  /// For example,
	  /// cdms3:thredds-test-data?test-grib-without-index/cosmo-eu.grib2#delimiter=/
	  /// 
	  /// becomes
	  /// cdms3:thredds-test-data?test-grib-without-index/cosmo-eu.grib2.gbx9#delimiter=/
	  /// </summary>
	  /// <param name="filename"> </param>
	  /// <returns> the index filename </returns>
	  public static string makeIndexFileName(string filename, string idxNameSuffix)
	  {
		string idxPath = filename;
		if (!filename.EndsWith(idxNameSuffix, StringComparison.Ordinal))
		{
		  MFile testIdxMFile = MFiles.create(filename);
		  string mfileName = testIdxMFile.Name;
		  string idxName = testIdxMFile.Name + idxNameSuffix;
		  int i = idxPath.LastIndexOf(mfileName, StringComparison.Ordinal);
					var tempVar = i + mfileName.Length;
		  idxPath = idxPath.Substring(0, i) + idxName + idxPath.Substring(tempVar, filename.Length - (tempVar));
		}
		return idxPath;
	  }
	}

}
