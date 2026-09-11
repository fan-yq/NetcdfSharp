// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.time
{
	using Matcher = NetcdfSharp.com.google.re2j.Matcher;
	using Pattern = NetcdfSharp.com.google.re2j.Pattern;
	using Chronology = NetcdfSharp.org.joda.time.Chronology;
	using JDateTime = NetcdfSharp.org.joda.time.JDateTime;
	using DateTimeZone = NetcdfSharp.org.joda.time.DateTimeZone;
	using DateTimeFormat = NetcdfSharp.org.joda.time.format.DateTimeFormat;
	using DateTimeFormatter = NetcdfSharp.org.joda.time.format.DateTimeFormatter;
	using DateFormatter = NetcdfSharp.ucar.nc2.units.DateFormatter;

	/// <summary>
	/// Threadsafe static routines for date formatting.
	/// Replacement for NetcdfSharp.ucar.nc2.units.DateFormatter
	/// 
	/// @author John
	/// @since 7/9/11
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @ThreadSafe public class CalendarDateFormatter
	public class CalendarDateFormatter
	{
	  // these are thread-safe (yeah!)
	  private static DateTimeFormatter isof = DateTimeFormat.forPattern("yyyy-MM-dd'T'HH:mm:ss'Z'").withZoneUTC();
	  private static DateTimeFormatter isof_with_millis_of_second = DateTimeFormat.forPattern("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'").withZoneUTC();
	  private static DateTimeFormatter dtf = DateTimeFormat.forPattern("yyyy-MM-dd HH:mm:ss'Z'").withZoneUTC();

	  private static DateTimeFormatter dtf_with_millis_of_second = DateTimeFormat.forPattern("yyyy-MM-dd HH:mm:ss.SSS'Z'").withZoneUTC();
	  private static DateTimeFormatter df = DateTimeFormat.forPattern("yyyy-MM-dd").withZoneUTC();
	  private static DateTimeFormatter df_units = DateTimeFormat.forPattern("yyyy-MM-dd HH:mm:ss.SSS 'UTC'").withZoneUTC(); // udunits

	  public static string toDateTimeStringISO(CalendarDate cd)
	  {

		if (cd.JDateTime.getMillisOfSecond() == 0)
		{
		  return isof.print(cd.JDateTime);
		}
		else
		{
		  return isof_with_millis_of_second.print(cd.JDateTime);
		}

	  }

	  public static string toDateTimeStringISO(JDateTime d)
	  {
		return toDateTimeStringISO(CalendarDate.of(d));
	  }

	  public static string toDateTimeStringISO(System.DateTime d)
	  {
		return toDateTimeStringISO(CalendarDate.of(d));
	  }

	  public static string toDateTimeStringISO(long millisecs)
	  {
		return toDateTimeStringISO(CalendarDate.of(millisecs));
	  }

	  public static string toDateTimeString(CalendarDate cd)
	  {

		if (cd.JDateTime.getMillisOfSecond() == 0)
		{
		  return dtf.print(cd.JDateTime);
		}
		else
		{
		  return dtf_with_millis_of_second.print(cd.JDateTime);
		}
	  }

	  public static string toDateString(JDateTime date)
	  {
		return toDateString(CalendarDate.of(date));
	  }

	  public static string toDateTimeString(JDateTime date)
	  {
		return toDateTimeString(CalendarDate.of(date));
	  }

	  public static string toDateTimeStringPresent()
	  {
		return dtf.print(new JDateTime());
	  }

	  public static string toDateString(CalendarDate cd)
	  {
		return df.print(cd.JDateTime);
	  }

	  public static string toDateStringPresent()
	  {
		return df.print(new JDateTime());
	  }

	  /// <summary>
	  /// udunits formatting
	  /// </summary>
	  /// <param name="cd"> the calendar date </param>
	  /// <returns> udunits formated date </returns>
	  public static string toTimeUnits(CalendarDate cd)
	  {
		return df_units.print(cd.JDateTime);
	  }

	  public static string toTimeUnits(JDateTime date)
	  {
		return df_units.print(date.Ticks);
	  }

	  public static CalendarDateFormatter factory(CalendarPeriod period)
	  {
		switch (period.FieldProp.innerEnumValue)
		{
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Year:
			return new CalendarDateFormatter("yyyy");
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Month:
			return new CalendarDateFormatter("yyyy-MM");
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Day:
			return new CalendarDateFormatter("yyyy-MM-dd");
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Hour:
			return new CalendarDateFormatter("yyyy-MM-ddTHH");
		  default:
			return new CalendarDateFormatter("yyyy-MM-ddTHH:mm:ss");
		}
	  }


	  /////////////////////////////////////////////////////////////////////////////
	  // reading an ISO formatted date

	  /// <summary>
	  /// Convert an ISO formatted String to a CalendarDate.
	  /// </summary>
	  /// <param name="calt"> calendar, may be null for default calendar (Calendar.getDefault()) </param>
	  /// <param name="iso"> ISO 8601 date String
	  /// 
	  ///        <pre>
	  /// possible forms for W3C profile of ISO 8601
	  ///    Year:
	  ///     YYYY (eg 1997)
	  ///  Year and month:
	  ///     YYYY-MM (eg 1997-07)
	  ///  Complete date:
	  ///     YYYY-MM-DD (eg 1997-07-16)
	  ///  Complete date plus hours and minutes:
	  ///     YYYY-MM-DDThh:mmTZD (eg 1997-07-16T19:20+01:00)
	  ///  Complete date plus hours, minutes and seconds:
	  ///     YYYY-MM-DDThh:mm:ssTZD (eg 1997-07-16T19:20:30+01:00)
	  ///  Complete date plus hours, minutes, seconds and a decimal fraction of a second
	  ///     YYYY-MM-DDThh:mm:ss.sTZD (eg 1997-07-16T19:20:30.45+01:00)
	  /// 
	  /// where:
	  ///    YYYY = four-digit year
	  ///    MM   = two-digit month (01=January, etc.)
	  ///    DD   = two-digit day of month (01 through 31)
	  ///    hh   = two digits of hour (00 through 23) (am/pm NOT allowed)
	  ///    mm   = two digits of minute (00 through 59)
	  ///    ss   = two digits of second (00 through 59)
	  ///    s    = one or more digits representing a decimal fraction of a second
	  ///    TZD  = time zone designator (Z or +hh:mm or -hh:mm)
	  /// except:
	  ///   You may use a space instead of the 'T'
	  ///   The year may be preceeded by a '+' (ignored) or a '-' (makes the date BCE)
	  ///   The date part uses a '-' delimiter instead of a fixed number of digits for each field
	  ///   The time part uses a ':' delimiter instead of a fixed number of digits for each field
	  ///        </pre>
	  /// </param>
	  /// <returns> CalendarDate using given calendar </returns>
	  /// <exception cref="IllegalArgumentException"> if the String is not a valid ISO 8601 date </exception>
	  /// <seealso cref=""http://www.w3.org/TR/NOTE-datetime""/>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static CalendarDate isoStringToCalendarDate(Calendar calt, String iso) throws IllegalArgumentException
	  public static CalendarDate isoStringToCalendarDate(Calendar calt, string iso)
	  {
		JDateTime dt = parseIsoTimeString(calt, iso);
		Calendar useCal = Calendar.of(dt.getChronology());
		return new CalendarDate(useCal, dt);
	  }

	  /// <summary>
	  /// Does not handle non-standard Calendars
	  /// </summary>
	  /// <param name="iso"> iso formatted string </param>
	  /// <returns> Date </returns>
	  /// @deprecated use isoStringToCalendarDate 
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static NetcdfSharp.java.util.Date isoStringToDate(String iso) throws IllegalArgumentException
	  public static System.DateTime isoStringToDate(string iso)
	  {
		CalendarDate dt = isoStringToCalendarDate(null, iso);
		return dt.toDate();
	  }

	  // 1 2 3
	  public const string isodatePatternString = "([\\+\\-?\\d]+)([ t]([\\.\\:?\\d]*)([ \\+\\-]\\S*)?z?)?$"; // public
																													// for
																													// testing
	  // private static final String isodatePatternString = "([\\+\\-\\d]+)[ Tt]([\\.\\:\\d]*)([ \\+\\-]\\S*)?z?)?$";
	  private static readonly Pattern isodatePattern = Pattern.compile(isodatePatternString);

	  private static JDateTime parseIsoTimeString(Calendar calt, string iso)
	  {
		iso = iso.Trim();
		iso = iso.ToLower();

		Matcher m = isodatePattern.matcher(iso);
		if (!m.matches())
		{
		  throw new ArgumentException(iso + " does not match regexp " + isodatePatternString);
		}

		string dateString = m.group(1);
		string timeString = m.group(3);
		string zoneString = m.group(4);

		// Set the defaults for any values that are not specified
		int year = 0;
		int month = 1;
		int day = 1;
		int hour = 0;
		int minute = 0;
		double second = 0.0;

		try
		{
		  bool isMinus = false;
		  if (dateString.StartsWith("-", StringComparison.Ordinal))
		  {
			isMinus = true;
			dateString = dateString.Substring(1);
		  }
		  else if (dateString.StartsWith("+", StringComparison.Ordinal))
		  {
			dateString = dateString.Substring(1);
		  }

		  if (dateString.Contains("-"))
		  {
			StringTokenizer dateTokenizer = new StringTokenizer(dateString, "-");
			if (dateTokenizer.hasMoreTokens())
			{
			  year = int.Parse(dateTokenizer.nextToken());
			}
			if (dateTokenizer.hasMoreTokens())
			{
			  month = int.Parse(dateTokenizer.nextToken());
			}
			if (dateTokenizer.hasMoreTokens())
			{
			  day = int.Parse(dateTokenizer.nextToken());
			}
		  }
		  else
		  {
			int dateLength = dateString.Length;
			if (dateLength % 2 != 0)
			{
			  throw new ArgumentException(dateString + " is ambiguous. Cannot parse uneven " + "length date strings when no date delimiter is used.");
			}
			else
			{
			  // dateString length must be even - only four digit year, two digit month, and
			  // two digit day values are allowed when there is no date delimiter.
			  if (dateLength > 3)
			  {
				year = int.Parse(dateString.Substring(0, 4));
			  }
			  if (dateLength > 5)
			  {
				month = int.Parse(dateString.Substring(4, 2));
			  }
			  if (dateLength > 7)
			  {
				day = int.Parse(dateString.Substring(6, 2));
			  }
			}
		  }

		  // Parse the time if present
		  if (!string.ReferenceEquals(timeString, null) && timeString.Length > 0)
		  {
			if (timeString.Contains(":"))
			{
			  StringTokenizer timeTokenizer = new StringTokenizer(timeString, ":");
			  if (timeTokenizer.hasMoreTokens())
			  {
				hour = int.Parse(timeTokenizer.nextToken());
			  }
			  if (timeTokenizer.hasMoreTokens())
			  {
				minute = int.Parse(timeTokenizer.nextToken());
			  }
			  if (timeTokenizer.hasMoreTokens())
			  {
				second = double.Parse(timeTokenizer.nextToken());
			  }
			}
			else
			{
			  int timeLengthNoSubseconds = timeString.Length;
			  // possible this contains a seconds value with subseconds (i.e. 25.125 seconds)
			  // since the seconds value can be a Double, let's check check the length of the
			  // time, without the subseconds (if they exist)
			  if (timeString.Contains("."))
			  {
				timeLengthNoSubseconds = timeString.Split('.')[0].Length;
			  }
			  // Ok, so this is a little tricky.
			  // First: A udunit date of 1992-10-8t7 is valid. We want to make sure this still work, so
			  // there is a special case of timeString.length() == 1;
			  //
			  // Second: We have the length of the timeString, without
			  // any subseconds. Given that the values for hour, minute, and second must be two
			  // digit numbers, the length of the timeString, without subseconds, should be even.
			  // However, if someone has encoded time as hhms, there is no way we will be able to tell.
			  // So, this is the best we can do to ensure we are parsing the time properly.
			  // Known failure here: hhms will be interpreted as hhmm, but hhms is not following iso, and
			  // a bad idea to use anyway.
			  if (timeLengthNoSubseconds == 1)
			  {
				hour = int.Parse(timeString);
			  }
			  else if (timeLengthNoSubseconds % 2 != 0)
			  {
				throw new ArgumentException(timeString + " is ambiguous. Cannot parse uneven " + "length time strings (ignoring subseconds) when no time delimiter is used.");
			  }
			  else
			  {
				// dateString length must be even - only four digit year, two digit month, and
				// two digit day values are allowed when there is no date delimiter.
				if (timeString.Length > 1)
				{
				  hour = int.Parse(timeString.Substring(0, 2));
				}
				if (timeString.Length > 3)
				{
				  minute = int.Parse(timeString.Substring(2, 2));
				}
				if (timeString.Length > 5)
				{
				  second = double.Parse(timeString.Substring(4));
				}
			  }
			}
		  }

		  if (isMinus)
		  {
			year = -year;
		  }

		  // kludge to deal with legacy files using year 0. // 10/10/2013 jcaron
		  if ((year == 0) && (calt == Calendar.gregorian))
		  {
			calt = Calendar.proleptic_gregorian;
		  }

		  // if (year < -292275054 || year > 292278993)
		  // throw new IllegalArgumentException(" incorrect date specification = " + iso);

		  // Get a JDateTime object in this Chronology
		  Chronology cron = Calendar.getChronology(calt);
		  cron = cron.withUTC(); // default is UTC
		  JDateTime dt = new JDateTime(year, month, day, hour, minute, 0, 0, cron);

		  // Add the seconds
		  dt = dt.plus((long)(1000 * second));

		  // Parse the time zone if present
		  if (!string.ReferenceEquals(zoneString, null))
		  {
			zoneString = zoneString.Trim();
			if (zoneString.Length > 0 && !zoneString.Equals("Z", StringComparison.OrdinalIgnoreCase) && !zoneString.Equals("UTC", StringComparison.OrdinalIgnoreCase) && !zoneString.Equals("GMT", StringComparison.OrdinalIgnoreCase))
			{
			  isMinus = false;
			  if (zoneString.StartsWith("-", StringComparison.Ordinal))
			  {
				isMinus = true;
				zoneString = zoneString.Substring(1);
			  }
			  else if (zoneString.StartsWith("+", StringComparison.Ordinal))
			  {
				zoneString = zoneString.Substring(1);
			  }

			  // allow 01:00, 1:00, 01 or 0100
			  int hourOffset;
			  int minuteOffset = 0;
			  int posColon = zoneString.IndexOf(':');
			  if (posColon > 0)
			  {
				string hourS = zoneString.Substring(0, posColon);
				string minS = zoneString.Substring(posColon + 1);
				hourOffset = int.Parse(hourS);
				minuteOffset = int.Parse(minS);

			  }
			  else
			  { // no colon - assume 2 digit hour, optional minutes
				if (zoneString.Length > 2)
				{
				  string hourS = zoneString.Substring(0, 2);
				  string minS = zoneString.Substring(2);
				  hourOffset = int.Parse(hourS);
				  minuteOffset = int.Parse(minS);
				}
				else
				{
				  hourOffset = int.Parse(zoneString);
				}
			  }
			  if (isMinus)
			  {
				// DateTimeZone.forOffsetHoursMinutes: "If constructed with the values (-2, 30), the resulting zone is
				// '-02:30'.
				// so i guess dont make minuteOffset negetive
				hourOffset = -hourOffset;
			  }
			  DateTimeZone dtz = DateTimeZone.forOffsetHoursMinutes(hourOffset, minuteOffset);

			  // Apply the time zone offset, retaining the field values. This
			  // manipulates the millisecond instance.
			  dt = dt.withZoneRetainFields(dtz);
			  // Now convert to the UTC time zone, retaining the millisecond instant
			  dt = dt.withZone(DateTimeZone.UTC);
			} // else {
			  // dt = dt.withZone(DateTimeZone.UTC); // default UTC
			// }

			// } else {
			// dt = dt.withZone(DateTimeZone.UTC); // default UTC
		  }

		  return dt;
		}
		catch (Exception e)
		{ // catch random joda exceptions
		  throw new ArgumentException("Illegal base time specification: '" + dateString + "' " + e.Message);
		}
	  }

	  /////////////////////////////////////////////
	  private readonly DateTimeFormatter dflocal;

	  /// <summary>
	  /// Date formatter with specified pattern.
	  /// NOTE: we are using jodatime patterns right now, but may switch to jsr-310 when thats available in java 8.
	  /// Not sure whether these patterns will still work then, so use this formatter at the risk of having to
	  /// change it eventually. OTOH, its likely that the same functionality will be present in jsr-310.
	  /// <para>
	  /// The pattern syntax is mostly compatible with NetcdfSharp.java.text.SimpleDateFormat -
	  /// time zone names cannot be parsed and a few more symbols are supported.
	  /// All ASCII letters are reserved as pattern letters, which are defined as follows:
	  /// </para>
	  /// 
	  /// <pre>
	  /// Symbol  Meaning                      Presentation  Examples
	  /// ------  -------                      ------------  -------
	  /// G       era                          text          AD
	  /// C       century of era (&gt;=0)         number        20
	  /// Y       year of era (&gt;=0)            year          1996
	  /// 
	  /// x       weekyear                     year          1996
	  /// w       week of weekyear             number        27
	  /// e       day of week                  number        2
	  /// E       day of week                  text          Tuesday; Tue
	  /// 
	  /// y       year                         year          1996
	  /// D       day of year                  number        189
	  /// M       month of year                month         July; Jul; 07
	  /// d       day of month                 number        10
	  /// 
	  /// a       halfday of day               text          PM
	  /// K       hour of halfday (0~11)       number        0
	  /// h       clockhour of halfday (1~12)  number        12
	  /// 
	  /// H       hour of day (0~23)           number        0
	  /// k       clockhour of day (1~24)      number        24
	  /// m       minute of hour               number        30
	  /// s       second of minute             number        55
	  /// S       fraction of second           number        978
	  /// 
	  /// z       time zone                    text          Pacific Standard Time; PST
	  /// Z       time zone offset/id          zone          -0800; -08:00; America/Los_Angeles
	  /// 
	  /// '       escape for text              delimiter
	  /// ''      single quote                 literal       '
	  /// </pre>
	  /// </summary>
	  public CalendarDateFormatter(string pattern)
	  {
		dflocal = DateTimeFormat.forPattern(pattern).withZoneUTC();
	  }

	  public CalendarDateFormatter(string pattern, CalendarTimeZone tz, Calendar cal)
	  {
		Chronology chron = Calendar.getChronology(cal);
		dflocal = DateTimeFormat.forPattern(pattern).withChronology(chron).withZone(tz.JodaTimeZone);
	  }

	  public CalendarDateFormatter(string pattern, CalendarTimeZone tz)
	  {
		dflocal = DateTimeFormat.forPattern(pattern).withZone(tz.JodaTimeZone);
	  }

	  public virtual string toString(CalendarDate cd)
	  {
		return dflocal.print(cd.JDateTime);
	  }

	  public virtual CalendarDate parse(string timeString)
	  {
		JDateTime dt = dflocal.parseDateTime(timeString);
		Calendar cal = Calendar.get(dt.getChronology().ToString());
		return new CalendarDate(cal, dt);
	  }
	}

}
