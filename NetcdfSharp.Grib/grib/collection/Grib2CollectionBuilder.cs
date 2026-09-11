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
	using TimeUnitConverter = NetcdfSharp.thredds.featurecollection.TimeUnitConverter;
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
	using VertCoordType = NetcdfSharp.ucar.nc2.grib.coord.VertCoordType;
	using NetcdfSharp.ucar.nc2.grib.grib2;
	using Grib2Tables = NetcdfSharp.ucar.nc2.grib.grib2.table.Grib2Tables;
	using CalendarDate = NetcdfSharp.ucar.nc2.time.CalendarDate;
	using CalendarDateRange = NetcdfSharp.ucar.nc2.time.CalendarDateRange;
	using CalendarPeriod = NetcdfSharp.ucar.nc2.time.CalendarPeriod;
	using NetcdfSharp.ucar.nc2.util;

	/// <summary>
	/// Builds indexes for collections of Grib2 files.
	/// May create GC or PC
	/// 
	/// @author John
	/// @since 2/5/14
	/// </summary>
	internal class Grib2CollectionBuilder : GribCollectionBuilder
	{
	  private readonly FeatureCollectionConfig.GribConfig gribConfig;
	  private readonly TimeUnitConverter timeUnitConverter;
	  private Grib2Tables cust;

	  // LOOK prob name could be dcm.getCollectionName()
	  internal Grib2CollectionBuilder(string name, MCollection dcm, NetcdfSharp.org.slf4j.Logger logger) : base(false, name, dcm, logger)
	  {

		FeatureCollectionConfig config = (FeatureCollectionConfig) dcm.getAuxInfo(FeatureCollectionConfig.AUX_CONFIG);
		gribConfig = config.gribConfig;
		timeUnitConverter = gribConfig.TimeUnitConverter != null ? gribConfig.TimeUnitConverter : new TrivialTimeUnitConverter();
	  }

	  private class TrivialTimeUnitConverter : TimeUnitConverter
	  {
		public virtual int convertTimeUnit(int timeUnit)
		{
		  return timeUnit;
		}
	  }

	  // read all records in all files,
	  // divide into groups based on GDS hash and runtime
	  // each group has an arraylist of all records that belong to it.
	  // for each group, run rectlizer to derive the coordinates and variables
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public List<Grib2CollectionWriter.Group> makeGroups(List<NetcdfSharp.thredds.inventory.MFile> allFiles, boolean singleRuntime, Formatter errlog) throws NetcdfSharp.java.io.IOException
	  public override IList<GribCollectionBuilder.Group> makeGroups(IList<MFile> allFiles, bool singleRuntime, Formatter errlog)
	  {
		IDictionary<GroupAndRuntime, Grib2CollectionWriter.Group> gdsMap = new Dictionary<GroupAndRuntime, Grib2CollectionWriter.Group>();

		logger.debug("Grib2CollectionBuilder {}: makeGroups", name);
		int fileno = 0;
		GribRecordStats statsAll = new GribRecordStats(); // debugging

		logger.debug(" dcm={}", dcm);

		// place each record into its group
		int totalRecords = 0;
		using (CloseableIterator<MFile> iter = dcm.FileIterator)
		{ // not sorted
		  if (iter == null)
		  {
			return new List<GribCollectionBuilder.Group>(); // empty
		  }

		  while (iter.MoveNext())
		  {
			MFile mfile = iter.Current;
			Grib2Index index;

			try
			{
			  if (Grib.debugGbxIndexOnly)
			  {
				index = (Grib2Index) GribIndex.open(false, mfile);
			  }
			  else
			  {
				// this is where gbx9 files get recreated
				index = (Grib2Index) GribIndex.readOrCreateIndexFromSingleFile(false, mfile, CollectionUpdateType.test, logger);
			  }
			  allFiles.Add(mfile); // add on success

			}
			catch (IOException ioe)
			{
			  logger.error("Grib2CollectionBuilder " + name + " : reading/Creating gbx9 index for file " + mfile.Path + " failed", ioe);
			  continue;
			}
			if (index == null)
			{
			  logger.error("Grib2CollectionBuilder " + name + " : reading/Creating gbx9 index for file " + mfile.Path + " failed");
			  continue;
			}
			int n = index.NRecords;
			totalRecords += n;

			foreach (Grib2Record gr in index.Records)
			{ // we are using entire Grib2Record - memory limitations
			  if (this.cust == null)
			  {
				this.cust = Grib2Tables.factory(gr);
			  }
			  if (filterIntervals(gr, gribConfig.intvFilter))
			  {
				statsAll.filter++;
				continue; // skip
			  }

			  gr.File = fileno; // each record tracks which file it belongs to
			  Grib2Gds gds = gr.GDS; // use GDS to group records
			  int hashCode = gribConfig.convertGdsHash(gds.GetHashCode()); // allow external config to muck with gdsHash. Why?
																		// because of error in encoding and we need exact
																		// hash matching
			  if (0 == hashCode)
			  {
				continue; // skip this group
			  }
			  // GdsHashObject gdsHashObject = new GdsHashObject(gr.getGDS(), hashCode);

			  CalendarDate runtimeDate = gr.ReferenceDate;
			  long runtime = singleRuntime ? runtimeDate.Millis : 0; // separate Groups for each runtime, if
																		  // singleRuntime is true
			  GroupAndRuntime gar = new GroupAndRuntime(hashCode, runtime);
			  Grib2CollectionWriter.Group g = gdsMap[gar];
			  if (g == null)
			  {
				g = new Grib2CollectionWriter.Group(gr.GDSsection, hashCode, runtimeDate);
				gdsMap[gar] = g;
			  }
			  g.records.Add(gr);
			  g.runtimes.Add(runtimeDate.Millis);
			}
			fileno++;
			statsAll.recordsTotal += index.Records.Count;
		  }
		}

		if (totalRecords == 0)
		{
		  logger.warn("No records found in files. Check Grib1/Grib2 for collection {}. If wrong, delete gbx9.", name);
		  throw new System.InvalidOperationException("No records found in dataset " + name);
		}

		// rectilyze each group independently
		IList<Grib2CollectionWriter.Group> groups = new List<Grib2CollectionWriter.Group>(gdsMap.Values);
		foreach (Grib2CollectionWriter.Group g in groups)
		{
		  GribRecordStats stats = new GribRecordStats(); // debugging
		  Grib2Rectilyser rect = new Grib2Rectilyser(this, g.records, g.hashCode);
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

	  // true means discard
	  private bool filterIntervals(Grib2Record gr, FeatureCollectionConfig.GribIntvFilter intvFilter)
	  {
		// hack a whack - filter out records with unknown time units
		int timeUnit = gr.PDS.TimeUnit;
		if (Grib2Utils.getCalendarPeriod(timeUnit) == null)
		{
		  logger.info("Skip record with unknown time Unit= {}", timeUnit);
		  return true;
		}

		int[] intv = cust.getForecastTimeIntervalOffset(gr);
		if (intv == null)
		{
		  return false; // not an interval
		}
		int haveLength = intv[1] - intv[0];

		// discard zero length intervals if so configured
		if (haveLength == 0 && intvFilter != null && intvFilter.ZeroExcluded)
		{
		  return true;
		}

		// HACK
		if (intvFilter != null && intvFilter.hasFilter())
		{
		  int discipline = gr.Is.Discipline;
		  Grib2Pds pds = gr.PDS;
		  int category = pds.ParameterCategory;
		  int number = pds.ParameterNumber;
		  int id = (discipline << 16) + (category << 8) + number;

		  int prob = int.MinValue;
		  if (pds.Probability)
		  {
			Grib2Pds.PdsProbability pdsProb = (Grib2Pds.PdsProbability) pds;
			prob = (int)(1000 * pdsProb.ProbabilityUpperLimit);
		  }

		  // true means discard
		  return intvFilter.filter(id, intv[0], intv[1], prob);
		}

		return false;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override protected boolean writeIndex(String name, String indexFilepath, NetcdfSharp.ucar.nc2.grib.coord.CoordinateRuntime masterRuntime, List<? extends GribCollectionBuilder.Group> groups, List<NetcdfSharp.thredds.inventory.MFile> files, NetcdfSharp.ucar.nc2.time.CalendarDateRange dateRange) throws NetcdfSharp.java.io.IOException
	  protected internal override bool writeIndex<T1>(string name, string indexFilepath, CoordinateRuntime masterRuntime, IList<T1> groups, IList<MFile> files, CalendarDateRange dateRange)
	  {
		Grib2CollectionWriter writer = new Grib2CollectionWriter(dcm, logger);
		IList<Grib2CollectionWriter.Group> groups2 = new List<Grib2CollectionWriter.Group>();
		foreach (object g in groups)
		{
		  groups2.Add((Grib2CollectionWriter.Group) g); // copy to change GribCollectionBuilder.Group ->
		}
														// Grib2CollectionWriter.Group
		MFile indexFileInCache = GribIndexCache.getFileOrCache(indexFilepath);
		return writer.writeIndex(name, indexFileInCache, masterRuntime, groups2, files, type, dateRange);
	  }

	  internal class VariableBag : IComparable<VariableBag>, IComparable
	  {
		public readonly Grib2Record first;
		public readonly Grib2Variable gv;

		internal readonly IList<Grib2Record> atomList = new List<Grib2Record>(100); // not sorted
		internal CoordinateND<Grib2Record> coordND;
		internal CalendarPeriod timeUnit;

		internal IList<int> coordIndex;
		internal long pos;
		internal int length;

		internal VariableBag(Grib2Record first, Grib2Variable gv)
		{
		  this.first = first;
		  this.gv = gv;
		}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override public int compareTo(@Nonnull VariableBag o)
		public virtual int CompareTo(VariableBag o)
		{
		  return string.CompareOrdinal(Grib2Utils.getVariableName(first), Grib2Utils.getVariableName(o.first));
		}

		public virtual int CompareTo(object o)
		{
		  return CompareTo((VariableBag) o);
		}
	  }

	  private class Grib2Rectilyser
	  {
		  private readonly Grib2CollectionBuilder outerInstance;

		internal readonly int hashCode;
		internal readonly IList<Grib2Record> records;
		internal IList<VariableBag> gribvars;
		internal IList<Coordinate> coords;

		internal Grib2Rectilyser(Grib2CollectionBuilder outerInstance, IList<Grib2Record> records, int hashCode)
		{
			this.outerInstance = outerInstance;
		  this.records = records;
		  this.hashCode = hashCode;
		  /*
		   * int gdsHash = gribConfig.convertGdsHash(gdsHashObject.hashCode());
		   * gdsHashOverride = (gdsHash == gdsHashObject.hashCode()) ? 0 : gdsHash;
		   */
		}

		public virtual void make(FeatureCollectionConfig.GribConfig config, GribRecordStats counter, Formatter info)
		{
		  CalendarPeriod userTimeUnit = config.userTimeUnit;

		  // assign each record to unique variable using cdmVariableHash()
		  IDictionary<Grib2Variable, VariableBag> vbHash = new Dictionary<Grib2Variable, VariableBag>(100);
		  foreach (Grib2Record gr in records)
		  {
			Grib2Variable gv;
			try
			{
			  gv = new Grib2Variable(outerInstance.cust, gr, hashCode, outerInstance.gribConfig.intvMerge, outerInstance.gribConfig.useGenType);

			}
			catch (Exception t)
			{
			  outerInstance.logger.warn("Exception on record ", t);
			  continue; // keep going
			}
			VariableBag bag = vbHash[gv];
			if (bag == null)
			{
			  bag = new VariableBag(gr, gv);
			  vbHash[gv] = bag;
			}
			bag.atomList.Add(gr);
		  }
		  gribvars = new List<VariableBag>(vbHash.Values);
		  gribvars.Sort(); // make it deterministic by sorting

		  // create coordinates for each variable
		  foreach (VariableBag vb in gribvars)
		  {
			Grib2Pds pdsFirst = vb.first.PDS;
			int code = outerInstance.timeUnitConverter.convertTimeUnit(pdsFirst.TimeUnit);
			vb.timeUnit = userTimeUnit == null ? Grib2Utils.getCalendarPeriod(code) : userTimeUnit; // so can override the
																									// code in config
																									// "timeUnit"
			CoordinateND<Grib2Record>.Builder<Grib2Record> coordNBuilder = new CoordinateND<Grib2Record>.Builder<Grib2Record>();

			bool isTimeInterval = vb.first.PDS.isTimeInterval();
			CoordinateTime2D.Builder2 builder2D = new CoordinateTime2D.Builder2(isTimeInterval, outerInstance.cust, vb.timeUnit, code);
			coordNBuilder.addBuilder(builder2D);

			if (vb.first.PDS.Ensemble)
			{
			  coordNBuilder.addBuilder(new CoordinateEns.Builder2(0));
			}

			VertCoordType vertUnit = outerInstance.cust.getVertUnit(pdsFirst.LevelType1);
			if (vertUnit.VerticalCoordinate)
			{
			  coordNBuilder.addBuilder(new CoordinateVert.Builder2(pdsFirst.LevelType1, outerInstance.cust.getVertUnit(pdsFirst.LevelType1)));
			}

			// populate the coordinates with the inventory of data
			foreach (Grib2Record gr in vb.atomList)
			{
			  coordNBuilder.addRecord(gr);
			}

			// done, build coordinates and sparse array indicating which records to use
			vb.coordND = coordNBuilder.finish(vb.atomList, info);
		  }

		  // make shared coordinates across variables
		  CoordinateSharer<Grib2Record> sharify = new CoordinateSharer<Grib2Record>(config.unionRuntimeCoord, outerInstance.logger);
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
			total += vb.coordND.SparseArray.getTotalSize();
		  }

		  counter.recordsUnique += tot_used;
		  counter.dups += tot_dups;
		  counter.vars += gribvars.Count;
		  counter.recordsTotal += total;
		}

		public virtual void showInfo(Formatter f, Grib2Tables tables)
		{
		  GribRecordStats all = new GribRecordStats();

		  foreach (VariableBag vb in gribvars)
		  {
			f.format("Variable %s (%d)%n", tables.getVariableName(vb.first), vb.gv.GetHashCode());
			vb.coordND.showInfo(f, all);
			f.format("%n");
		  }
		  f.format("%n all= %s", all.show());
		}
	  }

	}

}
