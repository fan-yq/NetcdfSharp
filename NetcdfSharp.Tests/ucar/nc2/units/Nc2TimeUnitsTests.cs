// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/nc2/units/TestTimeUnits.java


using System;
using NetcdfSharp.ucar.nc2.units;
using Xunit;

namespace NetcdfSharp.ucar.nc2.units
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.units.TestTimeUnits - TimeUnit value/second conversion.
	/// </summary>
	public class Nc2TimeUnitsTests
	{
		private static bool NearlyEquals(double a, double b)
		{
			return Math.Abs(a - b) < 1.0e-8;
		}

		[Fact]
		public void TestTimes()
		{
			TimeUnit tu = new TimeUnit(3.0, "hours");

			string unitBefore = tu.UnitString;
			double secsBefore = tu.ValueInSeconds;

			tu.setValue(33.0);

			Assert.Equal(33.0, tu.Value);
			Assert.Equal(3600.0 * tu.Value, tu.ValueInSeconds);
			Assert.Equal(unitBefore, tu.UnitString);
			Assert.True(NearlyEquals(tu.ValueInSeconds, 11.0 * secsBefore));

			tu.ValueInSeconds = 3600.0;

			Assert.Equal(1.0, tu.Value);
			Assert.Equal(3600.0, tu.ValueInSeconds);
			Assert.Equal(unitBefore, tu.UnitString);
			Assert.True(NearlyEquals(3.0 * tu.ValueInSeconds, secsBefore));

			TimeUnit day = new TimeUnit(1.0, "day");
			double hoursInDay = day.convertTo(1.0, tu);
			Assert.Equal(24.0, hoursInDay);

			// note the value is ignored, only the base unit is used
			day = new TimeUnit(10.0, "day");
			hoursInDay = day.convertTo(1.0, tu);
			Assert.Equal(24.0, hoursInDay);

			hoursInDay = day.convertTo(10.0, tu);
			Assert.Equal(240.0, hoursInDay);
		}
	}
}
