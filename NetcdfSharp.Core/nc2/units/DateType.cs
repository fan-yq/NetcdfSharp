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
	using CalendarDateFormatter = NetcdfSharp.ucar.nc2.time.CalendarDateFormatter;
	using CalendarPeriod = NetcdfSharp.ucar.nc2.time.CalendarPeriod;

	/// <summary>
	/// Implements the thredds "dateType" and "dateTypeFormatted" XML element types.
	/// This is mostly a general way to specify dates in a string.
	/// It allows a date to mean "present". <strong>"Present" always sorts after any date, including dates in the
	/// future.</strong>
	/// It allows an optional attribute called "type" which is an enumeration like "created", "modified", etc
	/// taken from Dublin Core vocabulary.
	/// <p/>
	/// A DateType can be specified in the following ways:
	/// <ol>
	/// <li>an xsd:date, with form "CCYY-MM-DD"
	/// <li>an xsd:dateTime with form "CCYY-MM-DDThh:mm:ss"
	/// <li>a valid udunits date string
	/// <li>the string "present"
	/// </ol>
	/// 
	/// @author john caron </summary>
	/// <seealso cref="<a href=""https://www.unidata.ucar.edu/projects/THREDDS/tech/catalog/InvCatalogSpec.html.dateType"">THREDDS"
	///      dateType</a>/>

	public class DateType
	{
	  private string text, format, type;
	  private bool isPresent, isBlank;
	  private readonly CalendarDate date;

	  /// <summary>
	  /// Constructor using a NetcdfSharp.java.util.Date
	  /// </summary>
	  /// <param name="isPresent"> if true, this represents the "present time" </param>
	  /// <param name="date"> the given Date </param>
	  public DateType(bool isPresent, DateTime date)
	  {
		this.isPresent = isPresent;
		this.date = isPresent ? null : CalendarDate.of(date);
	  }

	  /// <summary>
	  /// Constructor using a NetcdfSharp.java.util.CalendarDate
	  /// </summary>
	  /// <param name="date"> the given CalendarDate </param>
	  public DateType(CalendarDate date)
	  {
		this.isPresent = false;
		this.date = date;
	  }

	  /// <summary>
	  /// no argument constructor for beans
	  /// </summary>
	  public DateType()
	  {
		isBlank = true;
		date = null;
	  }

	  /// <summary>
	  /// copy constructor
	  /// </summary>
	  /// <param name="src"> copy from here </param>
	  public DateType(DateType src)
	  {
		text = src.Text;
		format = src.Format;
		type = src.Type;
		isPresent = src.Present;
		isBlank = src.Blank;
		date = src.CalendarDate;
	  }

	  /// <summary>
	  /// Constructor.
	  /// </summary>
	  /// <param name="text"> string representation </param>
	  /// <param name="format"> using NetcdfSharp.java.text.SimpleDateFormat, or null </param>
	  /// <param name="type"> type of date, or null </param>
	  /// <exception cref="NetcdfSharp.java.text.ParseException"> if error parsing text </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public DateType(String text, String format, String type) throws NetcdfSharp.java.text.ParseException
	  public DateType(string text, string format, string type)
	  {

		text = (string.ReferenceEquals(text, null)) ? "" : text.Trim();
		this.text = text;
		this.format = format;
		this.type = type;

		// see if its blank
		if (text.Length == 0)
		{
		  isBlank = true;
		  date = null;
		  return;
		}

		// see if its the string "present"
		isPresent = text.Equals("present", StringComparison.OrdinalIgnoreCase);
		if (isPresent)
		{
		  this.date = null;
		  return;
		}

		// see if its got a format
		if (!string.ReferenceEquals(format, null))
		{
		  SimpleDateFormat dateFormat = new SimpleDateFormat(format);
		  DateTime d = dateFormat.parse(text);
		  date = CalendarDate.of(d);
		  return;
		}

		// see if its a udunits string
		if (text.IndexOf("since", StringComparison.Ordinal) > 0)
		{
		  date = CalendarDate.parseUdunits(null, text);
		  if (date == null)
		  {
			throw new NetcdfSharp.java.text.ParseException("invalid udunit date unit =" + text, 0);
		  }
		  return;
		}

		date = CalendarDate.parseISOformat(null, text);
		if (date == null)
		{
		  throw new NetcdfSharp.java.text.ParseException("invalid ISO date unit =" + text, 0);
		}
	  }


	  /// <summary>
	  /// Constructor.
	  /// </summary>
	  /// <param name="text"> string representation </param>
	  /// <param name="format"> using NetcdfSharp.java.text.SimpleDateFormat, or null </param>
	  /// <param name="type"> type of date, or null </param>
	  /// <param name="cal2"> NetcdfSharp.ucar.nc2.time.Calendar of date, or null </param>
	  /// <exception cref="NetcdfSharp.java.text.ParseException"> if error parsing text </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public DateType(String text, String format, String type, NetcdfSharp.ucar.nc2.time.Calendar cal2) throws NetcdfSharp.java.text.ParseException
	  public DateType(string text, string format, string type, NetcdfSharp.ucar.nc2.time.Calendar cal2)
	  {

		if (cal2 == null)
		{
		  cal2 = NetcdfSharp.ucar.nc2.time.Calendar.Default;
		}

		text = (string.ReferenceEquals(text, null)) ? "" : text.Trim();
		this.text = text;
		this.format = format;
		this.type = type;

		// see if its blank
		if (text.Length == 0)
		{
		  isBlank = true;
		  date = null;
		  return;
		}

		// see if its the string "present"
		isPresent = text.Equals("present", StringComparison.OrdinalIgnoreCase);
		if (isPresent)
		{
		  date = null;
		  return;
		}

		// see if its got a format
		if (!string.ReferenceEquals(format, null))
		{
		  SimpleDateFormat dateFormat = new SimpleDateFormat(format);
		  DateTime c = dateFormat.parse(text);
		  date = CalendarDate.of(cal2, c.Ticks / TimeSpan.TicksPerMillisecond);
		  return;
		}

		// see if its a udunits string
		string calName = (cal2 == null) ? null : cal2.ToString();
		if (text.IndexOf("since", StringComparison.Ordinal) > 0)
		{
		  date = CalendarDate.parseUdunits(calName, text);
		  if (date == null)
		  {
			throw new NetcdfSharp.java.text.ParseException("invalid udunit date unit =" + text, 0);
		  }
		  return;
		}

		date = CalendarDate.parseISOformat(calName, text);
		if (date == null)
		{
		  throw new NetcdfSharp.java.text.ParseException("invalid ISO date unit =" + text, 0);
		}
	  }


	  /// <summary>
	  /// Get this as a Date.
	  /// Does not handle non-standard Calendars.
	  /// </summary>
	  /// @deprecated use getCalendarDate() 
	  /// <returns> Date </returns>
	  public virtual DateTime Date
	  {
		  get
		  {
			return Present ? DateTime.Now : date.toDate();
		  }
	  }

	  public virtual DateTime getDate() => Date;

	  /// <summary>
	  /// Get this as a CalendarDate
	  /// </summary>
	  /// <returns> CalendarDate </returns>
	  public virtual CalendarDate CalendarDate
	  {
		  get
		  {
			return Present ? CalendarDate.present() : date;
		  }
	  }

	  /// <summary>
	  /// Does this represent the present time.
	  /// </summary>
	  /// <returns> true if present time. </returns>
	  public virtual bool Present
	  {
		  get
		  {
			return isPresent;
		  }
	  }

	  /// <summary>
	  /// Was blank text passed to the constructor.
	  /// </summary>
	  /// <returns> true if blank text passed to the constructor. </returns>
	  public virtual bool Blank
	  {
		  get
		  {
			return isBlank;
		  }
	  }

	  /// <summary>
	  /// Get a text representation.
	  /// </summary>
	  /// <returns> text representation </returns>
	  public virtual string Text
	  {
		  get
		  {
			if (isPresent)
			{
			  text = "present";
			}
			if (string.ReferenceEquals(text, null))
			{
			  text = toDateTimeString();
			}
			return text;
		  }
	  }

	  /// <summary>
	  /// Get the SimpleDateFormat format for parsing the text.
	  /// </summary>
	  /// <returns> SimpleDateFormat format, or null </returns>
	  public virtual string Format
	  {
		  get
		  {
			return format;
		  }
	  }

	  /// <summary>
	  /// Get the type of Date.
	  /// </summary>
	  /// <returns> type of Date, or null </returns>
	  public virtual string Type
	  {
		  get
		  {
			return type;
		  }
	  }

	  /// <summary>
	  /// Set the type of Date.
	  /// </summary>
	  /// <param name="type"> type of Date </param>
	  public virtual DateType setType(string type)
	  {
		this.type = type;
		return this;
	  }

	  /// <summary>
	  /// Is this date before the given date. if isPresent, always false.
	  /// </summary>
	  /// <param name="d"> test against this date </param>
	  /// <returns> true if this date before the given date </returns>
	  public virtual bool before(DateTime d)
	  {
		if (Present)
		{
		  return false;
		}
		return date.isBefore(CalendarDate.of(d));
	  }

	  /// <summary>
	  /// Is this date before the given date. if d.isPresent, always true, else if this.isPresent, false.
	  /// </summary>
	  /// <param name="d"> test against this date </param>
	  /// <returns> true if this date before the given date </returns>
	  public virtual bool before(DateType d)
	  {
		if (d.Present)
		{
		  return true;
		}
		if (Present)
		{
		  return false;
		}
		return date.isBefore(d.CalendarDate);
	  }

	  /// <summary>
	  /// Is this date after the given date. if isPresent, always true.
	  /// </summary>
	  /// <param name="d"> test against this date </param>
	  /// <returns> true if this date after the given date </returns>
	  public virtual bool after(DateTime d)
	  {
		if (Present)
		{
		  return true;
		}
		return date.isAfter(CalendarDate.of(d));
	  }

	  /// <summary>
	  /// Same as DateFormatter.toDateOnlyString()
	  /// </summary>
	  /// <returns> formatted date </returns>
	  public virtual string toDateString()
	  {
		if (Present)
		{
		  return CalendarDateFormatter.toDateStringPresent();
		}
		else
		{
		  return CalendarDateFormatter.toDateString(date);
		}
	  }

	  /// <summary>
	  /// Same as CalendarDateFormatter.toDateTimeStringISO
	  /// </summary>
	  /// <returns> formatted date </returns>
	  public virtual string toDateTimeString()
	  {
		if (Present)
		{
		  return CalendarDateFormatter.toDateTimeStringISO(DateTime.Now);
		}
		else
		{
		  return CalendarDateFormatter.toDateTimeStringISO(date);
		}
	  }

	  /// <summary>
	  /// Get ISO formatted string
	  /// </summary>
	  /// <returns> ISO formatted date </returns>
	  public virtual string toDateTimeStringISO()
	  {
		return toDateTimeString();
	  }

	  /// <summary>
	  /// String representation
	  /// </summary>
	  /// <returns> getText() </returns>
	  public override string ToString()
	  {
		return Text;
	  }

	  public override int GetHashCode()
	  {
		if (Blank)
		{
		  return 0;
		}
		if (Present)
		{
		  return 1;
		}
		return Date.GetHashCode();
	  }

	  public override bool Equals(object o)
	  {
		if (this == o)
		{
		  return true;
		}
		if (!(o is DateType))
		{
		  return false;
		}
		DateType oo = (DateType) o;
		if (Present && oo.Present)
		{
		  return true;
		}
		if (Blank && oo.Blank)
		{
		  return true;
		}
		return oo.Date.Equals(Date);
	  }

	  // private NetcdfSharp.java.util.Calendar cal = null;

	  public virtual DateType add(TimeDuration d)
	  {
		return add(d.TimeUnit);
	  }

	  public virtual DateType add(TimeUnit d)
	  {
		CalendarDate useDate = CalendarDate;
		CalendarDate result = useDate.add((int) d.ValueInSeconds, CalendarPeriod.Field.Second);
		return new DateType(result);
	  }

	  public virtual DateType subtract(TimeDuration d)
	  {
		return subtract(d.TimeUnit);
	  }

	  public virtual DateType subtract(TimeUnit d)
	  {
		CalendarDate useDate = CalendarDate;
		CalendarDate result = useDate.add((int) -d.ValueInSeconds, CalendarPeriod.Field.Second);
		return new DateType(result);
	  }
	}


}
