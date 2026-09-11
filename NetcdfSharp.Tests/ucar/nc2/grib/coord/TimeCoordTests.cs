// Ported from Java: netcdf-java grib/src/test/java/ucar/nc2/grib/coord/TestTimeCoord.java


using NetcdfSharp.ucar.nc2.grib.coord;
using NetcdfSharp.ucar.nc2.time;
using Xunit;

namespace NetcdfSharp.ucar.nc2.grib.coord
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.grib.coord.TestTimeCoord - time interval reference-date conversion.
	/// </summary>
	public class TimeCoordTests
	{
		[Fact]
		public void TestTinvDate()
		{
			CalendarDate start = CalendarDate.of(1269820799000L);
			CalendarDate end = CalendarDate.of(1269824399000L);
			TimeCoordIntvDateValue tinvDate = new TimeCoordIntvDateValue(start, end);
			Assert.Equal("(2010-03-28T23:59:59Z,2010-03-29T00:59:59Z)", tinvDate.ToString());

			CalendarDate refDate = CalendarDate.of(1269820800000L);
			CalendarPeriod timeUnit = CalendarPeriod.of("Hour");

			TimeCoordIntvValue tinv = tinvDate.convertReferenceDate(refDate, timeUnit);
			Assert.Equal("2010-03-29T00:00:00Z", refDate.ToString());
		}

		[Fact]
		public void ShouldPreserveTimeIntervalLengthWithStartAfterRefDate()
		{
			CalendarDate start = CalendarDate.parseISOformat(null, "2022-08-16T01:00:00Z");
			CalendarDate end = CalendarDate.parseISOformat(null, "2022-08-16T12:00:00Z");
			TimeCoordIntvDateValue timeCoordIntvDateValue = new TimeCoordIntvDateValue(start, end);
			CalendarPeriod timeUnit = CalendarPeriod.of("Hour");

			CalendarDate refDate = CalendarDate.parseISOformat(null, "2022-08-16T00:30:00Z");
			TimeCoordIntvValue timeCoordIntvValue = timeCoordIntvDateValue.convertReferenceDate(refDate, timeUnit);
			Assert.Equal(1, timeCoordIntvValue.Bounds1);
			Assert.Equal(12, timeCoordIntvValue.Bounds2);
			Assert.Equal(11, timeCoordIntvValue.IntervalSize);

			CalendarDate refDate2 = CalendarDate.parseISOformat(null, "2022-08-16T00:40:00Z");
			TimeCoordIntvValue timeCoordIntvValue2 = timeCoordIntvDateValue.convertReferenceDate(refDate2, timeUnit);
			Assert.Equal(0, timeCoordIntvValue2.Bounds1);
			Assert.Equal(11, timeCoordIntvValue2.Bounds2);
			Assert.Equal(11, timeCoordIntvValue2.IntervalSize);

			CalendarDate refDate3 = CalendarDate.parseISOformat(null, "2022-08-16T00:20:00Z");
			TimeCoordIntvValue timeCoordIntvValue3 = timeCoordIntvDateValue.convertReferenceDate(refDate3, timeUnit);
			Assert.Equal(1, timeCoordIntvValue3.Bounds1);
			Assert.Equal(12, timeCoordIntvValue3.Bounds2);
			Assert.Equal(11, timeCoordIntvValue3.IntervalSize);
		}

		[Fact]
		public void ShouldPreserveTimeIntervalLengthWithStartBeforeRefDate()
		{
			CalendarDate start = CalendarDate.parseISOformat(null, "2022-08-16T01:00:00Z");
			CalendarDate end = CalendarDate.parseISOformat(null, "2022-08-16T12:00:00Z");
			TimeCoordIntvDateValue timeCoordIntvDateValue = new TimeCoordIntvDateValue(start, end);
			CalendarPeriod timeUnit = CalendarPeriod.of("Hour");

			CalendarDate refDate = CalendarDate.parseISOformat(null, "2022-08-16T01:30:00Z");
			TimeCoordIntvValue timeCoordIntvValue = timeCoordIntvDateValue.convertReferenceDate(refDate, timeUnit);
			Assert.Equal(-1, timeCoordIntvValue.Bounds1);
			Assert.Equal(10, timeCoordIntvValue.Bounds2);
			Assert.Equal(11, timeCoordIntvValue.IntervalSize);

			CalendarDate refDate2 = CalendarDate.parseISOformat(null, "2022-08-16T01:40:00Z");
			TimeCoordIntvValue timeCoordIntvValue2 = timeCoordIntvDateValue.convertReferenceDate(refDate2, timeUnit);
			Assert.Equal(-1, timeCoordIntvValue2.Bounds1);
			Assert.Equal(10, timeCoordIntvValue2.Bounds2);
			Assert.Equal(11, timeCoordIntvValue2.IntervalSize);

			CalendarDate refDate3 = CalendarDate.parseISOformat(null, "2022-08-16T01:20:00Z");
			TimeCoordIntvValue timeCoordIntvValue3 = timeCoordIntvDateValue.convertReferenceDate(refDate3, timeUnit);
			Assert.Equal(0, timeCoordIntvValue3.Bounds1);
			Assert.Equal(11, timeCoordIntvValue3.Bounds2);
			Assert.Equal(11, timeCoordIntvValue3.IntervalSize);
		}
	}
}
