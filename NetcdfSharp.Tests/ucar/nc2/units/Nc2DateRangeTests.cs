// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/nc2/units/TestDateRange.java


using System;
using NetcdfSharp.ucar.nc2.units;
using Xunit;

namespace NetcdfSharp.ucar.nc2.units
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.units.TestDateRange - DateRange moves when start or end is "present".
	/// </summary>
	public class Nc2DateRangeTests
	{
		[Fact]
		public void TestStartPresentAndDuration()
		{
			DateRange drStartIsPresent = new DateRange(new DateType("present", null, null), null, new TimeDuration("P7D"), null);
			checkValuesAfterDelay(drStartIsPresent);
		}

		[Fact]
		public void TestEndPresentAndDuration()
		{
			DateRange drEndIsPresent = new DateRange(null, new DateType("present", null, null), new TimeDuration("P7D"), null);
			checkValuesAfterDelay(drEndIsPresent);
		}

		private void checkValuesAfterDelay(DateRange dr)
		{
			DateTime startDate = dr.Start.getDate();
			DateTime endDate = dr.End.getDate();

			System.Threading.Thread.Sleep(10);

			DateTime startDate2 = dr.Start.getDate();
			DateTime endDate2 = dr.End.getDate();

			Assert.NotEqual(startDate, startDate2);
			Assert.NotEqual(endDate, endDate2);
		}
	}
}
