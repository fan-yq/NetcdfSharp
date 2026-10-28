// Scan-backed implementation of the gbx9 index classes (NetcdfSharp.ucar.nc2.grib.GribIndex / GribIndexCache).
// We do not persist or read .gbx9 files; instead a single data file is scanned on demand and the
// resulting records are handed to the collection builders.
using System.Collections.Generic;

namespace NetcdfSharp.ucar.nc2.grib
{
	using MFile = NetcdfSharp.thredds.inventory.MFile;
	using MFiles = NetcdfSharp.thredds.inventory.MFiles;
	using CollectionUpdateType = NetcdfSharp.thredds.inventory.CollectionUpdateType;
	using ChangeChecker = NetcdfSharp.thredds.inventory.CollectionManager.ChangeChecker;
	using Grib1Record = NetcdfSharp.ucar.nc2.grib.grib1.Grib1Record;
	using Grib1RecordScanner = NetcdfSharp.ucar.nc2.grib.grib1.Grib1RecordScanner;
	using Grib2Record = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Record;
	using Grib2RecordScanner = NetcdfSharp.ucar.nc2.grib.grib2.Grib2RecordScanner;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;

	/// <summary>
	/// Scan-based replacement for NetcdfSharp.ucar.nc2.grib.GribIndex.
	/// Produces the record inventory for a single GRIB file without an index file.
	/// </summary>
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
			return scan(isGrib1, mfile);
		}

		public static GribIndex readOrCreateIndexFromSingleFile(bool isGrib1, MFile mfile, CollectionUpdateType force, NetcdfSharp.org.slf4j.Logger logger)
		{
			return scan(isGrib1, mfile);
		}

		private static GribIndex scan(bool isGrib1, MFile mfile)
		{
			if (mfile == null || !mfile.exists())
			{
				return null;
			}

			RandomAccessFile raf = new RandomAccessFile(mfile.Path, "r");
			try
			{
				raf.order(RandomAccessFile.BIG_ENDIAN);
				raf.seek(0);

				if (isGrib1)
				{
					Grib1RecordScanner scanner = new Grib1RecordScanner(raf);
					IList<Grib1Record> records = new List<Grib1Record>();
					while (scanner.hasNext())
					{
						Grib1Record gr = scanner.next();
						if (gr == null)
						{
							break;
						}
						records.Add(gr);
					}
					return new Grib1Index(records);
				}

				Grib2RecordScanner scanner2 = new Grib2RecordScanner(raf);
				IList<Grib2Record> records2 = new List<Grib2Record>();
				while (scanner2.hasNext())
				{
					Grib2Record gr = scanner2.next();
					if (gr == null)
					{
						break;
					}
					records2.Add(gr);
				}
				return new Grib2Index(records2);
			}
			finally
			{
				raf.close();
			}
		}
	}

	/// <summary>Scan-backed GRIB1 record inventory.</summary>
	public class Grib1Index : GribIndex
	{
		private readonly IList<Grib1Record> records;

		internal Grib1Index(IList<Grib1Record> records)
		{
			this.records = records ?? new List<Grib1Record>();
		}

		public virtual IList<Grib1Record> Records
		{
			get
			{
				return records;
			}
		}
	}

	/// <summary>Scan-backed GRIB2 record inventory.</summary>
	public class Grib2Index : GribIndex
	{
		public const int ScanModeMissing = -1;

		private readonly IList<Grib2Record> records;

		internal Grib2Index(IList<Grib2Record> records)
		{
			this.records = records ?? new List<Grib2Record>();
		}

		public virtual IList<Grib2Record> Records
		{
			get
			{
				return records;
			}
		}

		public virtual int NRecords
		{
			get
			{
				return records.Count;
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
