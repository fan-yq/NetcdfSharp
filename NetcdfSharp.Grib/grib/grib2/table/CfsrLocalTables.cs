// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.grib2.table
{
	using TimeCoordIntvDateValue = NetcdfSharp.ucar.nc2.grib.coord.TimeCoordIntvDateValue;
	using Grib2Parameter = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Parameter;
	using Grib2Pds = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Pds;
	using Grib2Record = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Record;
	using Grib2Utils = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Utils;
	using CalendarDate = NetcdfSharp.ucar.nc2.time.CalendarDate;
	using CalendarPeriod = NetcdfSharp.ucar.nc2.time.CalendarPeriod;
	using StringUtil2 = NetcdfSharp.ucar.unidata.util.StringUtil2;
	using Parameter = NetcdfSharp.ucar.nc2.grib.GribTables.Parameter;

	/// <summary>
	/// Read screen scraped DSS tables. CFSR 093 not needed - screen scraped NCEP ok 2/2/2012
	/// 
	/// @author caron
	/// @since 11/3/11
	/// </summary>
	internal class CfsrLocalTables : NcepLocalTables
	{
	  internal CfsrLocalTables(Grib2TableConfig config) : base(config)
	  {
		initLocalTable();
	  }

	  public override ImmutableList<Parameter> Parameters
	  {
		  get
		  {
			return LocalParameters;
		  }
	  }

	  public override string getParamTablePathUsedFor(int discipline, int category, int number)
	  {
		if ((category <= 191) && (number <= 191))
		{
		  return base.getParamTablePathUsedFor(discipline, category, number);
		}
		return config.Path;
	  }

	  public override string getGeneratingProcessName(int genProcess)
	  {
		if (genProcess == 197)
		{
		  return "CFSR";
		}
		return base.getGeneratingProcessName(genProcess);
	  }

	  // 193
	  // Average of N forecasts (or initialized analyses); each product has forecast period of P1 (P1=0 for initialized
	  // analyses);
	  // products have reference times at intervals of P2, beginning at the given reference time.
	  // 194
	  // Average of N uninitialized analyses, starting at reference time, at intervals of P2.
	  // 195:
	  // Average of forecast accumulations. P1 = start of accumulation period. P2 = end of accumulation period.
	  // Reference time is the start time of the first forecast, other forecasts at 24-hour intervals.
	  // 204
	  // Average of forecast accumulations. P1 = start of accumulation period. P2 = end of accumulation period.
	  // Reference time is the start time of the first forecast, other forecasts at 6-hour intervals.
	  // Number in Ave = number of forecast used
	  // 205
	  // Average of forecast averages. P1 = start of averaging period. P2 = end of averaging period.
	  // Reference time is the start time of the first forecast, other forecasts at 6-hour intervals. Number in Ave = number
	  // of forecast used

	  public override int[] getForecastTimeIntervalOffset(Grib2Record gr)
	  {
		Grib2Pds pds = gr.PDS;
		if (!pds.isTimeInterval())
		{
		  return null;
		}

		// LOOK this is hack for CFSR monthly combobulation
		// see http://rda.ucar.edu/datasets/ds093.2/#docs/time_ranges.html

		int statType = pds.getOctet(47);
		int n = pds.getInt4StartingAtOctet(50);
		int p2 = pds.getInt4StartingAtOctet(55);
		int p2mp1 = pds.getInt4StartingAtOctet(62);

		int p1 = p2 - p2mp1;
		int start, end;

		switch (statType)
		{
		  case 193:
			start = p1;
			end = p1 + n * p2;
			break;
		  case 194:
			start = 0;
			end = n * p2;
			break;
		  case 195:
		  case 204:
		  case 205:
			start = p1;
			end = p2;
			break;
		  default:
			throw new ArgumentException("unknown statType " + statType);
		}

		return new int[] {start, end};
	  }

	  public override TimeCoordIntvDateValue getForecastTimeInterval(Grib2Record gr)
	  {
		Grib2Pds pds = gr.PDS;
		if (!pds.isTimeInterval())
		{
		  return null;
		}

		int[] intv = getForecastTimeIntervalOffset(gr);
		Debug.Assert(intv != null);
		int intvLen = intv[1] - intv[0];

		int timeUnitOrg = pds.TimeUnit;
		int timeUnitConvert = convertTimeUnit(timeUnitOrg);
		CalendarPeriod unitPeriod = Grib2Utils.getCalendarPeriod(timeUnitConvert);
		if (unitPeriod == null)
		{
		  throw new ArgumentException("unknown CalendarPeriod " + timeUnitConvert + " org=" + timeUnitOrg);
		}

		CalendarPeriod.Field fld = unitPeriod.getField();

		CalendarDate start = gr.ReferenceDate.add(intv[0], fld);
		CalendarPeriod period = CalendarPeriod.of(intvLen, fld);

		return new TimeCoordIntvDateValue(start, period);
	  }

	  /// <summary>
	  /// Only use in GribVariable to decide on variable identity when intvMerge = false. By returning a
	  /// constant, we dont support intvMerge = false. Problem is we cant reconstruct interval length
	  /// without reference time, which is not in the pds.
	  /// </summary>
	  public override double getForecastTimeIntervalSizeInHours(Grib2Pds pds)
	  {
		return 6.0;
	  }

	  private bool isCfsr2(Grib2Pds pds)
	  {
		int genType = pds.GenProcessId;
		if ((genType != 82) && (genType != 89))
		{
		  return false;
		}

		Grib2Pds.PdsInterval pdsIntv = (Grib2Pds.PdsInterval) pds;
		Grib2Pds.TimeInterval[] ti = pdsIntv.TimeIntervals;
		return ti.Length != 1;
	  }


	  public override void showSpecialPdsInfo(Grib2Record gr, Formatter f)
	  {
		Grib2Pds pds = gr.PDS;
		if (!pds.isTimeInterval())
		{
		  return;
		}
		if (pds.RawLength < 65)
		{
		  return;
		}

		/*
		 * Octet(s) Description
		 * 47 From NCEP Code Table 4.10
		 * 48 Should be ignored
		 * 49 Should be ignored
		 * 50-53 Number of grids used in the average
		 * 54 Should be ignored
		 * 55-58 This is "P2" from the GRIB1 format
		 * 59 From NCEP Code Table 4.10
		 * 60 Should be ignored
		 * 61 Should be ignored
		 * 62-65 This is "P2 minus P1"; P1 and P2 are fields from the GRIB1 format
		 * 66 Should be ignored
		 * 67-70 Should be ignored
		 */

		int statType = pds.getOctet(47);
		int statType2 = pds.getOctet(59);
		int ngrids = pds.getInt4StartingAtOctet(50);
		int p2 = pds.getInt4StartingAtOctet(55);
		int p2mp1 = pds.getInt4StartingAtOctet(62);

		f.format("%nCFSR MM special encoding (NCAR)%n");
		f.format("  (47) Code Table 4.10 = %d%n", statType);
		f.format("  (50-53) N in avg     = %d%n", ngrids);
		f.format("  (55-58) Grib1 P2     = %d%n", p2);
		f.format("  (59) Code Table 4.10 = %d%n", statType2);
		f.format("  (62-65) P2 minus P1  = %d%n", p2mp1);
		f.format("                   P1  = %d%n", p2 - p2mp1);

		int[] intv = getForecastTimeIntervalOffset(gr);
		if (intv == null)
		{
		  return;
		}
		f.format("ForecastTimeIntervalOffset  = (%d,%d)%n", intv[0], intv[1]);
		f.format("      ForecastTimeInterval  = %s%n", getForecastTimeInterval(gr));

		/*
		 * Section 4 Octet 58 (possibly 32 bits: 55-58) is the length of the averaging period per unit.
		 * For cycle fractions, this is 24, for complete monthly averages, it is 6.
		 * The product of this and the num_in_avg {above} should always equal the total number of hours in a respective
		 * month.
		 * - Section 4 Octet 65 is the hours skipped between each calculation component.
		 * f.format("%nCFSR MM special encoding (Swank)%n");
		 * f.format("  (55-58) length of avg period per unit                     = %d%n", p2);
		 * f.format("  (62-65) hours skipped between each calculation component  = %d%n", p2mp1);
		 * f.format("  nhours in month %d should be  = %d%n", ngrids * p2, 24 * 31);
		 */
	  }

	  //////////////////////////////////////////////////////////////////////

	  // see http://www.nco.ncep.noaa.gov/pmb/docs/grib2/grib2_doc.shtml
	  private void initLocalTable()
	  {
		string tableName = config.Path;
		try
		{
				using (Stream @is = NetcdfSharp.ucar.nc2.grib.GribResourceReader.getInputStream(tableName))
				{
			  if (@is == null)
			  {
				throw new System.InvalidOperationException("Cant find " + tableName);
			  }
			  using (StreamReader br = new StreamReader(@is, Encoding.UTF8))
			  {
				while (true)
				{
				  string line = br.ReadLine();
				  if (string.ReferenceEquals(line, null))
				  {
					break;
				  }
				  if ((line.Length == 0) || line.StartsWith("#", StringComparison.Ordinal))
				  {
					continue;
				  }
				  string[] flds = StringUtil2.splitString(line);

				  int p1 = int.Parse(flds[0].Trim()); // must have a number
				  int p2 = int.Parse(flds[1].Trim()); // must have a number
				  int p3 = int.Parse(flds[2].Trim()); // must have a number
				  StringBuilder b = new StringBuilder();
				  int count = 3;

				  while (count < flds.Length && !flds[count].Equals("."))
				  {
					b.Append(flds[count++]).Append(' ');
				  }
				  string abbrev = b.ToString().Trim();
				  b.Length = 0;
				  count++;

				  while (count < flds.Length && !flds[count].Equals("."))
				  {
					b.Append(flds[count++]).Append(' ');
				  }
				  string name = b.ToString().Trim();
				  b.Length = 0;
				  count++;

				  while (count < flds.Length && !flds[count].Equals("."))
				  {
					b.Append(flds[count++]).Append(' ');
				  }
				  string unit = b.ToString().Trim();

				  Grib2Parameter s = new Grib2Parameter(p1, p2, p3, name, unit, abbrev, null);
				  localParams[makeParamId(p1, p2, p3)] = s;
				}
			  }

				}
		}
		catch (IOException ioe)
		{
		  throw new Exception(ioe.Message, ioe);
		}
	  }
	}

}
