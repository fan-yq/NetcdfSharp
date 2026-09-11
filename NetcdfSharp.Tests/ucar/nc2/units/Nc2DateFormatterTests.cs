// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/nc2/units/TestDateFormatter.java


using System;
using NetcdfSharp.ucar.nc2.units;
using Xunit;

namespace NetcdfSharp.ucar.nc2.units
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.units.TestDateFormatter - ISO date parse/format round-trip.
	/// </summary>
	public class Nc2DateFormatterTests
	{
		private static void test(string text)
		{
			DateFormatter formatter = new DateFormatter();
			DateTime date = formatter.getISODate(text);
			string text2 = formatter.toDateTimeStringISO(date);
			DateTime date2 = formatter.getISODate(text2);
			Assert.Equal(date, date2);
		}

		[Fact]
		public void TestStuff()
		{
			test("2001-09-11T12:09:20");
			test("2001-09-11 12:10:12");
			test("2001-09-11T12:10");
			test("2001-09-11 12:01");
			test("2001-09-11");
		}
	}
}
