// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;

/*
 * Copyright (c) 1998-2026 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.collection
{
	using NetcdfSharp.ucar.nc2.util;
	using CalendarDate = NetcdfSharp.ucar.nc2.time.CalendarDate;
	using CalendarDateRange = NetcdfSharp.ucar.nc2.time.CalendarDateRange;
	using CollectionManager = NetcdfSharp.thredds.inventory.CollectionManager;
	using CollectionUpdateType = NetcdfSharp.thredds.inventory.CollectionUpdateType;
	using Coordinate = NetcdfSharp.ucar.nc2.grib.coord.Coordinate;
	using CoordinateRuntime = NetcdfSharp.ucar.nc2.grib.coord.CoordinateRuntime;
	using CoordinateTime2D = NetcdfSharp.ucar.nc2.grib.coord.CoordinateTime2D;
	using CoordinateTimeAbstract = NetcdfSharp.ucar.nc2.grib.coord.CoordinateTimeAbstract;
	using FeatureCollectionConfig = NetcdfSharp.thredds.featurecollection.FeatureCollectionConfig;
	using GribIndex = NetcdfSharp.ucar.nc2.grib.GribIndex;
	using GribIndexCache = NetcdfSharp.ucar.nc2.grib.GribIndexCache;
	using MCollection = NetcdfSharp.thredds.inventory.MCollection;
	using MFile = NetcdfSharp.thredds.inventory.MFile;
	using MFiles = NetcdfSharp.thredds.inventory.MFiles;
	using PartitionManager = NetcdfSharp.thredds.inventory.partition.PartitionManager;
	using PartitionManagerFromIndexList = NetcdfSharp.thredds.inventory.partition.PartitionManagerFromIndexList;
	using StringUtil2 = NetcdfSharp.ucar.unidata.util.StringUtil2;

	/// <summary>
	/// Superclass to build indexes for collections of Grib files.
	/// 
	/// @author caron
	/// @since 2/19/14
	/// </summary>
	public abstract class GribCollectionBuilder
	{

		protected internal readonly MCollection dcm;
		protected internal readonly NetcdfSharp.org.slf4j.Logger logger;
		protected internal readonly bool isGrib1;
		protected internal GribCollectionImmutable.GribType type;

		protected internal readonly string name; // collection name
		protected internal readonly MFile directory; // top directory

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
		// ORIGINAL LINE: protected abstract List<? extends Group> makeGroups(List<NetcdfSharp.thredds.inventory.MFile> allFiles, boolean singleRuntime, Formatter errlog) throws NetcdfSharp.java.io.IOException;
		public abstract IList<Group> makeGroups(IList<MFile> allFiles, bool singleRuntime, Formatter errlog);

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: protected abstract boolean writeIndex(String name, String indexFilepath, NetcdfSharp.ucar.nc2.grib.coord.CoordinateRuntime masterRuntime, List<? extends Group> groups, List<NetcdfSharp.thredds.inventory.MFile> files, NetcdfSharp.ucar.nc2.time.CalendarDateRange dateRange) throws NetcdfSharp.java.io.IOException;
		protected internal abstract bool writeIndex<T1>(string name, string indexFilepath, CoordinateRuntime masterRuntime, IList<T1> groups, IList<MFile> files, CalendarDateRange dateRange) where T1 : Group;

		internal GribCollectionBuilder(bool isGrib1, string name, MCollection dcm, NetcdfSharp.org.slf4j.Logger logger)
		{
			this.dcm = dcm;
			this.logger = logger;
			this.isGrib1 = isGrib1;

			this.name = StringUtil2.replace(name, ' ', "_");
			this.directory = MFiles.create(dcm.Root);
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: boolean updateNeeded(NetcdfSharp.thredds.inventory.CollectionUpdateType ff) throws NetcdfSharp.java.io.IOException
		internal virtual bool updateNeeded(CollectionUpdateType ff)
		{
			if (ff == CollectionUpdateType.never)
			{
				return false;
			}
			if (ff == CollectionUpdateType.always)
			{
				return true;
			}

			string indexFilename = dcm.getIndexFilename(GribCdmIndex.NCX_SUFFIX);
			MFile collectionIndexFile = GribIndexCache.getExistingFileOrCache(indexFilename);
			if (collectionIndexFile == null)
			{
				return true;
			}

			if (ff == CollectionUpdateType.nocheck)
			{
				return false;
			}

			return needsUpdate(ff, collectionIndexFile);
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private boolean needsUpdate(NetcdfSharp.thredds.inventory.CollectionUpdateType ff, NetcdfSharp.thredds.inventory.MFile collectionIndexFile) throws NetcdfSharp.java.io.IOException
		private bool needsUpdate(CollectionUpdateType ff, MFile collectionIndexFile)
		{
			long collectionLastModified = collectionIndexFile.LastModified;
			ISet<string> newFileSet = new HashSet<string>();

			CollectionManager.ChangeChecker cc = GribIndex.ChangeChecker;
			using (CloseableIterator<MFile> iter = dcm.FileIterator)
			{
				while (iter != null && iter.MoveNext())
				{
					MFile memberOfCollection = iter.Current;
					if (cc.hasChangedSince(memberOfCollection, collectionLastModified))
					{
						return true; // checks both data and gbx9 file
					}
					newFileSet.Add(memberOfCollection.Path);
				}
			}
			if (ff == CollectionUpdateType.testIndexOnly)
			{
				return false;
			}

			// now see if any files were deleted, by reading the index and comparing to the files there
			GribCdmIndex reader = new GribCdmIndex(logger);
			IList<MFile> oldFiles = new List<MFile>();
			reader.readMFiles(collectionIndexFile.Path, oldFiles);
			ISet<string> oldFileSet = new HashSet<string>();
			foreach (MFile oldFile in oldFiles)
			{
				if (!newFileSet.Contains(oldFile.Path))
				{
					return true; // got deleted - must recreate the index
				}
				oldFileSet.Add(oldFile.Path);
			}

			// now see if any files were added
			foreach (string newFilename in newFileSet)
			{
				if (!oldFileSet.Contains(newFilename))
				{
					return true; // got added - must recreate the index
				}
			}

			return false;
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////////

		// Throw exception if failure
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: boolean createIndex(NetcdfSharp.thredds.featurecollection.FeatureCollectionConfig.PartitionType ptype, Formatter errlog) throws NetcdfSharp.java.io.IOException
		internal virtual bool createIndex(FeatureCollectionConfig.PartitionType ptype, Formatter errlog)
		{
			if (ptype == FeatureCollectionConfig.PartitionType.all)
			{
				return createAllRuntimeCollections(errlog);
			}
			else
			{
				return createMultipleRuntimeCollections(errlog);
			}
		}

		// Throw exception if failure
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private boolean createMultipleRuntimeCollections(Formatter errlog) throws NetcdfSharp.java.io.IOException
		private bool createMultipleRuntimeCollections(Formatter errlog)
		{
			long start = DateTimeHelper.CurrentUnixTimeMillis();

			IList<MFile> files = new List<MFile>();
			// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
			// ORIGINAL LINE: List<? extends Group> groups = makeGroups(files, false, errlog);
			IList<Group> groups = makeGroups(files, false, errlog);
			IList<MFile> allFiles = files.AsReadOnly();
			if (allFiles.Count == 0)
			{
				throw new System.InvalidOperationException("No files in this collection =" + name + " topdir=" + dcm.Root);
			}
			if (groups.Count == 0)
			{
				throw new System.InvalidOperationException("No records in this collection =" + name + " topdir=" + dcm.Root);
			}

			// Create the master runtimes, classify the result
			CalendarDateRange calendarDateRangeAll = null;
			// boolean allTimesAreOne = true;
			bool allTimesAreUnique = true;
			ISet<long> allRuntimes = new HashSet<long>();
			foreach (Group g in groups)
			{
				allRuntimes.UnionWith(g.CoordinateRuntimes);
				foreach (Coordinate coord in g.Coordinates)
				{
					if (coord is CoordinateTime2D)
					{
						CoordinateTime2D coord2D = (CoordinateTime2D)coord;
						if (allTimesAreUnique)
						{
							allTimesAreUnique = coord2D.hasUniqueTimes();
						}
					}
					if (coord is CoordinateTimeAbstract)
					{
						CalendarDateRange calendarDateRange = ((CoordinateTimeAbstract)coord).makeCalendarDateRange(null);
						if (calendarDateRangeAll == null)
						{
							calendarDateRangeAll = calendarDateRange;
						}
						else
						{
							calendarDateRangeAll = calendarDateRangeAll.extend(calendarDateRange);
						}
					}
				}
			}
			IList<long> sortedList = new List<long>(allRuntimes);
			sortedList.Sort();
			if (sortedList.Count == 0)
			{
				throw new ArgumentException("No runtimes in this collection =" + name);
			}

			else if (sortedList.Count == 1)
			{
				this.type = GribCollectionImmutable.GribType.SRC;
			}
			else if (allTimesAreUnique)
			{
				this.type = GribCollectionImmutable.GribType.MRUTC;
			}
			else
			{
				this.type = GribCollectionImmutable.GribType.MRC;
			}

			CoordinateRuntime masterRuntimes = new CoordinateRuntime(sortedList, null);
			MFile indexFileForRuntime = GribCollectionMutable.makeIndexMFile(this.name, directory);
			bool ok = writeIndex(this.name, indexFileForRuntime.Path, masterRuntimes, groups, allFiles, calendarDateRangeAll);

			/*
			 * if (this.type == GribCollectionImmutable.Type.MRC) {
			 * GribBestDatasetBuilder.makeDatasetBest();
			 * }
			 */

			long took = DateTimeHelper.CurrentUnixTimeMillis() - start;
			logger.debug("That took {} msecs", took);
			return ok;
		}

		// PartitionType = all; not currently used but leave it here in case it needs to be revived
		// creates separate collection and index for each runtime.
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private boolean createAllRuntimeCollections(Formatter errlog) throws NetcdfSharp.java.io.IOException
		private bool createAllRuntimeCollections(Formatter errlog)
		{
			long start = DateTimeHelper.CurrentUnixTimeMillis();
			this.type = GribCollectionImmutable.GribType.SRC;
			bool ok = true;

			IList<MFile> files = new List<MFile>();
			// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
			// ORIGINAL LINE: List<? extends Group> groups = makeGroups(files, true, errlog);
			IList<Group> groups = makeGroups(files, true, errlog);
			IList<MFile> allFiles = files.AsReadOnly();

			// gather into collections with a single runtime
			IDictionary<long, IList<Group>> runGroups = new Dictionary<long, IList<Group>>();
			foreach (Group g in groups)
			{
				if (!runGroups.TryGetValue(g.Runtime.Millis, out IList<Group> runGroup))
				{
					runGroup = new List<Group>();
					runGroups[g.Runtime.Millis] = runGroup;
				}
				runGroup.Add(g);
			}

			// write each rungroup separately
			bool multipleRuntimes = runGroups.Values.Count > 1;
			IList<MFile> partitions = new List<MFile>();
			foreach (IList<Group> runGroupList in runGroups.Values)
			{
				Group g = runGroupList[0];
				// if multiple Runtimes, we will write a partition. otherwise, we need to use the standard name (without runtime)
				// so we know the filename from the collection
				string gcname = multipleRuntimes ? GribCollectionMutable.makeName(this.name, g.Runtime) : this.name;
				MFile indexFileForRuntime = GribCollectionMutable.makeIndexMFile(gcname, directory); // not using disk cache LOOK
																									 // why ?
				partitions.Add(indexFileForRuntime);

				// create the master runtimes, consisting of the single runtime
				IList<long> runtimes = new List<long>(1);
				runtimes.Add(g.Runtime.Millis);
				CoordinateRuntime masterRuntimes = new CoordinateRuntime(runtimes, null);

				CalendarDateRange calendarDateRangeAll = null;
				foreach (Coordinate coord in g.Coordinates)
				{
					if (coord is CoordinateTimeAbstract)
					{
						CalendarDateRange calendarDateRange = ((CoordinateTimeAbstract)coord).makeCalendarDateRange(null);
						if (calendarDateRangeAll == null)
						{
							calendarDateRangeAll = calendarDateRange;
						}
						else
						{
							calendarDateRangeAll = calendarDateRangeAll.extend(calendarDateRange);
						}
					}
				}
				Debug.Assert(calendarDateRangeAll != null);

				// for each Group write an index file
				ok &= writeIndex(gcname, indexFileForRuntime.Path, masterRuntimes, runGroupList, allFiles, calendarDateRangeAll);
				logger.info("GribCollectionBuilder write {} ok={}", indexFileForRuntime.Path, ok);
			}

			// if theres more than one runtime, create a partition collection to collect all the runtimes together
			if (multipleRuntimes)
			{
				var sortedPartitions = partitions.OrderBy(m => m.getName(), StringComparer.Ordinal).ToList();
				partitions = sortedPartitions; // ??
				PartitionManager part = new PartitionManagerFromIndexList(dcm, partitions, logger);
				part.putAuxInfo(FeatureCollectionConfig.AUX_CONFIG, dcm.getAuxInfo(FeatureCollectionConfig.AUX_CONFIG));
				ok &= GribCdmIndex.updateGribCollectionFromPCollection(isGrib1, part, CollectionUpdateType.always, errlog, logger);
			}

			long took = DateTimeHelper.CurrentUnixTimeMillis() - start;
			logger.debug("That took {} msecs", took);
			return ok;
		}

		public interface Group
		{
			CalendarDate Runtime { get; }

			IList<Coordinate> Coordinates { get; }

			ISet<long> CoordinateRuntimes { get; }
		}

		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @Immutable protected static class GroupAndRuntime
		protected internal class GroupAndRuntime
		{
			internal readonly int hashCode;
			internal readonly long runtime;

			internal GroupAndRuntime(int hashCode, long runtime)
			{
				this.hashCode = hashCode;
				this.runtime = runtime;
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

				GroupAndRuntime that = (GroupAndRuntime)o;
				if (hashCode != that.hashCode)
				{
					return false;
				}
				return runtime == that.runtime;
			}

			public override int GetHashCode()
			{
				int result = hashCode;
				result = 31 * result + (int)(runtime ^ (runtime >>> 32));
				return result;
			}
		}
	}

}
