// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/nc2/time/TestCalendarDate.java
// and TestCalendarPeriod.java


using System;
using NetcdfSharp.ucar.nc2.time;
using Xunit;

namespace NetcdfSharp.ucar.nc2.time
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.time.TestCalendarDate + TestCalendarPeriod.
	/// </summary>
	public class Nc2TimeTests
	{
		[Fact]
		public void TestDateTimeFields()
		{
			CalendarDate cd;
			try
			{
				cd = CalendarDate.of(null, 1, 1, 1, 1, 1, 1);
			}
			catch (Exception)
			{
				Assert.Fail("should not throw");
			}
			try
			{
				cd = CalendarDate.of(null, 1, 0, 1, 1, 1, 1); // monthOfYear must be in [1,12]
				Assert.Fail("should throw");
			}
			catch (Exception)
			{
			}
			try
			{
				cd = CalendarDate.of(null, 1, 1, 0, 1, 1, 1); // dayOfMonth must be in [1,31]
				Assert.Fail("should throw");
			}
			catch (Exception)
			{
			}
			cd = CalendarDate.of(null, 1, 1, 1, 0, 1, 1);
			cd = CalendarDate.of(null, 1, 1, 1, 1, 0, 1);
			cd = CalendarDate.of(null, 1, 1, 1, 1, 1, 0);
		}

		[Fact]
		public void TestAddReturnsClosestDate()
		{
			// from https://github.com/jonescc
			string baseDate = "1950-01-01";
			double valueInMillisecs = 2025829799999.99977;
			string expectedResult = "2014-03-13T02:30:00Z";

			AssertAddReturnsExpectedDate(baseDate, valueInMillisecs, CalendarPeriod.Field.Millisec, expectedResult);
			AssertAddReturnsExpectedDate(baseDate, valueInMillisecs / CalendarDate.MILLISECS_IN_SECOND, CalendarPeriod.Field.Second, expectedResult);
			AssertAddReturnsExpectedDate(baseDate, valueInMillisecs / CalendarDate.MILLISECS_IN_MINUTE, CalendarPeriod.Field.Minute, expectedResult);
			AssertAddReturnsExpectedDate(baseDate, valueInMillisecs / CalendarDate.MILLISECS_IN_HOUR, CalendarPeriod.Field.Hour, expectedResult);
			AssertAddReturnsExpectedDate(baseDate, valueInMillisecs / CalendarDate.MILLISECS_IN_DAY, CalendarPeriod.Field.Day, expectedResult);
			AssertAddReturnsExpectedDate(baseDate, valueInMillisecs / CalendarDate.MILLISECS_IN_MONTH, CalendarPeriod.Field.Month, expectedResult);
			AssertAddReturnsExpectedDate(baseDate, valueInMillisecs / CalendarDate.MILLISECS_IN_YEAR, CalendarPeriod.Field.Year, expectedResult);
		}

		private void AssertAddReturnsExpectedDate(string baseDate, double value, CalendarPeriod.Field units, string expectedResult)
		{
			CalendarDate baseDt = CalendarDateFormatter.isoStringToCalendarDate(Calendar.gregorian, baseDate);
			CalendarDate result = baseDt.add(value, units);
			Assert.Equal(expectedResult, CalendarDateFormatter.toDateTimeStringISO(result));
		}

		[Fact]
		public void ShouldGetOffset()
		{
			CalendarPeriod cp = CalendarPeriod.of(1, CalendarPeriod.Field.Day);
			CalendarDate start = CalendarDate.parseUdunits(null, "3 days since 1970-01-01 12:00");
			CalendarDate end = CalendarDate.parseUdunits(null, "6 days since 1970-01-01 12:00");
			int offset = cp.getOffset(start, end);
			Assert.Equal(3, offset);
		}

		[Fact]
		public void ShouldGiveSymmetricResultsWhenRounding()
		{
			CalendarPeriod calendarPeriod = CalendarPeriod.of(1, CalendarPeriod.Field.Hour);
			CalendarDate start = CalendarDate.parseISOformat(null, "2000-01-01T00:00:00Z");

			CalendarDate end = CalendarDate.parseISOformat(null, "2000-01-01T00:10:00Z");
			Assert.Equal(-calendarPeriod.subtract(end, start), calendarPeriod.subtract(start, end));

			CalendarDate end2 = CalendarDate.parseISOformat(null, "2000-01-01T00:50:00Z");
			Assert.Equal(-calendarPeriod.subtract(end2, start), calendarPeriod.subtract(start, end2));

			CalendarDate end3 = CalendarDate.parseISOformat(null, "2000-01-01T00:30:00Z");
			Assert.Equal(-calendarPeriod.subtract(end3, start), calendarPeriod.subtract(start, end3));
		}
	}
}
