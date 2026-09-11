// Ported from Java: netcdf-java grib/src/test/java/ucar/nc2/grib/grib1/tables/TestGrib1Customizer.java


using NetcdfSharp.ucar.nc2.grib.grib1.tables;
using Xunit;

namespace NetcdfSharp.ucar.nc2.grib.grib1.tables
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.grib.grib1.tables.TestGrib1Customizer.
	/// </summary>
	public class Grib1CustomizerTests
	{
		[Fact]
		public void TestStuff()
		{
			Grib1Customizer cust = new Grib1Customizer(0, null);
			string units = cust.getLevelUnits(110);
			Assert.NotNull(units);
		}
	}
}
