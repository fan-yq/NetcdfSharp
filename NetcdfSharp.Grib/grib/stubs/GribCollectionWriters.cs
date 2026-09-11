// Minimal stubs for the ncx index writers (NetcdfSharp.ucar.nc2.grib.collection.Grib1CollectionWriter /
// Grib2CollectionWriter). Writing indices is out of scope ("Direction A"): only the Group
// structures needed by the builders and the writeIndex() entry point are provided.

using System.Collections.Generic;

namespace NetcdfSharp.ucar.nc2.grib.collection
{
	using CalendarDate = NetcdfSharp.ucar.nc2.time.CalendarDate;
	using CalendarDateRange = NetcdfSharp.ucar.nc2.time.CalendarDateRange;
	using Coordinate = NetcdfSharp.ucar.nc2.grib.coord.Coordinate;
	using CoordinateRuntime = NetcdfSharp.ucar.nc2.grib.coord.CoordinateRuntime;
	using Grib1Record = NetcdfSharp.ucar.nc2.grib.grib1.Grib1Record;
	using Grib1SectionGridDefinition = NetcdfSharp.ucar.nc2.grib.grib1.Grib1SectionGridDefinition;
	using Grib2Record = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Record;
	using Grib2SectionGridDefinition = NetcdfSharp.ucar.nc2.grib.grib2.Grib2SectionGridDefinition;
	using MCollection = NetcdfSharp.thredds.inventory.MCollection;
	using MFile = NetcdfSharp.thredds.inventory.MFile;

	public class Grib1CollectionWriter
	{
		internal Grib1CollectionWriter(MCollection dcm, NetcdfSharp.org.slf4j.Logger logger)
		{
		}

		internal virtual bool writeIndex(string name, MFile idxMFile, CoordinateRuntime masterRuntime, IList<Group> groups, IList<MFile> files, GribCollectionImmutable.GribType type, CalendarDateRange dateRange)
		{
			return true;
		}

		public class Group : GribCollectionBuilder.Group
		{
			internal readonly int hashCode;
			internal readonly CalendarDate runtime;
			internal IList<Grib1CollectionBuilder.VariableBag> gribVars;
			public IList<Coordinate> coords;
			public readonly ISet<long> runtimes = new HashSet<long>();
			public readonly IList<Grib1Record> records = new List<Grib1Record>();

			internal Group(Grib1SectionGridDefinition gdss, int hashCode, CalendarDate runtime)
			{
				this.hashCode = hashCode;
				this.runtime = runtime;
			}

			public virtual CalendarDate Runtime
			{
				get
				{
					return runtime;
				}
			}

			public virtual IList<Coordinate> Coordinates
			{
				get
				{
					return coords;
				}
			}

			public virtual ISet<long> CoordinateRuntimes
			{
				get
				{
					return runtimes;
				}
			}
		}
	}

	public class Grib2CollectionWriter
	{
		internal Grib2CollectionWriter(MCollection dcm, NetcdfSharp.org.slf4j.Logger logger)
		{
		}

		internal virtual bool writeIndex(string name, MFile idxMFile, CoordinateRuntime masterRuntime, IList<Group> groups, IList<MFile> files, GribCollectionImmutable.GribType type, CalendarDateRange dateRange)
		{
			return true;
		}

		public class Group : GribCollectionBuilder.Group
		{
			internal readonly int hashCode;
			internal readonly CalendarDate runtime;
			internal IList<Grib2CollectionBuilder.VariableBag> gribVars;
			public IList<Coordinate> coords;
			public readonly ISet<long> runtimes = new HashSet<long>();
			public readonly IList<Grib2Record> records = new List<Grib2Record>();

			internal Group(Grib2SectionGridDefinition gdss, int hashCode, CalendarDate runtime)
			{
				this.hashCode = hashCode;
				this.runtime = runtime;
			}

			public virtual CalendarDate Runtime
			{
				get
				{
					return runtime;
				}
			}

			public virtual IList<Coordinate> Coordinates
			{
				get
				{
					return coords;
				}
			}

			public virtual ISet<long> CoordinateRuntimes
			{
				get
				{
					return runtimes;
				}
			}
		}
	}
}
