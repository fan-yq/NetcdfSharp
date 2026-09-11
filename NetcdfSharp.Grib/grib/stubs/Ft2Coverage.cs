// Minimal stubs for out-of-scope NetcdfSharp.ucar.nc2.ft2.coverage types referenced by the grib module.
using System.Collections;
using System.Collections.Generic;

namespace NetcdfSharp.ucar.nc2.ft2.coverage
{
	using CalendarDate = NetcdfSharp.ucar.nc2.time.CalendarDate;
	using RangeIterator = NetcdfSharp.ucar.ma2.RangeIterator;

	/// <summary>Stub for NetcdfSharp.ucar.nc2.ft2.coverage.CoverageCollection (feature collection layer, not ported).</summary>
	public class CoverageCollection
	{
	}

	/// <summary>Stub for NetcdfSharp.ucar.nc2.ft2.coverage.SubsetParams.</summary>
	public class SubsetParams
	{
		public virtual CalendarDate getRunTime()
		{
			return null;
		}

		public virtual double getTimeOffset()
		{
			return 0;
		}

		public virtual double[] getTimeOffsetIntv()
		{
			return null;
		}

		public virtual double getVertCoord()
		{
			return 0;
		}

		public virtual double[] getVertCoordIntv()
		{
			return null;
		}

		public virtual double getEnsCoord()
		{
			return 0;
		}
	}

	/// <summary>Stub for NetcdfSharp.ucar.nc2.ft2.coverage.CoordsSet (iterable set of subset params).</summary>
	public class CoordsSet : IEnumerable<SubsetParams>
	{
		public virtual int[] getShape(RangeIterator yRange, RangeIterator xRange)
		{
			return new int[0];
		}

		public virtual IEnumerator<SubsetParams> GetEnumerator()
		{
			return new List<SubsetParams>().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
