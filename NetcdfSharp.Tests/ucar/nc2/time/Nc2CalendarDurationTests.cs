// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/nc2/time/TestCalendarDuration.java
// The Java test asserts Period.toString() (ISO-8601 like "PT1S"). The C# joda shim's Period has
// no ISO toString, so the assertion is on the equivalent Value + field-type instead.


using System;
using NetcdfSharp.org.joda.time;
using NetcdfSharp.ucar.nc2.time;
using Xunit;

namespace NetcdfSharp.ucar.nc2.time
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.time.TestCalendarDuration - CalendarDuration unit-to-period conversion.
	/// </summary>
	public class Nc2CalendarDurationTests
	{
		private static void test(string unit, PeriodType expectType, long expectValue)
		{
			Period jp = CalendarDuration.convertToPeriod(1, unit);
			Assert.NotNull(jp);
			Assert.Equal(expectType.Name, jp.Type.Name);
			Assert.Equal(expectValue, jp.Value);
		}

		[Fact]
		public void TestStuff()
		{
			test("sec", PeriodType.seconds(), 1);
			test("secs", PeriodType.seconds(), 1);
			test("minute", PeriodType.minutes(), 1);
			test("minutes", PeriodType.minutes(), 1);
			test("hour", PeriodType.hours(), 1);
			test("hours", PeriodType.hours(), 1);
			test("hr", PeriodType.hours(), 1);
			test("day", PeriodType.days(), 1);
			test("days", PeriodType.days(), 1);
			test("week", PeriodType.weeks(), 1);
			test("weeks", PeriodType.weeks(), 1);
			test("month", PeriodType.months(), 1);
			test("months", PeriodType.months(), 1);
			test("year", PeriodType.years(), 1);
			test("years", PeriodType.years(), 1);
		}
	}
}
