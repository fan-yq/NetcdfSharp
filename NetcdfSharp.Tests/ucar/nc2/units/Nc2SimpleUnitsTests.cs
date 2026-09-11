// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/nc2/units/TestSimpleUnits.java
// The testDates case only prints and is omitted. testIsDateUnit is ported.


using System;
using NetcdfSharp.ucar.nc2.units;
using Xunit;

namespace NetcdfSharp.ucar.nc2.units
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.units.TestSimpleUnits - SimpleUnit parsing, conversion and compatibility.
	/// </summary>
	public class Nc2SimpleUnitsTests
	{
		[Fact]
		public void TestUnits()
		{
			SimpleUnit su = SimpleUnit.factory("11 hPa");
			Assert.False(su is TimeUnit);
			Assert.Equal(1100.0, su.Value);
			Assert.Equal("Pa", su.UnitString);

			su = SimpleUnit.factory("11 km");
			Assert.False(su is TimeUnit);
			Assert.Equal(11000.0, su.Value);
			Assert.Equal("m", su.UnitString);

			SimpleUnit tu = SimpleUnit.factory("3 days");
			Assert.True(tu is TimeUnit);
			Assert.Equal("days", tu.UnitString);
			Assert.Equal(3.0, tu.Value);

			TryConvert();
		}

		private void TryConvert()
		{
			SimpleUnit t1 = SimpleUnit.factory("1 days");
			SimpleUnit t2 = SimpleUnit.factory("1 hour");
			double v = t1.convertTo(1.0, t2);
			Assert.Equal(24.0, v);
		}

		[Fact]
		public void TestCompatible()
		{
			SimpleUnit su = SimpleUnit.factory("11 hPa");
			Assert.True(su.isCompatible("mbar"));
			Assert.False(su.isCompatible("m"));
			Assert.False(su.isCompatible("sec"));
			Assert.False(su.isCompatible("3 days since 1930-07-27 12:00:00-05:00"));
		}

		[Fact]
		public void TestIsDateUnit()
		{
			const string timeUnitSince = "Year since 1970";
			Assert.True(SimpleUnit.isDateUnit(timeUnitSince));

			const string timeUnit = "Year";
			Assert.False(SimpleUnit.isDateUnit(timeUnit));
		}
	}
}
