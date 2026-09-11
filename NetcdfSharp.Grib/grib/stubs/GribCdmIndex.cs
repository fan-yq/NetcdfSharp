// Minimal stubs for the ncx index reading (NetcdfSharp.ucar.nc2.grib.collection.GribCdmIndex).
// Index creation/reading is out of scope ("Direction A"): the entry points return null / empty.

namespace NetcdfSharp.ucar.nc2.grib.collection
{
	using MFile = NetcdfSharp.thredds.inventory.MFile;
	using MFiles = NetcdfSharp.thredds.inventory.MFiles;
	using FeatureCollectionConfig = NetcdfSharp.thredds.featurecollection.FeatureCollectionConfig;
	using CollectionUpdateType = NetcdfSharp.thredds.inventory.CollectionUpdateType;
	using PartitionManager = NetcdfSharp.thredds.inventory.partition.PartitionManager;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;

	public class GribCdmIndex
	{
		public const string NCX_SUFFIX = ".ncx";

		public enum GribCollectionType
		{
			GRIB1, GRIB2, Partition1, Partition2
		}

		internal GribCdmIndex(NetcdfSharp.org.slf4j.Logger logger)
		{
		}

		public static GribCollectionType getType(RandomAccessFile raf)
		{
			return GribCollectionType.GRIB2;
		}

		public static GribCollectionImmutable openGribCollectionFromDataFile(bool isGrib1, MFile wantFile, CollectionUpdateType updateType, FeatureCollectionConfig gribConfig, Formatter errlog, NetcdfSharp.org.slf4j.Logger logger)
		{
			return null;
		}

		public static GribCollectionImmutable openGribCollectionFromRaf(RandomAccessFile raf, FeatureCollectionConfig gribConfig, CollectionUpdateType updateType, NetcdfSharp.org.slf4j.Logger logger)
		{
			return null;
		}

		public static MFile makeIndexFile(string name, MFile directory)
		{
			return directory.getChild(name + NCX_SUFFIX);
		}

		public static bool updateGribCollectionFromPCollection(bool isGrib1, PartitionManager part, CollectionUpdateType updateType, Formatter errlog, NetcdfSharp.org.slf4j.Logger logger)
		{
			return true;
		}

		internal virtual void readMFiles(string indexFile, System.Collections.Generic.IList<MFile> oldFiles)
		{
		}
	}
}
