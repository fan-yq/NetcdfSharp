// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/nc2/TestLongOffset.java
// The Java test ran Ncdump for debug output only (write module, out of scope); the open of the
// large-format file is the actual behavior under test.


using System;
using NetcdfSharp.ucar.nc2;
using NetcdfSharp.ucar.tests;
using Xunit;

namespace NetcdfSharp.ucar.nc2
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.TestLongOffset - reading a netCDF-3 file with long offsets ("large format").
	/// </summary>
	public class Nc2LongOffsetTests
	{
		[Fact]
		public void TestReadLongOffset()
		{
			using (NetcdfFile ncfile = MockDataSupport.TryOpen("longOffset.nc"))
			{
				if (ncfile == null)
				{
					return; // run generate_mock_data.py / copy testdata to create mock data
				}
				Assert.NotNull(ncfile.RootGroup.findDimension("time_step"));
				Assert.NotNull(ncfile.RootGroup.findDimension("num_nodes"));
				Assert.Equal(9, ncfile.Variables.Count);
			}
		}
	}
}
