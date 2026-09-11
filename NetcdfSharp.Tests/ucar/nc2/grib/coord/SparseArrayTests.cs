// Ported from Java: netcdf-java grib/src/test/java/ucar/nc2/grib/coord/TestSparseArray.java


using System.Collections.Generic;
using NetcdfSharp.ucar.nc2.grib.coord;
using Xunit;

namespace NetcdfSharp.ucar.nc2.grib.coord
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.grib.coord.TestSparseArray - density calc.
	/// </summary>
	public class SparseArrayTests
	{
		[Fact]
		public void TestInfo()
		{
			int[] sizes = new int[] { 3, 10, 10 };
			int[] track = new int[3 * 10 * 10];
			List<short> list = new List<short>();
			for (int i = 0; i < 3 * 10 * 10; i++)
			{
				track[i] = i % 11 == 0 ? 0 : 1;
				list.Add((short)i);
			}

			SparseArray<short> sa = new SparseArray<short>(sizes, track, list, 0);

			Assert.Equal(0.906667, (double)sa.Density, 1e-5);
		}
	}
}
