// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.coord
{
	using Grib = NetcdfSharp.ucar.nc2.grib.collection.Grib;
	using Grib1Record = NetcdfSharp.ucar.nc2.grib.grib1.Grib1Record;
	using Grib1Customizer = NetcdfSharp.ucar.nc2.grib.grib1.tables.Grib1Customizer;
	using Grib2Record = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Record;
	using Grib2Tables = NetcdfSharp.ucar.nc2.grib.grib2.table.Grib2Tables;
	using CalendarDate = NetcdfSharp.ucar.nc2.time.CalendarDate;
	using CalendarDateRange = NetcdfSharp.ucar.nc2.time.CalendarDateRange;
	using CalendarPeriod = NetcdfSharp.ucar.nc2.time.CalendarPeriod;
	using Counters = NetcdfSharp.ucar.nc2.util.Counters;
	using Indent = NetcdfSharp.ucar.nc2.util.Indent;

	/// <summary>
	/// Both runtime and time coordinates are tracked here. The time coordinate is dependent on the runtime, at least on the
	/// offset.
	/// isOrthogonal means all time coordinate offsets are the same for each runtime.
	/// isRegular means all time coordinate offsets are the same for each "runtime hour of day" (from FMRC).
	/// 
	/// @author caron
	/// @since 1/22/14
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class CoordinateTime2D extends CoordinateTimeAbstract implements Coordinate
	public class CoordinateTime2D : CoordinateTimeAbstract, Coordinate
	{
	  private static Logger logger = LoggerFactory.getLogger(typeof(CoordinateTime2D));

	  private readonly CoordinateRuntime runtime;
	  private readonly IList<Coordinate> times; // nruns time coordinates - original offsets
	  private readonly CoordinateTimeAbstract otime; // orthogonal time coordinates - only when isOrthogonal
	  private readonly SortedDictionary<int, CoordinateTimeAbstract> regTimes; // only when isRegular: <hour of day, time
																		 // coordinate>
	  private readonly int[] offset; // list all offsets from the base/first runtime, length nruns (LOOK can we use
								  // SmartArrayInt ?)

	  private readonly bool isRegular; // offsets are the same for each "runtime hour of day"
	  private readonly bool isOrthogonal; // offsets same for all runtimes, so 2d time array is (runtime X otime)
	  private readonly bool isTimeInterval;
	  private readonly int nruns;
	  private readonly int ntimes;

	  private readonly IList<Time2D> vals; // only present when building the GC, otherwise null

	  /// <summary>
	  /// Ctor. Most general, a CoordinateTime for each runtime.
	  /// </summary>
	  /// <param name="code"> pdsFirst.getTimeUnit() </param>
	  /// <param name="timeUnit"> time duration, based on code </param>
	  /// <param name="vals"> complete set of Time2D values, may be null (used only during creation) </param>
	  /// <param name="runtime"> list of runtimes </param>
	  /// <param name="times"> list of times, one for each runtime, offsets relative to its runtime, may not be null </param>
	  public CoordinateTime2D(int code, CalendarPeriod timeUnit, IList<Time2D> vals, CoordinateRuntime runtime, IList<Coordinate> times, int[] time2runtime) : base(code, timeUnit, runtime.FirstDate, time2runtime)
	  {

		this.runtime = runtime;
		this.times = times.AsReadOnly();
		this.otime = null;
		this.regTimes = null;
		this.isRegular = false;
		this.isOrthogonal = false;
		this.isTimeInterval = times[0] is CoordinateTimeIntv;

		int nmax = 0;
		foreach (Coordinate time in times)
		{
		  nmax = Math.Max(nmax, time.Size);
		}
		this.ntimes = nmax;
		this.nruns = runtime.Size;
		Debug.Assert(nruns == times.Count);

		this.offset = makeOffsets(times);
		this.vals = (vals == null) ? null : vals.AsReadOnly();
	  }

	  /// <summary>
	  /// Ctor. orthogonal - all offsets are the same for all runtimes, so 2d time array is (runtime X otime)
	  /// </summary>
	  /// <param name="code"> pdsFirst.getTimeUnit() </param>
	  /// <param name="timeUnit"> time duration, based on code </param>
	  /// <param name="vals"> complete set of Time2D values, may be null (used only during creation) </param>
	  /// <param name="runtime"> list of runtimes </param>
	  /// <param name="otime"> list of offsets, all the same for each runtime </param>
	  /// <param name="times"> list of times, one for each runtime, offsets relative to its runtime, may be null (Only available
	  ///        during creation, not stored in index) </param>
	  public CoordinateTime2D(int code, CalendarPeriod timeUnit, IList<Time2D> vals, CoordinateRuntime runtime, CoordinateTimeAbstract otime, IList<Coordinate> times, int[] time2runtime) : base(code, timeUnit, runtime.FirstDate, time2runtime)
	  {

		this.runtime = runtime;
		this.times = (times == null) ? null : times.AsReadOnly(); // need these for makeBest

		this.otime = otime;
		this.isOrthogonal = true;
		this.isRegular = false;
		this.regTimes = null;
		this.isTimeInterval = otime is CoordinateTimeIntv;
		this.ntimes = otime.Size;

		this.nruns = runtime.Size;
		this.offset = makeOffsets(timeUnit);
		this.vals = (vals == null) ? null : vals.AsReadOnly();
	  }

	  /// <summary>
	  /// Ctor. regular - all offsets are the same for each "runtime hour of day", eg all 0Z runtimes have the same offsets,
	  /// all 6Z runtimes have the same offsets, etc.
	  /// 2d time array is (runtime X otime(hour), where hour = runtime hour of day
	  /// </summary>
	  /// <param name="code"> pdsFirst.getTimeUnit() </param>
	  /// <param name="timeUnit"> time duration, based on code </param>
	  /// <param name="vals"> complete set of Time2D values, may be null (used only during creation) </param>
	  /// <param name="runtime"> list of runtimes </param>
	  /// <param name="regList"> list of offsets, one each for each possible runtime hour of day. </param>
	  /// <param name="times"> list of times, one for each runtime, offsets relative to its runtime, may be null (Only available
	  ///        during creation, not stored in index) </param>
	  public CoordinateTime2D(int code, CalendarPeriod timeUnit, IList<Time2D> vals, CoordinateRuntime runtime, IList<Coordinate> regList, IList<Coordinate> times, int[] time2runtime) : base(code, timeUnit, runtime.FirstDate, time2runtime)
	  {

		this.runtime = runtime;
		this.nruns = runtime.Size;

		this.times = (times == null) ? null : times.AsReadOnly(); // need these for makeBest
		this.otime = null;
		this.isOrthogonal = false;
		this.isRegular = true;
		CoordinateTimeAbstract first = (CoordinateTimeAbstract) regList[0];
		this.isTimeInterval = first is CoordinateTimeIntv;

		// regList may have different lengths
		int nmax = 0;
		foreach (Coordinate time in regList)
		{
		  nmax = Math.Max(nmax, time.Size);
		}
		this.ntimes = nmax;

		// make the offset map
		this.regTimes = new SortedDictionary<int, CoordinateTimeAbstract>();
		foreach (Coordinate coord in regList)
		{
		  CoordinateTimeAbstract time = (CoordinateTimeAbstract) coord;
		  CalendarDate @ref = time.RefDate;
		  int hour = @ref.HourOfDay;
		  this.regTimes[hour] = time;
		}

		this.offset = makeOffsets(timeUnit);
		this.vals = (vals == null) ? null : vals.AsReadOnly();
	  }

	  private int[] makeOffsets(IList<Coordinate> orgTimes)
	  {
		CalendarDate firstDate = runtime.FirstDate;
		int[] offsets = new int[nruns];
		for (int idx = 0; idx < nruns; idx++)
		{
		  CoordinateTimeAbstract coordTime = (CoordinateTimeAbstract) orgTimes[idx];
		  CalendarPeriod period = coordTime.TimeUnit; // LOOK are we assuming all have same period ??
		  offsets[idx] = period.getOffset(firstDate, runtime.getRuntimeDate(idx)); // LOOK possible loss of precision
		}
		return offsets;
	  }

	  private int[] makeOffsets(CalendarPeriod period)
	  {
		CalendarDate firstDate = runtime.FirstDate;
		int[] offsets = new int[nruns];
		for (int idx = 0; idx < nruns; idx++)
		{
		  offsets[idx] = period.getOffset(firstDate, runtime.getRuntimeDate(idx)); // LOOK possible loss of precision
		}
		return offsets;
	  }

	  public override string Name
	  {
		  set
		  {
			base.Name = value;
			if (Orthogonal)
			{
			  otime.Name = value;
			}
			else if (Regular)
			{
			  foreach (CoordinateTimeAbstract time in regTimes.Values)
			  {
				time.Name = value;
			  }
			}
			else
			{
			  foreach (Coordinate time in times)
			  {
				((CoordinateTimeAbstract) time).Name = value;
			  }
			}
		  }
	  }

	  ///////////////////////////////////////////////////////

	  public virtual CoordinateRuntime RuntimeCoordinate
	  {
		  get
		  {
			return runtime;
		  }
	  }

	  public virtual bool TimeInterval
	  {
		  get
		  {
			return isTimeInterval;
		  }
	  }

	  public virtual bool Orthogonal
	  {
		  get
		  {
			return isOrthogonal;
		  }
	  }

	  public virtual bool Regular
	  {
		  get
		  {
			return isRegular;
		  }
	  }

	  public virtual int Ntimes
	  {
		  get
		  {
			return ntimes;
		  }
	  }

	  public virtual int Nruns
	  {
		  get
		  {
			return nruns;
		  }
	  }

	  public virtual int getOffset(int runIndex)
	  {
		return offset[runIndex];
	  }

	  public virtual IEnumerable<int> RegularHourOffsets
	  {
		  get
		  {
			if (!isRegular)
			{
			  return null;
			}
			return regTimes.Keys;
		  }
	  }

	  public override void showInfo(Formatter info, Indent indent)
	  {
		info.format("%s%s:", indent, getType());
		info.format(" %s runtime=%s nruns=%d ntimes=%d isOrthogonal=%s isRegular=%s%n", name, runtime.Name, nruns, ntimes, isOrthogonal, isRegular);
		indent.incr();

		info.format("%sAll time values=", indent);
		System.Collections.IList timeValues = OffsetsSorted;
		foreach (object val in timeValues)
		{
		  info.format(" %s,", val);
		}
		info.format(" (n=%d)%n", timeValues.Count);

		if (isOrthogonal)
		{
		  otime.showInfo(info, indent);
		}

		else if (isRegular)
		{
		  foreach (int hour in regTimes.Keys)
		  {
			CoordinateTimeAbstract timeCoord = regTimes[hour];
			info.format("%shour %d: ", indent, hour);
			timeCoord.showInfo(info, new Indent(0));
		  }
		}

		else
		{
		  foreach (Coordinate time in times)
		  {
			info.format("%s%s:", indent, ((CoordinateTimeAbstract) time).RefDate);
			time.showInfo(info, new Indent(0));
		  }
		}
		indent.decr();
	  }

	  public override void showCoords(Formatter info)
	  {
		info.format("%s runtime=%s nruns=%d ntimes=%d isOrthogonal=%s isRegular=%s%n", name, runtime.Name, nruns, ntimes, isOrthogonal, isRegular);
		// runtime.showCoords(info);

		if (isOrthogonal)
		{
		  otime.showCoords(info);
		}

		else if (isRegular)
		{
		  foreach (int hour in regTimes.Keys)
		  {
			CoordinateTimeAbstract timeCoord = regTimes[hour];
			info.format("hour %d: ", hour);
			timeCoord.showInfo(info, new Indent(0));
		  }
		}

		else
		{
		  foreach (Coordinate time in times)
		  {
			time.showCoords(info);
		  }
		}
	  }

	  public override Counters calcDistributions()
	  {
		Counters counters = new Counters();
		counters.add("resol");

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: List<?> offsets = getOffsetsSorted();
		IList<object> offsets = OffsetsSorted;
		if (TimeInterval)
		{
		  counters.add("intv");
		  for (int i = 0; i < offsets.Count; i++)
		  {
			TimeCoordIntvValue tinv = (TimeCoordIntvValue) offsets[i];
			int intv = tinv.Bounds2 - tinv.Bounds1;
			counters.count("intv", intv);
			if (i > 0)
			{
			  int resol = tinv.Bounds1 - ((TimeCoordIntvValue) offsets[i - 1]).Bounds1;
			  counters.count("resol", resol);
			}
		  }

		}
		else
		{
		  for (int i = 0; i < offsets.Count - 1; i++)
		  {
			int diff = (int) offsets[i + 1] - (int) offsets[i];
			counters.count("resol", diff);
		  }
		}

		return counters;
	  }

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public List<?> getValues()
	  public override IList<object> Values
	  {
		  get
		  {
			return (vals == null) ? null : vals.Cast<object>().ToList();
		  }
	  }

	  public override object getValue(int idx)
	  {
		return vals[idx];
	  }

	  public override int getIndex(object val)
	  {
		return (vals == null) ? -1 : Collections.binarySearch(vals, (Time2D) val);
	  }

	  public override int Size
	  {
		  get
		  {
			return (vals == null) ? 0 : vals.Count;
		  }
	  }

	  public override int NCoords
	  {
		  get
		  {
			return OffsetsSorted.Count;
		  }
	  }

	  public override int estMemorySize()
	  {
		return 864 + nruns * (48 + 4) + ntimes * 24; // nruns * (calendar date + integer) + ntimes + integer)
	  }

	  public override Coordinate.Type getType()
	  {
		return NetcdfSharp.ucar.nc2.grib.coord.Coordinate.Type.time2D;
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

		CoordinateTime2D that = (CoordinateTime2D) o;
		if (isTimeInterval != that.isTimeInterval)
		{
		  return false;
		}
		if (!runtime.Equals(that.runtime))
		{
		  return false;
		}
		if (isOrthogonal != that.isOrthogonal)
		{
		  return false;
		}
		if (isRegular != that.isRegular)
		{
		  return false;
		}
		if (!object.Equals(otime, that.otime))
		{
		  return false;
		}
		if (!object.Equals(regTimes, that.regTimes))
		{
		  return false;
		}
		return object.Equals(times, that.times);

	  }

	  public override int GetHashCode()
	  {
		int result = runtime.GetHashCode();
		result = 31 * result + (times != null ? times.GetHashCode() : 0);
		result = 31 * result + (otime != null ? otime.GetHashCode() : 0);
		result = 31 * result + (regTimes != null ? regTimes.GetHashCode() : 0);
		result = 31 * result + (isRegular ? 1 : 0);
		result = 31 * result + (isOrthogonal ? 1 : 0);
		result = 31 * result + (isTimeInterval ? 1 : 0);
		return result;
	  }

	  /// <summary>
	  /// Check if all time intervals have the same length.
	  /// Only if isTimeInterval.
	  /// </summary>
	  /// <returns> time interval name or MIXED_INTERVALS </returns>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public String getTimeIntervalName()
	  public virtual string TimeIntervalName
	  {
		  get
		  {
			if (!isTimeInterval)
			{
			  return null;
			}

			if (isOrthogonal)
			{
			  return ((CoordinateTimeIntv) otime).TimeIntervalName;
			}

			if (isRegular)
			{
			  string firstValue = null;
			  foreach (CoordinateTimeAbstract timeCoord in regTimes.Values)
			  {
				CoordinateTimeIntv timeCoordi = (CoordinateTimeIntv) timeCoord;
				string value = timeCoordi.TimeIntervalName;
				if (string.ReferenceEquals(firstValue, null))
				{
				  firstValue = value;
				}
				else if (!value.Equals(firstValue))
				{
				  return MIXED_INTERVALS;
				}
				else if (value.Equals(MIXED_INTERVALS))
				{
				  return MIXED_INTERVALS;
				}
			  }
			  return firstValue;
			}

			// are they the same length ?
			string firstValue2 = null;
			foreach (Coordinate timeCoord in times)
			{
			  if (times.Count == 0)
			  {
				continue; // skip empties
			  }
			  CoordinateTimeIntv timeCoordi = (CoordinateTimeIntv) timeCoord;
			  string value = timeCoordi.TimeIntervalName;
			  if (string.ReferenceEquals(firstValue2, null))
			  {
				firstValue2 = value;
			  }
			  else if (!value.Equals(firstValue2))
			  {
				return MIXED_INTERVALS;
			  }
			  else if (value.Equals(MIXED_INTERVALS))
			  {
				return MIXED_INTERVALS;
			  }
			}
			return firstValue2;
		  }
	  }

	  public override CalendarDateRange makeCalendarDateRange(NetcdfSharp.ucar.nc2.time.Calendar cal)
	  {

		CoordinateTimeAbstract firstCoord = getTimeCoordinate(0);
		CoordinateTimeAbstract lastCoord = getTimeCoordinate(nruns - 1);

		CalendarDateRange firstRange = firstCoord.makeCalendarDateRange(cal);
		CalendarDateRange lastRange = lastCoord.makeCalendarDateRange(cal);

		return CalendarDateRange.of(firstRange.Start, lastRange.End);
	  }

	  ////////////////////////////////////////////////

	  public virtual CoordinateTimeAbstract getTimeCoordinate(int runIdx)
	  {
		if (isOrthogonal)
		{
		  // LOOK problem is cant use time.getRefDate(), must use time2D.getRefDate(runIdx) !!
		  return factory(otime, getRefDate(runIdx));
		}

		if (isRegular)
		{
		  CalendarDate @ref = getRefDate(runIdx);
		  int hour = @ref.HourOfDay;
		  return regTimes[hour];
		}

		return (CoordinateTimeAbstract) times[runIdx];
	  }

	  public virtual CoordinateTimeAbstract getRegularTimeCoordinate(int hour)
	  {
		return regTimes[hour];
	  }

	  public virtual CalendarDate getRefDate(int runIdx)
	  {
		return runtime.getRuntimeDate(runIdx);
	  }

	  public virtual long getRuntime(int runIdx)
	  {
		return runtime.getRuntime(runIdx);
	  }

	  private CoordinateTimeAbstract factory(CoordinateTimeAbstract org, CalendarDate refDate)
	  {
		CoordinateTimeAbstract result;
		if (isTimeInterval)
		{
		  result = new CoordinateTimeIntv((CoordinateTimeIntv) org, refDate);
		}
		else
		{
		  result = new CoordinateTime((CoordinateTime) org, refDate);
		}
		result.Name = org.Name;
		return result;
	  }

	  /// <summary>
	  /// Get the time coordinate at the given indices, into the 2D time coordinate array
	  /// </summary>
	  /// <param name="runIdx"> run index </param>
	  /// <param name="timeIdx"> time index </param>
	  /// <returns> time coordinate </returns>
	  public virtual Time2D getOrgValue(int runIdx, int timeIdx)
	  {
		CoordinateTimeAbstract time = getTimeCoordinate(runIdx);
		CalendarDate runDate = runtime.getRuntimeDate(runIdx);
		if (isTimeInterval)
		{
		  TimeCoordIntvValue valIntv = (TimeCoordIntvValue) time.getValue(timeIdx);
		  if (valIntv == null)
		  {
			throw new ArgumentException();
		  }
		  return new Time2D(runDate, null, valIntv);
		}
		else
		{
		  int? val = (int?) time.getValue(timeIdx);
		  if (val == null)
		  {
			throw new ArgumentException();
		  }
		  return new Time2D(runDate, val, null);
		}
	  }

	  /// <summary>
	  /// find the run and time indexes of want
	  /// the inverse of getOrgValue
	  /// </summary>
	  /// <param name="want"> find time coordinate that matches </param>
	  /// <param name="wholeIndex"> return index here </param>
	  public virtual bool getIndex(Time2D want, int[] wholeIndex)
	  {
		int runIdx = runtime.getIndex(want.refDate);
		CoordinateTimeAbstract time = getTimeCoordinate(runIdx);
		wholeIndex[0] = runIdx;
		if (isTimeInterval)
		{
		  wholeIndex[1] = time.getIndex(want.tinv);
		}
		else
		{
		  wholeIndex[1] = time.getIndex(want.time);
		}
		return (wholeIndex[0] >= 0) && (wholeIndex[1] >= 0);
	  }

	  /// <summary>
	  /// Find the index matching a runtime and time coordinate
	  /// </summary>
	  /// <param name="runIdx"> which run </param>
	  /// <param name="value"> time coordinate </param>
	  /// <returns> index in the time coordinate of the value </returns>
	  public virtual int matchTimeCoordinate(int runIdx, CoordinateTime2D.Time2D value)
	  {
		CoordinateTimeAbstract time = getTimeCoordinate(runIdx);
		int offset = timeUnit.getOffset(getRefDate(runIdx), value.getRefDate());

		object valueWithOffset;
		if (isTimeInterval)
		{
		  valueWithOffset = value.tinv.offset(offset);
		}
		else
		{
		  valueWithOffset = value.time + offset;
		}
		int result = time.getIndex(valueWithOffset);
		if (Grib.debugRead)
		{
		  logger.debug("  matchTimeCoordinate value wanted = ({}) valueWithOffset={} result={}", value, valueWithOffset, result);
		}

		return result;
	  }

	  ///////////////////////////////////////////////////////////////////////////////////////

	  protected internal override CoordinateTimeAbstract makeBestFromComplete(int[] best, int n)
	  {
		throw new System.NotSupportedException();
	  }

	  public virtual CoordinateTimeAbstract makeTime2Runtime(CoordinateRuntime master)
	  {
		if (isTimeInterval)
		{
		  return makeBestTimeIntv(master);
		}
		else
		{
		  return makeBestTime(master);
		}
	  }

	  public virtual CoordinateTimeAbstract makeBestTimeCoordinate(CoordinateRuntime master)
	  {
		if (isTimeInterval)
		{
		  return makeBestTimeIntv(master);
		}
		else
		{
		  return makeBestTime(master);
		}
	  }

	  private CoordinateTimeAbstract makeBestTime(CoordinateRuntime master)
	  {
		// make complete, unique set of coordinates
		ISet<int> values = new HashSet<int>(); // complete set of values
		for (int runIdx = 0; runIdx < nruns; runIdx++)
		{ // use times array, passed into constructor, with original inventory, if possible
		  CoordinateTime timeCoord = (times == null) ? (CoordinateTime) getTimeCoordinate(runIdx) : (CoordinateTime) times[runIdx];
		  foreach (int offset in timeCoord.OffsetSorted)
		  {
			values.Add(offset + getOffset(runIdx));
		  }
		}
		List<int> offsetSorted = new List<int>(values.Count);
		foreach (int val in values)
		{
		  offsetSorted.Add(val);
		}
		offsetSorted.Sort(); // complete set of values

		// fast lookup of offset val in the result CoordinateTime
		IDictionary<int, int> map = new Dictionary<int, int>();
		int count = 0;
		foreach (int val in offsetSorted)
		{
		  map[val] = count++;
		}

		// fast lookup of the run time in the master
		int[] run2master = new int[nruns];
		int masterIdx = 0;
		for (int run2Didx = 0; run2Didx < nruns; run2Didx++)
		{
		  while (!master.getRuntimeDate(masterIdx).Equals(runtime.getRuntimeDate(run2Didx)))
		  {
			masterIdx++;
		  }
		  run2master[run2Didx] = masterIdx;
		  masterIdx++;
		}
		Debug.Assert(masterIdx >= nruns);

		// now for each coordinate, use the latest runtime available
		int[] time2runtime = new int[offsetSorted.Count];
		for (int runIdx = 0; runIdx < nruns; runIdx++)
		{
		  CoordinateTime timeCoord = (times == null) ? (CoordinateTime) getTimeCoordinate(runIdx) : (CoordinateTime) times[runIdx];
		  Debug.Assert(timeCoord != null);
		  foreach (int offset in timeCoord.OffsetSorted)
		  {
			int bestValIdx;
			if (!map.TryGetValue(offset + getOffset(runIdx), out bestValIdx))
			{
			  throw new System.InvalidOperationException();
			}
			time2runtime[bestValIdx] = run2master[runIdx] + 1; // uses this runtime; later ones override; one based so 0 = missing
		  }
		}

		return new CoordinateTime(Code, TimeUnit, RefDate, offsetSorted, time2runtime);
	  }

	  // can we make into a MRUTC ?
	  public virtual bool hasUniqueTimes()
	  {
		if (isTimeInterval)
		{
		  // make unique set of coordinates
		  ISet<TimeCoordIntvValue> values = new HashSet<TimeCoordIntvValue>();
		  for (int runIdx = 0; runIdx < nruns; runIdx++)
		  { // use times array, passed into constructor, with original inventory, if possible
			CoordinateTimeIntv timeIntv = (times == null) ? (CoordinateTimeIntv) getTimeCoordinate(runIdx) : (CoordinateTimeIntv) times[runIdx];
			foreach (TimeCoordIntvValue tinv in timeIntv.TimeIntervals)
			{
			  TimeCoordIntvValue tinvAbs = tinv.offset(getOffset(runIdx)); // convert to absolute offset
			  if (values.Contains(tinvAbs))
			  {
				return false;
			  }
			  values.Add(tinvAbs);
			}
		  }
		}
		else
		{
		  ISet<int> values = new HashSet<int>(); // complete set of values
		  for (int runIdx = 0; runIdx < nruns; runIdx++)
		  { // use times array, passed into constructor, with original inventory, if possible
			CoordinateTime timeCoord = (times == null) ? (CoordinateTime) getTimeCoordinate(runIdx) : (CoordinateTime) times[runIdx];
			Debug.Assert(timeCoord != null);
			foreach (int offset in timeCoord.OffsetSorted)
			{
			  int offsetAbs = offset + getOffset(runIdx); // convert to absolute offset
			  if (values.Contains(offsetAbs))
			  {
				return false;
			  }
			  values.Add(offsetAbs);
			}
		  }
		}
		return true;
	  }

	  public virtual int[] getTimeIndicesFromMrutp(int timeIdx)
	  {
		int[] result = new int[2];
		if (isOrthogonal)
		{
		  int nPerRun = otime.NCoords;
		  result[0] = timeIdx / nPerRun;
		  result[1] = timeIdx % nPerRun;
		  return result;

		}
		else if (isRegular)
		{
		  int nPerRun = 0;
		  foreach (KeyValuePair<int, CoordinateTimeAbstract> entry in regTimes)
		  {
			nPerRun += entry.Value.NCoords;
		  }
		  if (nPerRun > 0)
		  {
			result[0] = timeIdx / nPerRun;
			result[1] = timeIdx % nPerRun;
		  }
		  return result;

		}
		else
		{

		  int runtime = 0;
		  int count = 0;
		  foreach (Coordinate time in times)
		  {
			if (count + time.NCoords > timeIdx)
			{
			  result[0] = runtime;
			  result[1] = timeIdx - count;
			  return result;
			}
			runtime++;
			count += time.NCoords;
		  }
		  throw new System.InvalidOperationException("timeIdx = " + timeIdx); // cant happen?
		}
	  }


	  private CoordinateTimeAbstract makeBestTimeIntv(CoordinateRuntime master)
	  {
		// make unique set of coordinates
		ISet<TimeCoordIntvValue> values = new HashSet<TimeCoordIntvValue>();
		for (int runIdx = 0; runIdx < nruns; runIdx++)
		{ // use times array, passed into constructor, with original inventory, if possible
		  CoordinateTimeIntv timeIntv = (times == null) ? (CoordinateTimeIntv) getTimeCoordinate(runIdx) : (CoordinateTimeIntv) times[runIdx];
		  foreach (TimeCoordIntvValue tinv in timeIntv.TimeIntervals)
		  {
			values.Add(tinv.offset(getOffset(runIdx)));
		  }
		}
		List<TimeCoordIntvValue> offsetSorted = new List<TimeCoordIntvValue>(values.Count);
		foreach (TimeCoordIntvValue val in values)
		{
		  offsetSorted.Add(val);
		}
		offsetSorted.Sort();

		// fast lookup of offset tinv in the result CoordinateTimeIntv
		IDictionary<TimeCoordIntvValue, int> map = new Dictionary<TimeCoordIntvValue, int>(); // lookup coord val to index
		int count = 0;
		foreach (TimeCoordIntvValue val in offsetSorted)
		{
		  map[val] = count++;
		}

		// fast lookup of the run time in the master
		int[] run2master = new int[nruns];
		int masterIdx = 0;
		for (int run2Didx = 0; run2Didx < nruns; run2Didx++)
		{
		  while (!master.getRuntimeDate(masterIdx).Equals(runtime.getRuntimeDate(run2Didx)))
		  {
			masterIdx++;
		  }
		  run2master[run2Didx] = masterIdx;
		  masterIdx++;
		}
		Debug.Assert(masterIdx >= nruns);

		int[] time2runtime = new int[offsetSorted.Count];
		for (int runIdx = 0; runIdx < nruns; runIdx++)
		{
		  CoordinateTimeIntv timeIntv = (times == null) ? (CoordinateTimeIntv) getTimeCoordinate(runIdx) : (CoordinateTimeIntv) times[runIdx];
		  foreach (TimeCoordIntvValue bestVal in timeIntv.TimeIntervals)
		  {
			int bestValIdx;
			if (!map.TryGetValue(bestVal.offset(getOffset(runIdx)), out bestValIdx))
			{
			  throw new System.InvalidOperationException();
			}
			time2runtime[bestValIdx] = run2master[runIdx] + 1; // uses this runtime; later ones override; one based so 0 = missing
		  }
		}

		return new CoordinateTimeIntv(Code, TimeUnit, RefDate, offsetSorted, time2runtime);
	  }

	  ////////////////////////////////////////////////////////

	  /// <summary>
	  /// public by accident - do not use
	  /// </summary>
	  public virtual IList<Coordinate> getTimesForSerialization()
	  {
		if (isOrthogonal)
		{
		  IList<Coordinate> list = new List<Coordinate>(1);
		  list.Add(otime);
		  return list;
		}
		else if (isRegular)
		{
		  return new List<Coordinate>(regTimes.Values);
		}
		else
		{
		  return times;
		}
	  }

	  /// <summary>
	  /// Find index in time coordinate from the time value
	  /// </summary>
	  /// <param name="val"> TimeCoordIntvValue or Integer </param>
	  /// <returns> indx in the time coordinate </returns>
	  public virtual int findTimeIndexFromVal(int runIdx, object val)
	  {
		if (isOrthogonal)
		{
		  return otime.getIndex(val);
		}

		CoordinateTimeAbstract time = getTimeCoordinate(runIdx);
		if (time.NCoords == 1)
		{
		  return 0;
		}
		return time.getIndex(val); // not sure if its relative to runtime ??
	  }

	  /// <summary>
	  /// Get a sorted list of the unique time coordinates
	  /// </summary>
	  /// <returns> List&lt;Integer&gt; or List&lt;TimeCoordIntvValue&gt; </returns>
	  public virtual List<object> OffsetsSorted
	  {
		  get
		  {
			if (isOrthogonal)
			{
			  return new List<object>(otime.Values);
			}

			IList<Coordinate> coords = isRegular ? new List<Coordinate>(regTimes.Values) : times;
			if (isTimeInterval)
			{
			  return getIntervalsSorted(coords);
			}
			else
			{
			  return getIntegersSorted(coords);
			}
		  }
	  }

	  private List<object> getIntegersSorted(IList<Coordinate> coords)
	  {
		ISet<int> set = new HashSet<int>(100);
		foreach (Coordinate coord in coords)
		{
		  foreach (object val in coord.Values)
		  {
			set.Add(((int?) val).Value);
		  }
		}
		List<int> sorted = new List<int>(set);
		sorted.Sort();
		List<object> result = new List<object>(sorted.Count);
		foreach (int val in sorted)
		{
		  result.Add(val);
		}
		return result;
	  }

	  private List<object> getIntervalsSorted(IList<Coordinate> coords)
	  {
		ISet<TimeCoordIntvValue> set = new HashSet<TimeCoordIntvValue>(100);
		foreach (Coordinate coord in coords)
		{
		  foreach (object val in coord.Values)
		  {
			set.Add((TimeCoordIntvValue) val);
		  }
		}
		List<TimeCoordIntvValue> sorted = new List<TimeCoordIntvValue>(set);
		sorted.Sort();
		List<object> result = new List<object>(sorted.Count);
		foreach (TimeCoordIntvValue val in sorted)
		{
		  result.Add(val);
		}
		return result;
	  }

	  //////////////////////////////////////////////////////////////////////////////////////////////////////////////

	  public class Time2D : IComparable<Time2D>, IComparable
	  {
		internal long refDate;
		internal int? time;
		internal TimeCoordIntvValue tinv;

		public Time2D(CalendarDate refDate, int? time, TimeCoordIntvValue tinv)
		{
		  this.refDate = refDate.Millis;
		  this.time = time;
		  this.tinv = tinv;
		}

		internal Time2D(long refDate, int? time, TimeCoordIntvValue tinv)
		{
		  this.refDate = refDate;
		  this.time = time;
		  this.tinv = tinv;
		}

		public virtual CalendarDate getRefDate()
		{
		  return CalendarDate.of(refDate);
		}

		public virtual int getValue()
		{
		  if (time != null)
		  {
			return time.Value;
		  }
		  else
		  {
			return tinv.Bounds2;
		  }
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

		  Time2D time2D = (Time2D) o;

		  if (refDate != time2D.refDate)
		  {
			return false;
		  }
		  if (!object.Equals(time, time2D.time))
		  {
			return false;
		  }
		  return object.Equals(tinv, time2D.tinv);

		}

		public override int GetHashCode()
		{
		  int result = (int)(refDate ^ (refDate >> 32));
		  result = 31 * result + (time != null ? time.GetHashCode() : 0);
		  result = 31 * result + (tinv != null ? tinv.GetHashCode() : 0);
		  return result;
		}

		public override string ToString()
		{
		  if (time != null)
		  {
			return time.ToString();
		  }
		  else
		  {
			return tinv.ToString();
		  }
		}

		public virtual int CompareTo(Time2D o)
		{
		  int r = refDate.CompareTo(o.refDate);
		  if (r == 0)
		  {
			if (time != null)
			{
			  r = time.Value.CompareTo(o.time.Value);
			}
			else
			{
			  r = tinv.CompareTo(o.tinv);
			}
		  }
		  return r;
		}

		public virtual int CompareTo(object o)
		{
		  return CompareTo((Time2D) o);
		}

	  }

	  //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	  public class Builder2 : CoordinateBuilderImpl<Grib2Record>, CoordinateBuilder<Grib2Record>.TwoD<Grib2Record>
	  {
		private readonly bool isTimeInterval;
		private readonly Grib2Tables cust;
		private readonly int code; // pdsFirst.getTimeUnit()
		private readonly CalendarPeriod timeUnit; // time duration, based on code

		private readonly CoordinateRuntime.Builder2 runBuilder;
		private readonly IDictionary<object, CoordinateBuilderImpl<Grib2Record>> timeBuilders; // one for each runtime

		public Builder2(bool isTimeInterval, Grib2Tables cust, CalendarPeriod timeUnit, int code)
		{
		  this.isTimeInterval = isTimeInterval;
		  this.cust = cust;
		  this.timeUnit = timeUnit;
		  this.code = code;

		  runBuilder = new CoordinateRuntime.Builder2(timeUnit);
		  timeBuilders = new Dictionary<object, CoordinateBuilderImpl<Grib2Record>>();
		}

		public override void addRecord(Grib2Record gr)
		{
		  base.addRecord(gr);
		  runBuilder.addRecord(gr);
		  Time2D val = (Time2D) extract(gr);
		  CoordinateBuilderImpl<Grib2Record> timeBuilder = timeBuilders[val.refDate];
		  timeBuilder.addRecord(gr);
		}

		public override object extract(Grib2Record gr)
		{
		  long run = (long) runBuilder.extract(gr);
		  CoordinateBuilderImpl<Grib2Record> timeBuilder;
		  if (!timeBuilders.TryGetValue(run, out timeBuilder))
		  {
			timeBuilder = isTimeInterval ? (CoordinateBuilderImpl<Grib2Record>) new CoordinateTimeIntv.Builder2(cust, code, timeUnit, CalendarDate.of(run)) : new CoordinateTime.Builder2(code, timeUnit, CalendarDate.of(run));
			timeBuilders[run] = timeBuilder;
		  }
		  object time = timeBuilder.extract(gr);
		  if (time is int)
		  {
			return new Time2D(run, (int) time, null);
		  }
		  else
		  {
			return new Time2D(run, null, (TimeCoordIntvValue) time);
		  }
		}

		public override Coordinate makeCoordinate(IList<object> values)
		{
		  CoordinateRuntime runCoord = (CoordinateRuntime) runBuilder.finish();

		  IList<Coordinate> times = new List<Coordinate>(runCoord.Size);
		  for (int idx = 0; idx < runCoord.Size; idx++)
		  {
			long runtime = runCoord.getRuntime(idx);
			CoordinateBuilderImpl<Grib2Record> timeBuilder = timeBuilders[runtime];
			times.Add(timeBuilder.finish());
		  }

		  List<Time2D> vals = new List<Time2D>(values.Count);
		  foreach (object val in values)
		  {
			vals.Add((Time2D) val);
		  }
		  vals.Sort();

		  return new CoordinateTime2D(code, timeUnit, vals, runCoord, times, null);
		}

		public override void addAll(Coordinate coord)
		{
		  base.addAll(coord);
		  foreach (object val in coord.Values)
		  {
			Time2D val2D = (Time2D) val;
			runBuilder.add(val2D.refDate);
			CoordinateBuilderImpl<Grib2Record> timeBuilder;
			if (!timeBuilders.TryGetValue(val2D.refDate, out timeBuilder))
			{
			  timeBuilder = isTimeInterval ? (CoordinateBuilderImpl<Grib2Record>) new CoordinateTimeIntv.Builder2(cust, code, timeUnit, val2D.getRefDate()) : new CoordinateTime.Builder2(code, timeUnit, val2D.getRefDate());
			  timeBuilders[val2D.refDate] = timeBuilder;
			}
			timeBuilder.add(isTimeInterval ? val2D.tinv : val2D.time);
		  }
		}

		public virtual int[] getCoordIndices(Grib2Record gr)
		{
		  CoordinateTime2D coord2D = (CoordinateTime2D) coord;
		  long run = (long) runBuilder.extract(gr);
		  int runIdx = coord2D.runtime.getIndex(run);
		  CoordinateTimeAbstract timeCoord = coord2D.getTimeCoordinate(runIdx);

		  CoordinateBuilderImpl<Grib2Record> timeBuilder = timeBuilders[run];
		  object time = timeBuilder.extract(gr);
		  int timeIdx = timeCoord.getIndex(time);

		  return new int[] {runIdx, timeIdx};
		}

	  }

	  //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	  public class Builder1 : CoordinateBuilderImpl<Grib1Record>, CoordinateBuilder<Grib1Record>.TwoD<Grib1Record>
	  {
		private readonly bool isTimeInterval;
		private readonly Grib1Customizer cust;
		private readonly int code; // pdsFirst.getTimeUnit()
		private readonly CalendarPeriod timeUnit;

		private readonly CoordinateRuntime.Builder1 runBuilder;
		private readonly IDictionary<object, CoordinateBuilderImpl<Grib1Record>> timeBuilders;

		public Builder1(bool isTimeInterval, Grib1Customizer cust, CalendarPeriod timeUnit, int code)
		{
		  this.isTimeInterval = isTimeInterval;
		  this.cust = cust;
		  this.timeUnit = timeUnit;
		  this.code = code;

		  runBuilder = new CoordinateRuntime.Builder1(timeUnit);
		  timeBuilders = new Dictionary<object, CoordinateBuilderImpl<Grib1Record>>();
		}

		public override void addRecord(Grib1Record gr)
		{
		  base.addRecord(gr);
		  runBuilder.addRecord(gr);
		  Time2D val = (Time2D) extract(gr);
		  CoordinateBuilderImpl<Grib1Record> timeBuilder = timeBuilders[val.refDate];
		  timeBuilder.addRecord(gr);
		}

		public override object extract(Grib1Record gr)
		{
		  long run = (long) runBuilder.extract(gr);
		  CoordinateBuilderImpl<Grib1Record> timeBuilder;
		  if (!timeBuilders.TryGetValue(run, out timeBuilder))
		  {
			timeBuilder = isTimeInterval ? (CoordinateBuilderImpl<Grib1Record>) new CoordinateTimeIntv.Builder1(cust, code, timeUnit, CalendarDate.of(run)) : new CoordinateTime.Builder1(cust, code, timeUnit, CalendarDate.of(run));
			timeBuilders[run] = timeBuilder;
		  }
		  object time = timeBuilder.extract(gr);
		  if (time is int)
		  {
			return new Time2D(run, (int) time, null);
		  }
		  else
		  {
			return new Time2D(run, null, (TimeCoordIntvValue) time);
		  }
		}

		public override Coordinate makeCoordinate(IList<object> values)
		{
		  CoordinateRuntime runCoord = (CoordinateRuntime) runBuilder.finish();

		  IList<Coordinate> times = new List<Coordinate>(runCoord.Size);
		  for (int idx = 0; idx < runCoord.Size; idx++)
		  {
			long runtime = runCoord.getRuntime(idx);
			CoordinateBuilderImpl<Grib1Record> timeBuilder = timeBuilders[runtime];
			times.Add(timeBuilder.finish());
		  }

		  List<Time2D> vals = new List<Time2D>(values.Count);
		  foreach (object val in values)
		  {
			vals.Add((Time2D) val);
		  }
		  vals.Sort();

		  return new CoordinateTime2D(code, timeUnit, vals, runCoord, times, null);
		}

		public override void addAll(Coordinate coord)
		{
		  base.addAll(coord);
		  foreach (object val in coord.Values)
		  {
			Time2D val2D = (Time2D) val;
			runBuilder.add(val2D.refDate);
			CoordinateBuilderImpl<Grib1Record> timeBuilder;
			if (!timeBuilders.TryGetValue(val2D.refDate, out timeBuilder))
			{
			  timeBuilder = isTimeInterval ? (CoordinateBuilderImpl<Grib1Record>) new CoordinateTimeIntv.Builder1(cust, code, timeUnit, val2D.getRefDate()) : new CoordinateTime.Builder1(cust, code, timeUnit, val2D.getRefDate());
			  timeBuilders[val2D.refDate] = timeBuilder;
			}
			timeBuilder.add(isTimeInterval ? val2D.tinv : val2D.time);
		  }
		}

		public virtual int[] getCoordIndices(Grib1Record gr)
		{
		  CoordinateTime2D coord2D = (CoordinateTime2D) coord;
		  long run = (long) runBuilder.extract(gr);
		  int runIdx = coord2D.runtime.getIndex(run);
		  CoordinateTimeAbstract timeCoord = coord2D.getTimeCoordinate(runIdx);

		  CoordinateBuilderImpl<Grib1Record> timeBuilder = timeBuilders[run];
		  object time = timeBuilder.extract(gr);
		  int timeIdx = timeCoord.getIndex(time);

		  return new int[] {runIdx, timeIdx};
		}
	  }

	}

}
