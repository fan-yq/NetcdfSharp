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
	using Chronology = NetcdfSharp.org.joda.time.Chronology;
	using JDateTime = NetcdfSharp.org.joda.time.JDateTime;
	using DateTimeFieldType = NetcdfSharp.org.joda.time.DateTimeFieldType;
	using DateTimeZone = NetcdfSharp.org.joda.time.DateTimeZone;

	/// <summary>
	/// A Calendar Date. Replaces NetcdfSharp.java.util.Date.
	/// Allows non-standard calendars. Default is Calendar.gregorian.
	/// Always in UTC time zone.
	/// 
	/// @author caron
	/// @since 3/21/11
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class CalendarDate implements Comparable<CalendarDate>
	public class CalendarDate : IComparable<CalendarDate>
	{
	  public static readonly CalendarDate UNKNOWN = CalendarDate.of(0);
	  public const double MILLISECS_IN_SECOND = 1000;
	  public static readonly double MILLISECS_IN_MINUTE = MILLISECS_IN_SECOND * 60;
	  public static readonly double MILLISECS_IN_HOUR = MILLISECS_IN_MINUTE * 60;
	  public static readonly double MILLISECS_IN_DAY = MILLISECS_IN_HOUR * 24;
	  public const double MILLISECS_IN_YEAR = 3.15569259747E10;
	  public static readonly double MILLISECS_IN_MONTH = MILLISECS_IN_YEAR / 12;

	  /// <summary>
	  /// Get a CalendarDate representing the present moment
	  /// </summary>
	  /// <returns> CalendarDate representing the present moment in UTC </returns>
	  public static CalendarDate present()
	  {
		return new CalendarDate(null, new JDateTime());
	  }

	  public static CalendarDate present(Calendar cal)
	  {
		return new CalendarDate(cal, new JDateTime());
	  }

	  /// <summary>
	  /// Get Calendar date from fields. Uses UTZ time zone
	  /// </summary>
	  /// <param name="cal"> calendar to use, or null for default </param>
	  /// <param name="year"> any integer </param>
	  /// <param name="monthOfYear"> 1-12 </param>
	  /// <param name="dayOfMonth"> 1-31 </param>
	  /// <param name="hourOfDay"> 0-23 </param>
	  /// <param name="minuteOfHour"> 0-59 </param>
	  /// <param name="secondOfMinute"> 0-59 </param>
	  /// <returns> CalendarDate </returns>
	  public static CalendarDate of(Calendar cal, int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour, int secondOfMinute)
	  {
		Chronology @base = Calendar.getChronology(cal);
		/*
		 * if (base == null)
		 * base = ISOChronology.getInstanceUTC(); // already in UTC
		 * else
		 * base = ZonedChronology.getInstance( base, DateTimeZone.UTC); // otherwise wrap it to be in UTC
		 */

		JDateTime dt = new JDateTime(year, monthOfYear, dayOfMonth, hourOfDay, minuteOfHour, secondOfMinute, @base);
		if (!Calendar.isDefaultChronology(cal))
		{
		  dt = dt.withChronology(Calendar.getChronology(cal));
		}
		dt = dt.withZone(DateTimeZone.UTC);
		return new CalendarDate(cal, dt);
	  }

	  public static CalendarDate withDoy(Calendar cal, int year, int doy, int hourOfDay, int minuteOfHour, int secondOfMinute)
	  {
		Chronology @base = Calendar.getChronology(cal);
		/*
		 * if (base == null)
		 * base = ISOChronology.getInstanceUTC(); // already in UTC
		 * else
		 * base = ZonedChronology.getInstance( base, DateTimeZone.UTC); // otherwise wrap it to be in UTC
		 */

		JDateTime dt = new JDateTime(year, 1, 1, hourOfDay, minuteOfHour, secondOfMinute, @base);
		dt = dt.withZone(DateTimeZone.UTC);
		dt = dt.withDayOfYear(doy);
		if (!Calendar.isDefaultChronology(cal))
		{
		  dt = dt.withChronology(Calendar.getChronology(cal));
		}

		return new CalendarDate(cal, dt);
	  }

	  /// <summary>
	  /// Create CalendarDate from a NetcdfSharp.java.util.Date.
	  /// Uses standard Calendar.
	  /// </summary>
	  /// <param name="date"> NetcdfSharp.java.util.Date </param>
	  /// <returns> CalendarDate in UTC </returns>
	  public static CalendarDate of(System.DateTime date)
	  {
		JDateTime dt = new JDateTime(date, DateTimeZone.UTC);
		return new CalendarDate(null, dt);
	  }

	  /// <summary>
	  /// Create CalendarDate from an existing JDateTime.
	  /// </summary>
	  public static CalendarDate of(JDateTime dt)
	  {
		return new CalendarDate(null, dt);
	  }

	  /// <summary>
	  /// Create CalendarDate from msecs since epoch
	  /// Uses standard Calendar.
	  /// </summary>
	  /// <param name="msecs"> milliseconds from 1970-01-01T00:00:00Z </param>
	  /// <returns> CalendarDate in UTC </returns>
	  public static CalendarDate of(long msecs)
	  {
		// Constructs an instance set to the milliseconds from 1970-01-01T00:00:00Z using ISOChronology in the specified
		// time zone.
		JDateTime dt = new JDateTime(msecs, DateTimeZone.UTC);
		return new CalendarDate(null, dt);
	  }

	  /// <summary>
	  /// Create CalendarDate from msecs since epoch
	  /// Uses the given Calendar.
	  /// </summary>
	  /// <param name="cal"> calendar to use, or null for default </param>
	  /// <param name="msecs"> milliseconds from 1970-01-01T00:00:00Z </param>
	  /// <returns> CalendarDate in UTC time zone. </returns>
	  public static CalendarDate of(Calendar cal, long msecs)
	  {
		Chronology @base = Calendar.getChronology(cal);
		JDateTime dt = new JDateTime(msecs, @base);
		return new CalendarDate(cal, dt);
	  }

	  /// <summary>
	  /// Get CalendarDate from ISO date string
	  /// </summary>
	  /// <param name="calendarName"> get Calendar from Calendar.get(calendarName). may be null </param>
	  /// <param name="isoOrUdunits"> ISO or udunits date string </param>
	  /// <returns> CalendarDate or null if not valid </returns>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public static CalendarDate parseUdunitsOrIso(String calendarName, String isoOrUdunits)
	  public static CalendarDate parseUdunitsOrIso(string calendarName, string isoOrUdunits)
	  {
		CalendarDate result;
		try
		{
		  result = parseISOformat(calendarName, isoOrUdunits);
		}
		catch (Exception)
		{
		  try
		  {
			result = parseUdunits(calendarName, isoOrUdunits);
		  }
		  catch (Exception)
		  {
			return null;
		  }
		}
		return result;
	  }



	  /// <summary>
	  /// Get CalendarDate from ISO date string
	  /// </summary>
	  /// <param name="calendarName"> get Calendar from Calendar.get(calendarName). may be null </param>
	  /// <param name="isoDateString"> ISO date string </param>
	  /// <returns> CalendarDate </returns>
	  public static CalendarDate parseISOformat(string calendarName, string isoDateString)
	  {
		Calendar cal = Calendar.get(calendarName);
		if (cal == null)
		{
		  cal = Calendar.Default;
		}
		return CalendarDateFormatter.isoStringToCalendarDate(cal, isoDateString);
	  }

	  /// <summary>
	  /// Get CalendarDate from udunit date string
	  /// </summary>
	  /// <param name="calendarName"> get Calendar from Calendar.get(calendarName). may be null </param>
	  /// <param name="udunits"> must be value (space) udunits string </param>
	  /// <returns> CalendarDate </returns>
	  public static CalendarDate parseUdunits(string calendarName, string udunits)
	  {
		int pos = udunits.IndexOf(' ');
		if (pos < 0)
		{
		  return null;
		}
		string valString = udunits.Substring(0, pos).Trim();
		string unitString = udunits.Substring(pos + 1).Trim();

		CalendarDateUnit cdu = CalendarDateUnit.of(calendarName, unitString);
		double val = double.Parse(valString);
		return cdu.makeCalendarDate(val);
	  }

	  // internal use only
	  internal static CalendarDate of(Calendar cal, JDateTime dateTime)
	  {
		return new CalendarDate(cal, dateTime);
	  }

	  ////////////////////////////////////////////////

	  private readonly JDateTime dateTime;
	  private readonly Calendar cal;

	  internal CalendarDate(Calendar cal, JDateTime dateTime)
	  {
		this.cal = cal == null ? Calendar.Default : cal;
		this.dateTime = dateTime;
	  }

	  public virtual Calendar Calendar
	  {
		  get
		  {
			return cal;
		  }
	  }

	  /// <summary>
	  /// Gets the milliseconds of the datetime instant from the Java epoch
	  /// of 1970-01-01T00:00:00Z.
	  /// </summary>
	  /// <returns> the number of milliseconds since 1970-01-01T00:00:00Z </returns>
	  public virtual long Millis
	  {
		  get
		  {
			return dateTime.getMillis();
		  }
	  }

	  // package private
	  internal virtual JDateTime JDateTime
	  {
		  get
		  {
			return dateTime;
		  }
	  }

	  public virtual int CompareTo(CalendarDate o)
	  {
		return dateTime.compareTo(o.dateTime);
	  }

	  public virtual bool isAfter(CalendarDate o)
	  {
		return dateTime.isAfter(o.dateTime);
	  }

	  public virtual bool isBefore(CalendarDate o)
	  {
		return dateTime.isBefore(o.dateTime);
	  }

	  public override bool Equals(object o)
	  {
		if (this == o)
		{
		  return true;
		}
		if (!(o is CalendarDate))
		{
		  return false;
		}
		CalendarDate other = (CalendarDate) o;
		return other.cal == cal && other.dateTime.Equals(dateTime);
	  }

	  public override int GetHashCode()
	  {
		int result = 17;
		if (cal != null)
		{
		  result += 37 * result + cal.GetHashCode();
		}
		result += 37 * result + dateTime.GetHashCode();
		return result;
	  }

	  /// <summary>
	  /// ISO formatted string
	  /// </summary>
	  /// <returns> ISO8601 format (yyyy-MM-ddTHH:mm:ss.SSSZ) </returns>
	  public override string ToString()
	  {
		return CalendarDateFormatter.toDateTimeStringISO(this);
	  }

	  /// <summary>
	  /// udunits formatting
	  /// </summary>
	  /// <returns> udunits formatted date </returns>
	  public virtual string TimeUnits
	  {
		  get
		  {
			return CalendarDateFormatter.toTimeUnits(this);
		  }
	  }

	  /// <summary>
	  /// Get the hour of day (0-23) field for this chronology.
	  /// </summary>
	  /// <returns> hour of day (0-23) </returns>
	  public virtual int HourOfDay
	  {
		  get
		  {
			return dateTime.getHourOfDay();
		  }
	  }

	  /*
	   * Millisec(PeriodType.millis()), Second(PeriodType.seconds()), Minute(PeriodType.minutes()),
	   * Hour(PeriodType.hours()),
	   * Day(PeriodType.days()), Month(PeriodType.months()), Year(PeriodType.years())
	   */
	  public virtual int getFieldValue(CalendarPeriod.Field fld)
	  {
		switch (fld.innerEnumValue)
		{
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Day:
			return dateTime.get(DateTimeFieldType.dayOfMonth());
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Hour:
			return dateTime.get(DateTimeFieldType.hourOfDay());
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Millisec:
			return dateTime.get(DateTimeFieldType.millisOfSecond());
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Minute:
			return dateTime.get(DateTimeFieldType.minuteOfHour());
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Month:
			return dateTime.get(DateTimeFieldType.monthOfYear());
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Second:
			return dateTime.get(DateTimeFieldType.secondOfMinute());
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Year:
			return dateTime.get(DateTimeFieldType.year());
		}
		throw new ArgumentException("unimplemented " + fld);
	  }

	  public virtual int DayOfMonth
	  {
		  get
		  {
			return dateTime.getDayOfMonth();
		  }
	  }

	  // old style udunits compatible.
	  // fixes from https://github.com/jonescc 8/26/14

	  /// @deprecated use CalendarDate add(CalendarPeriod period) 
	  public virtual CalendarDate add(double value, CalendarPeriod.Field unit)
	  {
		switch (unit.innerEnumValue)
		{
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Millisec:
			return new CalendarDate(cal, dateTime.plus((long)Math.Floor(value + 0.5)));
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Second:
			return new CalendarDate(cal, dateTime.plus((long)Math.Floor(value * MILLISECS_IN_SECOND + 0.5)));
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Minute:
			return new CalendarDate(cal, dateTime.plus((long)Math.Floor(value * MILLISECS_IN_MINUTE + 0.5)));
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Hour:
			return new CalendarDate(cal, dateTime.plus((long)Math.Floor(value * MILLISECS_IN_HOUR + 0.5)));
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Day:
			return new CalendarDate(cal, dateTime.plus((long)Math.Floor(value * MILLISECS_IN_DAY + 0.5)));
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Month: // LOOK should we throw warning ?
			return new CalendarDate(cal, dateTime.plus((long)Math.Floor(value * MILLISECS_IN_MONTH + 0.5)));
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Year: // LOOK should we throw warning ?
			return new CalendarDate(cal, dateTime.plus((long)Math.Floor(value * MILLISECS_IN_YEAR + 0.5)));
		}
		throw new System.NotSupportedException("period units = " + unit);
	  }

	  // calendar date field
	  public virtual CalendarDate add(CalendarPeriod period)
	  {
		switch (period.FieldProp.innerEnumValue)
		{
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Millisec:
			return new CalendarDate(cal, dateTime.plusMillis(period.Value));
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Second:
			return new CalendarDate(cal, dateTime.plusSeconds(period.Value));
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Minute:
			return new CalendarDate(cal, dateTime.plusMinutes(period.Value));
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Hour:
			return new CalendarDate(cal, dateTime.plusHours(period.Value));
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Day:
			return new CalendarDate(cal, dateTime.plusDays(period.Value));
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Month:
			return new CalendarDate(cal, dateTime.plusMonths(period.Value));
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Year:
			return new CalendarDate(cal, dateTime.plusYears(period.Value));
		}
		throw new System.NotSupportedException("period units = " + period);
	  }

	  // calendar date field
	  public virtual CalendarDate subtract(CalendarPeriod period)
	  {
		switch (period.FieldProp.innerEnumValue)
		{
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Millisec:
			return new CalendarDate(cal, dateTime.minusMillis(period.Value));
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Second:
			return new CalendarDate(cal, dateTime.minusSeconds(period.Value));
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Minute:
			return new CalendarDate(cal, dateTime.minusMinutes(period.Value));
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Hour:
			return new CalendarDate(cal, dateTime.minusHours(period.Value));
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Day:
			return new CalendarDate(cal, dateTime.minusDays(period.Value));
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Month:
			return new CalendarDate(cal, dateTime.minusMonths(period.Value));
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Year:
			return new CalendarDate(cal, dateTime.minusYears(period.Value));
		}
		throw new System.NotSupportedException("period units = " + period);
	  }

	  /// <summary>
	  /// truncate the CalendarDate, by zeroing all the fields that are less than the field.
	  /// So 2013-03-01T19:30 becomes 2013-03-01T00:00 if the field is "day"
	  /// </summary>
	  /// <param name="fld"> set to 0 all fields less than this one </param>
	  /// <returns> truncated result </returns>
	  public virtual CalendarDate truncate(CalendarPeriod.Field fld)
	  {
		switch (fld.innerEnumValue)
		{
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Minute:
			return CalendarDate.of(cal, dateTime.getYear(), dateTime.getMonthOfYear(), dateTime.getDayOfMonth(), dateTime.getHourOfDay(), dateTime.getMinuteOfHour(), 0);
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Hour:
			return CalendarDate.of(cal, dateTime.getYear(), dateTime.getMonthOfYear(), dateTime.getDayOfMonth(), dateTime.getHourOfDay(), 0, 0);
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Day:
			return CalendarDate.of(cal, dateTime.getYear(), dateTime.getMonthOfYear(), dateTime.getDayOfMonth(), 0, 0, 0);
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Month:
			return CalendarDate.of(cal, dateTime.getYear(), dateTime.getMonthOfYear(), 1, 0, 0, 0);
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Year:
			return CalendarDate.of(cal, dateTime.getYear(), 1, 1, 0, 0, 0);
		}
		return this;
	  }

	  /// <summary>
	  /// Get the equivilent NetcdfSharp.java.util.Date
	  /// </summary>
	  /// <returns> the equivalent Date </returns>
	  public virtual System.DateTime toDate()
	  {
		return dateTime.toDate();
	  }

	  /// <summary>
	  /// Get difference between two calendar dates in millisecs
	  /// </summary>
	  /// <param name="o"> other calendar date </param>
	  /// <returns> (this minus o) difference in millisecs </returns>
	  public virtual long getDifferenceInMsecs(CalendarDate o)
	  {
		return dateTime.getMillis() - o.dateTime.getMillis();
	  }

	  /// <summary>
	  /// Get difference between two calendar dates in given Field units
	  /// </summary>
	  /// <param name="o"> other calendar date </param>
	  /// <returns> (this minus o) difference in units of this Field </returns>
	  public virtual long getDifference(CalendarDate o, CalendarPeriod.Field fld)
	  {
		switch (fld.innerEnumValue)
		{
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Millisec:
			return getDifferenceInMsecs(o);
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Second:
			return (long)(getDifferenceInMsecs(o) / MILLISECS_IN_SECOND);
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Minute:
			return (long)(getDifferenceInMsecs(o) / MILLISECS_IN_MINUTE);
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Hour:
			return (long)(getDifferenceInMsecs(o) / MILLISECS_IN_HOUR);
		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Day:
			return (long)(getDifferenceInMsecs(o) / MILLISECS_IN_DAY);

		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Month:
			int tmonth = getFieldValue(CalendarPeriod.Field.Month);
			int omonth = o.getFieldValue(CalendarPeriod.Field.Month);
			int years = (int) this.getDifference(o, CalendarPeriod.Field.Year);
			return tmonth - omonth + 12 * years;

		  case NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.InnerEnum.Year:
			int tyear = getFieldValue(CalendarPeriod.Field.Year);
			int oyear = o.getFieldValue(CalendarPeriod.Field.Year);
			return tyear - oyear;
		}
		return dateTime.getMillis() - o.dateTime.getMillis();
	  }

	  public virtual GregorianCalendar toGregorianCalendar()
	  {
		DateTimeZone zone = dateTime.getZone();
		GregorianCalendar cal = new GregorianCalendar(zone.toTimeZone());
		cal.setTime(toDate());
		return cal;
	  }
	}

}
