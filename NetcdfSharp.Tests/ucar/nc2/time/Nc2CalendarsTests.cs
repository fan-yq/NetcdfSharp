// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/nc2/time/TestCalendars.java
// Only the assertion-bearing cases are ported: test360bug and
// testWhitespaceCompatibilityWithUdunits (the other cases mostly print).


using System;
using NetcdfSharp.ucar.nc2.time;
using Xunit;

namespace NetcdfSharp.ucar.nc2.time
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.time.TestCalendars - non-standard calendar arithmetic.
	/// </summary>
	public class Nc2CalendarsTests
	{
		[Fact]
		public void Test360bug()
		{
			CalendarDateUnit unit = CalendarDateUnit.withCalendar(Calendar.uniform30day, "days since 2289-12-1");
			CalendarDate cd1 = unit.makeCalendarDate(25200.0); // 70 years = 2359-12-1
			CalendarDate cd2 = unit.makeCalendarDate(46800.0); // 130 = 2419-12-1

			Assert.Equal("2359-12-01T00:00:00Z", cd1.ToString());
			Assert.Equal("2419-12-01T00:00:00Z", cd2.ToString());
		}

		[Fact]
		public void TestWhitespaceCompatibilityWithUdunits()
		{
			string date = "2022-1-1";
			string time = "12:34:00Z";
			string[] udunitsWhitespace = new string[] { " ", "  ", "\t", "\n", "\r", "\f" };
			string[] calendarNames = new string[] { "gregorian", "proleptic_gregorian", "noleap", "all_leap", "uniform30day", "julian", "none" };

			foreach (string calName in calendarNames)
			{
				Calendar cal = Calendar.get(calName);

				// unit with time in ISO format to use for testing
				CalendarDateUnit isoFormatUnit = CalendarDateUnit.withCalendar(cal, string.Format("secs since {0}T{1}", date, time));

				foreach (string whitespace in udunitsWhitespace)
				{
					CalendarDateUnit testUnit = CalendarDateUnit.withCalendar(cal, string.Format("secs since {0}{1}{2}", date, whitespace, time));
					Assert.Equal(isoFormatUnit.BaseCalendarDate.Millis, testUnit.BaseCalendarDate.Millis);
				}
			}
		}
	}
}
