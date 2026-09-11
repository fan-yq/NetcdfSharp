// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/nc2/units/TestDateFromString.java
// The testFromMain case only prints and is omitted.


using System;
using NetcdfSharp.ucar.nc2.units;
using Xunit;

namespace NetcdfSharp.ucar.nc2.units
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.units.TestDateFromString - date extraction from filenames.
	/// </summary>
	public class Nc2DateFromStringTests
	{
		private const string fileName = "xzy_tds_20051129_1235_junk.grib";
		private const string dateAsISOString = "2005-11-29T12:35";
		private const long dateAsLong = 1133267700000L;

		[Fact]
		public void TestGetDateUsingSimpleDateFormat()
		{
			string dateFormatString = "yyyyMMdd_HHmm";
			DateTime date = DateFromString.getDateUsingSimpleDateFormat(fileName, dateFormatString);
			Assert.Equal(dateAsLong, ToEpochMillis(date));
		}

		[Fact]
		public void TestGetDateUsingCompleteDateFormat()
		{
			string dateFormatString = "'xzy_tds_'yyyyMMdd_HHmm'_junk.grib'";

			DateTime date = DateFromString.getDateUsingCompleteDateFormat(fileName, dateFormatString);
			Assert.Equal(dateAsLong, ToEpochMillis(date));
		}

		[Fact]
		public void TestGetDateUsingRegExp()
		{
			string matchPattern = ".*([0-9]{4})([0-9]{2})([0-9]{2})_([0-9]{2})([0-9]{2}).*grib";
			string substitutionPattern = "$1-$2-$3T$4:$5";
			DateTime date = DateFromString.getDateUsingRegExp(fileName, matchPattern, substitutionPattern);
			Assert.Equal(dateAsLong, ToEpochMillis(date));
		}

		[Fact]
		public void TestGetDateUsingRegExpAndDateFormat()
		{
			string matchPattern = ".*([0-9]{4})([0-9]{2})([0-9]{2})_([0-9]{2})([0-9]{2}).*grib";
			string substitutionPattern = "$1$2$3_$4$5";
			string dateFormatString = "yyyyMMdd_HHmm";

			DateTime date = DateFromString.getDateUsingRegExpAndDateFormat(fileName, matchPattern, substitutionPattern, dateFormatString);
			Assert.Equal(dateAsLong, ToEpochMillis(date));
		}

		private static long ToEpochMillis(DateTime date)
		{
			// The parsed DateTime has Kind=Unspecified but always represents GMT (netcdf-java
			// SimpleDateFormat is UTC); construct an explicit-UTC DateTimeOffset, independent of
			// the machine's local timezone.
			return new DateTimeOffset(DateTime.SpecifyKind(date, DateTimeKind.Utc)).ToUnixTimeMilliseconds();
		}
	}
}
