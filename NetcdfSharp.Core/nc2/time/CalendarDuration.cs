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
	using Period = NetcdfSharp.org.joda.time.Period;

	/// <summary>
	/// A replacement for NetcdfSharp.ucar.nc2.units.TimeDuration. incomplete.
	/// 
	/// 
	/// A Duration is a fixed number of seconds.
	/// I think thats wrong: a CalendarDuration should be a integer and a CalendarPeriod.
	/// Optionally, a CalendarPeriod could have an integer value.
	/// 
	/// Implements the thredds "duration" XML element type: specifies a length of time.
	/// This is really the same as a NetcdfSharp.ucar.nc2.units.TimeUnit, but it allows xsd:duration syntax as well as udunits syntax.
	/// It also keeps track if the text is empty.
	/// <p/>
	/// A duration can be one of the following:
	/// <ol>
	/// <li>a valid udunits string compatible with "secs"
	/// <li>an xsd:duration type specified in the following form "PnYnMnDTnHnMnS" where:
	/// <ul>
	/// <li>P indicates the period (required)
	/// <li>nY indicates the number of years
	/// <li>nM indicates the number of months
	/// <li>nD indicates the number of days
	/// <li>T indicates the start of a time section (required if you are going to specify hours, minutes, or seconds)
	/// <li>nH indicates the number of hours
	/// <li>nM indicates the number of minutes
	/// <li>nS indicates the number of seconds
	/// </ul>
	/// </ol>
	/// 
	/// <para>
	/// change to JSR-310 javax.time.Duration when ready
	/// relation to javax.xml.datatype ?
	/// 
	/// @author john caron
	/// </para>
	/// </summary>
	/// <seealso cref=""https://www.unidata.ucar.edu/projects/THREDDS/tech/catalog/InvCatalogSpec.html.durationType""/>

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class CalendarDuration
	public class CalendarDuration
	{

	  /// <summary>
	  /// Convert a time udunit string
	  /// </summary>
	  /// <param name="value"> number of units </param>
	  /// <param name="udunit"> msec, sec, minute, hour, hr, day, week, month, year (plural form ok) </param>
	  /// <returns> joda Period </returns>
	  internal static Period convertToPeriod(int value, string udunit)
	  {
		if (udunit.EndsWith("s", StringComparison.Ordinal))
		{
		  udunit = udunit.Substring(0, udunit.Length - 1);
		}

		switch (udunit)
		{
		  case "msec":
			return Period.millis(value);
		  case "sec":
			return Period.seconds(value);
		  case "minute":
			return Period.minutes(value);
		  case "hour":
		  case "hr":
			return Period.hours(value);
		  case "day":
			return Period.days(value);
		  case "week":
			return Period.weeks(value);
		  case "month":
			return Period.months(value);
		  case "year":
			return Period.years(value);
		}

		throw new ArgumentException("cant convert " + udunit + " to Joda Period");
	  }

	  /////////////////////////////////////////////////////////////////////

	  // private final String text;
	  private readonly CalendarPeriod.Field timeUnit;
	  // private final boolean isBlank;
	  private readonly double value;
	  // private NetcdfSharp.org.joda.time.Period jperiod;

	  public CalendarDuration(int value, CalendarPeriod.Field timeUnit)
	  {
		this.value = value;
		this.timeUnit = timeUnit;
	  }

	  public virtual CalendarPeriod.Field TimeUnit
	  {
		  get
		  {
			return timeUnit;
		  }
	  }

	  public static CalendarDuration fromUnitString(string unitString)
	  {
		return new CalendarDuration(1, CalendarPeriod.fromUnitString(unitString));
	  }

	  /*
	   * Construct from 1) udunit time udunit string, 2) xsd:duration syntax, 3) blank string.
	   *
	   * @param text parse this text.
	   * 
	   * @throws NetcdfSharp.java.text.ParseException if invalid text.
	   *
	   * public CalendarDuration(String text) throws NetcdfSharp.java.text.ParseException {
	   * /* TimeUnit tunit = null;
	   * text = (text == null) ? "" : text.trim();
	   * this.text = text;
	   * this.isBlank = (text == null) || text.length() == 0;
	   * 
	   * // see if its blank
	   * if (isBlank) {
	   * try {
	   * tunit = new TimeUnit("1 sec");
	   * } catch (Exception e) { // cant happen
	   * }
	   * 
	   * } else {
	   * // see if its a udunits string
	   * try {
	   * tunit = new TimeUnit(text);
	   * 
	   * if (debug) System.out.println(" set time udunit= " + timeUnit);
	   * }
	   * 
	   * catch (Exception e) {
	   * // see if its a xsd:duration
	   * /*
	   * A time span as defined in the W3C XML Schema 1.0 specification:
	   * "PnYnMnDTnHnMnS, where nY represents the number of years, nM the number of months, nD the number of days,
	   * 'T' is the date/time separator, nH the number of hours, nM the number of minutes and nS the number of seconds.
	   * The number of seconds can include decimal digits to arbitrary precision."
	   *
	   * try {
	   * javax.xml.datatype.DatatypeFactory factory = javax.xml.datatype.DatatypeFactory.newInstance();
	   * javax.xml.datatype.Duration d = factory.newDuration(text);
	   * long secs = d.getTimeInMillis(new Date()) / 1000;
	   * tunit = new TimeUnit(secs + " secs");
	   * } catch (Exception e1) {
	   * throw new NetcdfSharp.java.text.ParseException(e1.getMessage(), 0);
	   * }
	   * }
	   * }
	   * 
	   * timeUnit = tunit;
	   * duration = tunit.toDurationStandard();
	   * }
	   */

	  /// <summary>
	  /// Get the duration in natural units, ie units of getTimeUnit()
	  /// </summary>
	  /// <returns> the duration in natural units </returns>
	  public virtual double Value
	  {
		  get
		  {
			return value;
		  }
	  }

	  /*
	   * Get the duration in seconds
	   * 
	   * @return the duration in seconds
	   *
	   * public double getValueInMillisecs() {
	   * if (timeUnit == CalendarPeriod.Millisec)
	   * return value;
	   * else if (timeUnit == CalendarPeriod.Second)
	   * return 1000 * value;
	   * else if (timeUnit == CalendarPeriod.Minute)
	   * return 60 * 1000 * value;
	   * else if (timeUnit == CalendarPeriod.Hour)
	   * return 60 * 60 * 1000 * value;
	   * else if (timeUnit == CalendarPeriod.Day)
	   * return 24 * 60 * 60 * 1000 * value;
	   * else if (timeUnit == CalendarPeriod.Month)
	   * return 30.0 * 24.0 * 60.0 * 60.0 * 1000.0 * value;
	   * else if (timeUnit == CalendarPeriod.Year)
	   * return 365.0 * 24.0 * 60.0 * 60.0 * 1000.0 * value;
	   * 
	   * else throw new IllegalStateException();
	   * }
	   * 
	   * public double getValueInSeconds() {
	   * if (timeUnit == CalendarPeriod.Millisec)
	   * return value/1000;
	   * else if (timeUnit == CalendarPeriod.Second)
	   * return value;
	   * else if (timeUnit == CalendarPeriod.Minute)
	   * return 60 * value;
	   * else if (timeUnit == CalendarPeriod.Hour)
	   * return 60 * 60 * value;
	   * else if (timeUnit == CalendarPeriod.Day)
	   * return 24 * 60 * 60 * value;
	   * else if (timeUnit == CalendarPeriod.Month)
	   * return 30.0 * 24.0 * 60.0 * 60.0 * value;
	   * else if (timeUnit == CalendarPeriod.Year)
	   * return 365.0 * 24.0 * 60.0 * 60.0 * value;
	   * 
	   * else throw new IllegalStateException();
	   * }
	   * 
	   * public double getValueInHours() {
	   * if (timeUnit == CalendarPeriod.Millisec)
	   * return value/1000/60/60;
	   * else if (timeUnit == CalendarPeriod.Second)
	   * return value/60/60;
	   * else if (timeUnit == CalendarPeriod.Minute)
	   * return value/60;
	   * else if (timeUnit == CalendarPeriod.Hour)
	   * return value;
	   * else if (timeUnit == CalendarPeriod.Day)
	   * return 24 * value;
	   * else if (timeUnit == CalendarPeriod.Month)
	   * return 30.0 * 24.0 * value;
	   * else if (timeUnit == CalendarPeriod.Year)
	   * return 365.0 * 24.0 * value;
	   * 
	   * else throw new IllegalStateException();
	   * }
	   * 
	   * public double getValueInDays() {
	   * if (timeUnit == CalendarPeriod.Millisec)
	   * return value/1000/60/60/24;
	   * else if (timeUnit == CalendarPeriod.Second)
	   * return value/60/60/24;
	   * else if (timeUnit == CalendarPeriod.Minute)
	   * return value/60/24;
	   * else if (timeUnit == CalendarPeriod.Hour)
	   * return value/24;
	   * else if (timeUnit == CalendarPeriod.Day)
	   * return value;
	   * else if (timeUnit == CalendarPeriod.Month)
	   * return 30.0 * value;
	   * else if (timeUnit == CalendarPeriod.Year)
	   * return 365.0 * value;
	   * 
	   * else throw new IllegalStateException();
	   * }
	   */

	  /*
	   * If this is a blank string
	   * 
	   * @return true if this is a blank string
	   *
	   * public boolean isBlank() {
	   * return isBlank;
	   * }
	   * 
	   * /*
	   * Get the String text
	   * 
	   * @return the text
	   *
	   * public String getText() {
	   * return text == null ? timeUnit.toString() : text;
	   * }
	   */

	  /// <summary>
	  /// String representation
	  /// </summary>
	  /// <returns> getText() </returns>
	  public override string ToString()
	  {
		return timeUnit.ToString();
	  }

	  /*
	   * Override to be consistent with equals
	   *
	   * public int hashCode2() {
	   * return isBlank() ? 0 : (int) getValueInSeconds();
	   * }
	   * 
	   * /*
	   * TimeDurations with same value in seconds are equals
	   *
	   * public boolean equals2(Object o) {
	   * if (this == o)
	   * return true;
	   * if (!(o instanceof CalendarDuration))
	   * return false;
	   * 
	   * CalendarDuration to = (CalendarDuration) o;
	   * return to.getValueInMillisecs() == getValueInMillisecs();
	   * }
	   */

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

		CalendarDuration that = (CalendarDuration) o;

		if (that.value.CompareTo(value) != 0)
		{
		  return false;
		}
		return timeUnit == that.timeUnit;
	  }

	  public override int GetHashCode()
	  {
		int result;
		long temp;
		result = timeUnit != null ? timeUnit.GetHashCode() : 0;
		temp = value != 0.0d ? System.BitConverter.DoubleToInt64Bits(value) : 0L;
		result = 31 * result + (int)(temp ^ (temp >>> 32));
		return result;
	  }
	}

}
