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
namespace NetcdfSharp.ucar.nc2.grib.coord
{
	using CalendarDate = NetcdfSharp.ucar.nc2.time.CalendarDate;
	using CalendarPeriod = NetcdfSharp.ucar.nc2.time.CalendarPeriod;

	/// <summary>
	/// Create union of CoordinateTime2D's, and/or convert CoordinateTime2D into using orthogonal or regular representation .
	/// Will build orthogonal and regular if possible.
	/// Does not actually depend on T.
	/// 
	/// Used by CoordinatePartitionUnionizer to create union across partitions for one variable.
	/// Used by CoordinateSharer to create common time2D across variables in same partition, when isRuntimeUnion=true.
	/// 
	/// Used by CoordinateSharer to regularize/orthogonalize time2D coords.
	/// CoordinateTime2D.Builder uses general ctor, this will build orthogonal / regular variants
	/// 
	/// @author caron
	/// @since 11/22/2014
	/// </summary>
	internal class CoordinateTime2DUnionizer<T> : CoordinateBuilderImpl<T>
	{

	  private bool isTimeInterval;
	  private bool makeVals;
	  private CalendarPeriod timeUnit;
	  private int code;
	  internal NetcdfSharp.org.slf4j.Logger logger;
	  private SortedDictionary<long, CoordinateTimeAbstract> timeMap = new SortedDictionary<long, CoordinateTimeAbstract>();

	  internal CoordinateTime2DUnionizer(bool isTimeInterval, CalendarPeriod timeUnit, int code, bool makeVals, NetcdfSharp.org.slf4j.Logger logger)
	  {
		this.isTimeInterval = isTimeInterval;
		this.timeUnit = timeUnit;
		this.code = code;
		this.makeVals = makeVals;
		this.logger = logger != null ? logger : LoggerFactory.getLogger(typeof(CoordinateTime2DUnionizer<>));
	  }

	  public override void addAll(Coordinate coord)
	  {
		CoordinateTime2D coordT2D = (CoordinateTime2D) coord;
		for (int runIdx = 0; runIdx < coordT2D.Nruns; runIdx++)
		{
		  CoordinateTimeAbstract times = coordT2D.getTimeCoordinate(runIdx);
		  long runtime = coordT2D.getRuntime(runIdx);
		  timeMap[runtime] = times; // possible duplicate runtimes from different partitions
									   // later partitions will override LOOK could check how many times there are and
									   // choose larger
		}
	  }

	  public override object extract(T gr)
	  {
		throw new Exception();
	  }

	  // used when isRuntimeUnion=true
	  // set the list of runtime coordinates; add any that are not already present, and make an empty CoordinateTimeAbstract
	  // for it
	  internal virtual CoordinateRuntime RuntimeCoords
	  {
		  set
		  {
			for (int idx = 0; idx < value.Size; idx++)
			{
			  CalendarDate cd = value.getRuntimeDate(idx);
			  long runtime = value.getRuntime(idx);
			  CoordinateTimeAbstract time = timeMap[runtime];
			  if (time == null)
			  {
				time = isTimeInterval ? (NetcdfSharp.ucar.nc2.grib.coord.CoordinateTimeAbstract)new CoordinateTimeIntv(this.code, this.timeUnit, cd, new List<TimeCoordIntvValue>(0), null) : new CoordinateTime(this.code, this.timeUnit, cd, new List<int>(0), null);
				timeMap[runtime] = time;
			  }
			}
		  }
	  }

	  public override Coordinate makeCoordinate(IList<object> values)
	  {

		// the set of unique runtimes, sorted
		IList<long> runtimes = new List<long>();
		IList<Coordinate> times = new List<Coordinate>(); // the corresponding time coordinate for each runtime
		IList<CoordinateTime2D.Time2D> allVals = new List<CoordinateTime2D.Time2D>(); // optionally all Time2D coordinates
		foreach (long runtimeKey in timeMap.Keys)
		{
		  runtimes.Add(runtimeKey);
		  CoordinateTimeAbstract time = timeMap[runtimeKey];
		  times.Add(time);
		  if (makeVals)
		  {
			CalendarDate cd = CalendarDate.of(runtimeKey);
			foreach (object timeVal in time.Values)
			{
			  allVals.Add(isTimeInterval ? new CoordinateTime2D.Time2D(cd, null, (TimeCoordIntvValue) timeVal) : new CoordinateTime2D.Time2D(cd, (int?) timeVal, null));
			}
		  }
		}
		allVals.Sort();

		CoordinateRuntime runtime = new CoordinateRuntime(runtimes, timeUnit);

		CoordinateTimeAbstract maxCoord = testOrthogonal(timeMap.Values);
		if (maxCoord != null)
		{
		  return new CoordinateTime2D(code, timeUnit, allVals, runtime, maxCoord, times, null);
		}

		IList<Coordinate> regCoords = testIsRegular();
		if (regCoords.Count > 0)
		{
		  return new CoordinateTime2D(code, timeUnit, allVals, runtime, regCoords, times, null);
		}

		return new CoordinateTime2D(code, timeUnit, allVals, runtime, times, null);
	  }

	  // regular means that all the times for each offset from 0Z can be made into a single time coordinate (FMRC algo)
	  private IList<Coordinate> testIsRegular()
	  {

		// group time coords by offset hour
		IDictionary<int, IList<CoordinateTimeAbstract>> hourMap = new SortedDictionary<int, IList<CoordinateTimeAbstract>>();
		foreach (CoordinateTimeAbstract coord in timeMap.Values)
		{
		  CalendarDate runDate = coord.RefDate;
		  int hour = runDate.HourOfDay;
		  IList<CoordinateTimeAbstract> hg;
		  if (!hourMap.TryGetValue(hour, out hg))
		  {
			hg = new List<CoordinateTimeAbstract>();
			hourMap[hour] = hg;
		  }
		  hg.Add(coord);
		}

		// see if each offset hour is orthogonal
		IList<Coordinate> result = new List<Coordinate>();
		foreach (int hour in hourMap.Keys)
		{
		  IList<CoordinateTimeAbstract> hg = hourMap[hour];
		  Coordinate maxCoord = testOrthogonal(hg);
		  if (maxCoord == null)
		  {
			return ImmutableList<Coordinate>.of();
		  }
		  result.Add(maxCoord);
		}
		return result;
	  }

	  // check if the coordinate with maximum # values includes all of the time in the collection
	  // if so, we can store time2D as orthogonal
	  // LOOK not right I think, consider one coordinate every 6 hours, and one every 24; should not be merged.
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable static CoordinateTimeAbstract testOrthogonal(Collection<CoordinateTimeAbstract> times)
	  internal static CoordinateTimeAbstract testOrthogonal(ICollection<CoordinateTimeAbstract> times)
	  {
		CoordinateTimeAbstract maxCoord = null;
		ISet<object> result = new HashSet<object>(100);

		int max = 0;
		foreach (CoordinateTimeAbstract coord in times)
		{
		  if (max < coord.Size)
		  {
			maxCoord = coord;
			max = coord.Size;
		  }

		  result.UnionWith(coord.Values);
		}

		// is the set of all values the same as the component times?
		// this means we can use the "orthogonal representation" of the time2D
		int totalMax = result.Count;
		return totalMax == max ? maxCoord : null;
	  }

	} // Time2DUnionBuilder

}
