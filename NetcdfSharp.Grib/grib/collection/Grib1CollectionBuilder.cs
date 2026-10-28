// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1998-2026 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.collection
{
	using FeatureCollectionConfig = NetcdfSharp.thredds.featurecollection.FeatureCollectionConfig;
	using GribConfig = NetcdfSharp.thredds.featurecollection.FeatureCollectionConfig.GribConfig;
	using CollectionUpdateType = NetcdfSharp.thredds.inventory.CollectionUpdateType;
	using MCollection = NetcdfSharp.thredds.inventory.MCollection;
	using MFile = NetcdfSharp.thredds.inventory.MFile;
	using GribIndex = NetcdfSharp.ucar.nc2.grib.GribIndex;
	using GribIndexCache = NetcdfSharp.ucar.nc2.grib.GribIndexCache;
	using Coordinate = NetcdfSharp.ucar.nc2.grib.coord.Coordinate;
	using CoordinateEns = NetcdfSharp.ucar.nc2.grib.coord.CoordinateEns;
	using NetcdfSharp.ucar.nc2.grib.coord;
	using CoordinateRuntime = NetcdfSharp.ucar.nc2.grib.coord.CoordinateRuntime;
	using CoordinateTime2D = NetcdfSharp.ucar.nc2.grib.coord.CoordinateTime2D;
	using CoordinateVert = NetcdfSharp.ucar.nc2.grib.coord.CoordinateVert;
	using GribRecordStats = NetcdfSharp.ucar.nc2.grib.coord.GribRecordStats;
	using NetcdfSharp.ucar.nc2.grib.grib1;
	using Grib1Customizer = NetcdfSharp.ucar.nc2.grib.grib1.tables.Grib1Customizer;
	using Grib2Utils = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Utils;
	using CalendarDate = NetcdfSharp.ucar.nc2.time.CalendarDate;
	using CalendarDateRange = NetcdfSharp.ucar.nc2.time.CalendarDateRange;
	using CalendarPeriod = NetcdfSharp.ucar.nc2.time.CalendarPeriod;
	using NetcdfSharp.ucar.nc2.util;

	/// <summary>
	/// Builds indexes for collections of Grib1 files.
	/// May create GC or PC
	/// 
	/// @author John
	/// @since 2/5/14
	/// </summary>
	public class Grib1CollectionBuilder : GribCollectionBuilder
	{
	  private readonly FeatureCollectionConfig.GribConfig gribConfig;
	  private Grib1Customizer cust;

	  internal Grib1CollectionBuilder(string name, MCollection dcm, NetcdfSharp.org.slf4j.Logger logger) : base(true, name, dcm, logger)
	  {

		FeatureCollectionConfig config = (FeatureCollectionConfig) dcm.getAuxInfo(FeatureCollectionConfig.AUX_CONFIG);
		gribConfig = config.gribConfig;
	  }

	  // read all records in all files,
	  // divide into groups based on GDS hash and optionally the runtime
	  // each group has an arraylist of all records that belong to it.
	  // for each group, call rectilizer to derive the coordinates and variables
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public List<Grib1CollectionWriter.Group> makeGroups(List<NetcdfSharp.thredds.inventory.MFile> allFiles, boolean singleRuntime, Formatter errlog) throws NetcdfSharp.java.io.IOException
	  public override IList<GribCollectionBuilder.Group> makeGroups(IList<MFile> allFiles, bool singleRuntime, Formatter errlog)
	  {
		IDictionary<GroupAndRuntime, Grib1CollectionWriter.Group> gdsMap = new Dictionary<GroupAndRuntime, Grib1CollectionWriter.Group>();

		logger.debug("Grib1CollectionBuilder {}: makeGroups", name);
		int fileno = 0;
		GribRecordStats statsAll = new GribRecordStats(); // debugging

		logger.debug(" dcm={}", dcm);

		// place each record into its group
		using (CloseableIterator<MFile> iter = dcm.FileIterator)
		{ // not sorted
		  if (iter == null)
		  {
			return new List<GribCollectionBuilder.Group>(); // empty
		  }

		  while (iter.MoveNext())
		  {
			MFile mfile = iter.Current;
			Grib1Index index;
			try
			{
			  if (Grib.debugGbxIndexOnly)
			  {
				index = (Grib1Index) GribIndex.open(true, mfile);
				if (index == null)
				{
				  continue;
				}
			  }
			  else
			  {
				// here is where gbx9 files get recreated
				index = (Grib1Index) GribIndex.readOrCreateIndexFromSingleFile(true, mfile, CollectionUpdateType.test, logger);
			  }
			  allFiles.Add(mfile); // add on success

			}
			catch (IOException ioe)
			{
			  logger.error("Grib1CollectionBuilder " + name + " : reading/Creating gbx9 index for file " + mfile.Path + " failed", ioe);
			  continue;
			}
			if (index == null)
			{
			  logger.error("Grib1CollectionBuilder " + name + " : reading/Creating gbx9 index for file " + mfile.Path + " failed");
			  continue;
			}

			foreach (Grib1Record gr in index.Records)
			{ // we are using entire Grib1Record - likely this is the memory
														// bottleneck for how big a collection can handle
			  if (this.cust == null)
			  {
				cust = Grib1Customizer.factory(gr, null);
				cust.TimeUnitConverter = gribConfig.TimeUnitConverter;
			  }
			  if (filterIntervals(gr, gribConfig.intvFilter))
			  {
				statsAll.filter++;
				continue; // skip
			  }

			  gr.File = fileno; // each record tracks which file it belongs to
			  Grib1Gds gds = gr.GDS; // use GDS to group records
			  int hashCode = gribConfig.convertGdsHash(gds.GetHashCode()); // allow external config to muck with gdsHash. Why?
																		// because of error in encoding and we need exact
																		// hash matching
			  if (0 == hashCode)
			  {
				continue; // skip this group
			  }
			  // GdsHashObject gdsHashObject = new GdsHashObject(gr.getGDS(), hashOverride);

			  CalendarDate runtimeDate = gr.ReferenceDate;
			  long runtime = singleRuntime ? runtimeDate.Millis : 0; // separate Groups for each runtime, if
																		  // singleRuntime is true
			  GroupAndRuntime gar = new GroupAndRuntime(hashCode, runtime);
			  Grib1CollectionWriter.Group g;
			  gdsMap.TryGetValue(gar, out g);
			  if (g == null)
			  {
				g = new Grib1CollectionWriter.Group(gr.GDSsection, hashCode, runtimeDate);
				gdsMap[gar] = g;
			  }
			  g.records.Add(gr);
			  g.runtimes.Add(runtimeDate.Millis);
			}
			fileno++;
			statsAll.recordsTotal += index.Records.Count;
		  }
		}

		// rectilyze each group independently
		IList<Grib1CollectionWriter.Group> groups = new List<Grib1CollectionWriter.Group>(gdsMap.Values);
		foreach (Grib1CollectionWriter.Group g in groups)
		{
		  GribRecordStats stats = new GribRecordStats(); // debugging
		  Grib1Rectilyser rect = new Grib1Rectilyser(this, g.records, g.hashCode);
		  rect.make(gribConfig, stats, errlog);
		  g.gribVars = rect.gribvars;
		  g.coords = rect.coords;

		  statsAll.add(stats);
		}

		// debugging and validation
		if (logger.isDebugEnabled())
		{
		  logger.debug(statsAll.show());
		}

		return new List<GribCollectionBuilder.Group>(groups);
	  }

	  // true means remove
	  private bool filterIntervals(Grib1Record gr, FeatureCollectionConfig.GribIntvFilter intvFilter)
	  {
		Grib1SectionProductDefinition pdss = gr.PDSsection;
		Grib1ParamTime ptime = gr.getParamTime(cust);
		if (!ptime.IsInterval())
		{
		  return false;
		}

		int[] intv = ptime.Interval;
		if (intv == null)
		{
		  return false;
		}
		int haveLength = intv[1] - intv[0];

		// keep zero length intervals unless configured otherwise
		if (haveLength == 0 && intvFilter != null && intvFilter.ZeroExcluded)
		{
		  return true;
		}

		if (intvFilter != null && intvFilter.hasFilter())
		{
		  int center = pdss.Center;
		  int subcenter = pdss.SubCenter;
		  int version = pdss.TableVersion;
		  int param = pdss.ParameterNumber;
		  int id = (center << 8) + (subcenter << 16) + (version << 24) + param;

		  return intvFilter.filter(id, intv[0], intv[1], int.MinValue);
		}

		return false;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override protected boolean writeIndex(String name, String indexFilepath, NetcdfSharp.ucar.nc2.grib.coord.CoordinateRuntime masterRuntime, List<? extends GribCollectionBuilder.Group> groups, List<NetcdfSharp.thredds.inventory.MFile> files, NetcdfSharp.ucar.nc2.time.CalendarDateRange dateRange) throws NetcdfSharp.java.io.IOException
	  protected internal override bool writeIndex<T1>(string name, string indexFilepath, CoordinateRuntime masterRuntime, IList<T1> groups, IList<MFile> files, CalendarDateRange dateRange)
	  {
		Grib1CollectionWriter writer = new Grib1CollectionWriter(dcm, logger);
		IList<Grib1CollectionWriter.Group> groups2 = new List<Grib1CollectionWriter.Group>();
		foreach (object g in groups)
		{
		  groups2.Add((Grib1CollectionWriter.Group) g); // why copy ?
		}
		MFile indexFileInCache = GribIndexCache.getFileOrCache(indexFilepath);
		return writer.writeIndex(name, indexFileInCache, masterRuntime, groups2, files, type, dateRange);
	  }

	  public class VariableBag : IComparable<VariableBag>, IComparable
	  {
		internal readonly Grib1Record first;
		internal readonly Grib1Variable gv;

		internal readonly IList<Grib1Record> atomList = new List<Grib1Record>(100); // not sorted
		internal CoordinateND<Grib1Record> coordND;
		internal CalendarPeriod timeUnit;

		internal IList<int> coordIndex; // index into List<Coordinate>
		internal long pos;
		internal int length;

		internal VariableBag(Grib1Record first, Grib1Variable gv)
		{
		  this.first = first;
		  this.gv = gv;
		}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override public int compareTo(@Nonnull VariableBag o)
		public virtual int CompareTo(VariableBag o)
		{
		  return string.CompareOrdinal(Grib1Utils.extractParameterCode(first), Grib1Utils.extractParameterCode(o.first));
		}

		public virtual int CompareTo(object o)
		{
		  return CompareTo((VariableBag) o);
		}

		public override string ToString()
		{
		  return "VariableBag{" + ", variable=" + gv.makeVariableName(new FeatureCollectionConfig.GribConfig()) + ", coordND=" + coordND + ", timeUnit=" + timeUnit + ", coordIndex=" + coordIndex + ", pos=" + pos + ", length=" + length + '}';
		}
	  }

	  // for a single group, create multidimensional (rectangular) variables
	  private class Grib1Rectilyser
	  {
		  private readonly Grib1CollectionBuilder outerInstance;

		internal readonly int hashCode;
		internal readonly IList<Grib1Record> records;
		internal IList<VariableBag> gribvars;
		internal IList<Coordinate> coords;

		internal Grib1Rectilyser(Grib1CollectionBuilder outerInstance, IList<Grib1Record> records, int hashCode)
		{
			this.outerInstance = outerInstance;
		  this.records = records;
		  this.hashCode = hashCode;
		  // int gdsHash = gribConfig.convertGdsHash(gdsHashObject.hashCode());
		  // gdsHashOverride = (gdsHash == gdsHashObject.hashCode()) ? 0 : gdsHash;
		}

		public virtual void make(FeatureCollectionConfig.GribConfig config, GribRecordStats counter, Formatter info)
		{
		  CalendarPeriod userTimeUnit = config.userTimeUnit;

		  // assign each record to unique variable using cdmVariableHash()
		  IDictionary<Grib1Variable, VariableBag> vbHash = new Dictionary<Grib1Variable, VariableBag>(100);
		  foreach (Grib1Record gr in records)
		  {
			Grib1Variable cdmHash;
			try
			{
			  cdmHash = new Grib1Variable(outerInstance.cust, gr, hashCode, outerInstance.gribConfig.useTableVersion, outerInstance.gribConfig.intvMerge, outerInstance.gribConfig.useCenter);
			}
			catch (Exception t)
			{
			  outerInstance.logger.warn("Exception on record ", t);
			  continue; // keep going
			}
			VariableBag bag;
			vbHash.TryGetValue(cdmHash, out bag);
			if (bag == null)
			{
			  bag = new VariableBag(gr, cdmHash);
			  vbHash[cdmHash] = bag;
			}
			bag.atomList.Add(gr);
		  }
		  gribvars = new List<VariableBag>(vbHash.Values);
		  gribvars.Sort(); // make it deterministic by sorting

		  // create dense coordinates for each variable
		  foreach (VariableBag vb in gribvars)
		  {
			Grib1SectionProductDefinition pdss = vb.first.PDSsection;
			Grib1ParamTime ptime = vb.first.getParamTime(outerInstance.cust);

			int unit = outerInstance.cust.convertTimeUnit(pdss.TimeUnit);
			vb.timeUnit = userTimeUnit == null ? Grib2Utils.getCalendarPeriod(unit) : userTimeUnit; // so can override the
																									// code // ok for GRIB1
			CoordinateND<Grib1Record>.Builder<Grib1Record> coordNBuilder = new CoordinateND<Grib1Record>.Builder<Grib1Record>();

			bool isTimeInterval = ptime.IsInterval();
			/*
			 * if (isDense) { // time is runtime X time coord
			 * coordNBuilder.addBuilder(new CoordinateRuntime.Builder1(vb.timeUnit));
			 * if (isTimeInterval)
			 * coordNBuilder.addBuilder(new CoordinateTimeIntv.Builder1(cust, unit, vb.timeUnit, null)); // null refdate not
			 * ok
			 * else
			 * coordNBuilder.addBuilder(new CoordinateTime.Builder1(cust, pdss.getTimeUnit(), vb.timeUnit, null)); // null
			 * refdate not ok
			 * 
			 * } else {
			 */
			// time is kept as 2D coordinate, separate list of times for each runtime
			CoordinateTime2D.Builder1 builder2D = new CoordinateTime2D.Builder1(isTimeInterval, outerInstance.cust, vb.timeUnit, unit);
			coordNBuilder.addBuilder(builder2D);
			// }

			if (vb.first.PDSsection.Ensemble)
			{
			  coordNBuilder.addBuilder(new CoordinateEns.Builder1(outerInstance.cust, 0));
			}

			if (outerInstance.cust.isVerticalCoordinate(pdss.LevelType))
			{
			  coordNBuilder.addBuilder(new CoordinateVert.Builder1(outerInstance.cust, pdss.LevelType));
			}

			// populate the coordinates with the inventory of data
			foreach (Grib1Record gr in vb.atomList)
			{
			  coordNBuilder.addRecord(gr);
			}

			// done, build coordinates and sparse array indicating which records to use
			vb.coordND = coordNBuilder.finish(vb.atomList, info);
		  }

		  // make shared coordinates across variables
		  CoordinateSharer<Grib1Record> sharify = new CoordinateSharer<Grib1Record>(config.unionRuntimeCoord, outerInstance.logger);
		  foreach (VariableBag vb in gribvars)
		  {
			sharify.addCoords(vb.coordND.Coordinates);
		  }
		  sharify.finish();
		  this.coords = sharify.UnionCoords;

		  int tot_used = 0;
		  int tot_dups = 0;
		  int total = 0;

		  // redo the variables against the shared coordinates
		  foreach (VariableBag vb in gribvars)
		  {
			vb.coordND = sharify.reindexCoordND(vb.coordND);
			vb.coordIndex = sharify.reindex2shared(vb.coordND.Coordinates);
			tot_used += vb.coordND.SparseArray.countNotMissing();
			tot_dups += vb.coordND.SparseArray.getNdups();
			total += vb.coordND.SparseArray.getTotalSize();		  }

		  // track stats
		  counter.recordsUnique += tot_used;
		  counter.dups += tot_dups;
		  counter.vars += gribvars.Count;
		  counter.recordsTotal += total;
		}

		// debugging only
		public virtual void showInfo(Formatter f, Grib1Customizer cust1)
		{
		  // f.format("%nVariables%n");
		  // f.format("%n %3s %3s %3s%n", "time", "vert", "ens");
		  GribRecordStats all = new GribRecordStats();

		  foreach (VariableBag vb in gribvars)
		  {
			f.format("Variable %s (%d)%n", Grib1Iosp.makeVariableName(outerInstance.cust, outerInstance.gribConfig, vb.first.PDSsection), vb.GetHashCode());
			vb.coordND.showInfo(f, all);
			// f.format(" %3d %3d %3d %s records = %d density = %f hash=%d", vb.timeCoordIndex, vb.vertCoordIndex,
			// vb.ensCoordIndex,
			// vname, vb.atomList.size(), vb.recordMap.density(), vb.cdmHash);
			f.format("%n");
		  }
		  f.format("%n all= %s", all.show());
		}
	  }

	}

}
