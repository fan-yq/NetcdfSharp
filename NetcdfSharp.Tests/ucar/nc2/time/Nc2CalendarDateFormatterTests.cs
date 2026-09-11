// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/nc2/time/TestCalendarDateFormatter.java
// The claimGood/claimBad matrix exercises the ISO date-string parser used by the read path.
// testStuff / shouldBeSameTime depend on NetcdfSharp.java.util.Date / SimpleDateFormat semantics that the
// port maps to .NET DateTime; those cases are omitted.


using System;
using NetcdfSharp.ucar.nc2.time;
using Xunit;

namespace NetcdfSharp.ucar.nc2.time
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.time.TestCalendarDateFormatter - ISO/udunits date-string parsing.
	/// </summary>
	public class Nc2CalendarDateFormatterTests
	{
		private static void ClaimGood(string s)
		{
			try
			{
				CalendarDate result = CalendarDateFormatter.isoStringToCalendarDate(null, s);
				Assert.NotNull(result);
			}
			catch (Exception)
			{
				Assert.Fail("should parse: " + s);
			}
		}

		private static void ClaimBad(string s)
		{
			try
			{
				CalendarDate result = CalendarDateFormatter.isoStringToCalendarDate(null, s);
				Assert.Fail("should NOT parse: " + s + " but got " + result);
			}
			catch (Exception)
			{
				// expected
			}
		}

		[Fact]
		public void TestBad()
		{
			ClaimBad("1143848700");
		}

		[Fact]
		public void TestW3cIso()
		{
			ClaimGood("1997");
			ClaimGood("1997-07");
			ClaimGood("1997-07-16");
			ClaimGood("1997-07-16T19:20+01:00");
			ClaimGood("1997-07-16T19:20:30+01:00");
		}

		[Fact]
		public void TestBasicFormatIso()
		{
			ClaimGood("19500101T000000Z"); // from https://github.com/Unidata/thredds/issues/772
			ClaimGood("199707");
			ClaimGood("19970716");
			ClaimGood("19970716T1920");
			ClaimGood("19970716T192030");
			ClaimGood("19970716T192030.1");
			ClaimGood("19970716T1920+01:00");
			ClaimGood("19970716T192030+0100");
			ClaimGood("19970716T192030+01");
			ClaimGood("19970716T192030.1+0100");
			ClaimGood("19970716T192030Z");
			ClaimGood("19970716T192030.1Z");
			// these should fail
			ClaimBad("19970716T192030.1UTC");
			ClaimBad("19501"); // fail because ambiguous
			ClaimBad("1950112"); // fail because ambiguous
			ClaimBad("19501120T121"); // fail because ambiguous
			ClaimBad("19501120T12151"); // fail because ambiguous
		}

		[Fact]
		public void TestChangeoverDate()
		{
			ClaimGood("1997-01-01");
			ClaimGood("1582-10-16");
			ClaimGood("1582-10-15");
			ClaimGood("1582-10-01");
			ClaimGood("1582-10-02");
			ClaimGood("1582-10-03");
			ClaimGood("1582-10-04");
		}

		[Fact]
		public void TestUdunits()
		{
			ClaimGood("1992-10-8 15:15:42.5 -6:00");
			ClaimGood("1992-10-8 15:15:42.5 +6");
			ClaimGood("1992-10-8 15:15:42.534");
			ClaimGood("1992-10-8 15:15:42");
			ClaimGood("1992-10-8 15:15");
			ClaimGood("1992-10-8 15");
			ClaimGood("1992-10-8T15");
			ClaimGood("1992-10-8");
			ClaimGood("199-10-8");
			ClaimGood("19-10-8");
			ClaimGood("1-10-8");
			ClaimGood("+1101-10-8");
			ClaimGood("-1101-10-8");
			ClaimGood("1992-10-8T7:00 -6:00");
			ClaimGood("1992-10-8T7:00 +6:00");
			ClaimGood("1992-10-8T7 -6:00");
			ClaimGood("1992-10-8T7 +6:00");
			ClaimGood("1992-10-8 7 -6:00");
			ClaimGood("1992-10-8 7 +6:00");
		}
	}
}
