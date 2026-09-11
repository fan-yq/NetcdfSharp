// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/nc2/TestUtils.java
// Only makeDummyGroup is ported; the read-data helpers need a NetcdfFile with iosp.


using NetcdfSharp.ucar.nc2;

namespace NetcdfSharp.ucar.nc2
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.TestUtils - static utilities for testing.
	/// </summary>
	public class TestUtils
	{
		public static Group makeDummyGroup()
		{
			return Group.builder().setName("").build();
		}
	}
}
