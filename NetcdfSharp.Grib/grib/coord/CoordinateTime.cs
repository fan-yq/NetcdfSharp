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
	using Grib2Utils = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Utils;
	using CalendarDate = NetcdfSharp.ucar.nc2.time.CalendarDate;
	using CalendarDateRange = NetcdfSharp.ucar.nc2.time.CalendarDateRange;
	using CalendarDateUnit = NetcdfSharp.ucar.nc2.time.CalendarDateUnit;
	using CalendarPeriod = NetcdfSharp.ucar.nc2.time.CalendarPeriod;
	using Counters = NetcdfSharp.ucar.nc2.util.Counters;
	using Indent = NetcdfSharp.ucar.nc2.util.Indent;
	using Misc = NetcdfSharp.ucar.nc2.util.Misc;

	/// <summary>
	/// Time coordinates that are offsets from the reference date (not intervals).
	/// 
	/// @author caron
	/// @since 11/24/13
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class CoordinateTime extends CoordinateTimeAbstract implements Coordinate
	public class CoordinateTime : CoordinateTimeAbstract, Coordinate
	{
	  private static readonly Logger logger = LoggerFactory.getLogger(typeof(CoordinateTime));

	  private readonly IList<int> offsetSorted;

	  public CoordinateTime(int code, CalendarPeriod timeUnit, CalendarDate refDate, IList<int> offsetSorted, int[] time2runtime) : base(code, timeUnit, refDate, time2runtime)
	  {
		this.offsetSorted = offsetSorted.AsReadOnly();
	  }

	  internal CoordinateTime(CoordinateTime org, CalendarDate refDate) : base(org.code, org.timeUnit, refDate, null)
	  {
		this.offsetSorted = org.OffsetSorted;
	  }

	  public virtual IList<int> OffsetSorted
	  {
		  get
		  {
			return offsetSorted;
		  }
	  }

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public NetcdfSharp.java.util.List<?> getValues()
	  public override IList<object> Values
	  {
		  get
		  {
			return offsetSorted.Cast<object>().ToList();
		  }
	  }

	  public override int getIndex(object val)
	  {
		return Collections.binarySearch(offsetSorted, (int?) val);
	  }

	  public override object getValue(int idx)
	  {
		return offsetSorted[idx];
	  }

	  public override int Size
	  {
		  get
		  {
			return offsetSorted.Count;
		  }
	  }

	  public override Coordinate.Type getType()
	  {
		return NetcdfSharp.ucar.nc2.grib.coord.Coordinate.Type.time;
	  }

	  public override int estMemorySize()
	  {
		return 320 + Size * (16);
	  }

	  public override CalendarDateRange makeCalendarDateRange(NetcdfSharp.ucar.nc2.time.Calendar cal)
	  {
		CalendarDateUnit cdu = CalendarDateUnit.withCalendar(cal, periodName + " since " + refDate);
		CalendarDate start = cdu.makeCalendarDate(timeUnit.Value * offsetSorted[0]);
		CalendarDate end = cdu.makeCalendarDate(timeUnit.Value * offsetSorted[Size - 1]);
		return CalendarDateRange.of(start, end);
	  }

	  public override void showInfo(Formatter info, Indent indent)
	  {
		info.format("%s%s:", indent, getType());
		foreach (int? cd in offsetSorted)
		{
		  info.format(" %3d,", cd);
		}
		info.format(" (%d) %n", offsetSorted.Count);
		if (time2runtime != null)
		{
		  info.format("%stime2runtime: %s", indent, "[" + string.Join(", ", time2runtime) + "]");
		}
	  }

	  public override void showCoords(Formatter info)
	  {
		info.format("Time offsets: (%s) ref=%s %n", TimeUnit, RefDate);
		foreach (int? cd in offsetSorted)
		{
		  info.format("   %3d%n", cd);
		}
	  }

	  public override Counters calcDistributions()
	  {
		Counters counters = new Counters();
		counters.add("resol");

		IList<int> offsets = OffsetSorted;
		for (int i = 0; i < offsets.Count - 1; i++)
		{
		  int diff = offsets[i + 1] - offsets[i];
		  counters.count("resol", diff);
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

		CoordinateTime that = (CoordinateTime) o;

		if (code != that.code)
		{
		  return false;
		}
// JAVA TO C# CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
// ORIGINAL LINE: return offsetSorted.equals(that.offsetSorted);
		return offsetSorted.SequenceEqual(that.offsetSorted);

	  }

	  public override int GetHashCode()
	  {
		int result = offsetSorted.GetHashCode();
		result = 31 * result + code;
		return result;
	  }

	  protected internal override CoordinateTimeAbstract makeBestFromComplete(int[] best, int n)
	  {
		IList<int> offsetSortedBest = new List<int>(offsetSorted.Count);
		int[] time2runtimeBest = new int[n];
		int count = 0;
		for (int i = 0; i < best.Length; i++)
		{
		  int time = best[i];
		  if (time >= 0)
		  {
			time2runtimeBest[count] = time;
			offsetSortedBest.Add(offsetSorted[i]);
			count++;
		  }
		}

		return new CoordinateTime(code, timeUnit, refDate, offsetSortedBest, time2runtimeBest);
	  }

	  //////////////////////////////////////////////////////

	  public class Builder2 : CoordinateBuilderImpl<Grib2Record>
	  {
		internal readonly int code; // pdsFirst.getTimeUnit()
		internal readonly CalendarPeriod timeUnit;
		internal readonly CalendarDate refDate;

		public Builder2(int code, CalendarPeriod timeUnit, CalendarDate refDate)
		{
		  this.code = code;
		  this.timeUnit = timeUnit;
		  this.refDate = refDate;
		}

		public Builder2(CoordinateTime from)
		{
		  this.code = from.Code;
		  this.timeUnit = from.TimeUnit;
		  this.refDate = from.RefDate;
		}

		public override object extract(Grib2Record gr)
		{
		  Grib2Pds pds = gr.PDS;
		  int offset = pds.ForecastTime;
		  int tuInRecord = pds.TimeUnit;
		  if (tuInRecord == code)
		  {
			return offset;

		  }
		  else
		  {
			CalendarPeriod period = Grib2Utils.getCalendarPeriod(tuInRecord);
			if (period == null)
			{
			  logger.warn("Cant find period for time unit=" + tuInRecord);
			  return offset;
			}
			CalendarDate validDate = refDate.add(period.multiply(offset));
			return timeUnit.getOffset(refDate, validDate);
		  }
		}

		public override Coordinate makeCoordinate(IList<object> values)
		{
		  IList<int> offsetSorted = new List<int>(values.Count);
		  foreach (object val in values)
		  {
			offsetSorted.Add(((int?) val).Value);
		  }
		  offsetSorted.Sort();
		  return new CoordinateTime(code, timeUnit, refDate, offsetSorted, null);
		}
	  }

	  public class Builder1 : CoordinateBuilderImpl<Grib1Record>
	  {
		internal readonly Grib1Customizer cust;
		internal readonly int code; // pdsFirst.getTimeUnit()
		internal readonly CalendarPeriod timeUnit;
		internal readonly CalendarDate refDate;

		public Builder1(Grib1Customizer cust, int code, CalendarPeriod timeUnit, CalendarDate refDate)
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

		  int offset = ptime.ForecastTime;
		  int tuInRecord = pds.TimeUnit;
		  if (tuInRecord == code)
		  {
			return offset;

		  }
		  else
		  {
			CalendarDate validDate = GribUtils.getValidTime(refDate, tuInRecord, offset);
			return timeUnit.getOffset(refDate, validDate);
		  }

		}

		public override Coordinate makeCoordinate(IList<object> values)
		{
		  IList<int> offsetSorted = new List<int>(values.Count);
		  foreach (object val in values)
		  {
			offsetSorted.Add(((int?) val).Value);
		  }
		  offsetSorted.Sort();
		  return new CoordinateTime(code, timeUnit, refDate, offsetSorted, null);
		}
	  }


	}

}
