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
namespace NetcdfSharp.ucar.nc2.units
{
	using CalendarDate = NetcdfSharp.ucar.nc2.time.CalendarDate;
	using NetcdfSharp.ucar.units;

	/// <summary>
	/// Handles udunits dates, represented as "n units of time since reference date" eg
	/// "1203 days since 1970-01-01 00:00:00".
	/// <para>
	/// This is a wrapper around ucar.units package.
	/// It tracks the value, the base time unit, and the date origin separately.
	/// 
	/// @author caron
	/// </para>
	/// </summary>

	public class DateUnit
	{ // extends SimpleUnit {

	  /// <summary>
	  /// Create a NetcdfSharp.java.util.Date from this udunits String.
	  /// </summary>
	  /// <param name="text"> a udunit string.
	  /// 
	  ///        <pre>
	  /// [number] (units) since [-]Y[Y[Y[Y]]]-MM-DD[(T| )hh[:mm[:ss[.sss*]]][ [+|-]hh[[:]mm]]]
	  ///        </pre>
	  /// </param>
	  /// <returns> Date or null if not date unit. </returns>
	  public static DateTime getStandardDate(string text)
	  {
		double value;
		string udunitString;

		text = text.Trim();
		StringTokenizer stoker = new StringTokenizer(text);
		string firstToke = stoker.nextToken();
		try
		{
		  value = double.Parse(firstToke);
		  udunitString = text.Substring(firstToke.Length);
		}
		catch (System.FormatException)
		{ // stupid way to test if it starts with a number
		  value = 0.0;
		  udunitString = text;
		}

		DateUnit du;
		try
		{
		  du = new DateUnit(udunitString);
		}
		catch (Exception)
		{
		  return default(DateTime);
		}

		return du.makeDate(value);
	  }

	  /// <summary>
	  /// Create a NetcdfSharp.java.util.Date from a udunit or ISO String.
	  /// </summary>
	  /// <param name="text"> a udunit or ISO string. </param>
	  /// <returns> Date or null if not date unit. </returns>
	  /// <seealso cref=".getStandardDate"/>
	  /// <seealso cref="DateFormatter.getISODate"/>
	  public static DateTime getStandardOrISO(string text)
	  {
		DateTime result = getStandardDate(text);
		if (result == null)
		{
		  DateFormatter formatter = new DateFormatter();
		  result = formatter.getISODate(text);
		}
		return result;
	  }

	  public static CalendarDate parseCalendarDate(string text)
	  {
		DateTime result = getStandardDate(text);
		if (result == null)
		{
		  DateFormatter formatter = new DateFormatter();
		  result = formatter.getISODate(text);
		}
		return CalendarDate.of(result);
	  }

	  ////////////////////////////////////////////////////////////////////////
	  private double value;
	  private string udunitString;
	  private TimeUnit timeUnit;
	  private Unit uu;

	  public static DateUnit UnixDateUnit
	  {
		  get
		  {
			try
			{
			  return new DateUnit("s since 1970-01-01 00:00:00");
			}
			catch (Exception e)
			{
			  Console.WriteLine(e.ToString());
			  Console.Write(e.StackTrace);
			  return null;
			}
		  }
	  }

	  public static DateUnit factory(string text)
	  {
		try
		{
		  return new DateUnit(text);
		}
		catch (Exception)
		{
		  return null;
		}
	  }

	  /// <summary>
	  /// Constructor.
	  /// </summary>
	  /// <param name="text"> udunits String, eg 3 secs since 1991-01-01T03:12 </param>
	  /// <exception cref="UnitException"> if malformed String. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public DateUnit(String text) throws UnitException
	  public DateUnit(string text)
	  {
		string timeUnitString;

		text = text.Trim();
		StringTokenizer stoker = new StringTokenizer(text);
		string firstToke = stoker.nextToken();
		try
		{
		  this.value = double.Parse(firstToke);
		  // if (this.value == 0.0)
		  // this.value = 1.0;
		  this.udunitString = text.Substring(firstToke.Length); // eliminate the value if there is one
		  timeUnitString = stoker.nextToken();

		}
		catch (System.FormatException)
		{ // stupid way to test if it starts with a number
		  this.value = 0.0;
		  this.udunitString = text;
		  timeUnitString = firstToke;
		}

		uu = SimpleUnit.makeUnit(udunitString); // always a base unit
		timeUnit = new TimeUnit(timeUnitString);
	  }

	  /// <summary>
	  /// Constructor that takes a value, timeUnitString, and a Date since
	  /// </summary>
	  /// <param name="value"> number of time units </param>
	  /// <param name="timeUnitString"> eg "secs" </param>
	  /// <param name="since"> date since, eg "secs since 1970-01-01T00:00:00Z" </param>
	  /// <exception cref="UnitException"> if not valid time unit. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public DateUnit(double value, String timeUnitString, NetcdfSharp.java.util.Date since) throws UnitException
	  public DateUnit(double value, string timeUnitString, DateTime since)
	  {
		this.value = value;
		DateFormatter df = new DateFormatter();
		this.udunitString = timeUnitString + " since " + df.toDateTimeStringISO(since);
		uu = SimpleUnit.makeUnit(this.udunitString);
		timeUnit = new TimeUnit(timeUnitString);
	  }

	  /// <summary>
	  /// Get the origin Date.
	  /// </summary>
	  /// <returns> Date or null if not a time unit. </returns>
	  public virtual DateTime DateOrigin
	  {
		  get
		  {
			if (!(uu is TimeScaleUnit))
			{
			  return default(DateTime);
			}
			TimeScaleUnit tu = (TimeScaleUnit) uu;
			return tu.Origin;
		  }
	  }

	  /// <summary>
	  /// For udunit dates, get the time unit only, as a String, eg "secs" or "days"
	  /// </summary>
	  /// <returns> time unit as a string </returns>
	  public virtual string TimeUnitString
	  {
		  get
		  {
			return timeUnit.UnitString;
		  }
	  }

	  /// <summary>
	  /// For udunit dates, get the time unit.
	  /// </summary>
	  /// <returns> time unit </returns>
	  public virtual TimeUnit TimeUnit
	  {
		  get
		  {
			return timeUnit;
		  }
	  }

	  /// <summary>
	  /// Get the equivalent NetcdfSharp.java.util.Date.
	  /// </summary>
	  /// <returns> Date or null if failure </returns>
	  public virtual DateTime Date
	  {
		  get
		  {
			double secs = timeUnit.getValueInSeconds(value);
			return new DateTime(DateOrigin.Ticks + (long)(1000 * secs));
		  }
	  }

	  /// <summary>
	  /// Get the equivalent CalendarDate.
	  /// </summary>
	  /// <returns> CalendarDate or null if failure </returns>
	  public virtual CalendarDate makeCalendarDate(double val)
	  {
		return CalendarDate.of(makeDate(val));
	  }

	  /// <summary>
	  /// Create a Date from this base unit and the given value.
	  /// </summary>
	  /// <param name="val"> value in the units of this base unit, eg sec since base date </param>
	  /// <returns> Date . </returns>
	  public virtual DateTime makeDate(double val)
	  {
		if (double.IsNaN(val))
		{
		  return default(DateTime);
		}
		double secs = timeUnit.getValueInSeconds(val);
		return new DateTime(DateOrigin.Ticks + (long)(1000 * secs));
	  }

	  /// <summary>
	  /// Create the equivalent value from this base unit and the given Date.
	  /// Inverse of makeDate.
	  /// </summary>
	  /// <param name="date"> to convert. </param>
	  /// <returns> value in units of this base unit. </returns>
	  public virtual double makeValue(DateTime date)
	  {
		double secs = date.Ticks / 1000.0;
		double origin_secs = DateOrigin.Ticks / 1000.0;
		double diff = secs - origin_secs;

		try
		{
		  timeUnit.ValueInSeconds = diff;
		}
		catch (Exception e)
		{
		  throw new Exception(e.Message);
		}
		return timeUnit.Value;
	  }

	  /// <summary>
	  /// Make a standard GMT string representation from this unit and given value.
	  /// </summary>
	  /// <param name="value"> of time in these units. </param>
	  /// <returns> String or null if not time unit. </returns>
	  public virtual string makeStandardDateString(double value)
	  {
		DateTime date = makeDate(value);
		if (date == null)
		{
		  return null;
		}
		DateFormatter formatter = new DateFormatter();
		return formatter.toDateTimeStringISO(date);
	  }

	  public override string ToString()
	  {
		return value + " " + udunitString;
	  }

	  /// <summary>
	  /// The udunits string, but no value, ie its a base unit.
	  /// </summary>
	  /// <returns> the udunits base string </returns>
	  public virtual string UnitsString
	  {
		  get
		  {
			return udunitString;
		  }
	  }

	}

}
