// Minimal stubs for the gbx9 index classes (NetcdfSharp.ucar.nc2.grib.GribIndex / GribIndexCache).
// Index creation is out of scope ("Direction A"): the statics return null / empty stubs.
using System.Collections.Generic;

namespace NetcdfSharp.ucar.nc2.grib
{
	using MFile = NetcdfSharp.thredds.inventory.MFile;
	using MFiles = NetcdfSharp.thredds.inventory.MFiles;
	using CollectionUpdateType = NetcdfSharp.thredds.inventory.CollectionUpdateType;
	using ChangeChecker = NetcdfSharp.thredds.inventory.CollectionManager.ChangeChecker;
	using Grib1Record = NetcdfSharp.ucar.nc2.grib.grib1.Grib1Record;
	using Grib2Record = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Record;

	/// <summary>Stub for NetcdfSharp.ucar.nc2.grib.GribIndex.</summary>
	public class GribIndex
	{
		public static ChangeChecker ChangeChecker
		{
			get
			{
				return new ChangeChecker();
			}
		}

		public static GribIndex open(bool isGrib1, MFile mfile)
		{
			return null;
		}

		public static GribIndex readOrCreateIndexFromSingleFile(bool isGrib1, MFile mfile, CollectionUpdateType force, NetcdfSharp.org.slf4j.Logger logger)
		{
			return null;
		}
	}

	/// <summary>Stub for NetcdfSharp.ucar.nc2.grib.grib1.Grib1Index.</summary>
	public class Grib1Index : GribIndex
	{
		public virtual IList<Grib1Record> Records
		{
			get
			{
				return new List<Grib1Record>();
			}
		}
	}

	/// <summary>Stub for NetcdfSharp.ucar.nc2.grib.grib2.Grib2Index.</summary>
	public class Grib2Index : GribIndex
	{
		public const int ScanModeMissing = -1;

		public virtual IList<Grib2Record> Records
		{
			get
			{
				return new List<Grib2Record>();
			}
		}

		public virtual int NRecords
		{
			get
			{
				return 0;
			}
		}
	}

	/// <summary>Stub for NetcdfSharp.ucar.nc2.grib.GribIndexCache.</summary>
	public static class GribIndexCache
	{
		public static MFile getFileOrCache(string path)
		{
			return MFiles.create(path);
		}

		public static MFile getExistingFileOrCache(string path)
		{
			return MFiles.create(path);
		}
	}
}
