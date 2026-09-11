// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System.Collections.Generic;
using System.Linq;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.grib.coord
{
	using GribUtils = NetcdfSharp.ucar.nc2.grib.GribUtils;
	using Grib1ParamTime = NetcdfSharp.ucar.nc2.grib.grib1.Grib1ParamTime;
	using Grib1Record = NetcdfSharp.ucar.nc2.grib.grib1.Grib1Record;
	using Grib1SectionProductDefinition = NetcdfSharp.ucar.nc2.grib.grib1.Grib1SectionProductDefinition;
	using Grib1Customizer = NetcdfSharp.ucar.nc2.grib.grib1.tables.Grib1Customizer;
	using Grib2Pds = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Pds;
	using Grib2Record = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Record;
	using Grib2Tables = NetcdfSharp.ucar.nc2.grib.grib2.table.Grib2Tables;
	using CalendarDate = NetcdfSharp.ucar.nc2.time.CalendarDate;
	using CalendarDateRange = NetcdfSharp.ucar.nc2.time.CalendarDateRange;
	using CalendarDateUnit = NetcdfSharp.ucar.nc2.time.CalendarDateUnit;
	using CalendarPeriod = NetcdfSharp.ucar.nc2.time.CalendarPeriod;
	using Counters = NetcdfSharp.ucar.nc2.util.Counters;
	using Indent = NetcdfSharp.ucar.nc2.util.Indent;
	using Misc = NetcdfSharp.ucar.nc2.util.Misc;

	/// <summary>
	/// Time coordinates that are intervals.
	/// 
	/// @author John
	/// @since 11/28/13
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class CoordinateTimeIntv extends CoordinateTimeAbstract implements Coordinate
	public class CoordinateTimeIntv : CoordinateTimeAbstract, Coordinate
	{
	  private readonly IList<TimeCoordIntvValue> timeIntervals;

	  public CoordinateTimeIntv(int code, CalendarPeriod timeUnit, CalendarDate refDate, IList<TimeCoordIntvValue> timeIntervals, int[] time2runtime) : base(code, timeUnit, refDate, time2runtime)
	  {
		this.timeIntervals = timeIntervals.AsReadOnly();
	  }

	  internal CoordinateTimeIntv(CoordinateTimeIntv org, CalendarDate refDate) : base(org.code, org.timeUnit, refDate, null)
	  {
		this.timeIntervals = org.TimeIntervals;
	  }

	  public virtual IList<TimeCoordIntvValue> TimeIntervals
	  {
		  get
		  {
			return timeIntervals;
		  }
	  }

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public NetcdfSharp.java.util.List<?> getValues()
	  public override IList<object> Values
	  {
		  get
		  {
			return timeIntervals.Cast<object>().ToList();
		  }
	  }

	  public override object getValue(int idx)
	  {
		return timeIntervals[idx];
	  }

	  public override int getIndex(object val)
	  {
		return Collections.binarySearch(timeIntervals, (TimeCoordIntvValue) val);
	  }

	  public override int Size
	  {
		  get
		  {
			return timeIntervals.Count;
		  }
	  }

	  public override int estMemorySize()
	  {
		return 616 + Size * (24); // LOOK wrong
	  }

	  public override Coordinate.Type getType()
	  {
		return NetcdfSharp.ucar.nc2.grib.coord.Coordinate.Type.timeIntv;
	  }

	  /// <summary>
	  /// Check if we all time intervals have the same length.
	  /// </summary>
	  /// <returns> time interval name or MIXED_INTERVALS </returns>
	  public virtual string TimeIntervalName
	  {
		  get
		  {
			// are they the same length ?
			int firstValue = -1;
			foreach (TimeCoordIntvValue tinv in timeIntervals)
			{
			  int value = (tinv.Bounds2 - tinv.Bounds1);
			  if (firstValue < 0)
			  {
				firstValue = value;
			  }
			  else if (value != firstValue)
			  {
				return MIXED_INTERVALS;
			  }
			}

			firstValue = (firstValue * timeUnit.Value);
			return firstValue + "_" + timeUnit.FieldProp;
		  }
	  }

	  /// <summary>
	  /// Make calendar date range, using the first and last ending bounds
	  /// </summary>
	  /// <param name="cal"> optional calendar, may be null </param>
	  /// <returns> calendar date range </returns>
	  public override CalendarDateRange makeCalendarDateRange(NetcdfSharp.ucar.nc2.time.Calendar cal)
	  {
		CalendarDateUnit cdu = CalendarDateUnit.of(cal, timeUnit.FieldProp, refDate);
		CalendarDate start = cdu.makeCalendarDate(timeUnit.Value * timeIntervals[0].Bounds2);
		CalendarDate end = cdu.makeCalendarDate(timeUnit.Value * timeIntervals[Size - 1].Bounds2);
		return CalendarDateRange.of(start, end);
	  }

	  public override void showInfo(Formatter info, Indent indent)
	  {
		info.format("%s%s:", indent, getType());
		foreach (TimeCoordIntvValue cd in timeIntervals)
		{
		  info.format(" %s,", cd);
		}
		info.format(" (%d) %n", timeIntervals.Count);
		if (time2runtime != null)
		{
		  info.format("%stime2runtime: %s", indent, "[" + string.Join(", ", time2runtime) + "]");
		}
	  }

	  public override void showCoords(Formatter info)
	  {
		info.format("Time Interval offsets: (%s) ref=%s%n", TimeUnit, RefDate);
		foreach (TimeCoordIntvValue cd in timeIntervals)
		{
		  info.format("   (%3d - %3d)  %d%n", cd.Bounds1, cd.Bounds2, cd.Bounds2 - cd.Bounds1);
		}
	  }

	  public override Counters calcDistributions()
	  {
		Counters counters = new Counters();
		counters.add("resol");
		counters.add("intv");

		IList<TimeCoordIntvValue> offsets = TimeIntervals;
		for (int i = 0; i < offsets.Count; i++)
		{
		  int intv = offsets[i].Bounds2 - offsets[i].Bounds1;
		  counters.count("intv", intv);
		  if (i < offsets.Count - 1)
		  {
			int resol = offsets[i + 1].Bounds1 - offsets[i].Bounds1;
			counters.count("resol", resol);
		  }
		}

		return counters;
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

		CoordinateTimeIntv that = (CoordinateTimeIntv) o;

		if (code != that.code)
		{
		  return false;
		}
// JAVA TO C# CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
// ORIGINAL LINE: return timeIntervals.equals(that.timeIntervals);
		return timeIntervals.SequenceEqual(that.timeIntervals);

	  }

	  public override int GetHashCode()
	  {
		int result = code;
		result = 31 * result + timeIntervals.GetHashCode();
		return result;
	  }

	  protected internal override CoordinateTimeIntv makeBestFromComplete(int[] best, int n)
	  {
		IList<TimeCoordIntvValue> timeIntervalsBest = new List<TimeCoordIntvValue>(timeIntervals.Count);
		int[] time2runtimeBest = new int[n];
		int count = 0;
		for (int i = 0; i < best.Length; i++)
		{
		  int time = best[i];
		  if (time >= 0)
		  {
			time2runtimeBest[count] = time;
			timeIntervalsBest.Add(timeIntervals[i]);
			count++;
		  }
		}

		return new CoordinateTimeIntv(code, timeUnit, refDate, timeIntervalsBest, time2runtimeBest);
	  }

	  ///////////////////////////////////////////////////////////

	  public class Builder2 : CoordinateBuilderImpl<Grib2Record>
	  {
		internal readonly Grib2Tables cust;
		internal readonly int code; // pdsFirst.getTimeUnit()
		internal readonly CalendarPeriod timeUnit;
		internal readonly CalendarDate refDate;

		public Builder2(Grib2Tables cust, int code, CalendarPeriod timeUnit, CalendarDate refDate)
		{
		  this.cust = cust;
		  this.code = code;
		  this.timeUnit = timeUnit;
		  this.refDate = refDate;
		}

		public Builder2(CoordinateTimeIntv from)
		{
		  this.cust = null;
		  this.code = from.Code;
		  this.timeUnit = from.TimeUnit;
		  this.refDate = from.RefDate;
		}

		public override object extract(Grib2Record gr)
		{
		  TimeCoordIntvValue tinv;

		  Grib2Pds pds = gr.PDS;
		  int tuInRecord = pds.TimeUnit;
		  if (tuInRecord == code)
		  {
			int[] intv = cust.getForecastTimeIntervalOffset(gr);
			if (intv == null)
			{
			  throw new System.InvalidOperationException("CoordinateTimeIntv must have TimeIntervalOffset");
			}
			tinv = new TimeCoordIntvValue(intv[0], intv[1]);

		  }
		  else
		  {
			TimeCoordIntvDateValue tinvd = cust.getForecastTimeInterval(gr); // converts to calendar date
			if (tinvd == null)
			{
			  throw new System.InvalidOperationException("CoordinateTimeIntv has no ForecastTime");
			}
			tinv = tinvd.convertReferenceDate(refDate, timeUnit);
			if (tinv == null)
			{
			  throw new System.InvalidOperationException("CoordinateTimeIntv has no ReferenceTime");
			}
		  }

		  return tinv;
		}

		public override Coordinate makeCoordinate(IList<object> values)
		{
		  IList<TimeCoordIntvValue> offsetSorted = new List<TimeCoordIntvValue>(values.Count);
		  foreach (object val in values)
		  {
			offsetSorted.Add((TimeCoordIntvValue) val);
		  }
		  offsetSorted.Sort();

		  return new CoordinateTimeIntv(code, timeUnit, refDate, offsetSorted, null);
		}
	  }

	  public class Builder1 : CoordinateBuilderImpl<Grib1Record>
	  {
		internal readonly Grib1Customizer cust;
		internal readonly int code; // pdsFirst.getTimeUnit()
		internal readonly CalendarPeriod timeUnit;
		internal readonly CalendarDate refDate;

		internal Builder1(Grib1Customizer cust, int code, CalendarPeriod timeUnit, CalendarDate refDate)
		{
		  this.cust = cust;
		  this.code = code;
		  this.timeUnit = timeUnit;
		  this.refDate = refDate;
		}

		public override object extract(Grib1Record gr)
		{
		  Grib1SectionProductDefinition pds = gr.PDSsection;
		  Grib1ParamTime ptime = gr.getParamTime(cust);
		  int[] intv = ptime.Interval;
		  TimeCoordIntvValue tinv = new TimeCoordIntvValue(intv[0], intv[1]);

		  // If its a different time unit, we have to adjust the interval.
		  int tuInRecord = pds.TimeUnit;
		  if (tuInRecord != code)
		  {
			CalendarPeriod unitInRecord = GribUtils.getCalendarPeriod(tuInRecord);
			tinv = tinv.convertReferenceDate(gr.ReferenceDate, unitInRecord, refDate, timeUnit);
		  }
		  return tinv;
		}

		public override Coordinate makeCoordinate(IList<object> values)
		{
		  IList<TimeCoordIntvValue> offsetSorted = new List<TimeCoordIntvValue>(values.Count);
		  foreach (object val in values)
		  {
			offsetSorted.Add((TimeCoordIntvValue) val);
		  }
		  offsetSorted.Sort();

		  return new CoordinateTimeIntv(code, timeUnit, refDate, offsetSorted, null);
		}
	  }

	}

}
