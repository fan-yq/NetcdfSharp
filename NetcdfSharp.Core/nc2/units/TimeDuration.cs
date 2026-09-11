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
	using ConversionException = NetcdfSharp.ucar.units.ShimConversionException;

	/// <summary>
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
	/// @author john caron </summary>
	/// <seealso cref=""https://www.unidata.ucar.edu/projects/THREDDS/tech/catalog/InvCatalogSpec.html.durationType""/>


	public class TimeDuration
	{
	  private static readonly NetcdfSharp.org.slf4j.Logger log = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(TimeDuration));

	  /*
	   * private static TimeUnit secUnits;
	   * 
	   * static {
	   * try {
	   * secUnits = new TimeUnit("1 sec");
	   * } catch (Exception e) {
	   * e.printStackTrace();
	   * }
	   * }
	   */

	  private string text;
	  private TimeUnit timeUnit;
	  private bool isBlank;

	  private static bool debug;

	  private TimeDuration()
	  {
	  }

	  /// <summary>
	  /// Copy constructor.
	  /// </summary>
	  /// <param name="src"> copy this </param>
	  public TimeDuration(TimeDuration src)
	  {
		text = src.Text;
		timeUnit = new TimeUnit(src.TimeUnit);
		isBlank = src.Blank;
	  }

	  /// <summary>
	  /// Construct from a TimeUnit.
	  /// </summary>
	  /// <param name="timeUnit"> copy this </param>
	  public TimeDuration(TimeUnit timeUnit)
	  {
		this.timeUnit = timeUnit;
		this.text = timeUnit.ToString();
	  }

	  /// <summary>
	  /// Construct from 1) udunit time unit string, 2) xsd:duration syntax, 3) blank string.
	  /// </summary>
	  /// <param name="text"> parse this text. </param>
	  /// <exception cref="NetcdfSharp.java.text.ParseException"> if invalid text. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public TimeDuration(String text) throws NetcdfSharp.java.text.ParseException
	  public TimeDuration(string text)
	  {
		text = (string.ReferenceEquals(text, null)) ? "" : text.Trim();
		this.text = text;

		// see if its blank LOOK WTF ?
		if (text.Length == 0)
		{
		  isBlank = true;
		  try
		  {
			timeUnit = new TimeUnit("1 sec");
		  }
		  catch (Exception)
		  { // cant happen
		  }
		  return;
		}

		// see if its a udunits string
		try
		{
		  timeUnit = new TimeUnit(text);
		  if (debug)
		  {
			Console.WriteLine(" set time unit= " + timeUnit);
		  }

		}
		catch (Exception e)
		{
		  // see if its a xsd:duration
		  try
		  {
			long secs = ParseW3CDurationSeconds(text);
			timeUnit = new TimeUnit(secs + " secs");

		  }
		  catch (Exception)
		  {
			throw new NetcdfSharp.java.text.ParseException(e.Message, 0);
		  }

		}

	  }

	  /// <summary>
	  /// Parse an xsd:duration string ("PnYnMnDTnHnMnS") into a total number of seconds.
	  /// Uses fixed-length conversions (1 year = 365 days, 1 month = 30 days), matching
	  /// the original javax.xml.datatype usage against a 1900-01-01 epoch.
	  /// </summary>
	  private static long ParseW3CDurationSeconds(string text)
	  {
		if (string.IsNullOrEmpty(text) || text[0] != 'P')
		{
		  throw new FormatException("bad xsd duration: " + text);
		}
		double years = 0, months = 0, days = 0, hours = 0, minutes = 0, seconds = 0;
		string body = text.Substring(1);
		int tIdx = body.IndexOf('T');
		string datePart = tIdx >= 0 ? body.Substring(0, tIdx) : body;
		string timePart = tIdx >= 0 ? body.Substring(tIdx + 1) : "";
		ParseDatePart(datePart, ref years, ref months, ref days);
		ParseTimePart(timePart, ref hours, ref minutes, ref seconds);
		// fixed-length: year = 365d, month = 30d
		long totalSecs = (long)((years * 365 + months * 30 + days) * 24 * 3600 + hours * 3600 + minutes * 60 + seconds);
		return totalSecs;
	  }

	  private static void ParseDatePart(string part, ref double y, ref double mo, ref double d)
	  {
		if (string.IsNullOrEmpty(part)) return;
		var num = new System.Text.StringBuilder();
		for (int i = 0; i < part.Length; i++)
		{
		  char c = part[i];
		  if (char.IsDigit(c) || c == '.')
		  {
			num.Append(c);
		  }
		  else
		  {
			double v = num.Length == 0 ? 0 : double.Parse(num.ToString(), System.Globalization.CultureInfo.InvariantCulture);
			switch (c)
			{
			  case 'Y': y += v; break;
			  case 'M': mo += v; break;
			  case 'D': d += v; break;
			}
			num.Clear();
		  }
		}
	  }

	  private static void ParseTimePart(string part, ref double h, ref double mi, ref double s)
	  {
		if (string.IsNullOrEmpty(part)) return;
		var num = new System.Text.StringBuilder();
		for (int i = 0; i < part.Length; i++)
		{
		  char c = part[i];
		  if (char.IsDigit(c) || c == '.')
		  {
			num.Append(c);
		  }
		  else
		  {
			double v = num.Length == 0 ? 0 : double.Parse(num.ToString(), System.Globalization.CultureInfo.InvariantCulture);
			switch (c)
			{
			  case 'H': h += v; break;
			  case 'M': mi += v; break;
			  case 'S': s += v; break;
			}
			num.Clear();
		  }
		}
	  }

	  /// <summary>
	  /// A time span as defined in the W3C XML Schema 1.0 specification:
	  /// "PnYnMnDTnHnMnS, where nY represents the number of years, nM the number of months, nD the number of days,
	  /// 'T' is the date/time separator, nH the number of hours, nM the number of minutes and nS the number of seconds.
	  /// The number of seconds can include decimal digits to arbitrary precision."
	  /// </summary>
	  /// <param name="text"> parse this text, format PnYnMnDTnHnMnS </param>
	  /// <returns> TimeDuration </returns>
	  /// <exception cref="NetcdfSharp.java.text.ParseException"> when text is misformed </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static TimeDuration parseW3CDuration(String text) throws NetcdfSharp.java.text.ParseException
	  public static TimeDuration parseW3CDuration(string text)
	  {
		TimeDuration td = new TimeDuration();

		text = (string.ReferenceEquals(text, null)) ? "" : text.Trim();
		td.text = text;

		try
		{
		  long secs = ParseW3CDurationSeconds(text);
		  td.timeUnit = new TimeUnit(secs + " secs");
		}
		catch (Exception e)
		{
		  throw new NetcdfSharp.java.text.ParseException(e.Message, 0);
		}
		return td;
	  }

	  /// <summary>
	  /// Get the duration in natural units, ie units of getTimeUnit()
	  /// </summary>
	  /// <returns> the duration in natural units </returns>
	  public virtual double Value
	  {
		  get
		  {
			return timeUnit.Value;
		  }
	  }

	  /// <summary>
	  /// Get the time duration in a specified unit of time.
	  /// </summary>
	  /// <param name="want"> in these units </param>
	  /// <returns> the duration in units </returns>
	  /// <exception cref="NetcdfSharp.ucar.units.ShimConversionException"> is specified unit is not compatible with time </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public double getValue(TimeUnit want) throws NetcdfSharp.ucar.units.ShimConversionException
	  public virtual double getValue(TimeUnit want)
	  {
		return timeUnit.convertTo(timeUnit.Value, want);
	  }

	  public virtual double getValue(string unit)
	  {
		try
		{
		  TimeUnit tdayUnit = new TimeUnit(unit);
		  return getValue(tdayUnit);

		}
		catch (Exception e)
		{
		  log.error("Illegal time unit = {}", unit, e);
		  return -1;
		}
	  }

	  /// <summary>
	  /// Get the duration in seconds
	  /// </summary>
	  /// <returns> the duration in seconds </returns>
	  public virtual double ValueInSeconds
	  {
		  get
		  {
			return timeUnit.ValueInSeconds;
		  }
		  set
		  {
			timeUnit.ValueInSeconds = value;
			text = null;
		  }
	  }


	  /// <summary>
	  /// If this is a blank string
	  /// </summary>
	  /// <returns> true if this is a blank string </returns>
	  public virtual bool Blank
	  {
		  get
		  {
			return isBlank;
		  }
	  }

	  /// <summary>
	  /// Get the corresponding time unit
	  /// </summary>
	  /// <returns> the corresponding time unit </returns>
	  public virtual TimeUnit TimeUnit
	  {
		  get
		  {
			return timeUnit;
		  }
	  }

	  /// <summary>
	  /// Get the String text
	  /// </summary>
	  /// <returns> the text </returns>
	  public virtual string Text
	  {
		  get
		  {
			return string.ReferenceEquals(text, null) ? timeUnit.ToString() : text;
		  }
	  }

	  /// <summary>
	  /// String representation
	  /// </summary>
	  /// <returns> getText() </returns>
	  public override string ToString()
	  {
		return Text;
	  }

	  /// <summary>
	  /// Override to be consistent with equals
	  /// </summary>
	  public override int GetHashCode()
	  {
		return Blank ? 0 : (int) ValueInSeconds;
	  }

	  /// <summary>
	  /// TimeDurations with same value in seconds are equals
	  /// </summary>
	  public override bool Equals(object o)
	  {
		if (this == o)
		{
		  return true;
		}
		if (!(o is TimeDuration))
		{
		  return false;
		}

		TimeDuration to = (TimeDuration) o;
		return to.ValueInSeconds == ValueInSeconds;
	  }

	  public virtual DateTime add(DateTime d)
	  {
		return timeUnit.add(d);
	  }
	}

}
