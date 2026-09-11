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
namespace NetcdfSharp.ucar.nc2.grib.grib2.table
{
	using TimeCoordIntvDateValue = NetcdfSharp.ucar.nc2.grib.coord.TimeCoordIntvDateValue;
	using Grib2Pds = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Pds;
	using Grib2Record = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Record;
	using Grib2Utils = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Utils;
	using CalendarDate = NetcdfSharp.ucar.nc2.time.CalendarDate;
	using CalendarPeriod = NetcdfSharp.ucar.nc2.time.CalendarPeriod;

	/// <summary>
	/// Center = (7) US National Weather Service, National Centres for Environmental Prediction (NCEP)
	/// SubCenter = (14) NWS Meteorological Development Laboratory
	/// Master Table = 1
	/// Local Table = 0
	/// 
	/// @author caron
	/// @since 1/28/2016.
	/// </summary>
	public class NwsMetDevTables : NcepLocalTables
	{

	  internal NwsMetDevTables(Grib2TableConfig grib2Table) : base(grib2Table)
	  {
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override @Nullable public NetcdfSharp.ucar.nc2.grib.coord.TimeCoordIntvDateValue getForecastTimeInterval(NetcdfSharp.ucar.nc2.grib.grib2.Grib2Record gr)
	  public override TimeCoordIntvDateValue getForecastTimeInterval(Grib2Record gr)
	  {
		Grib2Pds pds = gr.PDS;
		if (!pds.isTimeInterval())
		{
		  return null;
		}
		Grib2Pds.PdsInterval pdsIntv = (Grib2Pds.PdsInterval) gr.PDS;

		// override here only if timeRangeUnit = 255
		bool needOverride = false;
		foreach (Grib2Pds.TimeInterval ti in pdsIntv.TimeIntervals)
		{
		  needOverride = (ti.timeRangeUnit == 255);
		}
		if (!needOverride)
		{
		  return base.getForecastTimeInterval(gr);
		}

		CalendarDate intvEnd = pdsIntv.IntervalTimeEnd;
		int ftime = pdsIntv.ForecastTime;
		int timeUnitOrg = pds.TimeUnit;
		int timeUnitConvert = convertTimeUnit(timeUnitOrg);
		CalendarPeriod unitPeriod = Grib2Utils.getCalendarPeriod(timeUnitConvert);
		if (unitPeriod == null)
		{
		  throw new ArgumentException("unknown CalendarPeriod " + timeUnitConvert + " org=" + timeUnitOrg);
		}

		CalendarPeriod.Field fld = unitPeriod.getField();

		CalendarDate referenceDate = gr.ReferenceDate;
		CalendarDate intvStart = referenceDate.add(ftime, fld);

		return new TimeCoordIntvDateValue(intvStart, intvEnd);
	  }

	  /// <summary>
	  /// Only use in GribVariable to decide on variable identity when intvMerge = false.
	  /// By returning a constant, we dont intvMerge = false.
	  /// Problem is we cant reconstruct interval length without reference time, which is not in the pds.
	  /// </summary>
	  public override double getForecastTimeIntervalSizeInHours(Grib2Pds pds)
	  {
		Grib2Pds.PdsInterval pdsIntv = (Grib2Pds.PdsInterval) pds;

		// override here only if timeRangeUnit = 255
		bool needOverride = false;
		foreach (Grib2Pds.TimeInterval ti in pdsIntv.TimeIntervals)
		{
		  needOverride = (ti.timeRangeUnit == 255);
		}
		if (!needOverride)
		{
		  return base.getForecastTimeIntervalSizeInHours(pds);
		}


		return 12.0;
	  }

	}

}
