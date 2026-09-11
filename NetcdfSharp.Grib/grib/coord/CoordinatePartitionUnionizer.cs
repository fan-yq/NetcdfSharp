// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System.Collections.Generic;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.grib.coord
{
	using FeatureCollectionConfig = NetcdfSharp.thredds.featurecollection.FeatureCollectionConfig;
	using GribCollectionMutable = NetcdfSharp.ucar.nc2.grib.collection.GribCollectionMutable;
	using PartitionCollectionMutable = NetcdfSharp.ucar.nc2.grib.collection.PartitionCollectionMutable;
	using CalendarDate = NetcdfSharp.ucar.nc2.time.CalendarDate;
	using Grib2Record = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Record;

	/// <summary>
	/// Create the overall coordinate across the same variable in different partitions
	/// The CoordinateBuilders create unique sets of Coordinates.
	/// The CoordinateND result is then the cross-product of those Coordinates.
	/// 
	/// Used by GribPartitionBuilder
	/// This is a builder helper class, the result is obtained from List<Coordinate> finish().
	/// 
	/// So if theres a lot of missing records in that cross-product, we may have the variable wrong (?),
	/// or our assumption that the data comprises a multidim array may be wrong
	/// 
	/// @author John
	/// @since 12/10/13
	/// </summary>
	public class CoordinatePartitionUnionizer
	{
	  private const bool debugPartitionErrors = true;

	  private readonly FeatureCollectionConfig.GribIntvFilter intvFilter;
	  private readonly NetcdfSharp.org.slf4j.Logger logger;
	  private readonly GribCollectionMutable.VariableIndex vi;
	  private readonly IDictionary<long, PartitionCollectionMutable.Partition> timeMap;

	  public CoordinatePartitionUnionizer(GribCollectionMutable.VariableIndex vi, FeatureCollectionConfig.GribIntvFilter intvFilter, NetcdfSharp.org.slf4j.Logger logger)
	  {
		this.vi = vi;
		this.intvFilter = intvFilter;
		this.logger = logger;
		timeMap = (debugPartitionErrors) ? new Dictionary<long, PartitionCollectionMutable.Partition>() : null;
	  }

	  internal IList<Coordinate> unionCoords = new List<Coordinate>();

	  internal CoordinateBuilder<Grib2Record> runtimeBuilder;
	  internal CoordinateBuilder<Grib2Record> timeBuilder;
	  internal CoordinateBuilder<Grib2Record> timeIntvBuilder;
	  internal CoordinateBuilder<Grib2Record> vertBuilder;
	  internal CoordinateBuilder<Grib2Record> ensBuilder;
	  internal CoordinateTime2DUnionizer<Grib2Record> time2DBuilder;

	  internal bool duplicateRuntimeMessage; // only one message per CoordinatePartitionUnionizer instance

	  public virtual void addCoords(IList<Coordinate> coords, PartitionCollectionMutable.Partition part)
	  {
		Coordinate runtime = null;
		foreach (Coordinate coord in coords)
		{
		  switch (coord.getType().innerEnumValue)
		  {
			case Coordinate.Type.InnerEnum.runtime:
			  CoordinateRuntime rtime = (CoordinateRuntime) coord;
			  if (runtimeBuilder == null)
			  {
				runtimeBuilder = new CoordinateRuntime.Builder2(rtime.TimeUnits);
			  }
			  runtimeBuilder.addAll(coord);
			  runtime = coord;
			  if (debugPartitionErrors && !duplicateRuntimeMessage && part != null)
			  {
				testDuplicateRuntime(rtime, part);
			  }
			  break;

			case Coordinate.Type.InnerEnum.time:
			  CoordinateTime time = (CoordinateTime) coord;
			  if (timeBuilder == null)
			  {
				timeBuilder = new CoordinateTime.Builder2(coord.Code, time.TimeUnit, time.RefDate);
			  }
			  timeBuilder.addAll(coord);
			  break;

			case Coordinate.Type.InnerEnum.timeIntv:
			  CoordinateTimeIntv timeIntv = (CoordinateTimeIntv) coord;
			  if (timeIntvBuilder == null)
			  {
				timeIntvBuilder = new CoordinateTimeIntv.Builder2(null, coord.Code, timeIntv.TimeUnit, timeIntv.RefDate);
			  }
			  timeIntvBuilder.addAll(intervalFilter((CoordinateTimeIntv) coord).Cast<object>().ToList());
			  break;

			case Coordinate.Type.InnerEnum.time2D:
			  CoordinateTime2D time2D = (CoordinateTime2D) coord;
			  if (time2DBuilder == null)
			  {
				time2DBuilder = new CoordinateTime2DUnionizer<Grib2Record>(time2D.TimeInterval, time2D.TimeUnit, coord.Code, false, logger);
			  }
			  time2DBuilder.addAll(time2D);
			  // debug
			  CoordinateRuntime runtimeFrom2D = time2D.RuntimeCoordinate;
			  if (!runtimeFrom2D.Equals(runtime))
			  {
				logger.warn("CoordinateUnionizer runtimes not equal");
			  }
			  break;

			case Coordinate.Type.InnerEnum.ens:
			  if (ensBuilder == null)
			  {
				ensBuilder = new CoordinateEns.Builder2(coord.Code);
			  }
			  ensBuilder.addAll(coord);
			  break;

			case Coordinate.Type.InnerEnum.vert:
			  CoordinateVert vertCoord = (CoordinateVert) coord;
			  if (vertBuilder == null)
			  {
				vertBuilder = new CoordinateVert.Builder2(coord.Code, vertCoord.VertUnit);
			  }
			  vertBuilder.addAll(coord);
			  break;
		  }
		}
	  }

	  private void testDuplicateRuntime(CoordinateRuntime runtime, PartitionCollectionMutable.Partition part)
	  {
		PartitionCollectionMutable.Partition shownPrevPart = null;
		for (int idx = 0; idx < runtime.NCoords; idx++)
		{ // possible duplicate runtimes from different partitions
		  long time = runtime.getRuntime(idx);
		  PartitionCollectionMutable.Partition prevPart = timeMap[time];
		  if (prevPart != null && prevPart != part && prevPart != shownPrevPart)
		  {
			logger.warn("Variable {} Runtime {} in part {} and partition {}", vi.id(), CalendarDate.of(time), prevPart.Name, part.Name);
			shownPrevPart = prevPart; // eliminate extra messages
			duplicateRuntimeMessage = true;
		  }
		  timeMap[time] = part;
		}
	  }

	  private IList<TimeCoordIntvValue> intervalFilter(CoordinateTimeIntv coord)
	  {
		if (intvFilter == null)
		{
		  return coord.TimeIntervals;
		}
		IList<TimeCoordIntvValue> result = new List<TimeCoordIntvValue>();
		foreach (TimeCoordIntvValue tinv in coord.TimeIntervals)
		{
		  if (!intvFilter.filter(vi.getVarid(), tinv.Bounds1, tinv.Bounds2, int.MinValue))
		  {
			result.Add(tinv);
		  }
		}
		return result;
	  }

	  public virtual IList<Coordinate> finish()
	  {
		if (runtimeBuilder != null)
		{
		  unionCoords.Add(runtimeBuilder.finish());
		}
		else
		{
		  logger.warn("CoordinateUnionizer missing runtime");
		}

		if (timeBuilder != null)
		{
		  unionCoords.Add(timeBuilder.finish());
		}
		else if (timeIntvBuilder != null)
		{
		  unionCoords.Add(timeIntvBuilder.finish());
		}
		else if (time2DBuilder != null)
		{
		  unionCoords.Add(time2DBuilder.finish());
		}
		else
		{
		  logger.warn("CoordinateUnionizer missing time");
		}

		if (ensBuilder != null) // ens must come before vert to preserve order
		{
		  unionCoords.Add(ensBuilder.finish());
		}
		if (vertBuilder != null)
		{
		  unionCoords.Add(vertBuilder.finish());
		}

		// result = new CoordinateND<>(unionCoords);
		return unionCoords;
	  }

	}

}
