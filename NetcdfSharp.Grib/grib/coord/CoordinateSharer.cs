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
	using Grib2Record = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Record;

	/// <summary>
	/// Create shared coordinate variables across the variables in the same group.
	/// Use Coordinate.equals(), hashCode() to find unique coordinate variables. (so not merging similar)
	/// 
	/// Used by GribCollectionBuilder and GribPartitionBuilder
	/// This is a builder helper class.
	/// 
	/// @author John
	/// @since 1/4/14
	/// </summary>
	public class CoordinateSharer<T>
	{
	  internal bool isRuntimeUnion;
	  internal NetcdfSharp.org.slf4j.Logger logger;

	  /// <param name="isRuntimeUnion"> if true, make union of runtimes, otherwise keep separate runtimes if distinct </param>
	  public CoordinateSharer(bool isRuntimeUnion, NetcdfSharp.org.slf4j.Logger logger)
	  {
		this.isRuntimeUnion = false; // isRuntimeUnion; LOOK turn this off until we can fix it
		this.logger = logger;
	  }

	  internal ISet<Coordinate> runtimeSet = new HashSet<Coordinate>();
	  internal ISet<Coordinate> timeSet = new HashSet<Coordinate>();
	  internal ISet<Coordinate> timeIntvSet = new HashSet<Coordinate>();
	  internal ISet<Coordinate> vertSet = new HashSet<Coordinate>();
	  internal ISet<Coordinate> ensSet = new HashSet<Coordinate>();
	  internal ISet<Coordinate> time2DSet = new HashSet<Coordinate>(); // LOOK could we use a fuzzy compare here?

	  // result
	  internal IList<Coordinate> unionCoords = new List<Coordinate>(); // all the coordinates in the group
	  internal IDictionary<Coordinate, int> coordMap; // fast lookup of coordinate to list index

	  // isRuntimeUnion=true only
	  internal CoordinateRuntime.Builder2 runtimeAllBuilder;
	  internal CoordinateTime2DUnionizer<Grib2Record> time2DUnionizer;
	  internal CoordinateTime2DUnionizer<Grib2Record> timeIntv2DUnionizer;
	  internal CoordinateRuntime runtimeAll;
	  internal CoordinateTime2D time2Dall, timeIntv2Dall;

	  // add each variable's list of coordinate
	  public virtual void addCoords(IList<Coordinate> coords)
	  {
		CoordinateRuntime runtime = null;
		foreach (Coordinate coord in coords)
		{
		  switch (coord.getType().innerEnumValue)
		  {
			case Coordinate.Type.InnerEnum.runtime:
			  runtime = (CoordinateRuntime) coord;
			  if (isRuntimeUnion)
			  { // make union of all coords
				if (runtimeAllBuilder == null)
				{
				  runtimeAllBuilder = new CoordinateRuntime.Builder2(runtime.TimeUnits);
				}
				runtimeAllBuilder.addAll(coord);
			  }
			  else
			  {
				runtimeSet.Add(coord); // unique coordinates
			  }
			  break;

			case Coordinate.Type.InnerEnum.time:
			  timeSet.Add(coord);
			  break;

			case Coordinate.Type.InnerEnum.timeIntv:
			  timeIntvSet.Add(coord);
			  break;

			case Coordinate.Type.InnerEnum.time2D:
			  CoordinateTime2D time2D = (CoordinateTime2D) coord;
			  if (isRuntimeUnion)
			  { // make union of all coordsz
				if (time2D.TimeInterval)
				{
				  if (timeIntv2DUnionizer == null)
				  {
					timeIntv2DUnionizer = new CoordinateTime2DUnionizer<Grib2Record>(time2D.TimeInterval, time2D.TimeUnit, coord.Code, true, logger);
				  }
				  timeIntv2DUnionizer.addAll(time2D);
				}
				else
				{
				  if (time2DUnionizer == null)
				  {
					time2DUnionizer = new CoordinateTime2DUnionizer<Grib2Record>(time2D.TimeInterval, time2D.TimeUnit, coord.Code, true, logger);
				  }
				  time2DUnionizer.addAll(time2D);
				}
			  }
			  else
			  {
				time2DSet.Add(coord);
			  }
			  // debug
			  CoordinateRuntime runtimeFrom2D = time2D.RuntimeCoordinate;
			  if (!runtimeFrom2D.Equals(runtime))
			  {
				logger.warn("CoordinateSharer runtimes differ");
			  }
			  break;

			case Coordinate.Type.InnerEnum.vert:
			  vertSet.Add(coord);
			  break;

			case Coordinate.Type.InnerEnum.ens:
			  ensSet.Add(coord);
			  break;
		  }
		}
	  }

	  private IDictionary<Coordinate, CoordinateTime2D> swap = new Dictionary<Coordinate, CoordinateTime2D>(); // old, new coordinate

	  public virtual void finish()
	  {
		if (isRuntimeUnion)
		{ // have to redo any time2D with runtimeAll
		  runtimeAll = (CoordinateRuntime) runtimeAllBuilder.finish();
		  unionCoords.Add(runtimeAll);
		  if (time2DUnionizer != null)
		  {
			time2DUnionizer.RuntimeCoords = runtimeAll; // make sure theres a single runtime
			time2Dall = (CoordinateTime2D) time2DUnionizer.finish();
			unionCoords.Add(time2Dall);
		  }
		  if (timeIntv2DUnionizer != null)
		  {
			timeIntv2DUnionizer.RuntimeCoords = runtimeAll; // make sure theres a single runtime
			timeIntv2Dall = (CoordinateTime2D) timeIntv2DUnionizer.finish();
			unionCoords.Add(timeIntv2Dall);
		  }

		}
		else
		{ // not runtimeUnion

		  ((List<Coordinate>)unionCoords).AddRange(runtimeSet);

		  // CoordinateTime2D.Builder uses general ctor, CoordinateTime2DUnionizer will build orthogonal / regular variants
		  HashSet<CoordinateTime2D> coord2Dset = new HashSet<CoordinateTime2D>();
		  foreach (Coordinate coord in time2DSet)
		  {
			CoordinateTime2D coord2D = (CoordinateTime2D) coord;
			CoordinateTime2DUnionizer<Grib2Record> unionizer = new CoordinateTime2DUnionizer<Grib2Record>(coord2D.TimeInterval, coord2D.TimeUnit, coord2D.Code, true, logger);
			unionizer.addAll(coord2D);
			unionizer.finish();
			CoordinateTime2D result = (CoordinateTime2D) unionizer.Coordinate; // this tests for orthogonal and regular
			if (result.Orthogonal || result.Regular)
			{
			  if (!coord2Dset.Contains(result))
			  { // its possible that result is a duplicate CoordinateTime2D.
				unionCoords.Add(result); // use the new one
				coord2Dset.Add(result);
			  }
			  swap[coord2D] = result; // track old, new swap

			}
			else
			{
			  unionCoords.Add(coord2D); // use the old one
			  coord2Dset.Add(coord2D);
			}
		  }
		}

		((List<Coordinate>)unionCoords).AddRange(timeSet);
		((List<Coordinate>)unionCoords).AddRange(timeIntvSet);
		((List<Coordinate>)unionCoords).AddRange(vertSet);
		((List<Coordinate>)unionCoords).AddRange(ensSet);

		// fast lookup
		coordMap = new Dictionary<Coordinate, int>();
		for (int i = 0; i < this.unionCoords.Count; i++)
		{
		  coordMap[this.unionCoords[i]] = i;
		}
	  }

	  // this is the set of shared coordinates to be stored in the group
	  public virtual IList<Coordinate> UnionCoords
	  {
		  get
		  {
			return unionCoords;
		  }
	  }

	  /// <summary>
	  /// If using runtimeUnion, or time2D, you must reindex the CoordinateND
	  /// </summary>
	  /// <param name="prev"> previous CoordinateND </param>
	  /// <returns> new CoordinateND containing shared coordinates and sparseArray for the new coordinates
	  ///         or the prev CoordinateND if reindexing not needed. </returns>
	  public virtual CoordinateND<T> reindexCoordND(CoordinateND<T> prev)
	  {

		bool needReindex = false;
		foreach (Coordinate coord in prev.Coordinates)
		{
		  if (isRuntimeUnion && (coord.getType() == Coordinate.Type.runtime) && !coord.Equals(runtimeAll))
		  {
			needReindex = true;
		  }
		  CoordinateTime2D swapped;
		  if (swap.TryGetValue(coord, out swapped)) // time2D got swapped
		  {
			needReindex = true;
		  }
		}
		if (!needReindex)
		{
		  return prev;
		}

		// need to switch out the runtime and time2D
		IList<Coordinate> coords = new List<Coordinate>();
		foreach (Coordinate prevCoord in prev.Coordinates)
		{
		  if (isRuntimeUnion)
		  {
			if (prevCoord.getType() == Coordinate.Type.runtime)
			{
			  coords.Add(runtimeAll);
			}
			else if (prevCoord.getType() == Coordinate.Type.time2D)
			{
			  CoordinateTime2D time2D = (CoordinateTime2D) prevCoord;
			  if (time2D.TimeInterval)
			  {
				coords.Add(timeIntv2Dall);
			  }
			  else
			  {
				coords.Add(time2Dall);
			  }
			}
			else
			{
			  coords.Add(prevCoord);
			}

		  }
		  else
		  { // normal case - runTime2D may have gotten modified
			CoordinateTime2D newCoord;
			if (swap.TryGetValue(prevCoord, out newCoord))
			{
			  coords.Add(newCoord);
			}
			else
			{
			  coords.Add(prevCoord);
			}
		  }
		}

		return (new CoordinateND<T>.Builder<T>()).reindex(coords, prev);
	  }

	  // find indexes into unionCoords of a variable's coordinates
	  public virtual IList<int> reindex2shared(IList<Coordinate> prev)
	  {
		IList<int> result = new List<int>(prev.Count);

		foreach (Coordinate coord in prev)
		{
		  CoordinateTime2D swapCoord;
		  Coordinate coordEffective = coord;
		  if (swap.TryGetValue(coord, out swapCoord)) // time2D got swapped
		  {
			coordEffective = swapCoord;
		  }

		  int? idx = getIndexIntoShared(coordEffective); // index into unionCoords
		  if (idx == null)
		  {
			Formatter f = new Formatter();
			showInfo(f);
			f.format("%nprev:%n");
			foreach (Coordinate c in prev)
			{
			  f.format(" %d == (%s) %s%n", c.GetHashCode(), c, c.Name);
			}
			logger.error("CoordinateSharer cant find coordinate {} ({})", coordEffective.Name, f, new Exception());
		  }
		  else
		  {
			result.Add(idx.Value);
		  }
		}

		return result;
	  }

	  private int? getIndexIntoShared(Coordinate prev)
	  { // LOOK dont understand this, why cant you just use coordMap ??
		if (isRuntimeUnion)
		{
		  switch (prev.getType().innerEnumValue)
		  {
			case NetcdfSharp.ucar.nc2.grib.coord.Coordinate.Type.InnerEnum.runtime:
			  return coordMap[runtimeAll];
			case NetcdfSharp.ucar.nc2.grib.coord.Coordinate.Type.InnerEnum.time2D:
			  CoordinateTime2D time2D = (CoordinateTime2D) prev;
			  if (time2D.TimeInterval)
			  {
				return coordMap[timeIntv2Dall];
			  }
			  else
			  {
				return coordMap[time2Dall];
			  }
			default:
			  return coordMap[prev];
		  }
		}
		return coordMap[prev];
	  }

	  public virtual void showInfo(Formatter sb)
	  {
		sb.format("unionCoords:%n");
		foreach (Coordinate coord in this.unionCoords)
		{
		  sb.format(" %d == (%s) %s%n", coord.GetHashCode(), coord, coord.Name);
		}

		sb.format("%ncoordMap:%n");
		foreach (Coordinate coord in this.coordMap.Keys)
		{
		  sb.format(" %d == (%s) %s%n", coord.GetHashCode(), coord, coord.Name);
		}

		sb.format("%ntime2DBuilders:%n");
		foreach (Coordinate coord in this.time2DSet)
		{
		  sb.format(" %d == (%s) %s%n", coord.GetHashCode(), coord, coord.Name);
		}

		sb.format("%nswap:%n");
		foreach (KeyValuePair<Coordinate, CoordinateTime2D> entry in this.swap.SetOfKeyValuePairs())
		{
		  sb.format(" %d (%s) %s -> %d (%s) %s%n", entry.Key.GetHashCode(), entry.Key, entry.Key.Name, entry.Value.GetHashCode(), entry.Value, entry.Value.Name);
		}
	  }

	  ////////////////////////////////////////////////////////////////////////////////
	  // experimental

	  private const double smooshTolerence = .10;
	  private IList<RuntimeSmoosher> runtimes = new List<RuntimeSmoosher>();

	  private class RuntimeSmoosher
	  {
		internal CoordinateRuntime runtime;
		internal ISet<long> coordSet = new HashSet<long>();
		internal bool combined;

		internal RuntimeSmoosher(CoordinateRuntime runtime)
		{
		  this.runtime = runtime;
		  for (int i = 0; i < runtime.NCoords; i++)
		  {
			coordSet.Add(runtime.getRuntime(i));
		  }
		}

		// try to merge runtime whose values differ by < smooshTolerence
		public virtual bool nearlyEquals(RuntimeSmoosher that)
		{
		  HashSet<long> common = new HashSet<long>(this.coordSet);
		  common.IntersectWith(that.coordSet);
		  int total = Math.Min(this.runtime.Size, that.runtime.Size);

		  double nptsP = common.Count / (double) total;
		  return (nptsP < smooshTolerence);
		}
	  }
	}

}
