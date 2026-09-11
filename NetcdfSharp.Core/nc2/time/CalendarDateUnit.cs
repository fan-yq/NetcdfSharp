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

	/// <summary>
	/// A Calendar Date Unit: "unit since date"
	/// 
	/// <pre>
	/// UNIT since [-]Y[Y[Y[Y]]]-MM-DD[(T| )hh[:mm[:ss[.sss*]]][ [+|-]hh[[:]mm]]]
	/// 
	/// UNIT = "msecs" | "msec" |
	///          "seconds" | "second" | "secs" | "sec" | "s" |
	///          "minutes" | "minute" | "mins" | "min" |
	///          "hours" | "hour" | "hrs" | "hr" | "h" |
	///          "days" | "day" | "d"
	/// </pre>
	/// 
	/// "months since base_date by calendar field"
	/// "years since base_date by calendar field"
	/// 
	/// bob simon's proposal:
	/// 
	/// "calendar_month since base_date"
	/// "calendar_year since base_date"
	/// * only integer values are allowed (?)
	/// 
	/// jon blowers comment:
	/// In your view would the solution to add "by calendar field" to the existing udunits string be acceptable? It’s
	/// backward-compatible with the current interpretation and adds clarification for the cases in which we *do* want to
	/// do calendar-field arithmetic (instead of adding fixed intervals).
	/// There’s an alternative proposition, in which the new units of calendar_month and calendar_year are added, with the
	/// same semantic effect.
	/// (However, personally I like the "by calendar field" solution since it allows other fields to vary between calendars,
	/// e.g.
	/// because of leap-seconds.)
	/// 
	/// @author caron
	/// @since 3/18/11
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class CalendarDateUnit
	public class CalendarDateUnit
	{
	  public static readonly CalendarDateUnit unixDateUnit = CalendarDateUnit.of(null, CalendarPeriod.Field.Second, CalendarDate.parseISOformat(null, "1970-01-01T00:00:00"));

	  private const string byCalendarString = "calendar ";
	  // 1 2 3 4 5
	  public static readonly string udunitPatternString = "(\\w*)\\s*since\\s*" + CalendarDateFormatter.isodatePatternString;
	  // "([\\+\\-\\d]+)([ Tt]([\\.\\:\\d]*)([ \\+\\-]\\S*)?z?)?$"; // public for testing
	  private static readonly Pattern udunitPattern = Pattern.compile(udunitPatternString);

	  /// <summary>
	  /// Create a CalendarDateUnit from a calendar name and a udunit string = "unit since calendarDate"
	  /// </summary>
	  /// <param name="calendarName"> must match a calendar enum, or one of its aliases, see NetcdfSharp.ucar.nc2.time.Calendar.get() </param>
	  /// <param name="udunitString"> "unit since calendarDate" </param>
	  /// <returns> CalendarDateUnit </returns>
	  /// <exception cref="IllegalArgumentException"> if udunitString is not paresable </exception>
	  public static CalendarDateUnit of(string calendarName, string udunitString)
	  {
		Calendar calt = Calendar.get(calendarName);
		if (calt == null)
		{
		  calt = Calendar.Default;
		}
		return new CalendarDateUnit(calt, udunitString);
	  }

	  /// <summary>
	  /// Create a CalendarDateUnit from a calendar and a udunit string = "unit since calendarDate"
	  /// </summary>
	  /// <param name="calt"> use this Calendar, or null for default calendar </param>
	  /// <param name="udunitString"> "unit since calendarDate" </param>
	  /// <returns> CalendarDateUnit </returns>
	  /// <exception cref="IllegalArgumentException"> if udunitString is not paresable </exception>
	  public static CalendarDateUnit withCalendar(Calendar calt, string udunitString)
	  {
		if (calt == null)
		{
		  calt = Calendar.Default;
		}
		return new CalendarDateUnit(calt, udunitString);
	  }

	  /// <summary>
	  /// Create a CalendarDateUnit from a calendar, a CalendarPeriod.Field, and a base date
	  /// </summary>
	  /// <param name="calt"> use this Calendar, or null for default calendar </param>
	  /// <param name="periodField"> a CalendarPeriod.Field like Hour or second </param>
	  /// <param name="baseDate"> "since baseDate" </param>
	  /// <returns> CalendarDateUnit </returns>
	  public static CalendarDateUnit of(Calendar calt, CalendarPeriod.Field periodField, CalendarDate baseDate)
	  {
		if (calt == null)
		{
		  calt = Calendar.Default;
		}
		return new CalendarDateUnit(calt, periodField, baseDate);
	  }

	  ////////////////////////////////////////////////////////////////////////////////////////
	  private readonly Calendar cal;
	  // private final String unitString;
	  private readonly CalendarPeriod period;
	  private readonly CalendarPeriod.Field periodField;
	  private readonly CalendarDate baseDate;
	  private readonly bool isCalendarField;

	  private CalendarDateUnit(Calendar calt, string dateUnitString)
	  {

		dateUnitString = dateUnitString.Trim();
		dateUnitString = dateUnitString.replaceAll("\\s+", " ");
		dateUnitString = dateUnitString.ToLower();

		isCalendarField = dateUnitString.StartsWith(byCalendarString, StringComparison.Ordinal);
		if (isCalendarField)
		{
		  dateUnitString = dateUnitString.Substring(byCalendarString.Length).Trim();
		}

		Matcher m = udunitPattern.matcher(dateUnitString);
		if (!m.matches())
		{
		  throw new ArgumentException(dateUnitString + " does not match " + udunitPatternString);
		}

		string unitString = m.group(1);
		period = NetcdfSharp.ucar.nc2.time.CalendarPeriod.of(unitString);
		periodField = NetcdfSharp.ucar.nc2.time.CalendarPeriod.fromUnitString(unitString);

		int pos = dateUnitString.IndexOf("since", StringComparison.Ordinal);
		string iso = dateUnitString.Substring(pos + 5);
		baseDate = CalendarDateFormatter.isoStringToCalendarDate(calt, iso);

		// calendar might change !!
		calt = baseDate.Calendar;
		this.cal = calt;
	  }

	  private CalendarDateUnit(Calendar calt, CalendarPeriod.Field periodField, CalendarDate baseDate)
	  {
		this.cal = calt;
		this.periodField = periodField;
		this.period = NetcdfSharp.ucar.nc2.time.CalendarPeriod.of(1, periodField);
		this.baseDate = baseDate;

		isCalendarField = periodField == CalendarPeriod.Field.Month || periodField == CalendarPeriod.Field.Year;
	  }

	  // given a CalendarDate, find the values in this unit (secs, days, etc) from the baseDate
	  // inverse of makeCalendarDate
	  public virtual double makeOffsetFromRefDate(CalendarDate date)
	  {
		if (isCalendarField)
		{
		  if (date.Equals(baseDate))
		  {
			return 0.0;
		  }
		  return date.getDifference(baseDate, periodField);
		}
		else
		{
		  long msecs = date.getDifferenceInMsecs(baseDate);
		  return msecs / period.ValueInMillisecs;
		}
	  }

	  // given a value in this unit (secs, days, etc), create the CalendarDate from the RefDate
	  // inverse of makeOffsetFromRefDate
	  public virtual CalendarDate makeCalendarDate(double value)
	  {
		if (isCalendarField)
		{
		  return baseDate.add(NetcdfSharp.ucar.nc2.time.CalendarPeriod.of((int) value, periodField)); // LOOK int vs double
		}
		else
		{
		  return baseDate.add(value, periodField);
		}
	  }

	  public virtual CalendarDate makeCalendarDate(int value)
	  {
		if (isCalendarField)
		{
		  return baseDate.add(NetcdfSharp.ucar.nc2.time.CalendarPeriod.of(value, periodField));
		}
		else
		{
		  return baseDate.add(value, periodField);
		}
	  }

	  public virtual string UdUnit
	  {
		  get
		  {
			return ToString();
		  }
	  }

	  public virtual CalendarDate BaseCalendarDate
	  {
		  get
		  {
			return baseDate;
		  }
	  }

	  public virtual CalendarPeriod CalendarPeriod
	  {
		  get
		  {
			return period;
		  }
	  }

	  public virtual CalendarPeriod.Field getCalendarField()
	  {
		return periodField;
	  }

	  public virtual Calendar Calendar
	  {
		  get
		  {
			return cal;
		  }
	  }

	  public virtual bool CalendarField
	  {
		  get
		  {
			return isCalendarField;
		  }
	  }

	  /////////////////////////////////////

	  public override bool Equals(object o)
	  {
		if (this == o)
		{
		  return true;
		}
		if (o == null || this.GetType() != o.GetType())
		{
		  return false;
		}
		CalendarDateUnit that = (CalendarDateUnit) o;
		return isCalendarField == that.isCalendarField && cal == that.cal && period.Equals(that.period) && periodField == that.periodField && baseDate.Equals(that.baseDate);
	  }

	  public override int GetHashCode()
	  {
		return Objects.hash(cal, period, periodField, baseDate, isCalendarField);
	  }

	  public override string ToString()
	  {
		Formatter f = new Formatter();
		if (isCalendarField)
		{
		  f.format("%s", byCalendarString);
		}
		f.format("%s since %s", periodField, CalendarDateFormatter.toDateTimeStringISO(baseDate));
		return f.ToString();
	  }

	  /////////////////////////////////////////////
	  // testing
	  internal virtual DateTime BaseDate
	  {
		  get
		  {
			return baseDate.toDate();
		  }
	  }
	}

	/*
	 * 
	 * The acceptable units for time are listed in the udunits.dat file. The most commonly used of these strings (and their
	 * abbreviations)
	 * includes day (d), hour (hr, h), minute (min) and second (sec, s). Plural forms are also acceptable. The reference
	 * time string
	 * (appearing after the identifier since) may include date alone; date and time; or date, time, and time zone.
	 * The reference time is required. A reference time in year 0 has a special meaning (see Section 7.4, Climatological
	 * Statistics).
	 * 
	 * Note: if the time zone is omitted the default is UTC, and if both time and time zone are omitted the default is
	 * 00:00:00 UTC.
	 * 
	 * We recommend that the unit year be used with caution. The Udunits package defines a year to be exactly 365.242198781
	 * days
	 * (the interval between 2 successive passages of the sun through vernal equinox). It is not a calendar year.
	 * Udunits includes the following definitions for years: a common_year is 365 days, a leap_year is 366 days, a
	 * Julian_year is 365.25 days,
	 * and a Gregorian_year is 365.2425 days.
	 * 
	 * For similar reasons the unit month, which is defined in udunits.dat to be exactly year/12, should also be used with
	 * caution.
	 * 
	 * Example 4.4. Time axis
	 * 
	 * double time(time) ;
	 * time:long_name = "time" ;
	 * time:units = "days since 1990-1-1 0:0:0" ;
	 * 
	 * 
	 * A time coordinate is identifiable from its units string alone. The Udunits routines utScan() and utIsTime() can be
	 * used to
	 * make this determination.
	 * 
	 * Optionally, the time coordinate may be indicated additionally by providing the standard_name attribute with an
	 * appropriate
	 * value, and/or the axis attribute with the value
	 */

}
