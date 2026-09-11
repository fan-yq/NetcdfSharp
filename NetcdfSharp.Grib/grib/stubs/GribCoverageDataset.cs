// Minimal stubs for out-of-scope grib coverage types referenced by the grib module.

namespace NetcdfSharp.ucar.nc2.grib.coverage
{
	using CoverageCollection = NetcdfSharp.ucar.nc2.ft2.coverage.CoverageCollection;
	using GribCollectionImmutable = NetcdfSharp.ucar.nc2.grib.collection.GribCollectionImmutable;

	/// <summary>Stub for NetcdfSharp.ucar.nc2.grib.coverage.GribCoverageDataset (feature collection coverage layer, not ported).</summary>
	public class GribCoverageDataset
	{
		internal GribCoverageDataset(GribCollectionImmutable gc, GribCollectionImmutable.Dataset ds, GribCollectionImmutable.GroupGC group)
		{
		}

		public virtual CoverageCollection createCoverageCollection()
		{
			return null;
		}
	}
}
